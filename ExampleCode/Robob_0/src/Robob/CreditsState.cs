using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Robob
{
    public class CreditState : State
    {
        Texture2D background;
        SpriteFont font;

        public CreditState()
        {
            background = Engine.ContentLoader.Load<Texture2D>("TitleScreen");
            font = Engine.ContentLoader.Load<SpriteFont>("UtilityFontLarger");
        }

        public override void Activate()
        {
            Engine.Professor.EnqueueTimed("Programmer:\nJason Spafford", 5.0f);
            Engine.Professor.EnqueueTimed("Artist:\nDennis Wilkins", 5.0f);
            Engine.Professor.EnqueueTimed("Programmer:\nJesse 'Jeaye' Wilkerson", 5.0f);
            Engine.Professor.EnqueueTimed("Audio:\nDaniel Cook", 5.0f);
            Engine.Professor.EnqueueTimed("Support:\nRafael Sanchez", 5.0f);
            base.Activate();
        }

        public override void Update(GameTime gameTime)
        {
            if (Engine.NewKeyState.IsKeyDown (Keys.Escape) || Engine.NewPadState.IsButtonDown (Buttons.Back))
            {
                Engine.SetState (StateType.Title);
                Engine.Professor.Clear ();
            }

            base.Update(gameTime);
        }

        public override void Draw()
        {
            Engine.SpriteBatch.Begin();
            {
                Engine.SpriteBatch.Draw(background, new Rectangle(0, 0, 1024, 768), Color.White);

                Engine.SpriteBatch.DrawString(font, "GGJA", new Vector2(652.0f, 202.0f), Color.Black);
                Engine.SpriteBatch.DrawString(font, "GGJA", new Vector2(650.0f, 200.0f), Color.White);
            } Engine.SpriteBatch.End();

            base.Draw();
        }

    }
}
