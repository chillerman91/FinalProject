using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SensorsAndSuch.Sprites
{
    class Sprite
    {
        public Vector2 Position;
        public delegate void CollisionDelegate();

        protected Texture2D texture;
        protected Color color = Color.White;

        public Sprite(ContentManager content, string assetName)
        {
            texture = content.Load<Texture2D>(assetName);
        }

        public virtual void Draw(SpriteBatch batch)
        {
            batch.Draw(texture, Position, color);
        }

        public bool IsCollidingWith(Sprite spriteToCheck)
        {
            return CollisionRectangle.Intersects
                       (spriteToCheck.CollisionRectangle);
        }

        public Rectangle CollisionRectangle
        {
            get
            {
                return new Rectangle((int)Position.X,
                                     (int)Position.Y,
                                     Width,
                                     Height);
            }
        }

        public int Height
        {
            get { return texture.Height; }
        }

        public int Width
        {
            get { return texture.Width; }
        }
    }
}
