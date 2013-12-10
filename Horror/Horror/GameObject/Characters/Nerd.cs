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
    public class Nerd : Enemy
    {
        public const int NERD_WIDTH = 680;
        public const int NERD_HEIGHT = 145;
        public const float NERD_SPEED = 1.0f;
        public const int NERD_DAMAGE = 1;
        public const int NERD_HEALTH = 2;
        public const int NERD_ATTACKDELAY = 2;
        public const float NERD_AWARENESS = 170;

        public Nerd(Vector2 position, int i, Map map, Player player)
            : base(position, i, map, player)
        {
            totalFrames = 10;

            width = NERD_WIDTH/totalFrames;
            height = NERD_HEIGHT;
            speed = NERD_SPEED;
            damage = NERD_DAMAGE;
            health = NERD_HEALTH;
            attackDelay = NERD_ATTACKDELAY;
            awarenessRadius = NERD_AWARENESS;

            origin = new Vector2(width / 2, height - height / 4);
        }

        /// <summary>
        /// Allows the enemy to move
        /// </summary>
        /// <param name="player"></param>
        /// <param name="light"></param>
        public override void Move(LightSource light, GameTime gameTime)
        {
            if (!getBoundingBox().Intersects(player.getBoundingBox()))
            {
                base.Move(light, gameTime);
            }
        }

        /// <summary>
        /// Nerd version of bounding box.
        /// Specialized for the nerd physique.
        /// </summary>
        /// <returns></returns>
        public override Rectangle getBoundingBox()
        {
            return new Rectangle((int)(position.X - width / 4), (int)(position.Y), 32, 32);
        }

        /// <summary>
        /// Gets the Nerd's bounding box were it at given position
        /// </summary>
        /// <param name="newPos"></param>
        /// <returns></returns>
        public override Rectangle getBoundingBoxAt(Vector2 newPos)
        {
            return new Rectangle((int)(newPos.X - width / 4), (int)(newPos.Y), 32, 32);
        }
    }


}
