using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bejeweled
{
    public enum GameState
    {
        Start,
        Playing,
        End
    }

    public struct Int2
    {
        public int x,y;
        public Int2(int X, int Y)
        {
            x = X;
            y = Y;
        }
    }

    //Used to pass the information that a piece moved prom one position to another, and if the piece changed its ID
    public struct PieceMovement
    {
        public Int2 Pos1, Pos2;
        public int newId;
        public PieceMovement(Int2 pos1 , Int2 pos2, int id)
        {
            Pos1 = pos1;
            Pos2 = pos2;
            newId = id;
        }
        public PieceMovement(Int2 pos1, Int2 pos2)
        {
            Pos1 = pos1;
            Pos2 = pos2;
            newId = -1;
        }
    }
}
