using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossFightController : Singleton<BossFightController>
{
    public float score;
    private GameObject bossObject;
    private Boss boss;
    private GameObject astronautObject;
    private HealthWatcher astronautHP;
    private Astronaut astronaut;
    private bool isReadyForReset;
    private bool isReadyToQuit;

    // Start is called before the first frame update
    void Start()
    {
        // place the player
        astronautObject = Instantiate(Resources.Load("astronaut"), new Vector3(0, 0, -15), Quaternion.identity) as GameObject;
        astronautObject.transform.SetParent(transform);
        astronaut = astronautObject.GetComponent<Astronaut>();

        astronaut.switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Gun);
        astronaut.switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Gun);

        //grab the healthwatcher
        astronautHP = GameObject.FindWithTag("HUD").GetComponent<HealthWatcher>();

        reset();
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.isGameOver)
        {
            if (boss.hp <= 0)
            {
                GameManager.isGameOver = true;
                showWinScreen();
            }
            else if (astronautHP.health <= 0)
            {
                GameManager.isGameOver = true;
                showLoseScreen();
            }
        }
        if (isReadyToQuit && OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            SceneManager.LoadSceneAsync("MainMenu");
        }
        else if (isReadyForReset && OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            reset();
        }
    }

    public void reset()
    {
        astronaut.reset();
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().sendNotification("fight!");

        isReadyToQuit = false;
        isReadyForReset = false;
        score = 0;

        astronautObject.transform.position = new Vector3(0, 0, -15);
        astronautObject.transform.rotation = Quaternion.identity;
        astronautObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        astronautObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        GameManager.isInputEnabled = true;
        GameManager.isGameOver = false;

        //spawn a random boss
        if (bossObject)
        {
            Destroy(bossObject);
        }
        Object[] bosses = Resources.LoadAll<Object>("Entities/Bosses");
        Object chosenBoss = bosses[UnityEngine.Random.Range(0, bosses.Length)];
        bossObject = Instantiate(chosenBoss, Vector3.zero, Quaternion.identity) as GameObject;
        boss = bossObject.GetComponent<Boss>();

        //give the player a moment to assess before we start attacking
        boss.waitToWake();

        resetLights();

        return;
    }

    private void sendEndgameNote(string resultNote)
    {
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().pinNote(resultNote + "\nPress Y to restart\n");
        isReadyForReset = true;
        return;
    }
    private void sendEndgameAdventureNote(string resultNote)
    {
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().pinNote(resultNote + "\nPress Y to quit\n");
        isReadyToQuit = true;
        return;
    }

    public void showWinScreen()
    {
        string report = GameObject.FindWithTag("HUD").GetComponent<ScoreManager>().generateBossFightReport();
        GameManager.scoreMemory += GameObject.FindWithTag("HUD").GetComponent<ScoreManager>().score;
        GameManager.timeMemory  += GameObject.FindWithTag("HUD").GetComponent<GameClock>().timeElapsed;

        if (GameManager.isAdventureMode)
        {
            sendEndgameAdventureNote("Chapter 3 Report:\n" + report + "\nTotal score: " + GameManager.scoreMemory.ToString() + "\nEnter your name.");
            GameManager.showKeyboard();

            astronaut.enableRightLineRenderer();
            astronaut.enableLeftLineRenderer();

            GameObject.FindWithTag("Player").transform.position = Vector3.zero;
        }
        else
        {
            sendEndgameNote("Boss Fight Report:\n" + report + "\nEnter your name.");
            var kb = Resources.Load("Keyboard");
            GameObject keyboard = Instantiate(kb, Vector3.zero, Quaternion.identity) as GameObject;
            keyboard.transform.SetParent(GameObject.FindWithTag("Player").transform);
            keyboard.transform.localPosition = new Vector3(0, -200, 200);
            keyboard.transform.localRotation = Quaternion.Euler(30, 0, 0);
            keyboard.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            astronaut.enableRightLineRenderer();
            astronaut.enableLeftLineRenderer();
        }
        return;
    }
    public void showLoseScreen()
    {
        // redden the lights
        var lights = GameObject.FindGameObjectsWithTag("Lights");
        foreach (GameObject light in lights)
        {
            var thisLight = light.GetComponent<Light>();
            if (thisLight != null)
            {
                thisLight.color = Color.red;
            }
        }
        var headlights = GameObject.FindGameObjectsWithTag("Headlights");
        foreach (GameObject light in headlights)
        {
            var thisLight = light.GetComponent<Light>();
            if (thisLight != null)
            {
                thisLight.color = Color.red;
            }
        }

        GameManager.isInputEnabled = false;

        string killedByString = "You were killed by " + boss.name;
        sendEndgameNote(killedByString);

        return;
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
            }
            if (!GameManager.isDarkMode)
            {
                light.SetActive(false);
            }
        }
    }
}
