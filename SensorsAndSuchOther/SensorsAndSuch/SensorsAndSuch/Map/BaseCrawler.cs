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
    public class BaseCrawler
    {
        public static int AvgLife;
        protected int CentralX;
        protected int CentralY;

        public int X;
        public int Y;
        private Byte state;
        protected int forwardChance = 80;
        protected int leftChance = 10;
        protected int rightChance = 10;
        protected int backChance = 0;
        internal int age = 0;
        protected Vector2 dir;
        protected Color BaseColor;
        protected List<Func<Color>> Dislikes;
        protected List<Func<Color>> Likes;

        protected int DeleteWallThreshold = 2;
        protected int CreateWallThreshold = -2;
        protected bool CanMod = false;
        private static int CrawlerCount = 0;
        List<Vector2> previousPlaces = new List<Vector2>();

        public static BaseCrawler GetRandCrawler(List<BaseTile>[,] Grid)
        {
            CrawlerCount++;

            Vector2 pos = Globals.map.GetRandomFreePos(column => column.Count == 2);
            int count = 0;
            while (GetHeightDiffrence(Grid, (int)pos.X, (int)pos.Y, 4) < 50 && count < 10)
            {
                pos = Globals.map.GetRandomFreePos(column => column.Count == 2);
                count++;
            }

            int X = (int) pos.X;
            int Y = (int) pos.Y;

            if (X == 0) X++;
            if (X == Globals.map.MapWidth) X--;
            if (Y == 0) Y++;
            if (Y == Globals.map.MapHeight) Y--;

            if (Globals.rand.Next(100) < 25)
                return new ForestCrawler(X, Y);

            if (Globals.rand.Next(100) < 25)
                return new ShrubCrawler(X, Y);

            if (Globals.rand.Next(100) < 25)
                return new FireCrawler(X, Y);

            //if (Globals.rand.Next(100) < 50)
                return new DirtCrawler(X, Y);

            return new WaterCrawler(X, Y);
        }

        public BaseCrawler(int X, int Y, Color col)
        {
            Dislikes = new List<Func<Color>>();
            Likes = new List<Func<Color>>();
            Likes.Add(() => Wall.GetColor());
            CentralX = X;
            CentralY = Y;
            this.X = X;
            this.Y = Y;
            dir = BaseTile.GetRandDir();
            BaseColor = col;
        }

        protected void SetPreferedDir(List<BaseTile>[,] Grid)
        {
            //Add options
            List<int> weights = new List<int>();
            List<Vector2> possibleDir = Globals.map.GetAdjGridPos(X, Y);
            for (int i = 0; i < possibleDir.Count; i++)
            {
                possibleDir[i] = possibleDir[i] - new Vector2(X, Y);
                if (possibleDir[i].VEquals(dir))
                    weights.Add(forwardChance);
                else if (possibleDir[i].VEquals(dir * -1))
                    weights.Add(backChance);
                else if (possibleDir[i].VEquals(dir.Flip()))
                    weights.Add(dir.X == 0 ? leftChance : rightChance);
                else if (possibleDir[i].VEquals(dir.Flip() * -1))
                    weights.Add(dir.Y == 0 ? leftChance : rightChance);
                else
                    throw new Exception("Crawler Bad dir");
            }

            CanMod = true;
            #region Adj Weights based on preferences
            for (int i = 0; i < possibleDir.Count; i++)
            {
                Color ColorAtPos = Grid[X + (int)possibleDir[i].X, Y + (int)possibleDir[i].Y][Grid[X + (int)possibleDir[i].X, Y + (int)possibleDir[i].Y].Count - 1].color;
                foreach (Func<Color> checker in Likes)
                {
                    int comp = checker().Compare(ColorAtPos);
                    if (comp < 70)
                    {
                        weights[i] += 100;
                    }
                }

                foreach (Func<Color> checker in Dislikes)
                {
                    int comp = checker().Compare(ColorAtPos);
                    if (comp < 70)
                    {
                        weights[i] = 0;
                    }
                }
            }
            #endregion

            #region Choose Direction
            int randNumb = 0, sum = 0;
            for (int i = 0; i < possibleDir.Count; i++)
            {
                randNumb += weights[i];
            }
            if (randNumb == 0) 
            {
                dir = possibleDir[Globals.rand.Next(possibleDir.Count - 1)];
                return;
            }
            randNumb = Globals.rand.Next(randNumb);
            for (int i = 0; i < possibleDir.Count; i++)
            {
                sum += weights[i];
                if (randNumb < sum)
                {
                    dir = possibleDir[i];
                    break;
                }
            }
            #endregion
        }

        public virtual void TakeTurn(List<BaseTile>[,] Grid, bool canModWalls = true)
        {
            //pick a direction
            //if (age > AvgLife) return;
            SetPreferedDir(Grid);
            //Effect it or not
            if (Globals.map.isInBounds(X + (int) dir.X, Y + (int) dir.Y, Offset: 1))
            {
                //Move there
                X += (int) dir.X;
                Y += (int) dir.Y;
                previousPlaces.Add(new Vector2(X, Y));
                if (previousPlaces.Count > 3)
                    previousPlaces.RemoveAt(0);
                if (canModWalls)
                    ModWalls(Grid, X, Y);
                EffectAdj(Grid, previousPlaces[0]);
            }
            age++;
        }

        public virtual void EffectAdj(List<BaseTile>[,] Grid, Vector2 pos)
        {
            int X = (int)pos.X, Y = (int)pos.Y;
            List<List<BaseTile>> Adj = Globals.map.GetAdjColumsToList(X, Y);
            foreach (List<BaseTile> column in Adj)
            {
                column[column.Count - 1].color = column[column.Count - 1].color.Combine(GetColor());
                //column[column.Count - 1].color.A = 255;
                if (column.Count == 2)
                {
                    column[column.Count - 1].color = column[column.Count - 1].color.Combine(GetColor());
                    //List<List<BaseTile>> Adj2 = Globals.map.GetAdjColumsToList((int)column[0].GridPos.X, (int) column[0].GridPos.Y);
                    /*
                     * foreach (List<BaseTile> column2 in Adj2)
                    {
                        if (column2.Count == 2)
                            column2[column2.Count - 1].color = column[column.Count - 1].color.Combine(GetColor()).Combine(GetColor());
                    }
                    */
                }
            }
        }
        
        protected int GetHeightDiffrence(List<BaseTile>[,] Grid)
        {
            return GetHeightDiffrence(Grid, X, Y, 2);
        }

        protected static int GetHeightDiffrence(List<BaseTile>[,] Grid, int X, int Y, int offset) 
        {
            List<BaseTile>[,] adj = Globals.map.GetAdjColumsToArray(X, Y, offset: offset);
            int ret = 0;
            for (int i = 0; i <= 4; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    if (Globals.map.isInBounds(X + i, Y + j) && (i != 1 || j != 1) && adj[i, j] != null)
                    {
                        ret += adj[i, j].Count;
                        //if ((i == 1 || j == 1) && (i != 1 || j != 1))
                        if ((i != 1 && j != 1))
                        {
                            ret += (adj[i, j].Count * 2) - 3;//(adj[i, j].Count - 1);
                        }
                    }
                }
            }
            return ret - 8;
        }

        protected void ModWalls(List<BaseTile>[,] Grid, int pos1, int pos2)
        {
            if (Globals.map.isInBounds(pos1, pos2, Offset: 1) && CanMod)
            {
                int highDiff = GetHeightDiffrence(Grid);
                if (Grid[pos1, pos2].Count == 2 && -1 * highDiff < DeleteWallThreshold)
                {
                    Grid[pos1, pos2][Grid[pos1, pos2].Count - 1].Delete();
                    Grid[pos1, pos2].RemoveAt(Grid[pos1, pos2].Count - 1);
                }
                else if (Grid[pos1, pos2].Count == 1 && highDiff < CreateWallThreshold)
                {
                    Grid[pos1, pos2].Add(new Wall(pos1, pos2));
                }
                Grid[pos1, pos2][Grid[pos1, pos2].Count - 1].color = GetColor().Combine(Grid[pos1, pos2][Grid[pos1, pos2].Count - 1].color);
                //Grid[pos1, pos2][Grid[pos1, pos2].Count - 1].color.A = 255;
            }
        }

        public virtual Color GetColor()
        {
            return new Color((int) (BaseColor.R + Math.Cos(X / RandomMap.RoomWidth * 1f) * 10), (int) (BaseColor.G + Math.Cos(X / RandomMap.RoomWidth * 1f) * 15), (int) (BaseColor.B + Math.Cos(X / RandomMap.RoomWidth * 1f) * 10)); 
        }

    }
}
