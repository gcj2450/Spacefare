using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMechanism : MonoBehaviour
{
    private Astronaut astronaut;
    private AmmoManager ammoMan;
    private HandManager handMan;
    public AudioSource audioSource;
    public AudioClip bulletClip;
    public AudioClip glowBallClip;
    public float volume = 0.5f;

    public bool isRight;

    public enum FiringMode
    {
        Bullet,
        GlowBullet,
        Phaser
    }

    public FiringMode firingMode;


    // Start is called before the first frame update
    void Awake()
    {
        astronaut = GameObject.FindWithTag("Player").GetComponent<Astronaut>();
        ammoMan = transform.parent.parent.GetComponent<AmmoManager>();
        handMan = transform.parent.parent.GetComponent<HandManager>();
        firingMode = FiringMode.Bullet;
    }
    void Start()
    {
    }

    public void reset()
    {
    }

    public void OnEnable()
    {
        ammoMan.updateGlowballAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        bool AButton = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch);
        bool XButton = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch);

        if (GameManager.isInputEnabled && !GameManager.isGamePaused)
        {
            if (isRight && AButton && !handMan.isRightMenuUp)
            {
                if (firingMode == FiringMode.Bullet)
                {
                    Object bullet = Resources.Load("Tools/Bullet");
                    shoot(bullet);
                    ammoMan.incShotsTaken();
                    audioSource.PlayOneShot(bulletClip, volume);
                }
                else if (firingMode == FiringMode.GlowBullet && ammoMan.getGlowballAmmo() > 0)
                {
                    Object glowBall = Resources.Load("Tools/GlowBall");
                    shoot(glowBall);
                    audioSource.PlayOneShot(glowBallClip, volume);
                    ammoMan.useGlowballAmmo();
                }
                else if (firingMode == FiringMode.Phaser)
                {
                    StartCoroutine(shootPhaser());
                }
            }
            if (!isRight && XButton && !handMan.isLeftMenuUp)
            {
                if (firingMode == FiringMode.Bullet)
                {
                    Object bullet = Resources.Load("Tools/Bullet");
                    shoot(bullet);
                    ammoMan.incShotsTaken();
                    audioSource.PlayOneShot(bulletClip, volume);
                }
                else if (firingMode == FiringMode.GlowBullet && ammoMan.getGlowballAmmo() > 0)
                {
                    Object glowBall = Resources.Load("Tools/GlowBall");
                    shoot(glowBall);
                    audioSource.PlayOneShot(glowBallClip, volume);
                    ammoMan.useGlowballAmmo();
                }
                else if (firingMode == FiringMode.Phaser)
                {
                    StartCoroutine(shootPhaser());
                }
            }
        }
    }

    private IEnumerator shootPhaser()
    {
        if (isRight)
        {
            astronaut.enableRightLineRenderer();
        }
        else
        {
            astronaut.enableLeftLineRenderer();
        }
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, Mathf.Infinity);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Cleaner")
            {
                hit.collider.gameObject.GetComponent<Cleaner>().tagged();
            }
            else if (hit.collider.tag == "Monitor")
            {
                hit.collider.gameObject.GetComponent<Monitor>().tagged();
            }
        }
        yield return new WaitForSeconds(0.1f);
        if (isRight)
        {
            astronaut.disableRightLineRenderer();
        }
        else
        {
            astronaut.disableLeftLineRenderer();
        }
    }

    private void shoot(Object bulletType)
    {
        Vector3 thisPosition = transform.position;
        Quaternion rDirection = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // adjust the bullet to come out of the barrel
        thisPosition += transform.up * 0.05f + transform.forward * 0.2f;

        GameObject thisBullet = Instantiate(bulletType, thisPosition, Quaternion.identity) as GameObject;
        Rigidbody rb = thisBullet.GetComponent<Rigidbody>();
        rb.velocity = GameObject.FindWithTag("Player").GetComponent<Rigidbody>().velocity;
        StartCoroutine(addImpulse(thisBullet));

        // play a sound
        audioSource.PlayOneShot(glowBallClip, volume);
    }
    private IEnumerator addImpulse(GameObject thisBullet)
    {
        Rigidbody rb = thisBullet.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(0.01f);
        rb.AddForce(transform.forward * 0.5f);
        GameObject.FindWithTag("Player").GetComponent<Rigidbody>().AddForce(transform.forward * -0.5f);
    }

}
