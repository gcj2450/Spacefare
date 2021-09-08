using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WindowPlanter : Singleton<WindowPlanter>
{
    // (Optional) Prevent non-singleton constructor use.
    protected WindowPlanter() { }

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
        return (blue.Item1 * 48, blue.Item2 * 48, blue.Item3 * 48);
    }

    private void plantWindows((int, int, int) room)
    {
        MapManager.RoomType[,,] blueprints = MapManager.blueprints;
        int size = blueprints.GetLength(0);
        (int, int, int) start = ((size + 1) / 2, (size + 1) / 2, (size + 1) / 2);
        if (room == start)
        {
            return;
        }

        MapManager.RoomType thisRoom = blueprints[room.Item1, room.Item2, room.Item3];
        (int, int, int) thisLoc = getLoc(room);
        Vector3 windowCoords = Vector3.zero;
        Quaternion windowRot = Quaternion.identity;
        int choice = UnityEngine.Random.Range((int)0,(int)6);
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
            plantWindows(room);
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
