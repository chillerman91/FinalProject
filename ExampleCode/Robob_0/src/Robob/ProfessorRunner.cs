using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Robob
{
    public class ProfessorRunner
    {
        public ProfessorRunner()
        {
            messages = new Queue<ProfessorMessage> ();
            font = Engine.ContentLoader.Load<SpriteFont> ("UtilityFontLarger");
            professor = Engine.ContentLoader.Load<Texture2D> ("professor");
            chatBox = Engine.ContentLoader.Load<Texture2D> ("textBox");

            professorLocation = new Vector2(Engine.screenWidth - professor.Width, Engine.screenHeight / 2);
            chatBoxLocation = new Vector2((Engine.screenWidth / 2) - (chatBox.Width / 2), Engine.screenHeight / 2);

            chatboxColor = new Color(255, 255, 255, 200);
            textColor = Color.White;
        }

        public bool NeedsFocus
        {
            get { return professorDisplayed && !currentMessage.Timed; }
        }

        public void Update (GameTime gameTime)
        {
            if (professorDisplayed && this.currentMessage.Timed && !fading)
            {
                this.currentMessage.displayedTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
                if (currentMessage.displayedTime >= currentMessage.limit)
                    StartFade();
            }

            if (!professorDisplayed && messages.Count > 0)
            {
                this.currentMessage = messages.Dequeue ();
                professorDisplayed = true;
            }

            if (fading)
            {
                fadePassed += (float) gameTime.ElapsedGameTime.TotalSeconds;

                if (fadePassed >= fadeTotal)
                {
                    professorDisplayed = false;
                    fading = false;
                }
            }

            HandleInput ();
        }

        private void StartFade()
        {
            fadePassed = 0;
            fading = true;
        }

        public void Clear()
        {
            fading = false;
            professorDisplayed = false;
            currentMessage = null;
            messages.Clear();
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            if (!professorDisplayed)
                return;

            var lines = currentMessage.Message.Split ('\n');

            Color color = textColor;
            Color fadeColor = chatboxColor;
            Color professorColor = Color.White;

            if (fading)
            {
                float lerpAmount = fadePassed / fadeTotal;
                color = new Color (textColor.R, textColor.G, textColor.B, (int)MathHelper.Lerp (255, 0, lerpAmount));
                fadeColor = new Color (255, 255, 255, (int)MathHelper.Lerp (200, 0, lerpAmount));
                professorColor = new Color (255, 255, 255, (int)MathHelper.Lerp (255, 0, lerpAmount));
            }

            spriteBatch.Begin (SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw (professor, professorLocation, professorColor);
            spriteBatch.Draw (chatBox, chatBoxLocation, fadeColor);

            int offset = 0;
            int subOffset = lines.Length - 1;
            if (subOffset < 0)
                subOffset = 0;

            foreach (var line in lines)
            {
                Vector2 measure = font.MeasureString (line);
                textLocation = new Vector2 ((chatBoxLocation.X + (chatBox.Width / 2)) - (measure.X / 2), Engine.screenHeight / 2 + 100);

                spriteBatch.DrawString (font, line, textLocation + new Vector2 (0, (offset * font.LineSpacing) - (subOffset * font.LineSpacing)), color);
                offset++;
            }
            spriteBatch.End();
        }

        public void EnqueueMessageBox (string text, Action messageClosed)
        {
            ProfessorMessage message = new ProfessorMessage { Message = text, Action = messageClosed };
            messages.Enqueue (message);
        }

        public void ShowTimed (string text, float time)
        {
            ProfessorMessage message = new ProfessorMessage
            {
                Message = text,
                Timed = true,
                limit = time
            };

            professorDisplayed = true;
            currentMessage = message;
            messages.Clear();
        }

        public void EnqueueTimed(string text, float time)
        {
            ProfessorMessage message = new ProfessorMessage
            {
                Message = text,
                Timed = true,
                limit = time
            };

            messages.Enqueue (message);
        }

        private Queue<ProfessorMessage> messages;
        private SpriteFont font;
        private Texture2D professor;
        private Texture2D chatBox;
        private bool professorDisplayed;
        private Vector2 professorLocation;
        private Vector2 textLocation;
        private Vector2 chatBoxLocation;
        private ProfessorMessage currentMessage;
        private Color textColor;
        private Color chatboxColor;
        private bool fading;
        private float fadeTotal = 0.75f;
        private float fadePassed;

        private void HandleInput()
        {
            if (professorDisplayed && !currentMessage.Timed)
            {
                if (keyPressed (Keys.Enter) || (Engine.NewPadState.IsButtonDown(Buttons.A) && Engine.LastPadState.IsButtonUp(Buttons.A)))
                {
                    if (currentMessage.Action != null)
                        currentMessage.Action ();

                    professorDisplayed = false;
                }
            }
        }

        private bool keyPressed (Keys key)
        {
            return Engine.LastKeyState.IsKeyUp (key) && Engine.NewKeyState.IsKeyDown (key);
        }

        private class ProfessorMessage
        {
            public string Message;
            public Action Action;
            public bool Timed;
            public float limit;
            public float displayedTime;
        }
    }
}
