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
    public class Turret : Entity
    {

        int fireTimer = 0, timerCount = 40;

        float bulletSpeed = 2f;

        public Turret(int x, int y, Texture2D texture)
        {
            body = new Sprite(x, y);
            body.addTexture(texture);
            body.setSize(25, 25);
        }

        public void Update(Entity target)
        {
            fireTimer++;

            if(fireTimer >= timerCount)
            {
                fireTimer = 0;

                Vector2 aimVec = target.body.getPos() - body.getPos();
                aimVec.Normalize();
                aimVec *= bulletSpeed;

                Projectile p = new Projectile(body.getPos(), aimVec);
            }
        }
    }
}
