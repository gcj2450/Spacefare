using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;


public class GameBuilder : Singleton<GameBuilder>
{
    // (Optional) Prevent non-singleton constructor use.
    protected GameBuilder() { }

    private UnityEngine.Object astronautPrefab;
    private Astronaut astronaut;
    private GameObject healthbar;

    private bool isWaitingToChangeScene;

    public enum CauseOfDeath
    {
        Timeout,
        Killed,
        Suffocated
    }

    private int size;
    private bool isReadyForReset = false;
    void Awake()
    {
        astronautPrefab = Resources.Load("astronaut");
        isWaitingToChangeScene = false;

        // ensure size is odd
        size = (GameManager.mapFuel + 4) * 2 + 1;
    }

    void Start()
    {
        initRandom();

        // build the map
        if (GameManager.isTightSpaces)
        {
            PathBuilder2.Instance.buildMap(size, GameManager.mapFuel);
        }
        else
        {
            PathBuilderLevel2.Instance.buildMap(size, GameManager.mapFuel);
        }

        // place the player
        astronaut = (Instantiate(astronautPrefab, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<Astronaut>();
        astronaut.transform.SetParent(transform);
        astronaut.switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Hand);
        astronaut.switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Gun);

        if (healthbar == null)
        {
            healthbar = GameObject.FindWithTag("Health");
        }
        if(!GameManager.isOxygenMode)
        {
            GameObject.FindWithTag("LeftHand").SetActive(false);
        }
        reset();
    }

    void Update()
    {
        if(isWaitingToChangeScene && OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            isWaitingToChangeScene = false;
            GameManager.gameMode = GameManager.GameMode.TimeTrial;
            SceneManager.LoadSceneAsync("TimeTrials");
        }
        else if (isReadyForReset && OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            isReadyForReset = false;
            reset();
        }
        else if (astronaut.getHealth() <= 0 && !GameManager.isGameOver)
        {
            GameManager.isGameOver = true;
            showLoseScreen(GameBuilder.CauseOfDeath.Killed);
        }
    }

    public void reset()
    {
        astronaut.reset();

        if (GameManager.isHardcoreMode)
        {
            GameObject cleanerObj = GameObject.FindWithTag("Cleaner");
            if (cleanerObj)
            {
                Destroy(cleanerObj);
            }
            GameObject monitorObj = GameObject.FindWithTag("Monitor");
            if (monitorObj)
            {
                Destroy(monitorObj);
            }

            var theCleaner = Resources.Load("Entities/Cleaner");
            cleanerObj = Instantiate(theCleaner, Vector3.zero, Quaternion.identity) as GameObject;
            cleanerObj.transform.Find("Body").GetComponent<Cleaner>().execute();
            cleanerObj.transform.SetParent(transform);

            var theMonitor = Resources.Load("Entities/Monitor");
            monitorObj = Instantiate(theMonitor, Vector3.zero, Quaternion.identity) as GameObject;
            monitorObj.transform.Find("Body").GetComponent<Monitor>().execute();
            monitorObj.transform.SetParent(transform);
        }
        else
        {
            if (healthbar)
            {
                healthbar.SetActive(false);
            }
        }

        GameManager.isGameOver = false;

        // enable input
        enableInput();

        // give initial instruction
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().sendNotification("hit the targets");


        // reset the player position, velocity, and angular velocity
        var player = GameObject.FindWithTag("Player");
        Vector3 startLocation;
        Quaternion startRot;
        if (GameManager.isTightSpaces)
        {
            startLocation = new Vector3(((size + 1) / 2) * 20, ((size + 1) / 2) * 20, ((size + 1) / 2) * 20);
            startRot = Quaternion.Euler(0, 90, 180);
        }
        else
        {
            startLocation = new Vector3(((size + 1) / 2) * 60, ((size + 1) / 2) * 60, ((size + 1) / 2) * 60);
            startRot = Quaternion.Euler(0, 0, 0);
        }
        player.transform.position = startLocation;
        player.transform.rotation = startRot;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject target in targets)
        {
            Destroy(target);
        }
        GameObject[] mines = GameObject.FindGameObjectsWithTag("Mine");
        foreach (GameObject mine in mines)
        {
            Destroy(mine);
        }
        GameObject[] ammos = GameObject.FindGameObjectsWithTag("AmmoCapsule");
        foreach (GameObject ammo in ammos)
        {
            Destroy(ammo);
        }
        GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
        foreach (GameObject key in keys)
        {
            Destroy(key);
        }
        GameObject[] gates = GameObject.FindGameObjectsWithTag("Gate");
        foreach (GameObject gate in gates)
        {
            Destroy(gate);
        }
        GameObject[] anchors = GameObject.FindGameObjectsWithTag("Anchor");
        foreach (GameObject anchor in anchors)
        {
            Destroy(anchor);
        }
        GameObject[] posters = GameObject.FindGameObjectsWithTag("Poster");
        foreach (GameObject poster in posters)
        {
            Destroy(poster);
        }
        // reset the glowballs
        GameObject[] glowballs = GameObject.FindGameObjectsWithTag("GlowBullet");
        foreach (GameObject glowball in glowballs)
        {
            Destroy(glowball);
        }
        // reset the windows
        GameObject[] windows = GameObject.FindGameObjectsWithTag("WindowArray");
        foreach (GameObject window in windows)
        {
            Destroy(window);
        }

