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
            roomType = t;

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
            LoadRoom();
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

        public void LoadRoom()                                          //To be called when the map is generated
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

            roomName += roomID + ".txt";

            ReadFile(roomName);
        }

        public void EnterRoom(ContentManager Content, Player p)
        {
            string path = "../../../../MirrorKnightContent/roomEntities/";

            switch (roomType)
            {
                case Type.NORMAL:
                    path += "normal/";
                    break;

                case Type.BOSS:
                    path += "boss/";
                    break;

                case Type.PUZZLE:
                    path += "puzzle/";
                    break;

                case Type.SECRET:
                    path += "secret/";
                    break;

                case Type.SHOP:
                    path += "shop/";
                    break;

                case Type.TREASURE:
                    path += "treasure/";
                    break;

                case Type.VOID:
                    break;

                default:
                    Console.WriteLine("Error loading file, type is " + roomType);
                    break;
            }

            path += getRoomName();

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    bool beginReading = false;
                    for (int j = 0; !reader.EndOfStream; j++)
                    {
                        string line = reader.ReadLine();

                        if (beginReading)
                        {
                            string[] args = line.Split(' ');

                            int x = Convert.ToInt32(args[1]);
                            int y = Convert.ToInt32(args[2]);

                            Vector2 pos = Game1.TileToPixelCoords(x, y);

                            switch (args[0])
                            {
                                case "e":

                                    break;

                                case "t":
                                    if (args.Length < 4)
                                        Game1.entities.Add(new Turret((int)pos.X, (int)pos.Y, Content.Load<Texture2D>("textures/big_demon_idle_anim_f0"), p));

                                    else
                                    {
                                        int targetX = Convert.ToInt32(args[3]);
                                        int targetY = Convert.ToInt32(args[4]);
                                        Game1.entities.Add(new Turret((int)pos.X, (int)pos.Y, Content.Load<Texture2D>("textures/big_demon_idle_anim_f0"), new Vector2(targetX, targetY)));
                                    }
                                    break;
                            }
                        }


                        if (line == "*")
                        {
                            beginReading = true;
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

        private string getRoomName()
        {
            for(int i = roomName.Length - 1; i > 0; i--)
            {
                if(roomName[i] == '/')
                {
                    return roomName.Substring(i);
                }
            }
            return "Error";
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
                                default:
                                    Console.WriteLine("Thing not found");
                                    break;
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException e)
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
