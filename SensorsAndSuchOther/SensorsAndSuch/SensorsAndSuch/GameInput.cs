using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;

namespace SensorsAndSuch.Inputs
{
    internal class GameInput
    {
        public MouseState CurrentMouseState;
        public MouseState PreviousMouseState;
        Dictionary<Keys, bool> keyboardDefinedInputs = new Dictionary<Keys, bool>();
        public KeyboardState CurrentKeyboardState;
        public KeyboardState PreviousKeyboardState;

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
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

        private bool IsKeyboardInputPressed()
        {
            foreach (Keys aKey in keyboardDefinedInputs.Keys)
            {
                if ((keyboardDefinedInputs[aKey] && CurrentKeyboardState.IsKeyDown(aKey) && !PreviousKeyboardState.IsKeyDown(aKey))
                    || (!keyboardDefinedInputs[aKey] && CurrentKeyboardState.IsKeyDown(aKey)))
                {
                    return true;
                }
            }
            return false;
        }

        public void BeginUpdate()
        {
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();
            CurrentKeyboardState = Keyboard.GetState(PlayerIndex.One);
        }

        public void EndUpdate()
        {
            PreviousKeyboardState = CurrentKeyboardState;
        }

        public bool CheckMousePress(Button button)
        {
            if (CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (CurrentMouseState.X <= button.Position.X + button.Width &&
                    CurrentMouseState.X >= button.Position.X &&
                    CurrentMouseState.Y <= button.Position.Y + button.Height &&
                    CurrentMouseState.Y >= button.Position.Y)
                {
                    return true;
                }
            }
            return false;
        }
        
        public bool CheckMouseOver(Vector2 pos, int width, int height)
        {

            if (CurrentMouseState.X <= pos.X + width &&
                CurrentMouseState.X >= pos.X &&
                CurrentMouseState.Y <= pos.Y + height &&
                CurrentMouseState.Y >= pos.Y)
            {
                return true;
            }
            return false;
        }

        public bool CheckMousePress(Rectangle rect)
        {
            if (CurrentMouseState.LeftButton == ButtonState.Pressed)
            {
                if (CurrentMouseState.X <= rect.X + rect.Width &&
                    CurrentMouseState.X >= rect.X &&
                    CurrentMouseState.Y <= rect.Y + rect.Height &&
                    CurrentMouseState.Y >= rect.Y)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckMouseRelease(Button button)
        {
            if (CurrentMouseState.LeftButton == ButtonState.Released)
            {
                if (CurrentMouseState.X <= button.Position.X + button.Width &&
                    CurrentMouseState.X >= button.Position.X &&
                    CurrentMouseState.Y <= button.Position.Y + button.Height &&
                    CurrentMouseState.Y >= button.Position.Y)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
