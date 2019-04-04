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
    public class Pickup
    {
        Rectangle hitbox;
        Texture2D texture;

        public bool Collides(Rectangle rect)
        {
            return hitbox.Intersects(rect);
        }
    }
}
