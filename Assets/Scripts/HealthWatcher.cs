using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthWatcher : MonoBehaviour
{
    public int health;

    // Start is called before the first frame update
    void Awake()
    {
        GameManager.isGameOver = false;
        health = 100;
        updateHealth();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void subtractHealth(int value, string dealer)
    {
        // increment the points counter
        health -= value;
        updateHealth();
        GameManager.lastDamageDealer = dealer;
    }

    private void updateHealth()
    {
        GameObject healthCounter = GameObject.FindWithTag("Health");
        if (healthCounter != null)
        {
            healthCounter.GetComponent<UnityEngine.UI.Text>().text = health.ToString();
        }
    }

    public void reset()
    {
        GameManager.isGameOver = false;
        health = 100;
        updateHealth();
    }
}
