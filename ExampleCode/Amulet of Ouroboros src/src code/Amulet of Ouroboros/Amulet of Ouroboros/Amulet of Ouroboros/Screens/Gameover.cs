using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;
using Amulet_of_Ouroboros.Texts;

namespace Amulet_of_Ouroboros.Screens
{
    class Gameover : Screen
    {
        Background background;

        public Gameover(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics)
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
            background = new Background(content, "Images/GameoverText");
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
        }
    }
}
