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

namespace MirrorKnight
{
    public class ActiveItem
    {
        int HP_MOD = 0;                     //Additive boost to health
        double DMG_MOD = 1, SPD_MOD = 1;    //Multiplicative boost to other stats

        List<int> uniqueEffectIDs;                  //Contains the effect ID of unique effects outside of stat boosts (ex: multi-shot, bouncing bullets, etc.)

        Texture2D texture;


        bool isActive = false, isOnCooldown = false;

        public int maxDuration, currentDuration, cooldown, cooldownDuration;

        public ActiveItem(int id, Texture2D t)
        {
            string path = "../../../../MirrorKnightContent/items/active/" + id + ".txt";

            texture = t;

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {

                    string first = reader.ReadLine();

                    if(first != "0")
                    {
                        maxDuration = Convert.ToInt32(first);
                        currentDuration = maxDuration;

                        Console.WriteLine(currentDuration);

                        cooldown = maxDuration * 2;
                    }

                    while (!reader.EndOfStream)
                    {
                        string[] args = reader.ReadLine().Split(' ');

                        switch (args[0])
                        {

                            case "spd":
                                SPD_MOD = Convert.ToDouble(args[1]);
                                break;

                            case "dmg":
                                DMG_MOD = Convert.ToDouble(args[1]);
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Activate()
        {
            isActive = true;
        }

        public void Update()
        {

            if (isActive)
            {
                currentDuration--;

                Player.invincible = true;

                if (currentDuration <= 0)
                {
                    Player.invincible = false;

                    isActive = false;
                    currentDuration = maxDuration;

                    isOnCooldown = true;

                    MirrorKnight.Game_Objects.Inventory.FlipBars();
                }
            }
            else if(isOnCooldown)
            {
                cooldownDuration++;

                if(cooldownDuration >= cooldown)
                {
                    isOnCooldown = false;
                    cooldownDuration = 0;

                    MirrorKnight.Game_Objects.Inventory.FlipBars();
                }
            }
        }

        public bool IsActive()
        {
            return isActive;
        }

        public bool IsCoolingDown()
        {
            return isOnCooldown;
        }

        public int GetHPMod()
        {
            return HP_MOD;
        }

        public double GetDMGMod()
        {
            return DMG_MOD;
        }

        public double GetSPDMod()
        {
            return SPD_MOD;
        }

        public Texture2D GetImage()
        {
            return texture;
        }
    }
}
