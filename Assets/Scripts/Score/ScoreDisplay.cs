using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bejeweled
{
    public class ScoreDisplay : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] Text[] scoreTexts;
        [SerializeField] Text[] highscoreTexts;

        //Updates every score display text
        public void UpdateScoreDisplay(int score)
        {
            foreach (Text txt in scoreTexts)
            {
                txt.text = "Score: " + Mathf.FloorToInt(score);
            }            
        }

        //Updates every highscore display text
        public void UpdateHighscoreDisplay(int highscore)
        {
            foreach (Text txt in highscoreTexts)
            {
                txt.text = "Highscore: " + Mathf.FloorToInt(highscore);
            }
        }
    }
}
