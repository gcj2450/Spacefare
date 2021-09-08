using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenu;

    private TouchScreenKeyboard overlayKeyboard;
    public static string inputText = "";

    // game seeds to completion times
    public static Dictionary<string, List<string>> lBoards;

    private bool isShowingTimeLeaderboards;

    public enum LBMode
    {
        Adventure,
        Chapter1,
        Chapter2,
        Chapter3,
    }
    private LBMode lbMode;

    void Awake()
    {
        isShowingTimeLeaderboards = true;
        // broken fixed foveated rendering implementation
        //OVRManager.fixedFoveatedRenderingLevel = OVRManager.FixedFoveatedRenderingLevel.High;
        //OVRManager.useDynamicFixedFoveatedRendering = true;
        lbMode = LBMode.Adventure;
    }

    void Start()
    {
        GameManager.gameMode = GameManager.GameMode.Menu;
        GameManager.isAdventureMode = false;
    }

    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch) && OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            OVRManager.display.RecenterPose();
        }
        GameObject.FindWithTag("MainMenu").transform.Find("LeaderboardObjects/Leaderboard").GetComponent<Text>().text = ReadFromLeaderboards();
    }

    public void incrementFuel()
    {
        int thisFuel = int.Parse(mainMenu.transform.Find("Fuel/FuelValue").GetComponent<Text>().text);
        thisFuel++;
        mainMenu.transform.Find("Fuel/FuelValue").GetComponent<Text>().text = thisFuel.ToString();
    }
    public void decrementFuel()
    {
        int thisFuel = int.Parse(mainMenu.transform.Find("Fuel/FuelValue").GetComponent<Text>().text);
        if (thisFuel > 0)
        {
            thisFuel--;
        }
        mainMenu.transform.Find("Fuel/FuelValue").GetComponent<Text>().text = thisFuel.ToString();
    }

    public void startTutorial()
    {
        // load the GameBuilder
        GameManager.isNoBrakes = mainMenu.transform.Find("Toggles/BrakesToggle").GetComponent<Toggle>().isOn;
        GameManager.isDarkMode = mainMenu.transform.Find("Toggles/DarkModeToggle").GetComponent<Toggle>().isOn;
        GameManager.gameMode = GameManager.GameMode.Tutorial;
        SceneManager.LoadSceneAsync("Tutorial");
    }
    public void startFlightTraining()
    {
        // load the GameBuilder
        GameManager.isNoBrakes = mainMenu.transform.Find("Toggles/BrakesToggle").GetComponent<Toggle>().isOn;
        GameManager.isDarkMode = mainMenu.transform.Find("Toggles/DarkModeToggle").GetComponent<Toggle>().isOn;
        //GameManager.isTimerEnabled = mainMenu.transform.Find("TimerToggle").GetComponent<Toggle>().isOn;

        GameManager.gameMode = GameManager.GameMode.FlightTraining;
        SceneManager.LoadSceneAsync("FlightTraining");
    }
    public void startAdventure()
    {
        GameManager.isAdventureMode = true;
        GameManager.scoreMemory = 0;
        GameManager.timeMemory = 0;
        startSearch();
        return;
    }
    public void startSearch()
    {
        GameManager.gameMode = GameManager.GameMode.Search;

        mainMenu = GameObject.FindWithTag("MainMenu");
        GameManager.gameSeed = mainMenu.transform.Find("SeedInput/Placeholder").GetComponent<Text>().text;
        GameManager.mapFuel = int.Parse(mainMenu.transform.Find("Fuel/FuelValue").GetComponent<Text>().text);
        GameManager.mapDensity = mainMenu.transform.Find("MapDensitySlider/Slider").GetComponent<Slider>().value;
        GameManager.isNoBrakes = mainMenu.transform.Find("Toggles/BrakesToggle").GetComponent<Toggle>().isOn;
        GameManager.isDarkMode = mainMenu.transform.Find("Toggles/DarkModeToggle").GetComponent<Toggle>().isOn;
        GameManager.isOxygenMode = mainMenu.transform.Find("Toggles/OxygenToggle").GetComponent<Toggle>().isOn;
        GameManager.isHardcoreMode = mainMenu.transform.Find("Toggles/HardcoreModeToggle").GetComponent<Toggle>().isOn;
        GameManager.isTightSpaces = mainMenu.transform.Find("Toggles/TightSpacesToggle").GetComponent<Toggle>().isOn;


        if (GameManager.isTightSpaces)
        {
            SceneManager.LoadSceneAsync("Level1");
        }
        else
        {
            SceneManager.LoadSceneAsync("Level2");
        }
    }
    public void startTimeTrial()
    {
        mainMenu = GameObject.FindWithTag("MainMenu");
        GameManager.mapFuel = int.Parse(mainMenu.transform.Find("Fuel/FuelValue").GetComponent<Text>().text);
        GameManager.isNoBrakes = mainMenu.transform.Find("Toggles/BrakesToggle").GetComponent<Toggle>().isOn;
        GameManager.isDarkMode = mainMenu.transform.Find("Toggles/DarkModeToggle").GetComponent<Toggle>().isOn;
        GameManager.gameSeed = mainMenu.transform.Find("SeedInput/Placeholder").GetComponent<Text>().text;
        GameManager.gameMode = GameManager.GameMode.TimeTrial;

        // load the GameBuilder
        SceneManager.LoadSceneAsync("TimeTrials");
    }
    public void startBossFight()
    {
        // load the selections into the game manager
        mainMenu = GameObject.FindWithTag("MainMenu");
        GameManager.isNoBrakes = mainMenu.transform.Find("Toggles/BrakesToggle").GetComponent<Toggle>().isOn;
        GameManager.isDarkMode = mainMenu.transform.Find("Toggles/DarkModeToggle").GetComponent<Toggle>().isOn;
        GameManager.gameMode = GameManager.GameMode.BossFight;

        // load the GameBuilder
        SceneManager.LoadSceneAsync("BossFight");
    }

    public void switchModes()
    {
        switch(lbMode)
        {
            case LBMode.Adventure:
                lbMode = LBMode.Chapter1;
                GameObject.FindWithTag("MainMenu").transform.Find("LeaderboardObjects/ModeToggle/Text").gameObject.GetComponent<UnityEngine.UI.Text>().text = "Chapter1";
                break;
            case LBMode.Chapter1:
                lbMode = LBMode.Chapter2;
                GameObject.FindWithTag("MainMenu").transform.Find("LeaderboardObjects/ModeToggle/Text").gameObject.GetComponent<UnityEngine.UI.Text>().text = "Chapter2";
                break;
            case LBMode.Chapter2:
                lbMode = LBMode.Chapter3;
                GameObject.FindWithTag("MainMenu").transform.Find("LeaderboardObjects/ModeToggle/Text").gameObject.GetComponent<UnityEngine.UI.Text>().text = "Chapter3";
                break;
            case LBMode.Chapter3:
                lbMode = LBMode.Adventure;
                GameObject.FindWithTag("MainMenu").transform.Find("LeaderboardObjects/ModeToggle/Text").gameObject.GetComponent<UnityEngine.UI.Text>().text = "Adventure";
                break;
        }
    }

    public string ReadFromLeaderboards()
    {
        lBoards = new Dictionary<string, List<string>>();

        string gameMode = "";
        string seed = "";
        string fuel = "";
        string name = "";
        string time = "";
        string score = "";

        string[] lines = File.ReadAllLines(Application.dataPath + "/leaderboards.txt");
        foreach (string line in lines)
        {
            string[] values = line.Split('|');
            gameMode = values[0];
            switch(lbMode)
            {
                case LBMode.Adventure:
                    if (gameMode != "Adventure")
                    {
                        continue;
                    }
                    break;
                case LBMode.Chapter1:
                    if (gameMode != "Search")
                    {
                        continue;
                    }
                    break;
                case LBMode.Chapter2:
                    if (gameMode != "TimeTrial")
                    {
                        continue;
                    }
                    break;
                case LBMode.Chapter3:
                    if (gameMode != "BossFight")
                    {
                        continue;
                    }
                    break;
            }
            seed = values[1];
            fuel = values[2];
            name = values[3];
            time = values[4];
            score = values[5];

            List<string> currentEntries = new List<string> { };
            bool wasKeyAlreadyPresent = lBoards.TryGetValue(seed, out currentEntries);
            if (wasKeyAlreadyPresent)
            {
                bool throwaway = lBoards.Remove(seed);
            }
            else
            {
                currentEntries = new List<string> { };
            }
            currentEntries.Add(fuel + "|" + name + "|" + time + "|" + score + "|");
            lBoards.Add(seed, currentEntries);
        }

        List<(string, float, float)> playerTimeScores = new List<(string, float, float)> { };

        foreach (KeyValuePair<string, List<string>> kvp in lBoards)
        {
            if (kvp.Key == GameObject.FindWithTag("MainMenu").transform.Find("SeedInput/Placeholder").GetComponent<Text>().text)
            {
                foreach (string entry in kvp.Value)
                {
                    int mapFuel = getFuel(entry);
                    if (mapFuel == int.Parse(GameObject.FindWithTag("MainMenu").transform.Find("Fuel/FuelValue").GetComponent<Text>().text))
                    {
                        playerTimeScores.Add(getNameTimeScore(entry));
                    }
                }
            }
        }
        // sort playerTimeScores by the float value
        int compareTime((string, float, float) p1, (string, float, float) p2)
        {
            if (p1.Item2 < p2.Item2)
            {
                return -1;
            }
            else if (p1.Item2 == p2.Item2)
            {
                return 0;
            }
            else// if (p1.Item2 > p2.Item2)
            {
                return 1;
            }
        }
        int compareScore((string, float, float) p1, (string, float, float) p2)
        {
            if (p1.Item3 < p2.Item3)
            {
                return 1;
            }
            else if (p1.Item3 == p2.Item3)
            {
                return 0;
            }
            else// if (p1.Item3 > p2.Item3)
            {
                return -1;
            }
        }
        if (isShowingTimeLeaderboards)
        {
            playerTimeScores.Sort((p1, p2) => compareTime(p1, p2));
        }
        else
        {
            playerTimeScores.Sort((p1, p2) => compareScore(p1, p2));
        }
        string leaderboardData = "";

        leaderboardData += "(name, time, score)\n";
        foreach ((string, float, float) pTimeScore in playerTimeScores)
        {
            leaderboardData += pTimeScore;
            leaderboardData += "\n";
        }
        return leaderboardData;
    }
    private static int getFuel(string fuelNameTime)
    {
        string[] values = fuelNameTime.Split('|');
        string fuel = values[0];
        return int.Parse(fuel);
    }
    private static (string, float, float) getNameTimeScore(string fuelNameTimeScore)
    {
        string[] values = fuelNameTimeScore.Split('|');
        string fuel  = values[0];
        string name  = values[1];
        string time  = values[2];
        string score = values[3];
        float timeFloat  = float.Parse(time);
        float scoreFloat = float.Parse(score);
        return ((name, timeFloat, scoreFloat));

    }
    public void toggleTimeScore()
    {
        isShowingTimeLeaderboards = !isShowingTimeLeaderboards;
        if (isShowingTimeLeaderboards)
        {
            GameObject.FindWithTag("MainMenu").transform.Find("LeaderboardObjects/ToggleTimeScoreButton/Text").gameObject.GetComponent<UnityEngine.UI.Text>().text = "Sorted by Time";
        }
        else
        {
            GameObject.FindWithTag("MainMenu").transform.Find("LeaderboardObjects/ToggleTimeScoreButton/Text").gameObject.GetComponent<UnityEngine.UI.Text>().text = "Sorted by Score";
        }
    }

    public void exitSpacefare()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        // kill the app
        Application.Quit();
#endif
    }
}
