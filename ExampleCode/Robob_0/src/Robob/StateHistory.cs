using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Robob
{
	public class StateHistory
	{
        public StateHistory()
        {
            Input = new List<GameObject>();
            Output = new List<GameObject>();
            Snapshots = new List<Snapshot>();
        }

	    public void Start (GameTime gameTime)
		{
			startTime = DateTime.Now;
			currentTime = startTime;

			LastSampleTime = SampleRate;
			currentSnapshotIndex = -1;

            if (Snapshots.Count > 0)
            {
                currentSnapshotIndex = 0;
                SetInterpolatedValues();
            }

	        IsStarted = true;
		}

        public void Stop()
        {
            IsStarted = false;
            SaveSnapshot ();
        }

	    public bool IsStarted;

		public void Update (GameTime gameTime)
		{
			currentTime = currentTime.AddSeconds (gameTime.ElapsedGameTime.TotalSeconds);
			LastSampleTime += gameTime.ElapsedGameTime.TotalSeconds;

			if (LastSampleTime > SampleRate)
			{
				SaveSnapshot();

				LastSampleTime = 0;
                currentSnapshotIndex++;

                if (currentSnapshotIndex > Snapshots.Count)
                    currentSnapshotIndex = Snapshots.Count;
			}
			else
				SetInterpolatedValues();

		}

		private void SetInterpolatedValues()
		{
            // No snapshots to set!
            if (Snapshots.Count == 0 || currentSnapshotIndex == Snapshots.Count)
                return;

            int oldSnapshotIndex = currentSnapshotIndex - 1;
            bool onFirstSnapshot = currentSnapshotIndex == 0;

            if (onFirstSnapshot)
                oldSnapshotIndex = currentSnapshotIndex;

            Snapshot oldSnapshot = Snapshots[oldSnapshotIndex];
            Snapshot nextSnapshot = Snapshots[currentSnapshotIndex];

			foreach (GameObject gameObject in Output)
			{
                if (!nextSnapshot.GameObjects.ContainsKey (gameObject.ID)
                    || !oldSnapshot.GameObjects.ContainsKey (gameObject.ID))
                    continue;

                float lerpAmount = (float)(LastSampleTime / SampleRate);

				GameObject oldObject = oldSnapshot.GameObjects[gameObject.ID];
				GameObject newObject = nextSnapshot.GameObjects[gameObject.ID];

				Vector3.Lerp (ref oldObject.Translation, ref newObject.Translation, lerpAmount, out gameObject.Translation);
                Vector3.Lerp (ref oldObject.Rotation, ref newObject.Rotation, lerpAmount, out gameObject.Rotation);
                Vector3.Lerp (ref oldObject.Scale, ref newObject.Scale, lerpAmount, out gameObject.Scale);
                
                gameObject.lastHeading = newObject.lastHeading;

                //var lookVector = oldObject.Translation - newObject.Translation;
                //lookVector.Normalize();
                //gameObject.lastHeading = lookVector;
			}
		}

		private DateTime startTime;
		private DateTime currentTime;
		private double LastSampleTime;
		private double SampleRate = 0.016; // Sample 60 times per second

		private Snapshot lastFull;
		private List<Snapshot> Snapshots;
		private int currentSnapshotIndex;

		public List<GameObject> Output;
 		public List<GameObject> Input;

		private void SaveSnapshot ()
		{
			Snapshot newSnapshot = new Snapshot (currentTime);
            bool appendSnapshot = currentSnapshotIndex + 1 < Snapshots.Count;
            if (appendSnapshot)
                newSnapshot = Snapshots[currentSnapshotIndex];

			Snapshot fullSnapshot = new Snapshot (currentTime);

			foreach (GameObject gameObject in Input)
			{
				fullSnapshot.GameObjects.Add (gameObject.ID, gameObject.HardCopy());

				if (HasObjectChanged (gameObject, lastFull))
				{
					newSnapshot.GameObjects.Add (gameObject.ID, fullSnapshot.GameObjects[gameObject.ID]);
				}
			}
			
			// Save the snapshots
			this.lastFull = fullSnapshot;
            if (newSnapshot.GameObjects.Count > 0 && !appendSnapshot)
            {
                Snapshots.Add (newSnapshot);
            }
		}

		private bool HasObjectChanged (GameObject gameObject, Snapshot full)
		{
            // No previous snapshot so it obviously changed
            if (full == null || !full.GameObjects.ContainsKey(gameObject.ID))
                return true;

            var oldObject = full.GameObjects[gameObject.ID];

            if (gameObject.Translation != oldObject.Translation)
                return true;

            return false;
		}

	}
	public class Snapshot
	{
		public Snapshot(DateTime time)
		{
			Time = time;
            GameObjects = new Dictionary<int, GameObject>();
		}

		public Dictionary<int, GameObject> GameObjects;
		public DateTime Time;
	}
}
