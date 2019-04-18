﻿using System;
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

        Rectangle r = new Rectangle(0, 0, 50, 50);

        int maxX = 9, maxY = 9;

        Rooms[,] roomsEnum;

        Room[,] rooms;

        int maxRoomCount;
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

        public enum Floor
        {
            GARDEN,
            TEMPLE,
            CRYPT,
            ABYSS
        }

        enum Directions
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            NONE
        }

        public Map(Floor f)
        {

            bool boss, treasure, shop, secret;

            switch (f)
            {
                case Floor.GARDEN:
                    maxRoomCount = 10;
                    break;

                case Floor.TEMPLE:
                    maxRoomCount = 15;
                    break;

                case Floor.CRYPT:
                    maxRoomCount = 20;
                    maxX = 11;
                    maxY = 11;
                    break;

                case Floor.ABYSS:
                    maxRoomCount = 25;
                    maxX = 13;
                    maxY = 13;
                    break;
            }


            do
            {
                Generate();

                boss = TryPlacingBossRoom();
                treasure = TryPlacingTreasureRoom();
                shop = TryPlacingShop();
                secret = TryPlacingSecretRoom();
            }
            while (CountDeadEnds() < 3 && !boss && !treasure && !shop && !secret);               //Require minimum of three dead ends in a map. Boss, Treasure, Shop

            for(int y = 0; y < maxY; y++)
            {
                for(int x = 0; x < maxX; x++)
                {
                    //rooms[x, y] = new Room(roomsEnum[x, y])
                }
            }
        }


        void Generate()
        {
            roomsEnum = new Rooms[maxX, maxY];
            rooms = new Room[maxX, maxY];

            roomsEnum[maxX / 2, maxY / 2] = Rooms.EMPTY;                                              //Places starting room in the middle

            currentRoomCount = maxRoomCount;

            while (currentRoomCount > 0)
            {
                int x = rn.Next(maxX), y = rn.Next(maxY);

                if (roomsEnum[x, y] == Rooms.VOID && IsConnected(x, y))
                {
                    roomsEnum[x, y] = Rooms.EMPTY;
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
                roomsEnum[x, y] = Rooms.DEAD_END;
            }
            else
            {
                roomsEnum[x, y] = Rooms.EMPTY_AND_CHECKED;
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

        void RemoveExcessDeadEnds()     //Removes a dead end if it's connected to more than two roomsEnum
        {
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (roomsEnum[x, y] == Rooms.DEAD_END)
                    {
                        if (CountConnections(ConnectionArr(x, y, Rooms.EMPTY_AND_CHECKED)) + CountConnections(ConnectionArr(x, y, Rooms.DEAD_END)) > 1)      //Checks if dead end is connected to more than two roomsEnum
                        {
                            roomsEnum[x, y] = Rooms.EMPTY_AND_CHECKED;
                        }
                    }
                }
            }
        }

        int CountDeadEnds()     //Counts number of dead ends in the map
        {
            int n = 0;
            foreach (Rooms r in roomsEnum)
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
                    if (roomsEnum[x, y] == Rooms.DEAD_END && CheckDistance(x, y, maxX / 2, maxY / 2) > globalMax)
                    {
                        globalMax = CheckDistance(x, y, maxX / 2, maxY / 2);
                        finalCoords = new int[] { x, y };
                    }
                }
            }

            if (finalCoords[0] != -1 && finalCoords[1] != -1)
            {
                roomsEnum[finalCoords[0], finalCoords[1]] = Rooms.BOSS;
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
                    if (roomsEnum[x, y] == Rooms.DEAD_END && CheckDistance(x, y, maxX / 2, maxY / 2) < globalMin)
                    {
                        globalMin = CheckDistance(x, y, maxX / 2, maxY / 2);
                        finalCoords = new int[] { x, y };
                    }
                }
            }

            if (finalCoords[0] != -1 && finalCoords[1] != -1)
            {
                roomsEnum[finalCoords[0], finalCoords[1]] = Rooms.TREASURE;
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
                    if (roomsEnum[x, y] == Rooms.DEAD_END && CheckDistance(x, y, maxX / 2, maxY / 2) < globalMin)
                    {
                        globalMin = CheckDistance(x, y, maxX / 2, maxY / 2);
                        finalCoords = new int[] { x, y };
                    }
                }
            }

            if (finalCoords[0] != -1 && finalCoords[1] != -1)
            {
                roomsEnum[finalCoords[0], finalCoords[1]] = Rooms.SHOP;
                return true;
            }
            else Console.WriteLine("Error: Shop room could not be placed");
            return false;
        }

        bool TryPlacingSecretRoom()     //Finds a section where three or more roomsEnum are connected to an empty void and places a secret room there
        {
            bool notPlaced = true;
            for (int y = 0; y < maxY && notPlaced; y++)
            {
                for (int x = 0; x < maxX && notPlaced; x++)
                {
                    if (roomsEnum[x, y] == Rooms.VOID && CountConnections(ConnectionArr(x, y)) > 2 && CountConnections(ConnectionArr(x, y, Rooms.BOSS)) < 1);
                    {
                        roomsEnum[x, y] = Rooms.SECRET;
                        notPlaced = false;
                    }
                }
            }

            return !notPlaced;
        }



        void CreatePath(int x, int y, int finalX, int finalY)       //Creates a path of empty roomsEnum from (x,y) to (finalX, finalY)
        {
            while (x < finalX)
            {
                x++;
                roomsEnum[x, y] = Rooms.EMPTY;
            }
            while (x > finalX)
            {
                x--;
                roomsEnum[x, y] = Rooms.EMPTY;
            }

            while (y < finalY)
            {
                y++;
                roomsEnum[x, y] = Rooms.EMPTY;
            }
            while (y > finalY)
            {
                y--;
                roomsEnum[x, y] = Rooms.EMPTY;
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
                a = roomsEnum[x, y - 1] != Rooms.VOID;
            if (y + 1 < roomsEnum.GetLength(1))
                b = roomsEnum[x, y + 1] != Rooms.VOID;
            if (x - 1 >= 0)
                c = roomsEnum[x - 1, y] != Rooms.VOID;
            if (x + 1 < roomsEnum.GetLength(0))
                d = roomsEnum[x + 1, y] != Rooms.VOID;
            return new bool[] { a, b, c, d };
        }

        bool[] ConnectionArr(int x, int y, Rooms roomType)            //Returns which directions a room is connected to another Room of roomType with pattern {up, down, left, right}
        {
            bool a = false, b = false, c = false, d = false;
            if (y - 1 >= 0)
                a = roomsEnum[x, y - 1] == roomType;
            if (y + 1 < roomsEnum.GetLength(1))
                b = roomsEnum[x, y + 1] == roomType;
            if (x - 1 >= 0)
                c = roomsEnum[x - 1, y] == roomType;
            if (x + 1 < roomsEnum.GetLength(0))
                d = roomsEnum[x + 1, y] == roomType;
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
                a = roomsEnum[x - 1, y] == Rooms.EMPTY;
            if (x + 1 < roomsEnum.GetLength(0))
                b = roomsEnum[x + 1, y] == Rooms.EMPTY;
            if (y - 1 > 0)
                c = roomsEnum[x, y - 1] == Rooms.EMPTY;
            if (y + 1 < roomsEnum.GetLength(1))
                d = roomsEnum[x, y + 1] == Rooms.EMPTY;

            return a || b || c || d;
        }



        bool Random(int chance)             //Approx. 1/chance chance of returning true
        {
            return new Random().Next(2) % chance == 0;
        }

        public Room GetRoom(int x, int y)
        {
            return rooms[x, y];
        }

        public void SetRoom(Room r, int x, int y)
        {
            rooms[x, y] = r;
        }

        public Point GetDimensions()
        {
            return new Point(maxX, maxY);
        }
    }
}
