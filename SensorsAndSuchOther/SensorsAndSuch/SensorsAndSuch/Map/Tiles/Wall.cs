using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.SamplesFramework;
using SensorsAndSuch.Extensions;
using System;
namespace SensorsAndSuch.Maps
{
    public class Wall : BaseTile
    {
        Body rectangle;
        private FarseerPhysics.SamplesFramework.Sprite rectangleSprite;

        public Wall(int X, int Y)
            : this(new Vector2(X, Y)) { }
        float picSize = .75f/.64f;
        public Wall(Vector2 GridPos)
            : base("Tiles/Wall", GridPos)
        {
            //texture = Globals.content.Load<Texture2D>(tex);
            this.GridPos = GridPos;

            rectangle = BodyFactory.CreateCircle(Globals.World, radius: TileWidth/2, density: 5f);
                //BodyFactory.CreateRectangle(Globals.World, width: TileWidth, height: TileHeight, density: 5f);
            int image = Globals.rand.Next(10);
            string tex = "Tiles/tree";
            if(image < 3) tex = "Tiles/tree";
            else if(image < 6) tex = "Tiles/rock";
            else  tex = "Tiles/wall";
            rectangle.Position = new Vector2(GridPos.X * TileWidth, GridPos.Y * TileHeight);
            rectangleSprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.content.Load<Texture2D>(tex));
            rectangle.Position = new Vector2(GridPos.X * TileWidth, GridPos.Y * TileHeight);
            rectangle.Friction = 0.75f;
            rectangle.CollidesWith = Category.Cat1;
            color = GetColor();
            color.A = 0;
        }

        internal static Color GetColor() 
        {
            return Color.SaddleBrown;// new Color((int)(Globals.rand.Next(50) + 0), (int)(Globals.rand.Next(50) + 0), (int)(Globals.rand.Next(50) + 50));
        }

        private Color GetColor(Vector2 gridPos)
        {
            int X = (int) (gridPos.X) + Globals.rand.Next(3);
            int R = (int)(Globals.rand.Next(20) +  X/ 10 * 100+ 75) % 50; // +(int) gridPos.X%14 * 2
            int B = (int)(Globals.rand.Next((int)(gridPos.Y) % 10 * 4) + (int)(gridPos.Y) / 10 * 40 + 200) % 40;
            int G = (int)(Globals.rand.Next((int)(gridPos.Y) % 10 * 5) + (int)(gridPos.Y) / 10 * 40 + 100) % 40;
            return new Color(R, G, B);// G, B);
        }

        public override void Draw(SpriteBatch batch)
        {

            //color.A = (byte)Math.Max(0, color.A - 10);
            //Color use = color.Combine(BaseColor, .3f);
            //use.A = color.A;

            color.A = (byte)(color.A * gotoAlpha + desiredAlpha * (1 - gotoAlpha));
            if(color.A > 0)
                batch.Draw(rectangleSprite.Texture,
                       Globals.map.ScreenFromPhysics(rectangle.Position), null,
                       Globals.map.globalEffect(color), rectangle.Rotation, rectangleSprite.Origin, picSize * Globals.map.globalScale,
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
