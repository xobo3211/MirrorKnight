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
    class Player : Entity
    {
        ActiveItem active;

        List<PassiveItem> passives;

        public Player()
        {
            body = new Sprite(10, 10);

            MAX_HP = 6;
            HP = MAX_HP;

            passives = new List<PassiveItem>();
        }

        public void load()
        {
            body.setAnimation(true);
            Game1.sprites["knight"].Keys.Where((str, index) => str.Contains("m_idle")).ToList().ForEach(str =>body.addTexture(Game1.sprites["knight"][str]));
        }

        public void Attack(Vector2 aimVec)
        {

        }

        public void Pickup(PassiveItem p)
        {
            passives.Add(p);
        }

        public float GetSpeed()
        {
            return SPEED;
        }
    }
}
