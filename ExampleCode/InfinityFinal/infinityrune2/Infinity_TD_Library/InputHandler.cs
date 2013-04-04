
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Infinity_TD_Library
{
    public interface IInputHandler
    {
        bool WasPressed(int playerIndex, InputHandler.ButtonType button, Keys keys);

        KeyboardHandler KeyboardState { get; }

        GamePadState[] GamePads { get; }
        ButtonHandler ButtonHandler { get; }

#if !XBOX360
        MouseState MouseState { get; }
        MouseState PreviousMouseState { get; }
#endif
    };

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public partial class InputHandler
        : Microsoft.Xna.Framework.GameComponent, IInputHandler
    {
        public enum ButtonType { A, B, Back, LeftShoulder, LeftStick, RightShoulder, RightStick, Start, X, Y, DPadUp, DPadDown }

        private KeyboardHandler keyboard;
        private ButtonHandler gamePadHandler = new ButtonHandler();
        private GamePadState[] gamePads = new GamePadState[4];

#if !XBOX360
        private MouseState mouseState;
        private MouseState prevMouseState;
#endif

        private bool allowsExiting;

        public InputHandler(Game game) : this(game, false) { }
        public InputHandler(Game game, bool allowsExiting)
            : base(game)
        {
            this.allowsExiting = allowsExiting;

            // TODO: Construct any child components here
            game.Services.AddService(typeof(IInputHandler), this);


            //initialize our member fields
            keyboard = new KeyboardHandler();

#if !XBOX360
            Game.IsMouseVisible = true;
            prevMouseState = Mouse.GetState();
#endif
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            keyboard.Update();

            gamePadHandler.Update();

            if (allowsExiting)
            {
                if (keyboard.IsKeyDown(Keys.Escape))
                    Game.Exit();

                // Allows the default game to exit on Xbox 360 and Windows
                if (gamePadHandler.WasButtonPressed(0, ButtonType.Back))
                    Game.Exit();
            }

#if !XBOX360
            //Set our previous state
            prevMouseState = mouseState;
            //Get our new state
            mouseState = Mouse.GetState();
#endif

            gamePads[0] = GamePad.GetState(PlayerIndex.One);
            gamePads[1] = GamePad.GetState(PlayerIndex.Two);
            gamePads[2] = GamePad.GetState(PlayerIndex.Three);
            gamePads[3] = GamePad.GetState(PlayerIndex.Four);

            base.Update(gameTime);
        }


        #region IInputHandler Members
        public bool WasPressed(int playerIndex, ButtonType button, Keys keys)
        {
            if (keyboard.WasKeyPressed(keys) || gamePadHandler.WasButtonPressed(playerIndex, button))
                return (true);
            else
                return (false);
        }

        public KeyboardHandler KeyboardState
        {
            get { return (keyboard); }
        }

        public ButtonHandler ButtonHandler
        {
            get { return (gamePadHandler); }
        }

        public GamePadState[] GamePads
        {
            get { return(gamePads); }
        }

#if !XBOX360
        public MouseState MouseState
        {
            get { return(mouseState); }
        }

        public MouseState PreviousMouseState
        {
            get { return(prevMouseState); }
        }
#endif
        #endregion
    }

    public class ButtonHandler //: IButtonHandler
    {
        private GamePadState[] prevGamePadsState = new GamePadState[4];
        private GamePadState[] gamePadsState = new GamePadState[4];

        public ButtonHandler()
        {
            prevGamePadsState[0] = GamePad.GetState(PlayerIndex.One);
            prevGamePadsState[1] = GamePad.GetState(PlayerIndex.Two);
            prevGamePadsState[2] = GamePad.GetState(PlayerIndex.Three);
            prevGamePadsState[3] = GamePad.GetState(PlayerIndex.Four);
        }

        public void Update()
        {
            //set our previous state to our new state
            prevGamePadsState[0] = gamePadsState[0];
            prevGamePadsState[1] = gamePadsState[1];
            prevGamePadsState[2] = gamePadsState[2];
            prevGamePadsState[3] = gamePadsState[3];

            //get our new state
            //gamePadsState = GamePad.State .GetState();
            gamePadsState[0] = GamePad.GetState(PlayerIndex.One);
            gamePadsState[1] = GamePad.GetState(PlayerIndex.Two);
            gamePadsState[2] = GamePad.GetState(PlayerIndex.Three);
            gamePadsState[3] = GamePad.GetState(PlayerIndex.Four);
        }        

        public bool WasButtonPressed(int playerIndex, InputHandler.ButtonType button)
        {

            int pi = playerIndex;
            switch(button)
            {
                case InputHandler.ButtonType.A:
                    {
                        return (gamePadsState[pi].Buttons.A == ButtonState.Pressed &&
                            prevGamePadsState[pi].Buttons.A == ButtonState.Released);
                    }
                case InputHandler.ButtonType.B:
                    {
                        return (gamePadsState[pi].Buttons.B == ButtonState.Pressed &&
                            prevGamePadsState[pi].Buttons.B == ButtonState.Released);
                    }
                case InputHandler.ButtonType.Back:
                    {
                        return (gamePadsState[pi].Buttons.Back == ButtonState.Pressed &&
                            prevGamePadsState[pi].Buttons.Back == ButtonState.Released);
                    }
                case InputHandler.ButtonType.LeftShoulder:
                    {
                        return (gamePadsState[pi].Buttons.LeftShoulder == ButtonState.Pressed &&
                            prevGamePadsState[pi].Buttons.LeftShoulder == ButtonState.Released);
                    }
                case InputHandler.ButtonType.LeftStick:
                    {
                        return (gamePadsState[pi].Buttons.LeftStick == ButtonState.Pressed &&
                            prevGamePadsState[pi].Buttons.LeftStick == ButtonState.Released);
                    }
                case InputHandler.ButtonType.RightShoulder:
                    {
                        return (gamePadsState[pi].Buttons.RightShoulder == ButtonState.Pressed &&
                            prevGamePadsState[pi].Buttons.RightShoulder == ButtonState.Released);
                    }
                case InputHandler.ButtonType.RightStick:
                    {
                        return (gamePadsState[pi].Buttons.RightStick == ButtonState.Pressed &&
                            prevGamePadsState[pi].Buttons.RightStick == ButtonState.Released);
                    }
                case InputHandler.ButtonType.Start:
                    {
                        return (gamePadsState[pi].Buttons.Start == ButtonState.Pressed &&
                            prevGamePadsState[pi].Buttons.Start == ButtonState.Released);
                    }
                case InputHandler.ButtonType.X:
                    {
                        return (gamePadsState[pi].Buttons.X == ButtonState.Pressed &&
                            prevGamePadsState[pi].Buttons.X == ButtonState.Released);
                    }
                case InputHandler.ButtonType.Y:
                    {
                        return (gamePadsState[pi].Buttons.Y == ButtonState.Pressed &&
                            prevGamePadsState[pi].Buttons.Y == ButtonState.Released);
                    }
                case InputHandler.ButtonType.DPadUp:
                    {
                        return (gamePadsState[pi].DPad.Up == ButtonState.Pressed &&
                            prevGamePadsState[pi].DPad.Up == ButtonState.Released);
                    }

                case InputHandler.ButtonType.DPadDown:
                    {
                        return (gamePadsState[pi].DPad.Down == ButtonState.Pressed &&
                            prevGamePadsState[pi].DPad.Down == ButtonState.Released);
                    }

                default:
                    throw (new ArgumentException());
            }
        }
    }

    public class KeyboardHandler //: IKeyboardHandler
    {
        private KeyboardState prevKeyboardState;
        private KeyboardState keyboardState;

        public KeyboardHandler()
        {
            prevKeyboardState = Keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return (keyboardState.IsKeyDown(key));
        }

        public bool IsHoldingKey(Keys key)
        {
            return(keyboardState.IsKeyDown(key) && prevKeyboardState.IsKeyDown(key));
        }

        public bool WasKeyPressed(Keys key)
        {
            return (keyboardState.IsKeyDown(key) && prevKeyboardState.IsKeyUp(key));
        }

        public bool HasReleasedKey(Keys key)
        {
            return (keyboardState.IsKeyUp(key) && prevKeyboardState.IsKeyDown(key));
        }

        public void Update()
        {
            //set our previous state to our new state
            prevKeyboardState = keyboardState;

            //get our new state
            keyboardState = Keyboard.GetState();
        }
    }
}


