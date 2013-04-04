using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using SensorsAndSuch.Mobs;
using SensorsAndSuch;
using SensorsAndSuch.Extensions;

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
            //color = Color.Gray;
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
            CurrentPos = CurrentPos.Times(1);// + Globals.map.TranslateToPos(GridPos).Times(.05); Globals.map.TranslateToPos(CurrentPos)
            //batch.Draw(texture, new Rectangle((int)(CurrentPos).X, (int)(CurrentPos).Y, (int)TileWidth, (int)TileHeight), (Color)(adjColor == null ? color : adjColor));

            batch.Draw(texture,
                Globals.map.ScreenFromGrid(GridPos), null,
                color.Combine(Color.Gray, .3f), 0, new Vector2(0, 0), Globals.map.globalScale * TileWidth / .32f,
               SpriteEffects.None, 0f);
        }

        internal void Update()
        {
            List<List<BaseTile>> Adj = Globals.map.GetAdjColumsToList((int) GridPos.X, (int) GridPos.Y);
            foreach (List<BaseTile> column in Adj)
            {
                column[column.Count - 1].color = column[column.Count - 1].color.Combine(color);
                //column[column.Count - 1].color.A = 255;
                if (column.Count == 2)
                {
                    //column[column.Count - 1].color = column[column.Count - 1].color.Combine(color);
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

        internal virtual void Delete()
        {
            //if (texture != null)
                //texture.Dispose();
        }
    }
}
