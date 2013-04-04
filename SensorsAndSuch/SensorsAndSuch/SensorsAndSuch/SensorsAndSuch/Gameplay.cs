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
        int popAmount = 50;
        private ScreenState currentState = ScreenState.CreatingMap;
        int tick = 0;
        GraphicsDevice Device;
        protected bool GameStart = false;
        public Gameplay(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics, GraphicsDevice Device)
            : base(game, batch, changeScreen, graphics)
        {
            this.Device = Device;
        }

        protected override void LoadScreenContent(ContentManager content)
        {
            base.LoadContent(content);
            World.Gravity = new Vector2(0f, 0.1f);
            Globals.SetGeneral(content, Device, World);
            Globals.AssetCreatorr.LoadContent(content);
            Globals.SetLevelSpecific(new MobManager(), new RandomMap());
        }
        int GenNumb = 1;
        protected void StartGame()
        {
            tick = 0;
            MobManager.mobGroups = MobManager.MaxMonsters;
            player = new Player(content, Globals.map.GetRandomFreePos());
            Globals.player = player;
            GameStart = true;
            HUDPlayerInfo = new HUDPlayerInfo(content, player);
            for (int i = 0; i < popAmount; i++)
            {
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, gridPos: Globals.map.GetRandomFreePos(), age: Globals.rand.Next(50));
                Globals.Mobs.GetMobAt(i).SetPathway(i % MobManager.pathways);
            }
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            base.Update(gameTime);
            GameplayTurnCheck();
        }

        private void GameplayTurnCheck()
        {
            Globals.map.Update(input);
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
                    if (tick%1 == 0||input.CurrentMouseState.LeftButton == ButtonState.Pressed )//&& input.PreviousMouseState.LeftButton == ButtonState.Released)
                    {
                        if (Globals.map.CreateMap() == 4)
                        {
                            currentState = ScreenState.SettingPoints;
                            //StartGame();
                        }
                    }
                    return;
                }
            }
            else if (currentState == ScreenState.SettingPoints) 
            {
                Globals.map.ToLocation = Globals.map.ToLocation * .95f + new Vector2(0, 0) * (Globals.map.PosMod) * .05f;
                Globals.map.globalScale = Globals.map.globalScale * .99f + .25f * .01f;
                if (!input.PreviousKeyboardState.IsKeyDown(Keys.M) && input.CurrentKeyboardState.IsKeyDown(Keys.M) && Globals.Mobs.pathwayPts.Count == MobManager.pathways *2)
                {
                    StartGame();
                    currentState = ScreenState.Playing;
                }
                if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released && Globals.Mobs.pathwayPts.Count < MobManager.pathways * 2)
                {
                    Vector2 GridPos = Globals.map.GridFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                    if (Globals.map.isInBounds(GridPos) && Globals.Mobs.AddPathPt(GridPos))
                    {
                        List<BaseTile> column = Globals.map.GetTileColumn(GridPos);
                        if (Globals.map.GetTileColumn(GridPos).Count == 2)
                        {
                            Globals.map.CreateBlock(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                        }
                        if (Globals.Mobs.pathwayPts.Count % 2 == 1)
                        {
                            column[column.Count - 1].color = Color.Gold;
                        }
                        else
                        {
                            column[column.Count - 1].color = Color.OrangeRed;
                        }
                    }
                } 
                else if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released)
                {
                    Globals.map.CreateBlock(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                }
                if (input.CurrentMouseState.RightButton == ButtonState.Pressed)
                {
                    Vector2 pos = Globals.map.GridFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                    pos = new Vector2((int)pos.X, (int)pos.Y);
                    if (Globals.map.isFree(pos))
                        Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, pos);
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
                    Globals.map.CreateBlock(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                }
                if (input.CurrentMouseState.RightButton == ButtonState.Pressed)
                {
                    Vector2 pos = Globals.map.GridFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                    pos = new Vector2((int)pos.X, (int)pos.Y);
                    if(Globals.map.isFree(pos))
                        Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, pos);
                }
                UpdateAll();
            }
            else if (currentState == ScreenState.Playing)
            {
                Globals.map.globalScale = Globals.map.globalScale * .99f + .5f * .01f;
                Globals.map.ToLocation = Globals.map.ToLocation * .95f + new Vector2(((int)player.circle.Position.X) / Globals.map.PosMod, ((int)player.circle.Position.Y) / Globals.map.PosMod) * (Globals.map.PosMod) * .05f;

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
        int timesTurnedOver = 0;
        string outFile = "";
        private void UpdateAll() {
            int maxTickLoop = MobManager.MaxMonsters/MobManager.mobGroups + 1;

            //MobManager.mobGroups = 25;
            Globals.Mobs.UpdateMobs(tick % maxTickLoop);
            //Globals.map.Update();
            if (tick % 600 == Math.Min(20 * GenNumb + 50, 599))
            {
                if (timesTurnedOver == MobManager.pathways)
                {
                    GenNumb++;
                    outFile += "\n\nGneration  " + GenNumb + "\n";
                    tick = 0;
                    BaseMonster[] mon = Globals.Mobs.Monsters;
                    List<BaseMonster> SortedMob = new List<BaseMonster>();
                    for (int i = 0; i < Globals.Mobs.GetMobAmount(); i++)
                    {
                        mon[i].SetScore();
                        SortedMob.Add(mon[i]);
                    }
                    SortedMob.Sort();

                    int numbAdded = 0;
                    for (int i = 0; i < SortedMob.Count; i++)
                    {
                        if (numbAdded < popAmount)
                        {
                            int chilAmount = Math.Max((int)((50 - i * i) / 10), 1);
                            if (numbAdded + chilAmount <= popAmount)
                                SortedMob[i].MakeChildren(chilAmount);
                            else
                                SortedMob[i].MakeChildren(popAmount - numbAdded);
                            numbAdded += chilAmount;
                        }
                        outFile += "Agent " + i + "'s Fitness: " + SortedMob[i].Score + "\n";
                    }
                    if (SortedMob[0].Score > 29)
                    {
                        System.IO.File.WriteAllText("HW2.out", outFile);
                        return;
                    }
                    HUDPlayerInfo.GenInfo = "" + GenNumb + " " + SortedMob[0].Score + " " + SortedMob[1].Score + " " + SortedMob[popAmount - 1].Score;
                    for (int i = 0; i < popAmount; i++)
                    {
                        Globals.Mobs.KillMonster(0);
                    }

                    for (int i = 0; i < popAmount; i++)
                    {
                        Globals.Mobs.GetMobAt(i).SetPathway(i % MobManager.pathways);
                    }

                    timesTurnedOver = 0;
                }
                else
                {
                    tick = 0;
                    int mobAmount = Globals.Mobs.GetMobAmount();
                    for (int i = 0; i < mobAmount; i++)
                    {
                        Globals.Mobs.GetMobAt(i).ChangePathway();
                    }
                    timesTurnedOver++;
                }
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
