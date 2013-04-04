using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Infinity_TD.Tiles
{
    class Waypoint
    {
        public Vector2 position;
        public Rectangle area;
        public Texture2D teste;
        

        public enum Directions { UP, LEFT, RIGHT, DOWN };

        public List<Directions> DirectionList = new List<Directions>();

        public Directions getDirection()
        {
            int random = MapArrays.random.Next(0, DirectionList.Count());
            return DirectionList.ElementAt(random);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(teste, area, Color.White);
        }

    }
}
