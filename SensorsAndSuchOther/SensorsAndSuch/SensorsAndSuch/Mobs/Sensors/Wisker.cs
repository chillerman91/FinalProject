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
    public class Wisker : SensorBase
    {
        public static int totalRetVales;

        #region Datafields

        float wiskerLength;
        Texture2D texture;
        Body attatchedTo;
        private Vector2 dir;
        Color color;
        Color defaultC = Color.White;
        float OffSet;
        private float distance;

        #endregion

        #region Properties

        public float Distance
        {
            get { return distance; }
        }

        #endregion

        public Wisker(Body attatched, float offSet, float wiskerLengthGrid)
        {
            texture = Globals.content.Load<Texture2D>("Sensors/Wisker");
            attatchedTo = attatched;
            //WiskerR = WiskerLength / .16f;
            wiskerLength = wiskerLengthGrid;
            OffSet = offSet;
            color = defaultC;
        }

        internal override float[] GetReturnValues()
        {
            float[] distAsArray = new float[1];
            distance = Globals.map.isPathFree(Globals.map.GridFromPhysics(attatchedTo.Position), (attatchedTo.Rotation + OffSet).GetVecFromAng(), wiskerLength);
            distAsArray[0] = distance;
            return distAsArray;
        }

        internal float[] RayCast()
        {
            float[] distAsArray = new float[1];
            distance = 1;
            Globals.World.RayCast((fixture, point, normal, fraction) =>
            {
                if (fixture != null && fixture.CollisionCategories != Category.Cat2 && fixture.IsSensor == false && distance > fraction)
                {
                    distance = fraction;
                }
                return 1;
            }
            , attatchedTo.Position, attatchedTo.Position + (attatchedTo.Rotation + OffSet).GetVecFromAng() * wiskerLength * .16f);
            int r = (int)(240 * (distance - .3) / .7f) + 10;
            distAsArray[0] = distance;
            return distAsArray;
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(texture,
               Globals.map.ScreenFromPhysics(attatchedTo.Position ), null,
               color, attatchedTo.Rotation + OffSet, new Vector2(texture.Width / 2, texture.Height / 2), wiskerLength * distance*Globals.map.globalScale,
               SpriteEffects.None, 0f);
        }
    }
}