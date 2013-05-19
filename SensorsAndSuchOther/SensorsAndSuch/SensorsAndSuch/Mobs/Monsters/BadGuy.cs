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
        protected FarseerPhysics.SamplesFramework.Sprite Sprite;

        #region properties For NN
        protected static int wiskerNumber = 4;
        protected Wisker[] Wiskers = new Wisker[wiskerNumber];
        protected AgentDataSensor AdjDataSensor;
        int countWisLength = 1;
        float lengths = 3;
        public Vector2 StartPos;
        public double energy = 10;
        #endregion


        int lastHealth = 0;
        internal static int TicksPerLife;
        int ticksAtLastWarp = 0;
        public bool FirstGen = false;
        int Birth = 0;
        float ScoreDist = 0;
        float MurderBonus = 0;
        static int LifeSpan = 20;

        public static int BrainInputs { get { return wiskerNumber + AgentDataSensor.TotalRetVales + 2/* For GPS*/ + 0/* For Energy level*/; } }

        Text info = new Text(Globals.content.Load<SpriteFont>("Fonts/buttonFont"), displayText: "", displayPosition: new Vector2(0, 0), displayColor: Color.White,
             outlineColor: Color.Black, isTextOutlined: true, alignment: SensorsAndSuch.Texts.Text.Alignment.None, displayArea: Rectangle.Empty);
        protected BaseWeapon weapon;

        #region Constructors
        public BadGuy(Vector2 GridPos, BaseMonster parent, int id, int age = 0)
            : this(GridPos, Color.AntiqueWhite, id, age, circleRadius: .15f, parent: parent, Genome: null)
        {
        }

        public BadGuy(Vector2 GridPos, int id, NeatGenome Genome, int age = 0)
            : this(GridPos, Color.White, id, age, circleRadius: .15f, parent: null, Genome: Genome)
        {
        }


        public BadGuy(Vector2 GridPos, Color color, int id, int age, float circleRadius, BaseMonster parent, NeatGenome Genome)
            : base(null, GridPos, "Snake" + id, GetRandDir(), 15, 0, id, Genome)
        {
            shape = BodyFactory.CreateCircle(Globals.World, radius: circleRadius, density: 1f);
            
            Sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.content.Load<Texture2D>("Mobs/BadGuy"));

            Score = 0;
            shape.LinearDamping = 3f;
            shape.AngularDamping = 3f;
            ticksAtLastWarp = 0;
            shape.BodyType = BodyType.Dynamic;
            Vector2 pos = GridPos;
            shape.Position = new Vector2(pos.X * TileWidth, pos.Y * TileHeight);
            shape.Rotation = Globals.rand.Next(360);
            shape.Friction = 0.0f;
            shape.CollidesWith = Category.Cat1;
            Wiskers[0] = new Wisker(attatched: shape, offSet: 0, WiskerLength: 2f);
            Wiskers[1] = new Wisker(attatched: shape, offSet: (float)Math.PI / 2.4f, WiskerLength: 4f);
            Wiskers[2] = new Wisker(attatched: shape, offSet: (float)Math.PI / -2.4f, WiskerLength: 4f);
            //Wiskers[3] = new Wisker(attatched: shape, offSet: (float)Math.PI / 4f, WiskerLength: 4f);
            //Wiskers[4] = new Wisker(attatched: shape, offSet: (float)Math.PI / -4f, WiskerLength: 4f);
            Wiskers[3] = new Wisker(attatched: shape, offSet: (float)Math.PI, WiskerLength: 2f);

            AdjDataSensor = new AgentDataSensor(shape, new Color(255, 100, 50, 100));


            shape.CollisionCategories = Category.Cat1;
            shape.CollidesWith = Category.Cat1;
            
            ResetNNEvaluators();
            AddMonster(this, shape.BodyId);
        } 

        #endregion

        #region Methods for NeuroEvolution

        public override void ScoreSelf()
        {
            double ScoreLen = lengths / countWisLength;
            double ScoreLenPercent = 100 * ScoreLen;
            ScoreDist += (StartPos - shape.Position).Length();
            if (ScoreDist < 3)
                MurderBonus = 0;
            double Score = Math.Pow(20*ScoreDist * ScoreLen + 100*MurderBonus * MurderBonus * MurderBonus, 1/5);

            Genome.EvaluationInfo.SetFitness(Math.Max(0, Score));
            Genome.EvaluationInfo.AuxFitnessArr = null;
            ResetNNEvaluators();//
        }

        public void ResetNNEvaluators()
        {
            //shape.Rotation = Globals.rand.Next(360);

            StartPos = shape.Position;
            ticksAtLastWarp = 0; 
            ScoreDist = 0;
            lengths = 3;
            energy = 10;
            MurderBonus = 1;
            countWisLength = 1;
        }
        #endregion

        //Resets the agents position randomly, without effecting the score
        public override void SetRandPosSafe(int tick)
        {
            ScoreDist += (StartPos - shape.Position).Length();

            shape.Position = Globals.map.PhysicsFromGrid(Globals.map.GetRandomFreePos(notNearPlayer: true));
            shape.LinearVelocity = new Vector2(0, 0);
            weapon.updatePosition();
            StartPos = shape.Position;
        }

        public override void TakeTurn()
        {
            Age = .5f;

            //Starting Checks 
            if (Age < 0) return;
            if (Globals.GamesStart && Age > .2f && weapon == null)
            {
                weapon = new Sword(this, Item.Materials.Iron);
                weapon.StartUse();
            }

            if (weapon != null)
            {
                int numKilled = weapon.Use();
                if (numKilled != 0 && (shape.Position- StartPos).LengthSquared() < 4)
                {
                    //MurderBonus *= MurderBonus;
                    MurderBonus += .2f * numKilled;
                    MurderBonus *= MurderBonus;
                    MurderBonus = Math.Min(MurderBonus, 6000);
                }
            }
                    
            // MurderBonus += AdjDataSensor.count; 

            if (health <= 0)
            {
                SetRandPosSafe(Globals.tick);
                health = MaxHealth;
            }
            if (health != lastHealth)
            {
                adjColor = Color.Red;
            }
            lastHealth = health;
            //Deactivated because Age isn't being used
            if (false && Globals.GamesStart && Age >= 1.0f)
            {
                MakeChildren(1);
                Globals.Mobs.KillMonster(id);
            }

            #region Brain calculations
            //Set up inputs
            Brain.ResetState();
            ISignalArray BrainIn = Brain.InputSignalArray;
            int brainInIndex = 0;
            for (brainInIndex = 0; brainInIndex  < Wiskers.Length; brainInIndex ++) {
                BrainIn[brainInIndex] = Wiskers[brainInIndex].Update();
            }
            float[] proximalAgents = AdjDataSensor.Update(shape.Rotation.GetVecFromAng());
            foreach (float proxAgent in proximalAgents)
            {
                BrainIn[brainInIndex] = proxAgent;
                brainInIndex++;
            }
            BrainIn[brainInIndex++] = shape.Position.X / RandomMap.mapWidthPhysics;

            BrainIn[brainInIndex++] = shape.Position.Y / RandomMap.mapHeightPhysics;
            //BrainIn[brainInIndex++] = energy / 10;
            //AdjDataGetter.Update(shape.Rotation.GetVecFromAng(), Wiskers.Length, BrainIn);
            if (BrainIn[0] < 0 || BrainIn[1] < 0)
                throw new Exception("Error in BrainInVAl");
            
            //run Brain Calculation
            Brain.Activate();
            ISignalArray BrainOut = Brain.OutputSignalArray;

            //set up outputs
            if ((double)BrainOut[0] == double.NaN || (double)BrainOut[0] == double.NaN)
                throw new Exception("Error in Brain Out Error");

            double ret = BrainOut[0];
            //BrainOut[0] *= 4;// BrainOut[0];

            //otherBrain = BrainOut[2];

            #endregion
            

            //Change average Wisker length
            countWisLength++;
            lengths += (float)(BrainIn[0] * BrainIn[0]);

            //Change Agent's Physics
            Vector2 Dir = shape.Rotation.GetVecFromAng();
            if (energy > 0)
            {
                if (energy >= BrainOut[1])
                {

                    energy -= BrainOut[1];
                    BrainOut[1] = BrainOut[1] * 2 - 1;
                    shape.ApplyForce(Dir * (float)BrainOut[1] * speed, shape.Position);
                }
                if (energy >= BrainOut[0])
                {
                    energy -= BrainOut[0];
                    BrainOut[0] *= BrainOut[0];
                    BrainOut[0] = BrainOut[0] * 2 - 1;
                    shape.Rotation += (float)(BrainOut[0] * Math.PI / 2f / 10);
                }

                if (energy >= BrainOut[2])
                {
                    energy -= BrainOut[2];
                    BrainOut[2] = BrainOut[2] * 2 - 1;
                    shape.ApplyForce(Dir.Flip() * (float)BrainOut[2], shape.Position);
                }
                if (energy < 1)
                {
                    //energy = -10;
                }
            }
            energy = Math.Min(energy + 1.5, 10);

        }

        //removes this agent if its behavior is clearily bad and replaces it with a fresh creature.
        protected void IdiotThreshold()
        {
        }

        //gets the age of the agent based on a range from 0 - 1
        public float getAgeRatio()
        {
            return Math.Abs((float)(unchecked((int)DateTime.Now.Ticks) / (10000 * 1000) - Birth) / (float)LifeSpan) % 1.2f;
        }

        //Returns a string describing the agent
        public override string GetInfo()
        {
            double ScoreLen = lengths / countWisLength;
            double ScoreLenPercent = 100 * ScoreLen;
            double ScoreDistT = ScoreDist + (StartPos - shape.Position).Length();
            double Score = ScoreLen * ScoreLen * 50 * ScoreDistT * ScoreDistT + MurderBonus;
            return String.Format("Speies({0:D})\nScoreLen({1:F2})\nScoreDist({2:F2})\nMurderBonus({3:F2})\nScore({4:F2})", Genome.SpecieIdx, ScoreLenPercent, ScoreDistT, MurderBonus, Score);
        }

        public override void Draw(SpriteBatch batch)
        {
            if (viewDebuging)
            {                
                for (int i = 0; i < Wiskers.Length; i++)
                    Wiskers[i].Draw(batch);
                info.ChangeText(GetInfo());
                info.Position = Globals.map.ScreenFromPhysics(shape.Position);
                info.Draw(batch);
                AdjDataSensor.Draw(batch);
            }
            //AdjDataGetter.Draw(batch);
            if (weapon != null) weapon.Draw(batch);
            batch.Draw(Sprite.Texture,
                               Globals.map.ScreenFromPhysics(shape.Position), null,
                               Globals.map.globalEffect(adjColor == null ? color : (Color)adjColor), shape.Rotation, Sprite.Origin, Globals.map.globalScale * .75f,
                               SpriteEffects.None, 0f);
            adjColor = null;
        }

        // Disposes needed variables for removing the agent
        public override void Kill()
        {
            base.Delete();
            shape.Dispose();
            AdjDataSensor.Kill();
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