using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;


namespace Infinity_TD
{
    class Animacao
    {
        public Texture2D playerTextura;

        private int frameAtual;
        private int tempoFrame;
        private int duracaoFrame;
        private int contagemFrame;

        public int larguraFrame;
        public int alturaFrame;
        private float escala;
        public Vector2 Position;

        Rectangle sourceRect = new Rectangle();
        Rectangle destinationRect = new Rectangle();

        private bool Looping;
        public bool Estado;

        public int Width
        {
            get { return playerTextura.Width; }
        }

        public int Heigth
        {
            get { return playerTextura.Height; }
        }

        public Animacao(Texture2D _playerTextura,
                        Vector2 _position,
                        int _larguraFrame,
                        int _alturaFrame,
                        int _contagemFrame,
                        int _tempoFrame,
                        float _escala,
                        bool _looping) 
        {
            this.larguraFrame = _larguraFrame;
            this.alturaFrame = _alturaFrame;
            this.contagemFrame = _contagemFrame;
            this.tempoFrame = _tempoFrame;
            this.escala = _escala;
            this.Position = _position;

            playerTextura = _playerTextura;
            Looping = _looping;

            duracaoFrame = 0;
            frameAtual = 0;

            Estado = true;
        }

        public void Update(GameTime gameTime, Vector2 _position) 
        {
            if (Estado == false)
                return;

            duracaoFrame += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            //duracaoFrame += (int)gameTime.ElapsedGameTime.TotalSeconds;

            if (duracaoFrame > tempoFrame)
            {
                frameAtual++;
                if (frameAtual == contagemFrame)
                {
                    frameAtual = 0;
                    if (Looping == false)
                        Estado = false;
                }
                duracaoFrame = 0;
            }

            sourceRect = new Rectangle(frameAtual * larguraFrame, 0, larguraFrame, alturaFrame);

            Position = _position;

            destinationRect = new Rectangle((int)Position.X + (int)(larguraFrame * escala) / 2,
                                            (int)Position.Y + (int)(alturaFrame * escala) / 2,
                                            (int)(larguraFrame * escala),
                                            (int)(alturaFrame * escala));

        }

        public void Draw(SpriteBatch spriteBatch, float rotation) 
        {
            spriteBatch.Draw(playerTextura, destinationRect, sourceRect, Color.White, rotation, new Vector2(playerTextura.Width/4, playerTextura.Height/4), SpriteEffects.None, 0f);
        }
    }
}
