using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathBuilderTimeTrials : Singleton<PathBuilderTimeTrials>
{
    // (Optional) Prevent non-singleton constructor use.
    protected PathBuilderTimeTrials() { }

    public (int, int, int) m_root;

    public System.Random rand;

    private MapManager.RoomType[,,] m_blueprints;
    // one dimension of a cube
    public int m_size;

    public (int, int, int) finalRoom;

    public int numRooms;

    void Awake()
    {
    }
    void Start()
    {
    }

    public void reset()
    {
        numRooms = 0;
    }
    public void buildMap(int size, int fuel)
    {
        reset();
        numRooms = 0;
        m_size = size;
        m_root = ((m_size + 1) / 2, (m_size + 1) / 2, (m_size + 1) / 2);

        m_blueprints = generateNullMap(m_size);
        generateMap(fuel);
        closeDeadEnds();
        instantiateMap();

        MapManager.blueprints = m_blueprints;
        MapManager.root = m_root;
    }

    void generateMap(int fuel)
    {
        MapManager.RoomType firstRoom = roomConstructor(false, true, false, false, false, false);
        firstRoom.location = m_root;
        m_blueprints[m_root.Item1, m_root.Item2, m_root.Item3] = firstRoom;
        numRooms++;
        generateMapHelper((m_root.Item1+1, m_root.Item2, m_root.Item3), firstRoom, fuel);
    }

    public MapManager.RoomType roomConstructor(bool b, bool f, bool l, bool r, bool u, bool d)
    {
        MapManager.RoomType thisRoom;
        thisRoom.location = (-1, -1, -1);
        thisRoom.back = b;
        thisRoom.forth = f;
        thisRoom.left = l;
        thisRoom.right = r;
        thisRoom.up = u;
        thisRoom.down = d;
        return thisRoom;
    }

    void generateMapHelper((int, int, int) buildLocation, MapManager.RoomType sourceRoom, int fuel)
    {
        if (fuel < 0)
        {
            finalRoom = sourceRoom.location;
            return;
        }

        //MapManager.RoomType lastRoom = m_blueprints[sourceLocation.Item1, sourceLocation.Item2, sourceLocation.Item3];
        (int, int, int) deltaLocation =    (buildLocation.Item1 - sourceRoom.location.Item1,
                                            buildLocation.Item2 - sourceRoom.location.Item2,
                                            buildLocation.Item3 - sourceRoom.location.Item3);
        MapManager.RoomType incompleteRoom = buildIncompleteRoom(deltaLocation, buildLocation);

        (MapManager.RoomType, MapManager.RoomPart) thisRoomInfo = addAnExit(incompleteRoom);

        m_blueprints[buildLocation.Item1, buildLocation.Item2, buildLocation.Item3] = thisRoomInfo.Item1;
        numRooms++;

        (int, int, int) nextBuildLocation = (0, 0, 0);
        if (thisRoomInfo.Item2 == MapManager.RoomPart.Back)
        {
            nextBuildLocation = (buildLocation.Item1 - 1, buildLocation.Item2, buildLocation.Item3);
        }
        else if (thisRoomInfo.Item2 == MapManager.RoomPart.Forth)
        {
            nextBuildLocation = (buildLocation.Item1 + 1, buildLocation.Item2, buildLocation.Item3);
        }
        else if (thisRoomInfo.Item2 == MapManager.RoomPart.Left)
        {
            nextBuildLocation = (buildLocation.Item1, buildLocation.Item2, buildLocation.Item3 + 1);
        }
        else if (thisRoomInfo.Item2 == MapManager.RoomPart.Right)
        {
            nextBuildLocation = (buildLocation.Item1, buildLocation.Item2, buildLocation.Item3 - 1);
        }
        else if (thisRoomInfo.Item2 == MapManager.RoomPart.Up)
        {
            nextBuildLocation = (buildLocation.Item1, buildLocation.Item2 + 1, buildLocation.Item3);
        }
        else// if (thisRoomInfo.Item2 == MapManager.RoomPart.Down)
        {
            nextBuildLocation = (buildLocation.Item1, buildLocation.Item2 - 1, buildLocation.Item3);
        }

        generateMapHelper(nextBuildLocation, thisRoomInfo.Item1, fuel - 1);

        return;
    }

    private MapManager.RoomType buildIncompleteRoom((int, int, int) dLocation, (int, int, int) buildLocation)
    {
        MapManager.RoomType thisRoom = roomConstructor(false, false, false, false, false, false);
        thisRoom.location = buildLocation;
        if (dLocation.Item1 == -1)
        {
            thisRoom.forth = true;
        }
        else if (dLocation.Item1 == 1)
        {
            thisRoom.back = true;
        }
        else if (dLocation.Item2 == -1)
        {
            thisRoom.up = true;
        }
        else if (dLocation.Item2 == 1)
        {
            thisRoom.down = true;
        }
        else if (dLocation.Item3 == -1)
        {
            thisRoom.left = true;
        }
        else //if (dLocation.Item3 == 1)
        {
            thisRoom.right = true;
        }
        return thisRoom;
    }

    (MapManager.RoomType, MapManager.RoomPart) addAnExit(MapManager.RoomType thisRoom)
    {
        (MapManager.RoomType, MapManager.RoomPart) roomInfo;

        // ensure roomInfo is initialized
        roomInfo.Item1 = thisRoom;
        roomInfo.Item1.location = thisRoom.location;
        roomInfo.Item2 = MapManager.RoomPart.Back;

        while (!hasTwoExits(roomInfo.Item1))
        {
            int choice = UnityEngine.Random.Range((int)0, (int)6);
            MapManager.RoomType potentialRoom;
            switch (choice)
            {
                case 0:
                    potentialRoom = m_blueprints[roomInfo.Item1.location.Item1 - 1, roomInfo.Item1.location.Item2, roomInfo.Item1.location.Item3];
                    if (roomInfo.Item1.back || !potentialRoom.isNullRoom())
                    {
                        break;
                    }
                    roomInfo.Item1.back = true;
                    roomInfo.Item2 = MapManager.RoomPart.Back;
                    break;
                case 1:
                    potentialRoom = m_blueprints[roomInfo.Item1.location.Item1 + 1, roomInfo.Item1.location.Item2, roomInfo.Item1.location.Item3];
                    if (roomInfo.Item1.forth || !potentialRoom.isNullRoom())
                    {
                        break;
                    }
                    roomInfo.Item1.forth = true;
                    roomInfo.Item2 = MapManager.RoomPart.Forth;
                    break;
                case 2:
                    potentialRoom = m_blueprints[roomInfo.Item1.location.Item1, roomInfo.Item1.location.Item2, roomInfo.Item1.location.Item3 + 1];
                    if (roomInfo.Item1.left || !potentialRoom.isNullRoom())
                    {
                        break;
                    }
                    roomInfo.Item1.left = true;
                    roomInfo.Item2 = MapManager.RoomPart.Left;
                    break;
                case 3:
                    potentialRoom = m_blueprints[roomInfo.Item1.location.Item1, roomInfo.Item1.location.Item2, roomInfo.Item1.location.Item3 - 1];
                    if (roomInfo.Item1.right || !potentialRoom.isNullRoom())
                    {
                        break;
                    }
                    roomInfo.Item1.right = true;
                    roomInfo.Item2 = MapManager.RoomPart.Right;
                    break;
                case 4:
                    potentialRoom = m_blueprints[roomInfo.Item1.location.Item1, roomInfo.Item1.location.Item2 + 1, roomInfo.Item1.location.Item3];
                    if (roomInfo.Item1.up || !potentialRoom.isNullRoom())
                    {
                        break;
                    }
                    roomInfo.Item1.up = true;
                    roomInfo.Item2 = MapManager.RoomPart.Up;
                    break;
                case 5:
                    potentialRoom = m_blueprints[roomInfo.Item1.location.Item1, roomInfo.Item1.location.Item2 - 1, roomInfo.Item1.location.Item3];
                    if (roomInfo.Item1.down || !potentialRoom.isNullRoom())
                    {
                        break;
                    }
                    roomInfo.Item1.down = true;
                    roomInfo.Item2 = MapManager.RoomPart.Down;
                    break;
            }
        }
        return roomInfo;
    }

    MapManager.RoomType[,,] generateNullMap(int size)
    {
        MapManager.RoomType[,,] map = new MapManager.RoomType[size, size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    map[x, y, z].location = (x, y, z);
                    map[x, y, z].back  = false;
                    map[x, y, z].forth = false;
                    map[x, y, z].left  = false;
                    map[x, y, z].right = false;
                    map[x, y, z].up    = false;
                    map[x, y, z].down  = false;
                }
            }
        }
        return map;
    }

    private bool hasTwoExits(MapManager.RoomType thisRoom)
    {
        int numExits = 0;
        if (thisRoom.back)
        {
            numExits += 1;
        }
        if (thisRoom.forth)
        {
            numExits += 1;
        }
        if (thisRoom.left)
        {
            numExits += 1;
        }
        if (thisRoom.right)
        {
            numExits += 1;
        }
        if (thisRoom.up)
        {
            numExits += 1;
        }
        if (thisRoom.down)
        {
            numExits += 1;
        }

        if(numExits == 2)
        {
            return true;
        }
        return false;

    }

    void instantiateMap()
    {
        for (int x = 0; x < m_size; x++)
        {
            for (int y = 0; y < m_size; y++)
            {
                for (int z = 0; z < m_size; z++)
                {
                    MapManager.RoomType thisRoom = m_blueprints[x, y, z];
                    if( thisRoom.isNullRoom() )
                    {
                        // don't instantiate anything
                        continue;
                    }

                    var roomShell = Resources.Load("NewPaths/TTShell");
                    var roomNub   = Resources.Load("NewPaths/TTCap");
                    var roomJoint = Resources.Load("NewPaths/TTStraight");

                    Vector3 thisLocation = getLoc((x, y, z));
                    Transform roomBase = (Instantiate(roomShell, thisLocation, Quaternion.Euler(Vector3.zero)) as GameObject).transform;
                    if(thisRoom.back)
                    {
                        (Instantiate(roomJoint, getBackLoc((x,y,z)), Quaternion.Euler(new Vector3(0, -90, 0))) as GameObject).transform.SetParent(roomBase);
                    }
                    else
                    {
                        (Instantiate(roomNub, getBackLoc((x, y, z)), Quaternion.Euler(new Vector3(0, -90, 0))) as GameObject).transform.SetParent(roomBase);
                    }
                    if (thisRoom.forth)
                    {
                        (Instantiate(roomJoint, getFrontLoc((x, y, z)), Quaternion.Euler(new Vector3(0, 90, 0))) as GameObject).transform.SetParent(roomBase);
                    }
                    else
                    {
                        (Instantiate(roomNub, getFrontLoc((x, y, z)), Quaternion.Euler(new Vector3(0, 90, 0))) as GameObject).transform.SetParent(roomBase);
}
                    if (thisRoom.left)
                    {
                        (Instantiate(roomJoint, getLeftLoc((x, y, z)), Quaternion.Euler(new Vector3(0, 0, 0))) as GameObject).transform.SetParent(roomBase);
}
                    else
                    {
                        (Instantiate(roomNub, getLeftLoc((x, y, z)), Quaternion.Euler(new Vector3(0, 0, 0))) as GameObject).transform.SetParent(roomBase);
}
                    if (thisRoom.right)
                    {
                        (Instantiate(roomJoint, getRightLoc((x, y, z)), Quaternion.Euler(new Vector3(0, 180, 0))) as GameObject).transform.SetParent(roomBase);
}
                    else
                    {
                        (Instantiate(roomNub, getRightLoc((x, y, z)), Quaternion.Euler(new Vector3(0, 180, 0))) as GameObject).transform.SetParent(roomBase);
}
                    if (thisRoom.up)
                    {
                        (Instantiate(roomJoint, getUpLoc((x, y, z)), Quaternion.Euler(new Vector3(-90, 0, 0))) as GameObject).transform.SetParent(roomBase);
}
                    else
                    {
                        (Instantiate(roomNub, getUpLoc((x, y, z)), Quaternion.Euler(new Vector3(-90, 0, 0))) as GameObject).transform.SetParent(roomBase);
}
                    if (thisRoom.down)
                    {
                        (Instantiate(roomJoint, getDownLoc((x, y, z)), Quaternion.Euler(new Vector3(90, 0, 0))) as GameObject).transform.SetParent(roomBase);
}
                    else
                    {
                        (Instantiate(roomNub, getDownLoc((x, y, z)), Quaternion.Euler(new Vector3(90, 0, 0))) as GameObject).transform.SetParent(roomBase);
}
                }
            }
        }
        return;
    }

    void closeDeadEnds()
    {
        for (int x = 0; x < m_size; x++)
        {
            for (int y = 0; y < m_size; y++)
            {
                for (int z = 0; z < m_size; z++)
                {
                    MapManager. RoomType thisRoom = m_blueprints[x, y, z];
                    if (thisRoom.isNullRoom())
                    {
                        // don't instantiate anything
                        continue;
                    }

                    if (thisRoom.back)
                    {
                        var backRoom = m_blueprints[x - 1, y, z];
                        if (!backRoom.forth)
                        {
                            thisRoom.back = false;
                        }
                    }
                    if (thisRoom.forth)
                    {

                        var forthRoom = m_blueprints[x + 1, y, z];
                        if (!forthRoom.back)
                        {
                            thisRoom.forth = false;
                        }
                    }
                    if (thisRoom.left)
                    {

                        var leftRoom = m_blueprints[x, y, z + 1];
                        if (!leftRoom.right)
                        {
                            thisRoom.left = false;
                        }
                    }
                    if (thisRoom.right)
                    {

                        var rightRoom = m_blueprints[x, y, z - 1];
                        if (!rightRoom.left)
                        {
                            thisRoom.right = false;
                        }
                    }
                    if (thisRoom.up)
                    {

                        var upRoom = m_blueprints[x, y + 1, z];
                        if (!upRoom.down)
                        {
                            thisRoom.up = false;
                        }
                    }
                    if (thisRoom.down)
                    {

                        var downRoom = m_blueprints[x, y - 1, z];
                        if (!downRoom.up)
                        {
                            thisRoom.down = false;
                        }
                    }
                    m_blueprints[x, y, z] = thisRoom;
                }
            }
        }
    }
    public Vector3 getLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 20, room.Item2 * 20, room.Item3 * 20);
    }
    public Vector3 getBackLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 20 - 5, room.Item2 * 20, room.Item3 * 20);
    }
    public Vector3 getFrontLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 20 + 5, room.Item2 * 20, room.Item3 * 20);
    }
    public Vector3 getLeftLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 20, room.Item2 * 20, room.Item3 * 20 + 5);
    }
    public Vector3 getRightLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 20, room.Item2 * 20, room.Item3 * 20 - 5);
    }
    public Vector3 getUpLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 20, room.Item2 * 20 + 5, room.Item3 * 20);
    }
    public Vector3 getDownLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 20, room.Item2 * 20 - 5, room.Item3 * 20);
    }


}
