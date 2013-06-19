﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SensorsAndSuch.Maps;
using SensorsAndSuch.Mobs;
using SensorsAndSuch.Screens;
using System.Collections.Generic;
using SensorsAndSuch.Extensions;
using System;
using SensorsAndSuch.Items;
using FarseerPhysics.Dynamics;

namespace SensorsAndSuch.Sprites
{
    public class Player : Killer
    {
        public enum MoveOpt
        {
            LEFT,
            RIGHT,
            UP,
            DOWN,
            USEITEM,
            NONE
        }

        HUDPlayerInfo HUD;
        MoveOpt previousOpt;
        Vector2 oldPos;
        public int kills;
        public int lives = 3;
        public bool dead = false;
        public double timeAtDeath;
        protected int timeSpanDead = 10;
        public bool CanUseMap = Globals.Debugging;
        public int range { 
            get { return Globals.Debugging ? 100 : (dead ? 2: seeingRange); } 
            set{ seeingRange = value;}
        }
        protected int seeingRange = 2;
        public String BonusText = "Get 2 kills for a larger vision!!";
        public Player(ContentManager content, Vector2 GridPos)
            : base(GridPos, Color.Gold, .15f, Genome: null)
        {
            this.color = Color.Gold;
            //PieSliceSensor = new PieSlice(circle, new Color(10, 10, 10, 90), 2f);
            speed = .5f;

            weapon = new Sword(this, Item.Materials.Iron);
            weapon.StartUse();
            Dir = new Vector2(0, 0);
        }

        internal void SetThisHUD(HUDPlayerInfo HUD)
        {
            this.HUD = HUD;
        }

        public void TakeTurn(MoveOpt Opt)
        {
            if (health <= 0)
            {
                health = MaxHealth;
                lives--;
                if (Globals.Debugging)
                    SetRandPosSafe();
                else
                {
                    color = Color.Gold;
                    color.A = 10;
                    weapon.EndUse();
                    dead = true;
                    Body.LinearDamping = 1f;
                    Body.CollisionCategories = Category.Cat3;
                    Body.CollidesWith = Category.Cat3;
                    timeAtDeath = Globals.GameTime.TotalGameTime.TotalSeconds;
                    Globals.screen.SetDied();
                }
            }

            if (dead)
            {
                if (timeAtDeath + timeSpanDead < Globals.GameTime.TotalGameTime.TotalSeconds)
                {
                    if(Globals.map.isFree(Globals.map.GridFromPhysics(Body.Position)))
                    {
                        color = Color.Gold;
                        color.A = 255;
                        weapon.StartUse();
                        dead = false;
                        Body.LinearDamping = 3f;
                        Body.CollidesWith = Category.Cat1;
                        Body.CollisionCategories = Category.Cat1;
                        Globals.screen.SetLiving();
                    }

                }
            }

            for (int i = 0; i < Sensors.Length; i++)
            {
                Sensors[i].Update();
                Sensors[i].GetReturnValues();
            }

            switch (Opt)
            {
                case MoveOpt.LEFT:
                    {
                        Body.ApplyForce(new Vector2(1, 0) * speed, Body.Position);
                        Body.Rotation = 0;//MathHelper.Pi;
                        break;
                    }
                case MoveOpt.RIGHT:
                    {
                        Body.ApplyForce(new Vector2(-1, 0) * speed, Body.Position);
                        Body.Rotation = MathHelper.Pi;
                        break;
                    }
                case MoveOpt.UP:
                    {
                        Body.ApplyForce(new Vector2(0, -1) * speed, Body.Position);
                        Body.Rotation = MathHelper.Pi * 3f / 2f;
                        break;
                    }
                case MoveOpt.DOWN:
                    {
                        Body.ApplyForce(new Vector2(0, 1) * speed, Body.Position);
                        Body.Rotation = MathHelper.Pi / 2f;
                        break;
                    }
                case MoveOpt.USEITEM:
                    {
                        break;
                    }
            }
            int newKills = weapon.Use();
            
            kills += newKills;
            if (newKills != 0)
            {
                ApplyBonuses(kills);
            }
            // Update player position and heading.
            if (!(Body.Position - oldPos).HasNan())
            {
                this.Dir = (Body.Position - oldPos);
                this.Dir.Normalize();

            }

            if (this.Dir.HasNan())
            {
                this.Dir = new Vector2(0, 0);
            }
            // Update HUD for position/heading and each sensor type.
            // Update player info.
            //HUD.UpdatePlayer(string.Format("Player Position: X={0:F2} Y={1:F2}; Heading: X={2:F2} Y={3:F2}", this.CurrentGridPos.X, this.CurrentGridPos.Y, this.Dir.X, this.Dir.Y));

            //HUD.UpdatePieSlices(pieSliceInfo);
            oldPos = Body.Position;
        }
/*
        public override void Draw(SpriteBatch batch)
        {
            //AdjDataGetter.Draw(batch);
            if (weapon != null) weapon.Draw(batch);
            batch.Draw(Sprite.Texture,
                               Globals.map.ScreenFromPhysics(shape.Position), null,
                               adjColor == null ? color : (Color)adjColor, shape.Rotation, Sprite.Origin, Globals.map.globalScale * .75f,
                               SpriteEffects.None, 0f);
            adjColor = null;
        }
        */
        public void CreatePlayer(int clas, string name)
        {
            Name = name;
        }

        public string ApplyBonuses(int kills)
        {
            string ret = "";
            switch (kills)
            {
                case (2):
                {
                    range++;
                    BonusText = String.Format("Get 4 kills for a larger vision!!");
                    break;
                }
                case (4):
                {
                    range++;
                    BonusText = String.Format("Get 6 kills for a larger vision!!");
                    break;
                }
                case (6):
                {
                    range++;
                    BonusText = String.Format("Get 8 kills for a larger vision!!");
                    break;
                }
                case (8):
                {
                    range++;
                    BonusText = String.Format("Get 10 kills for an extra life!!");
                    break;
                }
                case (10):
                {
                    lives++;
                    BonusText = String.Format("Get 15 kills for a larger vision!!");
                    break;
                }
                case (15):
                {
                    range++;
                    BonusText = String.Format("Get 20 kills for a larger vision!!");
                    break;
                }
                case (20):
                {
                    range++;
                    BonusText = String.Format("Get 25 more kills for a larger vision!!");
                    break;
                }
                case (25):
                {
                    range++;
                    range++;
                    BonusText = String.Format("Get 35 more kills for a larger vision!!");
                    break;
                }
                case (35):
                {
                    range +=4;
                    BonusText = String.Format("Get 50 more kills for Full vision!!");
                    break;
                } 
                                
                case (50):
                {
                    range = 100;
                    CanUseMap = true;
                    BonusText = String.Format("Get sdfg more kills for a larger vision!!");
                    break;
                }
                speed += .1f;
            }

            if(kills < 81)
            {
                if (kills % 10 == 0)
                    lives++;
                return ret;
            }
            return null;
        }

        internal void Suicide()
        {
            health = 0;
        }

        internal void Rest()
        {
            health += 1;
        }

    }
}
