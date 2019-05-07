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

        bool invincible = false;

        int invincibilityTimer = 0, invincibilityMax = 60;

        public Player()
        {
            body = new Sprite(10, 10);
            body.setVisible(false);
            SPEED = 3f;

            MAX_HP = 6;
            HP = MAX_HP;

            passives = new List<PassiveItem>();

            Vector2 movement = Vector2.Zero;
            KeyControl.addKeyPress(Keys.W, () => movement.Y = -1);
            KeyControl.addKeyPress(Keys.S, () => movement.Y = 1);
            KeyControl.addKeyPress(Keys.A, () => movement.X = -1);
            KeyControl.addKeyPress(Keys.D, () => movement.X = 1);
            ControllerControl.add(gp =>
            {
                if (gp.ThumbSticks.Left != Vector2.Zero)
                {
                    movement = gp.ThumbSticks.Left;
                    movement.Y *= -1;
                }
                if (gp.ThumbSticks.Right != Vector2.Zero)
                {
                    movement = gp.ThumbSticks.Right;
                }
            });
            MouseState oldM = Mouse.GetState();
            body.setUpdate(() =>
            {
                if (movement != Vector2.Zero)
                {
                    movement.Normalize();
                    movement *= GetSpeed();
                    Move(Game1.map.GetRoom(Game1.x, Game1.y), movement);
                    movement = Vector2.Zero;
                }

                MouseState m = Mouse.GetState();
                if (m.LeftButton == ButtonState.Pressed && oldM.LeftButton == ButtonState.Released)
                {
                    Vector2 playerAimVec = new Vector2(m.X, m.Y) - body.getOriginPos();
                    Attack(playerAimVec);
                }
                oldM = m;
            });
            
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

        public override void Update()
        {
            //////////////////////Invincibility frames logic
            if(invincible)
            {
                invincibilityTimer++;
            }
            if(invincibilityTimer >= invincibilityMax)
            {
                invincible = false;
                invincibilityTimer = 0;
            }

            ////////////////Player getting damaged logic

            for(int i = 0; i < Game1.projectiles.Count; i++)
            {
                if(Game1.projectiles[i].body.intersects(body))
                {
                    Game1.projectiles[i].Remove();
                    Game1.projectiles.Remove(Game1.projectiles[i]);

                    if (!invincible)
                    {
                        HP--;
                        invincible = true;
                    }
                }
            }

            Vector2 start = Game1.PixelToTileCoords(GetPos());
            Vector2 end = Game1.PixelToTileCoords(GetPos() + new Vector2(body.getWidth(), body.getHeight()));

            Console.WriteLine(start + " : " + end);

            if (hasFlight)
            {
                for (int y = (int)start.Y; y <= end.Y; y++)
                {
                    for (int x = (int)start.X; x <= end.X; x++)
                    {
                        if (Game1.GetCurrentRoom().isTileHazardous(x, y))
                        {
                            if (!invincible)
                            {
                                HP--;
                                invincible = true;
                            }
                        }
                    }
                }
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
