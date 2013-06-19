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
        bool on = false;
        int lastTickOn = 0;
        static int lifeSpan = 100;
        //Stats
        int hitCounter = 0;
        int killCounter = 0;

        public Sword(BaseMonster holder, Materials material)
            :base(GetName(), holder)
        {
            this.material = material;
            shape = BodyFactory.CreateRectangle(Globals.World, width: .6f, height: .1f, density: 1f);
            Sprite = new FarseerPhysics.SamplesFramework.Sprite(Globals.content.Load<Texture2D>("Items/sword"));
            //shape.CollisionCategories = Category.Cat3;
            //shape.CollidesWith = Category.Cat3;

            shape.FixtureList[0].IsSensor = true;
            shape.FixtureList[0].OnCollision += CollisionHandler;
            shape.FixtureList[0].OnSeparation += SeperationHandler;

        }

        #region Handlers

        public bool CollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            BaseMonster mon = null;
            if (inUse && fixtureB.Body != holder.Body && Globals.Mobs.GetMonster(fixtureB.Body.BodyId, ref mon))
            {
                hitCounter++;
                if (mon.health <= 0) return false;
                mon.DoDamage(damage);
                mon.Body.ApplyLinearImpulse(shape.Rotation.GetVecFromAng()/5f);
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


        internal override void updatePosition()
        {
            shape.Rotation = holder.Body.Rotation;
            shape.Position = holder.Body.Position + (shape.Rotation + (float)Math.PI/2f).GetVecFromAng() * -0.0f + shape.Rotation.GetVecFromAng() * .25f;//
        }

        public void SetOn()
        {
            on = true;
            //timesSinceLast = Globals.get
        }

        public override int Use()
        {
            updatePosition();
            int temp = killCounter;
            killCounter = 0;
            return temp;
        }

        #region Static methods

        private static string GetName()
        {
            return "The Skull Chopper";
        }

        #endregion
    }
}