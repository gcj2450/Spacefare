using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cleaner : MonoBehaviour
{
    // (Optional) Prevent non-singleton constructor use.
    //protected Cleaner() { }

    private bool[,,] visited;
    private (int, int, int) location;
    private (int, int, int) destination;
    private (int, int, int) lastLocation;

    private (float,float,float) sourceRotation;
    private (float, float, float) destRotation;
    private float rotStartTime;
    //private float rotLength;
    private float xVelocity = 0.0f;
    private float yVelocity = 0.0f;
    private float zVelocity = 0.0f;


    private MapManager.RoomType[,,] m_blueprints;
    
    //private int interpolationFramesCount; // Number of frames to completely interpolate between the 2 positions
    //private int elapsedFrames;

    private bool isAwake;

    private float smoothTime;
    private Vector3 velocity;

    public AudioSource audioSource;
    public AudioClip thwackClip;
    public float volume = 1.0f;

    private bool toOrFro;

    void Awake()
    {
        //interpolationFramesCount = 60;
        //elapsedFrames = 0;
        isAwake = false;
        smoothTime = 2.5f;
        velocity = Vector3.zero;

        sourceRotation = (0.0f, 0.0f, 0.0f);
        destRotation   = (0.0f, 0.0f, 0.0f);
        //rotLength = 6.0f;

        toOrFro = true;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            // play a thwack sound
            audioSource.PlayOneShot(thwackClip, volume);

            // decrement the health counter
            HealthWatcher hpWatcher = GameObject.FindWithTag("HUD").GetComponent<HealthWatcher>();
            hpWatcher.subtractHealth(45, "The Cleaner");
            return;
        }
    }

    bool approximatelyThere()
    {
        float x = Mathf.Abs(transform.position.x - MapManager.getLoc(destination).x);
        float y = Mathf.Abs(transform.position.y - MapManager.getLoc(destination).y);
        float z = Mathf.Abs(transform.position.z - MapManager.getLoc(destination).z);

        double dist = Math.Sqrt(x * x + y * y + z * z);

        return (dist<1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAwake)
        {
            return;
        }

        //Debug.Log(getLoc(destination).ToString() + transform.position.ToString());

        if (approximatelyThere())
        {
            crawl();
        }



        /*
        Vector3 source = getLoc(location);
        Vector3 dest = getLoc(destination);

        if (elapsedFrames == interpolationFramesCount)
        {
            crawl();
        }

        float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
        Vector3 interpolatedPosition = Vector3.Lerp(source, dest, interpolationRatio);
        transform.position = interpolatedPosition;
        elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)
        */

        Vector3 dest = MapManager.getLoc(destination);
        transform.position = Vector3.SmoothDamp(transform.position, dest, ref velocity, smoothTime);

        /*
        float fracComplete = (Time.time - rotStartTime) / rotLength;
        transform.rotation = Quaternion.Slerp(sourceRotation, destRotation, fracComplete);
        */


        float newX = Mathf.SmoothDamp(transform.eulerAngles.x, destRotation.Item1, ref xVelocity, smoothTime);
        float newY = Mathf.SmoothDamp(transform.eulerAngles.y, destRotation.Item2, ref yVelocity, smoothTime);
        float newZ = Mathf.SmoothDamp(transform.eulerAngles.z, destRotation.Item3, ref zVelocity, smoothTime);
        transform.eulerAngles = new Vector3(newX, newY, newZ);
    }

    public void execute()
    {
        m_blueprints = MapManager.blueprints;
      
        int size = m_blueprints.GetLength(0);

        // reset visited
        visited = new bool[size, size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    visited[x, y, z] = false;
                }
            }
        }

        // determine the starting location
        location = MapManager.findStartRoom();
        destination = location;

        // tp to the starting location
        transform.position = MapManager.getLoc(location);

        // grow in size if we're in level 2
        //transform.localScale = new Vector3(2, 2, 2);

        // awaken
        isAwake = true;
    }


    // crawl sets the destination and the location
    // and it sets the rotation source and dest
    private void crawl()
    {
        MapManager.RoomType thisRoom = MapManager.blueprints[destination.Item1, destination.Item2, destination.Item3];


        (float,float,float) temp = sourceRotation;
        sourceRotation = destRotation;

        float driftX = UnityEngine.Random.value * 180;
        float driftY = UnityEngine.Random.value * 540 - 270;
        float driftZ = UnityEngine.Random.value * 720 + 720;

        driftX = 45;
        driftY = 180;
        driftZ = 1600;

        //optionally flip y
        if(toOrFro)
        {
            driftY *= -1;
        }
        toOrFro = !toOrFro;

        // build the dest rotation
        destRotation = (driftX, driftY, driftZ);
        rotStartTime = Time.time;

        if (!thisRoom.back & !thisRoom.forth & !thisRoom.left & !thisRoom.right & !thisRoom.up & !thisRoom.down)
        {
            // we've ended up outside
            // so let's go back from whence we came
            (int, int, int) tempDest = destination;
            destination = location;
            location = tempDest;
            return;
        }

        location = destination;


        bool isSearching = true;
        while(isSearching)
        {
            int choice = Mathf.RoundToInt((UnityEngine.Random.value * 6) - 0.5f);
            MapManager.RoomType nextRoom = m_blueprints[destination.Item1, destination.Item2, destination.Item3];
            switch (choice)
            {
                case 0:
                    if (!thisRoom.back)
                    {
                        continue;
                    }
                    nextRoom = m_blueprints[destination.Item1 - 1, destination.Item2, destination.Item3];
                    if (nextRoom.forth || nextRoom.isNullRoom())
                    {
                        destination = (location.Item1 - 1, location.Item2, location.Item3);
                        isSearching = false;
                    }
                    break;
                case 1:
                    if (!thisRoom.forth)
                    {
                        continue;
                    }
                    nextRoom = m_blueprints[location.Item1 + 1, location.Item2, location.Item3];
                    if (nextRoom.back || nextRoom.isNullRoom())
                    {
                        destination = (location.Item1 + 1, location.Item2, location.Item3);
                        isSearching = false;
                    }
                    break;
                case 2:
                    if (!thisRoom.left)
                    {
                        continue;
                    }
                    nextRoom = m_blueprints[location.Item1, location.Item2, location.Item3 + 1];
                    if (nextRoom.right || nextRoom.isNullRoom())
                    {
                        destination = (location.Item1, location.Item2, location.Item3 + 1);
                        isSearching = false;
                    }
                    break;
                case 3:
                    if (!thisRoom.right)
                    {
                        continue;
                    }
                    nextRoom = m_blueprints[location.Item1, location.Item2, location.Item3 - 1];
                    if (nextRoom.left || nextRoom.isNullRoom())
                    {
                        destination = (location.Item1, location.Item2, location.Item3 - 1);
                        isSearching = false;
                    }
                    break;
                case 4:
                    if (!thisRoom.up)
                    {
                        continue;
                    }
                    nextRoom = m_blueprints[location.Item1, location.Item2 + 1, location.Item3];
                    if (nextRoom.down || nextRoom.isNullRoom())
                    {
                        destination = (location.Item1, location.Item2 + 1, location.Item3);
                        isSearching = false;
                    }
                    break;
                case 5:
                    if (!thisRoom.down)
                    {
                        continue;
                    }
                    nextRoom = m_blueprints[location.Item1, location.Item2 - 1, location.Item3];
                    if (nextRoom.up || nextRoom.isNullRoom())
                    {
                        destination = (location.Item1, location.Item2 - 1, location.Item3);
                        isSearching = false;
                    }
                    break;
            }
        }
    }

    public void tagged()
    {
        execute();
    }
}

