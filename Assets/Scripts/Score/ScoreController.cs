using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Bejeweled
{
    public class ScoreController : MonoBehaviour
    {
        int score;
        int highscore;

        public UnityEvent<int> onScoreChanged;
        public UnityEvent<int> onHighscoreChanged;

        void Start()
        {
            if (PlayerPrefs.HasKey("Highscore"))
            {
                highscore = PlayerPrefs.GetInt("Highscore");
                onHighscoreChanged?.Invoke(highscore);
            }
            else
            {
                PlayerPrefs.SetInt("Highscore", score);
            }
        }

        public void AddScore(Int2 scoreAndCombo)
        {
            score += scoreAndCombo.x * (scoreAndCombo.y);
            onScoreChanged?.Invoke(score);
        }

        public void ResetScore()
        {
            score = 0;
            onScoreChanged?.Invoke(score);
        }

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
