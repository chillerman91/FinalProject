using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SensorsAndSuch.Maps;
using SensorsAndSuch.Mobs;
using SensorsAndSuch.Screens;
using System.Collections.Generic;
using SensorsAndSuch.Extensions;
using System;

namespace SensorsAndSuch.Sprites
{
    public class Player : BadGuy
    {
        public enum MoveOpt
        {
            LEFT,
            RIGHT,
            UP,
            DOWN,
            NONE
        }
        HUDPlayerInfo HUD;
        public Player(ContentManager content, Vector2 GridPos)
            : base(GridPos, Color.Purple, 0, 0, FarseerPhysics.Dynamics.BodyType.Dynamic, .15f, parent: null)
        {
            //PieSliceSensor = new PieSlice(circle, new Color(10, 10, 10, 90), 2f);
            speed = .5f;
        }

        internal void SetThisHUD(HUDPlayerInfo HUD)
        {
            this.HUD = HUD;
        }

        MoveOpt previousOpt;

        public void TakeTurn(MoveOpt Opt)
        {
            float[] ret = new float[2];
            float[] output = new float[2];

            ret[0] = Wiskers[0].Update();
            //ret[1] = Wiskers[1].Update();
            //ret[2] = Wiskers[2].Update();
            //ret[3] = Wiskers[3].Update();
            ret[1] = (circle.Rotation % (2f * (float)Math.PI)) / (2f * (float)Math.PI);
            //List<Vector2> circleContent = CircleSensor.Update(this.Dir);
            //int[] pieSliceContent = PieSliceSensor.Update(this.Dir);
            if (ret[1] > 1 || ret[0] < 0)
                return;
            output[0] = 0.5f;// 0.5f;
            output[1] = 0.5f;
            //output[2] = 1;
            switch (Opt)
            {
                case MoveOpt.LEFT:
                    {
                        output[0] = 1;
                        circle.ApplyForce(new Vector2(1, 0) * speed, circle.Position);
                        circle.Rotation = 0;//MathHelper.Pi;
                        break;
                    }
                case MoveOpt.RIGHT:
                    {
                        output[0] = 0;
                        circle.ApplyForce(new Vector2(-1, 0) * speed, circle.Position);
                        circle.Rotation = MathHelper.Pi;
                        break;
                    }
                case MoveOpt.UP:
                    {
                        output[1] = 0;
                        circle.ApplyForce(new Vector2(0, -1) * speed, circle.Position);
                        circle.Rotation = MathHelper.Pi * 3f / 2f;
                        break;
                    }
                case MoveOpt.DOWN:
                    {
                        output[1] = 1;
                        circle.ApplyForce(new Vector2(0, 1) * speed, circle.Position);
                        circle.Rotation = MathHelper.Pi / 2f;
                        break;
                    }
            }
            //output[0] = -1f;
            //output[1] = -1f; 
            if (MoveOpt.NONE != Opt && previousOpt != Opt)
            {/*
                Brain.Flush();
                float[] ret1 = Brain.Calculate(ret);
                Globals.Mobs.RunBackProp(inputs: ret, output: output);
                float[] outers = Brain.BackProp(ret, output);
                Brain.Flush();
                float[] ret2 = Brain.Calculate(ret);
                if (Math.Abs(ret1[0] - output[0]) < Math.Abs(ret2[0] - output[0]))
                {
                    return;
                }
                if (Math.Abs(ret1[1] - output[1]) < Math.Abs(ret2[1] - output[1]))
                {
                    return;
                }
                previousOpt = Opt;
                HUD.UpdateWhiskers(string.Format("{0:F2}", 1f), string.Format("{0:F2}", ret[0]), string.Format("{0:F2}", ret[1]));
            }
            else
            {
                ret[0] = .6f;
                ret[1] = .8f;
                Brain.Flush();
                output = Brain.Calculate(ret);
                output[0] = 3f;*/
            }
            // Update player position and heading.
            this.Dir = circle.Rotation.GetVecFromAng();
            this.CurrentGridPos = circle.Position;

            // Update HUD for position/heading and each sensor type.
            // Update player info.
            HUD.UpdatePlayer(string.Format("Player Position: X={0:F2} Y={1:F2}; Heading: X={2:F2} Y={3:F2}", this.CurrentGridPos.X, this.CurrentGridPos.Y, this.Dir.X, this.Dir.Y));

            // Update whisker info.

            // Update circleSensor info.
            string adjacentInfo = "Agents: ";
            int i = 0;
            /*
            foreach(Vector2 otherAgent in circleContent)
            {
                i++;
                adjacentInfo += string.Format("{0}) dist: {1:F1}, angle: {2:F0}, ", i, otherAgent.X, otherAgent.Y);
            }
            HUD.UpdateAdjacents(adjacentInfo);
            */
            // Update pieslice info.
            string pieSliceInfo = "PieSlices: ";
            //pieSliceInfo += string.Format("Front: {0}, Left: {1}, Back: {2}, Right: {3}",
            //    pieSliceContent[0], pieSliceContent[1], pieSliceContent[2], pieSliceContent[3]);
            HUD.UpdatePieSlices(pieSliceInfo);
        }

