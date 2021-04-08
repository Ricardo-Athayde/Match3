using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bejeweled
{
    public class ScoreController : MonoBehaviour
    {
        #region Events    
        [Header("Events")]
        public UnityEvent<int> onScoreChanged;
        public UnityEvent<int> onHighscoreChanged;
        #endregion

        #region Private Variables
        int score; //Current score
        int highscore;
        #endregion

        void Start()
        {
            //If there is a saved highscore, set it
            if (PlayerPrefs.HasKey("Highscore"))
            {
                highscore = PlayerPrefs.GetInt("Highscore");
                onHighscoreChanged?.Invoke(highscore);
            }
        }

        //Increases the score based on a formula
        public void AddScore(Int2 scoreAndCombo)
        {
            score += scoreAndCombo.x * (scoreAndCombo.y);
            onScoreChanged?.Invoke(score);
        }

        //Resets the current score to 0
        public void ResetScore()
        {
            score = 0;
            onScoreChanged?.Invoke(score);
        }

        //Sets a new highscore if the current score is bigger the the previous highscore
        public void SaveHighScore()
        {
            if (highscore < score)
            {
                PlayerPrefs.SetInt("Highscore", score);
                onHighscoreChanged?.Invoke(score);
            }
        }
    }
}
