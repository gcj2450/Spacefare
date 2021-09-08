using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTRoomManager : MonoBehaviour
{
    public bool isActivated;
    // Start is called before the first frame update
    void Start()
    {
        isActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (!isActivated)
        {
            TimeTrialManager.Instance.numRoomsCompleted++;
            isActivated = true;
        }
    }
}
