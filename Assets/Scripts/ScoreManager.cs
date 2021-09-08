using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public float score;
    
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.gameMode == GameManager.GameMode.TimeTrial)
        {
            // add time bonus
            float timeElapsed = GameObject.FindWithTag("HUD").GetComponent<GameClock>().timeElapsed;
            int numRooms = 0;
            foreach (GameObject room in GameObject.FindGameObjectsWithTag("Room"))
            {
                numRooms++;
            }
            score = calculateTimeTrialBonus(numRooms, timeElapsed);
            updateScore();
        }
    }

    private void updateScore()
    {
        string newPointsText = score.ToString();
        GameObject pointsCounter = transform.Find("Canvas Layer 3/Canvas/ScoreCounter").gameObject;
        if (pointsCounter)
        {
            pointsCounter.GetComponent<UnityEngine.UI.Text>().text = newPointsText;
        }
    }

    public void reset()
    {
        score = 0;
        updateScore();
    }

    public void addScore(float gain)
    {
        score += gain;
        updateScore();
    }

    public string generateScoreReport()
    {
        string report = "Base score: " + score.ToString() + "\n";
        // add time bonus
        float timeElapsed = GameObject.FindWithTag("HUD").GetComponent<GameClock>().timeElapsed;
        int numRooms = 0;
        foreach (GameObject room in GameObject.FindGameObjectsWithTag("Room"))
        {
            numRooms++;
        }
        float timeBonus = calculateTimeBonus(numRooms, timeElapsed);
        score += timeBonus;
        report += "time bonus: " + timeBonus.ToString() +"\n";

        // add accuracy bonus
        float accuracyBonus = calculateAccuracyBonus(GameObject.FindWithTag("HUD").GetComponent<PointsWatcher>().totalPoints, GameObject.FindWithTag("Player").transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace").GetComponent<AmmoManager>().getShotsTaken());
        //float accuracyBonus = 1000 * ((float)GameObject.FindWithTag("HUD").GetComponent<PointsWatcher>().totalPoints / (float)GameObject.FindWithTag("Gun").GetComponent<GunMechanism>().shotsTaken);
        score += accuracyBonus;
        report += "accuracy bonus: " + accuracyBonus.ToString() + "\n";

        report += "adjusted score: " + score.ToString() + "\n";
        if (GameManager.isDarkMode)
        {
            score *= 1.25f;
            report += "after dark mode bonus: " + score.ToString() + "\n";

        }
        if (GameManager.isNoBrakes)
        {
            score *= 1.25f;
            report += "after no-brakes bonus: " + score.ToString() + "\n";
        }
        if (GameManager.isOxygenMode)
        {
            score *= 1.25f;
            report += "after oxygen mode bonus: " + score.ToString() + "\n";
        }
        if (GameManager.isTightSpaces)
        {
            score *= 1.50f;
            report += "after tight spaces bonus: " + score.ToString() + "\n";
        }
        if (GameManager.isHardcoreMode)
        {
            score *= 2.00f;
            report += "after hardcore bonus: " + score.ToString() + "\n";
        }
        return report;
    }
    private float calculateTimeBonus(int numRooms, float timeElapsed)
    {
        float parTime = (float)(numRooms * 20);
        if(GameManager.isTightSpaces)
        {
            parTime = parTime / 2;
        }

        if(timeElapsed < parTime)
        {
            return 1000.0f;
        }
        float deltaTime = timeElapsed - parTime;
        float trueBonus = 1000.0f - (deltaTime * 10.0f);
        if (trueBonus <= 0)
        {
            return 0;
        }
        return trueBonus;
    }
    private float calculateAccuracyBonus(int numTargets, int shotsTaken)
    {
        float parShots = (float)(numTargets * 2);
        if (shotsTaken < parShots)
        {
            return 1000.0f;
        }
        float deltaShots = shotsTaken - parShots;
        float trueBonus = 1000.0f - (deltaShots * 50.0f);
        if (trueBonus <= 0)
        {
            return 0;
        }
        return trueBonus;
    }

    public string generateTimeTrialReport()
    {
        string report = "Base score: " + score.ToString() + "\n";
        // add time bonus
        float timeElapsed = GameObject.FindWithTag("HUD").GetComponent<GameClock>().timeElapsed;
        int numRooms = 0;
        foreach (GameObject room in GameObject.FindGameObjectsWithTag("Room"))
        {
            numRooms++;
        }
        float timeBonus = calculateTimeTrialBonus(numRooms, timeElapsed);
        //score += timeBonus;
        //report += "time bonus: " + timeBonus.ToString() + "\n";

        //report += "adjusted score: " + score.ToString() + "\n";
        if (GameManager.isDarkMode)
        {
            score *= 1.50f;
            report += "after dark mode bonus: " + score.ToString() + "\n";

        }
        if (GameManager.isNoBrakes)
        {
            score *= 2.00f;
            report += "after no-brakes bonus: " + score.ToString() + "\n";
        }
        return report;
    }
    private float calculateTimeTrialBonus(int numRooms, float timeElapsed)
    {
        float parTime = (float)(numRooms * 1.0);
        if (timeElapsed < parTime)
        {
            return 10000.0f;
        }
        float deltaTime = timeElapsed - parTime;
        float trueBonus = 10000.0f - (deltaTime * 100.0f);
        if (trueBonus <= 0)
        {
            return 0;
        }
        return trueBonus;
    }

    public string generateBossFightReport()
    {
        string report = "Base score: " + score.ToString() + "\n";
        // add time bonus
        float timeElapsed = GameObject.FindWithTag("HUD").GetComponent<GameClock>().timeElapsed;
        float timeBonus = calculateBossFightTimeBonus(timeElapsed);
        score += timeBonus;
        report += "time bonus: " + timeBonus.ToString() + "\n";

        // add accuracy bonus
        float accuracyBonus = calculateBossFightAccuracyBonus(GameObject.FindWithTag("Boss").GetComponent<Boss>().maxHP, GameObject.FindWithTag("Player").transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace").GetComponent<AmmoManager>().getShotsTaken());
        score += accuracyBonus;
        report += "accuracy bonus: " + accuracyBonus.ToString() + "\n";

        report += "adjusted score: " + score.ToString() + "\n";
        if (GameManager.isDarkMode)
        {
            score *= 2.00f;
            report += "after dark mode bonus: " + score.ToString() + "\n";

        }
        if (GameManager.isNoBrakes)
        {
            score *= 2.00f;
            report += "after no-brakes bonus: " + score.ToString() + "\n";
        }
        return report;
    }
    private float calculateBossFightTimeBonus(float timeElapsed)
    {
        float parTime = (float)100;
        if (timeElapsed < parTime)
        {
            return 1000.0f;
        }
        float deltaTime = timeElapsed - parTime;
        float trueBonus = 1000.0f - (deltaTime * 10.0f);
        if (trueBonus <= 0)
        {
            return 0;
        }
        return trueBonus;
    }
    private float calculateBossFightAccuracyBonus(int numTargets, int shotsTaken)
    {
        float parShots = (float)(numTargets * 8);
        if (shotsTaken < parShots)
        {
            return 10000.0f;
        }
        float deltaShots = shotsTaken - parShots;
        float trueBonus = 10000.0f - (deltaShots * 500.0f);
        if (trueBonus <= 0)
        {
            return 0;
        }
        return trueBonus;
    }
}
