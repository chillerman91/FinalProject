using System;
using Microsoft.Xna.Framework;
namespace Infinity_TD_Library
{
    public interface IGameState
    {
        GameState Value { get; }
    }

    public interface IGameStateManager
    {
        event EventHandler OnStateChange; 
        GameState State { get; }
        void PopState();
        void PushState(GameState state);
        bool ContainsState(GameState state);
        void ChangeState(GameState newState);
    }
}
