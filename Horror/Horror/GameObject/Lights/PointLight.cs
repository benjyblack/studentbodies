using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    public class PointLight : LightSource
    {
        Random lightFlicker = new Random();

        public const int POINTLIGHT_HEIGHT = 300;
        public const int POINTLIGHT_WIDTH = 288;

        public const float POINTLIGHT_SCALE = 1.0f;

        public PointLight(Player player)
            : base(player)
        {
            height = POINTLIGHT_HEIGHT;
            width = POINTLIGHT_WIDTH;
            lightScale = POINTLIGHT_SCALE;

            bodyWidth = 20;
            bodyHeight = 40;

            color = Color.Orange * 0.7f;

            origin = new Vector2(bodyWidth / 2, 0);
            lightEffectOrigin = new Vector2(width / 2, height / 2);
            lightCutoutOrigin = new Vector2(cutoutWidth / 2, cutoutHeight / 2);
            spotOrigin = new Vector2(this.position.X + this.width / 2, this.position.Y + this.height / 2);
        }

        public override void Update(Microsoft.Xna.Framework.Input.GamePadState currentStateGamePad, GameTime gameTime)
        {
            position = new Vector2(player.position.X + player.width/2, player.position.Y - player.height/3);
            spotOrigin = this.position;

            base.Update(currentStateGamePad, gameTime);
        }

        public override void Animate()
        {
            animationStagger++;
            if (animationStagger % 8 == 0)
            {
                int randomInt = lightFlicker.Next(650, 800);
                float randomFloat = (float)randomInt / 1000;
                color = Color.Orange * randomFloat;
            }
        }

        public override void AddEnhancement(float timer, GameTime gameTime)
        {
            base.AddEnhancement(timer, gameTime);
            lightScale = POINTLIGHT_SCALE * 2;
        }

        public override void RemoveEnhancement()
        {
            lightScale = POINTLIGHT_SCALE;
            base.RemoveEnhancement();
        }
    }
}
