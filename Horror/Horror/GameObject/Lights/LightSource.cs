using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    /***Explanation of Lightsource objects***/
    /* Lightsources are composed of three parts, the body, the effect and the cutout. 
     * The body is the physical part of the object, the effect is the emitted light,
     * and the cutout is the big black texture with a cutout for the effect (makes you only
     * able to see the area covered by the flashlight beam or candle flicker) */
    abstract public class LightSource : GameObject
    {
        public Player player;
        public Vector2 lightEffectOrigin, lightCutoutOrigin, spotOrigin;

        public float lightScale, lightRotation, enhancementTimer;
        public bool enhanced;
        public int bodyWidth, bodyHeight, cutoutWidth, cutoutHeight, distanceToSpot;

        public LightSource(Player plyr)
            : base(Vector2.Zero)
        {
            bodyWidth = 0;
            bodyHeight = 0;
            cutoutWidth = 3000;
            cutoutHeight = 3000;

            depth = 0.76f;

            distanceToSpot = 0;

            lightEffectOrigin = Vector2.Zero;
            lightCutoutOrigin = Vector2.Zero;
            lightScale = 1.0f;
            lightRotation = 0.0f;
            enhanced = false;
            enhancementTimer = 0.0f;

            player = plyr;
        }

        public virtual void Update(Microsoft.Xna.Framework.Input.GamePadState currentStateGamePad, GameTime gameTime)
        {
            Animate();
            if (enhanced && enhancementTimer <= gameTime.TotalGameTime.TotalSeconds) RemoveEnhancement();
        }
        
        public virtual void AddEnhancement(float timer, GameTime gameTime)
        {
            enhanced = true;
            enhancementTimer = timer + (int)gameTime.TotalGameTime.TotalSeconds;
        }

        public virtual void AddEnhancement(GameObject o, GameTime gameTime)
        {
        }

        public virtual void RemoveEnhancement()
        {
            enhanced = false;
        }

        public virtual void UseEnhancement(Enemy enemy)
        {
        }
    }
}
