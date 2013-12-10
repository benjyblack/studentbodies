using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    public class Boulder: MoveableHazard
    {
        public Boulder(Vector2 position, Player player, List<Enemy> enemies)
            : base(position, player, enemies)
        {
            width = 63;
            height = 60;

            origin = new Vector2(width / 2, height / 2);
        }
        
        /// <summary>
        /// Animates the hazard
        /// </summary>
        public override void Animate()
        {
            rotation += 0.01f;
            sourceFrame = new Rectangle((frameCount - 1) * (int)width, 0, (int)width, (int)height);
        }
    }
}
