using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SensorsAndSuch.Screens
{
    public enum ScreenState
    {
        Title,
        Gameplay
    }

    class ScreenStateSwitchboard
    {
        static Game game;
        static SpriteBatch batch;
        static GraphicsDeviceManager graphics;
        static Screen previousScreen;
        static Screen currentScreen;
        static Dictionary<ScreenState, Screen> screens
            = new Dictionary<ScreenState, Screen>();

        GraphicsDevice Device;
        private delegate Screen CreateScreen();

        public ScreenStateSwitchboard(Game game, SpriteBatch batch, GraphicsDeviceManager graphics, GraphicsDevice Device)
        {
            ScreenStateSwitchboard.game = game;
            ScreenStateSwitchboard.batch = batch;
            ScreenStateSwitchboard.graphics = graphics;
            ChangeScreen(ScreenState.Title);

            this.Device = Device;
        }

        private void ChangeScreen(ScreenState screenState)
        {
            switch (screenState)
            {
                case ScreenState.Title:
                    {
                        ChangeScreen(screenState, new CreateScreen(CreateTitleScreen));
                        break;
                    }

                case ScreenState.Gameplay:
                    {
                        ChangeScreen(screenState, new CreateScreen(CreateGameplayScreen));
                        break;
                    }
            }
        }

        private void ChangeScreen(ScreenState screenState, CreateScreen createScreen)
        {
            previousScreen = currentScreen;

            if (!screens.ContainsKey(screenState))
            {
                screens.Add(screenState, createScreen());
                screens[screenState].LoadContent();
            }
            currentScreen = screens[screenState];
            currentScreen.Activate();
        }

        private Screen CreateTitleScreen()
        {
            return new Title(game, batch, new Screen.ChangeScreen(ChangeScreen), graphics);
        }

        private Screen CreateGameplayScreen()
        {
            return new Gameplay(game, batch, new Screen.ChangeScreen(ChangeScreen), graphics, Device);
        }

        public void Update(GameTime gameTime)
        {
            currentScreen.Update(gameTime);
        }

        public void Draw()
        {
            currentScreen.Draw();
        }
    }
}
