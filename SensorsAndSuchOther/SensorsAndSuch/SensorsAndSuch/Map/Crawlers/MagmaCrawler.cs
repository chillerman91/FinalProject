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
    public class MagmaCrawler : BaseCrawler
    {
        //, BaseCrawler attatched, attatched: attatched
        internal static Color BaseColor = Color.OrangeRed;
        public MagmaCrawler(int X, int Y)
            : base(X: X, Y: Y, col: BaseColor)
        {
            leftChance = 30;
            forwardChance = 0;

            Likes.Add(() => FireCrawler.BaseColor);
        }
    }
}
