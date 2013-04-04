using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Robob.GameObjects
{
    public class BoxObject
        : GameObject, IActivatable
    {
        public Vector3 Original;
        public Vector3 Direction;
        public float MaxMovement;

        public override void Reset()
        {
            this.Translation = Original;
            this.Dense = true;
            this.Active = false;
        }

        public void Activate(GameObject activator, Vector3 heading)
        {
            var player = activator as PlayerObject;
            if (player == null || player.Type != PlayerObject.PlayerType.Strong)
                return;

            Direction.Normalize ();

            bool sameDirection = (this.Direction.X > 0 && heading.X > 0) ||
                (this.Direction.X < 0 && heading.X < 0) ||
                (this.Direction.Y < 0 && heading.Y < 0) ||
                (this.Direction.Y > 0 && heading.Y > 0);

            if (Vector3.Distance(Translation + this.Direction, Original) < this.MaxMovement && sameDirection)
                this.Translation += this.Direction;
        }

        public void Deactivate (GameObject deactivator)
        {
                
        }
    }
}
