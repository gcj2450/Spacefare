using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsWatcher : MonoBehaviour
{
    private int points;
    public int totalPoints;
    private int ringScore;

    // Start is called before the first frame update
    void Awake()
    {
        GameManager.isGameOver = false;
        points = 0;
        totalPoints = 0;
        ringScore = 0;
    }

    void Start()
    {
        if (GameManager.gameMode == GameManager.GameMode.FlightTraining)
        {
            updateRingScore();
        }
        else if(GameManager.gameMode == GameManager.GameMode.Search)
        {
            updateScore();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameMode == GameManager.GameMode.Search && points >= totalPoints && !GameManager.isGameOver)
        {
            GameManager.isGameOver = true;
            GameBuilder.Instance.showWinScreen();
        }
        else if (GameManager.gameMode == GameManager.GameMode.FlightTraining)
        {
            // do nothing
        }
    }

    public void addPoint()
    {
        // increment the points counter
        points++;
        updateScore();
    }

    public void addTarget()
    {
        totalPoints++;
        updateScore();
    }

    private void updateScore()
    {
        string newPointsText = points.ToString() + " / " + totalPoints.ToString();
        GameObject pointsCounter = GameObject.FindWithTag("Points");
        if (pointsCounter)
        {
            pointsCounter.GetComponent<UnityEngine.UI.Text>().text = newPointsText;
        }
    }

    public void reset()
    {
        GameManager.isGameOver = false;
        points = 0;
        totalPoints = 0;
        ringScore = 0;
        updateScore();
        updateRingScore();
    }

    public void addRingPoint()
    {
        ringScore++;
        updateRingScore();
    }

    private void updateRingScore()
    {
        GameObject pointsCounter = GameObject.FindWithTag("Points");
        if (pointsCounter)
        {
            pointsCounter.GetComponent<UnityEngine.UI.Text>().text = ringScore.ToString();
        }
    }
}
