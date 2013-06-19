/* ***************************************************************************
 * This file is part of SharpNEAT - Evolution of Neural Networks.
 * 
 * Copyright 2004-2006, 2009-2010 Colin Green (sharpneat@gmail.com)
 *
 * SharpNEAT is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * SharpNEAT is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with SharpNEAT.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using SharpNeat.Core;
using SharpNeat.DistanceMetrics;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.SpeciationStrategies;
using SharpNeat.Utility;
using SharpNeat.Phenomes;
using SensorsAndSuch.NEAT;
using SharpNeat.Genomes.Neat;

namespace SharpNeat.EvolutionAlgorithms
{
    /// <summary>
    /// Implementation of the NEAT evolution algorithm. 
    /// Incorporates:
    ///     - Speciation with fitness sharing.
    ///     - Creating offspring via both sexual and asexual reproduction.
    /// </summary>
    /// <typeparam name="TGenome">The genome type that the algorithm will operate on.</typeparam>
    public class BadGuyEvolutionAlgorithm<TGenome> : NeatEvolutionAlgorithm<TGenome>
        where TGenome : class, IGenome<TGenome>
    {
        List<TGenome> offspringList;
        bool emptySpeciesFlag;
        ulong GenCount = 0;

        public List<TGenome> GenomeList { get { return _genomeList; } }
        public NeatGenomeFactory GenomeFactory { get { return (NeatGenomeFactory) _genomeFactory; } }

        #region Constructors

        /// <summary>
        /// Constructs with the default NeatEvolutionAlgorithmParameters and speciation strategy 
        /// (KMeansClusteringStrategy with ManhattanDistanceMetric).
        /// </summary>
        public BadGuyEvolutionAlgorithm()
            : base()
        {
        }

        /// <summary>
        /// Constructs with the provided NeatEvolutionAlgorithmParameters and ISpeciationStrategy.
        /// </summary>
        public BadGuyEvolutionAlgorithm(NeatEvolutionAlgorithmParameters eaParams,
                                      ISpeciationStrategy<TGenome> speciationStrategy,
                                      IComplexityRegulationStrategy complexityRegulationStrategy)
            : base(eaParams, speciationStrategy, complexityRegulationStrategy)
        { }

        #endregion

        /// <summary>
        /// To run after loading a generation from a file
        /// Sorts the genomes and sets diffrent values for play
        /// </summary>>
        public bool PostLoadFileInitilazation(List<TGenome> genomes)
        {
            _genomeList = genomes;
            // Clear all genomes from species (we still have the elite genomes in _genomeList).
            ClearAllSpecies();

            // Speciate genomeList.
            _speciationStrategy.SpeciateGenomes(_genomeList, _specieList);

            return true;
        }


        /// <summary>
        /// Progress forward by one generation. Perform one generation/iteration of the evolution algorithm.
        /// </summary>
        public List<TGenome> GetUpdatedGeneration()
        {
            // Calculate statistics for each specie (mean fitness, target size, number of offspring to produce etc.)
            int offspringCount;
            SpecieStats[] specieStatsArr = CalcSpecieStats(out offspringCount);

            // Create offspring.
            offspringList = CreateOffspring(specieStatsArr, offspringCount);

            // Trim species back to their elite genomes.
            emptySpeciesFlag = TrimSpeciesBackToElite(specieStatsArr);

            // Rebuild _genomeList. It will now contain just themc elite genomes.
            RebuildGenomeList();

            // Append offspring genomes to the elite genomes in _genomeList. We do this before calling the
            // _genomeListEvaluator.Evaluate because some evaluation schemes re-evaluate the elite genomes 
            // (otherwise we could just evaluate offspringList).
            _genomeList.AddRange(offspringList);

            #region TRial
            //Don't evaluate here, evaluate in game's code
            //_genomeListEvaluator.Evaluate(_genomeList);
            // Integrate offspring into species.
            if (emptySpeciesFlag)
            {
                // We have one or more terminated species. Therefore we need to fully re-speciate all genomes to divide them
                // evenly between the required number of species.

                // Clear all genomes from species (we still have the elite genomes in _genomeList).
                ClearAllSpecies();

                // Speciate genomeList.
                _speciationStrategy.SpeciateGenomes(_genomeList, _specieList);
            }
            else
            {
                // Integrate offspring into the existing species. 
                _speciationStrategy.SpeciateOffspring(offspringList, _specieList);
            }
            Debug.Assert(!TestForEmptySpecies(_specieList), "Speciation resulted in one or more empty species.");


            #endregion
            OnUpdateEvent();
            return _genomeList;

        }

        /// <summary>
        /// Progress forward by one generation. Perform one generation/iteration of the evolution algorithm.
        /// </summary>
        public void EvaluateGeneration()
        {

            _currentGeneration++;
            // Sort the genomes in each specie. Fittest first (secondary sort - youngest first).
            SortSpecieGenomes();

            // Update stats and store reference to best genome.
            UpdateBestGenome();
            UpdateStats();

            // Determine the complexity regulation mode and switch over to the appropriate set of evolution
            // algorithm parameters. Also notify the genome factory to allow it to modify how it creates genomes
            // (e.g. reduce or disable additive mutations).
            _complexityRegulationMode = _complexityRegulationStrategy.DetermineMode(_stats);
            _genomeFactory.SearchMode = (int)_complexityRegulationMode;
            switch (_complexityRegulationMode)
            {
                case ComplexityRegulationMode.Complexifying:
                    _eaParams = _eaParamsComplexifying;
                    break;
                case ComplexityRegulationMode.Simplifying:
                    _eaParams = _eaParamsSimplifying;
                    break;
            }
            GenCount++;
            // TODO: More checks.
            Debug.Assert(_genomeList.Count == _populationSize);
            return;
            //Don't evaluate here, evaluate in game's code
            //_genomeListEvaluator.Evaluate(_genomeList);
            // Integrate offspring into species.
            if (emptySpeciesFlag)
            {
                // We have one or more terminated species. Therefore we need to fully re-speciate all genomes to divide them
                // evenly between the required number of species.

                // Clear all genomes from species (we still have the elite genomes in _genomeList).
                ClearAllSpecies();

                // Speciate genomeList.
                _speciationStrategy.SpeciateGenomes(_genomeList, _specieList);
            }
            else
            {
                // Integrate offspring into the existing species. 
                _speciationStrategy.SpeciateOffspring(offspringList, _specieList);
            }
            Debug.Assert(!TestForEmptySpecies(_specieList), "Speciation resulted in one or more empty species.");

            // Sort the genomes in each specie. Fittest first (secondary sort - youngest first).
            SortSpecieGenomes();

            // Update stats and store reference to best genome.
            UpdateBestGenome();
            UpdateStats();

            // Determine the complexity regulation mode and switch over to the appropriate set of evolution
            // algorithm parameters. Also notify the genome factory to allow it to modify how it creates genomes
            // (e.g. reduce or disable additive mutations).
            _complexityRegulationMode = _complexityRegulationStrategy.DetermineMode(_stats);
            _genomeFactory.SearchMode = (int)_complexityRegulationMode;
            switch (_complexityRegulationMode)
            {
                case ComplexityRegulationMode.Complexifying:
                    _eaParams = _eaParamsComplexifying;
                    break;
                case ComplexityRegulationMode.Simplifying:
                    _eaParams = _eaParamsSimplifying;
                    break;
            }
            GenCount++;
            // TODO: More checks.
            Debug.Assert(_genomeList.Count == _populationSize);
        }

        /// <summary>
        /// Initializes the evolution algorithm with the provided IGenomeListEvaluator
        /// and an IGenomeFactory that can be used to create an initial population of genomes.
        /// </summary>
        /// <param name="genomeFactory">The factory that was used to create the genomeList and which is therefore referenced by the genomes.</param>
        /// <param name="populationSize">The number of genomes to create for the initial population.</param>
        public void Initialize(IGenomeFactory<TGenome> genomeFactory, List<TGenome> genomeList)
        {

            BaseInitialize(null, genomeFactory, genomeList);
            Initialize();
        }

        /// <summary>
        /// Code common to both public Initialize methods.
        /// </summary>
        new protected void Initialize()
        {
            // Evaluate the genomes.
            //_genomeListEvaluator.Evaluate(_genomeList);
            for (int i = 0; i < _genomeList.Count; i++)
            {
                _genomeList[i].EvaluationInfo.SetFitness(0);
            }

            // Speciate the genomes.
            _specieList = _speciationStrategy.InitializeSpeciation(_genomeList, _eaParams.SpecieCount);
            Debug.Assert(!TestForEmptySpecies(_specieList), "Speciation resulted in one or more empty species.");

            // Sort the genomes in each specie fittest first, secondary sort youngest first.
            SortSpecieGenomes();

            // Store ref to best genome.
            UpdateBestGenome();
        }


        /// <summary>
        /// Updates the NeatAlgorithmStats object.
        /// </summary>
        protected void UpdateStats()
        {
            _stats._generation = _currentGeneration;
            _stats._totalEvaluationCount = GenCount;

            // Evaluation per second.
            DateTime now = DateTime.Now;
            TimeSpan duration = now - _stats._evalsPerSecLastSampleTime;

            // To smooth out the evals per sec statistic we only update if at least 1 second has elapsed 
            // since it was last updated.
            if (duration.Ticks > 9999)
            {
                long evalsSinceLastUpdate = (long)(GenCount - _stats._evalsCountAtLastUpdate);
                _stats._evaluationsPerSec = (int)((evalsSinceLastUpdate * 1e7) / duration.Ticks);

                // Reset working variables.
                _stats._evalsCountAtLastUpdate = GenCount;
                _stats._evalsPerSecLastSampleTime = now;
            }

            // Fitness and complexity stats.
            double totalFitness = _genomeList[0].EvaluationInfo.Fitness;
            double totalComplexity = _genomeList[0].Complexity;
            double maxComplexity = totalComplexity;

            int count = _genomeList.Count;
            for (int i = 1; i < count; i++)
            {
                totalFitness += _genomeList[i].EvaluationInfo.Fitness;
                totalComplexity += _genomeList[i].Complexity;
                maxComplexity = Math.Max(maxComplexity, _genomeList[i].Complexity);
            }

            _stats._maxFitness = _currentBestGenome.EvaluationInfo.Fitness;
            _stats._meanFitness = totalFitness / count;

            _stats._maxComplexity = maxComplexity;
            _stats._meanComplexity = totalComplexity / count;

            // Specie champs mean fitness.
            double totalSpecieChampFitness = _specieList[0].GenomeList[0].EvaluationInfo.Fitness;
            int specieCount = _specieList.Count;
            for (int i = 1; i < specieCount; i++)
            {
                totalSpecieChampFitness += _specieList[i].GenomeList[0].EvaluationInfo.Fitness;
            }
            _stats._meanSpecieChampFitness = totalSpecieChampFitness / specieCount;

            // Moving averages.
            _stats._prevBestFitnessMA = _stats._bestFitnessMA.Mean;
            _stats._bestFitnessMA.Enqueue(_stats._maxFitness);

            _stats._prevMeanSpecieChampFitnessMA = _stats._meanSpecieChampFitnessMA.Mean;
            _stats._meanSpecieChampFitnessMA.Enqueue(_stats._meanSpecieChampFitness);

            _stats._prevComplexityMA = _stats._complexityMA.Mean;
            _stats._complexityMA.Enqueue(_stats._meanComplexity);
        }
    }
}
