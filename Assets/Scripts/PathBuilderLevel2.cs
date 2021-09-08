using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathBuilderLevel2 : Singleton<PathBuilderLevel2>
{
    // (Optional) Prevent non-singleton constructor use.
    protected PathBuilderLevel2() { }

    public (int, int, int) m_root;

    public System.Random rand;

    private MapManager.RoomType[,,] m_blueprints;
    // one dimension of a cube
    public int m_size;

    public (int, int, int) cleanerRoom;

    void Awake()
    {
    }
    void Start()
    {
    }

    public void buildMap(int size, int fuel)
    {
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
        MapManager.RoomType firstRoom = buildRandomRoom(false, true, false, false, false, false);
        firstRoom.location = m_root;

        m_blueprints[m_root.Item1, m_root.Item2, m_root.Item3] = firstRoom;
        generateMapHelper(m_root, fuel);
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

    // input: true if we require a connection in that direction
    MapManager.RoomType buildRandomRoom(bool b, bool f, bool l, bool r, bool u, bool d)
    {
        MapManager.RoomType thisRoom = new MapManager.RoomType(b,f,l,r,u,d);
        if(!b)
        {
            thisRoom.back = (UnityEngine.Random.value <= GameManager.mapDensity);
        }
        if (!f)
        {
            thisRoom.forth = (UnityEngine.Random.value <= GameManager.mapDensity);
        }
        if (!l)
        {
            thisRoom.left = (UnityEngine.Random.value <= GameManager.mapDensity);
        }
        if (!r)
        {
            thisRoom.right = (UnityEngine.Random.value <= GameManager.mapDensity);
        }
        if (!u)
        {
            thisRoom.up = (UnityEngine.Random.value <= GameManager.mapDensity);
        }
        if (!d)
        {
            thisRoom.down = (UnityEngine.Random.value <= GameManager.mapDensity);
        }
        return thisRoom;
    
    }

void generateMapHelper((int, int, int) root, int fuel)
    {
        if(fuel<0)
        {
            return;
        }

        if (fuel == 0)
        {
            cleanerRoom = root;
        }

        MapManager.RoomType lastRoom = m_blueprints[root.Item1, root.Item2, root.Item3];

        if (lastRoom.back)
        {
            (int, int, int) newRoot = root;
            newRoot.Item1 -= 1;
            if(newRoot.Item1 >= 0 && m_blueprints[newRoot.Item1, newRoot.Item2, newRoot.Item3].isNullRoom())
            {
                MapManager.RoomType thisRoom = buildRandomRoom(false, true, false, false, false, false);
                m_blueprints[newRoot.Item1, newRoot.Item2, newRoot.Item3] = thisRoom;
                generateMapHelper(newRoot, fuel - 1);
            }
        }
        if (lastRoom.forth)
        {
            (int, int, int) newRoot = root;
            newRoot.Item1 += 1;
            if (newRoot.Item1 < m_size && m_blueprints[newRoot.Item1, newRoot.Item2, newRoot.Item3].isNullRoom())
            {
                MapManager.RoomType thisRoom = buildRandomRoom(true, false, false, false, false, false);
                m_blueprints[newRoot.Item1, newRoot.Item2, newRoot.Item3] = thisRoom;
                generateMapHelper(newRoot, fuel - 1);
            }
        }
        if (lastRoom.left)
        {
            (int, int, int) newRoot = root;
            newRoot.Item3 += 1;
            if (newRoot.Item3 < m_size && m_blueprints[newRoot.Item1, newRoot.Item2, newRoot.Item3].isNullRoom())
            {
                MapManager.RoomType thisRoom = buildRandomRoom(false, false, false, true, false, false);
                m_blueprints[newRoot.Item1, newRoot.Item2, newRoot.Item3] = thisRoom;
                generateMapHelper(newRoot, fuel - 1);
            }
        }
        if (lastRoom.right)
        {
            (int, int, int) newRoot = root;
            newRoot.Item3 -= 1;
            if (newRoot.Item3 >= 0 && m_blueprints[newRoot.Item1, newRoot.Item2, newRoot.Item3].isNullRoom())
            {
                MapManager.RoomType thisRoom = buildRandomRoom(false, false, true, false, false, false);
                m_blueprints[newRoot.Item1, newRoot.Item2, newRoot.Item3] = thisRoom;
                generateMapHelper(newRoot, fuel - 1);
            }
        }
        if (lastRoom.up)
        {
            (int, int, int) newRoot = root;
            newRoot.Item2 += 1;
            if(newRoot.Item2 < m_size && m_blueprints[newRoot.Item1, newRoot.Item2, newRoot.Item3].isNullRoom())
            {
                MapManager.RoomType thisRoom = buildRandomRoom(false, false, false, false, false, true);
                m_blueprints[newRoot.Item1, newRoot.Item2, newRoot.Item3] = thisRoom;
                generateMapHelper(newRoot, fuel-1);
            }
        }
        if (lastRoom.down)
        {
            (int, int, int) newRoot = root;
            newRoot.Item2 -= 1;
            if (newRoot.Item2 >= 0 && m_blueprints[newRoot.Item1, newRoot.Item2, newRoot.Item3].isNullRoom())
            {
                MapManager.RoomType thisRoom = buildRandomRoom(false, false, false, false, true, false);
                m_blueprints[newRoot.Item1, newRoot.Item2, newRoot.Item3] = thisRoom;
                generateMapHelper(newRoot, fuel - 1);
            }
        }

        if( !lastRoom.back && !lastRoom.forth && !lastRoom.left && !lastRoom.right && !lastRoom.up && !lastRoom.down)
        {
            Debug.Log("no connections!");
        }

        return;
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

                    var roomShell = Resources.Load("NewPaths/RoomShell");
                    var roomNub   = Resources.Load("NewPaths/roomNub");
                    var roomJoint = Resources.Load("NewPaths/hallwayJoint");

                    Vector3 thisLocation = getLoc((x, y, z));
                    Instantiate(roomShell, thisLocation, Quaternion.Euler(Vector3.zero));
                    if(thisRoom.back)
                    {
                        Instantiate(roomJoint, getBackLoc((x,y,z)), Quaternion.Euler(new Vector3(0, -90, 0)));
                    }
                    else
                    {
                        Instantiate(roomNub, getBackLoc((x, y, z)), Quaternion.Euler(new Vector3(0, -90, 0)));
                    }
                    if (thisRoom.forth)
                    {
                        Instantiate(roomJoint, getFrontLoc((x, y, z)), Quaternion.Euler(new Vector3(0, 90, 0)));
                    }
                    else
                    {
                        Instantiate(roomNub, getFrontLoc((x, y, z)), Quaternion.Euler(new Vector3(0, 90, 0)));
                    }
                    if (thisRoom.left)
                    {
                        Instantiate(roomJoint, getLeftLoc((x, y, z)), Quaternion.Euler(new Vector3(0, 0, 0)));
                    }
                    else
                    {
                        Instantiate(roomNub, getLeftLoc((x, y, z)), Quaternion.Euler(new Vector3(0, 0, 0)));
                    }
                    if (thisRoom.right)
                    {
                        Instantiate(roomJoint, getRightLoc((x, y, z)), Quaternion.Euler(new Vector3(0, 180, 0)));
                    }
                    else
                    {
                        Instantiate(roomNub, getRightLoc((x, y, z)), Quaternion.Euler(new Vector3(0, 180, 0)));
                    }
                    if (thisRoom.up)
                    {
                        Instantiate(roomJoint, getUpLoc((x, y, z)), Quaternion.Euler(new Vector3(-90, 0, 0)));
                    }
                    else
                    {
                        Instantiate(roomNub, getUpLoc((x, y, z)), Quaternion.Euler(new Vector3(-90, 0, 0)));
                    }
                    if (thisRoom.down)
                    {
                        Instantiate(roomJoint, getDownLoc((x, y, z)), Quaternion.Euler(new Vector3(90, 0, 0)));
                    }
                    else
                    {
                        Instantiate(roomNub, getDownLoc((x, y, z)), Quaternion.Euler(new Vector3(90, 0, 0)));
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
        return new Vector3(room.Item1 * 60, room.Item2 * 60, room.Item3 * 60);
    }
    public Vector3 getBackLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 60 - 10, room.Item2 * 60, room.Item3 * 60);
    }
    public Vector3 getFrontLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 60 + 10, room.Item2 * 60, room.Item3 * 60);
    }
    public Vector3 getLeftLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 60, room.Item2 * 60, room.Item3 * 60 + 10);
    }
    public Vector3 getRightLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 60, room.Item2 * 60, room.Item3 * 60 - 10);
    }
    public Vector3 getUpLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 60, room.Item2 * 60 + 10, room.Item3 * 60);
    }
    public Vector3 getDownLoc((int, int, int) room)
    {
        return new Vector3(room.Item1 * 60, room.Item2 * 60 - 10, room.Item3 * 60);
    }


}
