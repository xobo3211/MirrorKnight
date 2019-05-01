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

namespace MirrorKnight
{
    public class BasicEnemy : LivingEntity
    {

        public enum Behavior       //Handles enemy behavior
        {
            Idle,           //Do nothing
            ChasePlayer,    //Run at player
        }

        public enum FiringType     //Handles the different types of firing from enemies
        {
            Basic,          //Fires single shots
            Shotgun,        //Fires multiple shots at a target
            Burst           //Fires multiple shots in random directions
        }

        Behavior b = Behavior.Idle;                 //Default behavior is idle

        FiringType fireType = FiringType.Basic;     //Default firing type is basic

        Player p;

        int fireTimer = 0, fireTimerMax = 120;

        int bulletCount = 1;                        //Bullet count that will be shot each time enemy shoots. Default is 1

        Vector2 velocity = new Vector2(0, 0);

        public BasicEnemy(int x, int y, Texture2D texture, Player p)
        {
            body = new Sprite(x, y);
            body.addTexture(texture);
            body.setSize(25, 40);
            body.centerOrigin();

            this.p = p;
        }

        public override void Update()
        {
            fireTimer++;

            if (fireTimer >= fireTimerMax)          //Handle shooting
            {
                fireTimer = 0;
                Fire(fireType);
            }


            switch (b)              //Handle movement
            {
                case Behavior.Idle:
                    break;

                case Behavior.ChasePlayer:
                    velocity = GetOriginPos() - p.GetOriginPos();
                    velocity.Normalize();
                    velocity *= SPEED;
                    body.translate(velocity);
                    break;
            }
        }

        public void Fire(FiringType t)
        {
            Vector2 pos = GetOriginPos();

            Vector2 velocity = p.GetOriginPos() - pos;
            velocity.Normalize();
            velocity *= Projectile.defaultShotSpeed;

            Projectile proj = new Projectile(pos, velocity);

            switch (t)
            {
                case FiringType.Basic:
                    Game1.projectiles.Add(proj);
                    break;

                case FiringType.Shotgun:

                    double spread = Math.PI / 4;
                    //double iteration = spread * 2.0 / (bulletCount - 1);
                    double iteration = Math.PI / 8;
                    float rotation = 0;

                    int flip = -1;
                    for (int i = 0; i < bulletCount; i++)
                    {
                        
                        Projectile temp = new Projectile(pos, velocity);
                        temp.RotateVelocity(rotation);
                        Game1.projectiles.Add(temp);

                        if (i % 2 == 0)
                        {
                            rotation = Math.Abs(rotation) + (float)iteration;
                        }
                        rotation *= flip;
                        flip *= -1;

                    }
                    break;

                case FiringType.Burst:
                    break;
            }
        }

        public void SetFiringType(FiringType t)
        {
            fireType = t;
        }
    }
}
