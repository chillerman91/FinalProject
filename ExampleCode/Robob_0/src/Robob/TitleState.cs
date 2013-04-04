using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Robob
{
    public class TitleState : State
    {
        public class Button
        {
            public string Text;
            public Vector2 Translation;
        }

        public int SelectedItem;
        public Texture2D Background;
        public List<Button> Buttons;
        public int SelectedButton = 0;
        public SpriteFont Font;

        public TitleState()
        {
            Background = Engine.ContentLoader.Load<Texture2D>("TitleScreen");
            Font = Engine.ContentLoader.Load<SpriteFont>("UtilityFont");

            Buttons = new List<Button>();

            Button but = new Button();
            but.Text = "Start Game";
            but.Translation = new Vector2(640.0f, 160.0f);
            Buttons.Add(but);

            but = new Button();
            but.Text = "Credits";
            but.Translation = new Vector2(650.0f, 200.0f);
            Buttons.Add(but);

            but = new Button();
            but.Text = "Exit";
            but.Translation = new Vector2(665.0f, 240.0f);
            Buttons.Add(but);
        }

        public override void Update(GameTime gameTime)
        {
            if (Engine.EngineRef.IsFading)
                return;

            if (Engine.NewKeyState.IsKeyDown(Keys.Escape) || Engine.NewPadState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Back))
                Engine.ExitGame();

            if (Engine.NewKeyState.IsKeyDown (Keys.Down) && !Engine.LastKeyState.IsKeyDown (Keys.Down))
            {
                ++SelectedButton;
                if (SelectedButton > Buttons.Count - 1)
                    SelectedButton = 0;
            }
            else if (Engine.NewKeyState.IsKeyDown (Keys.Up) && !Engine.LastKeyState.IsKeyDown (Keys.Up))
            {
                --SelectedButton;
                if (SelectedButton < 0)
                    SelectedButton = Buttons.Count - 1;
            }
            else if ((Engine.NewKeyState.IsKeyDown(Keys.Enter) && !Engine.LastKeyState.IsKeyDown(Keys.Enter)) ||
                    (Engine.NewPadState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A) && Engine.LastPadState.IsButtonUp(Microsoft.Xna.Framework.Input.Buttons.A)))
            {
                switch (SelectedButton)
                {
                    case 0:
                        Engine.SetState(StateType.Game);
                        break;

                    case 1:
                        Engine.SetState(StateType.Credits);
                        break;

                    case 2:
                        Engine.ExitGame();
                        break; 

                    default:
                        break;
                }
            }
            if (Engine.NewPadState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadDown) && !Engine.LastPadState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadDown))
            {
                ++SelectedButton;
                if (SelectedButton > Buttons.Count - 1)
                    SelectedButton = 0;
            }
            else if (Engine.NewPadState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadUp) && !Engine.LastPadState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadUp))
            {
                --SelectedButton;
                if (SelectedButton < 0)
                    SelectedButton = Buttons.Count - 1;
            }

            base.Update(gameTime);
        }

        public override void Draw()
        {
            Engine.SpriteBatch.Begin();
            {
                Engine.SpriteBatch.Draw(Background, new Rectangle(0, 0, Engine.Graphics.PreferredBackBufferWidth, Engine.Graphics.PreferredBackBufferHeight), Color.White);

                for (int i = 0; i < Buttons.Count; ++i)
                {
                    Engine.SpriteBatch.DrawString(Font, Buttons[i].Text, Buttons[i].Translation + new Vector2(2, 2), Color.Black);

                    if (i == SelectedButton)
                        Engine.SpriteBatch.DrawString(Font, Buttons[i].Text, Buttons[i].Translation, Color.Red);
                    else
                        Engine.SpriteBatch.DrawString(Font, Buttons[i].Text, Buttons[i].Translation, Color.White);
                }
            } Engine.SpriteBatch.End();

            base.Draw();
        }
    }
}
