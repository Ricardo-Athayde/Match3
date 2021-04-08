using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bejeweled
{
    [CreateAssetMenu(fileName = "new Board Piece", menuName = "Bejeweled/Board Piece", order = 1)]
    public class BoardPiece : ScriptableObject
    {
        [Tooltip("Sprite Used to display this piece")]
        public Sprite sprite;

        [Tooltip("Color multiplier")]
        public Color color;
    }
}
