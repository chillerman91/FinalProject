using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using SensorsAndSuch.Texts;
using System;
using SensorsAndSuch.Maps;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.SamplesFramework;
using SensorsAndSuch.Extensions;
using SensorsAndSuch.Mobs.AI;
using SensorsAndSuch.Mobs;
using FarseerPhysics.Dynamics.Contacts;

//Stone monster: can move walls
//water creature: becomes pools
//magician: teleport: time warp
//visible only when seen
// invisible: seen with dust
namespace SensorsAndSuch.Items
{
    public class Sword : BaseWeapon
    {
        Materials material;
        int timesUsed = 0;
        int timesSinceLast = 0;


        //Stats
        int hitCounter = 0;
        int killCounter = 0;

        public Sword(BaseMonster holder, Materials material)
            :base(GetName(), holder)
        {
            this.material = material;
            shape = BodyFactory.CreateRectangle(Globals.World, width: .6f, height: .1f, density: 1f);
            Sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.AssetCreatorr.TextureFromShape(shape.FixtureList[0].Shape,
                                                                                MaterialType.Squares,
                                                                                Color.AliceBlue, 1f));
            //shape.CollisionCategories = Category.Cat2;
            //shape.CollidesWith = Category.All;

            shape.FixtureList[0].IsSensor = true;
            shape.FixtureList[0].OnCollision += CollisionHandler;
            shape.FixtureList[0].OnSeparation += SeperationHandler;

        }

        #region Handlers

        public bool CollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            BaseMonster mon = null;
            if (fixtureB.Body != holder.shape && BaseMonster.GetMonster(fixtureB.Body.BodyId, ref mon)  && mon.Age > .2f)
            {
                hitCounter++;
                if (mon.health <= 0) return false;
                mon.DoDamage(damage * 2);
                if (mon.health <= 0)
                {
                    //mon.MakeChildren(1);
                    killCounter++;
                }
            }
            return true;
        }

        public void SeperationHandler(Fixture fixtureA, Fixture fixtureB)
        {
        }

        #endregion
        
        public override int StartUse()
        {
            shape.Rotation = holder.shape.Rotation;
            int temp = killCounter;
            shape.Position = holder.shape.Position + shape.Rotation.GetVecFromAng()* .23f;
            base.StartUse();
            killCounter = 0;
            return temp;
        }

        public override bool EndUse()
        {
            return false;
        }

        #region Static methods

        private static string GetName()
        {
            return "The Skull Chopper";
        }

        #endregion
    }
}