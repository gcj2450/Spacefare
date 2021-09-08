using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    GameObject pauseMenu;
    GameObject settingsMenu;
    private bool isSettingDisplayed;
    private Astronaut astronaut;
    private HandManager handMan;

    void Awake()
    {
        astronaut = transform.parent.GetComponent<Astronaut>();
        handMan = astronaut.transform.Find("OVRPlayerController/OVRCameraRig/TrackingSpace").GetComponent<HandManager>();
    }
    void Start()
    {
        pauseMenu = GameObject.FindWithTag("PauseMenu");
        settingsMenu = GameObject.FindWithTag("SettingsMenu");
        reset();
    }

    public void reset()
    {
        if (pauseMenu)
        {
            pauseMenu.SetActive(false);
        }
        if (settingsMenu)
        {
            settingsMenu.SetActive(false);
        }
        astronaut.disableLeftLineRenderer();
        astronaut.disableRightLineRenderer();

        GameManager.isGamePaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameMode != GameManager.GameMode.Menu)
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                if (GameManager.isGamePaused)
                {
                    wipeMenu();
                }
                else
                {
                    displayMenu();
                }
            }
        }
    }

    public void displayMenu()
    {
        // wipe the multi-tool menu
        handMan.wipeLeftMenu();
        handMan.wipeRightMenu();

        GameManager.isGamePaused = true;
        pauseMenu.SetActive(true);
        astronaut.enableRightLineRenderer();
    }

    public void wipeMenu()
    {
        GameManager.isGamePaused = false;
        pauseMenu.SetActive(false);
        astronaut.disableRightLineRenderer();
    }

    public void quitGame()
    {
        resetControls();
        GameManager.isGamePaused = false;
        SceneManager.LoadSceneAsync("MainMenu");
        return;
    }

    public void restartGame()
    {
        resetControls();
        if (GameManager.gameMode == GameManager.GameMode.Search)
        {
            GameBuilder.Instance.reset();
        }
        else if (GameManager.gameMode == GameManager.GameMode.FlightTraining)
        {
            TrainingManager.Instance.reset();
        }
        else if (GameManager.gameMode == GameManager.GameMode.Tutorial)
        {
            TutorialManager.Instance.reset();
        }
        else if (GameManager.gameMode == GameManager.GameMode.TimeTrial)
        {
            TimeTrialManager.Instance.reset();
            return;
        }
        else if (GameManager.gameMode == GameManager.GameMode.BossFight)
        {
            BossFightController.Instance.reset();
            return;
        }
    }

    public void displaySettingsMenu()
    {
        settingsMenu.SetActive(true);
        return;
    }

    public void wipeSettingsMenu()
    {
        settingsMenu.SetActive(false);
        return;
    }

    private void resetControls()
    {
        GameManager.isRollEnabled = true;
        GameManager.isPitchEnabled = true;
        GameManager.isYawEnabled = true;
        GameManager.isForthEnabled = true;
        GameManager.isRightEnabled = true;
        GameManager.isUpEnabled = true;
    }
}
