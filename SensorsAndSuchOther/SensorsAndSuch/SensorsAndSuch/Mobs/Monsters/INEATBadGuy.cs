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
using SharpNeat.EvolutionAlgorithms;

namespace SensorsAndSuch.Mobs
{
    public abstract class NEATBadGuy : BaseMonster
    {
        public abstract int BrainInputs();
        public abstract int BrainOutputs();

        public abstract void ScoreSelf();
        public abstract void ResetNNEvaluators();
        protected NEATBadGuy(Vector2 GridPos, Vector2 dir, NeatGenome genome)
            : base(GridPos, dir, genome)
        { }
    }
}