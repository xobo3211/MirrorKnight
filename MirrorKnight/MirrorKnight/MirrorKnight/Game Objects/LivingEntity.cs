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
    public class LivingEntity : Entity
    {
        protected int Base_HP = 20, HP = 20;
        protected float SPEED = 1f, DMG = 1f;

        protected bool hasFlight;             //Flight allows entities to ignore hazards, pits, and obstacles when moving

        protected Rectangle hitbox;

        public LivingEntity(int x, int y, Texture2D t) : base(x, y, t)
        {

        }
        public LivingEntity()
        {

        }

        public int GetHP()
        {
            return HP;
        }

        public virtual int GetMaxHP()
        {
            return Base_HP;
        }

        public virtual float GetSpeed()
        {
            return SPEED;
        }

        public virtual double GetDamage()
        {
            return DMG;
        }

        public void Move(Room currentRoom, Vector2 moveVec)
        {
            Vector2 tempFinalPos = body.getOriginPos() + moveVec;
            
            Vector2 left = tempFinalPos - new Vector2(hitbox.Width / 2, 0);
            Vector2 right = tempFinalPos + new Vector2(hitbox.Width / 2, 0);
            Vector2 up = tempFinalPos - new Vector2(0, hitbox.Height / 2);
            Vector2 down = tempFinalPos + new Vector2(0, hitbox.Height / 2);

            Vector2 trueFinalMoveVec = Vector2.Zero;

            if (hasFlight == false)
            {

                if (up.Y > Game1.verticalOffset && (moveVec.Y < 0 && CheckPos(currentRoom, Game1.PixelToTileCoords(up))) || (moveVec.Y > 0 && CheckPos(currentRoom, Game1.PixelToTileCoords(down))))
                {
                    trueFinalMoveVec.Y = moveVec.Y;
                }

                if (left.X > 0 && (moveVec.X < 0 && CheckPos(currentRoom, Game1.PixelToTileCoords(left))) || (moveVec.X > 0 && CheckPos(currentRoom, Game1.PixelToTileCoords(right))))
                {
                    trueFinalMoveVec.X = moveVec.X;
                }

            }
            else
            {
                if(up.Y > Game1.verticalOffset && down.Y < Useful.getWHeight())
                {
                    trueFinalMoveVec.Y = moveVec.Y;
                }

                if(left.X > 0 && right.X < Useful.getWWidth())
                {
                    trueFinalMoveVec.X = moveVec.X;
                }
            }

            body.translate(trueFinalMoveVec);
        }

        private bool CheckPos(Room r, Vector2 pos)
        {
            return r.isTileWalkableThrough((int)pos.X, (int)pos.Y);
        }


        private bool isTileWalkableThrough(Room r, Vector2 pos)
        {
            Vector2 tempPos = new Vector2(pos.X, pos.Y - (Game1.verticalOffset / 2));

            if (tempPos.X < 0 || tempPos.Y < 0)
                return false;
            else if (tempPos.X > Game1.tileSize * 18 || tempPos.Y > Game1.tileSize * 10)
                return false;

            return r.isTileWalkableThrough((int)tempPos.X / Game1.tileSize, (int)tempPos.Y / Game1.tileSize);
        }


        public bool Intersects(Rectangle r)
        {
            return hitbox.Intersects(r);
        }

        public bool Intersects(Sprite s)
        {
            return s.getRectangle().Intersects(hitbox);
        }

        public Rectangle getHitbox()
        {
            return hitbox;
        }
    }
}
