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
    public class PieSlice
    {
        #region Datafields

        private Body slices;
        private Body attachedTo;
        private Color color = Color.LightGray;
        private FarseerPhysics.SamplesFramework.Sprite sprite;
        private List<Fixture> collided;
        private Texture2D texture { get; set; }
        private float pieLength;

        #endregion

        public PieSlice(Body attached, Color colorIn, float pieLengthIn)
        {
            texture = Globals.content.Load<Texture2D>("Sensors/Wisker");
            slices = BodyFactory.CreateRectangle(Globals.World, pieLengthIn * 2, pieLengthIn * 2, 1f);

            sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(slices.FixtureList[0].Shape,
                                                                                MaterialType.Squares, Color.Teal, 1f));

            
            slices.FixtureList[0].IsSensor = true;
            slices.FixtureList[0].OnCollision += CollisionHandler;
            slices.FixtureList[0].OnSeparation += SeparationHandler;
            collided = new List<Fixture>();

            attachedTo = attached;
            color = colorIn;
            pieLength = pieLengthIn;
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

        private int[] GetCloseAgents(Vector2 heading)
        {
            int[] quadrantActication = new int[4];
            heading.Normalize();

            foreach (Fixture col in collided)
            {
                Vector2 agentVector = col.Body.Position - slices.Position;
                float distance = agentVector.Length();
                agentVector.Normalize();

                if (pieLength > agentVector.X && pieLength > agentVector.Y)
                {
                    float angle = (float)(Math.Acos(Vector2.Dot(heading, agentVector)) * (180 / Math.PI));
                    
                    // Do a cross product to determine which side of the heading the agent is on.
                    // If the z component of the cross product is positive, the agent is closer to the right then the left.
                    // We adjust the degrees then to make possible for the degrees to have a range of 0-359. 
                    if (Vector3.Cross(new Vector3(heading, 0), new Vector3(agentVector, 0)).Z > 0)//Vector2.Dot(distance, normal) > )
                    {
                        angle = 360 - angle;
                    }

                    // Set which pie slice the agent is in based on angle.
                    if (angle < 45 || angle > 315)
                    {
                        quadrantActication[0]++;
                    }
                    else if (angle >= 45 && angle < 135)
                    {
                        quadrantActication[1]++;
                    }
                    else if (angle >= 135 && angle < 225)
                    {
                        quadrantActication[2]++;
                    }
                    else
                    {
                        quadrantActication[3]++;
                    }
                }
            }
            return quadrantActication;
        }
        
        public int[]  Update(Vector2 heading)
        {
            int[] nearAgents = GetCloseAgents(heading);

            slices.Position = attachedTo.Position;
            slices.Rotation = attachedTo.Rotation;

            return nearAgents;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(sprite.Texture, 
                            Globals.map.ScreenFromPhysics(slices.Position), null,
                            color, slices.Rotation, sprite.Origin, 1f, 
                            SpriteEffects.None, 0f);

            for (int i = 0; i < 4; i++)
            {
                float degreeMod = (float)(Math.PI / 4 + (i * Math.PI / 2));
                batch.Draw(texture, Globals.map.ScreenFromPhysics(attachedTo.Position),
                            null, new Color(color.R, color.G, color.B, color.A * 3), 
                            slices.Rotation + degreeMod, Vector2.Zero, pieLength * (float)Math.Sqrt(8),
                            SpriteEffects.None, 0f);
            }
        }

        internal void ClearCollisions()
        {
            collided.Clear();
        }
    }
}
