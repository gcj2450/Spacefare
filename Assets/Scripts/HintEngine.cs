using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintEngine : MonoBehaviour
{
    private Rigidbody astro;
    private bool isAngularDisplaying;
    private bool isMoveDisplaying;

    // Start is called before the first frame update
    void Start()
    {
        astro = transform.parent.GetComponent<Rigidbody>();
        isAngularDisplaying = false;
        isMoveDisplaying = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(astro.angularVelocity.magnitude);
        if(astro.angularVelocity.magnitude > 3)
        {
            isAngularDisplaying = true;
        }
        else
        {
            isAngularDisplaying = false;
        }

        if(isAngularDisplaying)
        {
            GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().sendNotification("Click Right Stick for rotation brakes");
        }

        //Debug.Log(astro.velocity.magnitude);
        if (astro.velocity.magnitude > 12)
        {
            isMoveDisplaying = true;
        }
        else
        {
            isMoveDisplaying = false;
        }

        if (isMoveDisplaying)
        {
            GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().sendNotification("Click Left Stick for movement brakes");
        }
    }

    public void reset()
    {
        return;
    }
}
