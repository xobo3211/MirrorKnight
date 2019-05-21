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
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace MirrorKnight.Game_Objects
{
    public class BossEnemy : BasicEnemy
    {
        public BossEnemy(int x, int y, Texture2D texture, Player p) : base(x, y, texture, p)
        {

        }
    }
}
