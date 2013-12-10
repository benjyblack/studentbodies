using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    public class Hole : ReusableHazard
    {
        public Hole(Vector2 position, Player player, List<Enemy> enemies)
            : base(position, player, enemies) 
        {
            rechargeTime = 0;
            damage = 100;
        }
    }
}
