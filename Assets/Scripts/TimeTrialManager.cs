using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class TimeTrialManager : Singleton<TimeTrialManager>
{
    public int numRoomsTotal;
    public int numRoomsCompleted;
    private bool isReadyForReset;
    public float score;
    private GameObject astronautObj;
    private Astronaut astronaut;

    public bool isWaitingToChangeScene = false;

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        // place the player
        astronautObj = Instantiate(Resources.Load("astronaut"), Vector3.zero, Quaternion.Euler(0, 90, 0)) as GameObject;
        astronaut = astronautObj.GetComponent<Astronaut>();
        astronaut.reset();
        astronaut.switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Empty);
        astronaut.switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Empty);

        GameObject.FindWithTag("Points").SetActive(false);
        GameObject.FindWithTag("Health").SetActive(false);

        initRandom();
        numRoomsTotal = 0;
        PathBuilderTimeTrials.Instance.buildMap(101, (GameManager.mapFuel + 1) * 5);
        reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaitingToChangeScene && OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            GameManager.gameMode = GameManager.GameMode.BossFight;
            SceneManager.LoadSceneAsync("BossFight");
        }
        if (numRoomsTotal == 0)
        {
            numRoomsTotal = PathBuilderTimeTrials.Instance.numRooms;
            return;
        }
        if(numRoomsTotal > 0 && numRoomsCompleted >= numRoomsTotal)
        {
            GameManager.isGameOver = true;
            isReadyForReset = true;
            numRoomsCompleted = 0;
            showWinScreen();
        }

        if (isReadyForReset && OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            isReadyForReset = false;
            reset();
        }
    }

    private void initRandom()
    {
        if (GameManager.gameSeed == "")
        {
            UnityEngine.Random.InitState(Guid.NewGuid().GetHashCode());
        }
        else
        {
            UnityEngine.Random.InitState(Animator.StringToHash(GameManager.gameSeed));
        }
    }

    public void reset()
    {
        astronaut.reset();
        GameManager.isGameOver = false;
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().sendNotification("Race to the finish!");

        // reset the player position, velocity, and angular velocity
        Vector3 startLocation = PathBuilderTimeTrials.Instance.getLoc(PathBuilderTimeTrials.Instance.m_root);
        Quaternion startRot = Quaternion.Euler(0, 90, 0);
        astronaut.transform.position = startLocation;
        astronaut.transform.rotation = startRot;
        astronaut.GetComponent<Rigidbody>().velocity = Vector3.zero;
        astronaut.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // activate the rooms
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject room in rooms)
        {
            room.GetComponent<TTRoomManager>().isActivated = false;
        }

        resetLights();

        numRoomsCompleted = 0;
        isReadyForReset = false;

    }

    private void sendEndgameNote(string resultNote)
    {
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().pinNote(resultNote + "\nPress Y to restart\n");
        return;
    }
    private void sendEndgameNoteAdventure(string resultNote)
    {
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().pinNote(resultNote + "\nPress Y to go to the next chapter.\n");
        return;
    }
    public void showWinScreen()
    {
        string report = GameObject.FindWithTag("HUD").GetComponent<ScoreManager>().generateTimeTrialReport();

        if (GameManager.isAdventureMode)
        {
            sendEndgameNoteAdventure("Chapter 2 Report:\n" + report);
            GameManager.scoreMemory += GameObject.FindWithTag("HUD").GetComponent<ScoreManager>().score;
            GameManager.timeMemory  += GameObject.FindWithTag("HUD").GetComponent<GameClock>().timeElapsed;
            isWaitingToChangeScene = true;
        }
        else
        {
            sendEndgameNote("Time Trial Report:\n" + report + "\nEnter your name.");
            GameManager.showKeyboard();
            astronaut.enableRightLineRenderer();
        }
    }

    public void resetLights()
    {
        var lights = GameObject.FindGameObjectsWithTag("Lights");
        foreach (GameObject light in lights)
        {
            var thisLight = light.GetComponent<Light>();
            if (thisLight != null)
            {
                thisLight.color = Color.white;
            }
            if (GameManager.isDarkMode)
            {
                light.SetActive(false);
            }
        }

        var headlights = GameObject.FindGameObjectsWithTag("Headlights");
        foreach (GameObject light in headlights)
        {
            var thisLight = light.GetComponent<Light>();
            if (thisLight != null)
            {
                thisLight.color = Color.white;
                light.SetActive(true);
            }
            if (!GameManager.isDarkMode)
            {
                //light.SetActive(false);
            }
        }
    }
}
