using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Robob.GameObjects;

namespace Robob
{
    public class GameObject
    {
        public Vector3 Translation;
        public Vector3 Rotation;
        public Vector3 Scale;
        public Model Mesh;
        public bool Dense;
        public HashSet<int> Tracking = new HashSet<int>();
        public Vector3 lastHeading;

        public virtual BoundingBox GetCurrentBoundingBox()
        {
            Matrix transformations = Matrix.CreateScale (Scale) * Matrix.CreateRotationY (Rotation.Y);
            BoundingBox bb = Mesh.UpdateBoundingBox (transformations);
            float modelWidth = bb.Max.X - bb.Min.X;
            float modelHeight = bb.Max.Z - bb.Min.Z;

            Collidable playerCollidable = new Collidable (
                Translation.X - (modelWidth / 2f),
                Translation.Z - (modelHeight / 2f),
                Translation.X + (modelWidth / 2f),
                Translation.Z + (modelHeight / 2f));

            return playerCollidable.Bounds;
        }

        public virtual void Reset()
        {

        }

        public virtual void Update (GameTime gameTime)
        {
        }

        public enum Direction
        {
            North,
            South,
            East,
            West
        }

        public int ID;
        public bool Active
        {
            get;
            set;
        }

        public GameObject()
        {
            Scale = new Vector3(1.0f, 1.0f, 1.0f);
        }

		public GameObject HardCopy()
		{
			return new GameObject
			{
				ID = this.ID,
				Active = this.Active,

				Translation = this.Translation,
				Rotation = this.Rotation,
				Scale = this.Scale,
                lastHeading = this.lastHeading
			};
		}
    }
}
