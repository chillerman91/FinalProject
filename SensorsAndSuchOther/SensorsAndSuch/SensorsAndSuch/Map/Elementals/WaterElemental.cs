using System.Collections.Generic;

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
    public class WaterElemental: BaseElemental
    {
        //private Color BaseColor = Color.ForestGreen;
        public WaterElemental(int X, int Y)
            :base(X: X, Y: Y, col: Color.Blue)
        {
            leftChance = 30;
            rightChance = 30;
            forwardChance = 20;
            DeleteWallThreshold = -50;
            CreateWallThreshold = 30;
            Dislikes.Add(() => ForestElemental.BaseColor);
            Dislikes.Add(() => Wall.GetColor());
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
