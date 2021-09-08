using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    private int glowballAmmoCount;
    private int phaserAmmoCount;
    private int shotsTaken;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void reset()
    {
        glowballAmmoCount = 0;
        phaserAmmoCount = 0;
        shotsTaken = 0;
    }

    public int getGlowballAmmo()
    {
        return glowballAmmoCount;
    }
    public int getPhaserAmmo()
    {
        return phaserAmmoCount;
    }
    public int getShotsTaken()
    {
        return shotsTaken;
    }
    public void incShotsTaken()
    {
        shotsTaken++;
    }

    public void updateGlowballAmmo()
    {
        var ammoNotes = GameObject.FindGameObjectsWithTag("Ammo");
        foreach (GameObject ammoNote in ammoNotes)
        {
            ammoNote.GetComponent<UnityEngine.UI.Text>().text = glowballAmmoCount.ToString();
        }
    }
    public void addGlowballAmmo(int amount)
    {
        glowballAmmoCount += amount;
        return;
    }
    public void setGlowballAmmo(int amount)
    {
        glowballAmmoCount = amount;
        var ammoNotes = GameObject.FindGameObjectsWithTag("Ammo");
        foreach (GameObject ammoNote in ammoNotes)
        {
            ammoNote.GetComponent<UnityEngine.UI.Text>().text = glowballAmmoCount.ToString();
        }
        return;
    }
    public void useGlowballAmmo()
    {
        glowballAmmoCount--;
        var ammoNotes = GameObject.FindGameObjectsWithTag("Ammo");
        foreach (GameObject ammoNote in ammoNotes)
        {
            ammoNote.GetComponent<UnityEngine.UI.Text>().text = glowballAmmoCount.ToString();
        }
        return;
    }

    public void addPhaserAmmo(int amount)
    {
        phaserAmmoCount += amount;
        //GameObject.FindWithTag("Ammo").GetComponent<UnityEngine.UI.Text>().text = glowballAmmoCount.ToString();
        return;
    }
    public void setPhaserAmmo(int amount)
    {
        phaserAmmoCount = amount;
        if (GameObject.FindWithTag("Ammo"))
        {
        //    GameObject.FindWithTag("Ammo").GetComponent<UnityEngine.UI.Text>().text = glowballAmmoCount.ToString();
        }
        return;
    }
    public void usePhaserAmmo()
    {
        phaserAmmoCount--;
        //GameObject.FindWithTag("Ammo").GetComponent<UnityEngine.UI.Text>().text = glowballAmmoCount.ToString();
        return;
    }
}
