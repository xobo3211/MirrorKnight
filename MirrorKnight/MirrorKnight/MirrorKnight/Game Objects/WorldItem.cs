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
using System.Text.RegularExpressions;
using MirrorKnight.Game_Objects;

namespace MirrorKnight.Game_Objects
{
    public class WorldItem : Entity
    {
        bool isPassive;

        int id = 1;

        static int size = 40;

        public WorldItem(int x, int y, Texture2D t)
        {
            Random rn = new Random();

            //double chance = rn.NextDouble();
            double chance = 1;

            if (chance <= 0.5)
            {
                isPassive = true;
            }
            else isPassive = false;

            if (isPassive)
            {
                id = rn.Next(1) + 1;                            //Determines what item is generated, bound of next being the number of items to choose from
            }
            else id = rn.Next(1) + 1;

            body = new Sprite(x, y);
            body.addTexture(t);
            body.setSize(40, 40);
            body.setDepth(Game1.ENTITY);
        }

        public bool Intersects(Rectangle r)
        {
            return body.getRectangle().Intersects(r);
        }

        public bool IsPassive()
        {
            return isPassive;
        }

        public int getID()
        {
            return id;
        }
    }
}
