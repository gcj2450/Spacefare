using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    private Astronaut astronaut;

    private GameObject leftHand;
    private GameObject leftController;
    private GameObject leftSwab;
    private GameObject leftGun;
    private GunMechanism leftGunMech;

    private GameObject rightHand;
    private GameObject rightController;
    private GameObject rightSwab;
    private GameObject rightGun;
    private GunMechanism rightGunMech;

    public bool isRightMenuUp;
    public bool isLeftMenuUp;
    private GameObject RightMenu;
    private GameObject LeftMenu;

    private GameObject rightLineRendererObj;
    private GameObject leftLineRendererObj;
    public bool isLeftLineRendering;
    public bool isRightLineRendering;

    void Awake()
    {
        getMyObjects();
    }

    void Start()
    {
        astronaut = GameObject.FindWithTag("Player").GetComponent<Astronaut>();
        getMyObjects();

        isRightLineRendering = false;
        isLeftLineRendering = false;

        wipeRightMenu();
        wipeLeftMenu();
    }

    private void getMyObjects()
    {
        if (!leftHand)
        {
            leftHand = transform.Find("LeftHandAnchor/CustomHandLeft").gameObject;
        }
        if (!leftController)
        {
            leftController = transform.Find("LeftHandAnchor/LeftControllerPf").gameObject;
        }
        if (!leftSwab)
        {
            leftSwab = transform.Find("LeftHandAnchor/Swab").gameObject;
        }
        if (!leftGun)
        {
            leftGun = transform.Find("LeftHandAnchor/Gun").gameObject;
        }
        if (!leftGunMech)
        {
            leftGunMech = leftGun.GetComponent<GunMechanism>();
        }

        if (!rightHand)
        {
            rightHand = transform.Find("RightHandAnchor/CustomHandRight").gameObject;
        }
        if (!rightController)
        {
            rightController = transform.Find("RightHandAnchor/RightControllerPf").gameObject;
        }
        if (!rightSwab)
        {
            rightSwab = transform.Find("RightHandAnchor/Swab").gameObject;
        }
        if (!rightGun)
        {
            rightGun = transform.Find("RightHandAnchor/Gun").gameObject;
        }
        if (!rightGunMech)
        {
            rightGunMech = rightGun.GetComponent<GunMechanism>();
        }

        if (!RightMenu)
        {
            RightMenu = GameObject.FindWithTag("HUD").transform.Find("RightMenu").gameObject;
        }
        if (!LeftMenu)
        {
            LeftMenu = GameObject.FindWithTag("HUD").transform.Find("LeftMenu").gameObject;
        }

        if (!rightLineRendererObj)
        {
            rightLineRendererObj = GameObject.FindWithTag("Player").transform.Find("RightHandEventSystem").gameObject;
        }
        if (!leftLineRendererObj)
        {
            leftLineRendererObj = GameObject.FindWithTag("Player").transform.Find("LeftHandEventSystem").gameObject;
        }
    }


    // Update is called once per frame
    void Update()
    {
        bool AButton = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch);
        bool BButton = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch);
        bool XButton = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch);
        bool YButton = OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch);

        if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch) && OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            OVRManager.display.RecenterPose();
        }
        if (GameManager.isInputEnabled && !GameManager.isGamePaused && !GameManager.isGameOver)
        {
            if (BButton)
            {
                if (isRightMenuUp)
                {
                    wipeRightMenu();
                }
                else
                {
                    displayRightMenu();
                }
            }
            if (YButton && GameManager.gameMode != GameManager.GameMode.Tutorial)
            {
                if (isLeftMenuUp)
                {
                    wipeLeftMenu();
                }
                else
                {
                    displayLeftMenu();
                }
            }
        }
    }

    public void switchHand(Astronaut.Hand thisHand, Astronaut.HandScheme thisScheme)
    {
        getMyObjects();
        switch (thisHand)
        {
            case Astronaut.Hand.Left:
                leftGun.SetActive(false);
                leftSwab.SetActive(false);
                leftController.SetActive(false);
                leftHand.SetActive(false);
                switch (thisScheme)
                {
                    case Astronaut.HandScheme.Hand:
                        leftHand.SetActive(true);
                        break;
                    case Astronaut.HandScheme.Gun:
                        leftGun.SetActive(true);
                        break;
                    case Astronaut.HandScheme.Swab:
                        leftSwab.SetActive(true);
                        break;
                    case Astronaut.HandScheme.Controller:
                        leftController.SetActive(true);
                        break;
                    case Astronaut.HandScheme.Empty:
                        break;
                }
                break;
            case Astronaut.Hand.Right:
                rightGun.SetActive(false);
                rightSwab.SetActive(false);
                rightHand.SetActive(false);
                rightController.SetActive(false);
                switch (thisScheme)
                {
                    case Astronaut.HandScheme.Hand:
                        rightHand.SetActive(true);
                        break;
                    case Astronaut.HandScheme.Gun:
                        rightGun.SetActive(true);
                        break;
                    case Astronaut.HandScheme.Swab:
                        rightSwab.SetActive(true);
                        break;
                    case Astronaut.HandScheme.Controller:
                        rightController.SetActive(true);
                        break;
                    case Astronaut.HandScheme.Empty:
                        break;
                }
                break;
        }
    }

    public void displayRightMenu()
    {
        if(isLeftMenuUp)
        {
            wipeLeftMenu();
        }
        isRightMenuUp = true;
        RightMenu.SetActive(true);
        astronaut.enableRightLineRenderer();
    }
    public void displayLeftMenu()
    {
        if (isRightMenuUp)
        {
            wipeRightMenu();
        }
        isLeftMenuUp = true;
        LeftMenu.SetActive(true);
        astronaut.enableLeftLineRenderer();
    }
    public void wipeRightMenu()
    {
        isRightMenuUp = false;
        RightMenu.SetActive(false);
        astronaut.disableRightLineRenderer();
    }
    public void wipeLeftMenu()
    {
        isLeftMenuUp = false;
        LeftMenu.SetActive(false);
        astronaut.disableLeftLineRenderer();
    }

    public void leftSelectBullet()
    {
        wipeLeftMenu();
        switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Gun);
        leftGunMech.firingMode = GunMechanism.FiringMode.Bullet;
    }
    public void leftSelectGlowball()
    {
        wipeLeftMenu();
        switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Gun);
        leftGunMech.firingMode = GunMechanism.FiringMode.GlowBullet;
    }
    public void leftSelectPhaser()
    {
        wipeLeftMenu();
        switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Gun);
        leftGunMech.firingMode = GunMechanism.FiringMode.Phaser;
    }
    public void leftSelectSwab()
    {
        wipeLeftMenu();
        switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Swab);
    }
    public void leftSelectHand()
    {
        wipeLeftMenu();
        switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Hand);
    }
    public void rightSelectBullet()
    {
        wipeRightMenu();
        switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Gun);
        rightGunMech.firingMode = GunMechanism.FiringMode.Bullet;
    }
    public void rightSelectGlowball()
    {
        wipeRightMenu();
        switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Gun);
        rightGunMech.firingMode = GunMechanism.FiringMode.GlowBullet;
    }
    public void rightSelectPhaser()
    {
        wipeRightMenu();
        switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Gun);
        rightGunMech.firingMode = GunMechanism.FiringMode.Phaser;
    }
    public void rightSelectSwab()
    {
        wipeRightMenu();
        switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Swab);
    }
    public void rightSelectHand()
    {
        wipeRightMenu();
        switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Hand);
    }

    public void enableRightLineRenderer()
    {
        isRightLineRendering = true;
        rightLineRendererObj.SetActive(true);
        rightLineRendererObj.GetComponent<LineRenderer>().enabled = false;
        StartCoroutine(enableRightLineRendererHelper());
    }
    public void disableRightLineRenderer()
    {
        isRightLineRendering = false;
        rightLineRendererObj.GetComponent<LineRenderer>().enabled = true;
        rightLineRendererObj.SetActive(false);
    }
    private IEnumerator enableRightLineRendererHelper()
    {
        // we wait to enable the actual line renderer,
        // bc otherwise it snaps from its last known position to its new one,
        // which looks really bad
        // we can go to about 0.015 here, but we're generous
        yield return new WaitForSeconds(0.025f);
        rightLineRendererObj.GetComponent<LineRenderer>().enabled = true;
    }
    public void enableLeftLineRenderer()
    {
        isLeftLineRendering = true;
        leftLineRendererObj.SetActive(true);
        leftLineRendererObj.GetComponent<LineRenderer>().enabled = false;
        StartCoroutine(enableLeftLineRendererHelper());
    }
    public void disableLeftLineRenderer()
    {
        isLeftLineRendering = false;
        leftLineRendererObj.GetComponent<LineRenderer>().enabled = true;
        leftLineRendererObj.SetActive(false);
    }
    private IEnumerator enableLeftLineRendererHelper()
    {
        // we wait to enable the actual line renderer,
        // bc otherwise it snaps from its last known position to its new one,
        // which looks really bad
        // we can go to about 0.015 here, but we're generous
        yield return new WaitForSeconds(0.025f);
        leftLineRendererObj.GetComponent<LineRenderer>().enabled = true;
    }

}
