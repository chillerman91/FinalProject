using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Infinity_TD
{
    class Tile
    {
        public Texture2D texture;
        int tileX, tileY;

        public Rectangle GetSourceRectangle(int tileIndex)
        {
            tileY = ((int)(tileIndex / (texture.Width / 32)));
            tileX = ((int)(tileIndex % (texture.Width / 32)));
            return new Rectangle(tileX * 32, tileY * 32, 32, 32);
        }
    }
}
