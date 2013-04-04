using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace A_Whole_SnakeWorld
{
    public class Circulo
    {

        #region Variables

        /// <summary>
        /// Radio del circulo
        /// </summary>
        public int Radio;

        /// <summary>
        /// Coordenada X del centro del circulo
        /// </summary>
        public int X;

        /// <summary>
        /// Coordenada Y del centro del circulo
        /// </summary>
        public int Y;

        /// <summary>
        /// Punto que indica el centro del circulo
        /// </summary>
        public Point Centro
        {
            get { return new Point(X, Y); }
            set { X = value.X; Y = value.Y; }
        }

        #endregion

        #region Constructores

        /// <summary>
        /// Crea un circulo dadas las coordenadas del centro y su radio
        /// </summary>
        /// <param name="X">Coordenada X del centro</param>
        /// <param name="Y">Coordenada Y del centro</param>
        /// <param name="Radio">Radio del circulo</param>
        public Circulo(int X, int Y, int Radio)
        {
            this.X = X;
            this.Y = Y;
            this.Radio = Radio;
        }

        #endregion

        #region Funciones

        /// <summary>
        /// Rectangulo de menor tamano para graficar
        /// </summary>
        /// <returns>Retorna un "Rectangle" que representa el BoundingRect</returns>
        public Rectangle BoundingRect()
        {
            return new Rectangle(X - Radio, Y - Radio, 2 * Radio, 2 * Radio);
        }

        /// <summary>
        /// Determina la distancia entre este el centro de este circulo y otro,
        /// para ver si se intersectan
        /// </summary>
        /// <param name="circ">El otro circulo a comparar</param>
        /// <returns>Int que indica aproximadamente la distancia</returns>
        public int Distancia(Circulo circ)
        {
            int difX = circ.X - X;
            int difY = circ.Y - Y;
            int disC = difX * difX + difY * difY;

            return (int)Math.Sqrt(disC);
        }

        /// <summary>
        /// Determina cuando este Circulo contiene completamente a otro
        /// </summary>
        /// <param name="circ">El Circulo que debe estar contenido</param>
        /// <returns>Un bool indicando si este Circulo contiene a Circ o no</returns>
        public bool Contiene(Circulo circ)
        {
            return Distancia(circ) + circ.Radio <= Radio;
        }

        /// <summary>
        /// Determina cuando este Circulo contiene completamente a un punto
        /// </summary>
        /// <param name="punto">El punto que debe estar contenido</param>
        /// <returns>Un bool indicando si este Circulo contiene al punto o no</returns>
        public bool Contiene(Point punto)
        {
            return Contiene(new Circulo(punto.X, punto.Y, 0));
        }

        /// <summary>
        /// Determina cuando este Circulo contiene completamente a un Rectangulo
        /// </summary>
        /// <param name="rect">El Rectangulo que debe estar contenido</param>
        /// <returns>Un bool indicando si este Circulo contiene al Rectangulo o no</returns>
        public bool Contiene(Rectangle rect)
        {
            Point[] esquinas = new Point[4]
                { new Point(rect.Left, rect.Top),
                  new Point(rect.Right, rect.Top),
                  new Point(rect.Left, rect.Bottom),
                  new Point(rect.Right, rect.Bottom) };

            foreach (Point p in esquinas)
                if (!Contiene(p))
                    return false;

            return true;
        }

        /// <summary>
        /// Determina cuando este Circulo Intersecta o Contiene a otro.
        /// </summary>
        /// <param name="circ">El otro circulo a comparar</param>
        /// <returns>Un bool indicando si hay Intereseccion o no</returns>
        public bool Intersecta(Circulo circ)
        {
            return Distancia(circ) <= Radio + circ.Radio;
        }

        /// <summary>
        /// Determina cuando este Circulo Intersecta o Contiene a un Rectangulo.
        /// </summary>
        /// <param name="rect">El otro rectangulo a comparar</param>
        /// <returns>Un bool indicando si hay Intereseccion o no</returns>
        public bool Intersecta(Rectangle rect) 
        {
            // El Rectangulo contiene el centro del Circulo:
            if (rect.Contains(Centro))
                return true;

            // El Circulo Contiene alguna de las 4 esquinas del Rectangulo.
            Point[] esquinas = new Point[4]
                { new Point(rect.Left, rect.Top),
                  new Point(rect.Right, rect.Top),
                  new Point(rect.Left, rect.Bottom),
                  new Point(rect.Right, rect.Bottom) };

            foreach (Point p in esquinas)
                if (Contiene(p))
                    return true;

            // El Circulo toca alguno de los bordes rectos del Rectangulo:
            // Bordes laterales:
            if (rect.Top <= Y && rect.Bottom >= Y)
                if (MathHelper.Distance(rect.Left, X) <= Radio || MathHelper.Distance(rect.Right, X) <= Radio)
                    return true;
            // Bordes Superior e Inferior:
            if (rect.Left <= X && rect.Right >= X)
                if (MathHelper.Distance(rect.Bottom, Y) <= Radio || MathHelper.Distance(rect.Top, Y) <= Radio)
                    return true;
            
            return false;
        }

        #endregion
    }
}