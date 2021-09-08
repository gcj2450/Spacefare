using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public static class GameManager
{
    public static string gameSeed { get; set; } = "";
    public static int mapFuel { get; set; } = 0;
    public static float mapDensity { get; set; } = 0.4f;
    public static bool isAdventureMode { get; set; } = false;
    public static bool isMusicMuted { get; set; } = false;

    public static UnityEngine.Random rand;
    public static float scoreMemory { get; set; } = 0.0f;
    public static float timeMemory { get; set; } = 0.0f;
    public static bool isNoBrakes { get; set; } = false;
    public static bool isDarkMode { get; set; } = false;
    public static bool isHardcoreMode { get; set; } = false;
    public static bool isOxygenMode { get; set; } = false;
    public static bool isTightSpaces { get; set; } = false;

    public static bool isGamePaused { get; set; } = false;
    public static bool isGameOver { get; set; } = false;
    public static bool isInputEnabled { get; set; } = true;
    public static string lastDamageDealer { get; set; }
    public static bool isTimerEnabled { get; set; } = false;
    public static float gameLength { get; set; }

    public enum GameMode
    {
        Menu,
        Search,
        TimeTrial,
        BossFight,
        FlightTraining,
        Tutorial,
        Debug
    }
    public static GameMode gameMode { get; set; }

    public static bool isRollEnabled { get; set; } = true;
    public static bool isPitchEnabled { get; set; } = true;
    public static bool isYawEnabled { get; set; } = true;
    public static bool isForthEnabled { get; set; } = true;
    public static bool isRightEnabled { get; set; } = true;
    public static bool isUpEnabled { get; set; } = true;


    public static void showKeyboard()
    {
        var kb = Resources.Load("Keyboard");
        GameObject keyboard = GameObject.Instantiate(kb, Vector3.zero, Quaternion.identity) as GameObject;
        keyboard.transform.SetParent(GameObject.FindWithTag("Player").transform);
        keyboard.transform.localPosition = new Vector3(0, -200, 200);
        keyboard.transform.localRotation = Quaternion.Euler(30, 0, 0);
        keyboard.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    public static void WriteToLeaderboards(string name, float time)
    {
        string path = Application.dataPath + "/leaderboards.txt";
        if(!File.Exists(path))
        {
            File.WriteAllText(path, "");
        }
        string entry = "";
        if (GameManager.isAdventureMode)
        {
            entry = "Adventure|" + GameManager.gameSeed + "|" + GameManager.mapFuel + "|" + name + "|" + timeMemory.ToString() + "|" + scoreMemory.ToString() + "\n";
        }
        else
        {
            float timeElapsed = GameObject.FindWithTag("HUD").GetComponent<GameClock>().timeElapsed;
            float score1 = GameObject.FindWithTag("HUD").GetComponent<ScoreManager>().score;
            float score2 = GameManager.scoreMemory;
            float score = (score1 > score2) ? score1 : score2;
            entry = GameManager.gameMode + "|" + GameManager.gameSeed + "|" + GameManager.mapFuel + "|" + name + "|" + timeElapsed.ToString() + "|" + score.ToString() + "\n";
        }
        File.AppendAllText(path, entry);
        return;
    }

}

