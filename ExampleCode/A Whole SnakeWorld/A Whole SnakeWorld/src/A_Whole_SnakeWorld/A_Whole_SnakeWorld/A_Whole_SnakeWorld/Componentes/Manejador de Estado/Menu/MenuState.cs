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
    public class MenuState : State
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
        /// Fuente del estado del menu
        /// </summary>
        public SpriteFont fuente;

        /// <summary>
        /// Textura del Fondo del Menu
        /// </summary>
        public Texture2D fondo;

        /// <summary>
        /// Menu correspondiente al estado, si es que es un menu
        /// </summary>
        public Menu menu = new Menu();

        /// <summary>
        /// Estado del control del juego anterior
        /// </summary>
        public GamePadState controlAnterior;

        /// <summary>
        /// Estado del control del juego actual
        /// </summary>
        public GamePadState controlActual;

        /// <summary>
        /// Estado del teclado del juego anterior
        /// </summary>
        public KeyboardState tecladoAnterior;

        /// <summary>
        /// Estado del teclado del juego actual
        /// </summary>
        public KeyboardState tecladoActual;

        /// <summary>
        /// Constructor del Estado Menu
        /// </summary>
        /// <param name="game"></param>
        public MenuState(Game game)
            : base(game)
        {
            spriteBatch = new SpriteBatch(base.graphics.GraphicsDevice);
            this.graficos = base.graphics;
           
           
        }

        /// <summary>
        /// Inicializa el estado, cargando su contenido y otras variables
        /// </summary>
        /// <param name="font">Fuente del menu</param>
        public void Initialize(String font,String fond) {

            fondo = content.Load<Texture2D>(fond);
            fuente = content.Load<SpriteFont>(font);
            controlActual = controlAnterior = GamePad.GetState(PlayerIndex.One);
            tecladoActual = tecladoAnterior = Keyboard.GetState();
        }

        /// <summary>
        /// Actualiza el menu
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {

                ManejadorControl();
            }
            else
            {
                ManejadorTeclado();

            }
        }
        
        /// <summary>
        /// Manejador del control
        /// </summary>
        private void ManejadorControl()
        {
            controlActual = GamePad.GetState(PlayerIndex.One);

            if (controlActual.Buttons.Start == ButtonState.Pressed && !(controlAnterior.Buttons.Start == ButtonState.Pressed))
            {
                if (menu.SelectedItem != null)
                {
                    menu.SelectedItem.PerformActivate();
                }
            }

            if (controlActual.ThumbSticks.Left.Y < 0 && !(controlAnterior.ThumbSticks.Left.Y < 0))
            {
                menu.SelectPrevious();
            }

            if (controlActual.ThumbSticks.Left.Y > 0 && !(controlAnterior.ThumbSticks.Left.Y > 0))
            {
                menu.SelectNext();
            }

            controlAnterior = controlActual;
        }

        /// <summary>
        /// Manejador del teclado
        /// </summary>
        private void ManejadorTeclado()
        {
            tecladoActual = Keyboard.GetState();

            if (tecladoActual.IsKeyDown(Keys.Enter) && !(tecladoAnterior.IsKeyDown(Keys.Enter)))
            {
                if (menu.SelectedItem != null)
                {
                    menu.SelectedItem.PerformActivate();
                }
            }

            if (tecladoActual.IsKeyDown(Keys.Up) && !(tecladoAnterior.IsKeyDown(Keys.Up)))
            {
                menu.SelectPrevious();
            }

            if (tecladoActual.IsKeyDown(Keys.Down) && !(tecladoAnterior.IsKeyDown(Keys.Down)))
            {
                menu.SelectNext();
            }

            tecladoAnterior = tecladoActual;
        }

        /// <summary>
        /// Dibuja los item del menu
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
           
         //  spriteBatch.Draw(fondo, new Rectangle(0, 0, fondo.Width, fondo.Height), Color.White);
            spriteBatch.Draw(fondo, new Rectangle(0, 0, 820, 620), Color.White);
            spriteBatch.End();
            menu.Draw(spriteBatch, fuente);
        }
    }
}
