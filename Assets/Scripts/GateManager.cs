using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GateManager : Singleton<GateManager>
{
    // (Optional) Prevent non-singleton constructor use.
    protected GateManager() { }
    private bool[,,] visited;

    public struct KeyInfo
    {   
        public Color color;
        public bool isPlanted;
        public bool hasDoor;
        public (int, int, int) location;
        public bool isNullKey;
    }

    public KeyInfo[] keyList;

    void Awake()
    {
        // init keylist
        keyList = new KeyInfo[GameManager.mapFuel];
        for (int i = 0; i < GameManager.mapFuel; i++)
        {
            KeyInfo thisKey;
            thisKey.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            thisKey.isPlanted = false;
            thisKey.hasDoor = false;
            thisKey.location = (-1, -1, -1);
            thisKey.isNullKey = false;
            keyList[i] = thisKey;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private (int, int, int) getLoc((int, int, int) blue)
    {
        return (blue.Item1 * 48, blue.Item2 * 48, blue.Item3 * 48);
    }

    private KeyInfo getKeyToPlant()
    {
        for(int i=0; i<keyList.Length; i++)
        {
            var thisKey = keyList[i];
            if (!thisKey.isPlanted)
            {
                thisKey.isPlanted = true;
                keyList[i] = thisKey;
                return thisKey;
            }
        }
        KeyInfo nullKey;
        nullKey.color = Color.red;
        nullKey.isPlanted = false;
        nullKey.hasDoor = false;
        nullKey.location = (-1, -1, -1);
        nullKey.isNullKey = true;
        return (nullKey);
    }
    private void plantKey((int, int, int) room)
    {

        MapManager.RoomType[,,] blueprints = MapManager.blueprints;

        MapManager.RoomType thisRoom = blueprints[room.Item1, room.Item2, room.Item3];
        (int, int, int) thisLoc = getLoc(room);
        Vector3 windowCoords = Vector3.zero;
        Quaternion windowRot = Quaternion.identity;

        if (UnityEngine.Random.value > 0.5f)
        {
            KeyInfo thisKey = getKeyToPlant();
            if (thisKey.isNullKey)
            {
                return;
            }

            float scatterX = UnityEngine.Random.value * 4.0f - 8.0f;
            float scatterY = UnityEngine.Random.value * 4.0f - 8.0f;
            float scatterZ = UnityEngine.Random.value * 4.0f - 8.0f;
            windowCoords = new Vector3(thisLoc.Item1 + scatterX, thisLoc.Item2 + scatterY, thisLoc.Item3 + scatterZ);

            var key = Resources.Load("Crawler/GateKey");
            GameObject plantedKey = Instantiate(key, windowCoords, windowRot) as GameObject;
            plantedKey.transform.Find("KeyBody").GetComponent<GateKey>().color = thisKey.color;
            plantedKey.transform.Find("KeyBody").GetComponent<Renderer>().material.color = thisKey.color;
        }
        return;
    }

    private KeyInfo getKeyForDoor()
    {
        for (int i = 0; i < keyList.Length; i++)
        {
            var thisKey = keyList[i];
            if (!thisKey.hasDoor && thisKey.isPlanted)
            {
                thisKey.hasDoor = true;
                keyList[i] = thisKey;
                return thisKey;
            }
        }
        KeyInfo nullKey;
        nullKey.color = Color.red;
        nullKey.isPlanted = false;
        nullKey.hasDoor = false;
        nullKey.location = (-1, -1, -1);
        nullKey.isNullKey = true;
        return (nullKey);
    }
    private void plantGate((int, int, int) room)
    {

        MapManager.RoomType[,,] blueprints = MapManager.blueprints;

        MapManager.RoomType thisRoom = blueprints[room.Item1, room.Item2, room.Item3];
        (int, int, int) thisLoc = getLoc(room);
        Vector3 windowCoords = Vector3.zero;
        Quaternion windowRot = Quaternion.identity;
        int choice = 0;

        if (UnityEngine.Random.value > 0.5f)
        {
            // plant gate half the time
            KeyInfo thisKey = getKeyForDoor();
            if (thisKey.isNullKey)
            {
                return;
            }

            bool isSearching = true;
            while (isSearching)
            {
                choice = UnityEngine.Random.Range((int)0, (int)6);
                switch (choice)
                {

                    case 0:
                        if (thisRoom.back)
                        {
                            windowCoords = new Vector3(thisLoc.Item1 - 16, thisLoc.Item2, thisLoc.Item3);
                            windowRot = Quaternion.Euler(0, 90, 0);
                            isSearching = false;
                        }
                        break;
                    case 1:
                        if (thisRoom.forth)
                        {
                            windowCoords = new Vector3(thisLoc.Item1 + 16, thisLoc.Item2, thisLoc.Item3);
                            windowRot = Quaternion.Euler(0, -90, 0);
                            isSearching = false;
                        }
                        break;
                    case 2:
                        if (thisRoom.left)
                        {
                            windowCoords = new Vector3(thisLoc.Item1, thisLoc.Item2, thisLoc.Item3 + 16);
                            windowRot = Quaternion.Euler(0, 180, 0);
                            isSearching = false;
                        }
                        break;
                    case 3:
                        if (thisRoom.right)
                        {
                            windowCoords = new Vector3(thisLoc.Item1, thisLoc.Item2, thisLoc.Item3 - 16);
                            windowRot = Quaternion.Euler(0, 0, 0);
                            isSearching = false;
                        }
                        break;
                    case 4:
                        if (thisRoom.up)
                        {
                            windowCoords = new Vector3(thisLoc.Item1, thisLoc.Item2 + 16, thisLoc.Item3);
                            windowRot = Quaternion.Euler(90, 0, 0);
                            isSearching = false;
                        }
                        break;
                    case 5:
                        if (thisRoom.down)
                        {
                            windowCoords = new Vector3(thisLoc.Item1, thisLoc.Item2 - 16, thisLoc.Item3);
                            windowRot = Quaternion.Euler(-90, 0, 0);
                            isSearching = false;
                        }
                        break;
                }
            }
            var window = Resources.Load("Crawler/RoomGate");
            GameObject plantedGate = Instantiate(window, windowCoords, windowRot) as GameObject; ;
            plantedGate.GetComponent<GateMechanism>().color = thisKey.color;
            plantedGate.transform.Find("Body/outsideJoint").GetComponent<Renderer>().material.color = thisKey.color;
            plantedGate.transform.Find("Body/insideJoint").GetComponent<Renderer>().material.color = thisKey.color;
        }
        return;
    }

    private void crawl((int, int, int) room)
    {
        if (visited[room.Item1, room.Item2, room.Item3])
        {
            return;
        }

        // mark visited
        visited[room.Item1, room.Item2, room.Item3] = true;

        MapManager.RoomType[,,] blueprints = MapManager.blueprints;
        MapManager.RoomType thisRoom = blueprints[room.Item1, room.Item2, room.Item3];

        if (thisRoom.back || thisRoom.forth || thisRoom.left || thisRoom.right || thisRoom.up || thisRoom.down)
        {
            plantGate(room);
            plantKey(room);
        }


        if (thisRoom.back)
        {
            (int, int, int) thisLoc = (room.Item1 - 1, room.Item2, room.Item3);
            crawl(thisLoc);
        }
        if (thisRoom.forth)
        {
            (int, int, int) thisLoc = (room.Item1 + 1, room.Item2, room.Item3);
            crawl(thisLoc);
        }
        if (thisRoom.left)
        {
            (int, int, int) thisLoc = (room.Item1, room.Item2, room.Item3 + 1);
            crawl(thisLoc);
        }
        if (thisRoom.right)
        {
            (int, int, int) thisLoc = (room.Item1, room.Item2, room.Item3 - 1);
            crawl(thisLoc);
        }
        if (thisRoom.up)
        {
            (int, int, int) thisLoc = (room.Item1, room.Item2 + 1, room.Item3);
            crawl(thisLoc);
        }
        if (thisRoom.down)
        {
            (int, int, int) thisLoc = (room.Item1, room.Item2 - 1, room.Item3);
            crawl(thisLoc);
        }

        return;
    }

    public void execute()
    {
        // get the blueprints
        MapManager.RoomType[,,] blueprints = MapManager.blueprints;
        int size = blueprints.GetLength(0);

        // reset visited
        initVisited();

        (int, int, int) start = ((size + 1) / 2, (size + 1) / 2, (size + 1) / 2);

        crawl(start);

    }

    void initVisited()
    {
        // read the map
        MapManager.RoomType[,,] blueprints = MapManager.blueprints;
        // one dimension of a cube
        int size = blueprints.GetLength(0);
        visited = new bool[size, size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    MapManager.RoomType thisRoom = blueprints[x, y, z];
                    if (thisRoom.isNullRoom())
                    {
                        visited[x, y, z] = true;
                    }
                    else
                    {
                        // we don't care to go to null rooms, so we'll call them visited
                        visited[x, y, z] = false;
                    }
                }
            }
        }
    }

}