        float turnMom = 0;

        public void TakeTurnPackPRop(MoveOpt Opt)
        {
            float[] ret = new float[Wiskers.Length];
            float[] output = new float[1];

            for (int i = 0; i < Wiskers.Length; i++)
                ret[i] = (Wiskers[i].Update());
            //ret[1] = Wiskers[1].Update();
            //ret[2] = Wiskers[2].Update();
            //ret[3] = Wiskers[3].Update();
            //ret[1] = (circle.Rotation %( 2f * (float)Math.PI)) / (2f * (float)Math.PI);
            //List<Vector2> circleContent = CircleSensor.Update(this.Dir);
            //int[] pieSliceContent = PieSliceSensor.Update(this.Dir);
            if (ret[1] > 1 || ret[0] < 0)
                return;
            //output[0] = 0.5f;// 0.5f;
            //output[1] = 0.5f;
            //output[2] = 1;
            //circle.Rotation = circle.Rotation * .7f + newRot * .3f;
            float startRot = circle.Rotation; //.GetVecFromAng();
            //circle.ApplyForce(dir * speed, circle.Position);
            switch (Opt)
            {
                case MoveOpt.LEFT:
                    {
                        turnMom += .01f;
                        break;
                    }
                case MoveOpt.RIGHT:
                    {

                        turnMom -= .01f;
                        break;
                    }
            }
            if (turnMom != 0)
                turnMom += -turnMom * .25f;
            circle.Rotation += turnMom;

            output[0] = (circle.Rotation - startRot)/10f;
            output[0] = Math.Max(-1, Math.Min(output[0], 1));
            output[0] = output[0] / 2 + .5f;
            output[0] = .5f;                
            if (MoveOpt.NONE != Opt)
            {            
                Vector2 dir = circle.Rotation.GetVecFromAng();
                circle.ApplyForce(dir * speed, circle.Position);
                //Globals.Mobs.RunBackProp(inputs: ret, output: output);
                //BackProp(ret, output);

                previousOpt = Opt;
                HUD.UpdateWhiskers(string.Format("{0:F2}", 1f), string.Format("{0:F2}", ret[0]), string.Format("{0:F2}", ret[1]));
            }
            else {
                //ret[0] = .6f;&& previousOpt != Opt
                //ret[1] = .8f;
                //Brain.Flush();
                //output = Brain.Calculate(ret);
                //output[0] = 3f;
            }
            // Update player position and heading.
                this.Dir = circle.Rotation.GetVecFromAng();
                this.CurrentGridPos = circle.Position;
            
            // Update HUD for position/heading and each sensor type.
            // Update player info.
            HUD.UpdatePlayer(string.Format("Player Position: X={0:F2} Y={1:F2}; Heading: X={2:F2} Y={3:F2}", this.CurrentGridPos.X, this.CurrentGridPos.Y, this.Dir.X, this.Dir.Y));

            // Update whisker info.

            // Update circleSensor info.
            string adjacentInfo = "Agents: ";
            //int i = 0;
            /*
            foreach(Vector2 otherAgent in circleContent)
            {
                i++;
                adjacentInfo += string.Format("{0}) dist: {1:F1}, angle: {2:F0}, ", i, otherAgent.X, otherAgent.Y);
            }
            HUD.UpdateAdjacents(adjacentInfo);
            */
            // Update pieslice info.
            string pieSliceInfo = "PieSlices: ";
            //pieSliceInfo += string.Format("Front: {0}, Left: {1}, Back: {2}, Right: {3}",
            //    pieSliceContent[0], pieSliceContent[1], pieSliceContent[2], pieSliceContent[3]);
            HUD.UpdatePieSlices(pieSliceInfo);
        }

        public override void Draw(SpriteBatch batch)
        {
            SetHUDData(batch);
            Wiskers[0].Draw(batch);
            base.Draw(batch);
        }

        public void SetHUDData(SpriteBatch batch)
        {
        }

        public void CreatePlayer(int clas, string name)
        {
            Name = name;
        }

        internal void Warp()
        {
            Vector2 pos = Globals.map.GetRandomFreePos();
            circle.Position = new Vector2 (pos.X * TileWidth, pos.Y * TileHeight);
            //CircleSensor.ClearCollisions();
            //PieSliceSensor.ClearCollisions();
        }

        internal void Rest()
        {
            health += 1;
        }
    }
}
