using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Infinity_TD
{
    class Tower : ICollidable
    {
        private int range = 200;
        public Vector2 Position;
        Animacao animation;
        float towerSpawn = 0.0f;
        SoundManager soundaManager;
        public Texture2D shot_text;
        public List<Shot> Shots
        {
            get;
            private set;
        }
        private Effect effect;

        private float Damage
        {
            get;
            set;
        }

        public float FireRate
        {
            get;
            set;
        }


        private Rectangle boundRect;
        public Rectangle BoundRect
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, animation.larguraFrame, animation.alturaFrame);
            }

            set
            {
                boundRect = value;
            }
        }

        public BoundingSphere boundCircle;
        public BoundingSphere BoundCircle
        {
            get
            {
                return new BoundingSphere(new Vector3(this.Position, 0.0f), range);
            }
            set
            {
                boundCircle = value;
            }
        }

        public static T1 getTower<T1>(Game game, float damage, Vector2 position, float fireRate) where T1 : Tower
        {
            return (T1)Activator.CreateInstance(typeof(T1), game, damage, position, fireRate);
        }

        public Tower(Game game, float damage, Vector2 position, float fireRate, string textureName, string shotTexture, Effect effect)
        {
            Damage = damage;
            this.FireRate = fireRate;
            this.Position = position;
            soundaManager = ((Game1)game).soundManager;
            Texture2D texture = game.Content.Load<Texture2D>(@"Graphics\Tower\" + textureName);
            shot_text = game.Content.Load<Texture2D>(@"Graphics\Stuff\Shots\" + shotTexture);
            this.effect = effect;
            this.animation = new Animacao(texture, this.Position, 32, 32, 2, 90, 1.0f, true);
            Shots = new List<Shot>();
        }

        public void onCollision(Object sender)
        {

        }

        public virtual void FireToEnemy(Enemy enemy, Vector2 positionSource, Texture2D texture) 
        {  
        }

        protected void FireToEnemy(Enemy enemy, Vector2 positionSource, Texture2D texture, int i)
        {
            if (towerSpawn > FireRate)
            {
                Shot shot = new Shot(texture, positionSource, enemy, 5.0f);
                effect.origin = this.Position;
                effect.Radious = this.range;
                shot.Effect = effect;
                soundaManager.playSound(i);
                Shots.Add(shot);
                towerSpawn = 0.0f;

            }
            // return shot;
        }

        public virtual void Update(GameTime gameTime)
        {
            this.animation.Update(gameTime, Position);

            towerSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < Shots.Count; ++i)
            {
                Shot shot = Shots[i];
                if (!shot.Alive)
                {
                    Shots.RemoveAt(i);
                }
            }

            foreach (Shot shot in Shots)
            {
                shot.Update(gameTime);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            this.animation.Draw(spriteBatch, MathHelper.ToRadians(0.0f));

            foreach (Shot shot in Shots)
            {
                shot.Draw(spriteBatch);
            }
        }
    }
}
