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
using SensorsAndSuch.Screens;
using FarseerPhysics.SamplesFramework;

namespace SensorsAndSuch
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        SpriteFont debugFont;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ScreenStateSwitchboard screen;
        public ScreenManager ScreenManager { get; set; }

        public Game1()
        {

            Window.Title = "The Sensor Game";
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1500;// 1024;
            graphics.PreferredBackBufferHeight = 800;// 1.2 * 768;
            Content.RootDirectory = "Content";
            Globals.ScreenManager = ScreenManager;
        }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            debugFont = Content.Load<SpriteFont>("Fonts/buttonFont");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            screen = new ScreenStateSwitchboard(this, spriteBatch, graphics, GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            screen.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            screen.Draw();
            base.Draw(gameTime);
        }
    }
}
