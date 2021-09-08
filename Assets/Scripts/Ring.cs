using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    private bool isDead;
    public bool isCenterRing = false;
    public AudioSource audioSource;
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = clip;
        audioSource.volume = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter()
    {
        if (!isDead)
        {
            // give a beep
            audioSource.Play();
            if (!isCenterRing)
            {
                isDead = true;
                // give a point if the game isn't over
                if (!GameManager.isGameOver)
                {
                    PointsWatcher pointsCounter = GameObject.FindWithTag("HUD").GetComponent<PointsWatcher>();
                    pointsCounter.addRingPoint();
                }

                // schedule a delete
                TrainingManager.Instance.scatterRing(false);
                Invoke("destroySelf", 0.5f);
            }
        }
    }

    void destroySelf()
    {
        Destroy(gameObject);
    }
}
