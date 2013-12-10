using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    public abstract class ReusableHazard : Hazard
    {
        public float rechargeTime;
        public ReusableHazard(Vector2 position, Player player, List<Enemy> enemies) : base(position, player, enemies) { }
    }
}
