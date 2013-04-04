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
    public class Menu
    {
        /// <summary>
        /// Lista de opciones disponibles en el menu
        /// </summary>
        public List<MenuItem> Items = new List<MenuItem>(); 
        /// <summary>
        /// Posicion en el item actual
        /// </summary>
        private int itemActual = 0;

        /// <summary>
        /// Devuelve el item seleccionado actualmente
        /// </summary>
        public MenuItem SelectedItem
        {
            get
            {
                if (Items.Count > 0)
                    return Items[itemActual];
                else
                    return null;
            }
        }

        /// <summary>
        /// Selecciona el siguiente elemento del menu
        /// </summary>
        public void SelectNext()
        {
            if (Items.Count > 0)
            {
                do
                {
                    itemActual = (itemActual + 1) % Items.Count;
                } while (SelectedItem.IsDisabled);
            }
        }

        /// <summary>
        /// Selecciona el anterior elemento del menu
        /// </summary>
        public void SelectPrevious()
        {
            if (Items.Count > 0)
            {
                do
                {
                    itemActual--;
                    if (itemActual < 0)
                        itemActual = Items.Count - 1;
                } while (SelectedItem.IsDisabled);
            }
        }

        /// <summary>
        /// Dibuja los elementos del menu en la pantalla
        /// </summary>
        /// <param name="spritebatch"></param>
        /// <param name="font">Fuente del Menu</param>
        public void Draw(SpriteBatch spritebatch, SpriteFont fuente)
        {
            spritebatch.Begin();

            for (int i = 0; i < Items.Count; i++)
            {
                MenuItem item = Items[i];
                Vector2 size = fuente.MeasureString(item.Name);

                Vector2 pos = new Vector2(
                    spritebatch.GraphicsDevice.Viewport.Width / 2,
                    spritebatch.GraphicsDevice.Viewport.Height / 2);

                pos -= size * 0.5f;
                pos.Y += i * (size.Y * 1.1f);

                Color color = Color.White;
                if (item == SelectedItem)
                    color = Color.Yellow;
                else if (item.IsDisabled)
                    color = Color.Coral;

                spritebatch.DrawString(fuente, item.Name, pos, color);
            }

            spritebatch.End();
        }
    }
}
