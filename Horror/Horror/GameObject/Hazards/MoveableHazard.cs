using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    public class MoveableHazard: Hazard
    {
        public Rectangle triggerArea;
        public Character prey;
        public float direction, speed;
        public Vector2 heading;

        public MoveableHazard(Vector2 position, Player player, List<Enemy> enemies) : base(position, player, enemies)
        {
            direction = 0;
            heading = Vector2.Zero;
            damage = 5;
            totalFrames = 1;
            speed = 0.8f;
            
            triggerArea = new Rectangle((int)position.X - getBoundingBox().Width*2, (int)position.Y - getBoundingBox().Height*2,
                                            getBoundingBox().Width * 6, getBoundingBox().Height * 6);
        }

        /// <summary>
        /// Returns the MoveabkeHazard's bounding box
        /// </summary>
        /// <returns></returns>
        public override Rectangle getBoundingBox()
        {
            return new Rectangle((int)(position.X - origin.X), (int)(position.Y - origin.Y), (int)width, (int)height);
        }

        /// <summary>
        /// Update method specifically for the MoveableHazards
        /// </summary>
        /// <param name="map"></param>
        /// <param name="gameTime"></param>
        public override void Update(Map map, GameTime gameTime)
        {
            position += heading * speed;
            
            prey = null;

            if (beingAnimated) base.Update(map, gameTime);

            else
            {
                if (triggerArea.Intersects(player.getBoundingBox())) prey = player;
                else
                    foreach (Enemy e in enemies) if (triggerArea.Intersects(e.getBoundingBox())) prey = e;

                if (prey != null)
                {
                    Console.WriteLine(GetType().ToString() + " triggered by " + prey.GetType());
                    direction = (float)Math.Atan2(prey.getBoundingBoxCenter().Y - this.position.Y, 
                                                    prey.getBoundingBoxCenter().X - this.position.X);

                    direction = Map.roundRadian(direction);

                    heading = new Vector2((float)Math.Cos(direction), (float)Math.Sin(direction));

                    beingAnimated = true;
                }
            }
        }
    }
}
