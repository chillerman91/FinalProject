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
        #region Variables
        Texture2D fondo;
        Texture2D[] objeto;
        public Vector2[] posicion;
        float rotacion;
        float[] offset;
        float[] radio;

        public float[] anguloObstaculo;
        int[] objetoObstaculo;
        float timeElapsed;
        float tiempoEspera;

        int minrandom;
        int maxrandom;
        float tiempo_de_random;
        #endregion

        #region Constructor

        public ManejadorMundo()
        {
            rotacion = 0;
            
            radio = new float[2];
            objeto = new Texture2D[2];
            offset = new float[2];

            posicion = new Vector2[12];
            anguloObstaculo = new float[12];
            objetoObstaculo = new int[12];
            tiempoEspera = 0;
            timeElapsed = 0;

            // Serpiente
            posicion[0] = new Vector2(400, 800);
            offset[0] = 1.23f;
            radio[0] = 0f;

            // Caja
            posicion[1] = new Vector2(380, 320);
            offset[1] = 0.05f;
            radio[1] = Vector2.Distance(posicion[0], posicion[1]);

            for (int i = 1; i > objetoObstaculo.Length; i++) 
            {
                posicion[i] = new Vector2(850,650 );
                anguloObstaculo[i] = MathHelper.Pi;
                objetoObstaculo[i] = 0;
            }

             minrandom = 5;
             maxrandom = 8;
             tiempo_de_random = 0;

        }

        #endregion

        #region Carga, Update y Draw

        public void LoadContent(ContentManager content, String[] nombres)
        {
            fondo = content.Load<Texture2D>("Images/Fondo");
            for (int i = 0; i < objeto.Length; i++)
                objeto[i] = content.Load<Texture2D>(nombres[i]);
        }

        public void Update(float tiempo)
        {
            rotar(tiempo, -1, 45);
            actualizarPosiciones(tiempo, 45);
            actualizarRandom(tiempo);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(fondo, new Vector2(425, 600), null, Color.White,
                                 rotacion/3, new Vector2(fondo.Width / 2, fondo.Height / 2), 1.45f, SpriteEffects.None, 0);
           // Debug.GS.Show("Rotacion", rotacion);
            spriteBatch.Draw(objeto[0], new Vector2((int)(posicion[0].X), (int)(posicion[0].Y)), null, Color.White,
                                rotacion, new Vector2(objeto[0].Width / 2, objeto[0].Height / 2), offset[0], SpriteEffects.None, 0);

            for (int i = 1; i < objetoObstaculo.Length; i++)
                if (objetoObstaculo[i] != 0)
                {
                    int j = objetoObstaculo[i];
                    spriteBatch.Draw(objeto[j], new Vector2((int)(posicion[i].X), (int)(posicion[i].Y)), null, Color.White,
                                 rotacion, new Vector2(objeto[j].Width / 2, objeto[j].Height / 2), offset[j], SpriteEffects.None, 0);
                }

           
        }

        #endregion

        #region Fisica

        public void actualizarRandom(float tiempo) {

            if (tiempo_de_random > 10) {
                minrandom = 3;
               
            }
            else if (tiempo_de_random > 12) { maxrandom = 4; }
            else if (tiempo_de_random > 20) { minrandom = 1 ; }
            else if (tiempo_de_random > 22) { maxrandom = 3; }
            else if (tiempo_de_random > 35) { maxrandom = 2; }
            else if (tiempo_de_random > 60) { minrandom = 1; maxrandom = 1; }

            tiempo_de_random += tiempo;
        }

        public void actualizarPosiciones(float tiempo, int velocidad) 
        {
            // Primero actualizo todos los obstaculos en movimiento
            for (int i = 1; i < objetoObstaculo.Length; i++) 
            {
                if (objetoObstaculo[i] != 0) {

                    if (anguloObstaculo[i] >= MathHelper.Pi)
                    {
                       posicion[i] = new Vector2(850,650 );;
                        anguloObstaculo[i] = 0;
                        objetoObstaculo[i] = 0;
                    }
                    else
                    {
                        int j = objetoObstaculo[i];

                        posicion[i].X = posicion[0].X + (float)(radio[j] * Math.Cos(anguloObstaculo[i]));
                        posicion[i].Y = posicion[0].Y + (float)(radio[j] * Math.Sin(anguloObstaculo[i]));

                        anguloObstaculo[i] -= MathHelper.ToRadians(velocidad * tiempo);
                    }
                }
            }

            // Ahora trato de poner un nuevo objeto en juego
            if (timeElapsed > tiempoEspera)
            {
                escogerItem();
                tiempoEspera = (new Random()).Next(minrandom, maxrandom);
                timeElapsed = 0;
            }
            else { timeElapsed += tiempo; }
        }

        public void escogerItem() 
        {
            int indice = 1;

            while (indice < objetoObstaculo.Length)
            {
                if (objetoObstaculo[indice] == 0)
                    break;
                indice++;
            }

            if (indice < objetoObstaculo.Length)
            {
                objetoObstaculo[indice] = (new Random()).Next(1, objeto.Length - 1);
                anguloObstaculo[indice] = 0f;
               posicion[indice] = new Vector2(850,650 );;
            }
        }

        public void rotar(float tiempo, int direccion, int velocidad)
        {
            rotacion += direccion * MathHelper.ToRadians(velocidad) * tiempo;
        }
        
        public Rectangle obtenerRectangulo(int indice)
        {
            if (objetoObstaculo[indice] != 0)
            {
                return new Rectangle((int)(posicion[indice].X - (objeto[1].Width * offset[1] / 2))+15, (int)(posicion[indice].Y - (objeto[1].Width * offset[1] / 2))+16, (int)(objeto[1].Width * offset[1])-20, (int)(objeto[1].Height * offset[1])-15);
            }
            else {

                return Rectangle.Empty;
            }
        }

        public virtual Vector3[][] PuntosRectangulo(float offX, float offY)
        {
            Vector3[][] Todospuntos = new Vector3[posicion.Length][];

            for (int i = 0; i < posicion.Length; i++)
            {
                if (objetoObstaculo[i] != 0)
                {
                    int j = objetoObstaculo[i];

                    Vector3[] puntos = new Vector3[5];

                    puntos[0] = new Vector3(posicion[i].X + offX - (objeto[j].Width * offset[j] / 2), posicion[i].Y + offY - (objeto[j].Height * offset[j] / 2), 0);

                    puntos[1] = new Vector3(objeto[j].Width * offset[j] + posicion[i].X + offX - (objeto[j].Width * offset[j] / 2), posicion[i].Y + offY - (objeto[j].Height * offset[j] / 2), 0);

                    puntos[4] = new Vector3(posicion[i].X + offX - (objeto[j].Width * offset[j] / 2), posicion[i].Y + offY - (objeto[j].Height * offset[j] / 2), 0);

                    puntos[2] = new Vector3(objeto[j].Width * offset[j] + posicion[i].X + offX - (objeto[j].Width * offset[j] / 2), objeto[j].Height * offset[j] + posicion[i].Y + offY - (objeto[j].Height * offset[j] / 2), 0);

                    puntos[3] = new Vector3(posicion[i].X + offX - (objeto[j].Width * offset[j] / 2), objeto[j].Height * offset[j] + posicion[i].Y + offY - (objeto[j].Height * offset[j] / 2), 0);

                    Todospuntos[i] = puntos;
                }

            }
            return Todospuntos;
        }

        #endregion

    }
}
