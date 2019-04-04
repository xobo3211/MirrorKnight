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
    class Player
    {
        Sprite body;

        Vector2 position;

        int MAX_HP = 6, HP;
        float SPEED = 1f, DMG = 2;

        ActiveItem active;

        List<PassiveItem> passives;

        public Player()
        {
            MAX_HP = 6;
            HP = MAX_HP;

            passives = new List<PassiveItem>();
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void Move(Vector2 moveVec)   //Handles player movement. moveVec assumed to be normalized
        {
            position += moveVec * SPEED;
        }

        public void Attack(Vector2 aimVec)  //
        {

        }

        public void Draw(SpriteBatch b)
        {
            body.draw(b);
        }
    }
}
