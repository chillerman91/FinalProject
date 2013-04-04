using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Robob.GameObjects;

namespace Robob.Levels
{
    public class LevelThree : Level
    {
        public LevelThree()
            : base ("LevelTimeIsMoney", true)
        {
            this.RobotsAllowed[(int)PlayerObject.PlayerType.Normal] = true;
            this.RobotsAllowed[(int)PlayerObject.PlayerType.Strong] = true;

            this.yLevel = 0.5f;
            this.CollisionMap = Engine.ContentLoader.Load<Texture2D> ("LevelTimeIsMoneyCollision");
            ParseCollisionMap ();
            levelObject.Scale = new Vector3 (1.8f, 1f, 1.8f);
            
            this.StartPoint = new Vector3(11.39f, yLevel, 4.9f);

            DoorObject door = base.CreateDoor (new Vector3 (12f, yLevel - 1.2f, 20), 1, 90f);
            ButtonObject button = base.CreateButton (new Vector3 (12f, yLevel, -11.35f), door);
            BoxObject crate1 = base.CreateCrate (new Vector3 (11.75f, yLevel, 14.80f), new Vector3(-1, 0, 0), 6f);
            BoxObject crate2 = base.CreateCrate (new Vector3 (11.75f, yLevel, -6.80f), new Vector3 (1, 0, 0), 6f);

            crate1.Scale += new Vector3 (0.12f, 0f, 0.12f);
            crate2.Scale += new Vector3 (0.1f, 0f, 0.1f);

            CrystalObject crystal = CreateCrystal (new Vector3 (11.8f, yLevel, 26.2f));

            TriggerZone winZone = base.CreateTrigger (new Collidable (9.61f, 23.41f, 14.4f, 28.39f), () =>
            {
                Engine.MusicRef.PlayMusic ("Victory");
                Engine.Professor.EnqueueMessageBox (" Good news, you won in record \n time! " + string.Format ("{0:ss} seconds", DateTime.Now - start)
                    + " using " + ((GameState)Engine.CurrentState).generationCount + " robobs.", () =>
                {
                    Engine.Professor.EnqueueMessageBox ("You've beat Robob!\nCongratulations.", () =>
                    {
                        Engine.NextLevelIndex = 0;
                        Engine.SetState (StateType.Title);
                    });
                });
            });

            // Add the objects to the level
            GameObjects.AddRange (new GameObject[] { button, door, crate1, crate2, winZone, crystal});
            Collidables.AddRange (new GameObject[] { button, door, crate1, crate2, winZone, crystal});

            this.CollisionGeometry.Add (new Collidable (14.74f, 7.28f, 19.45f, 12.90f));
            this.CollisionGeometry.Add (new Collidable (4.35f, -2.88f, 8.99f, 2f));
            this.CollisionGeometry.Add (new Collidable (14.99f, -3f, 23.79f, 1.86f));
            this.CollisionGeometry.Add (new Collidable (23.79f, 1.86f, 24.43f, 2.2f));
            this.CollisionGeometry.Add (new Collidable (24.43f, 2.2f, 26.83f, 19.51f));
            this.CollisionGeometry.Add (new Collidable (-.58f, 7.34f, 8.69f, 11.64f));
            this.CollisionGeometry.Add (new Collidable (8.69f, 11.64f, -8.59f, -12.45f));
            this.CollisionGeometry.Add (new Collidable (-8.59f, -12.45f, -.75f, 7.34f));
            this.CollisionGeometry.Add (new Collidable (-.75f, 7.34f, -.43f, -18.11f));
            this.CollisionGeometry.Add (new Collidable (-.43f, -18.11f, 8.91f, -8.29f));
            this.CollisionGeometry.Add (new Collidable (.17f, 12.4f, 3.84f, 18.17f));
            this.CollisionGeometry.Add (new Collidable (3.84f, 18.17f, -.49f, 18.17f));
            this.CollisionGeometry.Add (new Collidable (-.49f, 18.17f, 8.63f, 24.09f));

            // Bottom left
            this.CollisionGeometry.Add (new Collidable (9.81f, -16.49f, 25.51f, -14.07f));
            this.CollisionGeometry.Add (new Collidable (25.51f, -15.07f, 15f, -15.07f));
            this.CollisionGeometry.Add (new Collidable (14.35f, -15.07f, 20.02f, -8.96f));
            this.CollisionGeometry.Add (new Collidable (20.02f, -8.75f, 20.02f, -8.96f));
            this.CollisionGeometry.Add (new Collidable (19.35f, -8.96f, 24.91f, 1.1f));

            // Top
            this.CollisionGeometry.Add (new Collidable (27.51f, 30.65f, 26.51f, 30.65f));
            this.CollisionGeometry.Add (new Collidable (9.53f, 27.81f, 29.24f, 32.69f));
            this.CollisionGeometry.Add (new Collidable (4.03f, 24.85f, 8.99f, 27.06f));
            this.CollisionGeometry.Add (new Collidable (14.39f, 17.89f, 26.5f, 27.06f));
        }

        public override void Activate()
        {
            Engine.Professor.ShowTimed ("Those boxs look heavy.\nGive Strongbob a try!", 5f);
            base.Activate ();
        }
    }
}
