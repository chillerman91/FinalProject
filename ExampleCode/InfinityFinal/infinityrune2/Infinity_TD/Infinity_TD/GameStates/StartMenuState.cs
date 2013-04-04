using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Infinity_TD_Library;

namespace Infinity_TD
{
    public sealed class StartMenuState : BaseGameState, IStartMenuState
    {
        private Texture2D texture, arrowTexture;

        SpriteFont font;
        private string[] strings = new string[3];
        int menuStartY = 300;
        int menuStartX = 580;
        int arrowSelectionIndex;
        Vector2 arrowPosition;

        protected override void LoadContent()
        {
            texture = Content.Load<Texture2D>(@"Graphics\Stuff\optionsMenu");
            OurGame.soundManager.soundLoad("Menu");
            OurGame.soundManager.playSong();
            font = Content.Load<SpriteFont>(@"Fonts\menu_font");
            arrowTexture = Content.Load<Texture2D>(@"Graphics\Stuff\arrow");

            //START
            strings[0] = "START GAME";

            //OPTIONS
            strings[1] = "INSTRUCTIONS";

            //EXIT
            strings[2] = "QUIT";

            base.LoadContent();
        }

        public StartMenuState(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IStartMenuState), this);
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.WasPressed(0, InputHandler.ButtonType.X, Keys.Enter))
            {
                switch (arrowSelectionIndex)
                {
                    case 0:
                        if (GameManager.ContainsState(OurGame.PlayingState.Value))
                        {
                            GameManager.PopState(); //got here from our playing state, just pop myself off the stack
                            OurGame.soundManager.stopSong();
                        }
                        else
                            GameManager.ChangeState(OurGame.PlayingState.Value); 
                        break;
                    case 1:
                        if (GameManager.ContainsState(OurGame.OptionsMenuState.Value))
                        {
                            GameManager.PopState(); //got here from our playing state, just pop myself off the stack
                            OurGame.soundManager.stopSong();
                        }
                        else
                            GameManager.ChangeState(OurGame.OptionsMenuState.Value); 
                        break;
                    case 2:
                        OurGame.Exit();
                        break;
                }
            }

            if (Input.WasPressed(0, InputHandler.ButtonType.DPadDown, Keys.Down))
            {
                if (arrowSelectionIndex != strings.Length - 1)
                    arrowSelectionIndex++;
                else arrowSelectionIndex = 0;
            }
            if (Input.WasPressed(0, InputHandler.ButtonType.DPadUp, Keys.Up))
            {
                if (arrowSelectionIndex != 0)
                    arrowSelectionIndex--;
                else arrowSelectionIndex = strings.Length - 1;
            }
            arrowPosition = new Vector2(menuStartX, menuStartY + 11 + (50 * arrowSelectionIndex)); 

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {

            OurGame.SpriteBatch.Draw(texture, Vector2.Zero, Color.White);
            for (int i = 0; i < strings.Length; i++)
            {
                OurGame.SpriteBatch.DrawString(font, strings[i], 
                                                new Vector2(menuStartX + 50, 
                                                            menuStartY + (i * 50)),
                                                            Color.Black);
                OurGame.SpriteBatch.DrawString(font, strings[i],
                                new Vector2(menuStartX + 52,
                                            menuStartY + (i * 50) + 2),
                                            Color.White);
            }

            OurGame.SpriteBatch.Draw(arrowTexture, arrowPosition, Color.White);


            base.Draw(gameTime);
        }

        protected override void StateChanged(object sender, EventArgs e)
        {
            base.StateChanged(sender, e);

            if (GameManager.State != this.Value)
                Visible = true;
        }

    }
}
