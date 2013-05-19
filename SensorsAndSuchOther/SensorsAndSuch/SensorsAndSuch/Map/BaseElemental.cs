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
        public static int AvgLife;
        protected int CentralX;
        protected int CentralY;

        public int X;
        public int Y;
        private Byte state;
        protected int forwardChance = 80;
        protected int leftChance = 10;
        protected int rightChance = 10;
        protected int backChance = 0;
        internal int age = 0;
        protected Vector2 dir;
        protected Color BaseColor;
        protected List<Func<Color>> Dislikes;
        protected List<Func<Color>> Likes;

        protected int DeleteWallThreshold = 2;
        protected int CreateWallThreshold = -2;
        protected bool CanMod = false;
        private static int ElementalCount = 0;
        protected List<Vector2> previousPlaces = new List<Vector2>();
        CircleSensor sensor;
        Color color;
        protected FarseerPhysics.Dynamics.Body shape;
        FarseerPhysics.SamplesFramework.Sprite Sprite;
        protected List<Fixture> collided;
        protected float density;
        protected float radius = 1f;
        protected Vector2 GridPos;
        protected byte Alpha;
        protected float speed = 5;
        protected int oldX, oldY;
        bool hasBlock = true;
        public DumpsterElemental dumper;
        protected bool NeedToWait = false;
        internal Vector2 position { get { return shape.Position; } }

        public BaseElemental(int X, int Y, Color col)
        {
            color = col;
            color.A = 0;
            shape = BodyFactory.CreateCircle(Globals.World, radius: radius, density: .25f);
            Sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(shape.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                col, 1f));
            shape.BodyType = BodyType.Dynamic;
            //shape.CollidesWith = Category.None;
            shape.LinearDamping = 5f;
            shape.AngularDamping = 3f;

            shape.FixtureList[0].IsSensor = true;
            shape.FixtureList[0].OnCollision += CollisionHandler;
            shape.FixtureList[0].OnSeparation += SeperationHandler;
            collided = new List<Fixture>();
            Dislikes = new List<Func<Color>>();
            Likes = new List<Func<Color>>();
            Likes.Add(() => Wall.GetColor());
            shape.Position = Globals.map.PhysicsFromGrid(new Vector2(X, Y));
            dir = BaseTile.GetRandDir();
            BaseColor = col; 
            this.X = X;
            this.Y = Y;
        }

        public bool CollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            if (!collided.Contains(fixtureB) && BaseMonster.monsters.ContainsKey(fixtureB.FixtureId))
                collided.Add(fixtureB);
            return true;
        }

        public void CheckCollisions()
        {
            int max = collided.Count;
            for (int i = 0; i < max; i++) 
            {
                float dist = (collided[i].Body.Position - shape.Position).Length();
                if (dist > radius*1.2)
                {
                    collided.RemoveAt(i);
                    //max--;
                    i= max;//--;
                }
            }
                
        }

        public void SeperationHandler(Fixture fixtureA, Fixture fixtureB)
        {
            if(collided.Contains(fixtureB))
                collided.Remove(fixtureB);
        }

        protected virtual void setPreferedDir(List<BaseTile>[,] Grid)
        {
            //Add options
            List<int> weights = new List<int>();
            List<Vector2> possibleDir = Globals.map.GetAdjGridPos(X, Y);
            if (possibleDir.Count == 0)
            {
                return;
            }
            for (int i = 0; i < possibleDir.Count; i++)
            {
                possibleDir[i] = possibleDir[i] - new Vector2(X, Y);
                if (possibleDir[i].VEquals(dir))
                    weights.Add(forwardChance);
                else if (possibleDir[i].VEquals(dir * -1))
                    weights.Add(backChance);
                else if (possibleDir[i].VEquals(dir.Flip()))
                    weights.Add(dir.X == 0 ? leftChance : rightChance);
                else if (possibleDir[i].VEquals(dir.Flip() * -1))
                    weights.Add(dir.Y == 0 ? leftChance : rightChance);
                else
                    throw new Exception("Elemental Bad dir");
            }

            CanMod = true;
            #region Adj Weights based on preferences
            for (int i = 0; i < possibleDir.Count; i++)
            {
                Color ColorAtPos = Grid[X + (int)possibleDir[i].X, Y + (int)possibleDir[i].Y]
                    [Grid[X + (int)possibleDir[i].X, Y + (int)possibleDir[i].Y].Count - 1].color;
                /*
                foreach (Func<Color> checker in Likes)
                {
                    int comp = checker().Compare(ColorAtPos);
                    if (comp < 70)
                    {
                        weights[i] += 100;
                    }
                }

                foreach (Func<Color> checker in Dislikes)
                {
                    int comp = checker().Compare(ColorAtPos);
                    if (comp < 70)
                    {
                        weights[i] = 0;
                    }
                }
                */
            }
            #endregion

            #region Choose Direction
            int randNumb = 0, sum = 0;
            for (int i = 0; i < possibleDir.Count; i++)
            {
                randNumb += weights[i];
            }
            if (randNumb == 0)
            {
                dir = possibleDir[Globals.rand.Next(possibleDir.Count - 1)];
                return;
            }
            randNumb = Globals.rand.Next(randNumb);
            for (int i = 0; i < possibleDir.Count; i++)
            {
                sum += weights[i];
                if (randNumb < sum)
                {
                    dir = possibleDir[i];
                    break;
                }
            }
            #endregion
        }

        public virtual void takeTurn(List<BaseTile>[,] Grid, bool toldCanModWalls = true)
        {

            //pick a direction

            if (collided.Count != 0) { 
                CheckCollisions(); 
                //return; 
            }
            if (!NeedToWait)
            {
                Vector2 GridPos = Globals.map.GridFromPhysics(shape.Position);
                X = (int)GridPos.X;
                Y = (int)GridPos.Y;
                if (oldX != X || oldY != Y)
                    setPreferedDir(Grid);
                shape.ApplyForce(dir * speed * (collided.Count+1), shape.Position);
            }
            //Effect it or notdir

            if (Globals.map.isInBounds(X + (int)dir.X, Y + (int)dir.Y, Offset: 1))
            {
                //Move there
                //shape.Rotation += (float)(BrainOut[0] * Math.PI / 2f / 10);
                //Vector2 dir = shape.Rotation.GetVecFromAng();
                //shape.ApplyForce(dir.Flip() * (float)BrainIn[2], shape.Position);
                CanMod = true;

                previousPlaces.Add(new Vector2(X, Y));
                if (previousPlaces.Count > 3)
                    previousPlaces.RemoveAt(0);
                if (toldCanModWalls && (oldX != X || oldY != Y||NeedToWait))
                    NeedToWait = !ModWalls(Grid, X, Y);
                EffectAdj(Grid, previousPlaces[0]);
            }
            else
            {
                shape.LinearVelocity = new Vector2(0, 0);
                dir *= -1f;
                Vector2 push = -1 * new Vector2(((float)X - Globals.map.MapWidth / 2) / Globals.map.MapWidth, ((float)Y - Globals.map.MapHeight / 2) / Globals.map.MapHeight) * 5;
                shape.ApplyForce(push, shape.Position);
            }
            oldX = X; oldY = Y;
            age++;
        }

        public virtual void EffectAdj(List<BaseTile>[,] Grid, Vector2 pos)
        {
            int X = (int)pos.X, Y = (int)pos.Y;
            List<List<BaseTile>> Adj = Globals.map.GetAdjColumsToList(X, Y);
            foreach (List<BaseTile> column in Adj)
            {
                byte preA = column[column.Count - 1].color.A;
                column[column.Count - 1].color = column[column.Count - 1].color.Combine(GetColor());
                
                //column[column.Count - 1].color.A = 255;
                if (column.Count == 2)
                {
                    column[column.Count - 1].color = column[column.Count - 1].color.Combine(GetColor());
                    //List<List<BaseTile>> Adj2 = Globals.map.GetAdjColumsToList((int)column[0].GridPos.X, (int) column[0].GridPos.Y);
                    /*
                     * foreach (List<BaseTile> column2 in Adj2)
                    {
                        if (column2.Count == 2)
                            column2[column2.Count - 1].color = column[column.Count - 1].color.Combine(GetColor()).Combine(GetColor());
                    }
                    */

                }
                if (Globals.GamplayScreen.currentState == Screens.Gameplay.ScreenState.Ghost)
                {
                    foreach (BaseTile tile in column)
                    {
                        tile.color.A = 255;
                    }
                }
                else
                { 
                    column[column.Count - 1].color.A = preA;
                }
            }
        }

        protected int GetHeightDiffrence(List<BaseTile>[,] Grid)
        {
            return GetHeightDiffrence(Grid, X, Y, 2);
        }

        protected static int GetHeightDiffrence(List<BaseTile>[,] Grid, int X, int Y, int offset)
        {
            List<BaseTile>[,] adj = Globals.map.GetAdjColumsToArray(X, Y, offset: offset);
            int ret = 0;
            for (int i = 0; i <= 4; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    if (Globals.map.isInBounds(X + i, Y + j) && (i != 1 || j != 1) && adj[i, j] != null)
                    {
                        ret += adj[i, j].Count;
                        //if ((i == 1 || j == 1) && (i != 1 || j != 1))
                        if ((i != 1 && j != 1))
                        {
                            ret += (adj[i, j].Count * 2) - 3;//(adj[i, j].Count - 1);
                        }
                    }
                }
            }
            return ret - 8;
        }

        //returns true if it did the desired action(whatever it may be), or false if it couldn't cause of the dumper
        protected bool ModWalls(List<BaseTile>[,] Grid, int pos1, int pos2)
        {
            if (Globals.map.isInBounds(pos1, pos2, Offset: 1) && CanMod)
            {
                int highDiff = GetHeightDiffrence(Grid);
                if (Globals.map.isInBounds(pos1, pos2, 1) && Grid[pos1, pos2].Count == 2 && dumper.isFree(Grid))
                {
                    Grid[pos1, pos2][Grid[pos1, pos2].Count - 1].Delete();
                    Grid[pos1, pos2].RemoveAt(Grid[pos1, pos2].Count - 1);
                    dumper.createBlock(Grid);
                    //hasBlock = true;!hasBlock && && -1 * highDiff < DeleteWallThreshold 
                }
                else if (Grid[pos1, pos2].Count == 2)
                {
                    return false;
                }

                Grid[pos1, pos2][Grid[pos1, pos2].Count - 1].color = GetColor().Combine(Grid[pos1, pos2][Grid[pos1, pos2].Count - 1].color);

                if (Globals.GamplayScreen.currentState == Screens.Gameplay.ScreenState.Ghost)
                {
                    foreach (BaseTile tile in (Grid[pos1, pos2]))
                    {
                        tile.color.A = 255;
                    }
                }
                //Grid[pos1, pos2][Grid[pos1, pos2].Count - 1].color.A = 255;
            }
            return true;
        }

        public virtual Color GetColor()
        {
            return new Color((int)(BaseColor.R + Math.Cos(X / RandomMap.RoomWidth * 1f) * 10), (int)(BaseColor.G + Math.Cos(X / RandomMap.RoomWidth * 1f) * 15), (int)(BaseColor.B + Math.Cos(X / RandomMap.RoomWidth * 1f) * 10));
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Sprite.Texture,
                               Globals.map.ScreenFromPhysics(shape.Position), null,
                               color, shape.Rotation, Sprite.Origin, 1f * Globals.map.globalScale,
                               SpriteEffects.None, 0f);
        }
    }
}
