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
    public class SecondaryElemental: BaseElemental
    {
        //private Color BaseColor = Color.ForestGreen;
        BaseElemental attatched;
        public SecondaryElemental(int X, int Y, Color col, BaseElemental attatched)
            : base(X: X, Y: Y, col: col)
        {
            //radialreset = 0;
            //radialDist = 50;
            this.attatched = attatched;
        }

        public override void takeTurn(List<BaseTile>[,] Grid, bool canModWalls = true)
        {
            CentralX = attatched.X;
            CentralY = attatched.Y;
            if (CentralX - X == 0 && CentralY - Y == 0)
                dir = new Vector2(CentralX - X + 1, CentralY - Y);
            else
                dir = new Vector2(CentralX - X, CentralY - Y);
            dir = dir.Align();
            if (Globals.map.isInBounds(X + (int)dir.X, Y + (int)dir.Y, Offset: 1))
            {
                X += (int)dir.X;
                Y += (int)dir.Y;
            }
            base.takeTurn(Grid);
        }

    }
}
