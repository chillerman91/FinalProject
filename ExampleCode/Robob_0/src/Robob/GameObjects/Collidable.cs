using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Robob.GameObjects
{
    public class Collidable
    {
        public Collidable (float x1, float y1, float x2, float y2)
        {
            this.Bounds = new BoundingBox (new Vector3(x1, 0f, y1), new Vector3(x2, 1f, y2));     
        }

        public BoundingBox Bounds;
        public bool Dense = true;
    }
    
}
