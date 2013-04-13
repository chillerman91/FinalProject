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

namespace SensorsAndSuch.Mobs
{

    public class MobManager
    {
        public BaseMonster[] Monsters;
        public static int pathways = 5;

        public List<Vector2> pathwayPts = new List<Vector2>();
        
        public static int MaxMonsters = 200;

        public static int mobGroups;
        /*Text info = new Text(Globals.content.Load<SpriteFont>("Fonts/buttonFont"), displayText: "", displayPosition: new Vector2(0, 0), displayColor: Color.White,
                     outlineColor: Color.Black, isTextOutlined: true, alignment: SensorsAndSuch.Texts.Text.Alignment.None, displayArea: Rectangle.Empty);*/
        public int Count = 0;


        public float totalRank = .2f;
        public float totalDist = .2f;
        public int distancesUsed = 0;
        public int counter = 0;

        public MobManager() 
        {
            Monsters = new BaseMonster[MaxMonsters];
        }
        /*
        public int Compare(Vector2 pos, float avgLength, int turns)
        {
            counter++;
            if (counter > 100) 
            {
                totalRank = 0;
                totalDist = 0;
                distancesUsed = 0;
                counter = 0;
            }
            float dist = pos.Length();
            totalDist += dist;
            distancesUsed++;
            float ret = dist / (totalDist / distancesUsed);
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                i++;
            }
            float rank = ret + avgLength*5;
            totalRank += rank;
            rank = rank / (totalRank / distancesUsed);
            //if (turns >= 0)
            //    return 0 ;
            if (rank > 1.3 && i < 120) rank *= 3;
            if (rank > 1.5 && i < 120) rank *= 3;
            if (i > 50) return 0;
            if (i < 20) 
                AddMonster(SensorsAndSuch.Mobs.BaseMonster.MonTypes.Normal, Globals.map.GetRandomFreePos());
            return Math.Min((int)rank, 3);
        }
        */
        #region Adding things to the map

        public bool AddPlayer(Player player)
        {
            int i = GetMobAmount();
            Monsters[i] = player;
            return true;
        }

        public bool AddMonster(BaseMonster.MonTypes monType, Vector2 gridPos, NeatGenome genome)
        {
            int i = GetMobAmount();
            if (i >= MaxMonsters) return false;
            if (monType == BaseMonster.MonTypes.Normal)
                Monsters[i] = new BadGuy(gridPos, i, genome);
            return true;
        }

        internal bool AddMonster(BaseMonster.MonTypes monTypes, BaseMonster badGuy, Vector2 vector2)
        {
            int i = GetMobAmount();
            if (i >= MaxMonsters)
                return false;
            Monsters[i] = new BadGuy(vector2, badGuy, i);
            return true;
        }

        
        public bool AddMonster()
        {
            throw new NotImplementedException();
        }

        internal void AddMonster(BaseMonster mon1, BaseMonster mon2)
        {

            throw new NotImplementedException();
        }

        #endregion

        #region Getters and Setters

        public BaseMonster GetMobAt(int id)
        {
            return Monsters[id];
        }

        internal int GetMobAmount()
        {
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                i++;
            }
            return i;
        }

        #endregion

        public bool KillMonster(int i)
        {
            if (Monsters[i] == null)
                return false;
            Monsters[i].Kill();
            Monsters[i] = null;
            while (i + 1 < MaxMonsters && Monsters[i + 1] != null)
            {
                Monsters[i] = Monsters[i + 1];
                Monsters[i].id = i;

                i++;
            }
            Monsters[i] = null;
            if (i >= MaxMonsters) return false;
            return true;
        }

        public void Draw(SpriteBatch batch) {
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                Monsters[i].Draw(batch);
                i++;
            }
            //info.Draw(batch);
            Count = i - 1;
        }
        public void UpdateMobs(int t = 0)
        {
            int i = mobGroups * t; 
            while (i < MaxMonsters && i < mobGroups*(t+1) && Monsters[i] != null)
            {
                Monsters[i].TakeTurn();
                i++;
            }
        }


        internal void Update(Inputs.GameInput input)
        {
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                Vector2 tran = Globals.map.ScreenFromPhysics(Monsters[i].GridPos);
                //if (input.CheckMouseOver(tran, BaseTile.TileWidth, BaseTile.TileHeight))
                //{
                    //info.ChangeText(Monsters[i].GetInfo());
                    //info.Position = new Vector2(tran.X + BaseTile.TileWidth,tran.Y + BaseTile.TileHeight);
                   // return;
               // }
                i++;
            }
            //info.ChangeText("");
        }

        internal void RunBackProp(float[] inputs, float[] output)
        {
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                Monsters[i].BackProp(inputs, output);
                i++;
            }
        }
    }
}
