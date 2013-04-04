using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Robob.GameObjects;

namespace Robob
{
	public partial class GameState
	{
        private void CheckObjectInteractions()
        {
            bbTracker.Clear();
            
            foreach (GameObject source in CurrentLevel.GameObjects)
            {
                if (!activatedTracker.ContainsKey (source.ID))
                    activatedTracker[source.ID] = new HashSet<int>();

                // Only check players for interactions
                if (!(source is PlayerObject))
                    continue;

                BoundingBox boundingBox = TryGetOrStoreBox (source);
                CheckInteractables (source, ref boundingBox, source.lastHeading);
            }
        }

        private bool CheckInteractables(GameObject source, ref BoundingBox boundingBox, ref BoundingBox currentBox, Vector3 lookVector)
        {
            // Check against all the interactive objects
            foreach (GameObject target in CurrentLevel.Collidables)
            {
                IActivatable activatable = target as IActivatable;

                if (activatable == null)
                    continue;

                if (!boundingBox.Intersects (TryGetOrStoreBox (target)))
                {
                    if (source.Tracking.Contains (target.ID))
                    {
                        activatable.Deactivate (source);
                        source.Tracking.Remove (target.ID);
                    }

                    continue;
                }

                activatable.Activate (source, lookVector);
                source.Tracking.Add (target.ID);

                if (target.Dense && !currentBox.Intersects (TryGetOrStoreBox (target)))
                    return true;
            }

            return false;
        }

        private bool CheckInteractables (GameObject source, ref BoundingBox boundingBox, Vector3 lookVector)
        {
            bool failMove = false;

            // Check against all the interactive objects
            foreach (GameObject target in CurrentLevel.Collidables)
            {
                IActivatable activatable = target as IActivatable;

                if (activatable == null)
                    continue;

                if (!boundingBox.Intersects (TryGetOrStoreBox (target)))
                {
                    if (source.Tracking.Contains (target.ID))
                    {
                        activatable.Deactivate (source);
                        source.Tracking.Remove (target.ID);
                    }

                    continue;
                }

                activatable.Activate (source, lookVector);
                source.Tracking.Add (target.ID);

                if (target.Dense)
                    failMove = true;
            }

            return failMove;
        }

	    private Dictionary<int, HashSet<int>> activatedTracker = new Dictionary<int, HashSet<int>>();
        private bool lastSet = false;
        private  float x;
        private float y;
        private StringBuilder text = new StringBuilder();
        Dictionary<int, BBContainer> bbTracker = new Dictionary<int, BBContainer> ();

        private void CreateCollision()
        {
            if (IsKeyPressed (Engine.LastKeyState, Engine.NewKeyState, Keys.F6))
            {
                if (lastSet)
                {
                    text.Append (string.Format ("this.CollisionGeometry.Add (new Collidable ({0:###.##}f, {1:###.##}f, {2:###.##}f, {3:###.##}f));{4}",
                        x, y, CurrentLevel.CurrentCharacter.Translation.X, CurrentLevel.CurrentCharacter.Translation.Z, Environment.NewLine));

                    lastSet = false;
                }

                x = CurrentLevel.CurrentCharacter.Translation.X;
                y = CurrentLevel.CurrentCharacter.Translation.Z;
                lastSet = true;
            }
            else if (IsKeyPressed (Engine.LastKeyState, Engine.NewKeyState, Keys.F7))
            {
                lastSet = lastSet;
            }
        }

        private bool IsKeyPressed(KeyboardState last, KeyboardState current, Keys key)
        {
            return last.IsKeyUp (key) && current.IsKeyDown (key);
        }

        private BoundingBox TryGetOrStoreBox(GameObject source)
        {
            if (bbTracker.ContainsKey (source.ID))
                return bbTracker[source.ID].Bounds;

            bbTracker.Add (source.ID, new BBContainer(source.GetCurrentBoundingBox ()));
            return bbTracker[source.ID].Bounds;
        }

	    private class BBContainer
	    {
            public BBContainer (BoundingBox bounds)
            {
                this.Bounds = bounds;
            }

	        public BoundingBox Bounds;
	    }
	}
}
