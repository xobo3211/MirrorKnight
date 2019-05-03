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
    public class Projectile
    {
        public Sprite body;
        public Vector2 velocity;

        float shotSpeed = 5f;

        public static float defaultShotSpeed = 5f, defaultBulletSize = 10f;

        bool canHurtEnemies;
 
        //pos = starting position, vel = velocity it will travel at
        public Projectile(Vector2 pos, Vector2 vel)
        {
            body = new Sprite(pos.X, pos.Y);
            Load();
            body.setPos(pos);
            body.setSize(defaultBulletSize, defaultBulletSize);
            velocity = vel;
            velocity.Normalize();
            velocity *= shotSpeed;
        }

        public void Update()
        {
            body.translate(velocity);
        }

        public void Load()
        {
            body.setAnimation(false);
            body.addTexture(Game1.enemyBullet);
        }

        public void Dispose()
        {
            body.deleteThis();
        }

        public void RotateVelocity(float rotation)
        {
            if (rotation == 0)
                return;

            float baseRotation = (float)Math.Atan(velocity.Y / velocity.X);
            rotation += baseRotation;

            if (velocity.X > 0)
            {
                velocity.X = (float)Math.Cos(rotation) * velocity.Length();
                velocity.Y = (float)Math.Sin(rotation) * velocity.Length();
            }
            else
            {
                velocity.X = -(float)Math.Cos(rotation) * velocity.Length();
                velocity.Y = -(float)Math.Sin(rotation) * velocity.Length();
            }
            
        }

        public void Reflect(Vector2 aimVec, float speed)
        {
            aimVec.Normalize();
            velocity = aimVec * speed;
            body.addTexture(Game1.reflectedBullet);
            body.setTexture(1);
            body.setSize(defaultBulletSize, defaultBulletSize);
        }

        public bool CanHurtEnemies()
        {
            return canHurtEnemies;
        }

        public void HurtsEnemies(bool value)
        {
            canHurtEnemies = value;
        }
    }
}
