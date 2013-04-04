using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Amulet_of_Ouroboros.Texts;

namespace Amulet_of_Ouroboros.Screens
{
    class Gamewin : Screen
    {
        Background background;
        Texture2D gamewinText;
        public Gamewin(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics)
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
            gamewinText = content.Load<Texture2D>("Images/GamewinText");
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            if (input.CheckMousePress(ScreenRectangle))
            {
                changeScreenDelegate(ScreenState.Title);
            }
        }

        protected override void DrawScreen(SpriteBatch batch, DisplayOrientation displayOrientation)
        {
            background.Draw(batch);
            batch.Draw(gamewinText, new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
        }
    }
}
