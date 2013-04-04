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
        protected int rotRate = 90;// out of 100
        protected int leftChance = 8;
        protected int rightChance = 8;
        protected int age = 0;
        protected Vector2 dir;
        //protected int radialDist = 100;
        //protected int radialreset = 5;
        protected Color BaseColor;
        protected List<Func<Color>> Dislikes;
        protected List<Func<Color>> Likes;


        protected int DeleteWallThreshold = 2;
        protected int CreateWallThreshold = -2;
        protected bool CanMod = false;
        private static int CrawlerCount = 0;
        //private Color BaseColor = Color.ForestGreen;
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
            if (Globals.rand.Next(100) < 50)
                return new ForestCrawler(X, Y);
            if (Globals.rand.Next(100) < 50)
                return new DirtCrawler(X, Y);

            if (Globals.rand.Next(100) < 25)
                return new ShrubCrawler(X, Y);

            if (Globals.rand.Next(100) < 0)
                return new FireCrawler(X, Y);
            return new WaterCrawler(X, Y);
        }
        public BaseCrawler(int X, int Y, Color col)
        {
            Dislikes = new List<Func<Color>>();
            Likes = new List<Func<Color>>();
            CentralX = X;
            CentralY = Y;
            this.X = X;
            this.Y = Y;
            dir = BaseTile.GetRandDir();
            BaseColor = col;
        }

        protected void SetPreferedDir(List<BaseTile>[,] Grid)
        {
            int randNumb = Globals.rand.Next(rotRate + leftChance + rightChance);
            randNumb -= rotRate;
            if (randNumb > 0 && randNumb <= leftChance)
                dir = dir.Flip();
            else if (randNumb > 0 && randNumb >= leftChance)
                dir = dir.Flip() * -1;
            CanMod = true;
            if (!Globals.map.isInBounds(X + (int)dir.X, Y + +(int)dir.Y))
                dir = dir * -1;
            Color ColorAtPos = Grid[X + (int)dir.X, Y + +(int)dir.Y][Grid[X + (int)dir.X, Y + +(int)dir.Y].Count - 1].color;

            //if we like the color then leave move there.
            bool Happy = false;
            foreach (Func<Color> checker in Likes)
            {
                if (checker().Compare(ColorAtPos) < 30)
                {
                    Happy = true;
                    break;
                }
            }

            if (Happy)
            {
                return;
            }
            //General Fear 0 
            int locCompare = GetColor().Compare(ColorAtPos);
            if (locCompare > 30 && Globals.rand.Next(AvgLife/2) < age)
            {
                CanMod = false;
                if (locCompare > 60)
                    dir *= -1; 
                return;
            }
            
            bool Afraid = false;
            //Fear of specific colors
            foreach(Func<Color> checker in Dislikes)
            {
                if (checker().Compare(ColorAtPos) < 50) 
                {
                    Afraid = true;
                    break;
                }
            }
            if (Afraid) {
                dir *= -1; 
                return;
            }

        }

        public virtual void TakeTurn(List<BaseTile>[,] Grid)
        {
            if (age > AvgLife) return;
            SetPreferedDir(Grid);
            //pick a direction
            //Move there
            //Effect it or not

            if (Globals.map.isInBounds(X + (int) dir.X, Y + (int) dir.Y, Offset: 1))
            {
                X += (int) dir.X;
                Y += (int) dir.Y;
                AddDirt(Grid, X, Y);
            }
            EffectAdj(Grid);
            age++;
        }

        public virtual void EffectAdj(List<BaseTile>[,] Grid)
        {
            List<List<BaseTile>> Adj = Globals.map.GetAdjColumsToList(X, Y);
            foreach (List<BaseTile> column in Adj)
            {                    
                column[column.Count - 1].color = column[column.Count - 1].color.Combine(GetColor());
                //column[column.Count - 1].color.A = 255;
                if (column.Count == 2)
                {
                    column[column.Count - 1].color = GetColor();
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
        //TakeTurn Helpers
        protected void AddDirt(List<BaseTile>[,] Grid, int pos1, int pos2)
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

/*
         protected void SetPreferedDir(List<BaseTile>[,] Grid)
        {
            int compVal = Grid[X, Y][Grid[X, Y].Count - 1].color.Compare(GetColor());
            if ((CentralX - X) * (CentralX - X) + (CentralY - Y) * (CentralY - Y) > radialDist ||
                (compVal > 40 && Globals.rand.Next(AvgLife) < age))            
            {
                if(CentralX -X == 0 && CentralY - Y == 0)
                    dir = new Vector2(CentralX - X + 1, CentralY - Y);
                else
                    dir = new Vector2(CentralX -X, CentralY - Y);
                dir = dir.Align();
                return;
            }
            if (Globals.rand.Next(100) < radialreset)
            {
                CentralX = X;
                CentralY = Y;
            }
            int randNumb = Globals.rand.Next(rotRate + leftChance + rightChance);
            randNumb -= rotRate;
            if (randNumb > 0 && randNumb <= leftChance)
                dir = dir.Flip();
            else if (randNumb > 0 && randNumb >= leftChance)
                dir = dir.Flip() * -1;
        }
 
 
 
 
 
 */
