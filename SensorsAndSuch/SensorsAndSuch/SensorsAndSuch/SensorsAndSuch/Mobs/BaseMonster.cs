using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using SensorsAndSuch.Maps;
using System;
using SensorsAndSuch.Extensions;
using SensorsAndSuch.Mobs.AI;


namespace SensorsAndSuch.Mobs
{
    public abstract class BaseMonster : BaseTile, IComparable
    {
        //Neaural Var;
        //Lifspan
        //NutVal
        //seeable
        //desires
        public enum MonTypes
        {
            Normal, Static, Player
        }

        public abstract void Kill();
        public abstract void TakeTurn();
        public abstract void Draw(SpriteBatch batch);

        #region running states
        protected int Level = 1;
        protected int exp = 0;
        public int health = 20;
        protected int strength = 5;
        protected int hunger;
        public int Age = 0;
        public int kills = 0;
        #endregion

        #region Set At Level
        public string Name;
        public MonTypes type;
        public MonTypes friendly;
        public bool Male;
        public int id;
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

        public void MakeChildren(int numb)
        {
            for (int i = 0; i < numb; i++)
                Globals.Mobs.AddMonster(BaseMonster.MonTypes.Normal, this, Globals.map.GetRandomFreePos());
            //Globals.Mobs.KillMonster(id);
        }

        public virtual string GetInfo()
        {
            return "Strength(" + strength + ")\nHealth(" + health + ")\nAge(" + Age + ")";
        }

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
            for (int i = 0; i < Scores.Length; i++)
                Score += Scores[i];
            Score /= Scores.Length;
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

        public void flee() { Dir.Times(-1); }

        public bool SamePos(Vector2 otherPos)
        {
            return otherPos.VEquals(GridPos);
        }

        public int GetLevel(){return Level;}

        public int DoDamage(int dam, ref int exp)
        {
            Globals.map.GetTileColumn((int)GridPos.X, (int)GridPos.Y)[0].adjColor = Color.Red;
            if (health <= dam)
                exp += health;
            else
                exp += dam;

            health -= dam;
            if (health <= 0) {
                exp += dam * 2;
                Globals.Mobs.KillMonster(id);
                return 1 * Level;
            }
            return 0;
        }

        protected bool AttackPos(Vector2 attackPos)
        {
            return true; // AttackMon(Globals.Mobs.GetMobAt(attackPos));
        }

        public virtual bool Listen(BaseMonster mon)
        {
            return false;
        }

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
    }
}
