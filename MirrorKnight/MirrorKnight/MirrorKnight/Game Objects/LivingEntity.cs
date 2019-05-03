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
        protected int MAX_HP, HP;
        protected float SPEED, DMG;

        public int GetHP()
        {
            return HP;
        }

        public int GetMaxHP()
        {
            return MAX_HP;
        }

        public float GetSpeed()
        {
            return SPEED;
        }

        public float GetDamage()
        {
            return DMG;
        }

        public void Move(Room currentRoom, Vector2 moveVec)
        {
            
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
    }
}
