using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace A_Whole_SnakeWorld
{
    public class PlayingState : State
    {
        #region Variables

        public SpriteBatch spriteBatch;
        public GraphicsDeviceManager graficos;
        ManejadorGrafico asset;
        public GamePadState controlAnterior;
        public KeyboardState tecladoAnterior;
        public ManejadorMundo manejadorMundo;// WorldDriver
        public Personaje personaje; //Character
        public bool debug;
        public SpriteFont fuente; // Fountain
        public int puntaje; // Pointer
        public float Tpuntaje; // Ponter 

        public Song musica; // Music
        #endregion

        #region Constructor and initilizer

        public PlayingState(Game game)
            : base(game)
        {
            spriteBatch = new SpriteBatch(base.graphics.GraphicsDevice);
            this.graficos = base.graphics;
            asset = new ManejadorGrafico(base.game.GraphicsDevice);
        }

        public void Initialize()
        {
            controlAnterior = GamePad.GetState(PlayerIndex.One);
            tecladoAnterior = Keyboard.GetState();

            manejadorMundo = new ManejadorMundo();
            personaje = new Personaje(new Vector2(0, 0), new Vector2(400, 308));

            String[] nombres = { "Images/Serpiente", "Images/Muro"};

            manejadorMundo.LoadContent(content, nombres);
            personaje.LoadContent(content, "Images/DragonSheeter");
            debug = true;
            puntaje = 0;
            Tpuntaje = 0;

            fuente = content.Load<SpriteFont>("Fonts/menu");

            musica = content.Load<Song>("Sound/Tron");
            //MediaPlayer.Play(musica); 
        }

        #endregion

        #region Carga, Update y Draw

        public void SumarPuntaje(float tiempo) {

            if (Tpuntaje > 1)
            {

                puntaje += 1;
                Tpuntaje = 0;
            }
            else { Tpuntaje += tiempo; }

           

        }

        /// <summary>
        /// Actualiza el estado
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            if (MediaPlayer.State == MediaState.Paused) { MediaPlayer.Resume(); }
           

            float deltaTiempo = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState teclado = Keyboard.GetState();

            if ((teclado.IsKeyDown(Keys.Enter) && !(tecladoAnterior.IsKeyDown(Keys.Enter))))
            {
                MediaPlayer.Pause();
                stateManager.estadoActual = Gameestados.PauseMenu;
            }
            ManejadorFisica(deltaTiempo, 2500);
            manejadorMundo.Update(deltaTiempo);
            personaje.Update(deltaTiempo, tecladoAnterior, teclado);

            tecladoAnterior = teclado;

            SumarPuntaje(deltaTiempo);
        }
       

        /// <summary>
        /// Dibuja el estado
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
           
            manejadorMundo.Draw(spriteBatch);
            personaje.Draw(spriteBatch);
            spriteBatch.DrawString(fuente, "Puntaje = " + puntaje, new Vector2(600,50), Color.BlueViolet);
            spriteBatch.DrawString(fuente, "Enter = Pausar" , new Vector2(0, 0), Color.BlueViolet);
            spriteBatch.End();

           // if (debug)
              //  DrawDebug();
        }

        #endregion

        #region Fisica
        public void ManejadorFisica(float tiempo, float aceleracion)
        {
            if (!personaje._tocoPiso)
            {
                personaje._velocidad.Y += aceleracion * tiempo;

                if (personaje.recColision().Bottom > 380)
                {
                    personaje.tocarPiso();
                }
            }

            for(int i = 1; i< manejadorMundo.posicion.Length;i++){
                if (personaje.recColision().Intersects(manejadorMundo.obtenerRectangulo(i))) {

                    (stateManager.estados[Gameestados.PuntajeState] as PuntajeState).Initialize("Su puntaje es: ", "Images/FondoLoading","Fonts/menu",puntaje);
                    MediaPlayer.Stop();
                    Thread.Sleep(1000);

                    stateManager.estadoActual = Gameestados.PuntajeState;
                }
            
            }
        }
        #endregion

        #region Debug

        private void DrawDebug()
        {
            //for (int i = 0; i < grumpys.Length; i++)
            // {-362 -290
            asset.DrawLines(graficos, personaje.PuntosRectangulo(-250, -260), Color.Gold);
            //Vector3[] prueba = new Vector3[2] { new Vector3(0 - 250, 380 - 260, 0), new Vector3(800 - 250, 380 - 260 , 0) };
            //asset.DrawLines(graficos, prueba, Color.Gold);
            Vector3[][] prueba2 = manejadorMundo.PuntosRectangulo(-250, -260);
            for (int i = 0; i < prueba2.Length; i++)
            {
                asset.DrawLines(graficos, prueba2[i], Color.Gold);
            }

            // }
        }

        #endregion
    }
}
