using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Infinity_TD
{
    public class SoundManager
    {
        Song themeSong;
        EffectControl effectControl;
        public static bool IsRepeating { get; set; }
        public Boolean playing = true;
        public Game1 game;
        public string tema;

        public SoundManager(Game1 game)
        {
            this.game = game;
            effectControl = new EffectControl(game);
            effectControl.EffectLoad();

        }


        public void soundLoad(string url)
        {
            tema = url;
            themeSong = game.Content.Load<Song>("Sounds/Musics/" + url);
        }
        public void playSound(int i)
        {
            effectControl.lista[i].Play();
        }

        public void playSong()
        {
            if (playing == true)
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(themeSong);
                MediaPlayer.IsRepeating = true;
            }
            if (playing == false)
            {
                 MediaPlayer.Pause();
            }
        }

        public void stopSong()
        {
            MediaPlayer.Stop();
        }
    }
}
