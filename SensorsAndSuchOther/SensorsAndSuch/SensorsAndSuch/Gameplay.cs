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
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SensorsAndSuch;
//TODO: Right CLicking does something 
//TODO: consolidate config setting to one loaction
//TODO: USE Viewport and not Draw funciton
//TODO: Only Dras things on screen

namespace SensorsAndSuch.Screens
{
    class Gameplay : SensorScreen
    {
        HUDPlayerInfo HUDPlayerInfo;
        Player player;

        private enum ScreenState
        {
            CreatingMap,
            SettingPoints,
            Playing,
            Paused,
            Map
        }

        int popAmount = 20;
        private ScreenState currentState = ScreenState.CreatingMap;
        int tick = 0;
        GraphicsDevice Device;
        protected bool GameStart = false;

        int timesTurnedOver = 0;
        string outFile = "";
        NeatEvolutionAlgorithm<NeatGenome> EvolutionAlgorithm;
        int GenNumb = 1;

        public Gameplay(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics, GraphicsDevice Device)
            : base(game, batch, changeScreen, graphics)
        {
            RandomMap.TicksInCreate = 350;
            this.Device = Device;
        }

        protected override void LoadScreenContent(ContentManager content)
        {
            base.LoadContent(content);
            World.Gravity = new Vector2(0f, 0f);
            Globals.SetGeneral(content, Device, World);
            Globals.AssetCreatorr.LoadContent(content);
            Globals.SetLevelSpecific(new MobManager(), new RandomMap());
        }

        protected void StartGame()
        {
            tick = 0;
            MobManager.mobGroups = MobManager.MaxMonsters;
            player = new Player(content, Globals.map.GetRandomFreePos());
            Globals.player = player;
            GameStart = true;
            HUDPlayerInfo = new HUDPlayerInfo(content, player);

            //Creating Starting population
            EvolutionAlgorithm = Globals.NeatExp.CreateEvolutionAlgorithm(popAmount);
            List<NeatGenome> StartingBrains = EvolutionAlgorithm.GetUpdatedGeneration();
            for (int i = 0; i < StartingBrains.Count; i++)
            {
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, gridPos: Globals.map.GetRandomFreePos(), genome: StartingBrains[i]);
                Globals.Mobs.GetMobAt(i).SetPathway(i % MobManager.pathways);
            }
            Globals.GamesStart = true;
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            base.Update(gameTime);
            GameplayTurnCheck();
        }

        private void GameplayTurnCheck()
        {
            Globals.map.Update(input, tick);
            if (!input.PreviousKeyboardState.IsKeyDown(Keys.P) && input.CurrentKeyboardState.IsKeyDown(Keys.P))
            {
                paused = !paused;
            }
            if (paused)
            {
                return;
            } 
            ++tick;
            if (currentState == ScreenState.CreatingMap) 
            { 

                if (!GameStart)
                {
                    if (tick%20 == 0||input.CurrentMouseState.LeftButton == ButtonState.Pressed )//&& input.PreviousMouseState.LeftButton == ButtonState.Released)
                    {
                        if (Globals.map.CreateMap() == 4)
                        {
                            currentState = ScreenState.Map;
                            StartGame();
                        }
                    }
                    return;
                }
            }

            else if (currentState == ScreenState.Map)
            {
                Globals.map.ToLocation = Globals.map.ToLocation * .95f + new Vector2(0, 0) * (Globals.map.PosMod) * .05f;
                Globals.map.globalScale = Globals.map.globalScale * .99f + .3f * .01f;
                if (!input.PreviousKeyboardState.IsKeyDown(Keys.M) && input.CurrentKeyboardState.IsKeyDown(Keys.M))
                {
                    currentState = ScreenState.Playing;
                }
                if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released)
                {
                    Globals.map.CreateBlockFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                }
                if (input.CurrentMouseState.RightButton == ButtonState.Pressed)
                {
                    Vector2 pos = Globals.map.GridFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                    pos = new Vector2((int)pos.X, (int)pos.Y);
                    if (Globals.map.isFree(pos)) { }
                        //Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, pos);
                }
                UpdateAll();
            }
            else if (currentState == ScreenState.Playing)
            {
                Globals.map.globalScale = Globals.map.globalScale * .99f + .5f * .01f;
                Globals.map.ToLocation = Globals.map.ToLocation * .95f + new Vector2(((int)player.shape.Position.X) / Globals.map.PosMod, ((int)player.shape.Position.Y) / Globals.map.PosMod) * (Globals.map.PosMod) * .05f;

                UpdateAll();

                if (!input.PreviousKeyboardState.IsKeyDown(Keys.M) && input.CurrentKeyboardState.IsKeyDown(Keys.M))
                {
                    currentState = ScreenState.Map;
                }
                bool pressed = false;
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
                    player.Warp();
                    pressed = true;
                }
                if (!pressed)
                {
                    player.TakeTurn(Player.MoveOpt.NONE);
                }
            }
        }

        private void UpdateAll() {
            int maxTickLoop = MobManager.MaxMonsters/MobManager.mobGroups + 1;

            //MobManager.mobGroups = 25;
            Globals.Mobs.UpdateMobs(tick % maxTickLoop);
            //Globals.map.Update();
            if (tick % 1500 == 1499)
            {
                BaseMonster[] mon = Globals.Mobs.Monsters;
                int i = 0;
                while (mon[i] != null) {
                    mon[i].SetScore();
                }
                EvolutionAlgorithm.EvaluateGeneration();
                List<NeatGenome> StartingBrains = EvolutionAlgorithm.GetUpdatedGeneration();
                for (i = 0; i < StartingBrains.Count; i++)
                {
                    mon[i].ResetGenome(StartingBrains[i]);
                }
                tick = 0;
            }

        }

        protected override void DrawScreen(SpriteBatch batch, DisplayOrientation screenOrientation)
        {
            Globals.map.Draw(batch);
            if (GameStart)
            {
                Globals.Mobs.Draw(batch);
                player.Draw(batch);
                HUDPlayerInfo.Draw(batch);
            }
        }
    }
}
