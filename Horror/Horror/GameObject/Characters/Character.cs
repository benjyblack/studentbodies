using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    abstract public class Character : GameObject
    {
        public float speed;
        public int attackStagger, damage, hazardDamageStagger, health, recoveryTime;
        public Map map;
        public Boolean key1 = false;
        public int slowTimeStart;
        public Boolean isSlowed = false;
        public String Type;
        public Character(Vector2 position, Map _map)
            : base(position) 
        {
            map = _map;
            damage = 0;
           
            //So far this variable is only used by the player, its the 
            // amount of time after being hit that he can start regaining health
            recoveryTime = 0;

            sourceFrame = new Rectangle((frameCount - 1) * (int)(width), 0, (int)(width), (int)height);

            hazardDamageStagger = 0;
        }

        /// <summary>
        /// Allows characters to take damage (decrease their hp)
        /// </summary>
        /// <param name="opponent"></param>
        /// <param name="dmg"></param>
        /// <param name="gameTime"></param>
        virtual public void TakeDamage(int dmg, GameTime gameTime)
        {
            health -= dmg;
        }

        /// <summary>
        /// Allows characters to take damage from traps
        /// (there is an added stagger so they don't get hurt more than once per contact)
        /// </summary>
        /// <param name="dmg"></param>
        /// <param name="gameTime"></param>
        public virtual void TakeHazardDamage(int dmg, GameTime gameTime)
        {
            if (hazardDamageStagger <= gameTime.TotalGameTime.TotalSeconds)
            {
                hazardDamageStagger += (int)gameTime.TotalGameTime.TotalSeconds + 1;
                health -= dmg;
            }
        }

        /// <summary>
        /// Take a step
        /// </summary>
        /// <param name="nextStep"></param>
        virtual public void Step(Vector2 nextStep)
        {
            // If not hitting wall, move enemy
            if (!map.isHittingWall(getBoundingBoxAt(position + nextStep))) position += nextStep;

            // This else block lets the enemy move up/down/left/right against walls, otherwise he wouldn't be able to "slide"
            // on walls
            else
            {
                Vector2 xMovement = new Vector2(nextStep.X, 0);
                Vector2 yMovement = new Vector2(0, nextStep.Y);
                if (!map.isHittingWall(getBoundingBoxAt(position + xMovement))) position += xMovement;
                else if (!map.isHittingWall(getBoundingBoxAt(position + yMovement))) position += yMovement;
            }
        }

        /// <summary>
        /// Animates the character
        /// </summary>
        public override void Animate()
        {
            sourceFrame = new Rectangle((frameCount - 1) * (int)width, 0, (int)width, (int)height);

            if (totalFrames > 1)
            {
                if ((int)(animationStagger % 16 / speed) == 0)
                {
                    if (frameCount == 1) forwardAnimation = true;
                    else if (frameCount == totalFrames) forwardAnimation = false;

                    if (forwardAnimation) frameCount++;
                    else frameCount--;
                }
            }
        }

        /// <summary>
        /// Character version of bounding box.
        /// A character's bounding box is usually just the lower half of his body.
        /// </summary>
        /// <returns></returns>
        public override Rectangle getBoundingBox()
        {
            return new Rectangle((int)(position.X - width/4), (int)(position.Y), 31,31);
        }

        /// <summary>
        /// Gets the Character's bounding box were it at given position
        /// </summary>
        /// <param name="newPos"></param>
        /// <returns></returns>
        public override Rectangle getBoundingBoxAt(Vector2 newPos)
        {
            return new Rectangle((int)(newPos.X - width/4), (int)(newPos.Y), 31, 31);
        }

        /// <summary>
        /// Gets the middle of the Character's bounding box (this is where moving traps should aim)
        /// </summary>
        /// <returns></returns>
        public Vector2 getBoundingBoxCenter()
        {
            return new Vector2(getBoundingBox().Left + getBoundingBox().Width / 2, getBoundingBox().Top + getBoundingBox().Height / 2); 
        }

        /// <summary>
        /// Returns the normal bounding box (same as all other GameObjects)
        /// </summary>
        /// <returns></returns>
        public Rectangle getProperBoundingBox()
        {
            return base.getBoundingBox();
        }
    }
}
