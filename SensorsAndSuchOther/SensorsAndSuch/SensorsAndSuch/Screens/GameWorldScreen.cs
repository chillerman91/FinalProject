using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using SensorsAndSuch.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.SamplesFramework;
using FarseerPhysics.Collision;
using FarseerPhysics.Common.Decomposition;
using Microsoft.Xna.Framework.Content;
using SensorsAndSuch.Mobs;
using SensorsAndSuch.Maps;
using SharpNeat.Genomes.Neat;
using System.Xml;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Input;
using SharpNeat.Utility;

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
    internal abstract class GameWorldScreenBase : Screen
    {

        internal enum WorldState
        {
            CreatingMap,
            Playing,
            AnlyseCreatures,
            Paused,
            MapEditing,
            Ghost,
            Dead
        }

        public List<SwitchData> switchToStates = new List<SwitchData>();
        internal WorldState currentState = WorldState.CreatingMap;
        internal ScreenState thisState;
        #region Properties
        protected static HUDPlayerInfo HUDPlayerInfo;
        protected static Player player;
        protected static World World;
        protected GraphicsDevice device;
        //private Texture2D materials;
        //private BasicEffect effect;
        protected bool playerPaused = false;
        protected bool shouldDraw = true;
        protected int tick = 0;
        protected static bool gameStart = false;

        private string xmlDocName = "Config\\worldConfig.xml"; 
        #endregion
        public GameWorldScreenBase(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics, GraphicsDevice device)
            : base(game, batch, changeScreen, graphics)
        {
            this.device = device;
        }

        protected abstract void AdjustCam();

        public void LoadContent()
        {
            // base.LoadContent();

            //We enable diagnostics to show get values for our performance counters.
            //Settings.EnableDiagnostics = true;
            if (World == null)
            {
                World = new World(Vector2.Zero);
            }
            else
            {
                World.Clear();
            }
            Globals.SetGeneral(content, device, World, this);
            Globals.AssetCreatorr.LoadContent(content);
            Globals.SetLevelSpecific(new MobManager(), new RandomMap());
        }


        /// <summary>
        /// Gets the Config XML and loads in its variables
        /// </summary>
        public bool Initialize()
        {          
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlDocName);
            XmlNode xmlNode = doc.SelectSingleNode("/Root");
            XmlNodeReader xmlReader = new XmlNodeReader(xmlNode);
            XmlNodeList xChildren = doc.DocumentElement.ChildNodes;
            XmlIoUtils.MoveToElement(xmlReader, false, "Root");

            RandomMap.InitializeXMLData(xmlReader);

            xmlReader.Close();
            return true;
        }

        /// <summary>
        /// Create a default Config XML file
        /// </summary>
        public bool CreateDefaultXMLFile()
        {      
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");
            settings.OmitXmlDeclaration = true;
            playerPaused = !playerPaused;
                // Create the XmlWriter object and write some content.
                
            XmlWriter xWriter = XmlWriter.Create(xmlDocName, settings);

            xWriter.WriteStartElement("Root");

            RandomMap.CreateDefaultXMLFile(xWriter);

            xWriter.Close();
            return true;
        }

        protected void Update(GameTime gameTime)
        {
            Globals.GameTime = gameTime;

            if (gameStart && !input.PreviousKeyboardState.IsKeyDown(Keys.P) && input.CurrentKeyboardState.IsKeyDown(Keys.P))
            {
                playerPaused = !playerPaused;
            }
            Globals.tick = tick;
            ++tick;
            AdjustCam();
            UpdateWorld(gameTime);
            foreach (SwitchData data in switchToStates)
            {
                if (data.shouldTranfer(input))
                {
                    changeScreenDelegate(data.to);
                }
            }
        }

        public void UpdateWorld(GameTime gameTime, bool otherScreenHasFocus = false, bool coveredByOtherScreen = false)
        {
            if (playerPaused) return;
            if (!coveredByOtherScreen && !otherScreenHasFocus)
            {
                // variable time step but never less then 30 Hz
                World.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);//Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, (1f / 30f)));
            }
            else
            {
                World.Step(0f);
            }
            //Camera.Update(gameTime);
            //base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        protected void StartGame()
        {
            playerPaused = true;
            tick = 0;
            player = new Player(content, Globals.map.GetRandomFreePos());
            Globals.player = player;
            Reaper.Player = player;

            HUDPlayerInfo = new HUDPlayerInfo(content, player);

            Globals.Mobs.InilitilizeGenuses();

            Globals.GamesStart = true;
            gameStart = true;
        }

        protected void UpdateAll()
        {
            Globals.Mobs.UpdateMobs(tick);
            Globals.map.UpdateTiles(tick);           
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
            currentState = WorldState.Dead;
        }

        public void SetDied()
        {
            Globals.map.setDull();
            Globals.Mobs.setReaperStart();
            currentState = WorldState.Ghost;
        }

        public void SetLiving()
        {
            Globals.map.setNotDull();
            currentState = WorldState.Playing;
        }

        protected override void Draw(SpriteBatch batch, DisplayOrientation screenOrientation)
        {
            if (shouldDraw)
            {
                Globals.map.Draw(batch);
                if (gameStart)
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
}
