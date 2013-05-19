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
    public class LightCrawler : BaseCrawler
    {
        public static Color BaseColor = Color.White;
        public LightCrawler(int X, int Y)//, BaseCrawler attatched)
            : base(X: X, Y: Y, col: Color.White)//, attatched: attatched)
        {
            forwardChance /= 2;

            rightChance = 0;
            DeleteWallThreshold = 20;
            CreateWallThreshold = -5;
            this.X = X / 10 * 10;
            this.Y = Y / 10 * 10;
        }
        /*
        protected override void SetPreferedDir(List<BaseTile>[,] Grid)
        {
            //Add options
            CanMod = true;
            if (X % 10 == 0 && Y % 10 == 0)
                dir = dir.Flip();
            if (!Globals.map.isInBounds(X + (int)dir.X, Y + (int)dir.Y, Offset: 1))
                dir = BaseTile.GetRandDir();
        }

        public override void TakeTurn(List<BaseTile>[,] Grid, bool canModWalls = true)
        {
            //pick a direction
            //if (age > AvgLife) return;

            SetPreferedDir(Grid);
            //Effect it or not
            if (Globals.map.isInBounds(X + (int)dir.X, Y + (int)dir.Y, Offset: 1))
            {
                //Move there
                X += (int)dir.X;
                Y += (int)dir.Y;
                previousPlaces.Add(new Vector2(X, Y));
                if (previousPlaces.Count > 3)
                    previousPlaces.RemoveAt(0);
                if (canModWalls)
                    ModWalls(Grid, X, Y);
                EffectAdj(Grid, previousPlaces[0]);
            }
            age++;
        }*/
    }
}
