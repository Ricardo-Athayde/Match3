using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bejeweled
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] GameSettings gameSettings;

        [HideInInspector]
        public float gameTime;

        [HideInInspector]
        GameState gameState;

        public UnityEvent onStartPlaying;
        public UnityEvent onGameEnded;

        // Update is called once per frame
        void Update()
        {
            PassGametime();
        }

        public void StartGame()
        {
            gameState = GameState.Playing;
            gameTime = gameSettings.gameTime;
            onStartPlaying?.Invoke();
        }

        void EndGame()
        {
            gameState = GameState.End;
            onGameEnded?.Invoke();
        }

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
