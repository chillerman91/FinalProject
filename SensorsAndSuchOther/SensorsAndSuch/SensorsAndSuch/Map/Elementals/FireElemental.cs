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
    public class FireElemental: BaseElemental
    {

        internal static Color BaseColor = Color.Red;
        public FireElemental(int X, int Y)
            : base(X: X, Y: Y, col: BaseColor)
        {
            leftChance = 30;
            forwardChance = 10;
            DeleteWallThreshold = -5;
            CreateWallThreshold = 20;
            Likes.Add(() => ForestElemental.BaseColor);
        }
    }
}
