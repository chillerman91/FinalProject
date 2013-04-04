using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Amulet_of_Ouroboros.Sprites;

namespace Amulet_of_Ouroboros.Inputs
{
    class Input
    {
        Dictionary<Keys, bool> keyboardDefinedInputs = new Dictionary<Keys, bool>();
        Dictionary<Buttons, bool> gamepadDefinedInputs = new Dictionary<Buttons, bool>();
        
        static public Dictionary<PlayerIndex, GamePadState> CurrentGamePadState = new Dictionary<PlayerIndex, GamePadState>();
        static public Dictionary<PlayerIndex, GamePadState> PreviousGamePadState = new Dictionary<PlayerIndex, GamePadState>();
        static public KeyboardState CurrentKeyboardState;
        static public KeyboardState PreviousKeyboardState;
        static public Dictionary<PlayerIndex, bool> GamepadConnectionState = new Dictionary<PlayerIndex, bool>();


        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public Input()
        {
            if (CurrentGamePadState.Count == 0)
            {
                CurrentGamePadState.Add(PlayerIndex.One, GamePad.GetState(PlayerIndex.One));
                CurrentGamePadState.Add(PlayerIndex.Two, GamePad.GetState(PlayerIndex.Two));
                CurrentGamePadState.Add(PlayerIndex.Three, GamePad.GetState(PlayerIndex.Three));
                CurrentGamePadState.Add(PlayerIndex.Four, GamePad.GetState(PlayerIndex.Four));

                PreviousGamePadState.Add(PlayerIndex.One, GamePad.GetState(PlayerIndex.One));
                PreviousGamePadState.Add(PlayerIndex.Two, GamePad.GetState(PlayerIndex.Two));
                PreviousGamePadState.Add(PlayerIndex.Three, GamePad.GetState(PlayerIndex.Three));
                PreviousGamePadState.Add(PlayerIndex.Four, GamePad.GetState(PlayerIndex.Four));

                GamepadConnectionState.Add(PlayerIndex.One, CurrentGamePadState[PlayerIndex.One].IsConnected);
                GamepadConnectionState.Add(PlayerIndex.Two, CurrentGamePadState[PlayerIndex.Two].IsConnected);
                GamepadConnectionState.Add(PlayerIndex.Three, CurrentGamePadState[PlayerIndex.Three].IsConnected);
                GamepadConnectionState.Add(PlayerIndex.Four, CurrentGamePadState[PlayerIndex.Four].IsConnected);
            }
        }

        static public void BeginUpdate()
        {
            CurrentGamePadState[PlayerIndex.One] = GamePad.GetState(PlayerIndex.One);
            CurrentGamePadState[PlayerIndex.Two] = GamePad.GetState(PlayerIndex.Two);
            CurrentGamePadState[PlayerIndex.Three] = GamePad.GetState(PlayerIndex.Three);
            CurrentGamePadState[PlayerIndex.Four] = GamePad.GetState(PlayerIndex.Four);
            CurrentKeyboardState = Keyboard.GetState(PlayerIndex.One);
        }

        static public void EndUpdate()
        {
            PreviousGamePadState[PlayerIndex.One] = CurrentGamePadState[PlayerIndex.One];
            PreviousGamePadState[PlayerIndex.Two] = CurrentGamePadState[PlayerIndex.Two];
            PreviousGamePadState[PlayerIndex.Three] = CurrentGamePadState[PlayerIndex.Three];
            PreviousGamePadState[PlayerIndex.Four] = CurrentGamePadState[PlayerIndex.Four];
            PreviousKeyboardState = CurrentKeyboardState;
        }

        public void AddKeyboardInput(Keys theKey, bool isReleasedPreviously)
        {
            if (keyboardDefinedInputs.ContainsKey(theKey))
            {
                keyboardDefinedInputs[theKey] = isReleasedPreviously;
                return;
            }
            keyboardDefinedInputs.Add(theKey, isReleasedPreviously);
        }

        public void AddGamepadInput(Buttons theButton, bool isReleasedPreviously)
        {
            if (gamepadDefinedInputs.ContainsKey(theButton))
            {
                gamepadDefinedInputs[theButton] = isReleasedPreviously;
                return;
            }
            gamepadDefinedInputs.Add(theButton, isReleasedPreviously);
        }

        static public bool IsConnected(PlayerIndex thePlayerIndex)
        {
            return CurrentGamePadState[thePlayerIndex].IsConnected;
        }

        public bool IsPressed(PlayerIndex thePlayerIndex)
        {
            return IsPressed(thePlayerIndex, null);
        }

        public bool IsPressed(PlayerIndex thePlayerIndex, Rectangle? theCurrentObjectLocation)
        {
            if (IsKeyboardInputPressed())
            {
                return true;
            }

            if (IsGamepadInputPressed(thePlayerIndex))
            {
                return true;
            }

            return false;
        }

        private bool IsKeyboardInputPressed()
        {
            foreach (Keys aKey in keyboardDefinedInputs.Keys)
            {
                if (keyboardDefinedInputs[aKey] && CurrentKeyboardState.IsKeyDown(aKey) && !PreviousKeyboardState.IsKeyDown(aKey))
                {
                    return true;
                }
                else if (!keyboardDefinedInputs[aKey] && CurrentKeyboardState.IsKeyDown(aKey))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsGamepadInputPressed(PlayerIndex thePlayerIndex)
        {
            foreach (Buttons aButton in gamepadDefinedInputs.Keys)
            {
                if (gamepadDefinedInputs[aButton] && CurrentGamePadState[thePlayerIndex].IsButtonDown(aButton) && !PreviousGamePadState[thePlayerIndex].IsButtonDown(aButton))
                {
                    return true;
                }
                else if (!gamepadDefinedInputs[aButton] && CurrentGamePadState[thePlayerIndex].IsButtonDown(aButton))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
