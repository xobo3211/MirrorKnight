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
    public class Player : LivingEntity
    {
        ActiveItem active;

        List<PassiveItem> passives;

        float RANGE = 70f;

        public Player()
        {
            body = new Sprite(10, 10);
            body.setVisible(false);
            SPEED = 3f;

            MAX_HP = 6;
            HP = MAX_HP;

            passives = new List<PassiveItem>();
        }

        public void load()
        {
            body.setAnimation(true);
            body.useRegion(true);
            body.addTexture("packed/knight/idle");
            
            for (int i = 0; i < 4; i++)
            {
                body.defRegion(i*16, 0, 16, 28);
            }
            body.setOrigin(5, 15);
            body.setDepth(Game1.ENTITY);

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
                hitProjectiles[i].Reflect(aimVec - (hitProjectiles[i].body.getOriginPos() - body.getOriginPos()), Projectile.defaultShotSpeed);
            }
        }

        public float CalculateRotationalDistance(Vector2 vector, Vector2 point)                 //Checks rotational distance between player's aim vector and a position
        {
            Vector2 secondVector = point - body.getOriginPos();
            vector.Normalize();
            secondVector.Normalize();

            float rotation = (float)(Math.Asin((vector - secondVector).Length() / 2));

            return rotation * 180;
        }

        public bool Intersects(Rectangle r)
        {
            return body.getRectangle().Intersects(r);
        }
    }
}
