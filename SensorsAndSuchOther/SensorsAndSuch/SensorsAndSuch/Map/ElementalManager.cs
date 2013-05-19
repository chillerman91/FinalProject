using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Content;
using SensorsAndSuch.Maps;
using SensorsAndSuch.Mobs;
using SensorsAndSuch.Texts;
using SensorsAndSuch.Extensions;
using FarseerPhysics.Factories;
using FarseerPhysics.SamplesFramework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

namespace SensorsAndSuch.Maps
{
    public partial class BaseElemental
    {

        public enum Terrain
        {
            Forest = 0,
            Mountain = 1,
            Swamp = 2,
            BattleForGE = 3
        }

        public static int NumOfElementals;
        public static BaseElemental[] elementals;
        static Terrain? currentTerrain = null;
        public static DumpsterElemental[] dumpers;
        public static void CreateElementals(List<BaseTile>[,] Grid)
        {
            elementals = new BaseElemental[NumOfElementals];
            dumpers = new DumpsterElemental[NumOfElementals];

            for (int i = 0; i < elementals.Length; i++)
            {
                elementals[i] = BaseElemental.GetRandElemental(Grid);
                dumpers[i] = new DumpsterElemental(elementals[i].X, elementals[i].Y, elementals[i]);
                elementals[i].dumper = dumpers[i];
            } 
        }

        public static void TakeTurn(int index, List<BaseTile>[,] Grid)
        {
            elementals[index].takeTurn(Grid);
            dumpers[index].takeTurn(Grid);
        }

        public static void DrawElementals(SpriteBatch batch)
        {
            for (int i = 0; elementals != null && i < elementals.Length; i++)
            {
                elementals[i].Draw(batch);
                dumpers[i].Draw(batch);
            }
        }

        public static BaseElemental GetRandElemental(List<BaseTile>[,] Grid)
        {
            ElementalCount++;

            Vector2 pos = Globals.map.GetRandomFreePos(column => column.Count == 2);
            int count = 0;
            while (GetHeightDiffrence(Grid, (int)pos.X, (int)pos.Y, 4) < 50 && count < 10)
            {
                pos = Globals.map.GetRandomFreePos(column => column.Count == 2);
                count++;
            }
            if (currentTerrain == null)
            {
                currentTerrain = (Terrain.Forest);// Globals.rand.Next(4);
            }
            int X = (int)pos.X;
            int Y = (int)pos.Y;

            if (X == 0) X++;
            if (X == Globals.map.MapWidth) X--;
            if (Y == 0) Y++;
            if (Y == Globals.map.MapHeight) Y--;

            switch (currentTerrain)
            {
                case Terrain.Forest:
                    {
                        if (Globals.rand.Next(100) < 60)
                            return new ForestElemental(X, Y);
                        if (Globals.rand.Next(100) < 25)
                            return new ShrubElemental(X, Y);
                        if (Globals.rand.Next(100) < 25)
                            return new WaterElemental(X, Y);
                        break;
                    }
                case Terrain.Mountain:
                    {
                        if (Globals.rand.Next(100) < 50)
                            return new FireElemental(X, Y);
                        if (Globals.rand.Next(100) < 25)
                            return new MagmaElemental(X, Y);
                        break;
                    }
                case Terrain.Swamp:
                    {
                        if (Globals.rand.Next(100) < 50)
                            return new DarkElemental(X, Y);
                        if (Globals.rand.Next(100) < 50)
                            return new ShrubElemental(X, Y);
                        return new WaterElemental(X, Y);
                    }
                case Terrain.BattleForGE:
                    {
                        if (Globals.rand.Next(100) < 50)
                            return new DarkElemental(X, Y);
                        if (Globals.rand.Next(100) < 90)
                            return new LightElemental(X, Y);
                        break;
                    }

            }
            return new DirtElemental(X, Y);
        }
    }
}
