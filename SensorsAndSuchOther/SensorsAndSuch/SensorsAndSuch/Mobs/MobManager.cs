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
        Reaper Reaper;
        public static int MaxMonsters = 151;

        public static int mobGroups;
        Text info = new Text(Globals.content.Load<SpriteFont>("Fonts/buttonFont"), displayText: "", displayPosition: new Vector2(0, 0), displayColor: Color.White,
                     outlineColor: Color.Black, isTextOutlined: true, alignment: SensorsAndSuch.Texts.Text.Alignment.None, displayArea: Rectangle.Empty);
        public int Count = 0;

        public float totalRank = .2f;
        public float totalDist = .2f;
        public int distancesUsed = 0;
        public int counter = 0;

        public MobManager() 
        {
            Reaper = new Reaper(new Vector2(0, 0), 0);
            Monsters = new BaseMonster[MaxMonsters];
        }

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

        internal void SetMobsDebugging(bool setValue)
        {
            int i = 0;
            while (i < MaxMonsters && Monsters[i] != null)
            {
                Monsters[i].viewDebuging = setValue;
                i++;
            }
        }

        internal void ToggleMobsDebugging(List<BaseMonster> mon)
        {
            for (int i = 0; i < mon.Count; i++)
            {
                mon[i].viewDebuging = !mon[i].viewDebuging;
            }
        }

        //returns a list of Monsters in the set box
        //ScreenPos: the top left of the box in physics coordinates
        internal List<BaseMonster> GetMobsInBox(Vector2 topLeft, float width, float height)
        {
            int i = 0;
            List<BaseMonster> ret = new List<BaseMonster>();
            while (i < MaxMonsters && Monsters[i] != null)
            {
                Vector2 pos = Monsters[i].GetPosition() - topLeft;
                if (pos.X >= 0 && pos.Y >= 0 && pos.X < width && pos.Y < height)
                {
                    ret.Add(Monsters[i]);
                }
                i++;
            }
            return ret;
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
                if ((Globals.player.GetPosition() - Monsters[i].GetPosition()).LengthSquared() < (Globals.player.range - .3) * (Globals.player.range - .3))
                    Monsters[i].Draw(batch);
                i++;
            }

            if (Globals.GamplayScreen.currentState == Screens.Gameplay.ScreenState.Ghost)
            {
                Reaper.Draw(batch);
            }

            info.Draw(batch);
            Count = i - 1;
        }

        //All mobs take a turn
        public void UpdateMobs(int t = 0)
        {
            int i = mobGroups * t; 
            while (Globals.GamplayScreen.currentState != Screens.Gameplay.ScreenState.Ghost &&i < MaxMonsters && i < mobGroups*(t+1) && Monsters[i] != null)
            {
                Monsters[i].TakeTurn();
                i++;
            }
            if (Globals.GamplayScreen.currentState == Screens.Gameplay.ScreenState.Ghost)
            {
                Reaper.Active = true;
                Reaper.TakeTurn();
            }
        }

        internal void UpdateAnalyzeCreatures(Inputs.GameInput input)
        {
            int i = 0;
            List<BaseMonster> monEffected = new List<BaseMonster>();

            //Get List of creatures to effect
            while (i < MaxMonsters && Monsters[i] != null)
            {
                Vector2 monPos = Monsters[i].GetPosition();
                if ((monPos - Globals.map.PhysicsFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y)).Length() < .2)
                {
                    Monsters[i].adjColor = Color.Black;
                    monEffected.Add(Monsters[i]);
                }
                i++;
            }

            //On Left Click Toggle their debuggin status
            if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.PreviousMouseState.LeftButton == ButtonState.Released)
            {
                //reset monster on tile that is right clicked
                Globals.Mobs.ToggleMobsDebugging(monEffected);
            }

            //On Right button down, warp them(Will effect score)
            if (input.CurrentMouseState.RightButton == ButtonState.Pressed)
            {
                for (i = 0; i < monEffected.Count; i++)
                {
                    monEffected[i].SetRandPos();
                }
            }
        }

        //Safely Warp all creatures
        internal void WarpCreatures(int tick)
        {
            int i = 0;

            while (i < MaxMonsters && Monsters[i] != null)
            {
                Monsters[i].SetRandPosSafe(tick);
                i++;
            }
        }

        internal void setReaperStart()
        {
            Reaper.SetPosition(Globals.player.GetPosition(-2, true));
            Reaper._speed = .01f;
            Reaper.SetVelZero();
            if (Globals.player.range >= 100)
                Reaper.Afraid = true;
        }
    }
}
