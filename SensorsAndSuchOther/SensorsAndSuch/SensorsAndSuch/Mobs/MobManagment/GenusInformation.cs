using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Content;
using SensorsAndSuch.Maps;
using SensorsAndSuch.Texts;
using FarseerPhysics.Dynamics;
using SharpNeat.Genomes.Neat;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Domains;
using SharpNeatGUI;
using System.Windows.Forms;
using SharpNeat.Utility;
using SharpNeat.Core;

namespace SensorsAndSuch.Mobs
{
    public class GenusManager<genusType>
        where genusType: NEATBadGuy
    {
        public int popAmount;
        public int genNumb = 0;
        public int brainInputs;
        public int brainOutputs;
        public int ticksPerGeneration;
        public Dictionary<int, genusType> monsters = new Dictionary<int, genusType>();
        BadGuyEvolutionAlgorithm<NeatGenome> EvolutionAlgorithm;
        Func<NeatGenome, genusType> constructor;
        public GenusManager(int ticksPerGeneration, int popAmount, int brainInputs, int brainOutputs, Func<NeatGenome, genusType> constructor)
        {
            this.ticksPerGeneration = ticksPerGeneration;
            this.popAmount = popAmount;
            this.constructor = constructor;
            this.brainInputs = brainInputs;
            this.brainOutputs = brainOutputs;

        }

        public void Ininitilize()
        {
            List<NeatGenome> StartingBrains;
            EvolutionAlgorithm = Globals.NeatExp.CreateEvolutionAlgorithm(popAmount, brainInputs, brainOutputs);
            StartingBrains = EvolutionAlgorithm.GetUpdatedGeneration();
            for (int i = 0; i < popAmount; i++)
            {
                AddMonster(constructor(StartingBrains[i]));
            }
            if (Globals.Debugging)
            {
                ShowBestGenome();
                CreateFormOfBestAndMeanFiness();
            }
        }

        internal bool ShouldUpdate(int tick)
        {
            return tick % ticksPerGeneration == 0;
        }

        internal void UpdateGeneration()
        {
            foreach (KeyValuePair<int, genusType> keyVal in monsters)
            {
                genusType mon = keyVal.Value;
                mon.ScoreSelf();
            }
            EvolutionAlgorithm.EvaluateGeneration();

            List<NeatGenome> StartingBrains = EvolutionAlgorithm.GetUpdatedGeneration();
            int i = 0;
            foreach (KeyValuePair<int, genusType> keyVal in monsters)
            {
                genusType mon = keyVal.Value;
                mon.ResetGenome(StartingBrains[i++]);
            }
            genNumb++;
        }

        //Safely Warp all creatures
        internal void WarpCreatures()
        {
            foreach (KeyValuePair<int, genusType> keyVal in monsters)
            {
                genusType mon = keyVal.Value;
                mon.SetRandPosSafe();
           
            }
        }

        internal void TakeTurn()
        {
            foreach (KeyValuePair<int, genusType> keyVal in monsters)
            {
                genusType mon = keyVal.Value;
                mon.TakeTurn();
            }
        }

        public void AddMonster(genusType mon)
        {
            monsters.Add(mon.id, mon);
        }
        #region Getters
        public void GetMon(List<BaseMonster> monList, Func<BaseMonster, bool> Want)
        {

            foreach (KeyValuePair<int, genusType> keyVal in monsters) 
            {
                if (Want(keyVal.Value))
                    monList.Add(keyVal.Value);
            }
        }

        public bool GetMon(int id, ref BaseMonster mon)
        {
            if (monsters.ContainsKey(id))
            {
                mon = monsters[id];
                return true;
            }
            return false;
        }

        #endregion
        public void Draw(SpriteBatch batch)
        {
            foreach (KeyValuePair<int, genusType> keyVal in monsters)
            {
                BaseMonster mon = keyVal.Value;
                if ((Globals.player.GetPosition() - mon.GetPosition()).LengthSquared() < (Globals.player.range - .3) * (Globals.player.range - .3))
                    mon.Draw(batch);
            }
        }

        internal bool ContainsKey(int bodyId)
        {
            return monsters.ContainsKey(bodyId);
        }

        #region GUI Information
        GenomeForm _bestGenomeForm;
        List<TimeSeriesGraphForm> _timeSeriesGraphFormList = new List<TimeSeriesGraphForm>();

        /// <summary>Array of 'nice' colors for chart plots.</summary>
        Color[] _plotColorArr;

        private void ShowBestGenome()
        {
            AbstractGenomeView genomeView = new NeatGenomeView();
            if (null == genomeView)
            {
                return;
            }

            // Create form.
            _bestGenomeForm = new GenomeForm("Best Genome", genomeView, EvolutionAlgorithm);

            // Show the form.
            _bestGenomeForm.Show();
            _bestGenomeForm.RefreshView();
        }

        private void CreateFormOfBestAndMeanFiness()
        {
            // Create data sources.
            List<TimeSeriesDataSource> _dsList = new List<TimeSeriesDataSource>();


            _dsList.Add(new TimeSeriesDataSource("Best", TimeSeriesDataSource.DefaultHistoryLength, 0, System.Drawing.Color.Red, delegate()
            {
                return new Point2DDouble(EvolutionAlgorithm.CurrentGeneration, EvolutionAlgorithm.Statistics._maxFitness);
            }));

            _dsList.Add(new TimeSeriesDataSource("Mean", TimeSeriesDataSource.DefaultHistoryLength, 0, System.Drawing.Color.Black, delegate()
            {
                return new Point2DDouble(EvolutionAlgorithm.CurrentGeneration, EvolutionAlgorithm.Statistics._meanFitness);
            }));

            _dsList.Add(new TimeSeriesDataSource("Best (Moving Average)", TimeSeriesDataSource.DefaultHistoryLength, 0, System.Drawing.Color.Orange, delegate()
            {
                return new Point2DDouble(EvolutionAlgorithm.CurrentGeneration, EvolutionAlgorithm.Statistics._bestFitnessMA.Mean);
            }));

            // Create a data sources for any auxiliary fitness info.
            AuxFitnessInfo[] auxFitnessArr = EvolutionAlgorithm.CurrentChampGenome.EvaluationInfo.AuxFitnessArr;
            if (null != auxFitnessArr)
            {
                for (int i = 0; i < auxFitnessArr.Length; i++)
                {
                    // 'Capture' the value of i in a locally defined variable that has scope specific to each delegate creation (below). If capture 'i' instead then it will always have
                    // its last value in each delegate (which happens to be one past the end of the array).(i * 10) % 255, (i * 10) % 255, (i * 10) % 255)
                    int ii = i;
                    _dsList.Add(new TimeSeriesDataSource(EvolutionAlgorithm.CurrentChampGenome.EvaluationInfo.AuxFitnessArr[i]._name, TimeSeriesDataSource.DefaultHistoryLength, 0, new System.Drawing.Color(), delegate()
                    {
                        return new Point2DDouble(EvolutionAlgorithm.CurrentGeneration, EvolutionAlgorithm.CurrentChampGenome.EvaluationInfo.AuxFitnessArr[ii]._value);
                    }));
                }
            }

            // Create form.
            TimeSeriesGraphForm graphForm = new TimeSeriesGraphForm("Fitness (Best and Mean)", "Generation", "Fitness", string.Empty, _dsList.ToArray(), EvolutionAlgorithm);
            _timeSeriesGraphFormList.Add(graphForm);

            /*// Attach a event handler to update this main form when the graph form is closed.
            graphForm.FormClosed += new FormClosedEventHandler(delegate(object senderObj, FormClosedEventArgs eArgs)
            {
                _timeSeriesGraphFormList.Remove(senderObj as TimeSeriesGraphForm);
                fitnessBestMeansToolStripMenuItem.Enabled = true;
            });

            // Prevent creating more then one instance fo the form.
            fitnessBestMeansToolStripMenuItem.Enabled = false;
            */
            // Show the form.
            graphForm.Show();
        }  

        #endregion
    }
}