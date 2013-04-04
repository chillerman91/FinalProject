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

namespace SensorsAndSuch.Maps
{
    public class ForestCrawler : BaseCrawler
    {
        //private Color BaseColor = Color.ForestGreen;
        public ForestCrawler(int X, int Y)
            : base(X: X, Y: Y, col: Color.ForestGreen)
        {
            //radialDist = 150;
            //radialreset = 4;
            DeleteWallThreshold = 4;
            CreateWallThreshold = 10;
        }
    }
}
