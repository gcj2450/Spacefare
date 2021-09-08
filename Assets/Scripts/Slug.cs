using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slug : MonoBehaviour
{
    public float lifetime = 10.0f;

    void Awake()
    {
        Destroy(this.gameObject, lifetime);
    }
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
        if (col.collider.tag == "Player")
        {
            // play a thwack sound
            //audioSource.PlayOneShot(thwackClip, volume);

            // decrement the health counter
            HealthWatcher hpWatcher = GameObject.FindWithTag("HUD").GetComponent<HealthWatcher>();
            hpWatcher.subtractHealth(10, "The Attack Drone");
            return;
        }
    }
}
