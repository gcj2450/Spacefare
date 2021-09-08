using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenWatcher : MonoBehaviour
{
    public int oxygen;
    private bool isReplenishing;
    private float timeOfLastTick;

    public AudioSource audioSource;
    public AudioClip breatheSlow;
    public AudioClip breatheMedium;
    public AudioClip breatheFast;

    private enum breathType
    {
        Slow,
        Medium,
        Fast
    }

    private breathType m_breath;

    void Start()
    {
        isReplenishing = false;
        timeOfLastTick = Time.time;
        audioSource = GetComponent<AudioSource>();

        if (!GameManager.isOxygenMode)
        {
            GameObject myOxygen = GameObject.FindWithTag("Oxygen");
            if(myOxygen)
            {
                myOxygen.SetActive(false);
            }
        }
        else
        {
            reset();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.isOxygenMode)
        {
            breathe();
            if (!GameManager.isGameOver && isReplenishing && (Time.time - timeOfLastTick) > 1.0f)
            {
                // recover 10 oxygen every second
                addOxygen(10);
                timeOfLastTick = Time.time;
            }
            else if (!GameManager.isGameOver && (Time.time - timeOfLastTick) > 3.0f)
            {
                // lose 1 oxygen every 3 seconds
                subtractOxygen(1);
                timeOfLastTick = Time.time;
            }

            if (oxygen <= 0 && !GameManager.isGameOver)
            {
                GameManager.isGameOver = true;
                GameBuilder.Instance.showLoseScreen(GameBuilder.CauseOfDeath.Suffocated);
            }
        }
    }

    public void subtractOxygen(int value)
    {
        // increment the points counter
        oxygen -= value;
        updateOxygen();
    }
    public void addOxygen(int value)
    {
        // increment the points counter
        oxygen += value;
        if(oxygen>100)
        {
            oxygen = 100;
        }
        updateOxygen();
    }

    public void startReplenishOxygen()
    {
        isReplenishing = true;
    }
    public void stopReplenishOxygen()
    {
        isReplenishing = false;
    }

    private void updateOxygen()
    {
        GameObject oxygenCounter = GameObject.FindWithTag("Oxygen");
        if (oxygenCounter != null)
        {
            oxygenCounter.GetComponent<UnityEngine.UI.Text>().text = oxygen.ToString();
        }
    }

    private void breathe()
    {
        if(oxygen < 25 && m_breath != breathType.Fast)
        {
            Debug.Log("breath fast now");
            m_breath = breathType.Fast;
            audioSource.clip = breatheFast;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (25 < oxygen && oxygen < 50 && m_breath != breathType.Medium)
        {
            Debug.Log("breath medium now");
            m_breath = breathType.Medium;
            audioSource.clip = breatheMedium;
            audioSource.loop = true;
            audioSource.Play();
        }
        else if (  oxygen > 75 && m_breath != breathType.Slow)
        {
            Debug.Log("breath slow now");
            m_breath = breathType.Slow;
            audioSource.clip = breatheSlow;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            // do nothing
        }
    }

    public void reset()
    {
        oxygen = 100;
        updateOxygen();

        m_breath = breathType.Slow;
        audioSource.clip = breatheSlow;
        audioSource.loop = true;
        if (GameManager.isOxygenMode)
        {
            audioSource.Play();
        }
    }
}
