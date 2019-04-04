using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace MirrorKnight
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Map
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D t;

        Rectangle r = new Rectangle(0, 0, 50, 50);

        int maxX = 9, maxY = 9;

        Rooms[,] rooms;

        int maxRoomCount = 10;
        int currentRoomCount;

        Random rn = new Random();

        enum Rooms
        {
            VOID,
            EMPTY,
            EMPTY_AND_CHECKED,
            DEAD_END,
            TREASURE,
            SHOP,
            SECRET,
            BOSS
        }

        enum Directions
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            NONE
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public Map()
        {
            // TODO: Add your initialization logic here

            bool boss, treasure, shop, secret;

            do
            {
                Generate();

                boss = TryPlacingBossRoom();
                treasure = TryPlacingTreasureRoom();
                shop = TryPlacingShop();
                secret = TryPlacingSecretRoom();
            }
            while (CountDeadEnds() < 3 && !boss && !treasure && !shop && !secret);               //Require minimum of three dead ends in a map. Boss, Treasure, Shop
        }
        void Generate()
        {
            rooms = new Rooms[maxX, maxY];

            rooms[maxX / 2, maxY / 2] = Rooms.EMPTY;

            currentRoomCount = maxRoomCount;

            while (currentRoomCount > 0)
            {
                int x = rn.Next(maxX), y = rn.Next(maxY);

                if (rooms[x, y] == Rooms.VOID && IsConnected(x, y))
                {
                    rooms[x, y] = Rooms.EMPTY;
                    currentRoomCount--;
                }
            }

            SetDeadEnds(maxX / 2, maxY / 2, Directions.NONE);
            RemoveExcessDeadEnds();
        }

        void SetDeadEnds(int x, int y, Directions movementPath)
        {
            bool[] connections = ConnectionArr(x, y, Rooms.EMPTY);

            bool isDeadEnd = true;

            for (int i = 0; i < 4; i++)
            {
                if (i != (int)movementPath && connections[i])
                {
                    isDeadEnd = false;
                    break;
                }
            }


            if (isDeadEnd)
            {
                rooms[x, y] = Rooms.DEAD_END;
            }
            else
            {
                rooms[x, y] = Rooms.EMPTY_AND_CHECKED;
                for (int i = 0; i < 4; i++)
                {
                    if (i != (int)movementPath && connections[i])
                    {
                        switch (i)
                        {
                            case (int)Directions.UP:
                                SetDeadEnds(x, y - 1, Directions.DOWN);
                                break;

                            case (int)Directions.DOWN:
                                SetDeadEnds(x, y + 1, Directions.UP);
                                break;

                            case (int)Directions.LEFT:
                                SetDeadEnds(x - 1, y, Directions.RIGHT);
                                break;

                            case (int)Directions.RIGHT:
                                SetDeadEnds(x + 1, y, Directions.LEFT);
                                break;
                        }
                    }
                }
            }
        }

        void RemoveExcessDeadEnds()     //Removes a dead end if it's connected to more than two rooms
        {
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (rooms[x, y] == Rooms.DEAD_END)
                    {
                        if (CountConnections(ConnectionArr(x, y, Rooms.EMPTY_AND_CHECKED)) + CountConnections(ConnectionArr(x, y, Rooms.DEAD_END)) > 2)      //Checks if dead end is connected to more than two rooms
                        {
                            rooms[x, y] = Rooms.EMPTY_AND_CHECKED;
                        }
                    }
                }
            }
        }

        int CountDeadEnds()     //Counts number of dead ends in the map
        {
            int n = 0;
            foreach (Rooms r in rooms)
            {
                if (r == Rooms.DEAD_END)
                    n++;
            }
            return n;
        }

        bool TryPlacingBossRoom()           //Tries to place boss room in dead end furthest from start
        {
            int[] finalCoords = new int[] { -1, -1 };

            int globalMax = 0;

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (rooms[x, y] == Rooms.DEAD_END && CheckDistance(x, y, maxX / 2, maxY / 2) > globalMax)
                    {
                        globalMax = CheckDistance(x, y, maxX / 2, maxY / 2);
                        finalCoords = new int[] { x, y };
                    }
                }
            }

            if (finalCoords[0] != -1 && finalCoords[1] != -1)
            {
                rooms[finalCoords[0], finalCoords[1]] = Rooms.BOSS;
                return true;
            }
            else Console.WriteLine("Error: Boss room could not be placed");

            return false;
        }

        bool TryPlacingTreasureRoom()       //Places a treasure room in the dead end closest to the start
        {
            int[] finalCoords = new int[] { -1, -1 };

            int globalMin = 100;

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (rooms[x, y] == Rooms.DEAD_END && CheckDistance(x, y, maxX / 2, maxY / 2) < globalMin)
                    {
                        globalMin = CheckDistance(x, y, maxX / 2, maxY / 2);
                        finalCoords = new int[] { x, y };
                    }
                }
            }

            if (finalCoords[0] != -1 && finalCoords[1] != -1)
            {
                rooms[finalCoords[0], finalCoords[1]] = Rooms.TREASURE;
                return true;
            }
            else Console.WriteLine("Error: Treasure room could not be placed");
            return false;
        }

        bool TryPlacingShop()       //Places shop in dead end second closest to the spawn
        {
            int[] finalCoords = new int[] { -1, -1 };

            int globalMin = 100;

            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (rooms[x, y] == Rooms.DEAD_END && CheckDistance(x, y, maxX / 2, maxY / 2) < globalMin)
                    {
                        globalMin = CheckDistance(x, y, maxX / 2, maxY / 2);
                        finalCoords = new int[] { x, y };
                    }
                }
            }

            if (finalCoords[0] != -1 && finalCoords[1] != -1)
            {
                rooms[finalCoords[0], finalCoords[1]] = Rooms.SHOP;
                return true;
            }
            else Console.WriteLine("Error: Shop room could not be placed");
            return false;
        }

        bool TryPlacingSecretRoom()     //Finds a section where three or more rooms are connected to an empty void and places a secret room there
        {
            bool notPlaced = true;
            for (int y = 0; y < maxY && notPlaced; y++)
            {
                for (int x = 0; x < maxX && notPlaced; x++)
                {
                    if (rooms[x, y] == Rooms.VOID && CountConnections(ConnectionArr(x, y)) > 2)
                    {
                        rooms[x, y] = Rooms.SECRET;
                        notPlaced = false;
                    }
                }
            }

            return !notPlaced;
        }



        void CreatePath(int x, int y, int finalX, int finalY)       //Creates a path of empty rooms from (x,y) to (finalX, finalY)
        {
            while (x < finalX)
            {
                x++;
                rooms[x, y] = Rooms.EMPTY;
            }
            while (x > finalX)
            {
                x--;
                rooms[x, y] = Rooms.EMPTY;
            }

            while (y < finalY)
            {
                y++;
                rooms[x, y] = Rooms.EMPTY;
            }
            while (y > finalY)
            {
                y--;
                rooms[x, y] = Rooms.EMPTY;
            }
        }

        int CheckDistance(int x, int y, int bossX, int bossY)
        {
            return Math.Abs(bossX - x) + Math.Abs(bossY - y);
        }

        bool[] ConnectionArr(int x, int y)            //Returns which directions a room is connected to another room with pattern {up, down, left, right}
        {
            bool a = false, b = false, c = false, d = false;
            if (y - 1 >= 0)
                a = rooms[x, y - 1] != Rooms.VOID;
            if (y + 1 < rooms.GetLength(1))
                b = rooms[x, y + 1] != Rooms.VOID;
            if (x - 1 >= 0)
                c = rooms[x - 1, y] != Rooms.VOID;
            if (x + 1 < rooms.GetLength(0))
                d = rooms[x + 1, y] != Rooms.VOID;
            return new bool[] { a, b, c, d };
        }

        bool[] ConnectionArr(int x, int y, Rooms roomType)            //Returns which directions a room is connected to another Room of roomType with pattern {up, down, left, right}
        {
            bool a = false, b = false, c = false, d = false;
            if (y - 1 >= 0)
                a = rooms[x, y - 1] == roomType;
            if (y + 1 < rooms.GetLength(1))
                b = rooms[x, y + 1] == roomType;
            if (x - 1 >= 0)
                c = rooms[x - 1, y] == roomType;
            if (x + 1 < rooms.GetLength(0))
                d = rooms[x + 1, y] == roomType;
            return new bool[] { a, b, c, d };
        }

        int CountConnections(bool[] arr)
        {
            int n = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i])
                    n++;
            }
            return n;
        }

        bool IsConnected(int x, int y)
        {
            bool a = false, b = false, c = false, d = false;
            if (x - 1 > 0)
                a = rooms[x - 1, y] == Rooms.EMPTY;
            if (x + 1 < rooms.GetLength(0))
                b = rooms[x + 1, y] == Rooms.EMPTY;
            if (y - 1 > 0)
                c = rooms[x, y - 1] == Rooms.EMPTY;
            if (y + 1 < rooms.GetLength(1))
                d = rooms[x, y + 1] == Rooms.EMPTY;

            return a || b || c || d;
        }



        bool Random(int chance)             //Approx. 1/chance chance of returning true
        {
            return new Random().Next(2) % chance == 0;
        }
    }
}
