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
    public class Turret : Entity
    {

        int fireTimer = 0, timerCount = 75;

        Entity targetEntity;

        Vector2 target;

        bool targetsEntity;

        float bulletSpeed = 2f;

        public Turret(int x, int y, Texture2D texture, Entity target) : base(x, y, texture)
        {
            body.setSize(25, 25);
            body.centerOrigin();
            body.setScale(3);

            targetEntity = target;
            targetsEntity = true;
        }

        public Turret(int x, int y, Texture2D texture, Vector2 target) : base(x, y, texture)
        {
            body.setSize(25, 25);
            body.centerOrigin();
            body.setScale(3);

            this.target = target;
            targetsEntity = false;
        }

        public override void Update()
        {
            fireTimer++;

            if (fireTimer >= timerCount)
            {
                Fire();
            }
        }

        private void Fire()
        {
            Game1.bulletReg.Play(0.2f ,0 ,0);
            if(targetsEntity)
            {
                fireTimer = 0;
                
                Vector2 aimVec = targetEntity.body.getPos() - body.getPos();
                aimVec.Normalize();
                aimVec *= bulletSpeed;

                Projectile p = new Projectile(body.getOriginPos(), aimVec);
                p.HurtsEnemies(false);

                Game1.projectiles.Add(p);
            }
            else
            {
                fireTimer = 0;

                Projectile p = new Projectile(body.getOriginPos(), target);
                p.HurtsEnemies(false);

                Game1.projectiles.Add(p);
            }
        }
    }
}
