using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Infinity_TD
{
    class WaterTower : Tower
    {
        public WaterTower(Game game, float damage, Vector2 position, float fireRate)
            : base(game, damage, position, fireRate, @"glacier rune1", @"glacier", new Effect())
        {

        }

        public override void FireToEnemy(Enemy enemy, Vector2 positionSource, Texture2D texture)
        {
            FireToEnemy(enemy, positionSource, texture, 2);
        }
    }
}
