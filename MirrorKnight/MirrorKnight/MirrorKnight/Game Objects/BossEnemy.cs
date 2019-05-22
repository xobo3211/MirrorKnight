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
using System.Threading;

namespace MirrorKnight.Game_Objects
{
    public class BossEnemy : BasicEnemy
    {

        public const int Burst = 2;

        protected new int[] firingTime = new int[] { 60, 90, 120 };                   //Delay after firing before next action taken by the enemy
        protected new double[] probability = new double[] { 1, 0.3, 0.2 };

        private Bar healthBar;


        public BossEnemy(int x, int y, Texture2D texture, Player p) : base(x, y, texture, p)
        {
            Base_HP = 120;
            HP = Base_HP;

            healthBar = new Bar(40, Useful.getWHeight() - 30, Useful.getWWidth() - 80, 30, Base_HP);
        }

        public override void Update()
        {

            healthBar.setVal(HP);

            while (actionList.Count < bufferSize)
            {
                Random rn = new Random();

                Thread.Sleep(rn.Next(1, 6));

                double chance = rn.NextDouble();

                int type = Basic;

                for (int i = 1; i < probability.Length; i++)
                {
                    if (chance <= probability[i])
                    {
                        type = i;
                    }
                }

                actionList.Add(type);
                timeList.Add(firingTime[(int)type]);
            }

            if (actionList[0] == Burst)
            {
                b = Behavior.Idle;
            }
            else b = Behavior.ChasePlayer;

            timeList[0]--;

            if (timeList[0] <= 0)                    //Handles shooting
            {
                timeList.Remove(timeList[0]);
                Fire(actionList[0]);

                actionList.Remove(actionList[0]);
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

        public override void Fire(int t)
        {
            Vector2 pos = GetOriginPos();

            Vector2 velocity = p.GetOriginPos() - pos;
            velocity.Normalize();
            velocity *= Projectile.defaultShotSpeed;

            Projectile proj = new Projectile(pos, velocity);

            switch (t)
            {
                case Basic:
                    Game1.projectiles.Add(proj);
                    break;

                case Shotgun:

                    double spread = Math.PI / 6;
                    int bulletsPerSide = bulletCount / 2;
                    double rotation, iteration;
                    rotation = iteration = spread / bulletsPerSide;

                    Game1.projectiles.Add(proj);

                    for (int i = 1; i < bulletCount; i += 2)
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

                case Burst:

                    Game1.projectiles.Add(proj);

                    Random rn = new Random();

                    for (int i = 1; i < bulletCount * 3; i++)
                    {
                        Projectile p = new Projectile(pos, velocity);

                        double tempRotate = rn.NextDouble() * Math.PI * 2;

                        p.RotateVelocity((float)tempRotate);

                        Game1.projectiles.Add(p);
                    }
                    
                    break;
            }
        }

        public new void Remove()
        {
            this.Remove();
            healthBar.change(body => body.deleteThis());
        }
    }
}
