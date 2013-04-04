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
    public class LevelTwo
        : Level
    {
        public LevelTwo()
            : base("LevelWavelength", true)
        {
            StartPoint = new Vector3 (15.4f, 0, 0);
            levelObject.Scale = new Vector3 (1.4f, 1f, 1.4f);
            levelObject.Translation.Y -= 2;
            yLevel = -1f;
            float yOffset = -1f;
            float buttonOffset = -0.55f;

            DoorObject door1 = base.CreateDoor (new Vector3 (9f, yLevel + yOffset, 12f), 1, 0);
            DoorObject door2 = base.CreateDoor (new Vector3 (12f, yLevel + yOffset, 23.25f), 1, 0);
            DoorObject door3 = base.CreateDoor (new Vector3 (24.55f, yLevel + yOffset, 8.37f), 0, 90);

            door1.Scale += new Vector3 (0, 0, 0.2f);
            door2.Scale += new Vector3 (0, 0, 0.2f);

            ButtonObject button1 = base.CreateButton (new Vector3 (15.4f, yLevel + buttonOffset, 10.74f), door1);
            ButtonObject button2 = base.CreateButton (new Vector3 (-0.44f, yLevel + buttonOffset, 21.37f), door2);
            ButtonObject button3 = base.CreateButton (new Vector3 (24.35f, yLevel + buttonOffset, 21.37f), door3);

            GameObject crystal = base.CreateCrystal (new Vector3 (24.25f, yLevel, -2f));

            TriggerZone winZone = base.CreateTrigger (new Collidable (22.22f, -3.37f, 27.46f, 1.16f), () =>
            {
                Engine.MusicRef.PlayMusic ("Victory");
                Engine.Professor.EnqueueMessageBox (" Good news, you won in record \n time! " + string.Format ("{0:ss} seconds", DateTime.Now - start)
                    + " using " + ((GameState)Engine.CurrentState).generationCount + " robobs.", () =>
                    {
                        Engine.NextLevel ();
                    });
            });


            GameObjects.AddRange(new GameObject[]
            {
                door1,
                door2,
                door3,
                button1,
                button2,
                button3,
                winZone,
                crystal
            });

            Collidables.AddRange (new GameObject[]
            {
                door1,
                door2,
                door3,
                button1,
                button2,
                button3,
                winZone
            });

            this.CollisionGeometry.Add (new Collidable (-5.73f, -7.56f, 11.72f, 6.69f));
            this.CollisionGeometry.Add (new Collidable (11.72f, 6.69f, -16.43f, 6.69f));
            this.CollisionGeometry.Add (new Collidable (-16.43f, 6.69f, -5.07f, 29.32f));
            this.CollisionGeometry.Add (new Collidable (-5.07f, 29.32f, -17.4f, 29.32f));
            this.CollisionGeometry.Add (new Collidable (-17.4f, 27.32f, 35.08f, 32.59f));
            this.CollisionGeometry.Add (new Collidable (35.08f, 32.59f, 28.37f, -4.52f));
            this.CollisionGeometry.Add (new Collidable (28.37f, -4.52f, 34.76f, 31.33f));
            this.CollisionGeometry.Add (new Collidable (12.62f, -10f, 27.47f, -4.12f));

            this.CollisionGeometry.Add (new Collidable (19.90f, -3.37f, 20.75f, 16.66f));
            this.CollisionGeometry.Add (new Collidable (21.32f, 16.66f, 3.71f, 14.84f));
            this.CollisionGeometry.Add (new Collidable (3.71f, 14.84f, 20.35f, 17.79f));

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}