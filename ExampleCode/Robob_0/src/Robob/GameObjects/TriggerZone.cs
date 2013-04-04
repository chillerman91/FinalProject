using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Robob.GameObjects
{
    public class TriggerZone
        : GameObject, IActivatable
    {
        public TriggerZone (Collidable collidable, Action action)
        {
            this.Collision = collidable;
            this.action = action;
        }

        public Collidable Collision;
        public Action action;

        public override Microsoft.Xna.Framework.BoundingBox GetCurrentBoundingBox()
        {
            return Collision.Bounds;
        }

        public void Activate(GameObject activator, Vector3 heading)
        {
            var player = activator as PlayerObject;
            if (player.Type != PlayerObject.PlayerType.Normal)
                return;

            if (Active)
                return;

            this.Active = true;
            action ();
        }

        public void Deactivate(GameObject deactivator)
        {
            this.Active = false;
        }
    }
}
