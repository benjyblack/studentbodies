using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    class Goo : TimedHazard
    {
        
        public Goo(Vector2 position, Player player, List<Enemy> enemies, GameTime gameTime)
            : base(position, player, enemies, gameTime)
        {
            totalFrames = 1;
            damage = 2;
        }

        public override void DoDamage(Character character, GameTime gameTime)
        {
            if(character.GetType().ToString() != "Horror.Chemist") base.DoDamage(character, gameTime);
        }
    }
}
