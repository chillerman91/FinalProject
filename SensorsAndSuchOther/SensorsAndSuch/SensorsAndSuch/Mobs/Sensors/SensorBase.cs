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

namespace SensorsAndSuch.Mobs.Sensors
{
    public abstract class SensorBase
    {
        int totalRetVales;

        internal abstract float[] GetReturnValues();
        public virtual void Update()
        {
        }

        public abstract void Draw(SpriteBatch batch);
    }
}