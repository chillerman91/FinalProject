using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Amulet_of_Ouroboros.Screens
{
    public enum ScreenState
    {
        Title,
        Gameplay,
        Options,
        GameOver,
        GameWin,
        PreviousScreen,
        Exit
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

        private delegate Screen CreateScreen();

        public ScreenStateSwitchboard(Game game, SpriteBatch batch, GraphicsDeviceManager graphics)
        {
            ScreenStateSwitchboard.game = game;
            ScreenStateSwitchboard.batch = batch;
            ScreenStateSwitchboard.graphics = graphics;
            ChangeScreen(ScreenState.Title);
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

                case ScreenState.Options:
                    {
                        ChangeScreen(screenState, new CreateScreen(CreateOptionScreen));
                        break;
                    }

                case ScreenState.Gameplay:
                    {
                        ChangeScreen(screenState, new CreateScreen(CreateGameplayScreen));
                        break;
                    }

                case ScreenState.GameOver:
                    {
                        ChangeScreen(screenState, new CreateScreen(CreateGameOverScreen));
                        break;
                    }

                case ScreenState.GameWin:
                    {
                        ChangeScreen(screenState, new CreateScreen(CreateGameWinScreen));
                        break;
                    }

                case ScreenState.PreviousScreen:
                    {
                        currentScreen = previousScreen;
                        currentScreen.Activate();
                        break;
                    }

                case ScreenState.Exit:
                    {
                        game.Exit();
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

        private Screen CreateOptionScreen()
        {
            return new Options(game, batch, new Screen.ChangeScreen(ChangeScreen), graphics);
        }

        private Screen CreateGameplayScreen()
        {
            return new Gameplay(game, batch, new Screen.ChangeScreen(ChangeScreen), graphics);
        }

        private Screen CreateGameOverScreen()
        {
            return new Gameover(game, batch, new Screen.ChangeScreen(ChangeScreen), graphics);
        }

        private Screen CreateGameWinScreen()
        {
            return new Gamewin(game, batch, new Screen.ChangeScreen(ChangeScreen), graphics);
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
