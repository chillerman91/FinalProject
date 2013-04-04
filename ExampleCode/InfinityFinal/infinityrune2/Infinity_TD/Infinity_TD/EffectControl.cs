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
    class EffectControl
    {
        public List<SoundEffect> lista = new List<SoundEffect>();
        public Game1 game;
        private SoundEffect soundEffect;

        public EffectControl(Game1 game)
        {
            this.game = game;
        }


        public void EffectLoad()
        {
            soundEffect = game.Content.Load<SoundEffect>("Sounds/Effects/Robot Hit");
            lista.Add(soundEffect);
            soundEffect = game.Content.Load<SoundEffect>("Sounds/Effects/BossWalk");
            lista.Add(soundEffect);
            soundEffect = game.Content.Load<SoundEffect>("Sounds/Effects/Glacier Shoot");
            lista.Add(soundEffect);
            soundEffect = game.Content.Load<SoundEffect>("Sounds/Effects/Thunder Shoot");
            lista.Add(soundEffect);
            soundEffect = game.Content.Load<SoundEffect>("Sounds/Effects/Vlads voice");
            lista.Add(soundEffect);
        }
    }
}
