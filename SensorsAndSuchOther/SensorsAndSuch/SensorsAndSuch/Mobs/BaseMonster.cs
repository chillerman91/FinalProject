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


namespace SensorsAndSuch.Mobs
{
    public abstract class BaseMonster : BaseTile
    {
        public enum MonTypes
        {
            Normal, Static, Player
        }
        
        #region Set as live
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
        protected Vector2 CurrentGridPos;
        #endregion

        #region Set At Birth
        public string Name;
        public MonTypes type;
        public MonTypes friendly;
        public bool Male;
        public int id;
        internal Body shape;
        #endregion

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
        public abstract void ScoreSelf();

        internal void ResetGenome(NeatGenome neatGenome)
        {
            this.Genome = neatGenome;
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

        public BaseMonster(string tex, Vector2 GridPos, string Name, Vector2 moveDir, int NutVal, int Age, int id, NeatGenome Genome)
            :base (tex, GridPos)
        {
            this.Name = Name;
            this.Dir = moveDir;
            this.Age = Age;
            this.id = id;
            ResetGenome(Genome);
        }

        public virtual void MakeChildren(int numb)
        {
            if (Genome == null)
                return;
            for (int i = 0; i < numb; i++)
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, this, Globals.map.GetRandomFreePos());
        }

        public virtual string GetInfo()
        {
            return "Strength(" + strength + ")\nHealth(" + health + ")\nAge(" + Age + ")";
        }

        public int GetLevel(){ return Level; }

        public virtual void SetRandPos()
        {
            shape.Position = Globals.map.PhysicsFromGrid(Globals.map.GetRandomFreePos());
        }

        public abstract void SetRandPosSafe(int tick);

        internal void DoDamage(int p)
        {
            health -= p;
        }

        internal Vector2 GetPosition(float projected = 0, bool forceNonZero = false)
        {
            if (forceNonZero && Dir.Length() < .1f)
                return shape.Position + new Vector2(1, 0) * projected;
            return shape.Position + Dir * projected;
        }

        #region Static Methods/Properties
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
        }
        #endregion
    }
}
