using System.Linq;
using System.Text;
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
using FarseerPhysics.Dynamics.Contacts;
using SensorsAndSuch.Extensions;

namespace SensorsAndSuch.Mobs.Sensors
{
    public class AgentDataSensor: SensorBase
    {
        #region Datafields

        public float shortestDist;
        private Body slices;
        private static float slicesRadius = 2f;
        private Body attachedToBody;
        private BaseMonster mon;
        private Color color = Color.LightGray;
        private FarseerPhysics.SamplesFramework.Sprite sprite;
        private List<Fixture> collided;
        private Texture2D texture { get; set; }
        public int count { get { return collided.Count; } }

        public static int highestNumbAdj = 2;
        public static int valuesPerAdj = 5;
        public static int TotalRetVales = valuesPerAdj * highestNumbAdj;

        public static int NumSlices = 6; // NOTE: This should be pulled out into a config file so brain inputs can be dymically affected by this number changing.

        #endregion
        public AgentDataSensor(BaseMonster mon, Body attached, Color colorIn)
        {
            texture = Globals.content.Load<Texture2D>("Sensors/Wisker");

            slices = BodyFactory.CreateCircle(Globals.World, radius: slicesRadius, density: 1f);
            sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(slices.FixtureList[0].Shape,
                                                                                MaterialType.Squares, Color.Teal, 1f));
            
            slices.FixtureList[0].IsSensor = true;
            slices.FixtureList[0].OnCollision += CollisionHandler;
            slices.FixtureList[0].OnSeparation += SeparationHandler;
            collided = new List<Fixture>();

            attachedToBody = attached;
            this.mon = mon;
            color = colorIn;
        }

        public bool CollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            BaseMonster mon = null;
            if (!collided.Contains(fixtureB) && !fixtureB.Body.Equals(attachedToBody) && Globals.Mobs.GetMonster(fixtureB.Body.BodyId, ref mon))
                collided.Add(fixtureB);
            return true;
        }

        public void SeparationHandler(Fixture fixtureA, Fixture fixtureB)
        {
            collided.Remove(fixtureB);
        }

        public void RemoveAll()
        {
            collided.Clear();
        }

        public void CheckCollisions()
        {
            int max = collided.Count;
            for (int i = 0; i < max; i++)
            {
                float dist = (collided[i].Body.Position - slices.Position).Length();
                if (dist > slicesRadius * 1.2)
                {
                    collided.RemoveAt(i);
                    //max--;
                    i = max;//--;
                }
            }

        }

        internal override float[] GetReturnValues()
        {
            
            float[] ret = new float[TotalRetVales];
            float[] quadrantActivation = new float[NumSlices];
            CheckCollisions();


            //Set initial values
            for (int i = 0; i < TotalRetVales; i++)
            {
                ret[i] = 0;
            }
            return CreatureDataCollection(ret);

        }

        private float[] CreatureDataCollection(float[] data)
        {
            #region Set up data
            SortedDictionary<float, Fixture> closeDict = new SortedDictionary<float, Fixture>();
            Vector2 heading = mon.Dir;
            heading.Normalize();
            shortestDist = 1;
            #endregion

            #region Figure out what fixtures are closest
            foreach (Fixture col in collided)
            {
                Vector2 agentVector = col.Body.Position - slices.Position;
                float distance = agentVector.Length() / slicesRadius;
                if (distance < shortestDist) shortestDist = distance;
                while (closeDict.ContainsKey(distance))
                    distance += .00001f;
                closeDict.Add(distance, col);
            }
            #endregion


            #region Get @valuesPerAdj Data for the @highestNumbAdj characters
            int i = 0;
            foreach (KeyValuePair<float, Fixture> entry in closeDict)
            {
                //Only use data that the attatched monster can see
                if (.9f > Globals.map.isPathFree(mon.currentGridPos, Globals.map.GridFromPhysics(entry.Value.Body.Position)))
                {
                    BaseMonster seenMon = null;
                    Globals.Mobs.GetMonster(entry.Value.Body.BodyId, ref seenMon);

                    Vector2 agentVector = entry.Value.Body.Position - slices.Position;
                    float distance = agentVector.Length();
                    Vector2 OtherHeading = entry.Value.Body.Rotation.GetVecFromAng();
                    agentVector.Normalize();
                    float angle = (float)(Math.Acos(Vector2.Dot(heading, agentVector)) * (180 / Math.PI));
                    float angle2 = (float)(Math.Acos(Vector2.Dot(OtherHeading, agentVector)) * (180 / Math.PI));

                    if (float.IsNaN(angle))
                        angle = 0;
                    data[i++] = 1;

                    //ret[i++] = agentVector.X;
                    //ret[i++] = agentVector.Y;
                    data[i++] = 1 - entry.Key;
                    //ret[i++] = OtherHeading.X;
                    //ret[i++] = OtherHeading.Y;

                    data[i++] = (angle % 360 / 360);
                    data[i++] = (angle2 % 360 / 360);
                    data[i++] = seenMon.monRatio;
                    if (i >= TotalRetVales)
                        break;
                }
            }
            #endregion

            return data;
        }

        //TODO: fix this: QuadrentVersion
        private float[] QuadrentVersion()
        {
            #region Set up data
            Vector2 heading = mon.Dir;
            heading.Normalize();
            #endregion

            float[] quadrantActivation = new float[4];
            foreach (Fixture col in collided)
            {
                Vector2 agentVector = col.Body.Position - slices.Position;
                float distance = agentVector.Length();
                agentVector.Normalize();

                float sliceSize = (float)(360.0 / NumSlices);

                if (slicesRadius > agentVector.X && slicesRadius > agentVector.Y)
                {
                    float angle = (float)(Math.Acos(Vector2.Dot(heading, agentVector)) * (180 / Math.PI));

                    // Do a cross product to determine which side of the heading the agent is on.
                    // If the z component of the cross product is positive, the agent is closer to the right then the left.
                    // We adjust the degrees then to make it possible for the degrees to have a range of 0-359. 
                    if (Vector3.Cross(new Vector3(heading, 0), new Vector3(agentVector, 0)).Z > 0)//Vector2.Dot(distance, normal) > )
                    {
                        angle = 360 - angle;
                    }

                    // Set which pie slice the agent is in based on angle.
                    for (int i = 0; i < NumSlices; i++)
                    {
                        if (angle >= i * sliceSize && angle < (sliceSize + 1) * sliceSize)
                        {
                            quadrantActivation[i]++;
                            break; // If we found which slice the agent is in, no sense in checking the sectors.
                        }
                    }
                }
            }
            return quadrantActivation;
        }

        public override void Update()
        {            
            slices.Position = attachedToBody.Position;
            slices.Rotation = attachedToBody.Rotation;
        }

        internal void ClearCollisions()
        {
            collided.Clear();
        }

        public void Kill()
        {
            slices.Dispose();
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(sprite.Texture, 
                            Globals.map.ScreenFromPhysics(slices.Position), null,
                            color, slices.Rotation, sprite.Origin, Globals.map.globalScale, 
                            SpriteEffects.None, 0f);
        }


    }
}
