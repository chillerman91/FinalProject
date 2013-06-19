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
using SensorsAndSuch.Mobs.AI;
using SensorsAndSuch.Items;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SensorsAndSuch.Extensions;
using SensorsAndSuch.Mobs.Sensors;

//Stone monster: can move walls
//water creature: becomes pools
//magician: teleport: time warp
//visible only when seen
// invisible: seen with dust

namespace SensorsAndSuch.Mobs
{
    public class Killer : NEATBadGuy
    {

        #region properties For NN
        protected static int wiskerNumber = 4;
        protected Wisker[] Wiskers = new Wisker[wiskerNumber];
        protected AgentDataSensor AdjDataSensor;
        int countWisLength = 1;
        float lengths = 3;
        public Vector2 StartPos;
        public double energy = 10;
        float MurderBonus = 0;
        float AdjCreatures = 0;
        #endregion

        int lastHealth = 0;
        internal static int TicksPerLife;
        public bool FirstGen = false;
        int Birth = 0;
        int Deaths = 0;
        float ScoreDist = 0;
        static int LifeSpan = 20;
        static int MaxEnergy = 100;
        public static int brainInputs { get { return wiskerNumber + AgentDataSensor.TotalRetVales + 2/* For GPS*/; } }
        public static int brainOutputs { get { return 3; } }
        public override int BrainInputs() { return brainInputs; }
        public override int BrainOutputs() { return brainOutputs; }
        public override MonTypes monType { get { return MonTypes.Killer; } }
        protected float radius;
        Text info = new Text(Globals.content.Load<SpriteFont>("Fonts/buttonFont"), displayText: "", displayPosition: new Vector2(0, 0), displayColor: Color.White,
             outlineColor: Color.Black, isTextOutlined: true, alignment: SensorsAndSuch.Texts.Text.Alignment.None, displayArea: Rectangle.Empty);
        protected BaseWeapon weapon;

        #region Constructors

        public Killer(NeatGenome Genome = null)
            : this(Globals.map.GetRandomFreePos(), Genome)
        { }

        public Killer(Vector2 GridPos, NeatGenome Genome)
            : this(GridPos, Color.White, circleRadius: .15f, Genome: Genome)
        { }

        protected Killer(Vector2 GridPos, Color color, float circleRadius, NeatGenome Genome)
            : base(GridPos, GetRandDir(), Genome)
        {
            Body = BodyFactory.CreateCircle(Globals.World, radius: circleRadius, density: 1f);
           
            Sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.content.Load<Texture2D>("Mobs/BadGuy"));
            radius = circleRadius;
            Score = 0;
            Body.LinearDamping = 3f;
            Body.AngularDamping = 3f;
            Body.BodyType = BodyType.Dynamic;
            Vector2 pos = GridPos;
            Body.Position = new Vector2(pos.X * TileWidth, pos.Y * TileHeight);
            Body.Rotation = Globals.rand.Next(360);
            Body.Friction = 0.0f;
            Body.CollidesWith = Category.Cat1;
            Sensors = new SensorBase[5];
            speed = .8f;
            int i = 0;
            Sensors[i++] = new Wisker(attatched: Body, offSet: 0, wiskerLengthGrid: 4f);
            Sensors[i++] = new Wisker(attatched: Body, offSet: (float)Math.PI / 2.4f, wiskerLengthGrid: 4f);
            Sensors[i++] = new Wisker(attatched: Body, offSet: (float)Math.PI / -2.4f, wiskerLengthGrid: 4f);
            Sensors[i++] = new Wisker(attatched: Body, offSet: (float)Math.PI, wiskerLengthGrid: 4f);
            //Wiskers[4] = new Wisker(attatched: shape, offSet: (float)Math.PI / -4f, WiskerLength: 4f);
            //Wiskers[3] = new Wisker(attatched: shape, offSet: (float)Math.PI, WiskerLength: 2f);

            AdjDataSensor = new AgentDataSensor(this, Body, new Color(255, 100, 50, 100));
            Sensors[i++] = AdjDataSensor;

            Body.CollisionCategories = Category.Cat1;
            Body.CollidesWith = Category.Cat1;
            
            ResetNNEvaluators();
        } 

        #endregion

        #region Methods for NeuroEvolution

        public override void ScoreSelf()
        {
            double ScoreLen = lengths / countWisLength;
            double ScoreLenPercent = 100 * ScoreLen;
            ScoreDist += (StartPos - Body.Position).Length();
            if (ScoreDist < 3)
               MurderBonus = 0;
            //double Score = Math.Pow(Math.Max(Math.Max(50 * ScoreDist * ScoreLen , 100 * MurderBonus * MurderBonus * MurderBonus ), Deaths * -100 + AdjCreatures * 5), 1 / 3);
            double Score = Math.Pow(150 * ScoreDist * ScoreLen * MurderBonus + 100 * MurderBonus * MurderBonus * MurderBonus - Deaths * 100 + AdjCreatures / 4, 1.0 / 2.5); // + AdjCreatures
            if (double.IsNaN(Score) || double.IsInfinity(Score))
                Score = 0;
            Genome.EvaluationInfo.SetFitness(Math.Max(0, Score));
            Genome.EvaluationInfo.AuxFitnessArr = null;
            ResetNNEvaluators();//
        }

