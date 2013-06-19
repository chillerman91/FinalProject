using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Inputs;

namespace SensorsAndSuch.Screens
{
    abstract class Screen
    {
        protected static Game game;
        protected static ContentManager content;
        protected static SpriteBatch batch;
        protected static GraphicsDeviceManager graphics;
        protected static Random random = new Random();
        protected SpriteFont font;
        protected GameInput input = new GameInput();


        public ChangeScreen changeScreenDelegate;
        public delegate void ChangeScreen(ScreenState screen);

        public Screen(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics)
        {
            Screen.game = game;
            Screen.content = game.Content;
            Screen.batch = batch;
            Screen.graphics = graphics;
            changeScreenDelegate = changeScreen;
        }

        public virtual void Activate()
        {
        }

        public void LoadContent()
        {
            font = content.Load<SpriteFont>("Fonts/debugFont");
            LoadScreenContent(content);
        }

        public void Update(GameTime gameTime)
        {
            input.BeginUpdate();
            UpdateScreen(gameTime, game.GraphicsDevice.PresentationParameters.DisplayOrientation);
            input.EndUpdate();
        }

        protected virtual void LoadScreenContent(ContentManager content) { }
        
        protected virtual void UpdateScreen(GameTime gameTime, DisplayOrientation screenOrientation)
        {
        }

        public void Draw()
        {
            batch.Begin();
            Draw(batch, game.GraphicsDevice.PresentationParameters.DisplayOrientation);
            batch.End();
        }

        protected virtual void Draw(SpriteBatch batch, DisplayOrientation screenOrientation)
        {
        }

        public void SaveState()
        {
            SaveScreenState();
        }

        protected virtual void SaveScreenState()
        {
        }

        static public int ScreenWidth
        {
            get { return game.GraphicsDevice.PresentationParameters.BackBufferWidth; }
        }

        static public int ScreenHeight
        {
            get { return game.GraphicsDevice.PresentationParameters.BackBufferHeight; }
        }
    }
}
