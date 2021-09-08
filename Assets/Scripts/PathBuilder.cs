using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathBuilder : MonoBehaviour
{
    enum PortalType
    {
        Null,
        Straight,
        Back,
        Right,
        Left,
        Up,
        Down
    }

    struct RoomType
    {
        public PortalType entrance;
        public PortalType exit;
    }


    // Start is called before the first frame update
    void Start()
    {
        // generate the map data structure
        int size = 20; //guarantee this is odd in the next line
        int fuel = 10;
        RoomType[,,] blueprints = generateMap(2*size + 1, fuel);

        // instantiate the map pieces
        instantiateMap(blueprints);

        // place the player
        var playerCharacter = Resources.Load("astronaut");
        Vector3 thisLocation = new Vector3(-30, (size + 1) * 20, (size + 1) * 20);
        Instantiate(playerCharacter, thisLocation, Quaternion.Euler(0,90,180));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    RoomType[,,] generateNullMap(int size)
    {
        RoomType nullRoom;
        nullRoom.entrance = PortalType.Null;
        nullRoom.exit = PortalType.Null;

        RoomType[,,] map = new RoomType[size, size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    map[x, y, z] = nullRoom;
                }
            }
        }
        return map;
    }

    RoomType[,,] generateMap(int size, int fuel)
    {
        RoomType[,,] blueprints = generateNullMap(size);

        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());

        // entrance is a straight hallway at [0,(size-1)/2,(size-1)/2]
        RoomType firstRoom;
        firstRoom.entrance = PortalType.Back;
        firstRoom.exit = PortalType.Straight;
        blueprints[0, (size + 1) / 2, (size + 1) / 2] = firstRoom;
        //Vector3 lastLocation = new Vector3(0, (size - 1) / 2, (size - 1) / 2);
        (int, int, int) lastLocation = (0, (size + 1) / 2, (size + 1) / 2);

        RoomType lastRoom = firstRoom;


        while (fuel > 0)
        {
            Array values = Enum.GetValues(typeof(PortalType));
            PortalType newExit = (PortalType)values.GetValue(1+rand.Next(values.Length-1));

            RoomType thisRoom;
            thisRoom.entrance = PortalType.Null;
            thisRoom.exit = newExit;

            (int, int, int) thisLocation = lastLocation;
            switch (lastRoom.exit)
            {
                case PortalType.Straight:
                    thisRoom.entrance = PortalType.Back;
                    thisLocation.Item1 += 1;
                    break;
                case PortalType.Back:
                    thisRoom.entrance = PortalType.Straight;
                    thisLocation.Item1 -= 1;
                    break;
                case PortalType.Right:
                    thisRoom.entrance = PortalType.Left;
                    thisLocation.Item3 -= 1;
                    break;
                case PortalType.Left:
                    thisRoom.entrance = PortalType.Right;
                    thisLocation.Item3 += 1;
                    break;
                case PortalType.Up:
                    thisRoom.entrance = PortalType.Down;
                    thisLocation.Item2 += 1;
                    break;
                case PortalType.Down:
                    thisRoom.entrance = PortalType.Up;
                    thisLocation.Item2 -= 1;
                    break;
            }

            fuel -= 1;

            if (thisLocation.Item1 > size - 1 || thisLocation.Item1 < 0
             || thisLocation.Item2 > size - 1 || thisLocation.Item2 < 0
             || thisLocation.Item3 > size - 1 || thisLocation.Item3 < 0)
            {
                // outside of the array
                continue;
            }

            if(newExit==thisRoom.entrance)
            {
                // doubling back on ourselves
                continue;
            }

            if(blueprints[thisLocation.Item1, thisLocation.Item2, thisLocation.Item3].exit != PortalType.Null)
            {
                // chosen tile is non-empty
                continue;
            }

            // look ahead one tile to predict path
            (int, int, int) nextLocation = thisLocation;
            switch (thisRoom.exit)
            {
                case PortalType.Straight:
                    nextLocation.Item1 += 1;
                    break;
                case PortalType.Back:
                    nextLocation.Item1 -= 1;
                    break;
                case PortalType.Right:
                    nextLocation.Item3 -= 1;
                    break;
                case PortalType.Left:
                    nextLocation.Item3 += 1;
                    break;
                case PortalType.Up:
                    nextLocation.Item2 += 1;
                    break;
                case PortalType.Down:
                    nextLocation.Item2 -= 1;
                    break;
            }
            if (nextLocation.Item1 > size - 1 || nextLocation.Item1 < 0
             || nextLocation.Item2 > size - 1 || nextLocation.Item2 < 0
             || nextLocation.Item3 > size - 1 || nextLocation.Item3 < 0)
            {
                // outside of array bounds
                continue;
            }
            if (blueprints[nextLocation.Item1, nextLocation.Item2, nextLocation.Item3].exit != PortalType.Null)
            {
                // next tile is non-empty, we'd intersect next round
                continue;
            }

            // otherwise it was a good generation
            blueprints[thisLocation.Item1, thisLocation.Item2, thisLocation.Item3] = thisRoom;
            lastRoom = thisRoom;
            lastLocation = thisLocation;
        }

        return blueprints;
    }

    void instantiateMap(RoomType[,,] blueprints)
    {
        RoomType nullRoom;
        nullRoom.entrance = PortalType.Null;
        nullRoom.exit = PortalType.Null;

        int size = blueprints.GetLength(0); // should be a cube
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    RoomType thisRoom = blueprints[x, y, z];
                    if( thisRoom.entrance==PortalType.Null || thisRoom.exit == PortalType.Null)
                    {
                        // don't instantiate anything
                        continue;
                    }
                    var straight = Resources.Load("Paths/hallway0");
                    var bend = Resources.Load("Paths/hallway1");

                    if(    thisRoom.entrance == PortalType.Back     && thisRoom.exit == PortalType.Straight
                        || thisRoom.entrance == PortalType.Straight && thisRoom.exit == PortalType.Back)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(straight, thisLocation, Quaternion.Euler(new Vector3(0, 90, 0)));
                    }
                    else if ( thisRoom.entrance == PortalType.Right && thisRoom.exit == PortalType.Left
                           || thisRoom.entrance == PortalType.Left && thisRoom.exit == PortalType.Right)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(straight, thisLocation, Quaternion.identity);
                    }
                    else if ( thisRoom.entrance == PortalType.Down && thisRoom.exit == PortalType.Up
                           || thisRoom.entrance == PortalType.Up && thisRoom.exit == PortalType.Down)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(straight, thisLocation, Quaternion.Euler(new Vector3(90, 0, 0)));
                    }

                    else if( thisRoom.entrance == PortalType.Back && thisRoom.exit == PortalType.Left || thisRoom.entrance == PortalType.Left && thisRoom.exit == PortalType.Back )
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(180,0,0));
                    }
                    else if (thisRoom.entrance == PortalType.Back && thisRoom.exit == PortalType.Right || thisRoom.entrance == PortalType.Right && thisRoom.exit == PortalType.Back)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.identity);
                    }
                    else if (thisRoom.entrance == PortalType.Back && thisRoom.exit == PortalType.Up || thisRoom.entrance == PortalType.Up && thisRoom.exit == PortalType.Back)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(90,0,0));
                    }
                    else if (thisRoom.entrance == PortalType.Back && thisRoom.exit == PortalType.Down || thisRoom.entrance == PortalType.Down && thisRoom.exit == PortalType.Back)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(-90, 0, 0));
                    }

                    else if (thisRoom.entrance == PortalType.Straight && thisRoom.exit == PortalType.Left || thisRoom.entrance == PortalType.Left && thisRoom.exit == PortalType.Straight)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(0, 180, 0));
                    }
                    else if (thisRoom.entrance == PortalType.Straight && thisRoom.exit == PortalType.Right || thisRoom.entrance == PortalType.Right && thisRoom.exit == PortalType.Straight)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(0, 0, 180));
                    }
                    else if (thisRoom.entrance == PortalType.Straight && thisRoom.exit == PortalType.Up || thisRoom.entrance == PortalType.Up && thisRoom.exit == PortalType.Straight)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(90, -90, 90));
                    }
                    else if (thisRoom.entrance == PortalType.Straight && thisRoom.exit == PortalType.Down || thisRoom.entrance == PortalType.Down && thisRoom.exit == PortalType.Straight)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(-90, 90, 90));
                    }

                    else if (thisRoom.entrance == PortalType.Left && thisRoom.exit == PortalType.Up || thisRoom.entrance == PortalType.Up && thisRoom.exit == PortalType.Left)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(0,180,-90));
                    }
                    else if (thisRoom.entrance == PortalType.Left && thisRoom.exit == PortalType.Down || thisRoom.entrance == PortalType.Down && thisRoom.exit == PortalType.Left)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(0, 180, 90));
                    }

                    else if (thisRoom.entrance == PortalType.Right && thisRoom.exit == PortalType.Up || thisRoom.entrance == PortalType.Up && thisRoom.exit == PortalType.Right)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(0, 0, -90));
                    }
 
                    else if (thisRoom.entrance == PortalType.Right && thisRoom.exit == PortalType.Down || thisRoom.entrance == PortalType.Down && thisRoom.exit == PortalType.Right)
                    {
                        // correct
                        Vector3 thisLocation = new Vector3(x * 20, y * 20, z * 20);
                        Instantiate(bend, thisLocation, Quaternion.Euler(0, 0, 90));
                    }
 
                    // should never get here
                    else
                    {
                     
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