        public override void ResetNNEvaluators()
        {
            AdjCreatures = 0;
            Deaths = 0;
            StartPos = Body.Position;
            ScoreDist = 0;
            lengths = 3;
            energy = 10;
            MurderBonus = 1;
            countWisLength = 1;
        }
        #endregion

        //Resets the agents position randomly, without effecting the score
        public override void SetRandPosSafe()
        {
            ScoreDist += (StartPos - Body.Position).Length();

            Body.Position = Globals.map.PhysicsFromGrid(Globals.map.GetRandomFreePos(notNearPlayer: true));
            Body.LinearVelocity = new Vector2(0, 0);
            if (weapon != null) 
                weapon.updatePosition();
            StartPos = Body.Position;
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
            AdjCreatures += AdjDataSensor.count;
            if (weapon != null)
            {
                int numKilled = weapon.Use();
                if (numKilled != 0 && (Body.Position- StartPos).LengthSquared() < 4)
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
                Deaths++;
                SetRandPosSafe();
                health = MaxHealth;
            }
            if (health != lastHealth)
            {
                adjColor = Color.Red;
            }
            lastHealth = health;

            for (int i = 0; i < Sensors.Length; i++)
            {
                Sensors[i].Update();
            }
            #region Brain calculations
            //Set up inputs
            Brain.ResetState();
            ISignalArray BrainIn = Brain.InputSignalArray;
            int brainInIndex = 0;

            for (int i = 0; i < Sensors.Length; i++)
            {
                float[] sensorRets = Sensors[i].GetReturnValues();
                for (int j = 0; j < sensorRets.Length; j++)
                    BrainIn[brainInIndex++] = sensorRets[j];
            }

            BrainIn[brainInIndex++] = Body.Position.X / RandomMap.mapWidthPhysics;

            BrainIn[brainInIndex++] = Body.Position.Y / RandomMap.mapHeightPhysics;
            //BrainIn[brainInIndex++] = energy / MaxEnergy;
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
            Vector2 Dir = Body.Rotation.GetVecFromAng();
            if (energy > 0)
            {

                BrainOut[0] = BrainOut[0] * 2 - 1;
                BrainOut[0] *= BrainOut[0] * BrainOut[0];

                BrainOut[2] = BrainOut[2] * 2 - 1;
                    BrainOut[1] = BrainOut[1] * 2 - 1;
                    if (energy >= Math.Abs(BrainOut[1]))
                {

                    energy -= Math.Abs(BrainOut[1]);
                    Body.ApplyForce(Dir * (float)BrainOut[1] * speed, Body.Position);
                }
                if (energy >= Math.Abs(BrainOut[0]))
                {
                    energy -= Math.Abs(BrainOut[0]);
                    Body.Rotation += (float)(BrainOut[0] * Math.PI / 2f / 10);
                }

                if (energy >= Math.Abs(BrainOut[2]))
                {
                    energy -= Math.Abs(BrainOut[2]);
                    Body.ApplyForce(Dir.Flip() * (float)BrainOut[2], Body.Position);
                }
                if (energy < 1)
                {
                    energy = -MaxEnergy;
                }
            }
            else if (energy + 1.5 >= 0)
            {
                energy = MaxEnergy / 2;
            }
            energy = 20;// Math.Min(energy + 20, MaxEnergy);
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
            double ScoreDistT = ScoreDist + (StartPos - Body.Position).Length();
            double Score = ScoreLen * ScoreLen * 50 * ScoreDistT * ScoreDistT + MurderBonus;
            return String.Format("Speies({0:D})\nScoreLen({1:F2})\nScoreDist({2:F2})\nMurderBonus({3:F2})\nScore({4:F2})", Genome.SpecieIdx, ScoreLenPercent, ScoreDistT, MurderBonus, Score);
        }

        public override void Draw(SpriteBatch batch)
        {   

            if (viewDebuging)
            {                    for (int i = 0; i < Sensors.Length; i++)
                    Sensors[i].Draw(batch);      

                info.ChangeText(GetInfo());
                info.Position = Globals.map.ScreenFromPhysics(Body.Position);
                info.Draw(batch);
                AdjDataSensor.Draw(batch);
            }
            //AdjDataGetter.Draw(batch);
            if (weapon != null) weapon.Draw(batch);
            batch.Draw(Sprite.Texture,
                               Globals.map.ScreenFromPhysics(Body.Position), null,
                               Globals.map.globalEffect(adjColor == null ? color : (Color)adjColor), Body.Rotation, Sprite.Origin, Globals.map.globalScale * .75f * radius/.15f,
                               SpriteEffects.None, 0f);
            adjColor = null;
        }

        // Disposes needed variables for removing the agent
        public override void Kill()
        {
            base.Delete();
            Body.Dispose();
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