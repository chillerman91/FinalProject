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
    public class MenuItem
    {
        /// <summary>
        /// Nombre del item
        /// </summary>
        public string Name;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Activate;
        /// <summary>
        /// 
        /// </summary>
        public bool IsDisabled = false;
 
  
        /// <summary>
        /// Constructor del Item del Menu de opciones
        /// </summary>
        /// <param name="name">Nombre del item</param>
           public MenuItem(string name)
           {
               Name = name;
           }

      
         public void PerformActivate()
         {
             if (Activate != null)
             {
                 Activate(this, null);
             }
         }
    }
}
