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

// Stone monster: can move walls
// water creature: becomes pools
// magician: teleport: time warp
// visible only when seen
// invisible: seen with dust

namespace SensorsAndSuch.Mobs
{
    public class Rabbitt : Killer
    {
        #region properties For NN
        int countWisLength = 1;
        float lengths = 3;
        public Vector2 StartPos;
        public double energy = 10;
        float MurderBonus = 0;
        float AdjCreatures = 0;
        #endregion

        public int lastHealth { get; set; }
        float ScoreDist = 0;
        public static int brainInputs { get { return wiskerNumber + AgentDataSensor.TotalRetVales + 2/* For GPS*/;} }
        public static int brainOutputs { get { return 3;} }
        public override int BrainInputs() { return brainInputs; }
        public override int BrainOutputs() { return brainOutputs; }
        public override MonTypes monType { get { return MonTypes.Rabbit; } }

        #region Constructors
        public Rabbitt(NeatGenome Genome = null)
            : this(Globals.map.GetRandomFreePos(), Genome)
        { }

        public Rabbitt(Vector2 GridPos, NeatGenome Genome)
            : this(GridPos, Color.White, circleRadius: .1f, parent: null, Genome: Genome)
        {
        }

        public Rabbitt(Vector2 GridPos, Color color, float circleRadius, BaseMonster parent, NeatGenome Genome)
            : base(GridPos, color, circleRadius, Genome)
        {
            speed = .5f;
        } 

        #endregion

        #region Methods for NeuroEvolution

        public override void ScoreSelf()
        {
            double ScoreLen = lengths / countWisLength;
            double ScoreLenPercent = 100 * ScoreLen;
            ScoreDist += (StartPos - Body.Position).Length();
            //if (ScoreLen < 2) AdjCreatures = 0;
            double Score = 50 * ScoreDist * ScoreLen + AdjCreatures / 2 - deaths * 200;
            if (double.IsNaN(Score) || double.IsInfinity(Score))
                Score = 0;
            Genome.EvaluationInfo.SetFitness(Math.Max(0, Score));
            Genome.EvaluationInfo.AuxFitnessArr = null;
            ResetNNEvaluators();
        }

        public void ResetNNEvaluators()
        {
            AdjCreatures = 0;
            deaths = 0;
            StartPos = Body.Position;
            ScoreDist = 0;
            lengths = 3;
            energy = 10;
            MurderBonus = 1;
            countWisLength = 1;
        }
        #endregion

        public override void TakeTurn()
        {
            if (health <= 0)
            {
                deaths++;
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

            #region Effect NN evaluators
            AdjCreatures += 1 - AdjDataSensor.shortestDist;
            
            //Change average Wisker length
            countWisLength++;
            lengths += (float)(BrainIn[0] * BrainIn[0]);
            #endregion

            #region Change Agent's Physics
            Vector2 Dir = Body.Rotation.GetVecFromAng();
            if (energy > 0)
            {
                if (energy >= 2 * BrainOut[0])
                {
                    BrainOut[0] = BrainOut[0] * 2 - 1;
                    BrainOut[0] *= BrainOut[0] * BrainOut[0];
                    Body.Rotation += (float)(BrainOut[0] * Math.PI / 2f / 10);
                }
                if (energy >= BrainOut[1])
                {
                    //BrainOut[1] = BrainOut[1] * 2 - 1;
                    Body.ApplyForce(Dir * (float)BrainOut[1] * speed, Body.Position);
                }

                if (energy >= BrainOut[2])
                {
                    BrainOut[2] = BrainOut[2] * 2 - 1;
                    Body.ApplyForce(Dir.Flip() * (float)BrainOut[2] * speed, Body.Position);
                }

            }
            #endregion
        }
    }
}