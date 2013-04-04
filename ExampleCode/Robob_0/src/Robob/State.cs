using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Robob
{

    public enum StateType
    {
        Title,
        Game,
        Credits
    }

    public class State
    {
        public bool Running;

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw()
        {

        }

        public virtual void Activate()
        {
        }
    }
}
