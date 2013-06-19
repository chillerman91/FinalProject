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
    class AnalyzeCreatures : GameWorldScreenBase
    {
        /// <summary>
        /// The Screen were the player is moving around and fighting stuff.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="batch"></param>
        /// <param name="changeScreen"></param>
        /// <param name="graphics"></param>
        /// <param name="Device"></param>
        public AnalyzeCreatures(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics, GraphicsDevice device)
            : base(game, batch, changeScreen, graphics, device)
        {
            thisState = ScreenState.Playing;
            switchToStates.Add(new SwitchData(from: thisState, key: Keys.M, canTranfer: () => player.CanUseMap, to: ScreenState.MapEditing));
            switchToStates.Add(new SwitchData(from: thisState, key: Keys.C, canTranfer: () => true, to: ScreenState.Playing));
        }

        protected override void AdjustCam()
        {
            Globals.map.globalScale = Globals.map.globalScale * .99f + .5f * .01f;
            int ScreenposBasedOnCenterX = input.CurrentMouseState.X - Screen.ScreenWidth / 2;
            float broughtToOneX = (float)ScreenposBasedOnCenterX / (Screen.ScreenWidth / 2);
            float finalX = (float)(Math.Pow(broughtToOneX, 5) * 10);

            int ScreenposBasedOnCenterY = input.CurrentMouseState.Y - Screen.ScreenHeight / 2;
            float broughtToOneY = (float)ScreenposBasedOnCenterY / (Screen.ScreenHeight / 2);
            float finalY = (float)(Math.Pow(broughtToOneY, 9) * 7);

            Vector2 ToLocation = new Vector2(finalX, finalY);

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
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            base.Update(gameTime);
        }
    }
}
