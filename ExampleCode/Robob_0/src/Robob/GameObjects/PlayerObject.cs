using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Robob.GameObjects
{
    public class PlayerObject
        : GameObject
    {
        public enum PlayerType
        {
            Normal,
            Strong,
            Jump
        }

        public Model Sphere;
        public Vector3 SphereRotation;
        public PlayerType Type;

        public PlayerObject()
        {

        }
    }
}
