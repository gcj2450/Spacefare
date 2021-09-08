using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public static class MapManager
{
    public static RoomType[,,] blueprints { get; set; }
    public static (int, int, int) root { get; set; }
    public static (int, int, int) lastKnownPlayerLocation { get; set; }

    public enum RoomPart
    {
        Back,
        Forth,
        Left,
        Right,
        Up,
        Down
    }

    public struct RoomType
    {
        public (int, int, int) location;
        public bool back;
        public bool forth;
        public bool left;
        public bool right;
        public bool up;
        public bool down;

        public RoomType(bool b, bool f, bool l, bool r, bool u, bool d)
        {
            this.location = (-1, -1, -1);
            this.back = b;
            this.forth = f;
            this.left = l;
            this.right = r;
            this.up = u;
            this.down = d;
        }

        public bool isNullRoom()
        {
            return (!(this.back || this.forth || this.left || this.right || this.up || this.down));
        }

        public bool isMatch(bool b, bool f, bool l, bool r, bool u, bool d)
        {
            return (this.back == b && this.forth == f && this.left == l && this.right == r && this.up == u && this.down == d);
        }
    }

    public static Vector3 getLoc((int, int, int) blue)
    {
        if (GameManager.isTightSpaces)
        {
            return new Vector3(blue.Item1 * 20, blue.Item2 * 20, blue.Item3 * 20);
        }
        else// if (GameManager.gameMode == GameManager.GameMode.Survival2)
        {
            return new Vector3(blue.Item1 * 60, blue.Item2 * 60, blue.Item3 * 60);
        }
    }

    public static (int, int, int) findStartRoom()
    {
        (int, int, int) thisRoomLoc = root;
        thisRoomLoc = (thisRoomLoc.Item1, thisRoomLoc.Item2, thisRoomLoc.Item3);
        RoomType nextRoom = blueprints[thisRoomLoc.Item1, thisRoomLoc.Item2, thisRoomLoc.Item3];
        int myfuel = 1000;
        while (!nextRoom.isNullRoom() && myfuel > 0)
        {
            myfuel--;
            int choice = Mathf.RoundToInt((UnityEngine.Random.value * 6) - 0.5f);
            RoomType thisRoom = blueprints[thisRoomLoc.Item1, thisRoomLoc.Item2, thisRoomLoc.Item3];
            switch (choice)
            {
                case 0:
                    if (!thisRoom.back)
                    {
                        continue;
                    }
                    nextRoom = blueprints[thisRoomLoc.Item1 - 1, thisRoomLoc.Item2, thisRoomLoc.Item3];
                    if (nextRoom.forth)
                    {
                        thisRoomLoc = (thisRoomLoc.Item1 - 1, thisRoomLoc.Item2, thisRoomLoc.Item3);
                    }
                    break;
                case 1:
                    if (!thisRoom.forth)
                    {
                        continue;
                    }
                    nextRoom = blueprints[thisRoomLoc.Item1 + 1, thisRoomLoc.Item2, thisRoomLoc.Item3];
                    if (nextRoom.back)
                    {
                        thisRoomLoc = (thisRoomLoc.Item1 + 1, thisRoomLoc.Item2, thisRoomLoc.Item3);
                    }
                    break;
                case 2:
                    if (!thisRoom.left)
                    {
                        continue;
                    }
                    nextRoom = blueprints[thisRoomLoc.Item1, thisRoomLoc.Item2, thisRoomLoc.Item3 + 1];
                    if (nextRoom.right)
                    {
                        thisRoomLoc = (thisRoomLoc.Item1, thisRoomLoc.Item2, thisRoomLoc.Item3 + 1);
                    }
                    break;
                case 3:
                    if (!thisRoom.right)
                    {
                        continue;
                    }
                    nextRoom = blueprints[thisRoomLoc.Item1, thisRoomLoc.Item2, thisRoomLoc.Item3 - 1];
                    if (nextRoom.left)
                    {
                        thisRoomLoc = (thisRoomLoc.Item1, thisRoomLoc.Item2, thisRoomLoc.Item3 - 1);
                    }
                    break;
                case 4:
                    if (!thisRoom.up)
                    {
                        continue;
                    }
                    nextRoom = blueprints[thisRoomLoc.Item1, thisRoomLoc.Item2 + 1, thisRoomLoc.Item3];
                    if (nextRoom.down)
                    {
                        thisRoomLoc = (thisRoomLoc.Item1, thisRoomLoc.Item2 + 1, thisRoomLoc.Item3);
                    }
                    break;
                case 5:
                    if (!thisRoom.down)
                    {
                        continue;
                    }
                    nextRoom = blueprints[thisRoomLoc.Item1, thisRoomLoc.Item2 - 1, thisRoomLoc.Item3];
                    if (nextRoom.up)
                    {
                        thisRoomLoc = (thisRoomLoc.Item1, thisRoomLoc.Item2 - 1, thisRoomLoc.Item3);
                    }
                    break;
            }
        }
        return thisRoomLoc;
    }
}

