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
using SharpNeat.EvolutionAlgorithms;

namespace SensorsAndSuch.Mobs
{
    public class MobManager
    {
        public static int pathways = 5;

        Reaper Reaper;

        public int numGenuses = 2;
        public GenusManager<NEATBadGuy>[] Genuses = new GenusManager<NEATBadGuy>[2];
        public static int mobGroups;
        Text info = new Text(Globals.content.Load<SpriteFont>("Fonts/buttonFont"), displayText: "", displayPosition: new Vector2(0, 0), displayColor: Color.White,
                     outlineColor: Color.Black, isTextOutlined: true, alignment: SensorsAndSuch.Texts.Text.Alignment.None, displayArea: Rectangle.Empty);
        public int Count = 0;

        public float totalRank = .2f;
        public float totalDist = .2f;
        public int distancesUsed = 0;
        public int counter = 0;

        #region Population Amounts
        int genuses = 2;
        int rabbittPopAmount = 30;
        int killerPopAmount = 30;
        public int mobAmount { get { return killerPopAmount + rabbittPopAmount; } }
        #endregion

        BadGuyEvolutionAlgorithm<NeatGenome>[] EvolutionAlgorithms;

        public MobManager() 
        {
            Reaper = new Reaper(new Vector2(0, 0), 0);
        }

        #region Adding things to the map

        #endregion

        #region Getters and Setters
        /*
        internal void SetMobsDebugging(bool setValue)
        {
            int i = 0;
            foreach (KeyValuePair<int, BaseMonster> keyVal in monsters)
            {
                BaseMonster mon = keyVal.Value;
                mon.viewDebuging = setValue;
                i++;
            }
        }
        */
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
            List<BaseMonster> ret = new List<BaseMonster>();
            for(int i = 0; i < Genuses.Length; i++)
            {
                Genuses[i].GetMon(ret, mon =>
                {
                    Vector2 pos = mon.GetPosition() - topLeft;
                    return (pos.X >= 0 && pos.Y >= 0 && pos.X < width && pos.Y < height);
                });
            }
            return ret;
        }

        #endregion

        public void Draw(SpriteBatch batch) {
            for (int i = 0; i < Genuses.Length; i++)
            {
                Genuses[i].Draw(batch);
            }

            if (Globals.screen.currentState == Screens.PlayingScreen.WorldState.Ghost)
            {
                Reaper.Draw(batch);
            }

            info.Draw(batch);
        }

        //All mobs take a turn
        public void UpdateMobs(int tick)
        {

            if (Globals.screen.currentState != Screens.PlayingScreen.WorldState.Ghost)
            {

                Genuses[tick% Genuses.Length].TakeTurn();
                for (int i = 0; i < Genuses.Length; i++)
                {
                    if (Genuses[i].ShouldUpdate(tick))
                    {
                        Genuses[i].UpdateGeneration();
                        if (Globals.Debugging)
                        {
                            Genuses[i].WarpCreatures();
                        }
                    }
                }
            }
            if (Globals.screen.currentState == Screens.PlayingScreen.WorldState.Ghost)
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
            /*
            foreach (KeyValuePair<int, BaseMonster> keyVal in monsters)
            {
                BaseMonster mon = keyVal.Value;
                
                Vector2 monPos = mon.GetPosition();
                if ((monPos - Globals.map.PhysicsFromScreen(input.CurrentMouseState.X, input.CurrentMouseState.Y)).Length() < .2)
                {
                    mon.adjColor = Color.Black;
                    monEffected.Add(mon);
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
             * */
        }

        //Safely Warp all creatures
        internal void WarpCreatures(int tick)
        {

            for (int i = 0; i < Genuses.Length; i++)
            {
                Genuses[i].WarpCreatures();
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

        internal void InilitilizeGenuses()
        {
            #region Rabbits

            Genuses[0] = new GenusManager<NEATBadGuy>(500, rabbittPopAmount, Killer.brainInputs, Killer.brainOutputs, genome => new Killer(genome));
            Genuses[0].Ininitilize();
            #endregion

            #region Killers
            Genuses[1] = new GenusManager<NEATBadGuy>(1000, rabbittPopAmount, Killer.brainInputs, Killer.brainOutputs, genome => new Killer(genome));
            Genuses[1].Ininitilize();
            #endregion
        }

        internal void UpdateGeneration(int genusToUpdate)
        {
            Genuses[genusToUpdate].UpdateGeneration();
        }

        public bool GetMonster(int bodyId, ref BaseMonster mon)
        {
            if (Globals.player.id == bodyId)
            {
                mon = Globals.player;
                return true;
            }
            for (int i = 0; i < Genuses.Length; i++)
            {
                if (Genuses[i].GetMon(bodyId, ref mon))
                    return true;
            }
            return false;
        }

        internal bool ContainsKey(int bodyId)
        {
            if (Globals.player != null && Globals.player.id == bodyId)
            {
                return true;
            }
            for (int i = 0; i < Genuses.Length; i++)
            {
                if (Genuses[i] != null && Genuses[i].ContainsKey(bodyId))
                    return true;
            }
            return false;
        }
    }
}