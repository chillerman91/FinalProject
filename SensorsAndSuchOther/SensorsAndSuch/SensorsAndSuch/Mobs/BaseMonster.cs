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


namespace SensorsAndSuch.Mobs
{
    public abstract class BaseMonster : BaseTile, IComparable
    {
        public enum MonTypes
        {
            Normal, Static, Player
        }


        public abstract void Kill();
        public abstract void TakeTurn();
        public abstract void Draw(SpriteBatch batch);

        #region Set as live
        protected int Level = 1;
        protected int exp = 0;
        public int health = 100;
        protected int strength = 5;
        protected int hunger;
        public float Age = 0;
        public int kills = 0;
        #endregion

        #region Set At Birth
        public string Name;
        public MonTypes type;
        public MonTypes friendly;
        public bool Male;
        public int id;
        internal Body shape;
        #endregion

        #region HW2
        internal int areasHit = 0;
        internal int currentArea;
        internal float[] Scores;
        internal float Score;
        internal Vector2 desiredEnd;

        public virtual bool SetPathway(int path)
        {
            return false;
        }

        public virtual bool ChangePathway()
        {
            return false;
        }

        public virtual void SetScore()
        {
            Score = 0;
            int i = 1;
            for (i = 0; Scores != null && i < Scores.Length; i++)
                Score += Scores[i];
            Score /= (i +1);
        }

        public int CompareTo(Object obj)
        {
            BaseMonster other = obj as BaseMonster;
            if (other != null)
                return (int) ((other.Score - Score) * 100);
            else
                throw new ArgumentException("Object is not a BaseMonster");
        }

        #endregion

        public int MaxHealth = 20;
        //public int NutVal = 10;
        protected float speed = 1.2f;
        protected Vector2 Dir;
        protected Vector2 CurrentGridPos;
        internal Brain Brain;

        public BaseMonster(string tex, Vector2 GridPos, string Name, Vector2 moveDir, int NutVal, int Age, int id)
            :base (tex, GridPos)
        {
            this.Name = Name;
            this.Dir = moveDir;
            this.Age = Age;
            this.id = id;
        }

        public virtual void MakeChildren(int numb)
        {
            for (int i = 0; i < numb; i++)
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, this, Globals.map.GetRandomFreePos());
        }

        public virtual string GetInfo()
        {
            return "Strength(" + strength + ")\nHealth(" + health + ")\nAge(" + Age + ")";
        }

        public int GetLevel(){return Level;}

        public virtual void BackProp(float[] inputs, float[] output)
        {
            Brain.Flush();
            float[] ret1 = Brain.Calculate(inputs);
            float[] outers = Brain.BackProp(inputs, output);
            Brain.Flush();
            float[] ret2 = Brain.Calculate(inputs);
            for (int i = 0; i < ret1.Length; i++)
            {
                if (Math.Abs(ret1[i] - output[i]) <= Math.Abs(ret2[i] - output[i]))
                {
                    return;
                    throw new Exception("BackProp Gave bad change.");
                }
            }
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

        internal void DoDamage(int p)
        {
            health -= p;
        }
    }
}
