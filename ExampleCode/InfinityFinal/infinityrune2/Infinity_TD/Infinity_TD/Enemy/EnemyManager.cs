using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Infinity_TD
{
    class EnemyManager
    {
        public List<Enemy> Enemies { get; set; }

        private int drops;
        public int Drops
        {
            get
            {
                int tempDrops = drops;
                drops = 0;
                return tempDrops;
            }

            private set
            {
                drops = value;
            }
        }

        public EnemyManager()
        {
            Enemies = new List<Enemy>();
        }

        public void Add(Enemy enemy)
        {
            Enemies.Add(enemy);
        }

        public List<Enemy> Collide(ICollidable collidable)
        {

            List<Enemy> collideEnemies = new List<Enemy>(); ;
            foreach (Enemy enemy in Enemies)
            {
                if (enemy.BoundCircle.Intersects(collidable.BoundCircle))
                {
                    collideEnemies.Add(enemy);
                }
            }

            if (collideEnemies.Count == 0) return null;

            return collideEnemies;
        }

        public void generateEnemiesWave(Vector2 position, ContentManager content, EnemyWave enemyWave)
        {
            Texture2D texture;
            string basePath = @"Graphics\Enemy\_Robo";

            texture = content.Load<Texture2D>(basePath + (enemyWave.numEnemyTypes + 1));

            for (int i = 0; i < enemyWave.numEnemies; ++i)
            {
                Enemies.Add(new Enemy(position, texture));
            }
           
        }

        public void Update(GameTime gameTime, TileMap tileMap)
        {

            for (int i = 0; i < Enemies.Count; ++i)
            {
                Enemy enemy = Enemies[i];
                if (enemy.Alive)
                    enemy.Update(gameTime, tileMap);
                else
                {
                    this.Enemies.RemoveAt(i);
                    drops++;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }
    }
}
