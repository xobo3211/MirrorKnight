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

        public const int Basic = 0, Shotgun = 1, Burst = 2;

        int[] firingTime = new int[] { 60, 90, 120 };                   //Delay after firing before next action taken by the enemy
        double[] probability = new double[] { 1, 0.3, 0.2 };          //Percent probability for the different firing types at different moments, first one must be 1 as it's the default value if all others fail



        Behavior b = Behavior.ChasePlayer;                 //Default behavior is idle

        int fireType = Shotgun;     //Default firing type is basic

        Player p;

        int bulletCount = 5;                        //Bullet count that will be shot each time enemy shoots. Default is 1

        Vector2 velocity = new Vector2(0, 0);

        List<int> actionList = new List<int>();   //Contains list of future actions
        List<int> timeList = new List<int>();                   //Contains time left until future actions

        int bufferSize = 8;                                     //Number of how many actions are chosen in advance

        public BasicEnemy(int x, int y, Texture2D texture, Player p) : base(x, y, texture)
        {
            body.setScale(2.5);
            body.translate(-body.getWidth() / 2, -body.getHeight() / 2);
            body.centerOrigin();

            this.p = p;
        }

        public override void Update()
        {
            while(actionList.Count < bufferSize)
            {
                Random rn = new Random();

                Thread.Sleep(rn.Next(3));

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

        public void Fire(int t)
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

                case Burst:

                    Game1.projectiles.Add(proj);

                    Random rn = new Random();

                    for(int i = 1; i < bulletCount * 3; i++)
                    {
                        Projectile p = new Projectile(pos, velocity);

                        double tempRotate = rn.NextDouble() * Math.PI * 2;

                        p.RotateVelocity((float)tempRotate);

                        Game1.projectiles.Add(p);
                    }

                    break;
            }
        }

        public void SetFiringType(int t)
        {
            fireType = t;
        }
    }
}
