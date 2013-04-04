using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Robob.GameObjects;

namespace Robob.Levels
{
	public class LevelOne
		: Level
	{
		public LevelOne ()
            : base ("LevelFrequency")
		{
            this.yLevel = 0.5f;
		    this.CollisionMap = Engine.ContentLoader.Load<Texture2D> ("LevelOneCollision");
            ParseCollisionMap();

		    DoorObject door = base.CreateDoor (new Vector3(18f, yLevel - 1.2f, 13.4f), 1, 0f);
            ButtonObject button = base.CreateButton (new Vector3 (27, yLevel, 27), door, () =>
            {
                Engine.Professor.ShowTimed ("I think you need another\nRobob. Try pressing space?", 5f);
            });
            GameObject crystal = base.CreateCrystal (new Vector3 (7.5f, 0.0f, 25.0f));

            TriggerZone winZone = base.CreateTrigger (new Collidable (3.16f, 20.61f, 11.01f, 29.23f), () =>
            {
                Engine.MusicRef.PlayMusic ("Victory");
                Engine.Professor.EnqueueMessageBox (" Good news, you won in record \n time! " + string.Format ("{0:ss} seconds", DateTime.Now - start)
                    + " using " + ((GameState)Engine.CurrentState).generationCount + " robobs.", () =>
                    {
                        Engine.NextLevel ();
                    });
            });

	        // Add the objects to the level
			GameObjects.AddRange (new GameObject[] { button, door, winZone, crystal});
            Collidables.AddRange (new GameObject[] { button, door, winZone });

            // Collision Geometry
            this.CollisionGeometry.Add (new Collidable(10.91f, 17.95f, 22.53f, 31.31f));//
            this.CollisionGeometry.Add (new Collidable (31.40f, -1.6f, 35.62f, 32.23f));
            this.CollisionGeometry.Add (new Collidable (24.12f, 31.73f, 30.77f, 34.81f));
            this.CollisionGeometry.Add (new Collidable (30.77f, 34.81f, 3.01f, 4.28f));
            this.CollisionGeometry.Add (new Collidable (3.01f, 4.28f, 22.53f, 10f));//
            this.CollisionGeometry.Add (new Collidable (-3.96f, 7.44f, 2.26f, 30.63f));
            this.CollisionGeometry.Add (new Collidable (3.17f, 29.98f, 10.84f, 36.2f));
            this.CollisionGeometry.Add (new Collidable (20.09f, -3.23f, 22.53f, 3.52f));
            this.CollisionGeometry.Add (new Collidable (23.22f, -4.32f, 30.88f, -.47f));
		}

        public override void Activate()
        {
            Engine.Professor.ShowTimed ("Pick a Robob and help\nme get my crystal back.", 2f);
            base.Activate ();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update (gameTime);
        }
	}
}
