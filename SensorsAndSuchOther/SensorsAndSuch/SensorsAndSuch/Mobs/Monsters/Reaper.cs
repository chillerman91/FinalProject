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
using SensorsAndSuch.Items;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using FarseerPhysics.Dynamics.Contacts;
using SensorsAndSuch.Mobs.Sensors;

namespace SensorsAndSuch.Mobs
{
    public class Reaper : BaseMonster
    {
        private FarseerPhysics.SamplesFramework.Sprite Sprite;

        public float MaxSpeed = 20.0f;
        public bool Afraid = false;
        public float _speed { set { speed = value; } }
        #region properties For NN
        public static Player Player;
        protected static int wiskerNumber = 4;
        protected Wisker[] Wiskers = new Wisker[wiskerNumber];
        public override MonTypes monType { get { return MonTypes.Reaper; } }
        protected AgentDataSensor AdjDataSensor;
        int countWisLength = 1;
        float lengths = 3;
        public Vector2 StartPos;
        int lastHealth = 0;
        CircleSensor LargeCircle;
        #endregion
        public void SetPosition(Vector2 newPos)
        {
            Body.Position = newPos;
        }
        public bool Active;     
        Text info = new Text(Globals.content.Load<SpriteFont>("Fonts/buttonFont"), displayText: "", displayPosition: new Vector2(0, 0), displayColor: Color.White,
             outlineColor: Color.Black, isTextOutlined: true, alignment: SensorsAndSuch.Texts.Text.Alignment.None, displayArea: Rectangle.Empty);
        protected BaseWeapon weapon;

        #region Constructors

        public Reaper(Vector2 GridPos, int id)
            : base(GridPos, GetRandDir(), null)
        {
            Body = BodyFactory.CreateCircle(Globals.World, radius: .7f, density: .5f);
            
            Sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.content.Load<Texture2D>("Mobs/BadGuy"));
            color = Color.DarkRed;
            color.A = 100;
            Score = 0;
            Body.LinearDamping = 1f;
            Body.AngularDamping = 3f;
            Body.BodyType = BodyType.Dynamic;
            speed = 10.0f;
            Vector2 pos = GridPos;
            Body.Position = new Vector2(pos.X * TileWidth, pos.Y * TileHeight);
            Body.Rotation = Globals.rand.Next(360);
            Body.Friction = 0.0f;

            Body.CollisionCategories = Category.Cat3;
            Body.CollidesWith = Category.Cat3;
            LargeCircle = new CircleSensor(Body, Color.Gray, 2);
            Player = Globals.player;
            Body.FixtureList[0].OnCollision += CollisionHandler;

            Body.FixtureList[0].OnSeparation += SeperationHandler;
        } 

        #endregion

        public bool CollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            Globals.screen.GameEnd();
            return true;
        }

        public void SeperationHandler(Fixture fixtureA, Fixture fixtureB)
        {
            //collided.Remove(fixtureB);
        }
        //Resets the agents position randomly, without effecting the score
        public override void SetRandPosSafe()
        {
            Body.Position = Globals.map.PhysicsFromGrid(Globals.map.GetRandomFreePos());
            Body.LinearVelocity = new Vector2(0, 0);
            weapon.updatePosition();
            StartPos = Body.Position;
        }

        public void ScoreSelf()
        {
            throw new NotImplementedException();
        }

        public override void TakeTurn()
        {
            if (!Active) { speed = 0.01f; return; }
            speed = Math.Min(MaxSpeed, speed + .04f);
            //Set Rot as tward projected position
            float projectionAmount = (Player.GetPosition() - Body.Position).Length() / 5;
            if (Afraid)
            {
                Dir = Body.Position - Player.GetPosition(projectionAmount);
            }
            else
            {
                Dir = Player.GetPosition(projectionAmount) - Body.Position;
            }
            Dir.Normalize();
            Body.Rotation = (float) Math.Atan2((double) Dir.Y, (double) Dir.X);
            //Move forward
            //Change Agent's Physics
            //shape.Rotation += (float)(BrainOut[0] * Math.PI / 2f/10);
            //Vector2 dir = shape.Rotation.GetVecFromAng();
            //shape.ApplyForce(dir * (float)BrainIn[1], shape.Position);
            Body.ApplyForce(Dir * (float) Math.Pow(speed / MaxSpeed, 4) * MaxSpeed, Body.Position);
        }

        //Returns a string describing the agent
        public override string GetInfo()
        {
            return "";// String.Format("Speies({0:D})\nScoreLen({1:F2})\nScoreDist({2:F2})\nMurderBonus({3:F2})\nScore({4:F2})", Genome.SpecieIdx, ScoreLenPercent, ScoreDistT, MurderBonus, Score);
        }

        public override void Draw(SpriteBatch batch)
        {

            //LargeCircle.Draw(batch);Globals.map.globalEffect(adjColor == null ? color : (Color)adjColor)
            batch.Draw(Sprite.Texture,
                               Globals.map.ScreenFromPhysics(Body.Position), null,
                               color, Body.Rotation, Sprite.Origin, Globals.map.globalScale * 2f,
                               SpriteEffects.None, 0f);
            adjColor = null;
        }

        // Disposes needed variables for removing the agent
        public override void Kill()
        {
            base.Delete();
            Body.Dispose();
        }

        internal void SetVelZero()
        {
            Body.LinearVelocity = new Vector2(0, 0);
        }
    }
}