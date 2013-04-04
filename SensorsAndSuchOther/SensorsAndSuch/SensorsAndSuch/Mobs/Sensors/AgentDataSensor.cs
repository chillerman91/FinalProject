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
        private Body attachedTo;
        private Color color = Color.LightGray;
        private FarseerPhysics.SamplesFramework.Sprite sprite;
        private List<Fixture> collided;
        private Texture2D texture { get; set; }
        #endregion

        public static int HighestNumbAdj = 3;
        public static int ValuesPerAdj = 2;
        public static int TotalRetVales = ValuesPerAdj * HighestNumbAdj;
        public AgentDataSensor(Body attached, Color colorIn)
        {
            texture = Globals.content.Load<Texture2D>("Sensors/Wisker");
           
            slices = BodyFactory.CreateCircle(Globals.World, radius: .5f, density: 1f);
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

        private float[] GetCloseAgents(Vector2 heading)
        {
            float[] ret = new float[TotalRetVales];
            heading.Normalize();
            SortedDictionary<float, Fixture> closeDict = new SortedDictionary<float, Fixture>();
            //Figure out what fixtures are closest
            foreach (Fixture col in collided)
            {
                Vector2 agentVector = col.Body.Position - slices.Position;
                float distance = agentVector.Length();
                while (closeDict.ContainsKey(distance))
                    distance += .1f;
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
                ret[i] = entry.Key;
                ret[i + 1] = angle / 360;
                i = i + ValuesPerAdj;
                if (i >= TotalRetVales) 
                    break;
            }

            // if there are less then @HighestNumbAdj fixtures fill in default data for remaing parts
            for (; i < TotalRetVales; i = i + ValuesPerAdj)
            {
                ret[i] = 1;
                ret[i + 1] = 0;
            }
            return ret;
        }
        
        public float[] Update(Vector2 heading, int startIndex, float[] ret)
        {            
            slices.Position = attachedTo.Position;
            slices.Rotation = attachedTo.Rotation;
            float[] nearAgents = GetCloseAgents(heading);
            for (int i = 0; i < nearAgents.Length; i++)
                ret[startIndex + i] = nearAgents[i];
                return nearAgents;
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
                            color, slices.Rotation, sprite.Origin, 1f, 
                            SpriteEffects.None, 0f);
        }


    }
}
