
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Infinity_TD_Library;
#endregion

namespace Infinity_TD
{
    public partial class BaseGameState : GameState
    {
        protected Game1 OurGame;
        protected ContentManager Content;

        public BaseGameState(Game game)
            : base(game)
        {
            Content = game.Content;
            OurGame = (Game1)game;
        }
    }
}


