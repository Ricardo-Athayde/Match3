using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Bejeweled
{
    //Handles clicking and dragging of the board pieces
    [RequireComponent(typeof(RectTransform))]
    public class PieceDrag : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        #region Public Variables
        //Board position of this piece
        [HideInInspector]
        public Int2 boardPos; 

        //Invoked when the piece is dragged beyond a threshold
        [HideInInspector]
        public UnityEvent<PieceMovement> onPieceMoved;

        //Original position of this piece
        [HideInInspector]
        public Vector2 originalPos;
        #endregion

        #region Private Variables
        RectTransform rctTrans; //Rect Transform of this piece
        Canvas canvas; //Parent Canvas
       
        Vector2 dragPos; //Position piece should be in while dragging
        #endregion

        private void Awake()
        {
            rctTrans = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            if(!canvas) 
            {
                Debug.LogError("No Canvas found on parent object! Pieces must be inside a canvas to work. Destroying Piece.");
                Destroy(transform.gameObject);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {            
            dragPos = originalPos; //Resets the dragPosition
        }

        public void OnDrag(PointerEventData eventData)
        {
            //Updates the dragPos to reflect the dragging movement
            dragPos += (eventData.delta / canvas.scaleFactor);

            //Check if dragging if far enough to trigger onPieceMoved
            if (Vector2.Distance(originalPos, dragPos) > rctTrans.sizeDelta.x)
            {
                eventData.pointerDrag = null; //Stops the dragging
                EndDrag();
            }
        }

        public void EndDrag()
        {
            //Checks what direction the player tried to drag the piece (Left/Right/Up/Down)
            Int2 movementDirection;
            Vector2 movementVector = dragPos - originalPos;

            if (Mathf.Abs(movementVector.x) > Mathf.Abs(movementVector.y)) //Lef-Right
            {
                movementDirection = movementVector.x > 0 ? new Int2(boardPos.x + 1, boardPos.y) : new Int2(boardPos.x -1, boardPos.y);
            }
            else //Up-Down
            {
                movementDirection = movementVector.y > 0 ? new Int2(boardPos.x, boardPos.y - 1) : new Int2(boardPos.x, boardPos.y + 1);
            }

            //Calls the onPieceMoved passing the board position of this piece and the board position of the piece we moved towards 
            onPieceMoved?.Invoke(new PieceMovement(boardPos, movementDirection));

            //Resets the piece
            rctTrans.anchoredPosition = originalPos;
            dragPos = originalPos;
        }
    }
}