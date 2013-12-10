using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    public abstract class GameObject
    {
        public int width, height, totalFrames, frameCount, animationStagger;
        public Color color;
        public Vector2 position, origin;
        public Rectangle sourceFrame;
        public float depth;
        public bool tagged, forwardAnimation;

        public GameObject()
        {
        }

        public GameObject(Vector2 pos)
        {
            animationStagger = 0;
            forwardAnimation = true;
            frameCount = 1;
            totalFrames = 1;

            origin = new Vector2(width / 2, height / 2);
            position = pos;
            color = Color.White;
            depth = 0.0f;
            tagged = false;
        }

        /// <summary>
        /// Returns whether or not the position is within this object's given radius
        /// </summary>
        /// <param name="a"></param>
        /// <param name="rad"></param>
        /// <returns></returns>
        public bool inRadius(Vector2 a, float rad)
        {
            return rad * rad > (a.X - this.position.X) * (a.X - this.position.X) + (a.Y - this.position.Y) * (a.Y - this.position.Y);
        }

        /// <summary>
        /// Returns the GameObject's bounding box
        /// </summary>
        /// <returns></returns>
        public virtual Rectangle getBoundingBox()
        {
            return new Rectangle((int)(position.X - origin.X), (int)(position.Y - origin.Y), (int)width, (int)height);
        }

        /// <summary>
        /// Animates the object
        /// </summary>
        public virtual void Animate()
        {
            sourceFrame = new Rectangle((frameCount - 1) * (int)width, 0, (int)width, (int)height);

            if (animationStagger % 16 == 0)
            {
                if (frameCount == 1) forwardAnimation = true;
                else if (frameCount == totalFrames) forwardAnimation = false;

                if (forwardAnimation) frameCount++;
                else frameCount--;
            }
        }

        /// <summary>
        /// Specialized version of getBoundingBox()
        /// </summary>
        /// <param name="newPos"></param>
        /// <returns></returns>
        public virtual Rectangle getBoundingBoxAt(Vector2 newPos)
        {
            return new Rectangle((int)(newPos.X - origin.X), (int)(newPos.Y - origin.Y), (int)width, (int)height);
        }
     }
}


