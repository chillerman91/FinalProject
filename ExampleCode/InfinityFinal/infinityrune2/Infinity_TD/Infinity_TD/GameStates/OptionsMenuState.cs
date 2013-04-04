using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Infinity_TD_Library;

namespace Infinity_TD
{
    public sealed class OptionsMenuState : BaseGameState, IOptionsMenuState
    {
        Texture2D texture;
        SpriteFont font;

        protected override void LoadContent()
        {
            texture = Content.Load<Texture2D>(@"Graphics\Stuff\optionsMenu");
            font = Content.Load<SpriteFont>(@"Fonts\menu_font");
            OurGame.soundManager.soundLoad("Menu");
            OurGame.soundManager.playSong();
            base.LoadContent();
        }

        public OptionsMenuState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IOptionsMenuState), this);
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.WasPressed(0, InputHandler.ButtonType.Back, Keys.Escape))
                GameManager.PushState(OurGame.StartMenuState.Value); 
            
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Vector2 pos = new Vector2(TitleSafeArea.Left, TitleSafeArea.Top);
            OurGame.SpriteBatch.Draw(texture, pos, Color.White);
            OurGame.SpriteBatch.DrawString(font, "INSTRUCOES", new Vector2(550, 220), Color.Yellow);
            base.Draw(gameTime);
        }

    }
}
