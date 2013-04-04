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
    public class PuntajeState : State
    {

        /// <summary>
        /// Spritebatch principal
        /// </summary>
        public SpriteBatch spriteBatch;
        /// <summary>
        /// Manejador  de los recursos graficos
        /// </summary>
        public GraphicsDeviceManager graficos;

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

        int puntaje;

          /// <summary>
        /// Constructor para el Estado inicial del juego
        /// </summary>
        /// <param name="game"></param>
        public PuntajeState(Game game)
            : base(game)
        {
           
           
            spriteBatch = new SpriteBatch(base.graphics.GraphicsDevice);
            this.graficos = base.graphics;
           

           
        }

        /// <summary>
        /// Inicializa el estado, cargando su contenido y otras variables
        /// </summary>
        /// <param name="mensaje">Mensaje para instruccion al usuario</param>
        /// <param name="fondo1">Imagen de fondo</param>
        /// <param name="fuente1">Fuente del mensaje</param>
        public void Initialize(String mensaje, String fondo1, String fuente1, int point)
        {
            fuente = content.Load<SpriteFont>(fuente1);
            fondo = content.Load<Texture2D>(fondo1);
            cadena = mensaje;
            posicion = new Vector2(
                        graficos.PreferredBackBufferWidth / 2 - fuente.MeasureString(cadena).X / 2 + 8,
                        (graficos.PreferredBackBufferHeight / 2 - fuente.MeasureString(cadena).Y / 2) + 50);
            puntaje = point;
        }

        /// <summary>
        /// Actualiza el estado
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {

                //content.Unload();
            }
        }

        /// <summary>
        /// Dibuja el estado
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin();
            spriteBatch.Draw(fondo, new Rectangle(0, 0, 820, 620), Color.White);
            spriteBatch.DrawString(fuente, cadena+puntaje, posicion, Color.Black);

            spriteBatch.End();
        }
    }
}
