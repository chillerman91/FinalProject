using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using SensorsAndSuch.Mobs;
using SensorsAndSuch;
using SensorsAndSuch.Extensions;
using System;

namespace SensorsAndSuch.Maps
{
    public abstract class BaseTile
    {
        protected enum MoveConflict
        {
            Monster,
            Wall,
            None
        }

        public static float TileWidth = .75f;
        public static float TileHeight = .75f;
        public static int DistBetween = 30;
        public Vector2 GridPos;
        protected Texture2D texture;
        public Color color = Color.White;
        public Color? adjColor = null;
        public SpriteFont font;

        internal byte desiredAlpha = 0;
        protected float gotoAlpha = .95f;
        public static Color BaseColor = Color.Gray;

        #region for diffrent algorythems
        public bool hit = false;
        public int dist;
        public Vector2 dirToShortest;
        #endregion

        protected Vector2 CurrentPos;

        public BaseTile(string tex, Vector2 GridPos)
        {
            font = Globals.content.Load<SpriteFont>("Fonts/debugFont");
            if (tex != null)
                texture = Globals.content.Load<Texture2D>(tex);
            this.GridPos = GridPos;
            CurrentPos = new Vector2(0, 0);
        }

        public static Vector2 GetRandDir()
        {
            Vector2 newPos;
            int i = Globals.rand.Next(4);
            if (i == 0)
                newPos = new Vector2(-1, 0);
            else if (i == 1)
                newPos = new Vector2(1,0);
            else if (i == 2)
                newPos = new Vector2(0,-1);
            else
                newPos = new Vector2(0, 1);

            return newPos;
        }

        public virtual void Draw(SpriteBatch batch)
        {
            //color.A = (byte)Math.Max(0, color.A - 10);

            color.A = (byte)(color.A * gotoAlpha + desiredAlpha * (1 - gotoAlpha));
            Color use = color.Combine(BaseColor, .3f);
            use.A = color.A;
            if (color.A != 0)
                batch.Draw(texture,
                    Globals.map.ScreenFromGrid(GridPos), null,
                    Globals.map.globalEffect(use), 0, new Vector2(0, 0), Globals.map.globalScale * TileWidth / .32f,
                   SpriteEffects.None, 0f);
        }

        internal void Dilute()
        {
            List<List<BaseTile>> Adj = Globals.map.GetAdjColumsToList((int) GridPos.X, (int) GridPos.Y);
            foreach (List<BaseTile> column in Adj)
            {
                column[column.Count - 1].color = column[column.Count - 1].color.Combine(color);
            }
        }

        internal void AntiDilute()
        {
            List<BaseTile> tile = Globals.map.GetTileColumn((int)GridPos.X, (int)GridPos.Y);
            List<List<BaseTile>> Adj = Globals.map.GetAdjColumsToList((int)GridPos.X, (int)GridPos.Y);
            Vector3 change = new Vector3(0,0,0);
            foreach (List<BaseTile> column in Adj)
            {
                if (column[column.Count - 1].color.Compare(tile[tile.Count - 1].color) > 400)
                {
                    change.X += (byte)(Clamp((column[column.Count - 1].color.R - tile[tile.Count - 1].color.R) / 10, 20));
                    change.Y += (byte)(Clamp((column[column.Count - 1].color.G - tile[tile.Count - 1].color.G) / 10, 20));
                    change.Z += (byte)(Clamp((column[column.Count - 1].color.B - tile[tile.Count - 1].color.B) / 10, 20));
                }
                else
                {
                    tile[tile.Count - 1].color.R += (byte)(Globals.rand.Next(4) - 2);
                    tile[tile.Count - 1].color.G += (byte)(Globals.rand.Next(4) - 2);
                    tile[tile.Count - 1].color.B += (byte)(Globals.rand.Next(4) - 2);
                }
            }
            float total = (change.X + change.Y + change.Z)/2;
            change.X = (Clamp((int) (change.X / total), 2));
            change.Y = (Clamp((int) (change.Y / total), 2));
            change.Z = (Clamp((int) (change.Z / total), 2));

            tile[tile.Count - 1].color.R = (byte) Clamp(tile[tile.Count - 1].color.R + (int) change.X, 0, 255);
            tile[tile.Count - 1].color.G = (byte) Clamp(tile[tile.Count - 1].color.G + (int) change.Y, 0, 255);
            tile[tile.Count - 1].color.B = (byte) Clamp(tile[tile.Count - 1].color.B + (int) change.Z, 0, 255);
        }

        internal int Clamp(int X, int range)
        {
            return Math.Max(-range, Math.Min(range, X));
        }

        internal int Clamp(int X, int bottom, int top)
        {
            return Math.Max(bottom, Math.Min(top, X));
        }

        internal virtual void Delete()
        {
            //if (texture != null)
                //texture.Dispose();
        }

        internal void Seen(int i)
        {
            desiredAlpha = (byte)Math.Min(255, i);
        }
    }
}
