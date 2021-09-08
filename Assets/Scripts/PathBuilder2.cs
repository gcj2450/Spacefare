using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathBuilder2 : Singleton<PathBuilder2>
{
    // (Optional) Prevent non-singleton constructor use.
    protected PathBuilder2() { }

    public (int, int, int) m_root;

    public MapManager.RoomType[,,] m_blueprints;
    // one dimension of a cube
    public int m_size;

    public (int, int, int) cleanerRoom;

    public System.Random rand;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void buildMap(int size, int fuel)
    {
        m_size = size;
        m_root = ((m_size + 1) / 2, (m_size + 1) / 2, (m_size + 1) / 2);

        m_blueprints = generateNullMap(m_size);

        generateMap(fuel);
        //buildDebugMap();

        // instantiate the map pieces
        instantiateMap();

        MapManager.blueprints = m_blueprints;
        MapManager.root = m_root;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void buildDebugMap()
    {
        // the 2-hallways
        MapManager.RoomType bf = new MapManager.RoomType(true, true, false, false, false, false);
        m_blueprints[0, 0, 0] = bf;
        MapManager.RoomType bl = new MapManager.RoomType(true, false, true, false, false, false);
        m_blueprints[1, 0, 0] = bl;
        MapManager.RoomType br = new MapManager.RoomType(true, false, false, true, false, false);
        m_blueprints[2, 0, 0] = br;
        MapManager.RoomType bu = new MapManager.RoomType(true, false, false, false, true, false);
        m_blueprints[3, 0, 0] = bu;
        MapManager.RoomType bd = new MapManager.RoomType(true, false, false, false, false, true);
        m_blueprints[4, 0, 0] = bd;

        MapManager.RoomType fl = new MapManager.RoomType(false, true, true, false, false, false);
        m_blueprints[5, 0, 0] = fl;
        MapManager.RoomType fr = new MapManager.RoomType(false, true, false, true, false, false);
        m_blueprints[6, 0, 0] = fr;
        MapManager.RoomType fu = new MapManager.RoomType(false, true, false, false, true, false);
        m_blueprints[7, 0, 0] = fu;
        MapManager.RoomType fd = new MapManager.RoomType(false, true, false, false, false, true);
        m_blueprints[8, 0, 0] = fd;

        MapManager.RoomType lr = new MapManager.RoomType(false, false, true, true, false, false);
        m_blueprints[9, 0, 0] = lr;
        MapManager.RoomType lu = new MapManager.RoomType(false, false, true, false, true, false);
        m_blueprints[10, 0, 0] = lu;
        MapManager.RoomType ld = new MapManager.RoomType(false, false, true, false, false, true);
        m_blueprints[11, 0, 0] = ld;

        MapManager.RoomType ru = new MapManager.RoomType(false, false, false, true, true, false);
        m_blueprints[12, 0, 0] = ru;
        MapManager.RoomType rd = new MapManager.RoomType(false, false, false, true, false, true);
        m_blueprints[13, 0, 0] = rd;

        MapManager.RoomType ud = new MapManager.RoomType(false, false, false, false, true, true);
        m_blueprints[14, 0, 0] = ud;

        // the 3-hallways
        MapManager.RoomType threea = new MapManager.RoomType(true, true, true, false, false, false);
        m_blueprints[0, 0, 2] = threea;
        MapManager.RoomType threeb = new MapManager.RoomType(true, true, false, true, false, false);
        m_blueprints[1, 0, 2] = threeb;
        MapManager.RoomType threec = new MapManager.RoomType(true, true, false, false, true, false);
        m_blueprints[2, 0, 2] = threec;
        MapManager.RoomType threed = new MapManager.RoomType(true, true, false, false, false, true);
        m_blueprints[3, 0, 2] = threed;
        MapManager.RoomType threee = new MapManager.RoomType(true, false, true, true, false, false);
        m_blueprints[4, 0, 2] = threee;
        MapManager.RoomType threef = new MapManager.RoomType(true, false, true, false, true, false);
        m_blueprints[5, 0, 2] = threef;
        MapManager.RoomType threeg = new MapManager.RoomType(true, false, true, false, false, true);
        m_blueprints[6, 0, 2] = threeg;
        MapManager.RoomType threeh = new MapManager.RoomType(true, false, false, true, true, false);
        m_blueprints[7, 0, 2] = threeh;
        MapManager.RoomType threei = new MapManager.RoomType(true, false, false, true, false, true);
        m_blueprints[8, 0, 2] = threei;
        MapManager.RoomType threej = new MapManager.RoomType(true, false, false, false, true, true);
        m_blueprints[9, 0, 2] = threej;

        MapManager.RoomType threek = new MapManager.RoomType(false, true, true, true, false, false);
        m_blueprints[10, 0, 2] = threek;
        MapManager.RoomType threel = new MapManager.RoomType(false, true, true, false, true, false);
        m_blueprints[11, 0, 2] = threel;
        MapManager.RoomType threem = new MapManager.RoomType(false, true, true, false, false, true);
        m_blueprints[12, 0, 2] = threem;
        MapManager.RoomType threen = new MapManager.RoomType(false, true, false, true, true, false);
        m_blueprints[13, 0, 2] = threen;
        MapManager.RoomType threeo = new MapManager.RoomType(false, true, false, true, false, true);
        m_blueprints[14, 0, 2] = threeo;
        MapManager.RoomType threep = new MapManager.RoomType(false, true, false, false, true, true);
        m_blueprints[15, 0, 2] = threep;

        MapManager.RoomType threeq = new MapManager.RoomType(false, false, true, true, true, false);
        m_blueprints[16, 0, 2] = threeq;
        MapManager.RoomType threer = new MapManager.RoomType(false, false, true, true, false, true);
        m_blueprints[17, 0, 2] = threer;
        MapManager.RoomType threes = new MapManager.RoomType(false, false, true, false, true, true);
        m_blueprints[18, 0, 2] = threes;

        MapManager.RoomType threet = new MapManager.RoomType(false, false, false, true, true, true);
        m_blueprints[19, 0, 2] = threet;


        // the 4-hallways
        MapManager.RoomType foura = new MapManager.RoomType(true, true, true, true, false, false);
        m_blueprints[0, 0, 4] = foura;
        MapManager.RoomType fourb = new MapManager.RoomType(true, true, true, false, true, false);
        m_blueprints[1, 0, 4] = fourb;
        MapManager.RoomType fourc = new MapManager.RoomType(true, true, true, false, false, true);
        m_blueprints[2, 0, 4] = fourc;
        MapManager.RoomType fourd = new MapManager.RoomType(true, true, false, true, true, false);
        m_blueprints[3, 0, 4] = fourd;
        MapManager.RoomType foure = new MapManager.RoomType(true, true, false, true, false, true);
        m_blueprints[4, 0, 4] = foure;
        MapManager.RoomType fourf = new MapManager.RoomType(true, true, false, false, true, true);
        m_blueprints[5, 0, 4] = fourf;
        MapManager.RoomType fourg = new MapManager.RoomType(true, false, true, true, true, false);
        m_blueprints[6, 0, 4] = fourg;
        MapManager.RoomType fourh = new MapManager.RoomType(true, false, true, true, false, true);
        m_blueprints[7, 0, 4] = fourh;
        MapManager.RoomType fouri = new MapManager.RoomType(true, false, true, false, true, true);
        m_blueprints[8, 0, 4] = fouri;
        MapManager.RoomType fourj = new MapManager.RoomType(true, false, false, true, true, true);
        m_blueprints[9, 0, 4] = fourj;

        MapManager.RoomType fourk = new MapManager.RoomType(false, true, true, true, true, false);
        m_blueprints[10, 0, 4] = fourk;
        MapManager.RoomType fourl = new MapManager.RoomType(false, true, true, true, false, true);
        m_blueprints[11, 0, 4] = fourl;
        MapManager.RoomType fourm = new MapManager.RoomType(false, true, true, false, true, true);
        m_blueprints[12, 0, 4] = fourm;
        MapManager.RoomType fourn = new MapManager.RoomType(false, true, false, true, true, true);
        m_blueprints[13, 0, 4] = fourn;

        MapManager.RoomType fouro = new MapManager.RoomType(false, false, true, true, true, true);
        m_blueprints[14, 0, 4] = fouro;

        // the 5-hallways
        MapManager.RoomType fiveA = new MapManager.RoomType(false, true, true, true, true, true);
        m_blueprints[0, 0, 6] = fiveA;
        MapManager.RoomType fiveB = new MapManager.RoomType(true, false, true, true, true, true);
        m_blueprints[1, 0, 6] = fiveB;
        MapManager.RoomType fiveC = new MapManager.RoomType(true, true, false, true, true, true);
        m_blueprints[2, 0, 6] = fiveC;
        MapManager.RoomType fiveD = new MapManager.RoomType(true, true, true, false, true, true);
        m_blueprints[3, 0, 6] = fiveD;
        MapManager.RoomType fiveE = new MapManager.RoomType(true, true, true, true, false, true);
        m_blueprints[4, 0, 6] = fiveE;
        MapManager.RoomType fiveF = new MapManager.RoomType(true, true, true, true, true, false);
        m_blueprints[5, 0, 6] = fiveF;

        // the 6-hallway
        MapManager.RoomType six = new MapManager.RoomType(true, true, true, true, true, true);
        m_blueprints[0, 0, 8] = six;

        // the 1-hallways
        MapManager.RoomType oneA = new MapManager.RoomType(true, false, false, false, false, false);
        m_blueprints[0, 0, 10] = oneA;
        MapManager.RoomType oneB = new MapManager.RoomType(false, true, false, false, false, false);
        m_blueprints[1, 0, 10] = oneB;
        MapManager.RoomType oneC = new MapManager.RoomType(false, false, true, false, false, false);
        m_blueprints[2, 0, 10] = oneC;
        MapManager.RoomType oneD = new MapManager.RoomType(false, false, false, true, false, false);
        m_blueprints[3, 0, 10] = oneD;
        MapManager.RoomType oneE = new MapManager.RoomType(false, false, false, false, true, false);
        m_blueprints[4, 0, 10] = oneE;
        MapManager.RoomType oneF = new MapManager.RoomType(false, false, false, false, false, true);
        m_blueprints[5, 0, 10] = oneF;

        return;
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

        //System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());

        MapManager.RoomType lastRoom = m_blueprints[root.Item1, root.Item2, root.Item3];
        /*
        Debug.Log(lastRoom.back.ToString()
             + lastRoom.forth.ToString()
             + lastRoom.left.ToString()
              +lastRoom.right.ToString()
             +lastRoom.up.ToString()
             +lastRoom.down.ToString());
        */

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

    void generateMap(int fuel)
    {
        MapManager.RoomType firstRoom = buildRandomRoom(false, true, false, false, false, false);
        firstRoom.location = m_root;

        m_blueprints[(m_size + 1) / 2, (m_size + 1) / 2, (m_size + 1) / 2] = firstRoom;
        generateMapHelper(m_root, fuel);
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
                    var straight  = Resources.Load("Paths/hallway0");
                    var bend      = Resources.Load("Paths/hallway1");
                    var threewayA = Resources.Load("Paths/hallway2");
                    var threewayB = Resources.Load("Paths/hallway3");
                    var fourwayA  = Resources.Load("Paths/hallway4");
                    var fourwayB  = Resources.Load("Paths/hallway5");
                    var fiveway   = Resources.Load("Paths/hallway6");
                    var sixway    = Resources.Load("Paths/hallway7");
                    var cap       = Resources.Load("Paths/hallway8");

                    // the 1-hallways
                    if (thisRoom.isMatch(true, false, false, false, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(cap, thisLocation, Quaternion.Euler(new Vector3(0, 90, 0)));
                    }
                    else if (thisRoom.isMatch(false, true, false, false, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(cap, thisLocation, Quaternion.Euler(new Vector3(0, -90, 0)));
                    }
                    else if (thisRoom.isMatch(false, false, true, false, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(cap, thisLocation, Quaternion.Euler(new Vector3(0, 180, 0)));
                    }
                    else if (thisRoom.isMatch(false, false, false, true, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(cap, thisLocation, Quaternion.Euler(new Vector3(0, 0, 0)));
                    }
                    else if (thisRoom.isMatch(false, false, false, false, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(cap, thisLocation, Quaternion.Euler(new Vector3(90, 0, 0)));
                    }
                    else if (thisRoom.isMatch(false, false, false, false, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(cap, thisLocation, Quaternion.Euler(new Vector3(-90, 0, 0)));
                    }

                    // the 2-hallways
                    // DONE
                    else if (thisRoom.isMatch(true, true, false, false, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(straight, thisLocation, Quaternion.Euler(new Vector3(0, 90, 0)));
                    }
                    else if (thisRoom.isMatch(false,false,true,true,false,false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(straight, thisLocation, Quaternion.identity);
                    }
                    else if ( thisRoom.isMatch(false,false,false,false,true,true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(straight, thisLocation, Quaternion.Euler(new Vector3(90, 0, 0)));
                    }

                    else if (thisRoom.isMatch(true, false, true, false, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(180,0,0));
                    }
                    else if (thisRoom.isMatch(true, false, false, true, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.identity);
                    }
                    else if (thisRoom.isMatch(true, false, false, false, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(90,0,0));
                    }
                    else if (thisRoom.isMatch(true, false, false, false, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(-90, 0, 0));
                    }
                    else if (thisRoom.isMatch(false, true, true, false, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(0, 180, 0));
                    }
                    else if (thisRoom.isMatch(false, true, false, true, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(0, 0, 180));
                    }
                    else if (thisRoom.isMatch(false, true, false, false, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(90, -90, 90));
                    }
                    else if (thisRoom.isMatch(false, true, false, false, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(-90, 90, 90));
                    }
                    else if (thisRoom.isMatch(false, false, true, false, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(0,180,-90));
                    }
                    else if (thisRoom.isMatch(false, false, true, false, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(0, 180, 90));
                    }
                    else if (thisRoom.isMatch(false, false, false, true, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(0, 0, -90));
                    }
                    else if (thisRoom.isMatch(false, false, false, true, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(0, 0, 90));
                    }

                    // the 3-hallways
                    else if (thisRoom.isMatch(true, true, true, false, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayB, thisLocation, Quaternion.Euler(0, 90, 0));
                    }
                    else if (thisRoom.isMatch(true, true, false, true, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayB, thisLocation, Quaternion.Euler(0, -90, 0));
                    }
                    else if (thisRoom.isMatch(true, true, false, false, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayB, thisLocation, Quaternion.Euler(0, 90, -90));
                    }
                    else if (thisRoom.isMatch(true, true, false, false, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayB, thisLocation, Quaternion.Euler(0, 90, 90));
                    }
                    else if (thisRoom.isMatch(true, false, true, true, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayB, thisLocation, Quaternion.Euler(0, 0, 0));
                    }
                    else if (thisRoom.isMatch(true, false, true, false, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayA, thisLocation, Quaternion.Euler(0, 180, 0));
                    }
                    else if (thisRoom.isMatch(true, false, true, false, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayA, thisLocation, Quaternion.Euler(180, 0, 90));
                    }
                    else if (thisRoom.isMatch(true, false, false, true, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayA, thisLocation, Quaternion.Euler(0, 90, 0));
                    }
                    else if (thisRoom.isMatch(true, false, false, true, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayA, thisLocation, Quaternion.Euler(0, 0, 180));
                    }
                    else if (thisRoom.isMatch(true, false, false, false, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayB, thisLocation, Quaternion.Euler(90, 0, 0));
                    }

                    else if (thisRoom.isMatch(false, true, true, true, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayB, thisLocation, Quaternion.Euler(0, 0, 180));
                    }
                    else if (thisRoom.isMatch(false, true, true, false, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayA, thisLocation, Quaternion.Euler(90, 0, 0));
                    }
                    else if (thisRoom.isMatch(false, true, true, false, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayA, thisLocation, Quaternion.Euler(180, 0, 0));
                    }
                    else if (thisRoom.isMatch(false, true, false, true, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayA, thisLocation, Quaternion.Euler(0, 0, 0));
                    }
                    else if (thisRoom.isMatch(false, true, false, true, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayA, thisLocation, Quaternion.Euler(-90, 0, 0));
                    }
                    else if (thisRoom.isMatch(false, true, false, false, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayB, thisLocation, Quaternion.Euler(90, 0, 180));
                    }

                    else if (thisRoom.isMatch(false, false, true, true, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayB, thisLocation, Quaternion.Euler(0, 0, -90));
                    }
                    else if (thisRoom.isMatch(false, false, true, true, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayB, thisLocation, Quaternion.Euler(0, 0, 90));
                    }
                    else if (thisRoom.isMatch(false, false, true, false, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayB, thisLocation, Quaternion.Euler(-90, 0, 90));
                    }

                    else if (thisRoom.isMatch(false, false, false, true, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(threewayB, thisLocation, Quaternion.Euler(90, 0, 90));
                    }


                    // the 4-hallways

                    else if (thisRoom.isMatch(true, true, true, true, false, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayA, thisLocation, Quaternion.identity);
                    }
                    else if (thisRoom.isMatch(true, true, true, false, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayB, thisLocation, Quaternion.Euler(0,90,0));
                    }
                    else if (thisRoom.isMatch(true, true, true, false, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayB, thisLocation, Quaternion.Euler(0,90,90));
                    }
                    else if (thisRoom.isMatch(true, true, false, true, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayB, thisLocation, Quaternion.Euler(0, -90, 0));
                    }
                    else if (thisRoom.isMatch(true, true, false, true, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayB, thisLocation, Quaternion.Euler(0, -90, 90));
                    }
                    else if (thisRoom.isMatch(true, true, false, false, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayA, thisLocation, Quaternion.Euler(90,0,0));
                    }
                    else if (thisRoom.isMatch(true, false, true, true, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayB, thisLocation, Quaternion.Euler(0, 0, 0));
                    }
                    else if (thisRoom.isMatch(true, false, true, true, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayB, thisLocation, Quaternion.Euler(0, 0, 90));
                    }
                    else if (thisRoom.isMatch(true, false, true, false, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayB, thisLocation, Quaternion.Euler(90, 0, 0));
                    }
                    else if (thisRoom.isMatch(true, false, false, true, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayB, thisLocation, Quaternion.Euler(-90, 0, 0));
                    }
                    else if (thisRoom.isMatch(false, true, true, true, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayB, thisLocation, Quaternion.Euler(0, 0, -90));
                    }
                    else if (thisRoom.isMatch(false, true, true, true, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayB, thisLocation, Quaternion.Euler(0, 0, 180));
                    }
                    else if (thisRoom.isMatch(false, true, true, false, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayB, thisLocation, Quaternion.Euler(90, 0, -90));
                    }
                    else if (thisRoom.isMatch(false, true, false, true, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayB, thisLocation, Quaternion.Euler(90, 90, -90));
                    }
                    else if (thisRoom.isMatch(false, false, true, true, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fourwayA, thisLocation, Quaternion.Euler(0,0,90));
                    }


                    // the 5-hallways
                    // DONE
                    else if (thisRoom.isMatch(false, true, true, true, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fiveway, thisLocation, Quaternion.Euler(0,0,-90));
                    }
                    else if (thisRoom.isMatch(true, false, true, true, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fiveway, thisLocation, Quaternion.Euler(0, 0, 90));
                    }
                    else if (thisRoom.isMatch(true, true, false, true, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fiveway, thisLocation, Quaternion.Euler(-90, 0, 0));
                    }
                    else if (thisRoom.isMatch(true, true, true, false, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fiveway, thisLocation, Quaternion.Euler(90, 0, 0));
                    }
                    else if (thisRoom.isMatch(true, true, true, true, false, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fiveway, thisLocation, Quaternion.Euler(180, 0, 0));
                    }
                    else if (thisRoom.isMatch(true, true, true, true, true, false))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(fiveway, thisLocation, Quaternion.identity);
                    }

                    // the 6-hallway
                    // DONE
                    else if (thisRoom.isMatch(true, true, true, true, true, true))
                    {
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(sixway, thisLocation, Quaternion.identity);
                    }

                    // should never get here
                    else
                    {
                        // do nothing
                    }
                }
            }
        }





        return;
    }

    private UnityEngine.Object LoadPrefabFromFile(string filename)
    {
        var loadedObject = Resources.Load(filename);
        //if (loadedObject == null)
        //{
        //    throw new FileNotFoundException("...no file found - please check the configuration");
        //}
        return loadedObject;
    }



}
