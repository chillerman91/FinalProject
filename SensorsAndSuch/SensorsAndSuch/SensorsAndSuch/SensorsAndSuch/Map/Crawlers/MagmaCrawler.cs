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
    public class MagmaCrawler : SecondaryCrawler
    {
        //private Color BaseColor = Color.ForestGreen;
        public MagmaCrawler(int X, int Y, BaseCrawler attatched)
            : base(X: X, Y: Y, col: Color.OrangeRed, attatched: attatched)
        {
            leftChance = 30;
            rotRate = 0;
        }
    }
}
