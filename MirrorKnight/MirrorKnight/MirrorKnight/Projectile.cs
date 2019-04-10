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
    public class Projectile
    {
        public Sprite body;
        Vector2 velocity;

        float shotSpeed = 5f;

        public Projectile(Vector2 pos, Vector2 vel)
        {
            body = new Sprite(pos.X, pos.Y);
            body.setPos(pos);
            velocity = vel;
            velocity.Normalize();
            velocity *= shotSpeed;
            Load();
        }

        public void Update()
        {
            body.translate(velocity);
        }

        public void Load()
        {
            body.setAnimation(true);
            Texture2D[] textures = Game1.sprites["coin"].Values.ToArray();
            foreach(Texture2D t in textures)
            {
                body.addTexture(t);
            }
        }

        public void Dispose()
        {
            body.deleteThis();
        }
    }
}
