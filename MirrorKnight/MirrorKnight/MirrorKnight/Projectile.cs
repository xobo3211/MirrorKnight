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
        Texture2D texture;
        Vector2 position, velocity;

        public Projectile(Texture2D t, Vector2 pos, Vector2 vel)
        {
            texture = t;
            position = pos;
            velocity = vel;
        }

        public void Update()
        {
            position += velocity;
        }

        public void Draw(SpriteBatch b)
        {
            b.Draw(texture, position, Color.White);
        }
    }
}
