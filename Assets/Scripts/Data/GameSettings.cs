using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bejeweled
{
    [CreateAssetMenu(fileName = "new Game Settigns", menuName = "Bejeweled/Game Settings", order = 1)]
    public class GameSettings : ScriptableObject
    {
        [Tooltip("Controls animations and pacing of the game")]
        public float gameDelay;

        [Tooltip("Time the player has before the game ends")]
        public float gameTime;
    }
}
