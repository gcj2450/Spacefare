using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayManager : MonoBehaviour
{
    private GameObject lights;

    // Start is called before the first frame update
    void Start()
    {
        // turn off the lights
        lights = transform.Find("LightArray").gameObject;
        lightsOff();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            lightsOn();
        }

    }

    public void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            lightsOff();
        }
    }

    private void lightsOn()
    {
        lights.SetActive(true);

    }
    private void lightsOff()
    {
        lights.SetActive(false);
        return;
    }
}
