using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTarget : MonoBehaviour
{
    public Material inactiveMaterial;
    public Material resetMaterial;
    public Material hitMaterial;

    public AudioSource audioSource;
    public AudioClip clip;
    public float volume = 0.5f;

    public bool isActivated;
    private bool isBossAwake;

    void Awake()
    {
        reset();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Bullet" && !isActivated && isBossAwake)
        {
            // change the color
            GetComponent<MeshRenderer>().material = hitMaterial;
           
            // play a sound
            audioSource.PlayOneShot(clip, volume);

            // decrement boss health
            transform.parent.gameObject.GetComponent<AttackDrone>().decrementHealth();

            // make sure we only do this once
            isActivated = true;
        }
        return;
    }

    public void reset()
    {
        isBossAwake = false;
        isActivated = false;
        GetComponent<MeshRenderer>().material = resetMaterial;
    }

    public void powerDown()
    {
        GetComponent<MeshRenderer>().material = inactiveMaterial;
        return;
    }

    public void powerUp()
    {
        reset();
        isBossAwake = true;
    }
}
