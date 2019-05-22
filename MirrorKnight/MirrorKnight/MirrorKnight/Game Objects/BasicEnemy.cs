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

namespace MirrorKnight
{
    public class BasicEnemy : LivingEntity
    {

        public enum Behavior       //Handles enemy behavior
        {
            Idle,           //Do nothing
            ChasePlayer,    //Run at player
        }

        public const int Basic = 0, Shotgun = 1;

        protected int[] firingTime = new int[] { 60, 90 };                   //Delay after firing before next action taken by the enemy
        protected double[] probability = new double[] { 1, 0.3};          //Percent probability for the different firing types at different moments, first one must be 1 as it's the default value if all others fail



        protected Behavior b = Behavior.ChasePlayer;                 //Default behavior is idle

        int fireType = Shotgun;     //Default firing type is basic

        protected Player p;

        protected int bulletCount = 5;                        //Bullet count that will be shot each time enemy shoots. Default is 1

        protected Vector2 velocity = new Vector2(0, 0);

        protected List<int> actionList = new List<int>();   //Contains list of future actions
        protected List<int> timeList = new List<int>();                   //Contains time left until future actions

        protected int bufferSize = 8;                                     //Number of how many actions are chosen in advance

        public BasicEnemy(int x, int y, Texture2D texture, Player p) : base(x, y, texture)
        {
            Base_HP = 15;
            HP = Base_HP;

            body.setScale(2.5);
            body.translate(-body.getWidth() / 2, -body.getHeight() / 2);
            body.centerOrigin();
            body.setGroup("enemy");

            this.p = p;
        }

        public override void Update()
        {
            while(actionList.Count < bufferSize)
            {
                Random rn = new Random();

                Thread.Sleep(rn.Next(1, 6));

                double chance = rn.NextDouble();

                int type = Basic;

                for(int i = 1; i < probability.Length; i++)
                {
                    if(chance <= probability[i])
                    {
                        type = i;
                    }
                }

                actionList.Add(type);
                timeList.Add(firingTime[(int)type]);
            }

            timeList[0]--;

            if(timeList[0] <= 0)                    //Handles shooting
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

        public virtual void Fire(int t)
        {
            Vector2 pos = GetOriginPos();

            Vector2 velocity = p.GetOriginPos() - pos;
            velocity.Normalize();
            velocity *= Projectile.defaultShotSpeed;

            Projectile proj = new Projectile(pos, velocity);

            switch (t)
            {
                case Basic:

                    Game1.bulletReg.Play(0.2f, 0, 0);

                    Game1.projectiles.Add(proj);
                    break;

                case Shotgun:

                    Game1.bulletShotgun.Play(0.2f, 0, 0);

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

                    Console.WriteLine(Game1.projectiles.Count);

                    break;
            }
        }

        public void SetFiringType(int t)
        {
            fireType = t;
        }
    }
}
