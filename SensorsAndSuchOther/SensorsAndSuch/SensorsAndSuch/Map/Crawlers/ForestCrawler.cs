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
        internal static Color BaseColor = Color.ForestGreen;
        public ForestCrawler(int X, int Y)
            : base(X: X, Y: Y, col: BaseColor)
        {
            forwardChance = 80;// out of 100
            leftChance = 0;
            rightChance = 0;
            DeleteWallThreshold = 10;
            CreateWallThreshold = 20;
        }
    }
}
