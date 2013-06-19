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
    class CreatingMap : GameWorldScreenBase
    {
        /// <summary>
        /// The Screen were the player is moving around and fighting stuff.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="batch"></param>
        /// <param name="changeScreen"></param>
        /// <param name="graphics"></param>
        /// <param name="Device"></param>
        public CreatingMap(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics, GraphicsDevice device)
            : base(game, batch, changeScreen, graphics, device)
        {
            thisState = ScreenState.Playing;
            Initialize();
            LoadContent();
        }

        protected override void AdjustCam()
        {
            Globals.map.ToLocation = Globals.map.ToLocation * .95f + new Vector2(0, 0) * (Globals.map.PosMod) * .05f;
            Globals.map.globalScale = Globals.map.globalScale * .99f + .3f * .01f;
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            base.Update(gameTime);
            if ((tick % 20 == 0 && input.CurrentMouseState.LeftButton == ButtonState.Pressed) || input.PreviousMouseState.LeftButton == ButtonState.Released)
            {
                if (Globals.map.CreateMap() == 4)
                {
                    StartGame();
                    changeScreenDelegate(ScreenState.Playing);
                }
            }
        }
    }
}
