using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int hp;
    public int maxHP;
    public enum Phase
    {
        One,
        Two,
        Three,
        Four
    }
    new public string name = "null";
    protected Phase fightStage;
    protected GameObject astronaut;
    public AudioSource audioSource;

    protected bool isAwake = false;
    protected bool justWokeUp = false;
    public void waitToWake()
    {
        StartCoroutine(waitForWake());
    }
    protected IEnumerator waitForWake()
    {
        yield return new WaitForSeconds(3.0f);
        justWokeUp = true;
        isAwake = true;
    }



}
