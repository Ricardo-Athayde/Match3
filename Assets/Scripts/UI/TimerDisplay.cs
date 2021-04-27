using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bejeweled
{
    public class TimerDisplay : MonoBehaviour
    {
        [Header("Game References")]
        [SerializeField] GameSettings gameSettings;
        [SerializeField] GameManager gameManager;

        [Header("UI References")]
        [SerializeField] Image timer;

        void Update()
        {
            timer.fillAmount = gameManager.gameTime / gameSettings.gameTime;
        }
    }
}
