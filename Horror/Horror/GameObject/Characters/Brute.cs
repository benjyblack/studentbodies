using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace Horror
{
    class Brute : Enemy

    {
        public const int BRUTE_WIDTH = 120;
        public const int BRUTE_HEIGHT = 100;
        public const float BRUTE_SPEED = 1.0f;
        public const int BRUTE_DAMAGE = 4;
        public const int BRUTE_HEALTH = 5;
        public const int BRUTE_ATTACKDELAY = 3;
        public const float BRUTE_AWARENESS = 150;
        
      
       public Brute(Vector2 position, int i, Map map, Player player)
            : base(position, i, map, player)
        {
            totalFrames = 1;
            width = BRUTE_WIDTH/1;
            height = BRUTE_HEIGHT;
            speed = BRUTE_SPEED;
            health = BRUTE_HEALTH;
            damage = BRUTE_DAMAGE;
            attackDelay = BRUTE_ATTACKDELAY;
            awarenessRadius = BRUTE_AWARENESS;
           

            origin = new Vector2(width / 2, height - height / 4);
        }

       protected override void Attack(GameTime gameTime)
       {
           player.Slow(gameTime);
           base.Attack(gameTime);
       }
    }
}
