using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SensorsAndSuch.Maps
{
    public class Dirt: BaseTile
    {
        public Dirt(Vector2 GridPos)
            : base(tex: "Tiles/dirt", GridPos: GridPos)
        {
        }

        public Dirt(int X, int Y)
            : base(tex: "Tiles/dirt", GridPos: new Vector2(X, Y))
        {
        }

        public Dirt(int X, int Y, Color color)
            : base(tex: "Tiles/dirt", GridPos: new Vector2(X, Y))
        {
            this.color = color;
        }
    }
}
