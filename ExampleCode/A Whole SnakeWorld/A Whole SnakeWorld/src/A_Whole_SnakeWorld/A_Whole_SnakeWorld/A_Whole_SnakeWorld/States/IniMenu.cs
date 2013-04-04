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
    /// Clase que muestra un menu cuando arranca el juego
    /// </summary>
    public class IniMenu :  State
    {
        /// <summary>
        /// Spritebatch principal
        /// </summary>
        public SpriteBatch spriteBatch;

        /// <summary>
        /// Fuente para la cadena en pantalla
        /// </summary>
        public SpriteFont fuente;
        /// <summary>
        /// Mensaje para mostrar en pantalla
        /// </summary>
        public String cadena;
        /// <summary>
        /// Posicion de la cadena 
        /// </summary>
        public Vector2 posicion;

        /// <summary>
        /// Fondo del Estado Inicial del juego
        /// </summary>
        public Texture2D fondo;

        /// <summary>
        /// Constructor para el Estado inicial del juego
        /// </summary>
        /// <param name="game"></param>
        public IniMenu(Game game)
            : base(game)
        {
            spriteBatch = new SpriteBatch(base.graphics.GraphicsDevice);
        }

        /// <summary>
        ///Initilizes the State, cargando su contenido y otras variables
        /// </summary>
        /// <param name="mensaje">Mensaje para instruccion al usuario</param>
        /// <param name="fondo1">Imagen de fondo</param>
        /// <param name="fuente1">Fuente del mensaje</param>
        public void Initialize(String mensaje,String fondo1, String fuente1)
        {
            fuente = content.Load<SpriteFont>(fuente1);
            fondo = content.Load<Texture2D>(fondo1);
            cadena = mensaje;
            posicion = new Vector2(
                        graphics.PreferredBackBufferWidth / 2 - fuente.MeasureString(cadena).X / 2 - 10,
                        (graphics.PreferredBackBufferHeight / 2 - fuente.MeasureString(cadena).Y / 2) + 225);
        }

        /// <summary>
        /// Actualiza el estado
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter)) {
                
                //content.Unload();

                (stateManager.estados[Gameestados.PlayingState] as PlayingState).Initialize();
                (stateManager.estados[Gameestados.PauseMenu] as PauseMenu).Initialize("Fonts/menu", "Images/FondoPausa");
                stateManager.estadoActual = Gameestados.PlayingState;
            }
        }

        /// <summary>
        /// Dibuja el estado
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime) {

     
            spriteBatch.Begin();
            spriteBatch.Draw(fondo, new Rectangle(0, 0, 820, 620), Color.White);
            spriteBatch.DrawString(fuente, cadena, posicion, Color.White);
        
            spriteBatch.End();
        }

        
    }
}
