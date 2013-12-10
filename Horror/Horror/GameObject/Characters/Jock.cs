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
    class Jock : Enemy
    {
        public const int JOCK_WIDTH = 724;
        public const int JOCK_HEIGHT = 142;
        public const float JOCK_SPEED = 2.0f;
        public const int JOCK_DAMAGE = 2;
        public const int JOCK_HEALTH = 5;
        public const int JOCK_ATTACKDELAY = 2;
        public const float JOCK_AWARENESS = 175;

        int chargeTimer;

        public Jock(Vector2 position, int i, Map map, Player player)
            : base(position, i, map, player)
        {
            totalFrames = 8;

            width = JOCK_WIDTH/totalFrames;
            height = JOCK_HEIGHT;
            speed = JOCK_SPEED;
            health = JOCK_HEALTH;
            damage = JOCK_DAMAGE;
            attackDelay = JOCK_ATTACKDELAY;
            awarenessRadius = JOCK_AWARENESS;
            chargeTimer = 0;

            origin = new Vector2(width / 2, height - height / 4);
        }

        public override void Orient(Vector2 pos)
        {
            if (!inPursuit)
                chargeTimer = 0;

            if (chargeTimer <= 0)
            {
                chargeTimer = 120;
                base.Orient(pos);
            }
            chargeTimer--;
        }
        /// <summary>
        /// Jock version of bounding box.
        /// Specialized for the jock physique.
        /// </summary>
        /// <returns></returns>
        public override Rectangle getBoundingBox()
        {
            return new Rectangle((int)(position.X - width / 4), (int)(position.Y), 32, 32);
        }

        /// <summary>
        /// Gets the Jock's bounding box were it at given position
        /// </summary>
        /// <param name="newPos"></param>
        /// <returns></returns>
        public override Rectangle getBoundingBoxAt(Vector2 newPos)
        {
            return new Rectangle((int)(newPos.X - width / 4), (int)(newPos.Y), 32, 32);
        }
    }
}
