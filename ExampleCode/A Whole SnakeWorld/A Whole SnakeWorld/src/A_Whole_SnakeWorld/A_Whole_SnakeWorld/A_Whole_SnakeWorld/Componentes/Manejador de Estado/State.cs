using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Clase abstracta. Todos los estados deben derivar de esta clase
    /// </summary>
    public abstract class State
    {
        /// <summary>
        /// Manejador de estados
        /// </summary>
        public StateManager stateManager;       
        /// <summary>
        /// Manejador para los recursos del juego
        /// </summary>
        public ContentManager content;
        /// <summary>
        /// Manejador  de los recursos graficos
        /// </summary>
        public  GraphicsDeviceManager graphics; 
        /// <summary>
        /// Juego
        /// </summary>
        public Game game;      
        

        /// <summary>
        /// Constructor del estado de juego
        /// </summary>
        /// <param name="game">Juego</param>
        public State(Game game)
        {
            this.game = game;
            stateManager = game.Services.GetService(typeof(StateManager)) as StateManager;
            content = game.Services.GetService(typeof(ContentManager)) as ContentManager;
            graphics = game.Services.GetService(typeof(IGraphicsDeviceManager)) as GraphicsDeviceManager;

        }

      

        /// <summary>
        /// Actualiza el estado
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Dibuja el estado
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Draw(GameTime gameTime);

    }
}
