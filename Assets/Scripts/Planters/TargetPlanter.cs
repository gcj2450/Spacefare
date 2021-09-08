using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetPlanter : Singleton<TargetPlanter>
{
    // (Optional) Prevent non-singleton constructor use.
    protected TargetPlanter() { }

    private bool[,,] visited;

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
        return (blue.Item1 * 20, blue.Item2 * 20, blue.Item3 * 20);
    }

    private void plantTargets((int, int, int) room)
    {
        MapManager.RoomType[,,] blueprints = PathBuilder2.Instance.m_blueprints;
        int size = blueprints.GetLength(0);
        (int, int, int) start = ((size + 1) / 2, (size + 1) / 2, (size + 1) / 2);
        if (room == start)
        {
            return;
        }

        MapManager.RoomType thisRoom = blueprints[room.Item1, room.Item2, room.Item3];
        (int, int, int) thisLoc = getLoc(room);

        for (int i = 0; i < UnityEngine.Random.value * 3; i++)
        {
            // instantiate a target somewhere on a wall
            float scatterX = 0;
            float scatterY = 0;
            float scatterZ = 0;
            Quaternion targetRotation = Quaternion.identity;
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
                            if ((UnityEngine.Random.value > 0.5f)) // on a y-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    targetRotation = Quaternion.Euler(0, 0, 0);
                                    scatterY = -1.99f;
                                }
                                else
                                {
                                    targetRotation = Quaternion.Euler(180, 0, 0);
                                    scatterY = 1.99f;
                                }
                                scatterZ = UnityEngine.Random.value * 1.75f;
                            }
                            else // on a z-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    targetRotation = Quaternion.Euler(90, 0, 0);
                                    scatterZ = -1.99f;
                                }
                                else
                                {
                                    targetRotation = Quaternion.Euler(-90, 0, 0);
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
                                    targetRotation = Quaternion.Euler(0, 0, 0);
                                    scatterY = -1.99f;
                                }
                                else
                                {
                                    targetRotation = Quaternion.Euler(180, 0, 0);
                                    scatterY = 1.99f;
                                }
                                scatterZ = UnityEngine.Random.value * 1.75f;
                            }
                            else // on a z-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    targetRotation = Quaternion.Euler(90, 0, 0);
                                    scatterZ = -1.99f;
                                }
                                else
                                {
                                    targetRotation = Quaternion.Euler(-90, 0, 0);
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
                                    targetRotation = Quaternion.Euler(0, 0, 0);
                                    scatterY = -1.99f;
                                }
                                else
                                {
                                    targetRotation = Quaternion.Euler(180, 0, 0);
                                    scatterY = 1.99f;
                                }
                                scatterX = UnityEngine.Random.value * 1.75f;
                            }
                            else // on a x-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    targetRotation = Quaternion.Euler(0, 0, -90);
                                    scatterX = -1.99f;
                                }
                                else
                                {
                                    targetRotation = Quaternion.Euler(0, 0, 90);
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
                                    targetRotation = Quaternion.Euler(0, 0, 0);
                                    scatterY = -1.99f;
                                }
                                else
                                {
                                    targetRotation = Quaternion.Euler(180, 0, 0);
                                    scatterY = 1.99f;
                                }
                                scatterX = UnityEngine.Random.value * 1.75f;
                            }
                            else // on a x-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    targetRotation = Quaternion.Euler(0, 0, -90);
                                    scatterX = -1.99f;
                                }
                                else
                                {
                                    targetRotation = Quaternion.Euler(0, 0, 90);
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
                                    targetRotation = Quaternion.Euler(0, 0, -90);
                                    scatterX = -1.99f;
                                }
                                else
                                {
                                    targetRotation = Quaternion.Euler(0, 0, 90);
                                    scatterX = 1.99f;
                                }
                                scatterZ = UnityEngine.Random.value * 1.75f;
                            }
                            else // on a z-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    targetRotation = Quaternion.Euler(90, 0, 0);
                                    scatterZ = -1.99f;
                                }
                                else
                                {
                                    targetRotation = Quaternion.Euler(-90, 0, 0);
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
                                    targetRotation = Quaternion.Euler(0, 0, -90);
                                    scatterX = -1.99f;
                                }
                                else
                                {
                                    targetRotation = Quaternion.Euler(0, 0, 90);
                                    scatterX = 1.99f;
                                }
                                scatterZ = UnityEngine.Random.value * 1.75f;
                            }
                            else // on a z-wall
                            {
                                if ((UnityEngine.Random.value > 0.5f))
                                {
                                    targetRotation = Quaternion.Euler(90, 0, 0);
                                    scatterZ = -1.99f;
                                }
                                else
                                {
                                    targetRotation = Quaternion.Euler(-90, 0, 0);
                                    scatterZ = 1.99f;
                                }
                                scatterX = UnityEngine.Random.value * 1.75f;

                            }
                            isSearching = false;
                        }
                        break;
                }
            }

            Vector3 targetCoords = new Vector3(thisLoc.Item1 + scatterX, thisLoc.Item2 + scatterY, thisLoc.Item3 + scatterZ);

            var target = Resources.Load("Crawler/TargetLevel1");
            Instantiate(target, targetCoords, targetRotation);

            GameObject.FindWithTag("HUD").GetComponent<PointsWatcher>().addTarget();
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

        MapManager.RoomType[,,] blueprints = PathBuilder2.Instance.m_blueprints;
        MapManager.RoomType thisRoom = blueprints[room.Item1, room.Item2, room.Item3];

        if (thisRoom.back || thisRoom.forth || thisRoom.left || thisRoom.right || thisRoom.up || thisRoom.down)
        {
            plantTargets(room);
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

        (int, int, int) start = ((size + 1) / 2, (size + 1) / 2, (size + 1) / 2);

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
