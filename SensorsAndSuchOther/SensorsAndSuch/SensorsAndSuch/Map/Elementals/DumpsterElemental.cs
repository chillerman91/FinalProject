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
    public class DumpsterElemental : BaseElemental
    {
        public static Color BaseColor = Color.Purple;
        protected BaseElemental attatched;
        public DumpsterElemental(int X, int Y, BaseElemental attatched)
            : base(X: X, Y: Y, col: BaseColor)
        {
            forwardChance = 150;
            rightChance = 0;
            speed *= 4;
            DeleteWallThreshold = 20;
            CreateWallThreshold = -5;
            this.attatched = attatched;
        }

        public bool isFree(List<BaseTile>[,] Grid)
        {
            return collided.Count == 0 && Globals.map.isInBounds(new Vector2(X, Y)) && Grid[X, Y].Count == 1;
        }

        internal void createBlock(List<BaseTile>[,] Grid)
        {
            Grid[X, Y].Add(new Wall(X, Y));
            if (Grid[X, Y].Count != 2)
                throw new Exception();
            if (Globals.GamplayScreen.currentState == Screens.Gameplay.ScreenState.Ghost)
            {
                foreach (BaseTile tile in (Grid[X, Y]))
                {
                    tile.color.A = 255;
                }
            }
        }

        public override void takeTurn(List<BaseTile>[,] Grid, bool toldCanModWalls = true)
        {
            //pick a direction
            //if (age > AvgLife) return;  
            CheckCollisions();
            if (!NeedToWait)
            {
                Vector2 GridPos = Globals.map.GridFromPhysics(shape.Position);
                X = (int)GridPos.X;
                Y = (int)GridPos.Y;
                setPreferedDir(Grid);
                shape.ApplyForce(dir * speed, shape.Position);
                Vector2 towardAttatched = attatched.position - shape.Position;
                towardAttatched.Normalize();
                if (!towardAttatched.HasNan())
                    shape.ApplyForce(towardAttatched * speed / 4, shape.Position);

            }
            //Effect it or notdir

            if (Globals.map.isInBounds(X + (int)dir.X, Y + (int)dir.Y, Offset: 1))
            {
                //if (collided.Count == 0)
                    NeedToWait = isFree(Grid);
            }
            else
            {
                shape.LinearVelocity = new Vector2(0, 0);
                Vector2 push = -1 * new Vector2(((float)X - Globals.map.MapWidth / 2) / Globals.map.MapWidth, ((float)Y - Globals.map.MapHeight / 2) / Globals.map.MapHeight) * 5;
                shape.ApplyForce(push, shape.Position);
            }
            age++;
        }
    }
}
