using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using SensorsAndSuch.Mobs;
using SensorsAndSuch.Maps;
using Microsoft.Xna.Framework.Content;
using SensorsAndSuch.Sprites;
using FarseerPhysics.Dynamics;
using FarseerPhysics.SamplesFramework;

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
        
        public static AssetCreator AssetCreatorr { get; set; }
        public static ScreenManager ScreenManager { get; set; }
        public static void SetGeneral(ContentManager content, GraphicsDevice device, World World)
        {
            Globals.content = content;
            Globals.Device = device;
            AssetCreatorr = new AssetCreator(device);
            Globals.World = World;
        }

        public static void SetLevelSpecific(MobManager mobs, RandomMap map)
        {
            Globals.Mobs = mobs;
            Globals.map = map;
        }
    }
}
