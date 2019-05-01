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
    public class LivingEntity : Entity
    {
        protected int MAX_HP, HP;
        protected float SPEED, DMG;

        public int GetHP()
        {
            return HP;
        }

        public int GetMaxHP()
        {
            return MAX_HP;
        }

        public float GetSpeed()
        {
            return SPEED;
        }

        public float GetDamage()
        {
            return DMG;
        }
    }
}
