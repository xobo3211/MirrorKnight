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
    public class Entity
    {
        public Sprite body;

        protected int MAX_HP = 6, HP;
        protected float SPEED = 1f, DMG = 2;

        public Entity()
        {

        }
    }
}
