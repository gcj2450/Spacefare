using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
