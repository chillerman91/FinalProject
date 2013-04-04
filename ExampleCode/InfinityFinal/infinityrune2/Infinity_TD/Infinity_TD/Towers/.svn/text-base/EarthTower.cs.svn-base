using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Infinity_TD
{
    class EarthTower : Tower
    {
        public EarthTower(Game game, float damage, Vector2 position, float fireRate)
            : base(game, damage, position, fireRate, @"earthquake rune", @"fireball", new Effect())
        {
        }
        public override void FireToEnemy(Enemy enemy, Vector2 positionSource, Texture2D texture)
        {
            FireToEnemy(enemy, positionSource, texture, 0);
        }
    }
}
