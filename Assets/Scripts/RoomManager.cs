using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private GameObject lights;
    private bool lightState;
    private bool hasBeenVisited;

    void Awake()
    {
        lights = transform.Find("LightArray").gameObject;
        hasBeenVisited = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        // turn off the lights unless we're the starting room
        lightsOff();

        (int, int, int) thisLocation = (int.Parse(transform.position.x.ToString()) / 60,
                                        int.Parse(transform.position.y.ToString()) / 60,
                                        int.Parse(transform.position.z.ToString()) / 60);
        if(thisLocation == MapManager.root)
        {
            lightsOn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(lightState)
        {
            if(timeElapsed < lerpDuration)
            {
                valueToLerp = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
                foreach (Transform light in lights.transform)
                {
                    light.GetComponent<Light>().intensity = valueToLerp;
                }
            }
        }
        else
        {
            lightsOff();
        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            (int, int, int) thisLocation = (int.Parse(transform.position.x.ToString()) / 60,
                                            int.Parse(transform.position.y.ToString()) / 60,
                                            int.Parse(transform.position.z.ToString()) / 60);
            MapManager.lastKnownPlayerLocation = thisLocation;

            lightsOn();

            if (!hasBeenVisited)
            {
                hasBeenVisited = true;
                // add score
                ScoreManager scoreMan = GameObject.FindWithTag("HUD").GetComponent<ScoreManager>();
                scoreMan.addScore(10.0f);
            }
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            (int, int, int) thisLocation = (int.Parse(transform.position.x.ToString()) / 60,
                                            int.Parse(transform.position.y.ToString()) / 60,
                                            int.Parse(transform.position.z.ToString()) / 60);
            MapManager.lastKnownPlayerLocation = thisLocation;

        }

    }

    float timeElapsed;
    float lerpDuration = 1;
    float startValue = 0;
    float endValue = 1;
    float valueToLerp;

    private void lightsOn()
    {
        lights.SetActive(true);
        lightState = true;
        return;
    }

    private void lightsOff()
    {
        lights.SetActive(false);
        return;
    }
}
