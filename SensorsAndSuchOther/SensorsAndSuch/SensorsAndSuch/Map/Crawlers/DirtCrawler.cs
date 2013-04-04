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
    public class DirtCrawler : BaseCrawler
    {
        //private Color BaseColor = Color.ForestGreen;
        public DirtCrawler(int X, int Y)
            : base(X: X, Y: Y, col: Color.LightSlateGray)
        {

            rightChance = 0;
            leftChance = 50;
            forwardChance =  20;
            DeleteWallThreshold = 14;
            CreateWallThreshold = -8;
        }
        public override void TakeTurn(List<BaseTile>[,] Grid, bool canModWalls = true)
        {
            forwardChance -= 5;
            if (forwardChance < 15)
                forwardChance += 50;
            base.TakeTurn(Grid, canModWalls);
        }
        public override Color GetColor()
        {
            return new Color((int) (34 + Math.Cos(X / RandomMap.RoomWidth * 1f) * 5), (int) (34 + Math.Cos(X / RandomMap.RoomWidth * 1f) * 15), (int) (34 + Math.Cos(X / RandomMap.RoomWidth * 1f) * 5)); 
        }
    }
}
