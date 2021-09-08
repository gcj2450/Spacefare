using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCapsule : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip pickupClip;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.collider.tag == "Player")
        {
            pickup();
        }
    }
      
    void pickup()
    {
        // play a sound
        audioSource.PlayOneShot(pickupClip, 1.0f);

        // give the player some ammo
        GameObject.FindWithTag("Player").transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace").GetComponent<AmmoManager>().addGlowballAmmo(5);

        // destroy the body now and the parent after the explosion is done playing
        killBody();
        Invoke("destroyParent", 1.0f);
    }

    void killBody()
    {
        GetComponent<Rigidbody>().detectCollisions = false;
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    void destroyParent()
    {
        Destroy(transform.parent.gameObject);
    }

    void OnDestroy()
    {
        // unparent the sticky bullets and delete their joints
        foreach (Transform child in transform.parent)
        {
            if (child.tag == "GlowBullet")
            {
                child.GetComponent<StickyBullet>().Unstick();
            }
        }
    }
}
