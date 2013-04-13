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

//Stone monster: can move walls
//water creature: becomes pools
//magician: teleport: time warp
//visible only when seen
// invisible: seen with dust
namespace SensorsAndSuch.Mobs
{
    public class BadGuy : BaseMonster
    {
        private FarseerPhysics.SamplesFramework.Sprite Sprite;

        #region Parameters For NN
        protected static int wiskerNumber = 6;
        protected Wisker[] Wiskers = new Wisker[wiskerNumber];
        AgentDataSensor AdjDataGetter;
        int turns = 0;
        int countWisLength = 1;
        float lengths = 3;
        public Vector2 StartPos;
        #endregion

        public bool FirstGen = false;
        int Birth = 0;
        static int LifeSpan = 20;

        protected BaseWeapon weapon;

        #region Constructors
        public BadGuy(Vector2 GridPos, BaseMonster parent, int id, int age = 0, BodyType type = BodyType.Dynamic)
            : this(GridPos, Color.AntiqueWhite, id, age, type, circleRadius: .15f, parent: parent, Genome: null)
        {
        }

        public BadGuy(Vector2 GridPos, int id, NeatGenome Genome, int age = 0, BodyType type = BodyType.Dynamic)
            : this(GridPos, Color.ForestGreen, id, age, type, circleRadius: .15f, parent: null, Genome: Genome)
        {
        }


        public BadGuy(Vector2 GridPos, Color color, int id, int age, BodyType bodType, float circleRadius, BaseMonster parent, NeatGenome Genome)
            : base(null, GridPos, "Snake" + id, GetRandDir(), 15, 0, id, Genome)
        {
            shape = BodyFactory.CreateCircle(Globals.World, radius: circleRadius, density: 1f);
            Sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(shape.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                color, 1f));
            
            shape.LinearDamping = 3f;
            shape.AngularDamping = 3f;

            shape.BodyType = bodType;
            shape.Rotation = 0;// Globals.rand.Next(360);
            Vector2 pos = GridPos;
            shape.Position = new Vector2(pos.X * TileWidth, pos.Y * TileHeight);
            StartPos = new Vector2(pos.X * TileWidth, pos.Y * TileHeight);
            shape.Friction = 0.0f;

            Wiskers[0] = new Wisker(attatched: shape, offSet: 0, WiskerLength: 2f);
            Wiskers[1] = new Wisker(attatched: shape, offSet: (float)Math.PI / 2f, WiskerLength: 4f);
            Wiskers[2] = new Wisker(attatched: shape, offSet: (float)Math.PI / -2f, WiskerLength: 4f);
            Wiskers[3] = new Wisker(attatched: shape, offSet: (float)Math.PI / 4f, WiskerLength: 4f);
            Wiskers[4] = new Wisker(attatched: shape, offSet: (float)Math.PI / -4f, WiskerLength: 4f);
            Wiskers[5] = new Wisker(attatched: shape, offSet: (float)Math.PI, WiskerLength: 2f);

            AdjDataGetter = new AgentDataSensor(this.shape, new Color(255, 100, 50, 100));
            //shape.CollisionCategories = Category.Cat2;
            //shape.CollidesWith = Category.Cat1;
            Scores = new float[MobManager.pathways];
            for (int i = 0; i < MobManager.pathways; i++)
                Scores[i] = 0;
            
            if (parent == null)
            {
                Birth = unchecked((int)DateTime.Now.Ticks) / (10000 * 1000);// +Globals.rand.Next(30);
                //Brain = new NeatGenome();
                    //(inputs: Wiskers.Length + AgentDataSensor.TotalRetVales + 1 + 1, outputs: 3, HiddenRows: 1, NodesPerRow: 10);
                FirstGen = true;
            }
            else
            {
                Birth = unchecked((int)DateTime.Now.Ticks) / (10000 * 1000);
                this.Genome = parent.Genome.CreateOffspring(0);
                this.Brain = Globals.NeatExp.GetBlackBoxFromGenome(this.Genome);
                //Brain.Modify(2, 2, .5f);
            }
            AddMonster(this, shape.BodyId);
        } 

        #endregion

        #region Methods for NeuroEvolution

        public void ScoreSelf()
        {
            turns -= 60;
            turns = Math.Max(0, turns);
            Scores[currentArea] = lengths * 100 / countWisLength + (StartPos - shape.Position).LengthSquared()/4 + (50 - turns*turns)*.5f;
            //genome.EvaluationInfo.SetFitness(fitnessInfo._fitness);
            //genome.EvaluationInfo.AuxFitnessArr = fitnessInfo._auxFitnessArr;
        }

        public void ResetNNEvaluators()
        {
            StartPos = shape.Position;
            lengths = 3;
            countWisLength = 1;
            turns = 0;
        }

