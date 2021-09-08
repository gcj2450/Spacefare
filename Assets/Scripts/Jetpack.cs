using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour
{
    private Rigidbody rb;
    public Transform astronautHead;

    private float translationThrust;
    private float yawThrust;
    private float rollThrust;
    private float pitchThrust;

    public AudioSource audioSource1;

    private AudioSource[] audioSourceArray;
    private int audioToggle;
    private double nextEventTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        translationThrust = 3;
        yawThrust = 0.25f;
        rollThrust = 0.25f;
        pitchThrust = 0.25f;


        //update helmet position
        updateHead();
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.isInputEnabled)
        {
            float maxThrust = 0;

            float lIndexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            float rIndexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
            if (GameManager.isUpEnabled)
            {
                rb.AddForce(transform.up * (rIndexTrigger - lIndexTrigger) * translationThrust);
                if(lIndexTrigger > maxThrust)
                {
                    maxThrust = lIndexTrigger;
                }
                if (rIndexTrigger > maxThrust)
                {
                    maxThrust = rIndexTrigger;
                }
            }

            Vector2 lStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
            if (GameManager.isRightEnabled)
            {
                rb.AddForce(transform.right * lStick.x * translationThrust);
                if (Mathf.Abs(lStick.x) > maxThrust)
                {
                    maxThrust = Mathf.Abs(lStick.x);
                }
            }
            if (GameManager.isForthEnabled)
            {
                rb.AddForce(transform.forward * lStick.y * translationThrust);
                if (Mathf.Abs(lStick.y) > maxThrust)
                {
                    maxThrust = Mathf.Abs(lStick.y);
                }
            }

            Vector2 rStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
            if (GameManager.isPitchEnabled)
            {
                rb.AddTorque(transform.up * rStick.x * pitchThrust);
                if (Mathf.Abs(rStick.x) > maxThrust)
                {
                    maxThrust = Mathf.Abs(rStick.x);
                }
            }
            if (GameManager.isYawEnabled)
            {
                rb.AddTorque(transform.right * rStick.y * -1 * yawThrust);
                if (Mathf.Abs(rStick.y) > maxThrust)
                {
                    maxThrust = Mathf.Abs(rStick.y);
                }
            }

            float lHandTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);
            float rHandTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);
            if (GameManager.isRollEnabled)
            {
                rb.AddTorque(transform.forward * (lHandTrigger-rHandTrigger) * rollThrust);
                if (Mathf.Abs(lHandTrigger - rHandTrigger) > maxThrust)
                {
                    maxThrust = Mathf.Abs(lHandTrigger - rHandTrigger);
                }
            }

            bool lStickButton = OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch);
            bool rStickButton = OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch);
            if (lStickButton && !GameManager.isNoBrakes)
            {
                float brakeFactor = 1.0f;
                rb.AddForce(-brakeFactor * rb.velocity);
                if (brakeFactor * rb.velocity.magnitude > maxThrust)
                {
                    maxThrust = brakeFactor * rb.velocity.magnitude;
                }
            }
            if (rStickButton && !GameManager.isNoBrakes)
            {
                float brakeFactor = 0.25f;
                rb.AddTorque(-brakeFactor * rb.angularVelocity);
                if (brakeFactor * rb.angularVelocity.magnitude > maxThrust)
                {
                    maxThrust = brakeFactor * rb.angularVelocity.magnitude;
                }
            }
            if(maxThrust > 1)
            {
                maxThrust = 1;
            }



            if (maxThrust < 0.001)
            {
                audioSource1.loop = false;
            }
            else
            {
                audioSource1.volume = maxThrust;
                if (!audioSource1.loop)
                {
                    audioSource1.loop = true;
                    audioSource1.Play();
                }
            }

        }
    }

    void updateHead()
    {
        //snap the body to the head
        transform.position = astronautHead.position;
        return;
    }
}
