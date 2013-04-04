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
    public class ForestRegion
    {
        private int X;
        private int Y;
        private Byte state;
        public ForestRegion(int X, int Y)
        {
            color = new Color(Globals.rand.Next(255), Globals.rand.Next(255), Globals.rand.Next(255));
            state = 0;
            this.X = X;
            this.Y = Y;
        }
        public bool done = false;
        #region Change Map
        public void FillRegion(List<BaseTile>[,] Grid) 
        { 
            //get starting points 
            if (state == 0)
                AddDirt(Grid, 0, 2);
            else if (state == 1)                
                AddDirt(Grid, 0, RandomMap.RoomHeight - 2);
            else if (state == 2)
                AddDirt(Grid, RandomMap.RoomWidth - 1, 2);
            else if (state == 3)
                AddDirt(Grid, RandomMap.RoomWidth - 1, RandomMap.RoomHeight - 2);
            else if (state == 4)
                SetTilesToNotHit(Grid);
            else if (state == 5)// && X == 0)
                HitAttatchedTiles(Grid, X, Y + 2);
            else if (state == 6 && !AddTheNotHit(Grid))
                return;
            state++;
            if (state > 6) done = true;
        }
        private void AddDirt(List<BaseTile>[,] Grid, int pos1, int pos2)
        {
            pos1 += X;
            pos2 += Y;
            if (Globals.map.isInBounds(pos1, pos2))
            {
                if (Grid[pos1, pos2].Count == 0)
                    Grid[pos1, pos2].Add(new Dirt(pos1, pos2, color));
                else
                    Grid[pos1, pos2][0].color = color;
            }
        }
        private Color color;
        public void GetColor(List<BaseTile>[,] Grid)
        {

        }
        #endregion

        #region AttatchTiles

        private void SetTilesToNotHit(List<BaseTile>[,] Grid)
        {
            for (i = X; i < X + RandomMap.RoomWidth; i++)
            {
                for (j = Y; j < Y + RandomMap.RoomHeight; j++)
                {
                    if (Grid[i, j].Count != 0) Grid[i, j][0].hit = false;
                }
            }
        }

        //This function sets the hit var to true for every tile attatched to the starting tile.
        private void HitAttatchedTiles(List<BaseTile>[,] Grid, int i, int j)
        {
            if (isInBounds(i, j) && Grid[i, j].Count != 0)
            {
                if (Grid[i, j][0].hit) return;
                Grid[i, j][0].hit = true;
                //Grid[i, j][0].adjColor = Color.Blue;
                HitAttatchedTiles(Grid, i - 1, j);
                HitAttatchedTiles(Grid, i + 1, j);
                HitAttatchedTiles(Grid, i, j - 1);
                HitAttatchedTiles(Grid, i, j + 1);
            }
        }
        int i = 0; int j = 0;
        private bool AddTheNotHit(List<BaseTile>[,] Grid)
        {
            //Go through all tiles
            for (i = X; i <= X + RandomMap.RoomWidth; i++)
            {
                for (j = Y; j <= Y + RandomMap.RoomHeight; j++)
                {
                    //Only run on tiles that have not been hit
                    if (Grid[i, j].Count != 0 && !Grid[i, j][0].hit)
                    {
                        //For each piece of dirt that has not been hit, add dirt until it hits something that has been hit.
                        Vector2 newDir = BaseTile.GetRandDir();
                        int Tx = i, Ty = j;
                        while (!isInBounds(Tx, Ty) || !Grid[Tx, Ty][0].hit)
                        {
                            //if (Globals.rand.Next((int)Math.Abs((Math.Cos(i / 2f) * 10))) >= Globals.rand.Next((int)Math.Abs((Math.Cos(i / 40f) * 15)))) newDir = BaseTile.GetRandDir();
                            if (Globals.rand.Next(10) < Globals.rand.Next(8)) newDir = BaseTile.GetRandDir();
                        
                            if (isInBounds(Tx + (int)newDir.X, Ty + (int)newDir.Y, Offset: 1))
                            {
                                Tx += (int)newDir.X;
                                Ty += (int)newDir.Y;
                            }
                            else 
                            { 
                                newDir = BaseTile.GetRandDir();
                            }
                            if (isInBounds(Tx, Ty) && Grid[Tx, Ty].Count == 0)
                            {
                                AddDirt(Grid, Tx- X, Ty -Y);
                            }
                        }
                        HitAttatchedTiles(Grid, i, j);
                        return false;
                    }
                }
            }
            return false; ;
        }
        #endregion

        private bool isInBounds(int i, int j, int Offset = 0) {
            if (i - X < 0 || i - X >= RandomMap.RoomWidth) return false;
            if (j - Y < 0 || j - Y >= RandomMap.RoomHeight) return false;
            return true;
        }
    }
}
