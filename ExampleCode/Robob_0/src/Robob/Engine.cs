using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Robob.Levels;

namespace Robob
{
    public class Engine : Microsoft.Xna.Framework.Game
    {
		public static ContentManager ContentLoader;
        public static SpriteBatch SpriteBatch;
        public static GraphicsDeviceManager Graphics;
        public static Engine EngineRef;
        public static MusicManager MusicRef;
        public static ProfessorRunner Professor;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private MusicManager music;
        private ProfessorRunner professor;

        public static int screenWidth = 1024;
        public static int screenHeight = 768;

        public bool IsFading
        {
            get { return fading; }
        }

        public bool MusicEnabled
        {
            get { return musicEnabled; }
        }

        private bool musicEnabled = true;
        private bool fading;
        private float fadeStart;
        private float fadeEnd;
        private float fadeTimePassed;
        private float fadeTimeTotal;
        private Texture2D black;

        public static Level[] Levels;
        public static int LevelIndex = 0;
        public static int NextLevelIndex;
        
        public static State CurrentState;
        public static State NextState;
        public static List<State> States = new List<State>();

        public static KeyboardState LastKeyState;
        public static KeyboardState NewKeyState;
        public static GamePadState LastPadState;
        public static GamePadState NewPadState;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            music = new MusicManager ();
			IsFixedTimeStep = false;

            Content.RootDirectory = "Content";

            Engine.ContentLoader = this.Content;
            Engine.EngineRef = this;
            Engine.Graphics = this.graphics;
            Engine.MusicRef = this.music;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

            States.Add(new TitleState());
            States.Add(new GameState());
            States.Add(new CreditState());
            
            //SetState (StateType.Game);
            CurrentState = Engine.States[(int)StateType.Title];
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Engine.SpriteBatch = this.spriteBatch;

            professor = new ProfessorRunner ();
            Engine.Professor = this.professor;

            music.LoadMusic ("Theme");
            music.LoadMusic ("Song1");
            music.LoadMusic ("Song2");
            music.LoadMusic ("Song3");
            music.LoadMusic ("Song4");
            music.LoadMusic ("Song5");
            music.LoadMusic ("Victory");

            music.PlayMusic ("Theme");

            black = new Texture2D (graphics.GraphicsDevice, 1, 1);
            black.SetData (new [] { Color.Black});

            ReloadLevels();

            if (musicEnabled)
            {
                this.music.PlayMusic ("Theme");
                this.music.FadeIn (0.75f);
            }

            Fade (0f, 255f, 0.75f);
            professor.EnqueueTimed ("Hey! I need your help!\nCome into my lab quickly\n by pressing Start Game!", 5);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            NewKeyState = Keyboard.GetState();
            NewPadState = GamePad.GetState(PlayerIndex.One);

            if (CurrentState != null && !professor.NeedsFocus)
                CurrentState.Update (gameTime);

            if (fading)
                UpdateFade (gameTime);

            music.Update (gameTime);
            professor.Update (gameTime);

            LastKeyState = NewKeyState;
            LastPadState = NewPadState;

            base.Update(gameTime);
        }

        public void ResetGame()
        {
            ReloadLevels();
            Professor.Clear();

            var gameState = (GameState)States[(int)StateType.Game];
            gameState.Reset ();
        }

        public void Fade (float start, float end, float time)
        {
            fadeStart = start;
            fadeEnd = end;
            fadeTimeTotal = time;
            fadeTimePassed = 0;
            fading = true;
        }

        private void UpdateFade(GameTime gameTime)
        {
            fadeTimePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (fadeTimePassed >= fadeTimeTotal)
            {
                fading = false;

                if (Engine.NextState != null)
                {
                    if (NextState == States[(int)StateType.Title])
                        ResetGame ();

                    Engine.NextState.Activate();
                    Engine.CurrentState = Engine.NextState;
                    Engine.NextState = null;

                    // Go to the next level
                    if (Engine.NextLevelIndex != Engine.LevelIndex)
                        LevelIndex = NextLevelIndex;

                    Fade (0, 255, 0.75f);
                    EngineRef.music.PlayMusic ("Song1");
                    EngineRef.music.FadeIn (1.5f);
                }
            }
        }

        public static void NextLevel()
        {
            NextLevelIndex = LevelIndex + 1;
            NextState = Engine.States[(int)StateType.Game];
            EngineRef.Fade (255, 0, 0.75f);
            EngineRef.music.FadeOut (0.75f);
            LevelIndex++;
            (CurrentState as GameState).selectingRobot = true;
        }

        public static void SetState (StateType state)
        {
            NextState = Engine.States[(int)state];
            EngineRef.Fade (255, 0, 0.75f);
            EngineRef.music.FadeOut (0.75f);
        }

        public static void ExitGame()
        {
            Engine.EngineRef.Exit();
        }

        public void ReloadLevels()
        {
            Levels = new Level[]
            {
                new LevelOne(), 
                new LevelTwo(),
                new LevelThree()
            };
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (CurrentState != null)
                CurrentState.Draw();

            professor.Draw (spriteBatch);

            if (fading)
            {
                float lerpValue = MathHelper.Lerp (fadeStart, fadeEnd, fadeTimePassed / fadeTimeTotal);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
                spriteBatch.Draw (black, new Rectangle(0, 0, screenWidth, screenHeight), new Color(255, 255, 255, 255 - (int)lerpValue));
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
