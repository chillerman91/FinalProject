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
    public class ManejadorMundo
    {
        SpriteBatch spriteBatch;

        /// <summary>
        /// Arreglo de objetos a cargar (por ahora solo la serpiente)
        /// </summary>
        Texture2D[] objeto;
        Rectangle imgRect;
        Vector2[] posicion;
        float rotacion;
        Vector2 escala;

        public ManejadorMundo(SpriteBatch spriteBatch) 
        {
            this.spriteBatch = spriteBatch;
        }

        public void Initialize() 
        {
            posicion = new Vector2[1];
            objeto = new Texture2D[1];
            rotacion = 0;

            // Posicion de la serpiente
            posicion[0] = new Vector2(400, 800);
            escala = new Vector2(1000, 1000);

         
        }

        public void LoadContent(ContentManager content, String[] nombres) 
        {
            for (int i = 0; i < nombres.Length; i++)
                objeto[i] = content.Load<Texture2D>(nombres[i]);
            
        }

        public void crearRectangulo(Texture2D img)
        {
            int alto = img.Height;
            int ancho = img.Width;

            imgRect = new Rectangle(0, 0, ancho, alto);
        }

        public void Update(GameTime gametime) 
        {
            float tiempo = (float) gametime.ElapsedGameTime.TotalSeconds;
            Rotate(tiempo, -1, 45);
        }

        public void Draw() 
        {
            

            spriteBatch.Begin();

            for (int i = 0; i < objeto.Length; i++)
                spriteBatch.Draw(objeto[i], new Rectangle((int)posicion[i].X, (int)posicion[i].Y, (int)escala.X, (int)escala.Y), null, Color.White,
                                 rotacion, new Vector2(objeto[i].Width / 2, objeto[i].Height / 2), SpriteEffects.None, 0);

            spriteBatch.End();
        }

        /// <summary>
        /// Establece la rotacion del personaje de acuerdo al tiempo y direccion dados
        /// </summary>
        /// <param name="tiempo">Tiempo por el cual debe rotarse</param>
        /// <param name="direccion">Direccion a la cual rotar; -1 sentido reloj y +1 sentido contrario</param>
        public void Rotate(float tiempo, int direccion, int velocidad)
        {
            rotacion += direccion * MathHelper.ToRadians(velocidad) * tiempo;
        }


    }
}