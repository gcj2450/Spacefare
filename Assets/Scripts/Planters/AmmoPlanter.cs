using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AmmoPlanter : Singleton<AmmoPlanter>
{
    // (Optional) Prevent non-singleton constructor use.
    protected AmmoPlanter() { }

    private bool[,,] visited;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private (int,int,int) getLoc( (int,int,int) blue )
    {
        return (blue.Item1*20, blue.Item2 * 20, blue.Item3 * 20);
    }

    private void plantAmmo( (int,int,int) room )
    {
        MapManager.RoomType[,,] blueprints = PathBuilder2.Instance.m_blueprints;
        int size = blueprints.GetLength(0);
        (int, int, int) start = ((size + 1) / 2, (size + 1) / 2, (size + 1) / 2);
        if(room==start)
        {
            return;
        }

        MapManager.RoomType thisRoom = blueprints[room.Item1, room.Item2, room.Item3];
        (int, int, int) ammoLoc = getLoc(room);

        // instantiate an ammo somewhere
        float scatterX = 0;
        float scatterY = 0;
        float scatterZ = 0;

        bool isSearching = true;
        while (isSearching)
        {
            int choice = Mathf.RoundToInt(UnityEngine.Random.value * 5);
            switch (choice)
            {
                case 0:
                    if (thisRoom.back)
                    {
                        scatterX = (UnityEngine.Random.value * -7) - 2.5f;
                        scatterY = UnityEngine.Random.value * 3.0f - 1.5f;
                        scatterZ = UnityEngine.Random.value * 3.0f - 1.5f;
                    }
                    else
                    {
                        continue;
                    }
                    isSearching = false;
                    break;
                case 1:
                    if (thisRoom.forth)
                    {
                        scatterX = (UnityEngine.Random.value * + 7) + 2.5f;
                        scatterY = UnityEngine.Random.value * 3.0f - 1.5f;
                        scatterZ = UnityEngine.Random.value * 3.0f - 1.5f;
                    }
                    else
                    {
                        continue;
                    }
                    isSearching = false;
                    break;
                case 2:
                    if (thisRoom.left)
                    {
                        scatterX = UnityEngine.Random.value * 3.0f - 1.5f;
                        scatterY = UnityEngine.Random.value * 3.0f - 1.5f;
                        scatterZ = (UnityEngine.Random.value * + 7) + 2.5f;
                    }
                    else
                    {
                        continue;
                    }
                    isSearching = false;
                    break;
                case 3:
                    if (thisRoom.right)
                    {
                        scatterX = UnityEngine.Random.value * 3.0f - 1.5f;
                        scatterY = UnityEngine.Random.value * 3.0f - 1.5f;
                        scatterZ = (UnityEngine.Random.value * - 7) - 2.5f;
                    }
                    else
                    {
                        continue;
                    }
                    isSearching = false;
                    break;
                case 4:
                    if (thisRoom.up)
                    {
                        scatterX = UnityEngine.Random.value * 3.0f - 1.5f;
                        scatterY = (UnityEngine.Random.value * + 7) + 2.5f;
                        scatterZ = UnityEngine.Random.value * 3.0f - 1.5f;
                    }
                    else
                    {
                        continue;
                    }
                    isSearching = false;
                    break;
                case 5:
                    if (thisRoom.down)
                    {
                        scatterX = UnityEngine.Random.value * 3.0f - 1.5f;
                        scatterY = (UnityEngine.Random.value * - 7) - 2.5f;
                        scatterZ = UnityEngine.Random.value * 3.0f - 1.5f;
                    }
                    else
                    {
                        continue;
                    }
                    isSearching = false;
                    break;
            }


            Vector3 ammoCoords = new Vector3(ammoLoc.Item1 + scatterX, ammoLoc.Item2 + scatterY, ammoLoc.Item3 + scatterZ);

            var ammoCapsule = Resources.Load("Crawler/AmmoCapsule");
            Instantiate(ammoCapsule, ammoCoords, Quaternion.identity);
        }

        return;
    }

    private void crawl( (int,int,int) room )
    {
        if(visited[room.Item1, room.Item2, room.Item3])
        {
            return;
        }

        // mark visited
        visited[room.Item1, room.Item2, room.Item3] = true;

        MapManager.RoomType[,,] blueprints = PathBuilder2.Instance.m_blueprints;
        MapManager.RoomType thisRoom = blueprints[room.Item1, room.Item2, room.Item3];

        if(!thisRoom.isNullRoom())
        {
            // give a 5% chance to spawn an ammo capsule
            if (UnityEngine.Random.value < 0.05f)
            {
                plantAmmo(room);
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

    public void execute()
    {
        // get the blueprints
        MapManager.RoomType[,,] blueprints = PathBuilder2.Instance.m_blueprints;
        int size = blueprints.GetLength(0);

        // reset visited
        initVisited();
        
        (int,int,int) start = ((size + 1) / 2, (size + 1) / 2, (size + 1) / 2);

        crawl(start);

    }

    void initVisited()
    {
        // read the map
        MapManager.RoomType[,,] blueprints = PathBuilder2.Instance.m_blueprints;
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
