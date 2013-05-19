using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using SensorsAndSuch.Texts;

namespace SensorsAndSuch.Screens
{
    class Title : Screen
    {
        Texture2D titleText;

        Button gameButton;

        public Title(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics)
            : base(game, batch, changeScreen, graphics)
        {

        }

        protected override void LoadScreenContent(ContentManager content)
        {
            titleText = content.Load<Texture2D>("Images/TitleText");
            gameButton = new Button(content, "Game", new Vector2(ScreenWidth / 2 - 100, ScreenHeight*3/4), Color.Blue, Color.White);
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            if (input.CheckMousePress(gameButton))
            {
                changeScreenDelegate(ScreenState.Gameplay);
            }
        }

        protected override void DrawScreen(SpriteBatch batch, DisplayOrientation displayOrientation)
        {
            batch.Draw(titleText, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
            gameButton.Draw(batch);
        }
    }
}
