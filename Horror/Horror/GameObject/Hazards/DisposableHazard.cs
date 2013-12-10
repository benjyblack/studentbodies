using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    public abstract class DisposableHazard : Hazard
    {
        public DisposableHazard(Vector2 position, Player player, List<Enemy> enemies) : base(position, player, enemies)
        {
        }

        public override void DoDamage(Character character, GameTime gameTime)
        {
            Console.WriteLine("Disabling " + this.GetType().ToString() + "...");
            disabled = true;
            base.DoDamage(character, gameTime);
        }
    }
}
