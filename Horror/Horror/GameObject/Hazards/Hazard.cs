using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    public abstract class Hazard : GameObject
    {
        const float HAZARD_DEPTH = 0.92f;
        public int damage;
        public bool beingAnimated, disabled;

        public float rotation;

        public Player player;
        public List<Enemy> enemies;

        public Hazard(Vector2 position, Player plyr, List<Enemy> enmies) : base (position)
        {
            player = plyr;
            enemies = enmies;

            depth = HAZARD_DEPTH;
            beingAnimated = false;
            disabled = false;
            
            width = 32;
            height = 32;

            rotation = 0.0f;
        }

        /// <summary>
        /// Update method animates the hazard and checks for collisions between a player/enemy and a hazard
        /// </summary>
        /// <param name="map"></param>
        /// <param name="gameTime"></param>
        public virtual void Update(Map map, GameTime gameTime)
        {
            Animate();

            if(!disabled)
            {
                if (getBoundingBox().Intersects(player.getBoundingBox())) DoDamage(player, gameTime);
                foreach(Enemy e in enemies)
                    if(getBoundingBox().Intersects(e.getBoundingBox())) DoDamage(e, gameTime);
            }
        }

        /// <summary>
        /// Do damage to the character who stepped on the hazard
        /// </summary>
        /// <param name="character"></param>
        /// <param name="gameTime"></param>
        public virtual void DoDamage(Character character, GameTime gameTime)
        {
            beingAnimated = true;
            character.TakeHazardDamage(damage, gameTime);
        }

        /// <summary>
        /// Animates the hazard
        /// </summary>
        public override void Animate()
        {
            animationStagger++;
            sourceFrame = new Rectangle((frameCount-1) * (int)width, 0, (int)width, (int)height);

            if (frameCount == totalFrames) beingAnimated = false;

            else if (beingAnimated && animationStagger % 16 == 0 && frameCount < totalFrames) frameCount++; 
        }
    }
}
