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

        Behavior b = Behavior.ChasePlayer;                 //Default behavior is idle

        FiringType fireType = FiringType.Shotgun;     //Default firing type is basic

        Player p;

        int fireTimer = 0, fireTimerMax = 120;

        int bulletCount = 5;                        //Bullet count that will be shot each time enemy shoots. Default is 1

        Vector2 velocity = new Vector2(0, 0);

        public BasicEnemy(int x, int y, Texture2D texture, Player p) : base(x, y, texture)
        {
            body.setScale(2.5);
            body.translate(-body.getWidth() / 2, -body.getHeight() / 2);
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
                    velocity = p.GetOriginPos() - GetOriginPos();
                    velocity.Normalize();
                    velocity *= SPEED;
                    Move(Game1.GetCurrentRoom(), velocity);

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

                    double spread = Math.PI / 6;
                    int bulletsPerSide = bulletCount / 2;
                    double rotation, iteration;
                    rotation = iteration = spread / bulletsPerSide;

                    Game1.projectiles.Add(proj);

                    for(int i = 1; i < bulletCount; i += 2)
                    {
                        Projectile left, right;
                        left = new Projectile(pos, velocity);
                        right = new Projectile(pos, velocity);

                        left.RotateVelocity(-(float)rotation);
                        right.RotateVelocity((float)rotation);

                        Game1.projectiles.Add(left);
                        Game1.projectiles.Add(right);

                        rotation += iteration;
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
