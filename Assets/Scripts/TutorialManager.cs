using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : Singleton<TutorialManager>
{
    private GameObject thePlayer;
    private HandManager myHandMan;

    public Material normalTriggerMat;
    public Material blinkTriggerMat;
    private bool isFirstTime;

    private int tutorialSection;
    private bool isSectionCompleted;

    GameObject gameClock;
    GameObject pointsCounter;
    GameObject healthCounter;
    GameObject ammoCounter;
    GameObject oxygenCounter;

    public GameObject target1;
    public GameObject target2;
    public GameObject target3;

    GameObject tutorialAmmoCapsule;

    private Object ammoCapsuleObject;

    public GameObject oxygenBar;

    private enum TutorialSection
    {
        Flight,
        Gun,
        Swab,
        Oxygen
    }
    //private TutorialSection thisTutorialSection;

    void Awake()
    {
        ammoCapsuleObject = Resources.Load("Crawler/AmmoCapsule");
    }

    void Start()
    {
        // place the player
        Object playerCharacter = Resources.Load("astronaut");
        thePlayer = Instantiate(playerCharacter, Vector3.zero, Quaternion.identity) as GameObject;

        // disable pieces of the HUD
        gameClock = GameObject.FindWithTag("GameClock");
        pointsCounter = GameObject.FindWithTag("Points");
        healthCounter = GameObject.FindWithTag("Health");
        // todo: get both ammo counters
        ammoCounter = GameObject.FindWithTag("Ammo");
        oxygenCounter = GameObject.FindWithTag("Oxygen");
        myHandMan = GameObject.FindWithTag("Player").transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace").GetComponent<HandManager>();
       
        reset();

        return;
    }

    public void reset()
    {
        // disable and reset pieces of the HUD
        gameClock.SetActive(false);
        GameObject.FindWithTag("HUD").GetComponent<PointsWatcher>().reset();
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().reset();
        pointsCounter.SetActive(false);
        healthCounter.SetActive(false);
        ammoCounter.SetActive(false);
        oxygenCounter.SetActive(false);

        // disable all control axes individually
        GameManager.isRollEnabled = false;
        GameManager.isPitchEnabled = false;
        GameManager.isYawEnabled = false;
        GameManager.isForthEnabled = false;
        GameManager.isRightEnabled = false;
        GameManager.isUpEnabled = false;

        // set hands to controllers
        myHandMan.switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Controller);
        myHandMan.switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Controller);

        //reset the targets
        target1.GetComponent<Target>().reset();
        target2.GetComponent<Target>().reset();
        target3.GetComponent<Target>().reset();
        target1.SetActive(false);
        target2.SetActive(false);
        target3.SetActive(false);

        // disable the oxygen bar
        oxygenBar.SetActive(false);

        // allow for dark mode
        resetLights();

        //thisTutorialSection = TutorialSection.Flight;
    }


    void Update()
    {
        if(isSectionCompleted)
        {
            isSectionCompleted = false;
            tutorialSection++;
            isFirstTime = true;
            StartCoroutine(cancelBlinks());
        }
        switch (tutorialSection)
        {
            case 0: // the debug section
                isSectionCompleted = true;
                break;
            case 1:
                if (isFirstTime)
                {
                    notifyPressY("Use the left/right index trigger to go down/up.");
                    InvokeRepeating("blinkLIndexTrigger", 0f, 0.5f);
                    InvokeRepeating("blinkRIndexTrigger", 0f, 0.5f);
                    isFirstTime = false;
                    GameManager.isUpEnabled = true;
                }
                if(OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                {
                    isSectionCompleted = true;
                }
                break;
            case 2:
                if (isFirstTime)
                {
                    notifyPressY("Use the left stick to go forward/backward and left/right.");
                    GameManager.isForthEnabled = true;
                    GameManager.isRightEnabled = true;
                    InvokeRepeating("blinkLStick", 0f, 0.5f);
                    isFirstTime = false;
                }
                if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                {
                    isSectionCompleted = true;
                }
                break;
            case 3:
                if (isFirstTime)
                {
                    notifyPressY("Click in the left stick apply movement brakes.");
                    InvokeRepeating("blinkLStick", 0f, 0.5f);
                    isFirstTime = false;
                }
                if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                {
                    isSectionCompleted = true;
                }
                break;
            case 4:
                if (isFirstTime)
                {
                    notifyPressY("Use the right stick to pitch up/down and yaw left/right.");
                    GameManager.isPitchEnabled = true;
                    GameManager.isYawEnabled = true;
                    InvokeRepeating("blinkRStick", 0f, 0.5f);
                    isFirstTime = false;
                }
                if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                {
                    isSectionCompleted = true;
                }
                break;
            case 5:
                if (isFirstTime)
                {
                    notifyPressY("Use the left/right hand trigger to roll counter-clockwise/clockwise.");
                    GameManager.isRollEnabled = true;
                    InvokeRepeating("blinkLHandTrigger", 0f, 0.5f);
                    InvokeRepeating("blinkRHandTrigger", 0f, 0.5f);
                    isFirstTime = false;
                }
                if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                {
                    isSectionCompleted = true;
                }
                break;
            case 6:
                if (isFirstTime)
                {
                    notifyPressY("Click in the right stick apply rotation brakes.");
                    InvokeRepeating("blinkRStick", 0f, 0.5f);
                    isFirstTime = false;
                }
                if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                {
                    isSectionCompleted = true;
                }
                break;
            case 7:
                if (isFirstTime)
                {
                    notifyPressY("Click the menu button to toggle the menu.");
                    InvokeRepeating("blinkMenuButton", 0f, 0.5f);
                    isFirstTime = false;
                }
                if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                {
                    isSectionCompleted = true;
                }
                break;
            case 8:
                if (isFirstTime)
                {
                    notifyPressY("Press the A button to shoot your gun.");
                    myHandMan.switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Controller);
                    myHandMan.switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Gun);
                    InvokeRepeating("blinkAButton", 0f, 0.5f);
                    isFirstTime = false;
                }
                if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                {
                    isSectionCompleted = true;
                }
                break;
            case 9:
                if (isFirstTime)
                {
                    notify("Hit the targets.");
                    pointsCounter.SetActive(true);
                    GameObject.FindWithTag("HUD").GetComponent<PointsWatcher>().addTarget();
                    GameObject.FindWithTag("HUD").GetComponent<PointsWatcher>().addTarget();
                    GameObject.FindWithTag("HUD").GetComponent<PointsWatcher>().addTarget();
                    target1.SetActive(true);
                    target2.SetActive(true);
                    target3.SetActive(true);
                    GameObject.FindWithTag("Points").transform.Find("arrow").gameObject.SetActive(true);
                    isFirstTime = false;
                }
                if (target1.GetComponent<Target>().isActivated && target2.GetComponent<Target>().isActivated && target3.GetComponent<Target>().isActivated)
                {
                    isSectionCompleted = true;
                }
                break;
            case 10:
                if (isFirstTime)
                {
                    ammoCounter.SetActive(true);
                    GameObject.FindWithTag("Points").transform.Find("arrow").gameObject.SetActive(false);
                    GameObject.FindWithTag("Ammo").transform.Find("arrow").gameObject.SetActive(true);
                    notify("Pick up the ammo capsule.");
                    tutorialAmmoCapsule = Instantiate(ammoCapsuleObject, Vector3.zero, Quaternion.identity) as GameObject;
                    isFirstTime = false;
                }
                if (!tutorialAmmoCapsule)
                {
                    isSectionCompleted = true;
                }
                break;
            case 11:
                if (isFirstTime)
                {
                    notifyPressY("Use the B button to bring up the multi-tool menu.\nThe gun is used to hit targets."
                        + "\nThe glowball is used to mark the environment.\nThe phaser is used in self-defense.");
                    InvokeRepeating("blinkBButton", 0f, 0.5f);
                    isFirstTime = false;
                }
                if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                {
                    isSectionCompleted = true;
                }
                break;
            case 12:
                if (isFirstTime)
                {
                    GameObject.FindWithTag("Ammo").transform.Find("arrow").gameObject.SetActive(false);
                    GameManager.isOxygenMode = true;
                    oxygenCounter.SetActive(true);
                    GameObject.FindWithTag("Oxygen").transform.Find("arrow").gameObject.SetActive(true);
                    GameObject.FindWithTag("HUD").GetComponent<OxygenWatcher>().reset();
                    notifyPressY("This is your oxygen. If it reaches 0, you lose.");
                    isFirstTime = false;
                }
                GameObject.FindWithTag("HUD").GetComponent<OxygenWatcher>().oxygen = 101;
                if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                {
                    isSectionCompleted = true;
                }
                break;
            case 13:
                if (isFirstTime)
                {
                    GameObject.FindWithTag("HUD").GetComponent<OxygenWatcher>().oxygen = 50;
                    oxygenBar.SetActive(true);
                    myHandMan.switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Hand);
                    myHandMan.switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Hand);
                    notify("Locate the oxygen bar, then move your hand over the red oxygen bar and hold either A or X until your oxygen is full.");
                    InvokeRepeating("blinkAButton", 0f, 0.5f);
                    InvokeRepeating("blinkXButton", 0f, 0.5f);
                    isFirstTime = false;
                }
                if (GameObject.FindWithTag("HUD").GetComponent<OxygenWatcher>().oxygen < 21)
                {
                    GameObject.FindWithTag("HUD").GetComponent<OxygenWatcher>().oxygen = 21;
                }
                if (GameObject.FindWithTag("HUD").GetComponent<OxygenWatcher>().oxygen >= 100)
                {
                    isSectionCompleted = true;
                }
                break;
            case 14:
                if (isFirstTime)
                {
                    // cleanup after the oxygen tutorial
                    GameObject.FindWithTag("Oxygen").transform.Find("arrow").gameObject.SetActive(false);
                    GameManager.isOxygenMode = false;
                    GameObject.FindWithTag("HUD").GetComponent<OxygenWatcher>().reset();
                    isFirstTime = false;
                }
                if (true)
                {
                    isSectionCompleted = true;
                }
                break;
            case 15:
                if (isFirstTime)
                {
                    notifyPressY("This is your health. You lose at 0.");
                    healthCounter.SetActive(true);
                    GameObject.FindWithTag("Health").transform.Find("arrow").gameObject.SetActive(true);
                    isFirstTime = false;
                }
                if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                {
                    isSectionCompleted = true;
                }
                break;
            case 16:
                if (isFirstTime)
                {
                    // cleanup after the health tutorial
                    GameObject.FindWithTag("Health").transform.Find("arrow").gameObject.SetActive(false);
                    isFirstTime = false;
                }
                if (true)
                {
                    isSectionCompleted = true;
                }
                break;
            default:
                if (isFirstTime)
                {
                    notify("tutorial finished\nPress Y and B to start flight training");
                    isFirstTime = false;
                }
                if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch) && OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch))
                {
                    GameManager.isOxygenMode = false;
                    startFlightTraining();
                }
                if(!tutorialAmmoCapsule)
                {
                    tutorialAmmoCapsule = Instantiate(ammoCapsuleObject, Vector3.zero, Quaternion.identity) as GameObject;
                }
                break;
        }
    }

    void startFlightTraining()
    {
        GameManager.gameMode = GameManager.GameMode.FlightTraining;
        SceneManager.LoadSceneAsync("FlightTraining");
    }

    IEnumerator cancelBlinks()
    {
        CancelInvoke("blinkAButton");
        CancelInvoke("blinkBButton");
        CancelInvoke("blinkXButton");
        CancelInvoke("blinkYButton");
        CancelInvoke("blinkMenuButton");
        CancelInvoke("blinkLIndexTrigger");
        CancelInvoke("blinkRIndexTrigger");
        CancelInvoke("blinkLStick");
        CancelInvoke("blinkRStick");
        CancelInvoke("blinkLHandTrigger");
        CancelInvoke("blinkRHandTrigger");

        yield return new WaitForSeconds(0.5f);

        GameObject piece0 = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerPf/left_touch_controller_model_skel/lctrl:geometry_null/lctrl:main_trigger_PLY").gameObject;
        GameObject piece1 = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerPf/right_touch_controller_model_skel/rctrl:geometry_null/rctrl:main_trigger_PLY").gameObject;
        GameObject piece2 = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerPf/left_touch_controller_model_skel/lctrl:geometry_null/lctrl:side_trigger_PLY").gameObject;
        GameObject piece3 = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerPf/right_touch_controller_model_skel/rctrl:geometry_null/rctrl:side_trigger_PLY").gameObject;
        GameObject piece4 = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerPf/left_touch_controller_model_skel/lctrl:geometry_null/lctrl:thumbstick_ball_PLY").gameObject;
        GameObject piece5 = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerPf/right_touch_controller_model_skel/rctrl:geometry_null/rctrl:thumbstick_ball_PLY").gameObject;
        GameObject piece6 = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerPf/left_touch_controller_model_skel/lctrl:geometry_null/lctrl:x_button_PLY").gameObject;
        GameObject piece7 = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerPf/right_touch_controller_model_skel/rctrl:geometry_null/rctrl:a_button_PLY").gameObject;
        GameObject piece8 = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerPf/left_touch_controller_model_skel/lctrl:geometry_null/lctrl:y_button_PLY").gameObject;
        GameObject piece9 = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerPf/right_touch_controller_model_skel/rctrl:geometry_null/rctrl:b_button_PLY").gameObject;
        GameObject piece10 = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerPf/left_touch_controller_model_skel/lctrl:geometry_null/lctrl:o_button_PLY").gameObject;
           
        Material[] mats = piece0.GetComponent<SkinnedMeshRenderer>().materials;
        mats[0] = normalTriggerMat;

        piece0.GetComponent<SkinnedMeshRenderer>().materials = mats;
        piece1.GetComponent<SkinnedMeshRenderer>().materials = mats;
        piece2.GetComponent<SkinnedMeshRenderer>().materials = mats;
        piece3.GetComponent<SkinnedMeshRenderer>().materials = mats;
        piece4.GetComponent<SkinnedMeshRenderer>().materials = mats;
        piece5.GetComponent<SkinnedMeshRenderer>().materials = mats;
        piece6.GetComponent<SkinnedMeshRenderer>().materials = mats;
        piece7.GetComponent<SkinnedMeshRenderer>().materials = mats;
        piece8.GetComponent<SkinnedMeshRenderer>().materials = mats;
        piece9.GetComponent<SkinnedMeshRenderer>().materials = mats;
        piece10.GetComponent<SkinnedMeshRenderer>().materials = mats;

    }

    void notify(string note)
    {
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().pinNote(note);
    }
    void notifyPressY(string note)
    {
        string postscript = "\nPress Y to go to the next step";
        GameObject.FindWithTag("HUD").GetComponent<NotificationWatcher>().pinNote(note + postscript);
        InvokeRepeating("blinkYButton", 0f, 0.5f);
    }

    IEnumerator blink(GameObject controllerPiece)
    {
        Material[] mats = controllerPiece.GetComponent<SkinnedMeshRenderer>().materials;
        mats[0] = blinkTriggerMat;
        controllerPiece.GetComponent<SkinnedMeshRenderer>().materials = mats;
        yield return new WaitForSeconds(0.25f);
        mats[0] = normalTriggerMat;
        controllerPiece.GetComponent<SkinnedMeshRenderer>().materials = mats;
    }
    void blinkLIndexTrigger()
    {
        GameObject piece = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerPf/left_touch_controller_model_skel/lctrl:geometry_null/lctrl:main_trigger_PLY").gameObject;
        StartCoroutine(blink(piece));
    }
    void blinkRIndexTrigger()
    {
        GameObject piece = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerPf/right_touch_controller_model_skel/rctrl:geometry_null/rctrl:main_trigger_PLY").gameObject;
        StartCoroutine(blink(piece));
    }

    void blinkLHandTrigger()
    {
        GameObject piece = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerPf/left_touch_controller_model_skel/lctrl:geometry_null/lctrl:side_trigger_PLY").gameObject;
        StartCoroutine(blink(piece));
    }
    void blinkRHandTrigger()
    {
        GameObject piece = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerPf/right_touch_controller_model_skel/rctrl:geometry_null/rctrl:side_trigger_PLY").gameObject;
        StartCoroutine(blink(piece));
    }

    void blinkLStick()
    {
        GameObject piece = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerPf/left_touch_controller_model_skel/lctrl:geometry_null/lctrl:thumbstick_ball_PLY").gameObject;
        StartCoroutine(blink(piece));
    }
    void blinkRStick()
    {
        GameObject piece = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerPf/right_touch_controller_model_skel/rctrl:geometry_null/rctrl:thumbstick_ball_PLY").gameObject;
        StartCoroutine(blink(piece));
    }


    void blinkXButton()
    {
        GameObject piece = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerPf/left_touch_controller_model_skel/lctrl:geometry_null/lctrl:x_button_PLY").gameObject;
        StartCoroutine(blink(piece));
    }
    void blinkAButton()
    {
        GameObject piece = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerPf/right_touch_controller_model_skel/rctrl:geometry_null/rctrl:a_button_PLY").gameObject;
        StartCoroutine(blink(piece));
    }


    void blinkYButton()
    {
        GameObject piece = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerPf/left_touch_controller_model_skel/lctrl:geometry_null/lctrl:y_button_PLY").gameObject;
        StartCoroutine(blink(piece));
    }
    void blinkBButton()
    {
        GameObject piece = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/RightControllerPf/right_touch_controller_model_skel/rctrl:geometry_null/rctrl:b_button_PLY").gameObject;
        StartCoroutine(blink(piece));
    }

    void blinkMenuButton()
    {
        GameObject piece = thePlayer.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/LeftHandAnchor/LeftControllerPf/left_touch_controller_model_skel/lctrl:geometry_null/lctrl:o_button_PLY").gameObject;
        StartCoroutine(blink(piece));
    }

    public void resetLights()
    {
        var lights = GameObject.FindGameObjectsWithTag("Lights");
        foreach (GameObject light in lights)
        {
            if (GameManager.isDarkMode)
            {
                Debug.Log("disabling");

                light.SetActive(false);
            }
            else
            {
                light.SetActive(true);
            }
        }

        var headlights = GameObject.FindGameObjectsWithTag("Headlights");
        foreach (GameObject light in headlights)
        {
            if (GameManager.isDarkMode)
            {
                light.SetActive(true);
            }
            else
            {
                light.SetActive(false);
            }

        }
    }
}
