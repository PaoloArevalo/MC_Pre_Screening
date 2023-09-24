using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance;
    [SerializeField] 
    private GameObject blackScreen;
    [SerializeField] 
    private TextMeshProUGUI timerUI;
    [SerializeField] 
    private TextMeshProUGUI scoreUI;

    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject pauseScreen;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowScreen(bool show)
    {
        if (show)
            LeanTween.alpha(blackScreen, 0f, .5f).setOnComplete(OnDisappear =>
            {
                blackScreen.SetActive(false);
            });
        else
        {
            blackScreen.SetActive(true);
            LeanTween.alpha(blackScreen, 1f, .5f);
        }
    }
    public void EndGame(bool won)
    {
        if (won)
        {
            LeanTween.scale(winScreen, Vector3.zero, 0);
            winScreen.SetActive(true);
            LeanTween.scale(winScreen, Vector3.one, 1f).setEaseInBounce();
        }
        else
        {
            LeanTween.scale(loseScreen, Vector3.zero, 0);
            loseScreen.SetActive(true);
            LeanTween.scale(loseScreen, Vector3.one, 1f).setEaseInBounce();
        }
    }
    
    public void ChangeTimerUI(float val, bool isDanger)
    {
        if(timerUI == null ||!timerUI.gameObject.activeSelf)
            return;
        if (val <= 0)
        {
            timerUI.text = "00:00";
            return;
        }
        float seconds = Mathf.FloorToInt(val % 60);
        float minutes = Mathf.FloorToInt(val / 60);
        if (isDanger)
            timerUI.color = Color.red;
        else
            timerUI.color = Color.white;
        timerUI.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    public void UpdateScore(int score)
    {
        scoreUI.text = score.ToString("000000000");
    }

    public void ShouldPause(bool pause)
    {
        if(!GameManager.instance.GameOnGoing)
            return;
        if (pause)
            pauseScreen.SetActive(true);
        else
        {
            pauseScreen.SetActive(false);
            GameManager.instance.GameIsPause = false;
        }
    }
    
}
