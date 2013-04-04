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

namespace ChickenCross
{
    /// <summary>
    /// Estado que muestra el menu principal del juego
    /// </summary>
    public class MainMenu : MenuState
    {
        /// <summary>
        /// Constructor del menu principal del juego
        /// </summary>
        /// <param name="game"></param>
        public MainMenu(Game game) : base(game) {

            MenuItem item;

            //Añadimos las posibles opciones del menu
            item = new MenuItem("Jugar");
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
              //Ahora vamos a cargar los recursos del estados. Como supuestamente va
              //a tardar un poco en cargar todo, pasamos al estado que muestra
               //la pantalla de "cargando"
               //content.Unload();
               (manager.estados[GameStates.Loading] as Loading).Initialize("Fonts/fuente", "Cargando.....", "Images/FondoLoading"); 
               (manager.estados[GameStates.PlayingState] as PlayingState).Initialize();
               (manager.estados[GameStates.PauseMenu] as PauseMenu).Initialize("Fonts/menu", "Images/FondoPausa");
               manager.estadoActual = GameStates.Loading;
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
