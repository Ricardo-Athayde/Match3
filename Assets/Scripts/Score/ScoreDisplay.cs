using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bejeweled
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] Text[] scoreTexts;
        [SerializeField] Text[] highscoreTexts;

        public void UpdateScoreDisplay(int score)
        {
            foreach (Text txt in scoreTexts)
            {
                txt.text = "Score: " + Mathf.FloorToInt(score);
            }            
        }
        public void UpdateHighscoreDisplay(int highscore)
        {
            foreach (Text txt in highscoreTexts)
            {
                txt.text = "Highscore: " + Mathf.FloorToInt(highscore);
            }
        }
    }
}