        // finally we crawl!
        if (GameManager.isTightSpaces)
        {
            initRandom();
            TargetPlanter.Instance.execute();
            if (GameManager.isHardcoreMode)
            {
                MinePlanter.Instance.execute();
            }
            AmmoPlanter.Instance.execute();
            AnchorPlanter.Instance.execute();
        }
        else
        {
            // crawl!
            initRandom();
            Crawler.Instance.execute();
        }

        int numTargets = GameObject.FindWithTag("HUD").GetComponent<PointsWatcher>().totalPoints;
        GameObject.FindWithTag("Player").transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace").GetComponent<AmmoManager>().setGlowballAmmo(Mathf.RoundToInt(numTargets));

        resetLights();

        // reset the game length
        // 20 seconds per target
        GameManager.gameLength = (float)numTargets * 20;
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

    public void showLoseScreen(CauseOfDeath cause)
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

        disableInput();

        if (cause == CauseOfDeath.Killed)
        {
            string killedByString = "You were killed by " + GameManager.lastDamageDealer;
            sendEndgameNote(killedByString);
        }
        else if (cause == CauseOfDeath.Timeout)
        {
            sendEndgameNote("You timed out");
        }
        else if (cause == CauseOfDeath.Suffocated)
        {
            sendEndgameNote("You suffocated");
        }

        return;
    }
    private void sendEndgameNote(string resultNote)
    {
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().pinNote(resultNote + "\nPress Y to restart\n");
        isReadyForReset = true;
        return;
    }
    private void sendEndgameNoteAdventure(string resultNote)
    {
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().pinNote(resultNote + "\nPress Y to continue\n");
        isWaitingToChangeScene = true;
        return;
    }

    public void showWinScreen()
{
        string report = GameObject.FindWithTag("HUD").GetComponent<ScoreManager>().generateScoreReport();
        GameManager.scoreMemory = GameObject.FindWithTag("HUD").GetComponent<ScoreManager>().score;
        GameManager.timeMemory = GameObject.FindWithTag("HUD").GetComponent<GameClock>().timeElapsed;

        if (GameManager.isAdventureMode)
        {
            sendEndgameNoteAdventure("Chapter 1 Report:\n" + report);
        }
        else
        {
            sendEndgameNote("Search and Tag Report:\n" + report + "\nEnter your name.");
            GameManager.showKeyboard();

            astronaut.enableRightLineRenderer();
        }
        return;
    }

    
    public void enableInput()
    {
        GameManager.isInputEnabled = true;
        return;
    }

    public void disableInput()
    {
        GameManager.isInputEnabled = false;
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
                //light.SetActive(false);
            }
        }
    }

    public Vector3 getLevel2Loc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 60, room.Item2 * 60, room.Item3 * 60);
    }

    public Vector3 getLevel1Loc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 20, room.Item2 * 20, room.Item3 * 20);
    }
}
