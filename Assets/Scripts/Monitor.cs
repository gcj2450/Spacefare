using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Monitor : MonoBehaviour
{
    // (Optional) Prevent non-singleton constructor use.
    //protected Monitor() { }

    private (int, int, int) location;
    private (int, int, int) destination;
    private (int, int, int) lastLocation;

    private Quaternion destRotation;
    private float xVelocity = 0.0f;
    private float yVelocity = 0.0f;
    private float zVelocity = 0.0f;
    private float wVelocity = 0.0f;


    private MapManager.RoomType[,,] m_blueprints;


    private bool isAwake;

    private float smoothTime = 3.0f;
    private float facingSmoothTime = 0.4f;
    private Vector3 velocity;


    public AudioSource audioSource;
    public AudioClip clip;
    public AudioClip breathClip;
    public bool hasBreathed;

    private bool[,,] visited;
    public static Queue<(int, int, int)> roomsToSearch = new Queue<(int, int, int)>();

    private bool[,,] pathfindingVisited;
    public static Queue<(int, int, int)> pathfindingQueue = new Queue<(int, int, int)>();
    public static Stack<(int, int, int)> currentPath = new Stack<(int, int, int)>();
    int[,,] pfdist;
    (int, int, int)[,,] parents;

    void Awake()
    {
        isAwake = false;
        velocity = Vector3.zero;
        destRotation = Quaternion.identity;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            // play a thwack sound
            audioSource.PlayOneShot(clip, 0.5f);

            // decrement the health counter
            HealthWatcher hpWatcher = GameObject.FindWithTag("HUD").GetComponent<HealthWatcher>();
            hpWatcher.subtractHealth(100, "The Monitor");
            return;
        }
    }
    void resetVectors()
    {
        velocity = Vector3.zero;
        xVelocity = 0f;
        yVelocity = 0f;
        zVelocity = 0f;
        wVelocity = 0f;
    }

    void OnCollisionExit(Collision col)
    {
        /*
        // we may have been knocked off course
        if (col.collider.tag != "Bullet")
        {
            Invoke("resetVectors", 3.0f);
        }
        */
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && !hasBreathed)
        {
            audioSource.PlayOneShot(breathClip, 1.0f);
        }
        return;
    }
    void getReadyToBreathe()
    {
        hasBreathed = false;
    }
    void OnTriggerExit(Collider col)
    {
        // ensure we don't breathe again for 30 seconds
        hasBreathed = true;
        Invoke("getReadyToBreathe", 30.0f);
    }

    bool approximatelyThere()
    {
        float x = Mathf.Abs(transform.position.x - MapManager.getLoc(destination).x);
        float y = Mathf.Abs(transform.position.y - MapManager.getLoc(destination).y);
        float z = Mathf.Abs(transform.position.z - MapManager.getLoc(destination).z);

        double dist = Math.Sqrt(x * x + y * y + z * z);

        return (dist < 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAwake)
        {
            return;
        }

        if (approximatelyThere())
        {
            crawl();
        }

        Vector3 dest = MapManager.getLoc(destination);
        transform.position = Vector3.SmoothDamp(transform.position, dest, ref velocity, smoothTime);

        // x has a different smoothtime so the about-face happens quickly but leaves a lingering roll
        float newX = Mathf.SmoothDamp(transform.rotation.x, destRotation.x, ref xVelocity, facingSmoothTime);
        float newY = Mathf.SmoothDamp(transform.rotation.y, destRotation.y, ref yVelocity, facingSmoothTime);
        float newZ = Mathf.SmoothDamp(transform.rotation.z, destRotation.z, ref zVelocity, facingSmoothTime);
        float newW = Mathf.SmoothDamp(transform.rotation.w, destRotation.w, ref wVelocity, facingSmoothTime);

        transform.rotation = new Quaternion(newX, newY, newZ, newW);

    }

    public void execute()
    {
        // get the blueprints
        m_blueprints = MapManager.blueprints;
        int size = m_blueprints.GetLength(0);

        // reset visited
        visited = new bool[size, size, size];
        pathfindingVisited = new bool[size, size, size];
        pfdist = new int[size, size, size];
        parents = new (int, int, int)[size, size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    visited[x, y, z] = false;
                    pathfindingVisited[x, y, z] = false;
                    pfdist[x, y, z] = 999999;
                    parents[x, y, z] = (-1,-1,-1);
                }
            }
        }

        // determine the starting location
        location = MapManager.findStartRoom();
        destination = location;

        // tp to the starting location
        transform.position = MapManager.getLoc(location);

        // plan our route
        planRoute();

        // awaken
        isAwake = true;
    }

    // crawl sets the destination and the location
    // and it sets the rotation source and dest
    private void crawl()
    {
        MapManager.RoomType thisRoom = MapManager.blueprints[destination.Item1, destination.Item2, destination.Item3];

        location = destination;

        // TODO: set destination here
        while (roomsToSearch.Count == 0)
        {
            planRoute();
        }
        while (currentPath.Count == 0)
        {
            goToRoom(roomsToSearch.Dequeue());
        }
        
        destination = currentPath.Pop();

        int x = destination.Item1 - location.Item1;
        int y = destination.Item2 - location.Item2;
        int z = destination.Item3 - location.Item3;
        Vector3 forward = new Vector3(x, y, z);
        Vector3 up = new Vector3(0, 0, 0);
        destRotation.SetLookRotation(forward, up);
    }

    void initVisited(bool[,,] thisVisited)
    {
        // read the map
        MapManager.RoomType[,,] blueprints = MapManager.blueprints;
        // one dimension of a cube
        int size = blueprints.GetLength(0);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    MapManager.RoomType thisRoom = blueprints[x, y, z];
                    if (thisRoom.back || thisRoom.forth || thisRoom.left || thisRoom.right || thisRoom.up || thisRoom.down)
                    {
                        thisVisited[x, y, z] = false;
                    }
                    else
                    {
                        // we don't care to go to null rooms, so we'll call them visited
                        thisVisited[x, y, z] = true;
                    }
                }
            }
        }
    }

    void sniffRoom( (int,int,int) room )
    {
        MapManager.RoomType thisRoom = m_blueprints[room.Item1, room.Item2, room.Item3];

        if (thisRoom.back)
        {
            (int, int, int) nextPotential = (room.Item1 - 1, room.Item2, room.Item3);
            if (!visited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
            {
                roomsToSearch.Enqueue(nextPotential);
                visited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = true;
                sniffRoom(nextPotential);
            }
        }
        if (thisRoom.forth)
        {
            (int, int, int) nextPotential = (room.Item1 + 1, room.Item2, room.Item3);
            if (!visited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
            {
                roomsToSearch.Enqueue(nextPotential);
                visited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = true;
                sniffRoom(nextPotential);
            }
        }
        if (thisRoom.left)
        {
            (int, int, int) nextPotential = (room.Item1, room.Item2, room.Item3 + 1);
            if (!visited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
            {
                roomsToSearch.Enqueue(nextPotential);
                visited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = true;
                sniffRoom(nextPotential);
            }
        }
        if (thisRoom.right)
        {
            (int, int, int) nextPotential = (room.Item1, room.Item2, room.Item3 - 1);
            if (!visited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
            {
                roomsToSearch.Enqueue(nextPotential);
                visited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = true;
                sniffRoom(nextPotential);
            }
        }
        if (thisRoom.up)
        {
            (int, int, int) nextPotential = (room.Item1, room.Item2 + 1, room.Item3);
            if (!visited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
            {
                roomsToSearch.Enqueue(nextPotential);
                visited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = true;
                sniffRoom(nextPotential);
            }
        }
        if (thisRoom.down)
        {
            (int, int, int) nextPotential = (room.Item1, room.Item2 - 1, room.Item3);
            if (!visited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
            {
                roomsToSearch.Enqueue(nextPotential);
                visited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = true;
                sniffRoom(nextPotential);
            }
        }
    }

    void planRoute()
    {
        initVisited(visited);
        // perform a depth first search
        sniffRoom(location);
    }

    bool goToRoomHelper((int,int,int) destinationRoom)
    {
        if(pathfindingQueue.Count==0)
        {
            return false;
        }

        (int, int, int) room = pathfindingQueue.Dequeue();
        if(room == destinationRoom)
        {
            return true;
        }

        MapManager.RoomType thisRoom = m_blueprints[room.Item1, room.Item2, room.Item3];

        if (thisRoom.back)
        {
            (int, int, int) nextPotential = (room.Item1 - 1, room.Item2, room.Item3);
            if (!pathfindingVisited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
            {
                if(m_blueprints[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3].forth)
                {
                    pathfindingQueue.Enqueue(nextPotential);
                    int potentialDist = pfdist[room.Item1, room.Item2, room.Item3] + 1;
                    if (potentialDist < pfdist[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
                    {
                        pfdist[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = potentialDist;
                        parents[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = room;
                    }
                    pathfindingVisited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = true;
                }
            }
        }
        if (thisRoom.forth)
        {
            (int, int, int) nextPotential = (room.Item1 + 1, room.Item2, room.Item3);
            if (!pathfindingVisited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
            {
                if (m_blueprints[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3].back)
                {
                    pathfindingQueue.Enqueue(nextPotential);
                    int potentialDist = pfdist[room.Item1, room.Item2, room.Item3] + 1;
                    if (potentialDist < pfdist[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
                    {
                        pfdist[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = potentialDist;
                        parents[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = room;
                    }
                    pathfindingVisited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = true;
                }
            }
        }
        if (thisRoom.left)
        {
            (int, int, int) nextPotential = (room.Item1, room.Item2, room.Item3 + 1);
            if (!pathfindingVisited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
            {
                if (m_blueprints[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3].right)
                {
                    pathfindingQueue.Enqueue(nextPotential);
                    int potentialDist = pfdist[room.Item1, room.Item2, room.Item3] + 1;
                    if (potentialDist < pfdist[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
                    {
                        pfdist[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = potentialDist;
                        parents[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = room;
                    }
                    pathfindingVisited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = true;
                }
            }
        }
        if (thisRoom.right)
        {
            (int, int, int) nextPotential = (room.Item1, room.Item2, room.Item3 - 1);
            if (!pathfindingVisited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
            {
                if (m_blueprints[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3].left)
                {
                    pathfindingQueue.Enqueue(nextPotential);
                    int potentialDist = pfdist[room.Item1, room.Item2, room.Item3] + 1;
                    if (potentialDist < pfdist[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
                    {
                        pfdist[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = potentialDist;
                        parents[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = room;
                    }
                    pathfindingVisited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = true;
                }
            }
        }
        if (thisRoom.up)
        {
            (int, int, int) nextPotential = (room.Item1, room.Item2 + 1, room.Item3);
            if (!pathfindingVisited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
            {
                if (m_blueprints[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3].down)
                {
                    pathfindingQueue.Enqueue(nextPotential);
                    int potentialDist = pfdist[room.Item1, room.Item2, room.Item3] + 1;
                    if (potentialDist < pfdist[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
                    {
                        pfdist[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = potentialDist;
                        parents[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = room;
                    }
                    pathfindingVisited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = true;
                }
            }
        }
        if (thisRoom.down)
        {
            (int, int, int) nextPotential = (room.Item1, room.Item2 - 1, room.Item3);
            if (!pathfindingVisited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
            {
                if (m_blueprints[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3].up)
                {
                    pathfindingQueue.Enqueue(nextPotential);
                    int potentialDist = pfdist[room.Item1, room.Item2, room.Item3] + 1;
                    if (potentialDist < pfdist[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3])
                    {
                        pfdist[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = potentialDist;
                        parents[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = room;
                    }
                    pathfindingVisited[nextPotential.Item1, nextPotential.Item2, nextPotential.Item3] = true;
                }
            }
        }
        return(goToRoomHelper(destinationRoom));
    }

    void goToRoom( (int,int,int) destinationRoom )
    {
        initVisited(pathfindingVisited);
        MapManager.RoomType[,,] blueprints = MapManager.blueprints;
        // one dimension of a cube
        int size = blueprints.GetLength(0);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    pathfindingVisited[x, y, z] = false;
                    pfdist[x, y, z] = 999999; // "infinite"
                    parents[x, y, z] = (-1, -1, -1);
                }
            }
        }

        pfdist[location.Item1, location.Item2, location.Item3] = 0;

        pathfindingQueue = new Queue<(int, int, int)>();
        pathfindingQueue.Enqueue(location);
        bool roomFound = goToRoomHelper(destinationRoom);
        if(roomFound)
        {
            // reconstruct path
            (int, int, int) tempRoom = destinationRoom;
            while(tempRoom != location)
            {
                currentPath.Push(tempRoom);
                tempRoom = parents[tempRoom.Item1, tempRoom.Item2, tempRoom.Item3];
            }
        }
        else
        {
            Debug.Log("pathfinding failed");
        }
        return;
    }

    public void tagged()
    {
        execute();
    }
}

