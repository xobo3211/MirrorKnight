using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace MirrorKnight.Game_Objects
{
    class Hitbar
    {
        public int initialX;
        public int x;
        public int y;
        public int height;
        public int width;
        public int percent;

        public Rectangle rect;
        public Texture2d texture;
        private int v1;
        private int v2;
        private int v3;
        private int v4;
        private Texture redBlockThing;

        public Hitbar(int ex, int why, int h, int w)
        {
            x = ex;
            initialX = x;
            y = why;
            height = h;
            width = w;
            percent = 100;

            rect = new Rectangle(x, y, width, height);



        }
        

        public void decrease(int a)
        {
            percent -= a;
            updateBar();
        }

        public void increase(int a)
        {
            percent += a;
            updateBar();
        }

        public void updateBar()
        {
            x = initialX * percent;

            rect.Width = x;
        }



    }
}
