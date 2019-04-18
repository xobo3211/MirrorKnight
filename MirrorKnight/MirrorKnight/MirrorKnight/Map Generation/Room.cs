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
using SureDroid;
using System.IO;

namespace MirrorKnight
{
    public class Room
    {
        public enum Type
        {
            VOID,
            NORMAL,
            TREASURE,
            SHOP,
            BOSS,
            SECRET,
            PUZZLE
        }

        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        Type roomType;
        
        Tile[,] tiles;

        String roomName;        //Contains file path for the room

        bool hasBeenEntered = false;

        public Room(Type t)
        {
            tiles = new Tile[18, 10];

            string initialPath = "../../../../MirrorKnightContent/presetRooms/";

            switch(t)
            {
                case Type.NORMAL:
                    initialPath += "normal/";
                    break;

                case Type.BOSS:
                    initialPath += "boss/";
                    break;

                case Type.PUZZLE:
                    initialPath += "puzzle/";
                    break;

                case Type.SECRET:
                    initialPath += "secret/";
                    break;

                case Type.SHOP:
                    initialPath += "shop/";
                    break;

                case Type.TREASURE:
                    initialPath += "treasure/";
                    break;

                case Type.VOID:
                    break;

                default:
                    Console.WriteLine("Error loading file, type is " + t);
                    break;
            }
            
            roomName = initialPath;
        }

        public Room(Type t, String[,] tileArr, Texture2D texture)
        {
            tiles = new Tile[tileArr.GetLength(0), tileArr.GetLength(1)];
            for(int y = 0; y < tiles.GetLength(1); y++)
            {
                for(int x = 0; x < tiles.GetLength(0); x++)
                {
                    switch(tileArr[x, y])
                    {
                        case "n":       //normal
                            tiles[x, y] = new Tile(Tile.Type.NORMAL);
                            break;

                        case "o":       //obstacle
                            tiles[x, y] = new Tile(Tile.Type.OBSTACLE);
                            break;

                        case "p":       //pit
                            tiles[x, y] = new Tile(Tile.Type.PIT);
                            break;

                        case "h":       //hazard
                            tiles[x, y] = new Tile(Tile.Type.HAZARD);
                            break;
                    }
                }
            }
        }
        public Type getRoomType()
        {
            return roomType;
        }
        public void setRoomType(Type r)
        {
            roomType = r;
        }

        public void LoadRoom()                                          //To be called when player enters the room. Supposed to load everything in the room
        {
            Random rn = new Random();

            int roomID = 0;

            switch(roomType)
            {
                case Type.NORMAL:
                    roomID = rn.Next(3) + 1;
                    break;

                case Type.TREASURE:

                    break;

                case Type.SHOP:

                    break;

                case Type.BOSS:

                    break;

                case Type.PUZZLE:

                    break;

                case Type.SECRET:

                    break;
            }

            roomName += roomID;

            ReadFile(roomName);
        }

        private void ReadFile(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    for (int j = 0; !reader.EndOfStream; j++)
                    {
                        string line = reader.ReadLine();
                        string[] parts = line.Split(' ');
                        for (int i = 0; i < parts.Length; i++)
                        {
                            string c = parts[i];

                            switch (c)
                            {
                                case "n":
                                    tiles[i, j] = new Tile(Tile.Type.NORMAL);
                                    break;

                                case "o":
                                    tiles[i, j] = new Tile(Tile.Type.OBSTACLE);
                                    break;

                                case "p":
                                    tiles[i, j] = new Tile(Tile.Type.PIT);
                                    break;

                                case "h":
                                    tiles[i, j] = new Tile(Tile.Type.HAZARD);
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public void Draw(SpriteBatch b, int left, int top, int tileSize)
        {
            for(int y = 0; y < tiles.GetLength(1); y++)
            {
                for(int x = 0; x < tiles.GetLength(0); x++)
                {
                    tiles[x, y].Draw(b, left + (tileSize * x), top + (tileSize * y), tileSize);
                }
            }
        }
        
    }
}
