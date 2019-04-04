using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MirrorKnight
{
    public class Room
    {
        public enum Type
        {
            NORMAL,
            TREASURE,
            SHOP,
            BOSS,
            SECRET,
            PUZZLE
        }

        Type roomType;

        Tile[,] tiles;

        public Room(Type t)
        {
            
            switch (t)
            {
                case Type.NORMAL: //Normal
                    break;
                case Type.TREASURE: //Treasure
                    break;
                case Type.SHOP: //Shop
                    break;
                case Type.BOSS: //Boss
                    break;
                case Type.SECRET: //Secret
                    break;
                case Type.PUZZLE: //Puzzle
                    break;
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
