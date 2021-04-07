using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bejeweled
{
    [CreateAssetMenu(fileName = "new Board Settings", menuName = "Bejeweled/Board Settings", order = 0)]
    public class BoardSettings : ScriptableObject
    {
        [Header("Board Dimentions")]  

        [Tooltip("How many pieces the board has horizontally")]
        public int boardWidth;

        [Tooltip("How many pieces the board has vertically")]
        public int boardHeight;

        [Space(10f)]
        [Header("Pieces")]

        [Tooltip("List of pieces that will appear on this board")]
        public BoardPiece[] boardPieces;

        [Tooltip("Size of the pieces on screen")]
        public float pieceSize;

        [Tooltip("Spacing between pieces")]
        public float pieceSpacing;
    }
}
