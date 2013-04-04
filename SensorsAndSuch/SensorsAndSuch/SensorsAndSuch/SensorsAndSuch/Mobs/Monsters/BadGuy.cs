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

//Stone monster: can move walls
//water creature: becomes pools
//magician: teleport: time warp
//visible only when seen
// invisible: seen with dust
namespace SensorsAndSuch.Mobs
{
    public class BadGuy : BaseMonster
    {
        #region Datafields
        public Body circle;
        private FarseerPhysics.SamplesFramework.Sprite Sprite;

        protected static int wiskerNumber = 6;
        protected Wisker[] Wiskers = new Wisker[wiskerNumber];
        //protected //CircleSensor //CircleSensor;
        //protected PieSlice //PieSliceSensor;
        #endregion

        #region HW2
        public override bool SetPathway(int path)
        {
            currentArea = path;
            desiredEnd = Globals.Mobs.pathwayPts[path * 2 + 1];
            Scores = new float[MobManager.pathways];
            circle.Position = Globals.map.PhysicsFromGrid(Globals.Mobs.pathwayPts[currentArea * 2]);
            circle.Rotation = (float)Math.Atan2((double)(desiredEnd.Y - Globals.Mobs.pathwayPts[currentArea * 2].Y), (double)(desiredEnd.X - Globals.Mobs.pathwayPts[currentArea * 2].X));
            return true;
        }

        public override bool ChangePathway()
        {
            areasHit++;
            if (areasHit > MobManager.pathways)      
                return false;
            if (Scores == null)
                Scores = new float[MobManager.pathways];

            Scores[currentArea] = 30f - (desiredEnd - Globals.map.GridFromPhysics(circle.Position)).LengthSquared();
            currentArea = (currentArea + 1) % MobManager.pathways;
            desiredEnd = Globals.Mobs.pathwayPts[currentArea*2 + 1];

            circle.Position = Globals.map.PhysicsFromGrid(Globals.Mobs.pathwayPts[currentArea * 2]);
            circle.Rotation = (float)Math.Atan2((double)(desiredEnd.Y - Globals.Mobs.pathwayPts[currentArea * 2].Y), (double)(desiredEnd.X - Globals.Mobs.pathwayPts[currentArea * 2].X));
            return true;
        }

        #endregion
        
        public BadGuy(Vector2 GridPos, BaseMonster parent, int id, int age = 0, BodyType type = BodyType.Dynamic)
            : this(GridPos, Color.AntiqueWhite, id, age, type, circleRadius: .15f, parent: parent)
        {
        }
        public BadGuy(Vector2 GridPos, int id, int age = 0, BodyType type = BodyType.Dynamic)
            : this(GridPos, Color.ForestGreen, id, age, type, circleRadius: .15f, parent: null)
        {
        }
        public Vector2 StartPos;
        public bool FirstGen = false;
        int Birth = 0;
        static int LifeSpan = 20;
        public BadGuy(Vector2 GridPos, Color color, int id, int age, BodyType bodType, float circleRadius, BaseMonster parent)
            : base(null, GridPos, "Snake" + id, GetRandDir(), 15, 0, id)
        {
            circle = BodyFactory.CreateCircle(Globals.World, radius: circleRadius, density: 1f);
            Sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(circle.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                color, 1f));
            circle.LinearDamping = 3f;
            circle.AngularDamping = 3f;

            circle.BodyType = bodType;
            circle.Rotation = Globals.rand.Next(360);
            Vector2 pos = GridPos;
            circle.Position = new Vector2(pos.X * TileWidth, pos.Y * TileHeight);
            StartPos = new Vector2(pos.X * TileWidth, pos.Y * TileHeight);
            circle.Friction = 0.0f;
            
            Wiskers[0] = new Wisker(attatched: circle, offSet: 0, WiskerLength: 2f);
            Wiskers[1] = new Wisker(attatched: circle, offSet: (float)Math.PI / 2f, WiskerLength: 4f);
            Wiskers[2] = new Wisker(attatched: circle, offSet: (float)Math.PI / -2f, WiskerLength: 4f);
            Wiskers[3] = new Wisker(attatched: circle, offSet: (float)Math.PI / 4f, WiskerLength: 4f);
            Wiskers[4] = new Wisker(attatched: circle, offSet: (float)Math.PI / -4f, WiskerLength: 4f);
            Wiskers[5] = new Wisker(attatched: circle, offSet: (float)Math.PI, WiskerLength: 2f);

            circle.CollisionCategories = Category.Cat2;
            circle.CollidesWith = Category.Cat1;

