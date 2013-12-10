using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    abstract public class DirectionalLight : LightSource
    {
        public DirectionalLight(Player player)
            : base(player)
        {
        }

        public override void Update(Microsoft.Xna.Framework.Input.GamePadState currentStateGamePad, GameTime gameTime)
        {
            position = new Vector2(player.position.X + player.width/2 - 8, player.position.Y - player.height / 8);
            spotOrigin = new Vector2(this.position.X + (float)(Math.Cos(this.lightRotation) * distanceToSpot), 
                this.position.Y + (float)(Math.Sin(this.lightRotation) * distanceToSpot));
            lightRotation = (float)Math.Atan2(-(currentStateGamePad.ThumbSticks.Right.Y), currentStateGamePad.ThumbSticks.Right.X);

            base.Update(currentStateGamePad, gameTime);
        }

        public override void AddEnhancement(float timer, GameTime gameTime)
        {
            base.AddEnhancement(timer, gameTime);
        }

        public override void RemoveEnhancement()
        {
            base.RemoveEnhancement();
        }
    }
}
