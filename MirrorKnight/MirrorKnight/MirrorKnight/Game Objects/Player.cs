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
    class Player : LivingEntity
    {
        ActiveItem active;

        List<PassiveItem> passives;

        float RANGE = 50f;

        public Player()
        {
            body = new Sprite(10, 10);

            SPEED = 3f;

            MAX_HP = 6;
            HP = MAX_HP;

            passives = new List<PassiveItem>();
        }

        public void load()
        {
            body.setAnimation(true);
            Game1.sprites["knight"].Keys.Where((str, index) => str.Contains("m_idle")).ToList().ForEach(str =>body.addTexture(Game1.sprites["knight"][str]));
            body.centerOrigin();
        }

        public void Attack(Vector2 aimVec)
        {
            List<Projectile> hitProjectiles = new List<Projectile>();

            for(int i = 0; i < Game1.projectiles.Count; i++)
            {
                Projectile p = Game1.projectiles[i];
                if((body.getOriginPos() - p.body.getOriginPos()).Length() < RANGE)
                {
                    if(CalculateRotationalDistance(aimVec, p.body.getOriginPos()) <= 160)       //Checks whether or not the projectile is in front of the player
                    {
                        hitProjectiles.Add(p);
                    }
                }
            }

            for(int i = 0; i < hitProjectiles.Count; i++)
            {
                hitProjectiles[i].Reflect(aimVec, Projectile.defaultShotSpeed);
            }
        }

        public float CalculateRotationalDistance(Vector2 vector, Vector2 point)                 //Checks rotational distance between player's aim vector and a position
        {
            Vector2 secondVector = point - body.getOriginPos();

            vector.Normalize();
            secondVector.Normalize();

            float rotation = (float)(2 * Math.Asin((vector - secondVector).Length() / 2));

            Console.WriteLine(rotation * 180);

            return rotation * 180;
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
