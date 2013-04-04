using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Amulet_of_Ouroboros.Texts;

namespace Amulet_of_Ouroboros.Screens
{
    class Options : Screen
    {
        Background background;
        Texture2D optionsText;

        Button fullscreenButton;
        Button backButton;

        public Options(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics)
            : base(game, batch, changeScreen, graphics)
        {

        }

        protected override void SetupInputs()
        {

        }

        public override void Activate()
        {

        }

        protected override void LoadScreenContent(ContentManager content)
        {
            background = new Background(content, "Images/TitleBackground");
            optionsText = content.Load<Texture2D>("Images/OptionsText");
            fullscreenButton = new Button(content, "Full Screen", new Vector2(ScreenWidth / 2 - 100, 300), Color.Blue, Color.White);
            backButton = new Button(content, "Back", new Vector2(ScreenWidth / 2 - 100, 500), Color.Blue, Color.White);
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            if(input.CheckMousePress(fullscreenButton))
            {
                graphics.ToggleFullScreen();
            }

            if(input.CheckMousePress(backButton))
            {
                changeScreenDelegate(ScreenState.Title);
            }
        }

        protected override void DrawScreen(SpriteBatch batch, DisplayOrientation displayOrientation)
        {
            background.Draw(batch);
            batch.Draw(optionsText, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
            fullscreenButton.Draw(batch);
            backButton.Draw(batch);
        }
    }
}
