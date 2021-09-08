using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyManager : MonoBehaviour
{
    public Color[] myKeys;
    private int numKeysHeld;
    private List<GameObject> inventoryKeys;

    // Start is called before the first frame update
    void Awake()
    {
        inventoryKeys = new List<GameObject> { };
    }
    void Start()
    {
        myKeys = new Color[100];
        numKeysHeld = 0;
    }

    public void addKey(Color givenKey)
    {
        myKeys[numKeysHeld] = givenKey;
        numKeysHeld++;

        var thisKey = Resources.Load("KeyIcon");
        GameObject keyObj = Instantiate(thisKey, Vector3.zero, Quaternion.identity) as GameObject;
        inventoryKeys.Add(keyObj);

        //print sprite at location i
        keyObj.transform.SetParent(GameObject.FindWithTag("Points").transform);
        keyObj.transform.localPosition = new Vector3(3 * numKeysHeld - 20, -10, 0);
        keyObj.transform.localRotation = Quaternion.identity;
        keyObj.transform.localScale = new Vector3(8, 8, 1);
        keyObj.GetComponent<Renderer>().material.color = givenKey;

        numKeysHeld++;
    }

    public void reset()
    {
        foreach(GameObject key in inventoryKeys)
        {
            Destroy(key);
        }
        inventoryKeys.Clear();
        return;
    }

    public bool hasColor(Color col)
    {
        foreach (GameObject key in inventoryKeys)
        {
            if(key.GetComponent<Renderer>().material.color == col)
            {
                return true;
            }
        }
        return false;
    }
}
