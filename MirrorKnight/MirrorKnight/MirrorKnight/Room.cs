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

        Type roomType;
        
        Tile[,] tiles;

        public Room(Type t, String[,] tileArr, Texture2D texture)
        {
            for(int y = 0; y < tiles.GetLength(1); y++)
            {
                for(int x = 0; x < tiles.GetLength(0); x++)
                {
                    switch(tileArr[x, y])
                    {
                        case "n":       //normal
                            tiles[x, y] = new Tile(Tile.Type.NORMAL);
                            tiles[x, y].SetTexture(texture);
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
        
    }
}
