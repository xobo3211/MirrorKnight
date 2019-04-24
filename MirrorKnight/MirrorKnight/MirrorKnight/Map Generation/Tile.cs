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
using System.Threading;

namespace MirrorKnight
{
    public class Tile
    {
        public bool canMoveThrough;
        public bool canShootThrough;
        public bool damageHazard;
        Texture2D texture;
        static Dictionary<String, Texture2D> textures = Game1.sprites["floor"];
        Random rand = new Random((int)DateTime.Now.Ticks);

        public enum Type
        {
            NORMAL, //n
            OBSTACLE, //o
            PIT, //p
            HAZARD //h
        }

        public Tile(Type t)
        {
            Thread.Sleep(rand.Next(1,10));
            switch (t)
            {
                case Type.NORMAL: 
                    canMoveThrough = true;
                    canShootThrough = true;
                    damageHazard = false;
                    texture = textures["floor_" + rand.Next(1,9)];
                    break;

                case Type.OBSTACLE:
                    canMoveThrough = false;
                    canShootThrough = false;
                    damageHazard = false;
                    texture = Game1.sprites["crate"]["crate"];
                    break;

                case Type.PIT:
                    canMoveThrough = false;
                    canShootThrough = true;
                    damageHazard = false;
                    texture = Game1.sprites["hole"]["hole"];
                    break;

                case Type.HAZARD:
                    canMoveThrough = true;
                    canShootThrough = true;
                    damageHazard = true;
                    texture = textures["floor_spikes_anim_f3"];
                    break;
            }

        }

        public void SetTexture(Texture2D t)
        {
            texture = t;
        }

        public void Draw(SpriteBatch b, int x, int y, int size)
        {
            if(texture != null)
                b.Draw(texture, new Rectangle(x, y, size, size), Color.White);
        }
    }
}
