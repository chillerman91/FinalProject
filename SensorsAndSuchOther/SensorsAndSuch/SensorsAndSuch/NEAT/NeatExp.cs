using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using SharpNeat.Genomes.Neat;
using SharpNeat.Decoders.Neat;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.SpeciationStrategies;
using SharpNeat.DistanceMetrics;
using SharpNeat.EvolutionAlgorithms.ComplexityRegulation;
using SharpNeat.Domains;
using SensorsAndSuch.Mobs;

namespace SensorsAndSuch.NEAT
{
    internal class NeatExp : SimpleNeatExperiment
    {
        /// <summary>
        /// Gets the evaluator that scores individuals.
        /// </summary>
        public override IPhenomeEvaluator<IBlackBox> PhenomeEvaluator
        {
            get { return null; }
        }

        /// <summary>
        /// Defines the number of input nodes in the neural network.
        /// The network has one input for each square on the board,
        /// so it has 9 inputs total.
        /// </summary>
        public int InputCount = -1;

        /// <summary>
        /// Defines the number of output nodes in the neural network.
        /// The network has one output for each square on the board,
        /// so it has 9 outputs total.
        /// </summary>
        public int OutputCount = -1;

        /// <summary>
        /// Defines whether all networks should be evaluated every
        /// generation, or only new (child) networks. For Tic-Tac-Toe,
        /// we're evolving against a random player, so it's a good
        /// idea to evaluate individuals again every generation,
        /// to make sure it wasn't just luck.
        /// </summary>
        public override bool EvaluateParents
        {
            get { return true; }
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload requires no parameters and uses the default population size.
        /// </summary>
        public BadGuyEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm()
        {
            return CreateEvolutionAlgorithm(DefaultPopulationSize, InputCount, OutputCount);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload accepts a population size parameter that specifies how many genomes to create in an initial randomly
        /// generated population.
        /// </summary>
        public BadGuyEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(int populationSize, int InputCount, int OutputCount)
        {
            this._inputCount = InputCount;
            this._outputCount = OutputCount;
            // Create a genome2 factory with our neat genome2 parameters object and the appropriate number of input and output neuron genes.
            IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();

            // Create an initial population of randomly generated genomes.
            List<NeatGenome> genomeList = genomeFactory.CreateGenomeList(populationSize, 0);

            // Create evolution algorithm.
            return CreateEvolutionAlgorithm(genomeFactory, genomeList);
        }

        /// <summary>
        /// Create and return a NeatEvolutionAlgorithm object ready for running the NEAT algorithm/search. Various sub-parts
        /// of the algorithm are also constructed and connected up.
        /// This overload accepts a pre-built genome2 population and their associated/parent genome2 factory.
        /// </summary>
        public BadGuyEvolutionAlgorithm<NeatGenome> CreateEvolutionAlgorithm(IGenomeFactory<NeatGenome> genomeFactory, List<NeatGenome> genomeList)
        {
            // Create distance metric. Mismatched genes have a fixed distance of 10; for matched genes the distance is their weigth difference.
            IDistanceMetric distanceMetric = new ManhattanDistanceMetric(1.0, 0.0, 5.0);
            ISpeciationStrategy<NeatGenome> speciationStrategy = new ParallelKMeansClusteringStrategy<NeatGenome>(distanceMetric, _parallelOptions);

            // Create complexity regulation strategy.
            IComplexityRegulationStrategy complexityRegulationStrategy = ExperimentUtils.CreateComplexityRegulationStrategy(_complexityRegulationStr, _complexityThreshold);

            // Create the evolution algorithm.
            BadGuyEvolutionAlgorithm<NeatGenome> ea = new BadGuyEvolutionAlgorithm<NeatGenome>(_eaParams, speciationStrategy, complexityRegulationStrategy);

            // Create genome2 decoder.
            //IGenomeDecoder<NeatGenome, IBlackBox> 
            genomeDecoder = new NeatGenomeDecoder(_activationScheme);

            // Create a genome2 list evaluator. This packages up the genome2 decoder with the genome2 evaluator.
            //IGenomeListEvaluator<NeatGenome> genomeListEvaluator = new ParallelGenomeListEvaluator<NeatGenome, IBlackBox>(genomeDecoder, PhenomeEvaluator, _parallelOptions);

            // Wrap the list evaluator in a 'selective' evaulator that will only evaluate new genomes. That is, we skip re-evaluating any genomes
            // that were in the population in previous generations (elite genomes). This is determiend by examining each genome2's evaluation info object.
            /*if (!EvaluateParents)
                genomeListEvaluator = new SelectiveGenomeListEvaluator<NeatGenome>(genomeListEvaluator,
                                         SelectiveGenomeListEvaluator<NeatGenome>.CreatePredicate_OnceOnly());
            */
            // Initialize the evolution algorithm.
            ea.Initialize(genomeFactory, genomeList);

            // Finished. Return the evolution algorithm
            return ea;
        }
        

        internal IBlackBox GetBlackBoxFromGenome(NeatGenome genome)
        {
            IBlackBox ret = null;
            if (genome == null) return null;
            while (ret == null)
            {
                ret = genomeDecoder.Decode(genome);
            }
            return ret;
        }
    }
}
