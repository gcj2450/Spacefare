using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : Singleton<Crawler>
{
    // (Optional) Prevent non-singleton constructor use.
    protected Crawler() { }

    private MapManager.RoomType[,,] m_blueprints;

    public bool targetsEnabled;
    public bool minesEnabled;
    public bool windowsEnabled;
    public bool ammoEnabled;
    public bool anchorsEnabled;
    public bool keysAndGatesEnabled;
    public bool isOxygenEnabled;

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

    public void execute()
    {
        targetsEnabled = true;
        minesEnabled = GameManager.isHardcoreMode;
        windowsEnabled = true;
        ammoEnabled = true;
        anchorsEnabled = false;
        keysAndGatesEnabled = true;
        isOxygenEnabled = GameManager.isOxygenMode;

        // init keylist
        keyList = new KeyInfo[GameManager.mapFuel+5];
        for (int i = 0; i < GameManager.mapFuel+5; i++)
        {
            KeyInfo thisKey;
            thisKey.color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            thisKey.isPlanted = false;
            thisKey.hasDoor = false;
            thisKey.location = (-1, -1, -1);
            thisKey.isNullKey = false;
            keyList[i] = thisKey;
        }

        // get the blueprints
        m_blueprints = MapManager.blueprints;
        int size = m_blueprints.GetLength(0);

        // reset visited
        initVisited();

        (int, int, int) start = ((size + 1) / 2, (size + 1) / 2, (size + 1) / 2);

        crawl(start);

    }

    private void crawl((int, int, int) room)
    {
        if (visited[room.Item1, room.Item2, room.Item3])
        {
            return;
        }

        // mark visited
        visited[room.Item1, room.Item2, room.Item3] = true;

        MapManager.RoomType thisRoom = m_blueprints[room.Item1, room.Item2, room.Item3];

        if (thisRoom.back || thisRoom.forth || thisRoom.left || thisRoom.right || thisRoom.up || thisRoom.down)
        {
            if (targetsEnabled)
            {
                var target = Resources.Load("Crawler/TargetLevel2D");
                plantGenericOnWall(room, target, 3, true, false);
            }
            if (minesEnabled)
            {
                var pushMine = Resources.Load("Crawler/PushMine");
                plantGenericInSpace(room, pushMine, 3, 0);
                var spinMine = Resources.Load("Crawler/SpinMine");
                plantGenericInSpace(room, spinMine, 3, 0);
                //plantMines(room);
            }
            if (windowsEnabled)
            {
                //plant generic on wall?
                plantWindows(room);
            }
            if (ammoEnabled)
            {
                var ammo = Resources.Load("Crawler/AmmoCapsule");
                plantGenericInSpace(room, ammo, 1, 0.25f);
                //plantAmmo(room);
            }
            if (anchorsEnabled)
            {
                plantAnchors(room);
            }
            if (keysAndGatesEnabled)
            {
                plantGate(room);
                plantKey(room);
            }
            if (isOxygenEnabled)
            {
                var oBar = Resources.Load("Crawler/OxygenBar");
                plantGenericOnWall(room, oBar, 1, false, false);
            }
            if (true) //posters
            {
                var poster = Resources.Load("Crawler/Poster");
                plantGenericOnWall(room, poster, 12, false, true);
            }
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

    private (int, int, int) getLoc((int, int, int) blue)
    {
        return (blue.Item1 * 60, blue.Item2 * 60, blue.Item3 * 60);
    }
    void initVisited()
    {
        // one dimension of a cube
        int size = m_blueprints.GetLength(0);
        visited = new bool[size, size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    MapManager.RoomType thisRoom = m_blueprints[x, y, z];
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
    private void plantAnchors((int, int, int) room)
    {
        MapManager.RoomType thisRoom = m_blueprints[room.Item1, room.Item2, room.Item3];
        (int, int, int) thisLoc = getLoc(room);

        for (int i = 0; i < UnityEngine.Random.value * 3; i++)
        {
            // instantiate a anchor somewhere on a wall
            float scatterX = 0;
            float scatterY = 0;
            float scatterZ = 0;
            Quaternion anchorRotation = Quaternion.identity;
            bool isSearching = true;
            while (isSearching)
            {
                int choice = UnityEngine.Random.Range((int)0, (int)6);
                switch (choice)
                {
                    case 0:
                        if (thisRoom.back)
                        {
                            scatterX = (UnityEngine.Random.value * -7) - 2.5f;
                            if ((UnityEngine.Random.value > 0.5f)) // on a y-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    anchorRotation = Quaternion.Euler(0, 0, 0);
                                    scatterY = -1.99f;
                                }
                                else
                                {
                                    anchorRotation = Quaternion.Euler(180, 0, 0);
                                    scatterY = 1.99f;
                                }
                                scatterZ = UnityEngine.Random.value * 1.75f;
                            }
                            else // on a z-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    anchorRotation = Quaternion.Euler(90, 0, 0);
                                    scatterZ = -1.99f;
                                }
                                else
                                {
                                    anchorRotation = Quaternion.Euler(-90, 0, 0);
                                    scatterZ = 1.99f;
                                }
                                scatterY = UnityEngine.Random.value * 1.75f;

                            }
                            isSearching = false;
                        }
                        break;
                    case 1:
                        if (thisRoom.forth)
                        {
                            scatterX = (UnityEngine.Random.value * 7) + 2.5f;
                            if ((UnityEngine.Random.value > 0.5f)) // on a y-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    anchorRotation = Quaternion.Euler(0, 0, 0);
                                    scatterY = -1.99f;
                                }
                                else
                                {
                                    anchorRotation = Quaternion.Euler(180, 0, 0);
                                    scatterY = 1.99f;
                                }
                                scatterZ = UnityEngine.Random.value * 1.75f;
                            }
                            else // on a z-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    anchorRotation = Quaternion.Euler(90, 0, 0);
                                    scatterZ = -1.99f;
                                }
                                else
                                {
                                    anchorRotation = Quaternion.Euler(-90, 0, 0);
                                    scatterZ = 1.99f;
                                }
                                scatterY = UnityEngine.Random.value * 1.75f;

                            }
                            isSearching = false;
                        }
                        break;
                    case 2:
                        if (thisRoom.left)
                        {
                            scatterZ = (UnityEngine.Random.value * 7) + 2.5f;
                            if ((UnityEngine.Random.value > 0.5f)) // on a y-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    anchorRotation = Quaternion.Euler(0, 0, 0);
                                    scatterY = -1.99f;
                                }
                                else
                                {
                                    anchorRotation = Quaternion.Euler(180, 0, 0);
                                    scatterY = 1.99f;
                                }
                                scatterX = UnityEngine.Random.value * 1.75f;
                            }
                            else // on a x-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    anchorRotation = Quaternion.Euler(0, 0, -90);
                                    scatterX = -1.99f;
                                }
                                else
                                {
                                    anchorRotation = Quaternion.Euler(0, 0, 90);
                                    scatterX = 1.99f;
                                }
                                scatterY = UnityEngine.Random.value * 1.75f;

                            }
                            isSearching = false;
                        }
                        break;
                    case 3:
                        if (thisRoom.right)
                        {
                            scatterZ = (UnityEngine.Random.value * -7) - 2.5f;
                            if ((UnityEngine.Random.value > 0.5f)) // on a y-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    anchorRotation = Quaternion.Euler(0, 0, 0);
                                    scatterY = -1.99f;
                                }
                                else
                                {
                                    anchorRotation = Quaternion.Euler(180, 0, 0);
                                    scatterY = 1.99f;
                                }
                                scatterX = UnityEngine.Random.value * 1.75f;
                            }
                            else // on a x-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    anchorRotation = Quaternion.Euler(0, 0, -90);
                                    scatterX = -1.99f;
                                }
                                else
                                {
                                    anchorRotation = Quaternion.Euler(0, 0, 90);
                                    scatterX = 1.99f;
                                }
                                scatterY = UnityEngine.Random.value * 1.75f;

                            }
                            isSearching = false;
                        }
                        break;
                    case 4:
                        if (thisRoom.up)
                        {
                            scatterY = (UnityEngine.Random.value * 7) + 2.5f;
                            if ((UnityEngine.Random.value > 0.5f)) // on a x-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    anchorRotation = Quaternion.Euler(0, 0, -90);
                                    scatterX = -1.99f;
                                }
                                else
                                {
                                    anchorRotation = Quaternion.Euler(0, 0, 90);
                                    scatterX = 1.99f;
                                }
                                scatterZ = UnityEngine.Random.value * 1.75f;
                            }
                            else // on a z-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    anchorRotation = Quaternion.Euler(90, 0, 0);
                                    scatterZ = -1.99f;
                                }
                                else
                                {
                                    anchorRotation = Quaternion.Euler(-90, 0, 0);
                                    scatterZ = 1.99f;
                                }
                                scatterX = UnityEngine.Random.value * 1.75f;

                            }
                            isSearching = false;
                        }
                        break;
                    case 5:
                        if (thisRoom.down)
                        {
                            scatterY = (UnityEngine.Random.value * -7) - 2.5f;
                            if ((UnityEngine.Random.value > 0.5f)) // on a x-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    anchorRotation = Quaternion.Euler(0, 0, -90);
                                    scatterX = -1.99f;
                                }
                                else
                                {
                                    anchorRotation = Quaternion.Euler(0, 0, 90);
                                    scatterX = 1.99f;
                                }
                                scatterZ = UnityEngine.Random.value * 1.75f;
                            }
                            else // on a z-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    anchorRotation = Quaternion.Euler(90, 0, 0);
                                    scatterZ = -1.99f;
                                }
                                else
                                {
                                    anchorRotation = Quaternion.Euler(-90, 0, 0);
                                    scatterZ = 1.99f;
                                }
                                scatterX = UnityEngine.Random.value * 1.75f;

                            }
                            isSearching = false;
                        }
                        break;
                }
            }

            Vector3 anchorCoords = new Vector3(thisLoc.Item1 + scatterX, thisLoc.Item2 + scatterY, thisLoc.Item3 + scatterZ);

            var anchor = Resources.Load("Crawler/AnchorBar");
            Instantiate(anchor, anchorCoords, anchorRotation);
        }

        return;

    }
    private void plantWindows((int, int, int) room)
    {
        MapManager.RoomType thisRoom = m_blueprints[room.Item1, room.Item2, room.Item3];
        (int, int, int) thisLoc = getLoc(room);
        Vector3 windowCoords = Vector3.zero;
        Quaternion windowRot = Quaternion.identity;
        int choice = UnityEngine.Random.Range((int)0, (int)6);
        // plant windows half the time
        if (UnityEngine.Random.value > 0.5f)
        {
            switch (choice)
            {
                case 0:
                    windowCoords = new Vector3(thisLoc.Item1 - 10, thisLoc.Item2, thisLoc.Item3);
                    windowRot = Quaternion.Euler(0, 90, 0);
                    break;
                case 1:
                    windowCoords = new Vector3(thisLoc.Item1 + 10, thisLoc.Item2, thisLoc.Item3);
                    windowRot = Quaternion.Euler(0, -90, 0);
                    break;
                case 2:
                    windowCoords = new Vector3(thisLoc.Item1, thisLoc.Item2, thisLoc.Item3 + 10);
                    windowRot = Quaternion.Euler(0, 180, 0);
                    break;
                case 3:
                    windowCoords = new Vector3(thisLoc.Item1, thisLoc.Item2, thisLoc.Item3 - 10);
                    windowRot = Quaternion.Euler(0, 0, 0);
                    break;
                case 4:
                    windowCoords = new Vector3(thisLoc.Item1, thisLoc.Item2 + 10, thisLoc.Item3);
                    windowRot = Quaternion.Euler(90, 0, 0);
                    break;
                case 5:
                    windowCoords = new Vector3(thisLoc.Item1, thisLoc.Item2 - 10, thisLoc.Item3);
                    windowRot = Quaternion.Euler(-90, 0, 0);
                    break;
            }
            var window = Resources.Load("NewPaths/WindowArray");
            Instantiate(window, windowCoords, windowRot);
        }
        return;
    }

    private KeyInfo getKeyToPlant()
    {
        for (int i = 0; i < keyList.Length; i++)
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


        MapManager.RoomType thisRoom = m_blueprints[room.Item1, room.Item2, room.Item3];
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
            
            Vector3 initialTorque = new Vector3(45, 60, 90);
            plantedKey.transform.Find("KeyBody").GetComponent<Rigidbody>().AddTorque(initialTorque);
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
        MapManager.RoomType thisRoom = m_blueprints[room.Item1, room.Item2, room.Item3];
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
                            windowCoords = new Vector3(thisLoc.Item1 - 10.01f, thisLoc.Item2, thisLoc.Item3);
                            windowRot = Quaternion.Euler(0, 90, 0);
                            isSearching = false;
                        }
                        break;
                    case 1:
                        if (thisRoom.forth)
                        {
                            windowCoords = new Vector3(thisLoc.Item1 + 10.01f, thisLoc.Item2, thisLoc.Item3);
                            windowRot = Quaternion.Euler(0, -90, 0);
                            isSearching = false;
                        }
                        break;
                    case 2:
                        if (thisRoom.left)
                        {
                            windowCoords = new Vector3(thisLoc.Item1, thisLoc.Item2, thisLoc.Item3 + 10.01f);
                            windowRot = Quaternion.Euler(0, 180, 0);
                            isSearching = false;
                        }
                        break;
                    case 3:
                        if (thisRoom.right)
                        {
                            windowCoords = new Vector3(thisLoc.Item1, thisLoc.Item2, thisLoc.Item3 - 10.01f);
                            windowRot = Quaternion.Euler(0, 0, 0);
                            isSearching = false;
                        }
                        break;
                    case 4:
                        if (thisRoom.up)
                        {
                            windowCoords = new Vector3(thisLoc.Item1, thisLoc.Item2 + 10.01f, thisLoc.Item3);
                            windowRot = Quaternion.Euler(90, 0, 0);
                            isSearching = false;
                        }
                        break;
                    case 5:
                        if (thisRoom.down)
                        {
                            windowCoords = new Vector3(thisLoc.Item1, thisLoc.Item2 - 10.01f, thisLoc.Item3);
                            windowRot = Quaternion.Euler(-90, 0, 0);
                            isSearching = false;
                        }
                        break;
                }
            }
            var window = Resources.Load("Crawler/RoomGate");
            GameObject plantedGate = Instantiate(window, windowCoords, windowRot) as GameObject; ;
            plantedGate.GetComponent<GateMechanism>().color = thisKey.color;
            plantedGate.transform.Find("Body/Inside").GetComponent<Renderer>().material.color = thisKey.color;
            plantedGate.transform.Find("Body/Inside2").GetComponent<Renderer>().material.color = thisKey.color;
        }
        return;
    }

    private void plantGenericOnWall((int, int, int) room, Object thing, int maxNum, bool isTargets, bool isPoster)
    {
        MapManager.RoomType thisRoom = m_blueprints[room.Item1, room.Item2, room.Item3];
        (int, int, int) thisLoc = getLoc(room);

        float iterNum = UnityEngine.Random.value * (float)maxNum;
        for (int i = 0; i < iterNum; i++)
        {
            // instantiate a target somewhere on a wall
            float scatterX = 0;
            float scatterY = 0;
            float scatterZ = 0;
            Quaternion targetRotation = Quaternion.identity;
            int choice = UnityEngine.Random.Range((int)0, (int)6);
            switch (choice)
            {
                case 0:
                    // on the back wall
                    scatterX = -9.9f;
                    if (UnityEngine.Random.value > 0.5f)
                    {
                        scatterY = 6.5f + (UnityEngine.Random.value * 2.0f);
                        if (UnityEngine.Random.value > 0.5f)
                        {
                            scatterY *= -1.0f;
                        }
                        scatterZ = -8.5f + (UnityEngine.Random.value * 17.0f);
                    }
                    else
                    {
                        scatterZ = 6.5f + (UnityEngine.Random.value * 2.0f);
                        if (UnityEngine.Random.value > 0.5f)
                        {
                            scatterZ *= -1.0f;
                        }
                        scatterY = -8.5f + (UnityEngine.Random.value * 17.0f);
                    }
                    targetRotation = Quaternion.Euler(0, 90, 0);
                    break;
                case 1:
                    // on the front wall
                    scatterX = 9.9f;
                    if (UnityEngine.Random.value > 0.5f)
                    {
                        scatterY = 6.5f + (UnityEngine.Random.value * 2.0f);
                        if (UnityEngine.Random.value > 0.5f)
                        {
                            scatterY *= -1.0f;
                        }
                        scatterZ = -8.5f + (UnityEngine.Random.value * 17.0f);
                    }
                    else
                    {
                        scatterZ = 6.5f + (UnityEngine.Random.value * 2.0f);
                        if (UnityEngine.Random.value > 0.5f)
                        {
                            scatterZ *= -1.0f;
                        }
                        scatterY = -8.5f + (UnityEngine.Random.value * 17.0f);
                    }
                    targetRotation = Quaternion.Euler(0, -90, 0);
                    break;
                case 2:
                    // on the left wall
                    scatterZ = 9.9f;
                    if (UnityEngine.Random.value > 0.5f)
                    {
                        scatterX = 6.5f + (UnityEngine.Random.value * 2.0f);
                        if (UnityEngine.Random.value > 0.5f)
                        {
                            scatterX *= -1.0f;
                        }
                        scatterY = -8.5f + (UnityEngine.Random.value * 17.0f);
                    }
                    else
                    {
                        scatterY = 6.5f + (UnityEngine.Random.value * 2.0f);
                        if (UnityEngine.Random.value > 0.5f)
                        {
                            scatterY *= -1.0f;
                        }
                        scatterX = -8.5f + (UnityEngine.Random.value * 17.0f);
                    }
                    targetRotation = Quaternion.Euler(0, 180, 0);
                    break;
                case 3:
                    // on the right wall
                    scatterZ = -9.9f;
                    if (UnityEngine.Random.value > 0.5f)
                    {
                        scatterX = 6.5f + (UnityEngine.Random.value * 2.0f);
                        if (UnityEngine.Random.value > 0.5f)
                        {
                            scatterX *= -1.0f;
                        }
                        scatterY = -8.5f + (UnityEngine.Random.value * 17.0f);
                    }
                    else
                    {
                        scatterY = 6.5f + (UnityEngine.Random.value * 2.0f);
                        if (UnityEngine.Random.value > 0.5f)
                        {
                            scatterY *= -1.0f;
                        }
                        scatterX = -8.5f + (UnityEngine.Random.value * 17.0f);
                    }
                    targetRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case 4:
                    // on the up wall
                    scatterY = 9.9f;
                    if (UnityEngine.Random.value > 0.5f)
                    {
                        scatterX = 6.5f + (UnityEngine.Random.value * 2.0f);
                        if (UnityEngine.Random.value > 0.5f)
                        {
                            scatterX *= -1.0f;
                        }
                        scatterZ = -8.5f + (UnityEngine.Random.value * 17.0f);
                    }
                    else
                    {
                        scatterZ = 6.5f + (UnityEngine.Random.value * 2.0f);
                        if (UnityEngine.Random.value > 0.5f)
                        {
                            scatterZ *= -1.0f;
                        }
                        scatterX = -8.5f + (UnityEngine.Random.value * 17.0f);
                    }
                    targetRotation = Quaternion.Euler(90, 0, 0);
                    break;
                case 5:
                    // on the down wall
                    scatterY = -9.9f;
                    if (UnityEngine.Random.value > 0.5f)
                    {
                        scatterX = 6.5f + (UnityEngine.Random.value * 2.0f);
                        if (UnityEngine.Random.value > 0.5f)
                        {
                            scatterX *= -1.0f;
                        }
                        scatterZ = -8.5f + (UnityEngine.Random.value * 17.0f);
                    }
                    else
                    {
                        scatterZ = 6.5f + (UnityEngine.Random.value * 2.0f);
                        if (UnityEngine.Random.value > 0.5f)
                        {
                            scatterZ *= -1.0f;
                        }
                        scatterX = -8.5f + (UnityEngine.Random.value * 17.0f);
                    }
                    targetRotation = Quaternion.Euler(-90, 0, 0);
                    break;
            }

            Vector3 targetCoords = new Vector3(thisLoc.Item1 + scatterX, thisLoc.Item2 + scatterY, thisLoc.Item3 + scatterZ);

            GameObject thisThing = Instantiate(thing, targetCoords, targetRotation) as GameObject;

            if (isTargets)
            {
                GameObject.FindWithTag("HUD").GetComponent<PointsWatcher>().addTarget();
            }

            if(isPoster)
            {
                Sprite[] sprites = Resources.LoadAll<Sprite>("Crawler/Posters");
                thisThing.transform.localScale = new Vector3(1, 1, 1);
                thisThing.GetComponent<SpriteRenderer>().sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
            }
        }

        return;
    }
    private void plantGenericInSpace((int, int, int) room, Object thing, int maxNum, float chanceToSpawnOne)
    {
        (int, int, int) genLoc = getLoc(room);

        if (maxNum == 1)
        {
            if (UnityEngine.Random.value <= chanceToSpawnOne)
            {
                // instantiate a generic somewhere
                float scatterX = -9.0f + (UnityEngine.Random.value * 18.0f);
                float scatterY = -9.0f + (UnityEngine.Random.value * 18.0f);
                float scatterZ = -9.0f + (UnityEngine.Random.value * 18.0f);

                Vector3 genCoords = new Vector3(genLoc.Item1 + scatterX, genLoc.Item2 + scatterY, genLoc.Item3 + scatterZ);

                Instantiate(thing, genCoords, Quaternion.identity);
            }
        }
        else
        {
            float iterNum = UnityEngine.Random.value * (float)maxNum;
            for (int i = 0; i < iterNum; i++)
            {
                // instantiate a generic somewhere in a cube
                float scatterX = -9.0f + (UnityEngine.Random.value * 18.0f);
                float scatterY = -9.0f + (UnityEngine.Random.value * 18.0f);
                float scatterZ = -9.0f + (UnityEngine.Random.value * 18.0f);

                Vector3 genCoords = new Vector3(genLoc.Item1 + scatterX, genLoc.Item2 + scatterY, genLoc.Item3 + scatterZ);

                GameObject thisThing = Instantiate(thing, genCoords, Quaternion.identity) as GameObject;
                thisThing.GetComponentInChildren<Rigidbody>().AddTorque(new Vector3(90, 90, 180));
            }
        }
        return;
    }


}
