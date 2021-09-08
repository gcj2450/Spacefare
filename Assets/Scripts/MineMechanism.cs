using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineMechanism : MonoBehaviour
{
    private GameObject body;
    public bool isSimple;
    public Material matOne;
    public Material matTwo;
    public AudioSource audioSource;
    public AudioClip clip;
    public AudioClip beepClip;
    public float volume = 0.5f;
    private bool isDead;
    private Transform trackedPlayer;
    private bool isMatOne;

    // Start is called before the first frame update
    void Start()
    {
        body = transform.parent.Find("Body").gameObject;
        isDead = false;
        trackedPlayer = null;
        isMatOne = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
        {
            return;
        }
        //set transform to that of the body
        transform.parent.transform.position = body.transform.position;
        body.transform.localPosition = Vector3.zero;

        if (trackedPlayer==null)
        {
            return;
        }
        float distToPlayer = (trackedPlayer.position - transform.position).magnitude;

        if(distToPlayer < 1)
        {
            detonate();
            isDead = true;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (isDead || col.tag != "Player")
        {
            return;
        }
        trackedPlayer = col.transform;
        blinkAndBeep();
    }

    void OnTriggerExit(Collider col)
    {
        if (isDead || col.tag != "Player")
        {
            return;
        }
        trackedPlayer = null;
    }

    void detonate()
    {
        // play a sound
        audioSource.PlayOneShot(clip, volume);

        // decrement the health counter
        HealthWatcher hpWatcher = GameObject.FindWithTag("HUD").GetComponent<HealthWatcher>();
        hpWatcher.subtractHealth(25, "a mine");

        // trigger an explosion force
        Vector3 expPosition = transform.position;
        float power = 10.0f;
        float radius = 20.0f;
        float upwardsMod = 0.0f;

        // apply forces
        Collider[] colliders = Physics.OverlapSphere(expPosition, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (isSimple)
                {
                    Vector3 forceDirection = Vector3.Normalize(hit.transform.position - body.transform.position);
                    rb.AddForce(power * forceDirection, ForceMode.Impulse);
                }
                else
                {
                    rb.AddExplosionForce(power * 10, expPosition, radius, upwardsMod);
                }
            }
        }

        // destroy the body now and the parent after the explosion is done playing
        Destroy(body);
        Invoke("destroyParent", 5.0f);
    }

    void destroyParent()
    {
        Destroy(transform.parent.gameObject);
    }

    void blinkAndBeep()
    {
        if(isDead)
        {
            return;
        }
        if (trackedPlayer == null)
        {
            if(body==null)
            {
                return;
            }
            body.GetComponent<MeshRenderer>().material = matOne;
            return;
        }

        //toggle the material
        float distToPlayer = (trackedPlayer.position - transform.position).magnitude;
        float blinkInterval = (distToPlayer - 1) * 0.5f;
        float maxBlinkDuration = 0.3f;
        if (isMatOne)
        {
            body.GetComponent<MeshRenderer>().material = matTwo;
            float thisInterval = (blinkInterval < maxBlinkDuration) ? blinkInterval : maxBlinkDuration;
            //invoke self in just a moment
            Invoke("blinkAndBeep", thisInterval);
            audioSource.PlayOneShot(beepClip, volume);
        }
        else
        {
            body.GetComponent<MeshRenderer>().material = matOne;
            //invoke self in interval time
            Invoke("blinkAndBeep", blinkInterval);
        }
        isMatOne = !isMatOne;

        return;
    }

}
