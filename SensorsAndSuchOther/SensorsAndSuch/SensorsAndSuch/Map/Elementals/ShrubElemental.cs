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
    public class ShrubElemental : BaseElemental
    {
        //private Color BaseColor = Color.ForestGreen;
        public ShrubElemental(int X, int Y)//, BaseElemental attatched)
            : base(X: X, Y: Y, col: Color.LimeGreen)//, attatched: attatched)
        {
            forwardChance = 100;

            rightChance = 0;
        }
    }
}
