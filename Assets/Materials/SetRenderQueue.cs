using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Rendering/SetRenderQueue")]

public class SetRenderQueue : MonoBehaviour
{
    [SerializeField]
    protected int queueNum;

    [SerializeField]
    protected int[] m_queues = new int[] { 3000 };

    protected void Update()
    {
        Material materials = GetComponent<Renderer>().material;
        Debug.Log("setting material");
        materials.renderQueue = queueNum; // m_queues[i];
        //for ( int i=0; i<materials.Length && 1 < m_queues.Length; i++)
        {
        }
    }
}
