using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodController : MonoBehaviour
{
    public bool isCleaning;
    private Animation anim;

    // Start is called before the first frame update
    void Start()
    {
        isCleaning = false;
        anim = GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isCleaning)
        {
            foreach (AnimationState state in anim)
            {
                state.speed = 0.25f;
            }
        }
        else
        {
            // disable animation OR set animation speed to zero
            foreach (AnimationState state in anim)
            {
                state.speed = 0.0f;
            }
        }
    }

    public void startCleaning()
    {
        isCleaning = true;
    }
    public void stopCleaning()
    {
        isCleaning = false;
    }

    public void destroySelf()
    {
        Destroy(gameObject);
    }
}
