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
    public class Personaje
    {
        #region Variables

        Texture2D _textura;
        public Vector2 _velocidad;
        public Vector2 _posicion;
        Vector2 _posInicial;
        public bool _tocoPiso;
        Vector2 _tamFrame;
        int frameXTotal;
        int frameYTotal;
        public int frameX;
        int frameY;
        float offset;

        public float timePerFrame;
        public float totalElapsed;

        #endregion

        #region Constructor

        public Personaje(Vector2 vec, Vector2 pos)
        {
            _posInicial = pos;
            _velocidad = vec;
            _posicion = pos;
            frameX = 0;
            frameY = 0;
            frameXTotal = 5;
            frameYTotal = 1;
            _tocoPiso = true;
            offset = 0.15f;

            timePerFrame = 0.2f;
            totalElapsed = 0;
        }

        #endregion

        #region Carga, Update y Draw
        public void LoadContent(ContentManager content, String nombre)
        {
            _textura = content.Load<Texture2D>(nombre);
            _tamFrame = new Vector2(offset * _textura.Width / frameXTotal, offset * _textura.Height / frameYTotal);
        }

        public void Update(float tiempo, KeyboardState anterior, KeyboardState actual)
        {
            if (_tocoPiso)
            {
                if (actual.IsKeyDown(Keys.Space) && anterior.IsKeyUp(Keys.Space))
                {
                    _velocidad.Y -= 1000;
                    _tocoPiso = false;
                    frameX = 4;
                    //frameY = 2;
                }
            }
            Animacion2D(4, tiempo);
            _posicion += _velocidad * tiempo;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
           

            Vector2 tamRA = new Vector2(_tamFrame.X / offset, _tamFrame.Y / offset);

            Rectangle sourceRect = new Rectangle(2*(int)tamRA.X * frameX, (int)tamRA.Y * frameY, (int)tamRA.X, (int)tamRA.Y);
            spriteBatch.Draw(_textura, recColision(), sourceRect, Color.White);
        }


        protected virtual void Animacion2D(int lastframe, float gametime)
        {
           

            totalElapsed += gametime;


            if (totalElapsed > timePerFrame && frameX < lastframe)
            {


                frameX++;
                if (frameX >= frameXTotal || frameX >= lastframe)
                {
                    frameX = 0;
                   
                }
   
                totalElapsed -= timePerFrame;

            }
        }
        #endregion

        #region Fisica: physics

        public void tocarPiso()
        {
            _tocoPiso = true;
            _velocidad.Y = 0;
            frameY = 0;
            frameX = 0;
            _posicion = _posInicial;
        }

        public Rectangle recColision()
        {
            return new Rectangle((int)(_posicion.X - _tamFrame.X / 2), (int)(_posicion.Y - _tamFrame.Y / 2), (int)_tamFrame.X, (int)_tamFrame.Y);
        }

        public virtual Vector3[] PuntosRectangulo(float offX, float offY)
        {
            Vector3[] puntos = new Vector3[5];

            puntos[0] = new Vector3(_posicion.X + offX - (_tamFrame.X / 2), _posicion.Y + offY - (_tamFrame.Y / 2), 0);
            puntos[1] = new Vector3(_tamFrame.X + _posicion.X + offX - (_tamFrame.X / 2), _posicion.Y + offY - (_tamFrame.Y / 2), 0);
            puntos[4] = new Vector3(_posicion.X + offX - (_tamFrame.X / 2), _posicion.Y + offY - (_tamFrame.Y / 2), 0);
            puntos[2] = new Vector3(_tamFrame.X + _posicion.X + offX - (_tamFrame.X / 2), _tamFrame.Y + _posicion.Y + offY - (_tamFrame.Y / 2), 0);
            puntos[3] = new Vector3(_posicion.X + offX - (_tamFrame.X / 2), _tamFrame.Y + _posicion.Y + offY - (_tamFrame.Y / 2), 0);

            return puntos;
        }

        #endregion

    }
}