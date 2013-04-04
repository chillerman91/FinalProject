using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Amulet_of_Ouroboros.Sounds;
using Amulet_of_Ouroboros.Inputs;

namespace Amulet_of_Ouroboros.Screens
{
    class Screen
    {
        protected static Game game;
        protected static ContentManager content;
        protected static SpriteBatch batch;
        protected static GraphicsDeviceManager graphics;
        protected static Random random = new Random();
        protected SpriteFont font;
        protected static Music music;
        protected static SoundEffects soundEffects;

        public ChangeScreen changeScreenDelegate;
        public delegate void ChangeScreen(ScreenState screen);

        protected GameInput input = new GameInput();

        const string ActionBack = "Back";

        public Screen(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics)
        {
            Screen.game = game;
            Screen.content = game.Content;
            Screen.batch = batch;
            Screen.graphics = graphics;
            changeScreenDelegate = changeScreen;

            if (music == null)
            {
                music = new Music(content);
            }

            if (soundEffects == null)
            {
                soundEffects = new SoundEffects(content);
            }
        }

        public virtual void Activate()
        {
        }

        public void LoadContent()
        {
            font = content.Load<SpriteFont>("Fonts/screenFont");
            LoadScreenContent(content);
            input.AddGamePadInput(ActionBack, Buttons.Back, true);
            SetupInputs();
        }

        protected virtual void SetupInputs()
        {
        }

        protected virtual void LoadScreenContent(ContentManager content)
        {
        }

        public void Update(GameTime gameTime)
        {
            input.BeginUpdate();

            UpdateScreen(gameTime, game.GraphicsDevice.PresentationParameters.DisplayOrientation);

            input.EndUpdate();
        }

        
        protected virtual void UpdateScreen(GameTime gameTime, DisplayOrientation screenOrientation)
        {
        }

        public void Draw()
        {
            batch.Begin();
            DrawScreen(batch, game.GraphicsDevice.PresentationParameters.DisplayOrientation);
            batch.End();
        }

        protected virtual void DrawScreen(SpriteBatch batch, DisplayOrientation screenOrientation)
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

        static public Rectangle ScreenRectangle
        {
            get { return new Rectangle(0, 0, ScreenWidth, ScreenHeight); }
        }

        static public Rectangle ScreenLeftHalf
        {
            get { return new Rectangle(0, 0, (int)(ScreenWidth / 2), ScreenHeight); }
        }

        static public Rectangle ScreenRightHalf
        {
            get { return new Rectangle((int)(ScreenWidth / 2), 0, (int)(ScreenWidth / 2), ScreenHeight); }
        }

    }
}
