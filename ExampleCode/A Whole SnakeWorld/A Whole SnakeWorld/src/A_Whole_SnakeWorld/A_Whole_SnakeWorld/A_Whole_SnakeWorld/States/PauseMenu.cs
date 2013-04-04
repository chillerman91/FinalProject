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
    /// Estado que muestra un menu cuando el juego esta en pausa
    /// </summary>
    public class PauseMenu : MenuState
    {

        public PauseMenu(Game game) : base(game) {

            MenuItem item;

            //Añadimos las posibles opciones del menu
            item = new MenuItem("Volver al Juego");
            item.Activate += UnJugadorActivated;
            menu.Items.Add(item);

            item = new MenuItem("Salir");
            item.Activate += QuitActivated;
            menu.Items.Add(item);

            controlActual = controlAnterior = GamePad.GetState(PlayerIndex.One);
            tecladoActual = tecladoAnterior = Keyboard.GetState();
        }


     
        #region Manejadores de Eventos:
        /// <summary>
        /// Acciones a realizar cuando se activa la opcion "Jugar"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void UnJugadorActivated(object sender, EventArgs args)
        {
           
            stateManager.estadoActual = Gameestados.PlayingState;
        }

        /// <summary>
        /// Acciones a realizar cuando se activa la opcion "Salir"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void QuitActivated(object sender, EventArgs args)
        {
            game.Exit();
        }

        #endregion

        /// <summary>
        /// Actualiza el estado
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
       public override void Update(GameTime gameTime)
        { base.Update(gameTime); }

  
    }
}
