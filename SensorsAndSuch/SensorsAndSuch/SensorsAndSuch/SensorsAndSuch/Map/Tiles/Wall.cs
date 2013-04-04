using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.SamplesFramework;

namespace SensorsAndSuch.Maps
{
    public class Wall : BaseTile
    {
        Body rectangle;
        private FarseerPhysics.SamplesFramework.Sprite rectangleSprite;

        public Wall(int X, int Y)
            : this(new Vector2(X, Y)) { }

        public Wall(Vector2 GridPos)
            : base("Tiles/Wall", GridPos)
        {
            //texture = Globals.content.Load<Texture2D>(tex);
            this.GridPos = GridPos;

            rectangle = BodyFactory.CreateRectangle(Globals.World, width: TileWidth, height: TileHeight, density: 5f);
            rectangleSprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(rectangle.FixtureList[0].Shape,
                                                                    MaterialType.Squares,
                                                                    Color.White, 1f));
            rectangle.Position = new Vector2(GridPos.X * TileWidth, GridPos.Y * TileHeight);
            rectangle.Friction = 0.75f;
            color = GetColor();
        }

        private Color GetColor() 
        {
            return new Color((int)(Globals.rand.Next(50) + 200), (int)(Globals.rand.Next(50) + 200), (int)(Globals.rand.Next(50) + 200));
        }

        private Color GetColor(Vector2 gridPos)
        {
            int X = (int) (gridPos.X) + Globals.rand.Next(3);
            int R = (int)(Globals.rand.Next(20) +  X/ 10 * 100+ 75) % 255; // +(int) gridPos.X%14 * 2
            int B = (int)(Globals.rand.Next((int)(gridPos.Y) % 10 * 4) + (int)(gridPos.Y) / 10 * 40 + 200) % 255;
            int G = (int)(Globals.rand.Next((int)(gridPos.Y) % 10 * 5) + (int)(gridPos.Y) / 10 * 40 + 100) % 255;
            return new Color(R, G, B);// G, B);
        }

        public override void Draw(SpriteBatch batch)
        {
            // CurrentPos = CurrentPos.Times(0) + Globals.map.TranslateToPos(GridPos); // .Times(.05);
            // batch.Draw(texture, new Rectangle((int)(CurrentPos).X, (int)(CurrentPos).Y, TileWidth, TileHeight), (Color)(adjColor == null ? color : adjColor));
            batch.Draw(rectangleSprite.Texture,
                   Globals.map.ScreenFromPhysics(rectangle.Position), null,
                   color, rectangle.Rotation, rectangleSprite.Origin, Globals.map.globalScale * 1f,
                   SpriteEffects.None, 0f);
        }

        internal override void Delete()
        {
            base.Delete();
            //rectangleSprite.Texture.Dispose();
            rectangle.Dispose();
        }
    }
}
