using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : Singleton<TrainingManager>
{
    private bool isReadyToReset;
    private Astronaut astronaut;

    // Start is called before the first frame update
    void Start()
    {
        // place the player
        var playerCharacter = Resources.Load("astronaut");
        
        astronaut = (Instantiate(playerCharacter, Vector3.zero, Quaternion.identity) as GameObject).GetComponent<Astronaut>();
        astronaut.switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Empty);
        astronaut.switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Empty);

        GameObject.FindWithTag("Health").SetActive(false);

        reset();

        scatterRing(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isReadyToReset && OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            reset();
        }
    }

    public void scatterRing(bool hasLifespan)
    {   
        var ring = Resources.Load("Crawler/Ring");
        Vector3 thisLocation = UnityEngine.Random.insideUnitSphere * 15;
        while(thisLocation.magnitude < 5)
        {
            thisLocation = UnityEngine.Random.insideUnitSphere * 15;
        }
        Quaternion thisRotation = Quaternion.Euler(UnityEngine.Random.value*360, UnityEngine.Random.value * 360, UnityEngine.Random.value * 360);

        Object ringClone = Instantiate(ring, thisLocation, thisRotation);

        // schedule a delete
        if(hasLifespan)
        {
            StartCoroutine(destroyRing(ringClone));
        }
    }

    IEnumerator destroyRing(Object thisRing)
    {
        Debug.Log("destroy ring");
        yield return new WaitForSeconds(10.0f);
        Destroy(thisRing);
    }

    public void showLoseScreen()
    {
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().pinNote("Game Over\nPress Y to restart");
        isReadyToReset = true;
        GameManager.isGameOver = true;
        return;
    }

    public void reset()
    {
        // manage reset flags
        isReadyToReset = false;
        GameManager.isGameOver = false;

        // reset points
        GameObject.FindWithTag("HUD").GetComponent<PointsWatcher>().reset();

        // reset clock
        GameObject.FindWithTag("HUD").GetComponent<GameClock>().reset();

        // reset astronaut
        GameObject.FindWithTag("Player").transform.position = new Vector3(0,0,-10);
        GameObject.FindWithTag("Player").transform.rotation = Quaternion.identity;
        GameObject.FindWithTag("Player").GetComponent<Rigidbody>().velocity = Vector3.zero;
        GameObject.FindWithTag("Player").GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        // give an initial instruction
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().reset();
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().sendNotification("Fly through the blue rings to earn points");

        // let the lights reflect dark mode
        resetLights();

        return;
    }

    public void resetLights()
    {
        var lights = GameObject.FindGameObjectsWithTag("Lights");
        foreach (GameObject light in lights)
        {
            if (GameManager.isDarkMode)
            {
                light.SetActive(false);
            }
        }

        var headlights = GameObject.FindGameObjectsWithTag("Headlights");
        foreach (GameObject light in headlights)
        {
            if (!GameManager.isDarkMode)
            {
                light.SetActive(false);
            }
            else
            {
                light.SetActive(true);
            }

        }
    }
}
