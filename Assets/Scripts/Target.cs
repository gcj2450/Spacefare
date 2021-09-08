using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Material resetMaterial;
    public Material hitMaterial;

    public AudioSource audioSource;
    public AudioClip clip;
    public float volume = 0.5f;

    public bool isActivated;

    // Start is called before the first frame update
    void Start()
    {
        reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Bullet" && !isActivated)
        {
            // change the color
            GetComponent<MeshRenderer>().material = hitMaterial;
           
            // play a sound
            audioSource.PlayOneShot(clip, volume);
          
            // increment the points counter
            PointsWatcher pointsCounter = GameObject.FindWithTag("HUD").GetComponent<PointsWatcher>();
            pointsCounter.addPoint();

            // add score
            ScoreManager scoreMan = GameObject.FindWithTag("HUD").GetComponent<ScoreManager>();
            scoreMan.addScore(100.0f);

            // try to play the disintegration animation
            var anim = transform.parent.GetComponent<Animator>();
            if (anim)
            {
                anim.Play("Base Layer.targetDisintAnim");
            }

            // make sure we only do this once
            isActivated = true;
        }
        return;
    }

    public void reset()
    {
        isActivated = false;
        GetComponent<MeshRenderer>().material = resetMaterial;
    }

    public void destroySelf()
    {
        Destroy(transform.parent.gameObject);
    }
}
