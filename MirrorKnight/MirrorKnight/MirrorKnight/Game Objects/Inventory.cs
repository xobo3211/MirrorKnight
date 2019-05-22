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

        public ActiveItem active;

        int timer = 0, timerMax = 120;

        bool timerStarted = false;

        static Bar activeBar, cooldownBar;

        public Inventory()
        {
            items = new List<PassiveItem>();
        }

        public void Update()
        {
            active.Update();


            /////////////////////////////Item pickup timer, creates a delay  between pickups so as to not create an infinite loop when dropping an active
            if (timerStarted)
            {
                timer++;
                if (timer >= timerMax)
                {
                    timer = 0;
                    timerStarted = false;
                }
            }

            activeBar.setVal(active.currentDuration);
            cooldownBar.setVal(active.cooldownDuration);
            
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
            timerStarted = true;
        }

        public void Add(ActiveItem i, Player p)
        {
            if(active != null)
            {
                Game1.entities.Add(new WorldItem((int)p.body.getX(), (int)p.body.getY(), Game1.placeHc));
            }
            active = i;

            timerStarted = true;

            if(activeBar != null)
            {
                activeBar.change(body => body.deleteThis());
                cooldownBar.change(body => body.deleteThis());
            }

            activeBar = new Bar(Useful.getWWidth() - 80, 50, 50, 15, active.maxDuration);
            cooldownBar = new Bar(Useful.getWWidth() - 80, 50, 50, 15, active.cooldown);

            cooldownBar.change(body => body.setVisible(false));
        }

        public bool HasActive()
        {
            return active != null;
        }

        public Texture2D GetActiveImage()
        {
            return active.GetImage();
        }

        public bool CanPickUp()
        {
            return !timerStarted;
        }

        public void Activate()
        {
            active.Activate();
        }

        public static void FlipBars()
        {
            activeBar.change(body => body.setVisible(!body.isVisable()));
            cooldownBar.change(body => body.setVisible(!body.isVisable()));
        }
    }
}
