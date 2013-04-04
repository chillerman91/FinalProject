using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Amulet_of_Ouroboros.Texts;

namespace Amulet_of_Ouroboros.Screens
{
    class Title : Screen
    {
        Background background;
        Texture2D titleText;

        Button gameButton;
        Button optionsButton;
        Button exitButton;

        MouseState CurrentMouseState;
        MouseState PreviousMouseState;

        public Title(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics)
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
            titleText = content.Load<Texture2D>("Images/TitleText");
            gameButton = new Button(content, "Game", new Vector2(ScreenWidth / 2 - 100, 400), Color.Blue, Color.White);
            optionsButton = new Button(content, "Options", new Vector2(ScreenWidth / 2 - 100, 475), Color.Blue, Color.White);
            exitButton = new Button(content, "Exit", new Vector2(ScreenWidth / 2 - 100, 550), Color.Blue, Color.White);
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            if (input.CheckMousePress(gameButton))
            {
                changeScreenDelegate(ScreenState.Gameplay);
            }

            if (input.CheckMousePress(optionsButton))
            {
                changeScreenDelegate(ScreenState.Options);
            }

            if (input.CheckMousePress(exitButton))
            {
                changeScreenDelegate(ScreenState.Exit);
            }
        }

        protected override void DrawScreen(SpriteBatch batch, DisplayOrientation displayOrientation)
        {
            background.Draw(batch);
            batch.Draw(titleText, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
            gameButton.Draw(batch);
            optionsButton.Draw(batch);
            exitButton.Draw(batch);
        }
    }
}
