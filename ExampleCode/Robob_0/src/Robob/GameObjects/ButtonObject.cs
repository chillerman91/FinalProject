using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Robob.GameObjects;

namespace Robob
{
    public class ButtonObject : GameObject, IActivatable 
    {
        public List<GameObject> Targets = new List<GameObject>();
        public Collidable Collision;
        public Action Action;

        private HashSet<int> Children = new HashSet<int> ();
        private DateTime lastButtonSoundPlayed;
        private float secondSoundDelay = 1f;
        private bool firstActivate;

        public void Activate(GameObject activator, Vector3 heading)
        {
            if (Active || Children.Contains (activator.ID))
                return;

            if ((DateTime.Now - lastButtonSoundPlayed).TotalSeconds > secondSoundDelay)
            {
                Engine.ContentLoader.Load<SoundEffect> (Path.Combine ("Sound", "Button")).Play (1f, 1f, 0f);
                lastButtonSoundPlayed = DateTime.Now;
            }
            this.Translation.Y -= downSunkAmount;
            this.Active = true;

            if (!firstActivate)
            {
                if (Action != null) Action ();
                firstActivate = true;
            }

            PerformActionOnTargets (o => o.Activate (this, Vector3.Zero));
            Children.Add (activator.ID);
        }

        public void Deactivate(GameObject deactivator)
        {
            if (!Children.Contains (deactivator.ID))
                return;

            if ((DateTime.Now - lastButtonSoundPlayed).TotalSeconds > secondSoundDelay)
            {
                Engine.ContentLoader.Load<SoundEffect> (Path.Combine ("Sound", "Button")).Play (1f, 1f, 0f);
                lastButtonSoundPlayed = DateTime.Now;
            }
            this.Translation.Y += downSunkAmount;
            this.Active = false;

            PerformActionOnTargets (o => o.Deactivate (this));
            Children.Remove (deactivator.ID);
        }

        private float downSunkAmount = 0.50f;

        private void PerformActionOnTargets (Action<IActivatable> action)
        {
            foreach (GameObject obj in Targets)
            {
                IActivatable o = obj as IActivatable;
                if (o != null)
                    action(o);
            }
        }
    }
}
