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
using MirrorKnight.Game_Objects;

namespace MirrorKnight.Game_Objects
{
    public class Inventory
    {
        List<PassiveItem> items;

        ActiveItem active;

        public Inventory()
        {
            items = new List<PassiveItem>();
        }

        public void Update()
        {
            active.Update();
        }

        public int getHPMod()
        {
            int boost = 0;
            foreach(PassiveItem i in items)
            {
                boost += i.GetHPMod();
            }
            return boost;
        }

        public double getDMGMod()
        {
            double boost = 1;
            foreach (PassiveItem i in items)
            {
                boost *= i.GetDMGMod();
            }
            if(active != null && active.IsActive())
            {
                boost *= active.GetDMGMod();
            }
            return boost;
        }

        public double getSPDMod()
        {
            double boost = 1;
            foreach (PassiveItem i in items)
            {
                boost *= i.GetSPDMod();
            }
            if (active != null && active.IsActive())
            {
                boost *= active.GetSPDMod();
            }
            return boost;
        }

        public void Add(PassiveItem i)
        {
            items.Add(i);
        }

        public void Add(ActiveItem i, Player p)
        {
            if(active != null)
            {
                Game1.entities.Add(new WorldItem((int)p.body.getX(), (int)p.body.getY(), Game1.placeHc));
            }
            active = i;
        }
    }
}
