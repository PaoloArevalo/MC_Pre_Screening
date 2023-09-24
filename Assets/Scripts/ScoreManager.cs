using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private int playerScore = 0;
    [SerializeField] private int scorePerGem;
    [SerializeField] private float comboMultiplier;

    private void Awake()
    {
        Instance = this;
    }

    public void EvaluateScore(int gemsFall)
    {
        if (gemsFall < 5)
        {
            playerScore += scorePerGem * gemsFall;
        }
        else
        {
            playerScore += (scorePerGem * (int)(gemsFall - 5 * comboMultiplier) * gemsFall);
        }
        GameUI.Instance.UpdateScore(playerScore);
    }
    
}
