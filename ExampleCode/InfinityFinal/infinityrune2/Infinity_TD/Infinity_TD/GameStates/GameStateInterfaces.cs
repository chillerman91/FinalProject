using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Infinity_TD_Library;

namespace Infinity_TD
{

        public interface ITitleIntroState : IGameState { }
        public interface IStartMenuState : IGameState { }
        public interface IOptionsMenuState : IGameState { }
        public interface IPlayingState : IGameState { }
        public interface IPausedState : IGameState { }

}
