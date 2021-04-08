using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bejeweled
{
    public class BoardDisplay : MonoBehaviour
    {
        #region Public Variables
        [Header("Prefabs")]
        [SerializeField] GameObject boardGamePiece;

        [Header("References")]
        [SerializeField] BoardController boardController;
        [SerializeField] BoardSettings boardSettings;
        [SerializeField] GameSettings gameSettings;
        #endregion

        #region Private Variables
        Image[,] boardDisplaySprites; //Image component of the pieces
        RectTransform[,] boardDisplayRects; //Rect Transform Component of the pieces

        Int2 selection;
        int numberSelected = 0;
        #endregion

        //Update board display
        public void UpdateBoard()
        {
            //Initializes the boardDisplaySprites and array boardDisplayRects if null
            if (boardDisplaySprites == null) 
            { 
                boardDisplaySprites = new Image[boardSettings.boardWidth, boardSettings.boardHeight];
                boardDisplayRects = new RectTransform[boardSettings.boardWidth, boardSettings.boardHeight];
            }

            
            for (int i = 0; i < boardSettings.boardWidth; i++) //Col
            {
                for (int j = 0; j < boardSettings.boardHeight; j++) //Row
                {
                    if(boardDisplaySprites[i,j] == null) //Piece not created yet
                    {
                        CreateBoardPiece(new Int2(i, j));
                    }
                    if(boardController.curBoard[i, j] >= 0) //If not an empty space
                    {
                        BoardPiece thisBoardPiece = boardController.boardSettings.boardPieces[boardController.curBoard[i, j]];
                        boardDisplaySprites[i, j].sprite = thisBoardPiece.sprite;
                        boardDisplaySprites[i, j].color = thisBoardPiece.color;
                    }
                    else //Empty space
                    {
                        boardDisplaySprites[i, j].sprite = null;
                        boardDisplaySprites[i, j].color = new Color(0,0,0,0);
                    }                    
                }
            }
        }

        //Instantiate new board pieces, sets them up and save the needed references
        void CreateBoardPiece(Int2 pos)
        {
            boardDisplaySprites[pos.x, pos.y] = Instantiate(boardGamePiece, transform).GetComponent<Image>();
            boardDisplaySprites[pos.x, pos.y].gameObject.name = pos.x.ToString() + "/" + pos.y.ToString();

            //Sets the correct position and size
            RectTransform pieceRect;
            boardDisplayRects[pos.x,pos.y] = pieceRect = boardDisplaySprites[pos.x, pos.y].GetComponent<RectTransform>();
            pieceRect.anchoredPosition = GetDisplayPosition(pos);
            pieceRect.sizeDelta = new Vector2(boardSettings.pieceSize, boardSettings.pieceSize);

            //Sets the click behaviour
            pieceRect.GetComponent<Button>().onClick.AddListener(delegate { Select(pos); });

            //Sets the dragging component
            PieceDrag pieceDrag = pieceRect.GetComponent<PieceDrag>();
            pieceDrag.onPieceMoved.AddListener(boardController.ChangePieces);
            pieceDrag.onPieceMoved.AddListener(RemoveSelection);
            pieceDrag.boardPos = pos;
            pieceDrag.originalPos = pieceRect.anchoredPosition;
        }
        
        //Called when a piece is selected
        public void Select(Int2 pos)
        {            
            if(numberSelected == 0)
            {
                selection = pos;
                numberSelected++;
            }
            else if(numberSelected == 1)
            {
                boardController.ChangePieces(new PieceMovement(selection, pos));
                numberSelected = 0;
            }
        }

        //No piece is selected
        void RemoveSelection(PieceMovement movement)
        {
            numberSelected = 0;
        }

        public void PieceAnimation(PieceMovement movement)
        {
            StartCoroutine(I_PieceAnimation(movement));
        }

        //Animate a piece from one position to another
        IEnumerator I_PieceAnimation(PieceMovement movement)
        {
            Vector2 posA = movement.Pos1.y < 0 ? GetDisplayPosition(new Int2(movement.Pos2.x, -1)) : GetDisplayPosition(movement.Pos1);
            Vector2 posB = GetDisplayPosition(movement.Pos2);
            
            boardDisplaySprites[movement.Pos2.x, movement.Pos2.y].sprite = boardSettings.boardPieces[movement.newId].sprite;
            boardDisplaySprites[movement.Pos2.x, movement.Pos2.y].color = boardSettings.boardPieces[movement.newId].color;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / gameSettings.gameDelay;
                if (t > 1) { t = 1; }
                boardDisplayRects[movement.Pos2.x, movement.Pos2.y].anchoredPosition = Vector2.Lerp(posA, posB, t);
                yield return null;
            }
            boardDisplayRects[movement.Pos2.x, movement.Pos2.y].anchoredPosition = posB;
        }

        //Empty the space of a piece
        public void RemovePiece(Int2 pos)
        {
            boardDisplaySprites[pos.x, pos.y].sprite = null;
            boardDisplaySprites[pos.x, pos.y].color = new Color(0, 0, 0, 0);
        }

        //Returns the correct display position of a position on the board
        Vector2 GetDisplayPosition(Int2 pos)
        {
            Vector2 displayPos = new Vector2(pos.x * (boardSettings.pieceSize + boardSettings.pieceSpacing) - (boardSettings.boardWidth * (boardSettings.pieceSize + boardSettings.pieceSpacing)) / 2,
                                            -pos.y * (boardSettings.pieceSize + boardSettings.pieceSpacing) + (boardSettings.boardHeight * (boardSettings.pieceSize + boardSettings.pieceSpacing)) / 2);

            return displayPos;
        }
    }
}
