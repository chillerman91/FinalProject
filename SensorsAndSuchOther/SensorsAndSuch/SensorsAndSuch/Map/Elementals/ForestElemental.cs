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
    public class ForestElemental : BaseElemental
    {
        internal static Color BaseColor = Color.ForestGreen;
        public ForestElemental(int X, int Y)
            : base(X: X, Y: Y, col: BaseColor)
        {
            forwardChance = 250;// out of 100
            speed *= 4f;
            leftChance = 10;
            rightChance = 10;
            DeleteWallThreshold = 10;
            CreateWallThreshold = 20;
        }
    }
}
