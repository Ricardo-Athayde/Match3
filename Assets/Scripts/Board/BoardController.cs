using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace Bejeweled
{
    public class BoardController : MonoBehaviour
    {
        #region Public Variables
        [Header("Game References")]
        public BoardSettings boardSettings;
        public GameSettings gameSettings;

        [HideInInspector]
        public int[,] curBoard { get; private set; } //Current game board

        [HideInInspector]
        public bool checkingMatches; //Checking if there are matches on the board
        public bool interactable;
        #endregion

        #region Events
        [Header("Events")]
        public UnityEvent onBoardChanged;
        public UnityEvent<PieceMovement> onPieceMovement; //Invoked whenever a piece moved from one place to another (Ex: dropping)
        public UnityEvent<Int2> onPieceRemoved; //Invoked whenever a piece is removed ftom the board.
        public UnityEvent<Int2> onPieceRemovedCombo; //Invoked whenever all matching pieces are removed from the board
        #endregion

        //Creates a new board
        public void CreateBoard()
        {
            if(curBoard == null) { curBoard = new int[boardSettings.boardWidth, boardSettings.boardHeight]; }

            if(boardSettings.boardPieces.Length <= 1) { Debug.LogError("Not enought avaliable pieces. There must be at least 2 pieces on the board settings."); return; }

            for (int i = 0; i < curBoard.GetLength(0); i++) //Col
            {
                for (int j = 0; j < curBoard.GetLength(1); j++) //Row
                {
                    //Makes sure there are no matches when creating a new board
                    List<int> avaliablePieces = new List<int>();

                    for (int k = 0; k < boardSettings.boardPieces.Length; k++)
                    {
                        avaliablePieces.Add(k); //Add all possible pieces to the pool of avaliable pieces
                    }

                    avaliablePieces.Remove(SafeCheckPieceID(new Int2(i, j-1))); //Removes the piece above the current position from the list
                    avaliablePieces.Remove(SafeCheckPieceID(new Int2(i-1, j))); //Removes the piece left of the current position from the list

                    //Places a "Random" piece on the board
                    curBoard[i, j] = avaliablePieces[Random.Range(0, avaliablePieces.Count)];
                }
            }
            onBoardChanged?.Invoke();
        }

        //Swap 2 pieces
        public void ChangePieces(PieceMovement movement)
        {
            //Can't modify board while still checking for matches and when it is not interactable            
            if (checkingMatches || !interactable) { return; }

            //Invalid board position
            if (movement.Pos1.x > boardSettings.boardWidth - 1 || movement.Pos1.x < 0 ||
                movement.Pos1.y > boardSettings.boardHeight - 1 || movement.Pos1.y < 0 ||
                movement.Pos2.x > boardSettings.boardWidth - 1 || movement.Pos2.x < 0 ||
                movement.Pos2.y > boardSettings.boardHeight - 1 || movement.Pos2.y < 0)
            {
                return;
            }

            //Can only change adjacent pieces
            Int2 distance = new Int2(Mathf.Abs(movement.Pos2.x - movement.Pos1.x), Mathf.Abs(movement.Pos2.y - movement.Pos1.y));
            if (distance.x > 1 || distance.y > 1 || (distance.x == 1 && distance.y == 1))
            {
                return;
            }

            StartCoroutine(I_BoardUpdate(movement));
        }

        //Change board pieces
        IEnumerator I_BoardUpdate(PieceMovement movement)
        { 
            checkingMatches = true;

            //Swaps the 2 pieces
            int pos1Id = curBoard[movement.Pos1.x, movement.Pos1.y];
            curBoard[movement.Pos1.x, movement.Pos1.y] = curBoard[movement.Pos2.x, movement.Pos2.y];
            curBoard[movement.Pos2.x, movement.Pos2.y] = pos1Id;
            onPieceMovement?.Invoke(new PieceMovement(movement.Pos1, movement.Pos2, curBoard[movement.Pos2.x, movement.Pos2.y]));
            onPieceMovement?.Invoke(new PieceMovement(movement.Pos2, movement.Pos1, curBoard[movement.Pos1.x, movement.Pos1.y]));

            yield return new WaitForSeconds(gameSettings.gameDelay); //Waits for the global delay

            //Start checking if there where any matches
            bool keepCheckingMatches = true;
            bool firstMovement = true;
            int comboCount = 0;

            //Columns that had changes, and the maximum Y position of those changes.
            //Used to check only the columns and, rows at or above those changes
            Dictionary<int, int> modifiedColumnsAndRow = new Dictionary<int, int>();
            AddModifiedColumns(ref modifiedColumnsAndRow, movement.Pos1);
            AddModifiedColumns(ref modifiedColumnsAndRow, movement.Pos2);

            while (keepCheckingMatches) //Loops until no matches are found on the board
            {
                //Check for matches
                Int2[] matchedPieces = CheckForMaches(ref modifiedColumnsAndRow);

                modifiedColumnsAndRow.Clear(); //Resets the dictionary for the next check

                //No matches found
                if (matchedPieces == null || matchedPieces.Length == 0)
                {
                    keepCheckingMatches = false;

                    //If this is the first movement, that means the player tried to change 2 pieces that didn't result in a match.
                    if (firstMovement)
                    {
                        //Place the pieces in their original positions
                        curBoard[movement.Pos2.x, movement.Pos2.y] = curBoard[movement.Pos1.x, movement.Pos1.y];
                        curBoard[movement.Pos1.x, movement.Pos1.y] = pos1Id;
                        onPieceMovement?.Invoke(new PieceMovement(movement.Pos1, movement.Pos2, curBoard[movement.Pos2.x, movement.Pos2.y]));
                        onPieceMovement?.Invoke(new PieceMovement(movement.Pos2, movement.Pos1, curBoard[movement.Pos1.x, movement.Pos1.y]));
                    }
                }

                //Found Matches
                else
                {
                    comboCount++;
                    firstMovement = false;

                    //Remove the matching pieces from the board. Checks how manny pieces where removed
                    int removedPiecesCount = 0;
                    for (int i = 0; i < matchedPieces.Length; i++)
                    {
                        if (curBoard[matchedPieces[i].x, matchedPieces[i].y] != -1) //If piece wasn't already removed
                        {
                            curBoard[matchedPieces[i].x, matchedPieces[i].y] = -1;
                            AddModifiedColumns(ref modifiedColumnsAndRow, matchedPieces[i]); 
                            onPieceRemoved?.Invoke(matchedPieces[i]);
                            removedPiecesCount++;
                        }
                    }

                    onPieceRemovedCombo?.Invoke(new Int2(removedPiecesCount, comboCount));

                    yield return new WaitForSeconds(gameSettings.gameDelay);  //Waits so the player can see the pieces removed

                    //Drops the pieces that have empty spaces below them and create new pieces on top of the board
                    DropPieces(ref modifiedColumnsAndRow);

                    yield return new WaitForSeconds(gameSettings.gameDelay);  //Waits so the player can see the pieces drop
                }
            }
            checkingMatches = false;
        }

        //Check if there are any matches on the modified columns and rows
        Int2[] CheckForMaches(ref Dictionary<int, int> modifiedColumnsAndRow)
        {
            List<Int2> piecesMatched = new List<Int2>();
            foreach (KeyValuePair<int, int> entry in modifiedColumnsAndRow) //For every column modified
            {
                for (int j = 0; j <= entry.Value; j++) //Moves down on the rows until the last modified
                {
                    Int2 curCheckPos;
                    int col = entry.Key;
                    int row = j;

                    #region Horizontal Check
                    List<Int2> horizontalMatch = new List<Int2>();

                    //Check Left
                    curCheckPos = new Int2(col - 1, row);
                    while (SafeCheckPieceID(curCheckPos) == curBoard[col, row])
                    {
                        horizontalMatch.Add(curCheckPos);
                        curCheckPos = new Int2(curCheckPos.x - 1, curCheckPos.y);
                    }

                    //Check Right
                    curCheckPos = new Int2(col + 1, row);
                    while (SafeCheckPieceID(curCheckPos) == curBoard[col, row])
                    {
                        horizontalMatch.Add(curCheckPos);
                        curCheckPos = new Int2(curCheckPos.x + 1, curCheckPos.y);
                    }

                    //If found 2 or more, then there is a match
                    if (horizontalMatch.Count >= 2)
                    {
                        piecesMatched.Add(new Int2(col, row));
                        foreach (Int2 position in horizontalMatch)
                        {
                            piecesMatched.Add(position);
                        }
                    }
                    #endregion

                    #region Vertical Check
                    List<Int2> verticalMatch = new List<Int2>();

                    //Check Up
                    curCheckPos = new Int2(col, row - 1);
                    while (SafeCheckPieceID(curCheckPos) == curBoard[col, row])
                    {
                        verticalMatch.Add(curCheckPos);
                        curCheckPos = new Int2(curCheckPos.x, curCheckPos.y - 1);
                    }

                    //Check Down
                    curCheckPos = new Int2(col, row + 1);
                    while (SafeCheckPieceID(curCheckPos) == curBoard[col, row])
                    {
                        verticalMatch.Add(curCheckPos);
                        curCheckPos = new Int2(curCheckPos.x, curCheckPos.y + 1);
                    }

                    //If found 2 or more, then there is a match
                    if (verticalMatch.Count >= 2)
                    {
                        piecesMatched.Add(new Int2(col, row));
                        foreach (Int2 position in verticalMatch)
                        {
                            piecesMatched.Add(position);
                        }
                    }
                    #endregion
                }
            }

            //If Found matching pieces
            if (piecesMatched.Count > 0)
            {
                return piecesMatched.ToArray();
            }

            //No matching pieces found
            else return null;
        }

        //Drops the pieces on the board
        void DropPieces(ref Dictionary<int, int> modifiedColumnsAndRow)
        {
            foreach (KeyValuePair<int, int> entry in modifiedColumnsAndRow) //For every column modified
            {
                for (int j = entry.Value; j >= 0 ; j--) //Moves up on the rows from the last modified
                {
                    int col = entry.Key;
                    int row = j;

                    //If the position is empty, check all pieces above. If a piece is found, put that piece in the position. 
                    if (curBoard[col, row] == -1) 
                    {
                        Int2 curCheckPos;

                        //Check Up
                        curCheckPos = new Int2(col, row - 1);
                        while (SafeCheckPieceID(curCheckPos) == -1)
                        {
                            curCheckPos = new Int2(curCheckPos.x, curCheckPos.y - 1);
                        }

                        //Found a piece above
                        if (SafeCheckPieceID(curCheckPos) != -2) 
                        {
                            curBoard[col, row] = curBoard[curCheckPos.x, curCheckPos.y];
                            curBoard[curCheckPos.x, curCheckPos.y] = -1;
                            onPieceMovement?.Invoke(new PieceMovement(curCheckPos, new Int2(col, row), curBoard[col, row]));
                        }

                        //No piece above, create new piece
                        else
                        {
                            curBoard[col, row] = Random.Range(0, boardSettings.boardPieces.Length);
                            onPieceMovement?.Invoke(new PieceMovement(curCheckPos, new Int2(col, row), curBoard[col, row]));
                        }
                    }
                }
            }
        }


        //Safely chacks the ID of a piece on a board.
        int SafeCheckPieceID(Int2 pos)
        {
            //If the position is outside of the board, return -2
            if (pos.x < 0 || pos.y < 0 || pos.x >= curBoard.GetLength(0) || pos.y >= curBoard.GetLength(1))
            {
                return -2;
            }
            else
            {
                return curBoard[pos.x, pos.y];
            }
        }

        public void SetInteractable(bool state)
        {
            interactable = state;
        }


        //Add columns and rows to the dictionary
        void AddModifiedColumns(ref Dictionary<int, int> dictionary, Int2 pos)
        {
            if (!dictionary.ContainsKey(pos.x)) //Column not added yet
            {
                dictionary.Add(pos.x, pos.y);
            }
            else //Column already added
            {
                if (pos.y > dictionary[pos.x]) //If the new row is higher
                {
                    dictionary[pos.x] = pos.y;
                }
            }
        }

    }
}