        public override bool SetPathway(int path)
        {
            for (int i = 0; i < MobManager.pathways; i++)
                Scores[i] = 0;
            SetRandPos();
            shape.Rotation = 0;// (float)Math.Atan2((double)(desiredEnd.Y - Globals.Mobs.pathwayPts[currentArea * 2].Y), (double)(desiredEnd.X - Globals.Mobs.pathwayPts[currentArea * 2].X));
            ResetNNEvaluators();
            areasHit = 0;
            return true;
        }

        public override bool ChangePathway()
        {
            areasHit++;
            if (areasHit > MobManager.pathways)
                throw new Exception("To many ChangePathway Calls");
            ScoreSelf();
            currentArea = (currentArea + 1) % MobManager.pathways;
            shape.Position = Globals.map.PhysicsFromGrid(Globals.map.GetRandomFreePos());
            ResetNNEvaluators();
            return true;
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
        #endregion

        public void SetRandPos()
        {
            shape.Position = Globals.map.PhysicsFromGrid(Globals.map.GetRandomFreePos());
        }

        public override void TakeTurn()
        {
            Age = getAgeRatio();
            //Starting Checks 
            if (Age < 0) return;
            if (Globals.GamesStart && Age > .2f && weapon == null)
                weapon = new Sword(this, Item.Materials.Iron);

            if (weapon != null)
            {
                lengths += 20 * weapon.StartUse();
            }

            if (health <= 0)
            {
                SetRandPos();
                health = MaxHealth;
            }           
            if (Globals.GamesStart && Age >= 1.0f)
            {
                //MakeChildren(Globals.Mobs.Compare(shape.Position, lengths, turns));
                //Globals.Mobs.KillMonster(id);
            }

            #region Brain calculations
            //Set up inputs
            Brain.ResetState();
            ISignalArray BrainIn = Brain.InputSignalArray;
            for (int i = 0; i < Wiskers.Length;i++ )
                BrainIn[i] = Wiskers[i].Update();
            //AdjDataGetter.Update(shape.Rotation.GetVecFromAng(), Wiskers.Length, BrainIn);
            if (BrainIn[0] < 0 || BrainIn[1] < 0)
                throw new Exception("Error in BrainInVAl");
            
            //run Brain Calculation
            Brain.Activate();
            ISignalArray BrainOut = Brain.OutputSignalArray;

            //set up outputs
            if ((double)BrainOut[0] == double.NaN || (double)BrainOut[0] == double.NaN)
                throw new Exception("Error in Brain Out Error");
            BrainOut[0] = BrainOut[0] * 2 - 1;
            //otherBrain = BrainOut[2];
            double ret = BrainOut[0];
            
            if (BrainOut[0] > .9f)
            {
                BrainOut[0] = 1f;
                turns++;
            }
            else if (BrainOut[0] < -.9f)
            {
                BrainOut[0] = -1f;
                turns++;
            }
            else
            {
                BrainOut[0] = 0f;
            }

            #endregion

            //Change average Wisker length
            countWisLength++;
            lengths += Math.Min(1, (float) BrainIn[0]);

            //Change Agent Physics
            shape.Rotation += (float)(BrainOut[0] * Math.PI / 2f);
            Vector2 dir = shape.Rotation.GetVecFromAng();
            shape.ApplyForce(dir * speed, shape.Position);

        }
        protected void IdiotThreshold()
        {
            if (turns > 200 || lengths / countWisLength < .1)
            {
                Globals.Mobs.KillMonster(id);              
                //Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, gridPos: Globals.map.GetRandomFreePos());
            }
        }
        //gets the age of the agent based on a range from 0 - 1
        public float getAgeRatio()
        {
            return Math.Abs((float)(unchecked((int)DateTime.Now.Ticks) / (10000 * 1000) - Birth) / (float)LifeSpan) % 1.2f;
        }

        public override void Draw(SpriteBatch batch)
        {
            Wiskers[0].Draw(batch);
            //AdjDataGetter.Draw(batch);
            if (weapon != null) weapon.Draw(batch);
            batch.Draw(Sprite.Texture,
                               Globals.map.ScreenFromPhysics(shape.Position), null,
                               GetAgeColor(), shape.Rotation, Sprite.Origin, Globals.map.globalScale * 1f,
                               SpriteEffects.None, 0f);
        }

        // Disposes needed variables for removing the agent
        public override void Kill()
        {
            base.Delete();
            shape.Dispose();
            AdjDataGetter.Kill();
            if (weapon != null)
                weapon.kill();
        }

        public Color GetAgeColor() 
        {
            float temp = (float)(int)(Age * 3)/3;
            return new Color(temp, temp, 1 - temp);
        }
    }
}