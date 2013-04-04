//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;

//namespace JungleSurvivor
//{
//    public class Game1 : Microsoft.Xna.Framework.Game
//    {
//        GraphicsDeviceManager graphics;

//        SpriteBatch spriteBatch;

//        Song song;

//        SpriteFont font;

//        TileMap tileMap = new TileMap();

//        List<Hole> holeList = new List<Hole>();

//        Hole hole = new Hole();

//        MouseState state, previousMouseState;

//        enum Screens { MENU, GAME, PAUSE, END }

//        Screens currentScreen = Screens.MENU;

//        Texture2D menuTexture, levelBackground, heroTexture, bar1, bar2, bar3, bar4, upperBar, tools1, tools2, tools3, tools4, tools5;

//        KeyboardState previousState;

//        public Game1()
//        {
//            graphics = new GraphicsDeviceManager(this);
//            Content.RootDirectory = "Content";
//            graphics.PreferredBackBufferHeight = 768;
//            graphics.PreferredBackBufferWidth = 1024;
//            IsMouseVisible = true;

//        }

 
//        protected override void Initialize()
//        {
//            hole.LoadTexture(this.Content);
//            hole.position = new Vector2(448, 448);
//            hole.area = new Rectangle(488, 448, 8, 64);

//            tileMap.initializeMap();
//            base.Initialize();
//        }

//        protected override void LoadContent()
//        {
//            song = Content.Load<Song>("song");
//            bar1 = Content.Load<Texture2D>("Graphics/bars/barra_agua");
//            bar2 = Content.Load<Texture2D>("Graphics/bars/barra_corda");
//            bar3 = Content.Load<Texture2D>("Graphics/bars/barra_madeira");
//            bar4 = Content.Load<Texture2D>("Graphics/bars/barra_pedra");
//            upperBar = Content.Load<Texture2D>("Graphics/bars/barra_superior");
//            tools1 = Content.Load<Texture2D>("Graphics/buttons/buraco");
//            tools2 = Content.Load<Texture2D>("Graphics/buttons/estilingue");
//            tools3 = Content.Load<Texture2D>("Graphics/buttons/lama");
//            tools4 = Content.Load<Texture2D>("Graphics/buttons/rede");
//            tools5 = Content.Load<Texture2D>("Graphics/buttons/bola_lama");
//            font = Content.Load<SpriteFont>("Graphics/font");
//            menuTexture = Content.Load<Texture2D>("Graphics/menu");
//            levelBackground = Content.Load<Texture2D>("Graphics/background");
//            heroTexture = Content.Load<Texture2D>("Graphics/hero");
//            spriteBatch = new SpriteBatch(GraphicsDevice);
//        }

//        protected override void UnloadContent()
//        {
//        }

//        protected override void Update(GameTime gameTime)
//        {
//            state = Mouse.GetState();

//            if ((state.LeftButton == ButtonState.Pressed) && (Resources.wood >= 50) && (previousMouseState.LeftButton == ButtonState.Released))
//            {
//                Resources.wood -= 50;
//                Hole newhole = new Hole();
//                newhole.position = new Vector2(state.X-(state.X%64), state.Y-(state.Y%64));
//                newhole.area.X = (int)newhole.position.X+56;
//                newhole.area.Y = (int)newhole.position.Y;
//                newhole.area.Width = 8;
//                newhole.area.Height = 64;
//                newhole.LoadTexture(this.Content);
//                holeList.Add(newhole);
//            }

//            if (MediaPlayer.State == MediaState.Stopped)
//            {
//                MediaPlayer.Play(song);
//            }

//            switch (currentScreen)
//            {

//                case Screens.MENU:
//                    {
//                        if ((Keyboard.GetState().IsKeyDown(Keys.Enter)) && (previousState.IsKeyUp(Keys.Enter))) currentScreen = Screens.GAME;
//                    }
//                    break;
//                case Screens.GAME:
//                    {
//                        if ((Keyboard.GetState().IsKeyDown(Keys.Escape)) && (previousState.IsKeyUp(Keys.Escape))) this.Exit();
//                        for (int i = 0; i < tileMap.EnemyList.Count(); i++)
//                        {
//                            tileMap.EnemyList[i].UpdateMovement(gameTime);
//                            if (tileMap.EnemyList[i].active == false) tileMap.EnemyList.RemoveAt(i);
//                        }
//                        tileMap.UpdateSpawners(gameTime, this.Content);
//                        foreach (Hole upHole in holeList)
//                        {
//                            upHole.Update(tileMap.EnemyList, gameTime);
//                        }
//                    }
//                    break;
//                case Screens.PAUSE:
//                    {

//                    }
//                    break;
//            }

//            previousState = Keyboard.GetState();
//            previousMouseState = Mouse.GetState();

//            base.Update(gameTime);
//        }

//        protected override void Draw(GameTime gameTime)
//        {
//            GraphicsDevice.Clear(Color.CornflowerBlue);

//            spriteBatch.Begin();

//            switch (currentScreen)
//            {
//                case Screens.MENU:
//                    spriteBatch.Draw(menuTexture, Vector2.Zero, Color.White);
//                    spriteBatch.DrawString(font, "PRESSIONE ENTER PARA COMECAR", new Vector2(242, 602), Color.Black);
//                    spriteBatch.DrawString(font, "PRESSIONE ENTER PARA COMECAR", new Vector2(240, 600), Color.Red);
                    
//                    break;
//                case Screens.GAME:
//                    {
//                        spriteBatch.Draw(levelBackground, Vector2.Zero, Color.White);
//                        foreach (Hole drawHole in holeList) drawHole.Draw(spriteBatch);
//                        foreach (Enemy enemy in tileMap.EnemyList) enemy.Draw(spriteBatch);
//                        spriteBatch.Draw(heroTexture, new Rectangle(960, 352, 64,64), null, Color.White, 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);

//                        spriteBatch.Draw(bar1, new Vector2(50, 600), Color.White);
//                        spriteBatch.Draw(bar2, new Vector2(278, 600), Color.White);
//                        spriteBatch.Draw(bar3, new Vector2(506, 600), Color.White);
//                        spriteBatch.Draw(bar4, new Vector2(734, 600), Color.White);
//                        spriteBatch.DrawString(font, Resources.water.ToString(), new Vector2(202, 647), Color.Black);
//                        spriteBatch.DrawString(font, Resources.water.ToString(), new Vector2(200, 645), Color.Blue);
//                        spriteBatch.DrawString(font, Resources.rope.ToString(), new Vector2(430, 647), Color.Black);
//                        spriteBatch.DrawString(font, Resources.rope.ToString(), new Vector2(428, 645), Color.Blue);
//                        spriteBatch.DrawString(font, Resources.wood.ToString(), new Vector2(658, 647), Color.Black);
//                        spriteBatch.DrawString(font, Resources.wood.ToString(), new Vector2(656, 645), Color.Brown);
//                        spriteBatch.DrawString(font, Resources.stone.ToString(), new Vector2(886, 647), Color.Black);
//                        spriteBatch.DrawString(font, Resources.stone.ToString(), new Vector2(884, 645), Color.Blue);

//                        //spriteBatch.Draw(upperBar, Vector2.Zero, Color.White);

//                        //DEBUG

//                        spriteBatch.DrawString(font, hole.cooldown.ToString(), Vector2.Zero, Color.White);
                        
//                    }
//                    break;
//            }

//            spriteBatch.End();

//            base.Draw(gameTime);
//        }
//    }
//}
