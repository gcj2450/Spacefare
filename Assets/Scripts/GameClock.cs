using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameClock : MonoBehaviour
{
    public float startTime;
    private float FlightTestDuration = 60.0f;
    public float timeElapsed;
    void Awake()
    {
        timeElapsed = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.isTimerEnabled)
        {
            if (GameManager.gameMode == GameManager.GameMode.Search)
            {
                if(!GameManager.isGameOver)
                {
                    float timeSinceStart = (Time.time - startTime);
                    float thisTime = (GameManager.gameLength - timeSinceStart);
                    if (thisTime <= 0.0f)
                    {
                        GameManager.isGameOver = true;
                        GameBuilder.Instance.showLoseScreen(GameBuilder.CauseOfDeath.Timeout);
                    }
                    updateClock(thisTime);
                }
            }
            else if (GameManager.gameMode == GameManager.GameMode.FlightTraining)
            {
                if (GameManager.isGameOver)
                {
                    updateClock(0.0f);
                }
                else
                {
                    float thisTime = (FlightTestDuration - (Time.time - startTime));
                    if (thisTime <= 0.0f)
                    {
                        GameManager.isGameOver = true;
                        TrainingManager.Instance.showLoseScreen();
                        thisTime = 0.0f;
                    }
                    updateClock(thisTime);
                }
            }
        }
        else
        {
            if (!GameManager.isGameOver)
            {
                timeElapsed = Time.time - startTime;
                updateClock(timeElapsed);
            }
        }
    }

    private void updateClock(float thisTime)
    {
        GameObject gameClock = GameObject.FindWithTag("GameClock");
        if (gameClock != null)
        {
            TimeSpan thisSpan = TimeSpan.FromSeconds(thisTime);
            string thisTimeString = string.Format("{0:D2}:{1:D2}", thisSpan.Minutes, thisSpan.Seconds);
            gameClock.GetComponent<UnityEngine.UI.Text>().text = thisTimeString;
        }
    }

    public void reset()
    {
        startTime = Time.time;
    }
}
