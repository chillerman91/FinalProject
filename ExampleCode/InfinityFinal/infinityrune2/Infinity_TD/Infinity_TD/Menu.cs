using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace Infinity_TD
{
    class Menu
    {
        // Classe sem uso no jogo

        public Texture2D background, ouroboros, arrow;

        public string[] menuStrings = new string[4];

        float snakeRotation;
        

        enum Options { START, CONTINUE, INSTRUCTIONS, EXIT }

        Options selectedOption = Options.START;

        public void initializeMenu(ContentManager content)
        {
            menuStrings[1] = "START GAME";

            //TODO: ADD TEXTURAS

            background = content.Load<Texture2D>("");
            ouroboros = content.Load<Texture2D>("");
        }

        public void updateMenu()
        {


        }

        public void drawMenu(SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            spriteBatch.Draw(background, Vector2.Zero, Color.White);

            for (int i=1; i <= 4; i++)
            {
                spriteBatch.DrawString(spriteFont, menuStrings[i], Vector2.Zero, Color.White);
            }

            spriteBatch.Draw(ouroboros, Vector2.Zero, null, Color.White, snakeRotation, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }


    }
}
