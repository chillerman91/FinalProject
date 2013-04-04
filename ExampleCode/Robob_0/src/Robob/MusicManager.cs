using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Robob
{
    public class MusicManager
    {
        public MusicManager()
        {
            songs = new Dictionary<string, Song>();
            songQueue = new List<Song>();
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
        }

        public void LoadMusic (string name)
        {
            songs.Add (name, Engine.ContentLoader.Load<Song> (Path.Combine(soundRoot, name)));
        }

        public void PlayMusic (string name)
        {
            MediaPlayer.Play (songs[name]);
        }

        public void NextSong()
        {
            if (++songIndex >= songQueue.Count)
                songIndex = 0;

            MediaPlayer.Play (songQueue[songIndex]);
        }

        public void QueueMusic(string name)
        {
            if (songQueue == null)
            {
                songQueue = new List<Song>();
                songIndex = 0;
                MediaPlayer.IsRepeating = false;
            }

            songQueue.Add (songs[name]);
        }

        public void ClearQueue()
        {
            songQueue = null;
            songIndex = 0;
            MediaPlayer.IsRepeating = true;
        }

        public void FadeIn (float seconds)
        {
            startFade (0f, 1f, seconds);
        }

        public void FadeOut (float seconds)
        {
            startFade (1f, 0f, seconds);
        }

        public void Update (GameTime gameTime)
        {
            if (!fading)
                return;

            timePassed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            MediaPlayer.Volume = MathHelper.Lerp (start, end, MathHelper.Clamp (timePassed / timeTotal, 0f, 1f));

            if (MediaPlayer.Volume == end)
                fading = false;
        }

        private string soundRoot = "Sound";
        private Dictionary<string, Song> songs;
        private bool fading = false;
        private float timePassed;
        private float timeTotal;
        private float start;
        private float end;
        private int songIndex;
        private List<Song> songQueue; 

        private void startFade(float start, float end, float time)
        {
            this.start = start;
            this.end = end;
            this.timeTotal = time;
            this.timePassed = 0;

            fading = true;
            MediaPlayer.Volume = start;
        }

        private void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if (MediaPlayer.State == MediaState.Stopped && songQueue.Count > 0)
                NextSong ();
        }
    }
}
