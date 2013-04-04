using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace A_Whole_SnakeWorld
{
    /// <summary>
    /// Gestor de estados. Se encarga de que toda la gestion de estados funcione bien
    /// </summary>
    public class StateManager : DrawableGameComponent
    {
      

        /// <summary>
        /// Diccionario de los estados del juego
        /// </summary>
        public Dictionary<Gameestados, State> estados;
        /// <summary>
        /// Estado actual del juego
        /// </summary>
        public Gameestados estadoActual;

        public StateManager(Game game, Gameestados inicial)
            : base(game)
        {
            estados = new Dictionary<Gameestados, State>();
            estadoActual = inicial;
            
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Actually activate the currentState
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            State tempState;
            if (estados.TryGetValue(estadoActual, out tempState))
            {
                tempState.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the active State.
        /// </summary>
        /// <param name="gameTime">The current time of the game</param>
        public override void Draw(GameTime gameTime)
        {
            State tempState;
            if (estados.TryGetValue(estadoActual, out tempState))
            {
                tempState.Draw(gameTime);
            }
        }
    
    }
}
