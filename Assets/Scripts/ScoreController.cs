using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    [SerializeField] Text scoreText;

    public static ScoreController instance;
    float score;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        
    }

    public void AddScore(int points, int combo)
    {
        score += points * (combo);
        scoreText.text = "Score: " + Mathf.FloorToInt(score);
    }
}
