using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    public class LaserPointer : DirectionalLight
    {
        public const float LASERPOINTER_DEPTH = 0.10f;
        public const int LASERPOINTER_WIDTH = 119;
        public const int LASERPOINTER_HEIGHT = 25;

        public GameObject[] taggedObjects;
        public int tagIndex;
        public LaserPointer(Player player) : base(player) 
        {
            depth = LASERPOINTER_DEPTH;
            width = LASERPOINTER_WIDTH;
            height = LASERPOINTER_HEIGHT;

            bodyWidth = 27;
            bodyHeight = 17;

            distanceToSpot = 100 + bodyWidth;
            color = Color.Red;

            origin = new Vector2(0, 8);
            lightEffectOrigin = new Vector2(9 - bodyWidth, 11);
            lightCutoutOrigin = new Vector2(1500, 1500);

            taggedObjects = new GameObject[3];
            tagIndex = 0;
        }

        /// <summary>
        /// Tag a GameObject g so that they may be seen in the dark
        /// </summary>
        /// <param name="g"></param>
        public void Tag(GameObject g)
        {
            if (g.tagged)
                return;

            g.tagged = true;

            if (taggedObjects[0] == null)
                taggedObjects[0] = g;
            else if (taggedObjects[1] == null)
                taggedObjects[1] = g;
            else if (taggedObjects[2] == null)
                taggedObjects[2] = g;
            else
            {
                Console.WriteLine("TAGS: Removing " + taggedObjects[tagIndex].GetType().ToString() + " from tags...");
                taggedObjects[tagIndex].tagged = false;
                Console.WriteLine("TAGS: Adding " + g.GetType().ToString() + " to tags...");
                taggedObjects[tagIndex] = g;
                tagIndex = ++tagIndex % 3;
            }
        }

        /// <summary>
        /// Reset all tags
        /// </summary>
        public void ResetTags()
        {
            for (int i = 0; i < 3; i++)
            {
                if (taggedObjects[i] != null)
                {
                    taggedObjects[i].tagged = false;
                    taggedObjects[i] = null;
                    tagIndex = 0;
                }
            }
        }

        public override void AddEnhancement(GameObject o, GameTime gameTime)
        {
            Tag(o);
            base.AddEnhancement(o, gameTime);
        }

        public override void RemoveEnhancement()
        {
            base.RemoveEnhancement();
        }
    }
}
