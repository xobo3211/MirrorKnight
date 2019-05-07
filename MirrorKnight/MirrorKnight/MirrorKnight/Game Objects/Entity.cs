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
    public abstract class Entity
    {
        public Sprite body;

        public Entity(int x, int y, Texture2D t)
        {
            body = new Sprite(x, y);
            body.addTexture(t);
            body.setDepth(Game1.ENTITY);
        }

        public Entity()
        {

        }

        public Vector2 GetPos()
        {
            return body.getPos();
        }

        public Vector2 GetOriginPos()
        {
            return body.getOriginPos();
        }

        public Rectangle GetRectangle()
        {
            return body.getRectangle();
        }

        public void Remove()
        {
            body.deleteThis();
        }

        public virtual void Update()
        {

        }
    }
}
