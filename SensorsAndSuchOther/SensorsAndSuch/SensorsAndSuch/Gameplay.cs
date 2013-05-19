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
using SensorsAndSuch.FrameWork;
using System.Collections.Generic;
using System.Xml;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SensorsAndSuch;
using FarseerPhysics.Dynamics;
//TODO: consolidate config setting to one loaction
//TODO: USE Viewport and not Draw funciton
//TODO: Only Draw things that are on screen 
//TODO: Is the sword reset thing working?
//TODO: Add More sensors (DONE 4/20/2013)
//Speiation
//Speed based on # pressed
//Deal with growing Globals file
 //Reaper
//remove stuttering
//add images
//Can't leave map while a ghost
namespace SensorsAndSuch.Screens
{
    class Gameplay : SensorScreen
    {
        HUDPlayerInfo HUDPlayerInfo;
        Player player;

        internal enum ScreenState
        {
            CreatingMap,
            SettingPoints,
            Playing,
            AnlyseCreatures,
            Paused,
            MapEditing, 
            Ghost,
            Dead
        }

        int popAmount;
        string XMLDocName = "data.xml";
        internal ScreenState currentState = ScreenState.CreatingMap;
        int tick = 0;
        GraphicsDevice Device;
        protected bool GameStart = false;
        Vector2 LastBlock = new Vector2(-1, -1);
        private XmlWriter xmlW;

        BadGuyEvolutionAlgorithm<NeatGenome> EvolutionAlgorithm;
        int GenNumb = 1;

        public Gameplay(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics, GraphicsDevice Device)
            : base(game, batch, changeScreen, graphics)
        {
            RandomMap.TicksInCreate = 200;
            popAmount = 50;
            BaseElemental.NumOfElementals = 7;
            this.Device = Device;
            BadGuy.TicksPerLife = 1500;
            MobManager.mobGroups = MobManager.MaxMonsters;
        }

        protected override void LoadScreenContent(ContentManager content)
        {
            base.LoadContent(content);
            World.Gravity = new Vector2(0f, 0f);
            Globals.SetGeneral(content, Device, World, this);
            Globals.AssetCreatorr.LoadContent(content);
            Globals.SetLevelSpecific(new MobManager(), new RandomMap());
        }

        protected void StartGame()
        {
            List<NeatGenome> StartingBrains;
            XmlNode xNode;
            XmlDocument doc = new XmlDocument();
            paused = true;
            tick = 0;
            player = new Player(content, Globals.map.GetRandomFreePos());
            Globals.player = player;
            Reaper.Player = player;

            GameStart = true;
            HUDPlayerInfo = new HUDPlayerInfo(content, player);

            //Creating Starting population

            EvolutionAlgorithm = Globals.NeatExp.CreateEvolutionAlgorithm(popAmount);
            try
            {
                doc.Load(XMLDocName);

                xNode = doc.GetElementsByTagName("Root")[0];
                StartingBrains = Globals.NeatExp.LoadPopulation(xNode, EvolutionAlgorithm.GenomeFactory);
                EvolutionAlgorithm.PostLoadFileInitilazation(StartingBrains);
            }
            catch(Exception E)
            {
                StartingBrains = EvolutionAlgorithm.GetUpdatedGeneration();
            }
            for (int i = 0; i < StartingBrains.Count; i++)
            {
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, gridPos: Globals.map.GetRandomFreePos(), genome: StartingBrains[i]);
            }
            Globals.GamesStart = true;
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            base.Update(gameTime);
            Globals.GameTime = gameTime;
            
            Globals.tick = tick;
            if (GameStart && !input.PreviousKeyboardState.IsKeyDown(Keys.P) && input.CurrentKeyboardState.IsKeyDown(Keys.P))
            {
                paused = !paused;
            }
            ++tick;
            if (!input.PreviousKeyboardState.IsKeyDown(Keys.F1) && input.CurrentKeyboardState.IsKeyDown(Keys.F1))
            {
                // Create an XmlWriterSettings object with the correct options. 
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = ("\t");
                settings.OmitXmlDeclaration = true;
                paused = !paused;
                // Create the XmlWriter object and write some content.
                xmlW = XmlWriter.Create(XMLDocName, settings);
                Globals.NeatExp.SavePopulation(xmlW, EvolutionAlgorithm.GenomeList/*Genome List of type IList<NeatGenome>*/);
            }

