using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using SensorsAndSuch.Mobs;
using SensorsAndSuch.Maps;
using Microsoft.Xna.Framework.Content;
using SensorsAndSuch.Sprites;
using FarseerPhysics.Dynamics;
using FarseerPhysics.SamplesFramework;
using SensorsAndSuch.NEAT;
using System.Xml;
using SensorsAndSuch.Screens;

namespace SensorsAndSuch
{
    static class Globals
    {
        public static Random rand = new Random();
        public static MobManager Mobs;
        public static RandomMap map;
        public static ContentManager content;
        public static GraphicsDevice Device;
        public static Player player;
        public static World World;
        public static bool GamesStart = false;
        public static AssetCreator AssetCreatorr { get; set; }
        public static NeatExp NeatExp { get; set; }
        public static ScreenManager ScreenManager { get; set; }
        public static int tick = 0;
        public static Gameplay GamplayScreen;
        public static bool Debugging = true;
        public static GameTime GameTime;
        public static void SetGeneral(ContentManager content, GraphicsDevice device, World World, Gameplay GamplayScreen)
        {
            Globals.content = content;
            Globals.Device = device;
            Globals.GamplayScreen = GamplayScreen;
            AssetCreatorr = new AssetCreator(device);
            Globals.World = World;
            Globals.NeatExp = new NeatExp();
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load("tictactoe.config.xml");
            Globals.NeatExp.Initialize("TicTacToe", xmlConfig.DocumentElement);
        }

        public static void SetLevelSpecific(MobManager mobs, RandomMap map)
        {
            Globals.Mobs = mobs;
            Globals.map = map;
        }
    }
}