            if (parent == null)
            {
                Birth = unchecked((int)DateTime.Now.Ticks) / (10000 * 1000);
                Brain = new Brain(inputs: Wiskers.Length + 2, outputs: 2, HiddenRows: 1, NodesPerRow: 10);
                FirstGen = true;
            }
            else
            {
                Birth = unchecked((int)DateTime.Now.Ticks) / (10000 * 1000);
                Brain = Brain.Clone(parent.Brain);
                Brain.Modify();
            }
        }
        public override void BackProp(float[] inputs, float[] output)
        {
            return;
            float[] ret = new float[3];
            for (int i = 0; i < Wiskers.Length; i++)
                ret[i] = (Wiskers[i].Update());
            if (FirstGen)
                base.BackProp(inputs: ret, output: output);
        }
        int kidNumber = -1;
        int Sterile = 5;
        int countWisLength = 1;
        float lengths = 0;
        public void MakeChildren(int numb)
        {
            for (int i = 0; i < numb; i++)
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, this, Globals.map.GetRandomFreePos());
        }

        public override void TakeTurn()
        {
            if ((Globals.map.GridFromPhysics(circle.Position) - desiredEnd).LengthSquared() < .1 )
                return;

            float[] ret = new float[Wiskers.Length+2];
            for (int i = 0; i < Wiskers.Length;i++ )
                ret[i] = (Wiskers[i].Update());
            Vector2 myPos = Globals.map.GridFromPhysics(circle.Position);
            Vector2 temp = (desiredEnd - myPos);
            temp.Normalize();
            ret[ret.Length - 2] = temp.X;
            ret[ret.Length - 1] = temp.Y;

            if (lengths / countWisLength < .2f)
                Brain.Modify(RowNumber: 100, PerRow: 1, ModAmount: 10);
            if (ret[0] < 0 || ret[1] < 0) 
                return;

            countWisLength++;
            lengths += Math.Min(1, ret[0]/2);
            Brain.Flush();
            ret = Brain.Calculate(ret);

            ret[0] = ret[0] * 2 - 1;
            ret[1] = ret[1] * 2 - 1;
            /*
            if (ret[0] == float.NaN || ret[0] == float.NaN)
                return;
            if (float.IsInfinity(ret[0]) || float.IsInfinity(ret[0]))
                return;*/

            if ((double)ret[0] == double.NaN || (double)ret[0] == double.NaN)
                return;

            if (ret[0] == 0)
            {
                ret[0] = .0001f;
            }
            circle.Rotation = (float)Math.Atan2((double)ret[1], (double)ret[0]) * .1f + circle.Rotation*.9f;
            Vector2 dir = circle.Rotation.GetVecFromAng();
            circle.ApplyForce(dir * speed, circle.Position);

        }
        public void TakeTurnOld()
        {
            float age = getAgeRatio();
            if (age > 1)
                Globals.Mobs.KillMonster(id);
            if (getAgeRatio() > .6)
            {
                if (kidNumber == -1)
                {
                    Globals.Mobs.KillMonster(id);
                }
                if (kidNumber > 0)
                {
                    kidNumber--;
                    Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, this, new Vector2(circle.Position.X / TileWidth, circle.Position.Y / TileHeight));
                }
                else 
                {
                    Globals.Mobs.KillMonster(id);
                }

                return;
            }
            float[] ret =  new float[3];
            ret[0] = Wiskers[0].Update();
            ret[1] = Wiskers[1].Update();
            ret[2] = Wiskers[2].Update();

            ret = Brain.Calculate(ret);

            circle.ApplyForce(circle.Rotation.GetVecFromAng() * Math.Min(ret[0] * speed, speed), circle.Position);
            ret[1] -= .5f;
            circle.ApplyAngularImpulse(ret[1] / 300f);
        }

        public float getAgeRatio()
        {
            return Math.Abs((float)(unchecked((int)DateTime.Now.Ticks) / (10000 * 1000) - Birth) / (float)LifeSpan) % 1.2f;
        }

        public override void Draw(SpriteBatch batch)
        {

            Wiskers[0].Draw(batch);

            batch.Draw(Sprite.Texture,
                               Globals.map.ScreenFromPhysics(circle.Position), null,
                               GetColor(), circle.Rotation, Sprite.Origin, Globals.map.globalScale * 1f,
                               SpriteEffects.None, 0f);
        }

        public override void Kill()
        {
            base.Delete();
            circle.Dispose();
        }

        public Color GetColor() 
        {
            float timeLeft = getAgeRatio();
            timeLeft = 1- Math.Min(1, timeLeft);
            timeLeft %= 1;
            return new Color(timeLeft, timeLeft, timeLeft);
        }
    }
}