using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Infinity_TD
{
    public static class RuneManager
    {
        public static int[] RuneBag = new int[10];
        public static bool[] CheckRecipe = new bool[12];
        public static String[] RecipeBag = new String[12];
        public static Combinator.Runes[] RuneList = new Combinator.Runes[10];

        public static void RemoveRune(int i, int quantid)
        {
            RuneBag[i] -= quantid;
        }

        public static void InsertRune(int i, int quantid)
        {
            RuneBag[i] += quantid;
        }

        public static void AddExistingRune(Combinator.Runes rune)
        {
            switch (rune)
            {
                case Combinator.Runes.FIRE:
                    RuneBag[0]++;
                    break;
                case Combinator.Runes.WATER:
                    RuneBag[1]++;
                    break;
                case Combinator.Runes.AIR:
                    RuneBag[2]++;
                    break;
                case Combinator.Runes.EARTH:
                    RuneBag[3]++;
                    break;
                case Combinator.Runes.LIGHTNING:
                    RuneBag[4]++;
                    break;
                case Combinator.Runes.NATURE:
                    RuneBag[5]++;
                    break;
                case Combinator.Runes.LIGHT:
                    RuneBag[6]++;
                    break;
                case Combinator.Runes.DARKNESS:
                    RuneBag[7]++;
                    break;
                case Combinator.Runes.COSMIC:
                    RuneBag[8]++;
                    break;

            }
        }


        public static void RecipeLoad()
        {

            #region RecipeArrays
            RecipeBag[0] = "Fireball Rune \nFire + Fire + Air";
            RecipeBag[1] = "Glacier Rune \nWater + Water + Air";
            RecipeBag[2] = "Tornado Rune \nAir + Air + Air";
            RecipeBag[3] = "Thunderstorm Rune \nWater + Air + Thunder";
            RecipeBag[4] = "Earthquake Rune \nEarth + Earth + Fire";
            RecipeBag[5] = "Corrosive Rune \nNature + Nature + Earth";
            RecipeBag[6] = "Magmatic Rune \nFire + Earth + Fire";
            RecipeBag[7] = "Glyph of Blinding Light \nLight + Light + Thunder";
            RecipeBag[8] = "Glyph of Dark Flames \nDark + Fire + Dark";
            RecipeBag[9] = "Glyph of the Sun \nCosmic + Light + Fire";
            RecipeBag[10] = "Glyph of Black Hole \nCosmic + Dark + Dark";
            RecipeBag[11] = "Essence of Infinity \nInfinity x 3";

            CheckRecipe[0] = true;
            CheckRecipe[1] = true;
            CheckRecipe[2] = true;
            CheckRecipe[3] = true;
            CheckRecipe[4] = true;
            CheckRecipe[5] = true;
            CheckRecipe[6] = true;
            CheckRecipe[7] = true;
            CheckRecipe[8] = true;
            CheckRecipe[9] = true;
            CheckRecipe[10] = true;
            CheckRecipe[11] = true;
            #endregion

            #region RuneArrays

            RuneList[0] = Combinator.Runes.FIRE;
            RuneList[1] = Combinator.Runes.WATER;
            RuneList[2] = Combinator.Runes.AIR;
            RuneList[3] = Combinator.Runes.EARTH;
            RuneList[4] = Combinator.Runes.LIGHTNING;
            RuneList[5] = Combinator.Runes.NATURE;
            RuneList[6] = Combinator.Runes.LIGHT;
            RuneList[7] = Combinator.Runes.DARKNESS;
            RuneList[8] = Combinator.Runes.COSMIC;
            RuneList[9] = Combinator.Runes.INFINITY;

            #endregion

        }

        public static void OpenRecipe(int i)
        {

            CheckRecipe[i] = true;

        }
    }
}
