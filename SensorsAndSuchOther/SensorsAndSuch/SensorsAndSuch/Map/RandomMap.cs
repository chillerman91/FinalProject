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
    public class RandomMap
    {        
        #region Properties
        public static int RoomWidth = 12;
        public static int RoomHeight = 8;
        internal static int TicksInCreate = 500;

        private static int mapWidth = RoomWidth * 4;
        private static int mapHeight = RoomHeight * 3;

        public int PosMod = 9;
        public float globalScale = .25f;
        public Vector2 ToLocation = new Vector2(0, 0);

        List<BaseTile>[,] grid;

        public int state = 0;
        Vector2 freePos;
        BaseCrawler[] elementals;
        Text info = new Text(Globals.content.Load<SpriteFont>("Fonts/buttonFont"), displayText: "", displayPosition: new Vector2(0, 0), displayColor: Color.White,
            outlineColor: Color.Black, isTextOutlined: true, alignment: SensorsAndSuch.Texts.Text.Alignment.None, displayArea: Rectangle.Empty);

        public int MapWidth
        {
            get { return mapWidth; }
        }

        public int MapHeight
        {
            get { return mapHeight; }
        }

        #endregion

        public RandomMap()
        {
            BaseCrawler.AvgLife = 200;
            grid = new List<BaseTile>[MapWidth, MapHeight];
            for (int x = 0; x < MapWidth; x++)
                for (int y = 0; y < MapHeight; y++)
                    grid[x, y] = new List<BaseTile>();
        }

        public int CreateMap()
        {
            if (state <= 0)
            {
                for (int x = -1 * state; x < MapWidth; x++)
                {
                    for (int y = 0; y < MapHeight; y++)
                    {
                        if (grid[x, y].Count == 0)
                        {
                            grid[x, y].Add(new Dirt(new Vector2(x, y)));
                            grid[x, y].Add(new Wall(new Vector2(x, y)));
                        }
                    }
                }
                elementals = new BaseCrawler[20];

                state--;
                if (-1 * state >= MapWidth)
                    state = 1;
                return state;

            }
            else if (state == TicksInCreate)
            {
                ClearBorder(100);
                for (int x = 0; x < MapWidth; x++)
                {
                    for (int y = 0; y < MapHeight; y++)
                    {
                        if (grid[x, y].Count == 0)
                        {
                            grid[x, y].Add(new Dirt(new Vector2(x, y)));
                            grid[x, y].Add(new Wall(new Vector2(x, y)));
                        }
                    }
                }
                return 4;
            }
            else if (state % 50 == 5)
            {
                elementals[state / 50] = BaseCrawler.GetRandCrawler(grid);
            }
            for (int i = 0; i < elementals.Length && elementals[i] != null; i++)
                elementals[i].TakeTurn(grid);
            state++;
            return 0;
        }

        public bool isInBounds(int i, int j, int Offset = 0)
        {
            if (i - Offset < 0 || i + Offset >= MapWidth) return false;
            if (j - Offset < 0 || j + Offset >= MapHeight) return false;
            return true;
        }

        internal bool isInBounds(Vector2 GridPos)
        {
            return isInBounds((int)GridPos.X, (int)GridPos.Y);
        }

        public bool isFree(int i, int j)
        {
            if (i < 0 || i >= MapWidth) return false;
            if (j < 0 || j >= MapHeight) return false;
            return grid[i, j].Count == 1 || grid[i, j].Count == 0;
        }

        public bool isFree(Vector2 gridPos)
        {
            return isFree((int) gridPos.X, (int) gridPos.Y);
        }

        #region Modify Map walls
        public void RemoveRandPoint()
        {
            int Xt = Globals.rand.Next(MapWidth), Yt = Globals.rand.Next(MapHeight);
            if (grid[Xt, Yt].Count == 0) {  return; }// RemoveRandPoint();
            grid[Xt, Yt].Clear();
        }
        public void AddRandPoint()
        {
            int Xt = Globals.rand.Next(MapWidth), Yt = Globals.rand.Next(MapHeight);
            if (grid[Xt, Yt].Count != 0) {  return; } //AddRandPoint();
            grid[Xt,Yt].Add( new Dirt( new Vector2( Xt, Yt)));

        }
        public void AddRoomEdges(int xbound, int SizeRange, int prob)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                if (Globals.rand.Next(100) <= prob)
                    grid[0, j].Add(new Dirt(new Vector2(0, j)));
                if (Globals.rand.Next(100) <= prob)
                    grid[MapWidth - 1, j].Add(new Dirt(new Vector2(MapWidth - 1, j)));
            }
            for (int i = 0; i < MapWidth; i++)
            {
                if (Globals.rand.Next(100) <= prob)
                    grid[i, 0].Add(new Dirt(new Vector2(i, 0)));
                if (Globals.rand.Next(100) <= prob)
                    grid[i, MapHeight - 1].Add(new Dirt(new Vector2(i, MapHeight - 1)));
            }
        }
        public void AddSquare(int xbound, int SizeRange)
        {
            int size = Globals.rand.Next(SizeRange) + 2;
            int Xt = Globals.rand.Next(5) + xbound, Yt = Globals.rand.Next(MapHeight);
            for (int i = 0; i < size && i + Xt < MapWidth; i++)
            {
                for (int j = 0; j < size && j + Yt < MapHeight; j++)
                {
                    if (grid[i + Xt, j + Yt].Count == 0)
                        grid[i + Xt, j + Yt].Add( new Dirt(new Vector2(i + Xt, j + Yt)));
                }
            }
        }
        public void AddBorder(int prob)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                if (Globals.rand.Next(100) <= prob)
                    grid[0, j].Add( new Dirt(new Vector2(0, j)));
                if (Globals.rand.Next(100) <= prob)
                    grid[MapWidth - 1, j].Add( new Dirt(new Vector2(MapWidth - 1, j)));
            }
            for (int i = 0; i < MapWidth; i++)
            {
                if (Globals.rand.Next(100) <= prob)
                    grid[i, 0].Add( new Dirt(new Vector2(i, 0)));
                if (Globals.rand.Next(100) <= prob)
                    grid[i, MapHeight - 1].Add( new Dirt(new Vector2(i, MapHeight - 1)));
            }
        }
        public void ClearBorder(int prob)
        {
            for (int j = 0; j < MapHeight; j++)
            {
                if (Globals.rand.Next(100) <= prob)
                    grid[0, j].Clear();
                if (Globals.rand.Next(100) <= prob)
                    grid[MapWidth - 1, j].Clear();
            }
            for (int i = 0; i < MapWidth; i++)
            {
                if (Globals.rand.Next(100) <= prob)
                    grid[i, 0].Clear();
                if (Globals.rand.Next(100) <= prob)
                    grid[i, MapHeight - 1].Clear();
            }
        }
        #endregion

        #region Get Tiles or columns

        public Vector2 GetRandomFreePos()
        {
            int i = Globals.rand.Next(MapWidth);
            int j = Globals.rand.Next(MapHeight);
            if (grid[i, j].Count == 2) return GetRandomFreePos();
            return new Vector2(i, j);
        }

        public Vector2 GetRandomFreePos(Func<List<BaseTile>, bool> GoodTile)
        {
            int i = Globals.rand.Next(MapWidth);
            int j = Globals.rand.Next(MapHeight);
            if (!GoodTile(grid[i, j])) return GetRandomFreePos(GoodTile);
            return new Vector2(i, j);
        }

        private BaseTile GetTileBase(int i, int j)
        {
            if (i < 0 || i >= MapWidth) return null;
            if (j < 0 && j >= MapHeight) return null;
            return grid[i, j][0];
        }

        internal List<BaseTile> GetTileColumn(Vector2 vec)
        {
            return GetTileColumn((int) vec.X, (int) vec.Y);
        }

        internal List<BaseTile> GetTileColumn(int i, int j)
        {
            if (isInBounds(i, j))
                return grid[i, j];
            return null;
        }        
        
        public int GetNumbAtHeight(int height)
        {
            int amount = 0;
            for (int X = 0; X < MapWidth; X++)
            {
                for (int Y = 0; Y < MapWidth; Y++)
                {
                    if (GetTileColumn(X, Y) != null && GetTileColumn(X, Y).Count == height)
                        amount++;
                }
            }
            return amount;
        }

        public List<List<BaseTile>> GetAdjColumsToList(int i, int j, bool diagnols = false) 
        {
            List<List<BaseTile>> adj = new List<List<BaseTile>>();
            if (GetTileColumn(i - 1, j) != null)
                adj.Add(GetTileColumn(i - 1, j));
            if (GetTileColumn(i + 1, j) != null)
                adj.Add(GetTileColumn(i + 1, j));
            if (GetTileColumn(i, j - 1) != null)
                adj.Add(GetTileColumn(i, j - 1));
            if (GetTileColumn(i, j + 1) != null)
                adj.Add(GetTileColumn(i, j + 1));
            if (diagnols)
            {
                if (GetTileColumn(i - 1, j - 1) != null)
                    adj.Add(GetTileColumn(i - 1, j - 1));
                if (GetTileColumn(i + 1, j + 1) != null)
                    adj.Add(GetTileColumn(i + 1, j - 1));
                if (GetTileColumn(i + 1, j - 1) != null)
                    adj.Add(GetTileColumn(i, j - 1));
                if (GetTileColumn(i - 1, j + 1) != null)
                    adj.Add(GetTileColumn(i - 1, j + 1));
            
            }

            return adj;
        }

        public List<BaseTile>[,] GetAdjColumsToArray(int X, int Y, int offset = 1)
        {
            List<BaseTile>[,] adj = new List<BaseTile>[2 * offset + 1, 2 * offset + 1];
            for(int i = -offset; i <= offset; i++)
            {
                for (int j = -offset; j <= offset; j++)
                {
                    if (isInBounds(X + i, Y + j))
                        adj[i + offset, j + offset] = grid[X + i, Y + j];
                }
            }
            return adj;
        }
        
        public List<Vector2> GetAdjGridPos(int i, int j)
        {
            List<Vector2> ret = new List<Vector2>();
            if (isInBounds(i - 1, j))
                ret.Add(new Vector2(i - 1, j));
            if (isInBounds(i + 1, j))
                ret.Add(new Vector2(i + 1, j));
            if (isInBounds(i, j - 1))
                ret.Add(new Vector2(i, j - 1));
            if (isInBounds(i, j + 1))
                ret.Add(new Vector2(i, j + 1));
            return ret;
        }

        public Vector2? FindAtHeightFree(int i, int j, int height, int radius)
        {
            //if radius <= 0 then stop
            if (radius <= 0 || !isInBounds(i, j) || !grid[i, j][0].hit) return null;
            if (grid[i, j].Count != height) return null;
            return new Vector2(i, j);
            grid[i, j][0].hit = true;

            Vector2? temp;
            temp = FindAtHeightFree(i - 1, j, height, radius - 1);
            if (temp != null) return temp;

            temp = FindAtHeightFree(i + 1, j, height, radius - 1);
            if (temp != null) return temp;

            temp = FindAtHeightFree(i, j - 1, height, radius - 1);
            if (temp != null) return temp;

            return FindAtHeightFree(i, j + 1, height, radius - 1);

        }
        #endregion

        public void Draw(SpriteBatch batch) {
            //return;
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    
                   if ( grid[i, j].Count != 0) {
                       foreach (BaseTile tile in grid[i, j])
                       {
                           tile.Draw(batch);
                       }
                   }
                }
            }
            info.Draw(batch);
        }

        #region Converisons
        //Farseer to Screen
        public Vector2 ScreenFromPhysics(Vector2 farseerPos)
        {
            Vector2 temp = farseerPos - ToLocation;
            return (temp * 100f + new Vector2(BaseTile.TileWidth * 50f, BaseTile.TileHeight * 50f)) * globalScale;
        }
        //Grid to screen
        public Vector2 ScreenFromGrid(Vector2 gridPos)
        {
            return ScreenFromPhysics(new Vector2(gridPos.X * BaseTile.TileWidth, gridPos.Y * BaseTile.TileHeight)) - new Vector2(BaseTile.TileWidth * 50f, BaseTile.TileHeight * 50f) * globalScale;
        }

        public Vector2 GridFromPhysics(Vector2 farseerPos)
        {
            return new Vector2(farseerPos.X / BaseTile.TileWidth, farseerPos.Y / BaseTile.TileHeight);
        }

        public Vector2 PhysicsFromGrid(Vector2 gridPos)
        {
            return new Vector2(gridPos.X * BaseTile.TileWidth, gridPos.Y * BaseTile.TileHeight);
        }
        //Convert from the Screen position to a gridPos
        public Vector2 GridFromScreen(int X, int Y)
        {
            Vector2 temp = GridFromPhysics(ToLocation);
            temp =  new Vector2(X / (BaseTile.TileWidth * 100f), Y / (BaseTile.TileHeight * 100f)) / globalScale + temp;

            return new Vector2((int)temp.X, (int)temp.Y);
        }

        #endregion

        internal void Update()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {

                    if (grid[i, j].Count != 0)
                    {
                        foreach (BaseTile tile in grid[i, j])
                        {
                            tile.Update();
                        }
                    }
                }
            }
        }

        internal void Update(Inputs.GameInput input, int tick)
        {
            int i, j;
            for (i = 0; i < MapWidth; i++)
            {
                for (j = 0; j < MapHeight; j++)
                {
                    Vector2 tran = Globals.map.ScreenFromGrid(new Vector2(i, j));
                    if (input.CheckMouseOver(tran, (int)(BaseTile.TileWidth * 100 * globalScale), (int)(BaseTile.TileHeight * 100 * globalScale)))
                    {
                        String Text = "X:" + i + " Y:" + j + " Count:" + grid[i, j].Count + " ";
                        if(grid[i, j].Count != 0)  
                            Text+= grid[i, j][0].hit;
                        info.ChangeText(Text);
                        info.Position = new Vector2(tran.X + BaseTile.TileWidth, tran.Y + BaseTile.TileHeight);

                        i = MapWidth;
                        j = MapHeight;
                    }
                }
            }
            if (Globals.GamesStart && tick % 50 == 0)
            {
                for (i = 0; i < elementals.Length && elementals[i] != null; i++)
                {
                    elementals[i].age = 0;
                    elementals[i].TakeTurn(grid, false);

                }
                i = Globals.rand.Next(MapWidth);
                j = Globals.rand.Next(MapHeight);
                if (grid[i, j].Count > 0)
                    grid[i, j][grid[i, j].Count - 1].Update();
            }
        }

        internal void CreateBlockFromScreen(int X, int Y)
        {
            Vector2 CreatePos = GridFromScreen(X, Y);
            X = (int) CreatePos.X; 
            Y = (int) CreatePos.Y;
            if (isInBounds(X, Y))
            {
                if (grid[X, Y].Count == 1)
                {
                    //grid[X, Y].Add(new Dirt(new Vector2(X, Y)));
                    grid[X, Y].Add(new Wall(new Vector2(X, Y)));
                }
                else
                {
                    //foreach (BaseTile tile in grid[X, Y])
                    //{
                    grid[X, Y][grid[X, Y].Count - 1].Delete();
                    //}
                    grid[X, Y].RemoveAt(grid[X, Y].Count - 1);
                }
            }
        }

        #region Old Stuff
        /*
        #region cover All Tiles
        
        public void SetTilesToNotHit()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (grid[i, j].Count != 0) grid[i, j][0].hit = false;
                }
            }
        }

        //This function sets the hit var to true for every tile attatched to the starting tile.
        public void HitAttatchedTiles(int i, int j)
        {
            if (isInBounds(i, j) && grid[i, j].Count != 0)
            { 
                if (grid[i, j][0].hit) return;
                grid[i, j][0].hit = true;
                grid[i, j][0].adjColor = Color.Goldenrod;
                HitAttatchedTiles(i - 1, j);
                HitAttatchedTiles(i + 1, j);
                HitAttatchedTiles(i, j - 1);
                HitAttatchedTiles(i, j + 1);
            }
        }
        int i = 0; int j = 0;
        public void AddTheNotHit()
        {

            //Go throuhg all tiles
            for (i = 0; i < MapWidth; i++)
            {
                for (j = 0; j < MapHeight; j++)
                {
                    //Only run on tiles that have not been hit
                    if (grid[i, j].Count != 0 && !grid[i, j][0].hit)
                    {
                        //For each piece of dirt that has not been hit, add dirt until it hits something that has been hit.
                        Vector2 newDir = BaseTile.GetRandDir();
                        int Tx = i, Ty = j;
                        while (!isInBounds(Tx, Ty) || !grid[Tx, Ty][0].hit)
                        {
                            if (Globals.rand.Next((int)Math.Abs((Math.Cos(i / 2f) * 10))) >= Globals.rand.Next((int)Math.Abs((Math.Cos(i / 40f) * 10)))) newDir = BaseTile.GetRandDir();
                            if (isInBounds(Tx + (int)newDir.X, Ty + (int)newDir.Y, Offset: 1))
                            {
                                Tx += (int)newDir.X;
                                Ty += (int)newDir.Y;
                            }
                            else 
                            { 
                                newDir = BaseTile.GetRandDir();
                            }
                            if (isInBounds(Tx, Ty) && grid[Tx, Ty].Count == 0)
                            {
                                grid[Tx, Ty].Add( new Dirt(new Vector2(Tx, Ty)));
                            }
                        }
                        HitAttatchedTiles(i, j);
                        return;
                    }
                }
            }
        }
        
        private bool AndAdjHit(int i, int j) {
            BaseTile tile;
            tile = GetTileBase(i + 1, j);
            if (tile != null && tile.hit) return true;

            tile = GetTileBase(i - 1, j);
            if (tile != null && tile.hit) return true;

            tile = GetTileBase(i, j + 1);
            if (tile != null && tile.hit) return true;

            tile = GetTileBase(i, j - 1);
            if (tile != null && tile.hit) return true;

            return false;
        }


        public Vector2? FindAtHeightFree(int i, int j, int height, int radius)
        {
            //if radius <= 0 then stop
            if (radius <= 0 || !isInBounds(i, j) || !grid[i, j][0].hit) return null;
            if (grid[i, j].Count != height) return null;
            return new Vector2(i, j);
            grid[i, j][0].hit = true;
            
            Vector2? temp;
            temp = FindAtHeightFree(i - 1, j, height, radius -1);
            if (temp != null) return temp;

            temp = FindAtHeightFree(i + 1, j, height, radius -1);
            if (temp != null) return temp;

            temp = FindAtHeightFree(i, j - 1, height, radius -1);
            if (temp != null) return temp;

            return FindAtHeightFree(i, j + 1, height, radius - 1);

        }
        #endregion

        public List<Vector2> GetShortestPathFromType(int X, int Y, Func<BaseMonster, bool> GoodMon, int MaxDist, Func<List<BaseTile>, bool> traversable)
        {
            BaseTile CurrTile = grid[X, Y][0];
            float tempDist = int.MaxValue;
            int X2 = 10, Y2 = 10;
            List<Vector2> retDirections = new List<Vector2>();
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (grid[i, j].Count != 0) grid[i, j][0].dist = int.MaxValue;
                    BaseMonster mon = null;

                    if (mon != null && GoodMon(mon))
                    {
                        if ((mon.GridPos+ new Vector2(X, Y)).Length() < tempDist)
                        {
                            tempDist = (mon.GridPos + new Vector2(X, Y)).Length();
                            X2 = (int)mon.GridPos.X; Y2 = (int)mon.GridPos.Y;
                        }
                    }
                }
            }
            CurrTile.dist = 0;

            DijkstrasAlgHelper(X, Y, 1, MaxDist, traversable);

            if (grid[X2, Y2][0].dist == int.MaxValue) { return null; }
            while (X2 != X || Y2 != Y)
            {
                BaseTile tile = grid[X2, Y2][0];
                Vector2 dir = (-1 * tile.dirToShortest + new Vector2(X2, Y2));
                retDirections.Add(dir);
                X2 = (int)tile.dirToShortest.X;
                Y2 = (int)tile.dirToShortest.Y;
            }

            return retDirections;
        }
        

        private void DijkstrasAlgHelper(int X, int Y, int currDist, int MaxDist, Func<List<BaseTile>, bool> traversable)
        {
            if (currDist >= MaxDist) return;
            BaseTile CurrTile = grid[X, Y][0];
            List<List<BaseTile>> columns = GetAdjColumsToList(X, Y);
            for (int i = 0; i < columns.Count; i++)
            {
                if (traversable(columns[i]))
                {
                    if (currDist < columns[i][0].dist)
                    {
                        columns[i][0].dist = currDist;
                        columns[i][0].dirToShortest = CurrTile.GridPos;
                        DijkstrasAlgHelper((int)columns[i][0].GridPos.X, (int)columns[i][0].GridPos.Y, currDist + 1, MaxDist, traversable);
                    }
                }
            }
        }
        private void ScreamHelper(BaseMonster mon, int X, int Y, int currDist, int MaxDist, Func<List<BaseTile>, bool> traversable)
        {
            if (currDist >= MaxDist) return;
            BaseTile CurrTile = grid[X, Y][0];
            List<List<BaseTile>> columns = GetAdjColumsToList(X, Y);
            for (int i = 0; i < columns.Count; i++)
            {
                if (traversable(columns[i]))
                {
                    if (currDist < columns[i][0].dist)
                    {
                        columns[i][0].dist = currDist;
                        columns[i][0].dirToShortest = CurrTile.GridPos;
                        DijkstrasAlgHelper((int)columns[i][0].GridPos.X, (int)columns[i][0].GridPos.Y, currDist + 1, MaxDist, traversable);
                    }
                }
            }
        }

        //DijkstrasAlg
        public bool Scream(int X, int Y, int MaxDist, BaseMonster mon)
        {
            BaseTile CurrTile = grid[X, Y][0];
            Func<List<BaseTile>, bool> traversable = tileCol => tileCol.Count == 1;
            List<Vector2> retDirections = new List<Vector2>();
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (grid[i, j].Count != 0) grid[i, j][0].dist = int.MaxValue;
                }
            }
            CurrTile.dist = 0;

            DijkstrasAlgHelper(X, Y, 1, MaxDist, traversable);
            BaseMonster Listener = null;
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    //Listener = Globals.Mobs.GetMobAt(i, j);
                    if (grid[i, j][0].dist != int.MaxValue && Listener != null) { }// && Listener.Listen(mon));
                }
            }
            return true;
        }

        public List<Vector2> GetPath(int X, int Y, int X2, int Y2)
        {

            List<Vector2> retDirections = new List<Vector2>();
            while (X2 != X || Y2 != Y)
            {
                BaseTile tile = grid[X2, Y2][0];
                Vector2 dir = (-1 * tile.dirToShortest + new Vector2(X2, Y2));
                tile.adjColor = Color.Gray;
                retDirections.Add(dir.Times(-1));
                X2 = (int)tile.dirToShortest.X;
                Y2 = (int)tile.dirToShortest.Y;
            }
            return retDirections.Flip();
        }
        //DijkstrasAlg
        public List<Vector2> GetShortestPath(int X, int Y, int X2, int Y2, int MaxDist, Func<List<BaseTile>, bool> traversable)
        {
            BaseTile CurrTile = grid[X, Y][0];
            List<Vector2> retDirections = new List<Vector2>();
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (grid[i, j].Count != 0) grid[i, j][0].dist = int.MaxValue;
                }
            }
            CurrTile.dist = 0;

            DijkstrasAlgHelper(X, Y, 1, MaxDist, traversable);

            if (grid[X2, Y2][0].dist == int.MaxValue) { return null; }
            while (X2 != X || Y2 != Y)
            {
                BaseTile tile = grid[X2, Y2][0];
                Vector2 dir = (-1 * tile.dirToShortest + new Vector2(X2, Y2));
                retDirections.Add(dir);
                X2 = (int)tile.dirToShortest.X;
                Y2 = (int)tile.dirToShortest.Y;
            }

            return retDirections;
        }
        */
        #endregion
    }
}
