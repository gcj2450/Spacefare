using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardController : MonoBehaviour
{
    public GameObject mainMenu;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void sendToSeed(char thisChar)
    {
        if (GameManager.gameMode == GameManager.GameMode.Menu)
        {
            string currentText = mainMenu.transform.Find("SeedInput/Placeholder").GetComponent<Text>().text;
            string nextText = currentText + thisChar;
            mainMenu.transform.Find("SeedInput/Placeholder").GetComponent<Text>().text = nextText;
        }
        else // we must be querying for name at the end
        {
            string currentText = GameObject.FindWithTag("HUD").transform.Find("Canvas Layer 7/Canvas/NameEntry").GetComponent<Text>().text;
            string nextText = currentText + thisChar;
            GameObject.FindWithTag("HUD").transform.Find("Canvas Layer 7/Canvas/NameEntry").GetComponent<Text>().text = nextText;
        }
    }
    public void doneWithNameEntry()
    {
        string name = GameObject.FindWithTag("HUD").transform.Find("Canvas Layer 7/Canvas/NameEntry").GetComponent<Text>().text;
        float completionTime = GameObject.FindWithTag("HUD").GetComponent<GameClock>().timeElapsed;

        GameObject.FindWithTag("Player").GetComponent<Astronaut>().disableLeftLineRenderer();
        GameObject.FindWithTag("Player").GetComponent<Astronaut>().disableRightLineRenderer();

        // write to file
        GameManager.WriteToLeaderboards(name, completionTime);

        //wipe the name
        GameObject.FindWithTag("HUD").transform.Find("Canvas Layer 7/Canvas/NameEntry").GetComponent<Text>().text = "";

        // close the keyboard
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void clearSeed()
    {
        if (GameManager.gameMode == GameManager.GameMode.Menu)
        {
            mainMenu.transform.Find("SeedInput/Placeholder").GetComponent<Text>().text = "";

        }
        else // we must be querying for name at the end
        {
            GameObject.FindWithTag("HUD").transform.Find("Canvas Layer 7/Canvas/NameEntry").GetComponent<Text>().text = "";
        }
    }
    public void sendSpace()
    {
        sendToSeed(' ');
    }
    public void sendA()
    {
        sendToSeed('A');
    }
    public void sendB()
    {
        sendToSeed('B');
    }
    public void sendC()
    {
        sendToSeed('C');
    }
    public void sendD()
    {
        sendToSeed('D');
    }
    public void sendE()
    {
        sendToSeed('E');
    }
    public void sendF()
    {
        sendToSeed('F');
    }
    public void sendG()
    {
        sendToSeed('G');
    }
    public void sendH()
    {
        sendToSeed('H');
    }
    public void sendI()
    {
        sendToSeed('I');
    }
    public void sendJ()
    {
        sendToSeed('J');
    }
    public void sendK()
    {
        sendToSeed('K');
    }
    public void sendL()
    {
        sendToSeed('L');
    }
    public void sendM()
    {
        sendToSeed('M');
    }
    public void sendN()
    {
        sendToSeed('N');
    }
    public void sendO()
    {
        sendToSeed('O');
    }
    public void sendP()
    {
        sendToSeed('P');
    }
    public void sendQ()
    {
        sendToSeed('Q');
    }
    public void sendR()
    {
        sendToSeed('R');
    }
    public void sendS()
    {
        sendToSeed('S');
    }
    public void sendT()
    {
        sendToSeed('T');
    }
    public void sendU()
    {
        sendToSeed('U');
    }
    public void sendV()
    {
        sendToSeed('V');
    }
    public void sendW()
    {
        sendToSeed('W');
    }
    public void sendX()
    {
        sendToSeed('X');
    }
    public void sendY()
    {
        sendToSeed('Y');
    }
    public void sendZ()
    {
        sendToSeed('Z');
    }
    public void send1()
    {
        sendToSeed('1');
    }
    public void send2()
    {
        sendToSeed('2');
    }
    public void send3()
    {
        sendToSeed('3');
    }
    public void send4()
    {
        sendToSeed('4');
    }
    public void send5()
    {
        sendToSeed('5');
    }
    public void send6()
    {
        sendToSeed('6');
    }
    public void send7()
    {
        sendToSeed('7');
    }
    public void send8()
    {
        sendToSeed('8');
    }
    public void send9()
    {
        sendToSeed('9');
    }
    public void send0()
    {
        sendToSeed('0');
    }
}
