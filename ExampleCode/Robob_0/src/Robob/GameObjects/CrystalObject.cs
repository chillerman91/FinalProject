using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Robob.GameObjects
{
    public class CrystalObject
        : GameObject
    {
        public override void Update(GameTime gameTime)
        {
            this.Rotation.Y += 0.2f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update (gameTime);
        }
    }
}
