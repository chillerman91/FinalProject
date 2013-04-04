using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using SensorsAndSuch.Texts;
using System;
using SensorsAndSuch.Maps;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.SamplesFramework;
using SensorsAndSuch.Extensions;
using SensorsAndSuch.Mobs.AI;
using SensorsAndSuch.Mobs;


namespace SensorsAndSuch.Items
{
    public class BaseWeapon : Item
    {
        protected BaseMonster holder;
        int timesUsed = 0;
        int timesSinceLast = 0;
        protected bool inUse;
        public int damage { get; set; }

        protected Body shape;
        protected FarseerPhysics.SamplesFramework.Sprite Sprite;

        public BaseWeapon(string Name, BaseMonster holder)
            :base(Name)
        {
            this.holder = holder;
            inUse = false;
            damage = 50;
        }

        public virtual int StartUse()
        {
            inUse = true;
            timesUsed++;
            return 0;
        }

        public virtual bool EndUse()
        {
            inUse = false;
            return true;
        }

        public virtual void Draw(SpriteBatch batch)
        {
            if (!inUse) return;
            batch.Draw(Sprite.Texture,
                               Globals.map.ScreenFromPhysics(shape.Position), null,
                               Color.White, shape.Rotation, Sprite.Origin, Globals.map.globalScale * 1f,
                               SpriteEffects.None, 0f);
        }

        internal void kill()
        {
            shape.Dispose();
        }
    }
}