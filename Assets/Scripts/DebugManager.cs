using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private Astronaut astronaut;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.isInputEnabled = true;
        GameManager.isNoBrakes = false;
        GameManager.isDarkMode = false;
        GameManager.gameMode = GameManager.GameMode.Debug;
        GameManager.isGameOver = false;

        // place the player
        //var playerCharacter = Resources.Load("astronaut");
        //Instantiate(playerCharacter, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0));

        //GameObject.FindWithTag("Gun").GetComponent<GunMechanism>().addAmmo(50);

        //GameManager.showKeyboard();

        astronaut = GameObject.FindWithTag("Player").GetComponent<Astronaut>();
        astronaut.switchHand(Astronaut.Hand.Left, Astronaut.HandScheme.Swab);
        astronaut.switchHand(Astronaut.Hand.Right, Astronaut.HandScheme.Swab);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        Destroy(GameObject.FindWithTag("Player"));
    }
}
