using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using SensorsAndSuch.Maps;
using System;
using SensorsAndSuch.Extensions;
using SensorsAndSuch.Mobs.AI;
using FarseerPhysics.Dynamics;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SensorsAndSuch.Mobs.Sensors;


namespace SensorsAndSuch.Mobs
{
    public abstract class BaseMonster : BaseTile
    {
        public enum MonTypes
        {
            Reaper = 0, 
            Player = 1, 
            Killer = 1, 
            Rabbit = 2, 
            Max = 2
        }

        public abstract MonTypes monType { get; }
        public float monRatio { get { return (float)((float)monType / (float)MonTypes.Max); } }
        protected FarseerPhysics.SamplesFramework.Sprite Sprite;

        #region Set as live
        protected int deaths;
        protected int Level = 1;
        protected int exp = 0;
        public int health = 100;
        protected int strength = 5;
        protected int hunger;
        public float Age = 0;
        public int kills = 0;
        public int MaxHealth = 100;
        protected float speed = 1.2f;
        public Vector2 Dir;
        public Vector2 currentGridPos{ get{ return Globals.map.GridFromPhysics(Body.Position);} }
        #endregion

        #region Set At Birth
        public string Name;
        public MonTypes type;
        public MonTypes friendly;
        public bool Male;
        public int id{ get{ return Body.BodyId;} }
        internal Body Body;
        #endregion

        protected SensorBase[] Sensors;
        public bool viewDebuging = false;
        public abstract void Kill();
        public abstract void TakeTurn();
        public abstract void Draw(SpriteBatch batch);

        #region Parameters For NEAT
        internal float Score;
        internal IBlackBox Brain;
        internal NeatGenome Genome;
        #endregion

        #region Methods for NEAT

        internal void ResetGenome(NeatGenome neatGenome)
        {
            this.Genome = neatGenome;
            if (neatGenome == null) return;
            this.Brain = Globals.NeatExp.GetBlackBoxFromGenome(this.Genome);
            if (Genome != null)
            {
                if (Genome.SpecieIdx == 0)
                    color = Color.Brown;
                else if (Genome.SpecieIdx == 1)
                    color = Color.BlanchedAlmond;
                else if (Genome.SpecieIdx == 2)
                    color = Color.BlueViolet;
                else if (Genome.SpecieIdx == 3)
                    color = Color.CadetBlue;
                else if (Genome.SpecieIdx == 4)
                    color = Color.Firebrick;
                //color = new Color(Genome.SpecieIdx % 5f / 4f, Genome.SpecieIdx * 20 % 3f, 1 - Genome.SpecieIdx % 5f / 5f);
            }
        }
        #endregion
        #region Constructors

        public BaseMonster(Vector2 GridPos, Vector2 moveDir, NeatGenome Genome)
            :base (null, GridPos)
        {
            this.Dir = moveDir;
            this.Age = Age;
            ResetGenome(Genome);
        }

        #endregion

        public virtual string GetInfo()
        {
            return "Strength(" + strength + ")\nHealth(" + health + ")\nAge(" + Age + ")";
        }

        public int GetLevel(){ return Level; }

        public virtual void SetRandPos()
        {
            Body.Position = Globals.map.PhysicsFromGrid(Globals.map.GetRandomFreePos());
        }

        public abstract void SetRandPosSafe();

        internal void DoDamage(int p)
        {
            health -= p;
        }

        internal Vector2 GetPosition(float projected = 0, bool forceNonZero = false)
        {
            if (forceNonZero && Dir.Length() < .1f)
                return Body.Position + new Vector2(1, 0) * projected;
            return Body.Position + Dir * projected;
        }

        #region Static Methods/Properties
        /*
        public static Dictionary<int, BaseMonster> monsters = new Dictionary<int, BaseMonster>();

        public static void AddMonster(BaseMonster mon, int BodyID)
        {
            monsters.Add(BodyID, mon);
        }

        public static bool GetMonster(int BodyID, ref BaseMonster mon)
        {
            if (monsters.ContainsKey(BodyID))
            {
                mon = monsters[BodyID];
                return true;
            }
            return false;
        }*/
        #endregion
    }
}
