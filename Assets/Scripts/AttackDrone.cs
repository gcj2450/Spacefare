using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDrone : Boss
{
    private float timeOfLastShot;

    //public AudioSource audioSource;
    public AudioClip slugClip;

    public BossTarget target1;
    public BossTarget target2;
    public BossTarget target3;
    public BossTarget target4;

    // Start is called before the first frame update
    void Start()
    {
        fightStage = Phase.One;
        astronaut = GameObject.FindWithTag("Player");
        hp = 4;
        maxHP = 4;
        name = "Attack Drone";

        powerDown();
    }

    // Update is called once per frame
    void Update()
    {
        // make sure we always stay in the center
        transform.position = Vector3.zero;
        if (hp <= 0 || !isAwake)
        {
            // explode
            return;
        }
        if (justWokeUp)
        {
            justWokeUp = false;
            powerUp();
        }
        if(!astronaut)
        {
            return;
        }

        GameObject futurePosObj = new GameObject();
        switch (fightStage)
        {
            case Phase.One:
                // shoot directly at the astronaut once per second
                transform.LookAt(astronaut.transform);
                if (Time.time - timeOfLastShot > 1.0f)
                {
                    shoot();
                }
                break;
            case Phase.Two:
                // lead a little bit
                futurePosObj.transform.position = astronaut.transform.position;
                futurePosObj.transform.position += astronaut.GetComponent<Rigidbody>().velocity * 0.25f;
                transform.LookAt(futurePosObj.transform);
                if (Time.time - timeOfLastShot > 1.0f)
                {
                    shoot();
                }
                break;
            case Phase.Three:
                // shoot a bit faster
                futurePosObj.transform.position = astronaut.transform.position;
                futurePosObj.transform.position += astronaut.GetComponent<Rigidbody>().velocity * 0.5f;
                transform.LookAt(futurePosObj.transform);
                if (Time.time - timeOfLastShot > 0.75f)
                {
                    shoot();
                }
                break;
            case Phase.Four:
                // lead a bit more and shoot a bit faster
                futurePosObj.transform.position = astronaut.transform.position;
                futurePosObj.transform.position += astronaut.GetComponent<Rigidbody>().velocity * 0.75f;
                transform.LookAt(futurePosObj.transform);
                if (Time.time - timeOfLastShot > 0.66f)
                {
                    shoot();
                }
                break;
        }
        Destroy(futurePosObj);
    }

    public void decrementHealth()
    {
        hp--;
        switch (hp)
        {
            case 4:
                fightStage = Phase.One;
                break;
            case 3:
                fightStage = Phase.Two;
                break;
            case 2:
                fightStage = Phase.Three;
                break;
            case 1:
                fightStage = Phase.Four;
                break;
        }
        return;
    }


    private void shoot()
    {
        if (isAwake)
        {
            timeOfLastShot = Time.time;
            Vector3 thisPosition = transform.position;

            // adjust the bullet to come out of the barrel
            thisPosition += transform.forward * 3.25f;

            GameObject thisBullet = Instantiate(Resources.Load("Slug"), thisPosition, Quaternion.identity) as GameObject;
            StartCoroutine(addImpulse(thisBullet));

            // play a sound
            audioSource.PlayOneShot(slugClip, 1.0f);
        }
    }

    private IEnumerator addImpulse(GameObject thisBullet)
    {
        Rigidbody rb = thisBullet.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(0.01f);
        rb.AddForce(transform.forward * 100.0f, ForceMode.Impulse);
    }

    private void powerUp()
    {
        target1.powerUp();
        target2.powerUp();
        target3.powerUp();
        target4.powerUp();
    }

    private void powerDown()
    {
        target1.powerDown();
        target2.powerDown();
        target3.powerDown();
        target4.powerDown();
    }
}
