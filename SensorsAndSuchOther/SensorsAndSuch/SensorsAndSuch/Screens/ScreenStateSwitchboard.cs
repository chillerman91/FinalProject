using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Inputs;

namespace SensorsAndSuch.Screens
{
    public class SwitchData 
    {
        ScreenState from;
        Keys key;
        Func<bool> canTranfer;
        public ScreenState to { get; private set;}

        internal SwitchData(ScreenState from, Keys key, Func<bool> canTranfer, ScreenState to)
        {
            this.from = from;
            this.key = key;
            this.canTranfer = canTranfer;
            this.to = to;
        }

        internal bool shouldTranfer(GameInput input)
        {
            return !input.PreviousKeyboardState.IsKeyDown(key) && input.CurrentKeyboardState.IsKeyDown(key) && canTranfer();
        }
    }
    
    public enum ScreenState
    {
        Title,
        CreatingMap,
        Playing,
        AnlyseCreatures,
        MapEditing
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

                case ScreenState.Playing:
                    {
                        ChangeScreen(screenState, new CreateScreen(CreatePlayingScreen));
                        break;
                    }
                case ScreenState.MapEditing:
                    {
                        ChangeScreen(screenState, new CreateScreen(CreateMapEditing));
                        break;
                    }

                case ScreenState.AnlyseCreatures:
                    {
                        ChangeScreen(screenState, new CreateScreen(CreateAnalyzeCreatures));
                        break;
                    }
                    
                 case ScreenState.CreatingMap:
                    {
                        ChangeScreen(screenState, new CreateScreen(CreateCreatingMap));
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
        #region Creating Screen
        private Screen CreateTitleScreen()
        {
            return new Title(game, batch, new Screen.ChangeScreen(ChangeScreen), graphics);
        }

        private Screen CreatePlayingScreen()
        {
            return new PlayingScreen(game, batch, new Screen.ChangeScreen(ChangeScreen), graphics, Device);
        }

        private Screen CreateAnalyzeCreatures()
        {
            return new AnalyzeCreatures(game, batch, new Screen.ChangeScreen(ChangeScreen), graphics, Device);
        }

        private Screen CreateMapEditing()
        {
            return new MapEditing(game, batch, new Screen.ChangeScreen(ChangeScreen), graphics, Device);
        }

        private Screen CreateCreatingMap()
        {
            return new CreatingMap(game, batch, new Screen.ChangeScreen(ChangeScreen), graphics, Device);
        }
        #endregion
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
