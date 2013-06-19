using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using SensorsAndSuch.Texts;
using SensorsAndSuch.Maps;
using SensorsAndSuch.Mobs;
using FarseerPhysics.SamplesFramework;
using System.Collections.Generic;
using System.Xml;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SensorsAndSuch;
using FarseerPhysics.Dynamics;
using SharpNeat.Domains;

namespace SensorsAndSuch.Screens
{
    class PlayingScreen : GameWorldScreenBase
    {
        /// <summary>
        /// The Screen were the player is moving around and fighting stuff.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="batch"></param>
        /// <param name="changeScreen"></param>
        /// <param name="graphics"></param>
        /// <param name="Device"></param>
        public PlayingScreen(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics, GraphicsDevice device)
            : base(game, batch, changeScreen, graphics, device)
        {
            thisState = ScreenState.Playing;
            switchToStates.Add(new SwitchData(from: thisState, key: Keys.M, canTranfer: () => player.CanUseMap, to: ScreenState.MapEditing));
            switchToStates.Add(new SwitchData(from: thisState, key: Keys.C, canTranfer: () => true, to: ScreenState.AnlyseCreatures));
        }

        protected override void AdjustCam()
        {
            Globals.map.globalScale = Globals.map.globalScale * .99f + .7f * .01f;
            Vector2 temp2 = Globals.map.getScreenSizeInPhysics();
            Globals.map.ToLocation = Globals.map.ToLocation * .99f + (player.Body.Position + player.Dir * 5 * Globals.map.globalScale - temp2 * .5f) * .01f;
            if (!(Globals.map.ToLocation.X < 0 && Globals.map.ToLocation.X > RandomMap.mapWidth * BaseTile.TileWidth - temp2.X))
            {
                if (Globals.map.ToLocation.X < 0)
                    Globals.map.ToLocation.X = 0;
                if (Globals.map.ToLocation.X > RandomMap.mapWidth * BaseTile.TileWidth - temp2.X)
                    Globals.map.ToLocation.X = RandomMap.mapWidth * BaseTile.TileWidth - temp2.X;
            }

            if (!(Globals.map.ToLocation.Y < 0 && Globals.map.ToLocation.Y >= RandomMap.mapHeight * BaseTile.TileHeight - temp2.Y))
            {
                if (Globals.map.ToLocation.Y < 0)
                    Globals.map.ToLocation.Y = 0;
                if (Globals.map.ToLocation.Y >= RandomMap.mapHeight * BaseTile.TileHeight - temp2.Y)
                    Globals.map.ToLocation.Y = RandomMap.mapHeight * BaseTile.TileHeight - temp2.Y;
            }
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            base.Update(gameTime);
                
            if (!input.PreviousKeyboardState.IsKeyDown(Keys.O) && input.CurrentKeyboardState.IsKeyDown(Keys.O))
            {
                Globals.player.kills++;
                Globals.player.ApplyBonuses(Globals.player.kills);
            }


            if (!playerPaused)
            {
                playerMovement();
                UpdateAll();
            }
        }
/*
        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            base.Update(gameTime);
            Globals.GameTime = gameTime;
            
            Globals.tick = tick;
            if (gameStart && !input.PreviousKeyboardState.IsKeyDown(Keys.P) && input.CurrentKeyboardState.IsKeyDown(Keys.P))
            {
                paused = !paused;
            }
            ++tick;
            if (!input.PreviousKeyboardState.IsKeyDown(Keys.F1) && input.CurrentKeyboardState.IsKeyDown(Keys.F1))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("\t");
                settings.OmitXmlDeclaration = true;
                paused = !paused;
            }

            #region State Handling
            if (currentState == ScreenState.CreatingMap) 
            { 

                if (!gameStart)
                {
                    if ((tick%20 == 0 && input.CurrentMouseState.LeftButton == ButtonState.Pressed) || input.PreviousMouseState.LeftButton == ButtonState.Released)
                    {
                        if (Globals.map.CreateMap() == 4)
                        {
                            currentState = ScreenState.Playing;
                            StartGame();
                        }
                    }
                    return;
                }
            }
            else if (currentState == ScreenState.AnlyseCreatures)
            {
                if (!input.PreviousKeyboardState.IsKeyDown(Keys.C) && input.CurrentKeyboardState.IsKeyDown(Keys.C))
                {
                    currentState = ScreenState.Playing;
                }


                Globals.map.globalScale = Globals.map.globalScale * .99f + .5f * .01f;
                int ScreenposBasedOnCenterX = input.CurrentMouseState.X - Screen.ScreenWidth / 2;
                float broughtToOneX = (float)ScreenposBasedOnCenterX / (Screen.ScreenWidth / 2);
                float finalX = (float) (Math.Pow(broughtToOneX, 5) * 10);

                int ScreenposBasedOnCenterY = input.CurrentMouseState.Y - Screen.ScreenHeight / 2;
                float broughtToOneY = (float)ScreenposBasedOnCenterY / (Screen.ScreenHeight / 2);
                float finalY = (float)(Math.Pow(broughtToOneY, 9) * 7);

                Vector2 ToLocation = new Vector2(finalX, finalY);
                //ToLocation.
                ToLocation = Globals.map.ToLocation + ToLocation;
                Globals.map.ToLocation = Globals.map.ToLocation * .95f + ToLocation * .05f;
                if (Globals.map.ToLocation.X < 0)
                    Globals.map.ToLocation.X = 0;
                if (Globals.map.ToLocation.Y < 0)
                    Globals.map.ToLocation.Y = 0;
                Vector2 temp2 = Globals.map.getScreenSizeInPhysics();

                if (Globals.map.ToLocation.X > RandomMap.mapWidth * BaseTile.TileWidth - temp2.X)
                    Globals.map.ToLocation.X = RandomMap.mapWidth * BaseTile.TileWidth - temp2.X;
                if (Globals.map.ToLocation.Y >= RandomMap.mapHeight * BaseTile.TileHeight - temp2.Y)
                    Globals.map.ToLocation.Y = RandomMap.mapHeight * BaseTile.TileHeight - temp2.Y;

                Globals.Mobs.UpdateAnalyzeCreatures(input);
            }
            else if (currentState == ScreenState.MapEditing)
            {
                Globals.map.ToLocation = Globals.map.ToLocation * .95f + new Vector2(0, 0) * (Globals.map.PosMod) * .05f;
                Globals.map.globalScale = Globals.map.globalScale * .99f + .3f * .01f;
                if (!input.PreviousKeyboardState.IsKeyDown(Keys.M) && input.CurrentKeyboardState.IsKeyDown(Keys.M))
                {
                    Globals.map.SetTextEmpty();
                    currentState = ScreenState.Playing;
                    return;
                } 
                if (!input.PreviousKeyboardState.IsKeyDown(Keys.D) && input.CurrentKeyboardState.IsKeyDown(Keys.D))
                {
                    shouldDraw = !shouldDraw;
                    return;
                }

            }
            else if (currentState == ScreenState.Playing || currentState == ScreenState.Ghost)
            {
                //Globals.map.globalScale = Globals.map.globalScale * .99f + .5f * .01f;
                //Globals.map.ToLocation = Globals.map.ToLocation * .95f + new Vector2(((int)player.shape.Position.X) / Globals.map.PosMod, ((int)player.shape.Position.Y) / Globals.map.PosMod) * (Globals.map.PosMod) * .05f;

                Globals.map.globalScale = Globals.map.globalScale * .99f + .7f * .01f;
                Vector2 temp2 = Globals.map.getScreenSizeInPhysics();
                Globals.map.ToLocation = Globals.map.ToLocation * .99f + (player.Body.Position + player.Dir * 5 * Globals.map.globalScale - temp2 * .5f) * .01f;
                if (!(Globals.map.ToLocation.X < 0 && Globals.map.ToLocation.X > RandomMap.mapWidth * BaseTile.TileWidth - temp2.X))
                {
                    if (Globals.map.ToLocation.X < 0)
                        Globals.map.ToLocation.X = 0;
                    if (Globals.map.ToLocation.X > RandomMap.mapWidth * BaseTile.TileWidth - temp2.X)
                        Globals.map.ToLocation.X = RandomMap.mapWidth * BaseTile.TileWidth - temp2.X;
                }

                if (!(Globals.map.ToLocation.Y < 0 && Globals.map.ToLocation.Y >= RandomMap.mapHeight * BaseTile.TileHeight - temp2.Y))
                {
                    if (Globals.map.ToLocation.Y < 0)
                        Globals.map.ToLocation.Y = 0;
                    if (Globals.map.ToLocation.Y >= RandomMap.mapHeight * BaseTile.TileHeight - temp2.Y)
                        Globals.map.ToLocation.Y = RandomMap.mapHeight * BaseTile.TileHeight - temp2.Y;
                }
                if (!input.PreviousKeyboardState.IsKeyDown(Keys.M) && input.CurrentKeyboardState.IsKeyDown(Keys.M))
                {
                    if (player.CanUseMap)
                    {
                        currentState = ScreenState.MapEditing;
                    }
                }

                if (!input.PreviousKeyboardState.IsKeyDown(Keys.C) && input.CurrentKeyboardState.IsKeyDown(Keys.C))
                {
                    currentState = ScreenState.AnlyseCreatures;
                }

            } else if (currentState == ScreenState.Dead)
            {
                if (Globals.Debugging)
                {
                    if (!input.PreviousKeyboardState.IsKeyDown(Keys.O) && input.CurrentKeyboardState.IsKeyDown(Keys.O))
                    {
                        currentState = ScreenState.Playing;
                        Globals.player.SetRandPosSafe();
                        Globals.map.setNotDull();

                    }
                    Globals.player.timeAtDeath = Globals.GameTime.TotalGameTime.TotalSeconds - 10;
                    playerMovement();
                }
            }
            if (!input.PreviousKeyboardState.IsKeyDown(Keys.O) && input.CurrentKeyboardState.IsKeyDown(Keys.O))
            {
                Globals.player.kills++;
                Globals.player.ApplyBonuses(Globals.player.kills);
            }
            #endregion

            if (!paused)
            {
                playerMovement();
                UpdateAll();
            }               
        }

        */
    }
}
