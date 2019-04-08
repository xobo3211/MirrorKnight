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

namespace MirrorKnight
{
    public class Tile
    {
        public bool canMoveThrough;
        public bool canShootThrough;
        public bool damageHazard;

        Texture2D texture;

        public enum Type
        {
            NORMAL, //n
            OBSTACLE, //o
            PIT, //p
            HAZARD //h
        }

        public Tile(Type t)
        {
            switch (t)
            {
                case Type.NORMAL: 
                    canMoveThrough = true;
                    canShootThrough = true;
                    damageHazard = false;
                    break;

                case Type.OBSTACLE:
                    canMoveThrough = false;
                    canShootThrough = false;
                    damageHazard = false;
                    break;

                case Type.PIT:
                    canMoveThrough = false;
                    canShootThrough = true;
                    damageHazard = false;
                    break;

                case Type.HAZARD:
                    canMoveThrough = true;
                    canShootThrough = true;
                    damageHazard = true;
                    break;
            }

        }

        public void Draw(SpriteBatch b, int x, int y, int size)
        {
            b.Draw(texture, new Rectangle(x, y, size, size), Color.White);
        }
    }
}
