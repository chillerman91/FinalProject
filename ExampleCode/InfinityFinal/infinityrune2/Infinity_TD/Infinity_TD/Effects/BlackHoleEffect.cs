using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infinity_TD
{
    class BlackHoleEffect : Effect
    {
        public override float ReduceAmountVelocity { get { return 0; } }
        public override float ReduceAmountLife { get { return 0; } }
        public override float Damage { get { return random.Next(1, 15); } }
    }
}
