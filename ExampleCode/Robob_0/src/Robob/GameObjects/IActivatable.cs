using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Robob
{
    public interface IActivatable
    {
        void Activate (GameObject activator, Vector3 lookVector);
        void Deactivate (GameObject deactivator);
    }
}
