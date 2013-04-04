using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Amulet_of_Ouroboros.Texts;
using System;

namespace Amulet_of_Ouroboros.Sprites
{
    class Minion : Sprite
    {
        public int HP;
        public int DAM;
        public int XP;
        public bool dead;
        Text Health;
        Random ran = new Random();
        public Minion(ContentManager content)
            : base(content, "Images/Minion")
        {
            dead = false;
            HP = ran.Next(5, 20);
            DAM = ran.Next(2, 5);
            XP = 100;
            Position.X = Screens.Screen.ScreenWidth / 2 - this.Width / 2;
            Position.Y = Screens.Screen.ScreenHeight / 2 - this.Height / 2;
            Health = new Text(content.Load<SpriteFont>("Fonts/buttonFont"), "HP: " + HP, new Vector2(Screens.Screen.ScreenWidth / 2 - 100, Screens.Screen.ScreenHeight / 2 - 200), Color.Blue);
        }

        public void Reset()
        {
            dead = false;
            HP = ran.Next(5,20);
            DAM = ran.Next(2,5);
            Health.ChangeText("HP: " + HP);
        }

        protected override void DrawSprite(SpriteBatch batch)
        {
            Health.Draw(batch);
            base.DrawSprite(batch);
        }

        protected override void UpdateSprite(GameTime gameTime)
        {
            Health.ChangeText("HP: " + HP);
            base.UpdateSprite(gameTime);
        }
    }
}
