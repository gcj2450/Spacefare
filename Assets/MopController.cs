using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MopController : MonoBehaviour
{
    private Transform hand;
    public bool isRight;

    private float offsetx;
    private float offsety;
    private float offsetz;
    // Start is called before the first frame update
    void Start()
    {
        offsetx = 0;
        offsety = 0.4f;
        offsetz = 0.2f;

        // get the hand transforms
        if (isRight)
        {
            hand = GameObject.FindWithTag("Player").transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor");
        }
        else
        {
            hand = GameObject.FindWithTag("Player").transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor");
        }

        // configure snap-to-hands
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = hand.position + hand.up * offsety + hand.forward * offsetz + hand.right * offsetx;
        transform.rotation = hand.rotation * Quaternion.Euler(30,0,0);

        /*
        if(OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            GameObject.FindWithTag("Player").GetComponent<Astronaut>().switchHands(Astronaut.HandScheme.GunAndHand);
        }
        */
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Blood")
        {
            col.gameObject.GetComponent<BloodController>().startCleaning();
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Blood")
        {
            col.gameObject.GetComponent<BloodController>().stopCleaning();
        }
    }
}
