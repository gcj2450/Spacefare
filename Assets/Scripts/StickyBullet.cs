using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBullet : MonoBehaviour
{
    private Vector3 growthRate = new Vector3(0.001f, 0.001f, 0.001f);
    private bool isStuck = false;
    private bool isGrown = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGrown)
        {
            // grow 5 times our size
            Transform thisParent = transform.parent;
            transform.parent = null;
            if (transform.localScale.x < 0.15)
            {
                transform.localScale += growthRate;
            }
            else
            {
                isGrown = true;
            }
            transform.parent = thisParent;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (!isStuck && col.collider.tag != "Player" && col.collider.tag != "Bullet" && col.collider.tag != "GlowBullet")
        {
            isStuck = true;
            var joint = gameObject.AddComponent<FixedJoint>();
            if (col.rigidbody != null)
            {
                transform.SetParent(col.collider.transform.root);
                joint.connectedBody = col.rigidbody;
            }
        }
    }

    public void Unstick()
    {
        transform.SetParent(null);
        Destroy(gameObject.GetComponent<FixedJoint>());
        isStuck = false;
    }
}
