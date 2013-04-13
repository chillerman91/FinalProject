using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using SharpNeat.Genomes.Neat;
using SharpNeat.Decoders.Neat;
using SharpNeat.EvolutionAlgorithms;

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
        public override int InputCount
        {
            get { return 6; }
        }

        /// <summary>
        /// Defines the number of output nodes in the neural network.
        /// The network has one output for each square on the board,
        /// so it has 9 outputs total.
        /// </summary>
        public override int OutputCount
        {
            get { return 1; }
        }

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
        public void StartExp()
        {
            NeatEvolutionAlgorithm<NeatGenome> EA = CreateEvolutionAlgorithm();


        }

        public void GetGenomeList()
        { 
            
        }

        public void CreateNewGenomes()
        {

        }
        
        internal NeatGenome GetStartingGenome()
        {                    
            // Create a genome2 factory with our neat genome2 parameters object and the appropriate number of input and output neuron genes.
            IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();
            NeatGenome ret = null;
            while (ret == null)
            {
                ret = genomeFactory.CreateGenomeList(1, 0)[0];
            }
            // Create an initial population of randomly generated genomes.
            return ret; 
        }

        public List<NeatGenome> GetStartingGenomes(int populationSize)
        {
            // Create a genome2 factory with our neat genome2 parameters object and the appropriate number of input and output neuron genes.
            IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();

            // Create an initial population of randomly generated genomes.
            return genomeFactory.CreateGenomeList(populationSize, 0);
        }

        public IBlackBox GetStartingBlackBox(int populationSize)
        {
            IBlackBox ret = null;
            IGenomeDecoder<NeatGenome, IBlackBox> decoder = CreateGenomeDecoder();
            while (ret == null)
            {
                // Create a genome2 factory with our neat genome2 parameters object and the appropriate number of input and output neuron genes.
                IGenomeFactory<NeatGenome> genomeFactory = CreateGenomeFactory();

                // Create an initial population of randomly generated genomes.

                ret = decoder.Decode(genomeFactory.CreateGenomeList(populationSize, 0)[0]); ;
            }
            return ret;
        }

        public IBlackBox GetBlackBoxFromGenome(NeatGenome genome)
        {
            IBlackBox ret = null;
            if (genome == null) return null;
            IGenomeDecoder<NeatGenome, IBlackBox> decoder = CreateGenomeDecoder();
            while (ret == null)
            {
                ret = decoder.Decode(genome);
            }
            return ret;
        }
    }
}
