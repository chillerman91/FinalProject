using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using SensorsAndSuch.Maps;
using System;
using SensorsAndSuch.Extensions;
using SensorsAndSuch.Mobs.AI;


namespace SensorsAndSuch.Items
{
    public abstract class Item
    {
        string name;
        public enum Materials
        { 
            Iron,
            Leather,
            Gold
        }
        public Item(string name)
        {
            this.name = name;
        }

        public bool Collect()
        {
            throw new NotImplementedException();
        }
    }
}
