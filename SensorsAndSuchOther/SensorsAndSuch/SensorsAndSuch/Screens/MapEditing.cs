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
    class MapEditing : GameWorldScreenBase
    {
        Vector2 LastBlock = new Vector2(-1, -1);
        /// <summary>
        /// The Screen were the player is moving around and fighting stuff.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="batch"></param>
        /// <param name="changeScreen"></param>
        /// <param name="graphics"></param>
        /// <param name="Device"></param>
        public MapEditing(Game game, SpriteBatch batch, ChangeScreen changeScreen, GraphicsDeviceManager graphics, GraphicsDevice device)
            : base(game, batch, changeScreen, graphics, device)
        {
            thisState = ScreenState.Playing;
            switchToStates.Add(new SwitchData(from: thisState, key: Keys.M, canTranfer: () => true, to: ScreenState.Playing));
            switchToStates.Add(new SwitchData(from: thisState, key: Keys.C, canTranfer: () => true, to: ScreenState.AnlyseCreatures));
        }

        protected override void AdjustCam()
        {
            Globals.map.ToLocation = Globals.map.ToLocation * .95f + new Vector2(0, 0) * (Globals.map.PosMod) * .05f;
            Globals.map.globalScale = Globals.map.globalScale * .99f + .3f * .01f;
        }

        protected override void UpdateScreen(GameTime gameTime, DisplayOrientation displayOrientation)
        {
            base.Update(gameTime);
            if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released)
            {
                Globals.map.ToggleBlockFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y);
            }
            if (input.CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                if (LastBlock != Globals.map.GridFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y))
                {
                    LastBlock = Globals.map.GridFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                    Globals.map.ToggleBlockFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y);
                }
            }

            Globals.map.UpdateText(input);
        }
    }
}
