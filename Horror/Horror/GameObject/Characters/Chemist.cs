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
    public class Chemist : Enemy

    {
        public const int CHEM_WIDTH = 120;
        public const int CHEM_HEIGHT = 160;
        public const float CHEM_SPEED = 1.0f;
        public const int CHEM_DAMAGE = 0;
        public const int CHEM_HEALTH = 5;
        public const int CHEM_ATTACKDELAY = 3;
        public const float CHEM_AWARENESS = 150;
        public Chemist(Vector2 position, int i, Map map, Player player)
            : base(position, i, map, player)
        {
            totalFrames = 6;
            width = CHEM_WIDTH/1;
            height = CHEM_HEIGHT;
            speed = CHEM_SPEED;
            health = CHEM_HEALTH;
            damage = CHEM_DAMAGE;
            attackDelay = CHEM_ATTACKDELAY;
            awarenessRadius = CHEM_AWARENESS;
           

            origin = new Vector2(width / 2, height - height / 4);
        }

        /// <summary>
        /// Returns flipped if enemy sprite should be mirrored
        /// </summary>
        /// <returns></returns>
        public override SpriteEffects isMirrored()
        {
            if (direction <= Math.PI / 2 && direction > -Math.PI / 2)
                return SpriteEffects.None;
            return SpriteEffects.FlipHorizontally;
        }

        /// <summary>
        /// Animates the character
        /// </summary>
        public override void Animate()
        {
            sourceFrame = new Rectangle((frameCount) * (int)width, 0, (int)width, (int)height);

            if (totalFrames > 1)
            {
                if (animationStagger % 16 / (int)speed == 0)
                {
                    //if (frameCount == 1) forwardAnimation = true;
                    //else if (frameCount == totalFrames) forwardAnimation = false; 
                    frameCount = (frameCount + 1) % (totalFrames-1);
                    //else frameCount--;
                }
            }
        }

        public override Rectangle getBoundingBox()
        {
            return new Rectangle((int)(position.X - width / 4), (int)(position.Y), 32, 32);
        }

        public override Rectangle getBoundingBoxAt(Vector2 newPos)
        {
            return new Rectangle((int)(newPos.X - width / 4), (int)(newPos.Y), 32, 32);
        }
    
    }
    
   

}
