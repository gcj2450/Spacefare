using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astronaut : MonoBehaviour
{
    private HandManager myHandManager;
    private NotificationWatcher myNote;
    private PointsWatcher myPoints;
    private GameClock myClock;
    private KeyManager myInv;
    private PauseMenu myMenu;
    private ScoreManager myScore;
    private HintEngine myHints;
    private HealthWatcher myHP;
    private OxygenWatcher myO2;
    private AmmoManager myAmmoMan;

    private GameObject[] myControllers;
    private GameObject myHand;
    private GameObject myGunObj;
    private GunMechanism myGun;
    private GameObject mySwab;

    public enum Hand
    {
        Left,
        Right
    }
    public enum HandScheme
    {
        Hand,
        Gun,
        Swab,
        Controller,
        Empty
    }

    void Awake()
    {
        myHandManager = transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace").GetComponent<HandManager>();
        GameObject myHUD = transform.Find("HUD").gameObject;
        myNote = myHUD.GetComponent<NotificationWatcher>();
        myPoints = myHUD.GetComponent<PointsWatcher>();
        myClock = myHUD.GetComponent<GameClock>();
        myInv = myHUD.GetComponent<KeyManager>();
        myMenu = myHUD.GetComponent<PauseMenu>();
        myScore = myHUD.GetComponent<ScoreManager>();
        myHints = myHUD.GetComponent<HintEngine>();
        myHP = myHUD.GetComponent<HealthWatcher>();
        myO2 = myHUD.GetComponent<OxygenWatcher>();

        myControllers = GameObject.FindGameObjectsWithTag("ControllerModel");
        myHand = GameObject.FindWithTag("LeftHand");
        myGunObj = GameObject.FindWithTag("Gun");
        myGun = myGunObj.GetComponent<GunMechanism>();
        mySwab = transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace/RightHandAnchor/Swab").gameObject;

        myAmmoMan = transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace").GetComponent<AmmoManager>();
    }

    void Start()
    {
    }

    void Update()
    {
        
    }

    public void reset()
    {
        myNote.reset();
        myPoints.reset();
        myClock.reset();
        myInv.reset();
        myMenu.reset();
        myScore.reset();
        myHints.reset();
        myHP.reset();
        myO2.reset();
        //myGun.reset();
        myAmmoMan.reset();
        disableRightLineRenderer();
        disableLeftLineRenderer();
    }

    public void switchHand(Hand thisHand, HandScheme thisScheme)
    {
        myHandManager.switchHand(thisHand, thisScheme);
    }
    public void enableRightLineRenderer()
    {
        myHandManager.enableRightLineRenderer();
    }
    public void disableRightLineRenderer()
    {
        myHandManager.disableRightLineRenderer();
    }
    public void enableLeftLineRenderer()
    {
        myHandManager.enableLeftLineRenderer();
    }
    public void disableLeftLineRenderer()
    {
        myHandManager.disableLeftLineRenderer();
    }

    public int getHealth()
    {
        return (myHP.health);
    }

}
