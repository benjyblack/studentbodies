using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Horror
{
    public class Player : Character
    {
        public const float PLAYER_DEPTH = 0.90f;
        public const float PLAYER_SPEED = 0.8f;
        public const int PLAYER_HEALTH = 10;
        public const int PLAYER_WIDTH = 155;
        public const int PLAYER_HEIGHT = 119;
        private int regainHealthStagger;
        private float slowTimer;
        private float baseSpeed;

        public List<Key> keys;

        private SoundEffectInstance gasp;

        public Player(Vector2 position, Map _map, SoundEffectInstance _gasp)
            : base(position, _map)
        {
            totalFrames = 3;
            depth = PLAYER_DEPTH;
            width = PLAYER_WIDTH/totalFrames;
            height = PLAYER_HEIGHT;
            origin = new Vector2(width / 2, height / 2);
            
            health = PLAYER_HEALTH;
            baseSpeed = speed = PLAYER_SPEED;
            
            regainHealthStagger = 0;

            sourceFrame = new Rectangle((frameCount - 1) * (int)(width), 0, (int)(width), (int)height);

            gasp = _gasp;

            origin = new Vector2(width / 2, height - height / 4);

            keys = new List<Key>();
        }

        /// <summary>
        /// Updates the player
        /// </summary>
        /// <param name="currentStateGamePad"></param>
        /// <param name="map"></param>
        /// <param name="gameTime"></param>
        public void Update(GamePadState currentStateGamePad, KeyboardState currentStateKeyboard, GameTime gameTime)
        {
            Move(currentStateGamePad, currentStateKeyboard);
            RegainHealth(gameTime);
            Animate();
            animationStagger++;

            //Handle special slowness case
            if (isSlowed && gameTime.TotalGameTime.Seconds > slowTimer)
            {
                isSlowed = false;
                speed = Player.PLAYER_SPEED;
            }

        }

        /// <summary>
        /// Player regenerates one point of health every 8 seconds
        /// </summary>
        /// <param name="gameTime"></param>
        protected void RegainHealth(GameTime gameTime)
        {
            if (health < 10 && recoveryTime < gameTime.TotalGameTime.TotalSeconds && regainHealthStagger < gameTime.TotalGameTime.TotalSeconds)
            {
                    Console.WriteLine("Player Health = " + health + " + 1 = " + (health+1));
                    health++;
                    regainHealthStagger = (int)gameTime.TotalGameTime.TotalSeconds + 2;
            }
        }

        /// <summary>
        /// Moves the player
        /// </summary>
        /// <param name="currentStateGamePad"></param>
        /// <param name="map"></param>
        protected void Move(GamePadState currentStateGamePad, KeyboardState currentStateKeyboard)
        {
            speed = baseSpeed + currentStateGamePad.Triggers.Right*2;

            Step(new Vector2(currentStateGamePad.ThumbSticks.Left.X * (this.speed + 1), 
                (- currentStateGamePad.ThumbSticks.Left.Y * (this.speed + 1))));

            if(currentStateKeyboard.IsKeyDown(Keys.W))  position.Y--;
            if(currentStateKeyboard.IsKeyDown(Keys.S))  position.Y++;
            if(currentStateKeyboard.IsKeyDown(Keys.A))  position.X--;
            if(currentStateKeyboard.IsKeyDown(Keys.D))  position.X++;
        }

        /// <summary>
        /// Allows the player to take damage
        /// </summary>
        /// <param name="opponent"></param>
        /// <param name="dmg"></param>
        /// <param name="gameTime"></param>
        public override void TakeDamage(int dmg, GameTime gameTime)
        {
            gasp.Play();
            base.TakeDamage(dmg, gameTime);
            Console.WriteLine("Player took " + dmg + " damage. New health: " + health);
            recoveryTime = (int)gameTime.TotalGameTime.TotalSeconds + 5;
        }

        /// <summary>
        /// Self-explanatory
        /// (player has their own because of recoveryTime and because of the gasp)
        /// </summary>
        /// <param name="dmg"></param>
        /// <param name="gameTime"></param>
        public override void TakeHazardDamage(int dmg, GameTime gameTime)
        {
            if (hazardDamageStagger <= gameTime.TotalGameTime.TotalSeconds)
            {
                gasp.Play();
                hazardDamageStagger = (int)gameTime.TotalGameTime.TotalSeconds + 1;
                health -= dmg;
                Console.WriteLine("Player took " + dmg + " damage. New health: " + health);
            }
            recoveryTime = (int)gameTime.TotalGameTime.TotalSeconds + 5;
        }

        /// <summary>
        /// Animates the player
        /// </summary>
        public override void Animate()
        {
            if (!GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Equals(Vector2.Zero)) base.Animate();
        }

        /// <summary>
        /// Slow the player down
        /// </summary>
        /// <param name="gameTime"></param>
        public void Slow(GameTime gameTime)
        {
            isSlowed = true;
            slowTimer = gameTime.TotalGameTime.Seconds + 3;

            speed /= 2;
        }
    }
}
