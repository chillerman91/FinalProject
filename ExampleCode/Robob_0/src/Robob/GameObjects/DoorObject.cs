using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Robob
{
    public class DoorObject : GameObject, IActivatable
    {
        public DoorObject (int buttonTargetedCount)
        {
            this.buttonTargetedCount = buttonTargetedCount;
        }

        public Vector3 Original;
        private DateTime lastButtonSoundPlayed;
        private float secondSoundDelay = 1f;

        public override void Update (Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (!lerping)
                return;

            /*timePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timePassed >= lerpTime)
            {
                lerping = false;
                //this.Dense = this.Active;
            }
            
            if (this.Active)
                Vector3.Lerp (ref Original, ref sunk, timePassed / lerpTime, out Translation);
            else
                Vector3.Lerp (ref sunk, ref Original, timePassed / lerpTime, out Translation);
            */
            base.Update (gameTime);
        }

        public override void Reset()
        {
            this.Translation = Original;
            this.lerping = false;
            this.Dense = true;
            this.Active = false;
        }

        public void Activate(GameObject activator, Vector3 heading)
        {
            if (!(activator is ButtonObject))
                return;

            activatedCount++;

            if (activatedCount < buttonTargetedCount)
                return;

            if ((DateTime.Now - lastButtonSoundPlayed).TotalSeconds > secondSoundDelay)
            {
                Engine.ContentLoader.Load<SoundEffect> (Path.Combine ("Sound", "Door")).Play ();
                lastButtonSoundPlayed = DateTime.Now;
            }

            Translation.Y -= downSunkAmount;
 
            timePassed = 0;
            lerping = true;
            this.Dense = false;

            this.Active = true;
        }

        public void Deactivate(GameObject deactivator)
        {
            if (!(deactivator is ButtonObject))
                return;

            activatedCount--;

            if (activatedCount >= buttonTargetedCount || !this.Active)
                return;

            Translation.Y += downSunkAmount;

            timePassed = 0;
            lerping = true;
            
            this.Dense = true;
            this.Active = false;
        }

        private float downSunkAmount = 5f;
        private Vector3 sunk;
        private int activatedCount;
        private int buttonTargetedCount;

        private float lerpTime = 0.5f;
        private float timePassed;
        private bool lerping;
    }
}
