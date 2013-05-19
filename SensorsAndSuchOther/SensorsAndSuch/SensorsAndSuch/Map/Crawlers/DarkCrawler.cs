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
    public class DarkCrawler : BaseCrawler
    {
        public static Color BaseColor = Color.Purple;
        public DarkCrawler(int X, int Y)//, BaseCrawler attatched)
            : base(X: X, Y: Y, col: BaseColor)//, attatched: attatched)
        {
            forwardChance /= 2;

            rightChance = 0;
            DeleteWallThreshold = 20;
            CreateWallThreshold = -5;
            this.X = X / 10 * 10;
            this.Y = Y / 10 * 10;
            Dislikes.Add(() => BaseColor);
            Dislikes.Add(() => LightCrawler.BaseColor);
        }
        /*
        protected override void SetPreferedDir(List<BaseTile>[,] Grid)
        {
            //Add options
            CanMod = true;
            Vector2 newDir;
            if (Globals.rand.Next(100) > 10)
            {
                dir = dir.Flip()*(Globals.rand.Next(2)*2-1);
            }
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
