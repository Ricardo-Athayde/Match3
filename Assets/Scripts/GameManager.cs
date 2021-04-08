using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bejeweled
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game References")]
        [SerializeField] GameSettings gameSettings;

        [HideInInspector]
        public float gameTime { get; private set; }

        [HideInInspector]
        GameState gameState; //Current state of the game

        #region Events
        [Header("Events")]
        public UnityEvent onStartPlaying; //Called on Start Game
        public UnityEvent onGameEnded; //Called when there is no more time and the game ends
        #endregion

        // Update is called once per frame
        void Update()
        {
            PassGametime();
        }

        //Starts a new game
        public void StartGame()
        {
            gameState = GameState.Playing;
            gameTime = gameSettings.gameTime; //Resets the game time
            onStartPlaying?.Invoke();
        }

        //End the current game
        void EndGame()
        {
            gameState = GameState.End;
            onGameEnded?.Invoke();
        }

        //Decreases the game time and checks if the game has ended
        void PassGametime()
        {
            if(gameState == GameState.Playing)
            {
                gameTime -= Time.deltaTime;

                if(gameTime <= 0)
                {
                    EndGame();
                }
            }
        }
    }
}
