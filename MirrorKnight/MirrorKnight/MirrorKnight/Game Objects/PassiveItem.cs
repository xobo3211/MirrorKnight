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

namespace MirrorKnight
{
    public class PassiveItem
    {

        int HP_MOD = 0;                     //Additive boost to health
        double DMG_MOD = 1, SPD_MOD = 1;    //Multiplicative boost to other stats

        List<int> uniqueEffectIDs;                  //Contains the effect ID of unique effects outside of stat boosts (ex: multi-shot, bouncing bullets, etc.)

        public PassiveItem(int id)
        {
            string path = "../../../../MirrorKnightContent/items/passive/" + id + ".txt";

            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    while(!reader.EndOfStream)
                    {
                        string[] args = reader.ReadLine().Split(' ');

                        switch(args[0])
                        {
                            case "hp":
                                HP_MOD = Convert.ToInt32(args[1]);
                                break;

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

            }
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

    }
}
