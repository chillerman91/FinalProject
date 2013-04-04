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

// Stone monster: can move walls
// water creature: becomes pools
// magician: teleport: time warp
// visible only when seen
// invisible: seen with dust
namespace SensorsAndSuch.Mobs
{
    public class CircleSensor
    {
        #region Datafields
        protected Body circle;
        private FarseerPhysics.SamplesFramework.Sprite Sprite;

        protected static int wiskerNumber = 3;
        protected Body attachedTo;
        protected float radius;
        protected Color color = Color.White;
        protected List<Fixture> collided;
        protected float WiskerR;
        public Texture2D texture { get; set; }
        #endregion

        public CircleSensor(Body attatched, Color color, float radius = 1f)
        {
            texture = Globals.content.Load<Texture2D>("Sensors/Wisker");
            color.A = 255 / 10;
            circle = BodyFactory.CreateCircle(Globals.World, radius: radius, density: 1f);
            Sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(circle.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.White, 1f));
            WiskerR = .16f;
            circle.FixtureList[0].IsSensor = true;
            circle.FixtureList[0].OnCollision += CollisionHandler;

            circle.FixtureList[0].OnSeparation += SeperationHandler;
            collided = new List<Fixture>();
            //circle.FixtureList[0].OnSeparation += this.GetCollisionHandler;

            circle.Position = attatched.Position;
            attachedTo = attatched;
            this.radius = radius;
        }

        public bool CollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact) {
            if (!collided.Contains(fixtureB) && !fixtureB.Body.Equals(attachedTo))
                collided.Add(fixtureB);
            return true;
        }

        public void SeperationHandler(Fixture fixtureA, Fixture fixtureB)
        {
            collided.Remove(fixtureB);
        }
	
        public List<Vector2> GetCloseColliders(Vector2 heading) 
        {
            List<Vector2> nearAgents = new List<Vector2>();
            heading.Normalize();
            //Vector2 normal = new Vector2(heading.Y, -1 * heading.X);

            //float  distSqrd = int.MaxValue;
            foreach(Fixture col in collided)
            {
                Vector2 agentVector = col.Body.Position - circle.Position;
                float distance = agentVector.Length();
                agentVector.Normalize();
                //distSqrd = (agentVector).LengthSquared();
                if (radius > distance)
                {
                    float angle = (float)(Math.Acos(Vector2.Dot(heading, agentVector)) * (180/Math.PI));
                    // Do a cross product to determine which side of the heading the agent is on.
                    // If the z component of the cross product is positive, the agent is closer to the right then the left.
                    // We adjust the degrees then to make possible for the degrees to have a range of 0-359. 
                    if (Vector3.Cross(new Vector3(heading, 0), new Vector3(agentVector, 0)).Z > 0)//Vector2.Dot(distance, normal) > )
                    {
                        angle = 360 - angle;
                    }
                    nearAgents.Add(new Vector2(distance, angle));
                }
            }
            return nearAgents;
        }

        public List<Vector2> Update(Vector2 heading)
        {
            List<Vector2> CloseColliders = GetCloseColliders(heading);
            //color = Color.White;

            //if (distSqrd != int.MaxValue)
            if (CloseColliders.Count > 0)
            {
                float minDist = int.MaxValue;
                foreach (Vector2 collider in CloseColliders)
                {
                    if (collider.X < minDist)
                    {
                        minDist = (collider.X / radius);//(radius * radius));
                    }
                }
                
                color = new Color(255- (int)(minDist * 255), 0, 0, 255 / 3);
            } 
            else
                color = new Color(0, 0, 0, 255 / 3);
            
            circle.Position = attachedTo.Position;

            return CloseColliders;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Sprite.Texture,
                               Globals.map.ScreenFromPhysics(circle.Position), null,
                               color, circle.Rotation, Sprite.Origin, 1f,
                               SpriteEffects.None, 0f);
            float dist;
            if (radius != 1)
            {
                
                foreach(Fixture col in collided)
                {
                    Vector2 temp = col.Body.Position - circle.Position;
                    dist = (temp).Length();
                    batch.Draw(texture, Globals.map.ScreenFromPhysics(attachedTo.Position), 
                            null, Color.Gainsboro, 
                            (float)Math.Atan2(col.Body.Position.Y - attachedTo.Position.Y,col.Body.Position.X - attachedTo.Position.X), 
                            new Vector2(texture.Width / 2, texture.Height / 2), (dist) / WiskerR,
                            SpriteEffects.None, 0f);
                }
            }
        }

        internal void ClearCollisions()
        {
            collided.Clear();
        }
    }
}