            #region State Handling
            if (currentState == ScreenState.CreatingMap) 
            { 

                if (!GameStart)
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
                if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released)
                {
                    Globals.map.ToggleBlockFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                }
                if (input.CurrentMouseState.RightButton == ButtonState.Pressed )
                {
                    if (LastBlock != Globals.map.GridFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y))
                    {
                        LastBlock = Globals.map.GridFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                        Globals.map.ToggleBlockFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                    }
                }

                Globals.map.UpdateText(input);
            }
            else if (currentState == ScreenState.Playing || currentState == ScreenState.Ghost)
            {
                //Globals.map.globalScale = Globals.map.globalScale * .99f + .5f * .01f;
                //Globals.map.ToLocation = Globals.map.ToLocation * .95f + new Vector2(((int)player.shape.Position.X) / Globals.map.PosMod, ((int)player.shape.Position.Y) / Globals.map.PosMod) * (Globals.map.PosMod) * .05f;

                Globals.map.globalScale = Globals.map.globalScale * .99f + .7f * .01f;
                Vector2 temp2 = Globals.map.getScreenSizeInPhysics();
                Globals.map.ToLocation = Globals.map.ToLocation * .99f + (player.shape.Position + player.Dir * 5 * Globals.map.globalScale - temp2 * .5f) * .01f;
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
                        Globals.player.SetRandPosSafe(0);
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

        protected void playerMovement()
        {
            bool pressed = false;
            if (player != null)
            {
                if (input.CurrentKeyboardState.IsKeyDown(Keys.A))
                {
                    player.TakeTurn(Player.MoveOpt.RIGHT);
                    pressed = true;
                }
                if (input.CurrentKeyboardState.IsKeyDown(Keys.D))
                {
                    player.TakeTurn(Player.MoveOpt.LEFT);
                    pressed = true;
                }
                if (input.CurrentKeyboardState.IsKeyDown(Keys.W))
                {
                    player.TakeTurn(Player.MoveOpt.UP);
                    pressed = true;
                }
                if (input.CurrentKeyboardState.IsKeyDown(Keys.S))
                {
                    player.TakeTurn(Player.MoveOpt.DOWN);
                    pressed = true;
                }
                if (input.CurrentKeyboardState.IsKeyDown(Keys.Space))
                {
                    player.TakeTurn(Player.MoveOpt.USEITEM);
                    pressed = true;
                }
                if (!input.PreviousKeyboardState.IsKeyDown(Keys.R) && input.CurrentKeyboardState.IsKeyDown(Keys.R))
                {
                    player.Suicide();
                    if (Globals.Debugging)
                        Globals.Mobs.WarpCreatures(tick);

                    pressed = true;
                }
                if (!pressed)
                {
                    player.TakeTurn(Player.MoveOpt.NONE);
                }
            }

        }

        public void GameEnd()
        {
            Globals.map.setDead();
            currentState = ScreenState.Dead;
        }

        public void SetDied()
        {
           Globals.map.setDull();
           Globals.Mobs.setReaperStart();
           currentState = ScreenState.Ghost;
        }

        public void SetLiving()
        {
            Globals.map.setNotDull();
            currentState = ScreenState.Playing;
        }

        private void UpdateAll() {
            if (tick % (BadGuy.TicksPerLife + 1) == BadGuy.TicksPerLife)
            {
                BaseMonster[] mon = Globals.Mobs.Monsters;
                int i = 0;
                while (mon[i] != null)
                {
                    mon[i].ScoreSelf();
                    i++;
                }
                EvolutionAlgorithm.EvaluateGeneration();
                List<NeatGenome> StartingBrains = EvolutionAlgorithm.GetUpdatedGeneration();
                for (i = 0; i < StartingBrains.Count; i++)
                {
                    mon[i].ResetGenome(StartingBrains[i]);
                }
                tick = 0;
                GenNumb++;
                if (Globals.Debugging)
                    Globals.Mobs.WarpCreatures(tick);
                HUDPlayerInfo.GenInfo = "Gen. Numb " + GenNumb;
            }
            else 
            {
                int maxTickLoop = MobManager.MaxMonsters / MobManager.mobGroups + 1;

                //MobManager.mobGroups = 25;
                Globals.Mobs.UpdateMobs(tick % maxTickLoop);
                Globals.map.UpdateTiles(tick);
                //Globals.map.Update();
            }
        }

        protected override void DrawScreen(SpriteBatch batch, DisplayOrientation screenOrientation)
        {
            Globals.map.Draw(batch);
            if (GameStart)
            {
                Globals.Mobs.Draw(batch);
                if (player != null)
                    player.Draw(batch);
                if (HUDPlayerInfo != null)
                    HUDPlayerInfo.Draw(batch);
            }
        }
    }
}
