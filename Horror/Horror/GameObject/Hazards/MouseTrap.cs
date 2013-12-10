using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    class MouseTrap : DisposableHazard
    {
        public MouseTrap(Vector2 position, Player player, List<Enemy> enemies)
            : base(position, player, enemies)
        {
            damage = 2;
            totalFrames = 4;
        }
    }
}
