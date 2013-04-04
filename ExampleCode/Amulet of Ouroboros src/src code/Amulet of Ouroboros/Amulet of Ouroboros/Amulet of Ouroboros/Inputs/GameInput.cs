using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Amulet_of_Ouroboros.Sprites;

namespace Amulet_of_Ouroboros.Inputs
{
    class GameInput
    {
        Dictionary<string, Input> inputs = new Dictionary<string, Input>();

        public MouseState CurrentMouseState;
        public MouseState PreviousMouseState;

        public GameInput()
        {
        }

        public Input MyInput(string theAction)
        {
            if (inputs.ContainsKey(theAction) == false)
            {
                inputs.Add(theAction, new Input());
            }

            return inputs[theAction];
        }

        public void BeginUpdate()
        {
            CurrentMouseState = Mouse.GetState();
            Input.BeginUpdate();
        }

        public void EndUpdate()
        {
            PreviousMouseState = CurrentMouseState;
            Input.EndUpdate();
        }

        public bool IsConnected(PlayerIndex thePlayer)
        {
            if (Input.GamepadConnectionState[thePlayer] == false)
            {
                return true;
            }

            return Input.IsConnected(thePlayer);
        }

        public bool IsPressed(string theAction,
                              Rectangle theCurrentObjectLocation)
        {
            if (!inputs.ContainsKey(theAction))
            {
                return false;
            }

            return inputs[theAction].IsPressed(PlayerIndex.One,
                                               theCurrentObjectLocation);
        }

        public bool IsPressed(string theAction)
        {
            if (!inputs.ContainsKey(theAction))
            {
                return false;
            }
            return inputs[theAction].IsPressed(PlayerIndex.One);
        }

        public bool IsPressed(string theAction, PlayerIndex thePlayer)
        {
            if (inputs.ContainsKey(theAction) == false)
            {
                return false;
            }

            return inputs[theAction].IsPressed(thePlayer);
        }

        public bool IsPressed(string theAction, PlayerIndex? thePlayer)
        {
            if (thePlayer == null)
            {
                PlayerIndex theReturnedControllingPlayer;
                return IsPressed(theAction,
                                 thePlayer,
                                 out theReturnedControllingPlayer);
            }

            return IsPressed(theAction, (PlayerIndex)thePlayer);
        }

        public bool IsPressed(string theAction,
                              PlayerIndex? thePlayer,
                              out PlayerIndex theControllingPlayer)
        {
            if (!inputs.ContainsKey(theAction))
            {
                theControllingPlayer = PlayerIndex.One;
                return false;
            }

            if (thePlayer == null)
            {
                if (IsPressed(theAction, PlayerIndex.One))
                {
                    theControllingPlayer = PlayerIndex.One;
                    return true;
                }

                if (IsPressed(theAction, PlayerIndex.Two))
                {
                    theControllingPlayer = PlayerIndex.Two;
                    return true;
                }

                if (IsPressed(theAction, PlayerIndex.Three))
                {
                    theControllingPlayer = PlayerIndex.Three;
                    return true;
                }

                if (IsPressed(theAction, PlayerIndex.Four))
                {
                    theControllingPlayer = PlayerIndex.Four;
                    return true;
                }

                theControllingPlayer = PlayerIndex.One;
                return false;
            }

            theControllingPlayer = (PlayerIndex)thePlayer;
            return IsPressed(theAction, (PlayerIndex)thePlayer);
        }

        public void AddGamePadInput(string theAction,
                                    Buttons theButton,
                                    bool isReleasedPreviously)
        {
            MyInput(theAction).AddGamepadInput(theButton,
                                               isReleasedPreviously);
        }

        public void AddKeyboardInput(string theAction,
                                     Keys theKey,
                                     bool isReleasedPreviously)
        {
            MyInput(theAction).AddKeyboardInput(theKey,
                                                isReleasedPreviously);
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
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
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
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
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
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }
}
