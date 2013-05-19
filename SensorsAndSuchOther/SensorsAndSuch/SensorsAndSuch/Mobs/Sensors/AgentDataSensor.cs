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
namespace SensorsAndSuch.Mobs
{
    public class AgentDataSensor
    {
        #region Datafields

        private Body slices;
        private static float slicesRadius = 1.5f;
        private Body attachedTo;
        private Color color = Color.LightGray;
        private FarseerPhysics.SamplesFramework.Sprite sprite;
        private List<Fixture> collided;
        private Texture2D texture { get; set; }
        public int count { get { return collided.Count; } }

        public static int HighestNumbAdj = 1;
        public static int ValuesPerAdj = 4;
        public static int TotalRetVales = ValuesPerAdj * HighestNumbAdj;

        public static int NumSlices = 6; // NOTE: This should be pulled out into a config file so brain inputs can be dymically affected by this number changing.

        #endregion
        public AgentDataSensor(Body attached, Color colorIn)
        {
            texture = Globals.content.Load<Texture2D>("Sensors/Wisker");

            slices = BodyFactory.CreateCircle(Globals.World, radius: slicesRadius, density: 1f);
            sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(slices.FixtureList[0].Shape,
                                                                                MaterialType.Squares, Color.Teal, 1f));
            
            slices.FixtureList[0].IsSensor = true;
            slices.FixtureList[0].OnCollision += CollisionHandler;
            slices.FixtureList[0].OnSeparation += SeparationHandler;
            collided = new List<Fixture>();

            attachedTo = attached;
            color = colorIn;
        }

        public bool CollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (!collided.Contains(fixtureB) && !fixtureB.Body.Equals(attachedTo))
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
        private float[] GetCloseAgents(Vector2 heading)
        {
            float[] ret = new float[TotalRetVales];
            float[] quadrantActivation = new float[NumSlices];
            heading.Normalize();
            CheckCollisions();
            #region Commented (closest in circle stuff)
            SortedDictionary<float, Fixture> closeDict = new SortedDictionary<float, Fixture>();
            //Figure out what fixtures are closest
            foreach (Fixture col in collided)
            {
                Vector2 agentVector = col.Body.Position - slices.Position;
                float distance = agentVector.Length();
                while (closeDict.ContainsKey(distance))
                    distance += .001f;
                closeDict.Add(distance, col);
            }
            int i = 0;

            //Add data to ret for each fixture, with the closest fixtures comming first
            //Limit of fixtures is in static vairable TotalRetVales
            foreach (KeyValuePair<float, Fixture> entry in closeDict)
            {
                Vector2 agentVector = entry.Value.Body.Position - slices.Position;
                float distance = agentVector.Length();
                agentVector.Normalize();
                float angle = (float)(Math.Acos(Vector2.Dot(heading, agentVector)) * (180 / Math.PI));
                if (float.IsNaN(angle))
                    angle = 0;
                ret[i] = 1;
                ret[i + 1] = angle / 360;
                ret[i + 2] = entry.Key;
                ret[i + 3] = entry.Value.Body.Rotation/(2*(float) Math.PI);
                i = i + ValuesPerAdj;
                if (i >= TotalRetVales) 
                    break;
            }

            //if there are less then @HighestNumbAdj fixtures fill in default data for remaing parts
            for (; i < TotalRetVales; i = i + ValuesPerAdj)
            {
                ret[i] = 1;
                ret[i + 1] = 0;
                ret[i + 2] = 0;
                ret[i + 3] = 0;
            }
            return ret;
            #endregion
            /*
            #region Tweeked Pie Slice Logic
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
            #endregion
            */
        }
        
        public float[] Update(Vector2 heading)
        {            
            slices.Position = attachedTo.Position;
            slices.Rotation = attachedTo.Rotation;
            //float[] nearAgents = GetCloseAgents(heading);
            //for (int i = 0; i < nearAgents.Length; i++)
            //    ret[startIndex + i] = nearAgents[i];
            //return nearAgents;
            float[] proximalAgents = GetCloseAgents(heading);
            return proximalAgents;
        }

        internal void ClearCollisions()
        {
            collided.Clear();
        }

        public void Kill()
        {
            slices.Dispose();
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(sprite.Texture, 
                            Globals.map.ScreenFromPhysics(slices.Position), null,
                            color, slices.Rotation, sprite.Origin, Globals.map.globalScale, 
                            SpriteEffects.None, 0f);
        }


    }
}
