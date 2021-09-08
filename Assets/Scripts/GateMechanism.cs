using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateMechanism : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    public float volume = 0.5f;

    public Color color;

    public bool isOpened;

    private GameObject body;

    // Start is called before the first frame update
    void Start()
    {
        body = transform.Find("Body").gameObject;
        reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {

        if (col.gameObject.tag == "Bullet" && !isOpened && GameObject.FindWithTag("HUD").GetComponent<KeyManager>().hasColor(color))
        {
            // deactivate the gate
            body.SetActive(false);
            GetComponent<Collider>().enabled = false;

            // play a sound
            audioSource.PlayOneShot(clip, volume);

            // make sure we only do this once
            isOpened = true;
            
            // unparent the sticky bullets and delete their joints
            foreach (Transform child in transform)
            {
                if (child.tag == "GlowBullet")
                {
                    child.GetComponent<StickyBullet>().Unstick();
                }
            }

            // add score
            ScoreManager scoreMan = GameObject.FindWithTag("HUD").GetComponent<ScoreManager>();
            scoreMan.addScore(50.0f);
        }
        return;
    }

    public void reset()
    {
        isOpened = false;
        // ensure the gate is enabled
        body.SetActive(true);
        GetComponent<Collider>().enabled = true;
    }
}
