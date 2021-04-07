using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bejeweled
{
    public class BoardDisplay : MonoBehaviour
    {
        [SerializeField] GameObject boardGamePiece;
        [SerializeField] BoardController boardController;
        Image[,] boardDisplaySprites;
        RectTransform[,] boardDisplayRects;

        Int2 selection;
        int numberSelected = 0;
        public void UpdateBoard()
        {
            Debug.Log("Board Updated");
            //Initializes the boardDisplaySprites array if null
            if (boardDisplaySprites == null) 
            { 
                boardDisplaySprites = new Image[boardController.boardSettings.boardWidth, boardController.boardSettings.boardHeight];
                boardDisplayRects = new RectTransform[boardController.boardSettings.boardWidth, boardController.boardSettings.boardHeight];
            }

            for (int i = 0; i < boardController.curBoard.GetLength(0); i++) //Col
            {
                for (int j = 0; j < boardController.curBoard.GetLength(1); j++) //Row
                {
                    if(boardDisplaySprites[i,j] == null) 
                    {
                        CreateBoardPiece(new Int2(i, j));
                    }
                    if(boardController.curBoard[i, j] >= 0)
                    {
                        BoardPiece thisBoardPiece = boardController.boardSettings.boardPieces[boardController.curBoard[i, j]];
                        boardDisplaySprites[i, j].sprite = thisBoardPiece.sprite;
                        boardDisplaySprites[i, j].color = thisBoardPiece.color;
                    }
                    else
                    {
                        boardDisplaySprites[i, j].sprite = null;
                        boardDisplaySprites[i, j].color = new Color(0,0,0,0);
                    }                    
                }
            }
        }

        void CreateBoardPiece(Int2 pos)
        {
            boardDisplaySprites[pos.x, pos.y] = Instantiate(boardGamePiece, transform).GetComponent<Image>();
            boardDisplaySprites[pos.x, pos.y].gameObject.name = pos.x.ToString() + "/" + pos.y.ToString();

            RectTransform pieceRect;
            boardDisplayRects[pos.x,pos.y] = pieceRect = boardDisplaySprites[pos.x, pos.y].GetComponent<RectTransform>();
            pieceRect.anchoredPosition = GetDisplayPosition(pos);
            pieceRect.sizeDelta = new Vector2(boardController.boardSettings.pieceSize, boardController.boardSettings.pieceSize);
            pieceRect.GetComponent<Button>().onClick.AddListener(delegate { Select(pos); });
            PieceDrag pieceDrag = pieceRect.GetComponent<PieceDrag>();
            pieceDrag.onPieceMoved.AddListener(boardController.ChangePieces);
            pieceDrag.boardPos = pos;
            pieceDrag.originalPos = pieceRect.anchoredPosition;

        }
        
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

        public void PieceAnimation(PieceMovement movement)
        {
            StartCoroutine(I_PieceAnimation(movement));
        }

        IEnumerator I_PieceAnimation(PieceMovement movement)
        {
            Vector2 posA = movement.Pos1.y < 0 ? GetDisplayPosition(new Int2(movement.Pos2.x, -1)) : GetDisplayPosition(movement.Pos1);
            Vector2 posB = GetDisplayPosition(movement.Pos2);
            
            boardDisplaySprites[movement.Pos2.x, movement.Pos2.y].sprite = boardController.boardSettings.boardPieces[movement.newId].sprite;
            boardDisplaySprites[movement.Pos2.x, movement.Pos2.y].color = boardController.boardSettings.boardPieces[movement.newId].color;

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / boardController.gameSettings.gameDelay;
                if (t > 1) { t = 1; }
                boardDisplayRects[movement.Pos2.x, movement.Pos2.y].anchoredPosition = Vector2.Lerp(posA, posB, t);
                yield return null;
            }
            boardDisplayRects[movement.Pos2.x, movement.Pos2.y].anchoredPosition = posB;
        }

        Vector2 GetDisplayPosition(Int2 pos)
        {
            BoardSettings settings = boardController.boardSettings;
            Vector2 displayPos = new Vector2(pos.x * (settings.pieceSize + settings.pieceSpacing) - (settings.boardWidth * (settings.pieceSize + settings.pieceSpacing)) / 2,
                                            -pos.y * (settings.pieceSize + settings.pieceSpacing) + (boardController.boardSettings.boardHeight * (settings.pieceSize + settings.pieceSpacing)) / 2);

            return displayPos;
        }

        void RemovePiece(Int2 pos)
        {
            boardDisplaySprites[pos.x, pos.y].sprite = null;
            boardDisplaySprites[pos.x, pos.y].color = new Color(0, 0, 0, 0);
        }

        private void OnEnable()
        {
            boardController.onBoardChanged.AddListener(UpdateBoard);
            boardController.onPieceMovement.AddListener(PieceAnimation);
            boardController.onPieceRemoved.AddListener(RemovePiece);
        }

        private void OnDisable()
        {
            boardController.onBoardChanged.RemoveListener(UpdateBoard);
            boardController.onPieceMovement.RemoveListener(PieceAnimation);
            boardController.onPieceRemoved.RemoveListener(RemovePiece);
        }
    }
}
