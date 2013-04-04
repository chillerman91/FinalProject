using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Infinity_TD
{
    class Tower
    {
        private float dmg
        {
            get;
            set;
        }

        private Vector2 pos;
        private Texture2D textura;
        private const int RANGE = 4;

        private float fireRate
        {
            get;
            set;
        }

        public Tower(Texture2D _textura, float _dmg, Vector2 _pos, float _fireRate)
        {
            dmg = _dmg;
            textura = _textura;
            fireRate = _fireRate;
            pos = _pos;

        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(textura, pos, Color.Wheat);
        }
        public void Update()
        {

        }
    }
}
