﻿using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Content;
using SensorsAndSuch.Maps;
using SensorsAndSuch.Mobs;
using SensorsAndSuch.Texts;
using SensorsAndSuch.Extensions;

namespace SensorsAndSuch.Maps
{
    public class FireCrawler: BaseCrawler
    {
        //private Color BaseColor = Color.ForestGreen;
        public FireCrawler(int X, int Y)
            :base(X: X, Y: Y, col: Color.Red)
        {
            leftChance = 30;
            rotRate = 20;
            DeleteWallThreshold = -5;
            CreateWallThreshold = -5;
            //radialreset = 30;
        }
        /*
        protected void SetPreferedDir(List<BaseTile>[,] Grid)
        {
            if (Grid[X + (int)dir.X, Y + (int)dir.Y].Count == 1 && Grid[X + (int)dir.X, Y + (int)dir.Y][0].color.G > 100)
                return;
            int randNumb = Globals.rand.Next(rotRate + leftChance + rightChance);
            randNumb -= rotRate;
            if (randNumb > 0 && randNumb <= leftChance)
                dir = dir.Flip();
            else if (randNumb > 0 && randNumb >= leftChance)
                dir = dir.Flip() * -1;
            if (Globals.map.isInBounds(X + (int)dir.X, Y + (int)dir.Y))
            {
                X += (int)dir.X;
                Y += (int)dir.Y;
                AddDirt(Grid, X, Y);
            }
        }

        public void TakeTurn(List<BaseTile>[,] Grid)
        {
            SetPreferedDir(Grid);
            if (Globals.map.isInBounds(X + (int)dir.X, Y + (int)dir.Y))
            {
                X += (int)dir.X;
                Y += (int)dir.Y;
                AddDirt(Grid, X, Y);
            }
        }
         */
    }
}
