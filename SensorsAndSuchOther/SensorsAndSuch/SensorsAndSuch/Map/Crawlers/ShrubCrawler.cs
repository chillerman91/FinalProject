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
    public class ShrubCrawler : BaseCrawler
    {
        //private Color BaseColor = Color.ForestGreen;
        public ShrubCrawler(int X, int Y)//, BaseCrawler attatched)
            : base(X: X, Y: Y, col: Color.LimeGreen)//, attatched: attatched)
        {
            forwardChance /= 2;

            rightChance = 0;
        }
    }
}
