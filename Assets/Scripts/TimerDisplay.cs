using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bejeweled
{
    public class TimerDisplay : MonoBehaviour
    {
        [SerializeField] GameSettings gameSettings;
        [SerializeField] GameManager gameManager;
        [SerializeField] Image timer;

        // Update is called once per frame
        void Update()
        {
            timer.fillAmount = gameManager.gameTime / gameSettings.gameTime;
        }
    }
}
