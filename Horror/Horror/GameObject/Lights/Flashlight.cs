using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    public class Flashlight : DirectionalLight
    {
        public const int FLASHLIGHT_WIDTH = 200;
        public const int FLASHLIGHT_HEIGHT = 100;

        public Flashlight(Player player)
            : base(player)
        {
            width = FLASHLIGHT_WIDTH;
            height = FLASHLIGHT_HEIGHT;

            bodyWidth = 38;
            bodyHeight = 25;

            distanceToSpot = 175;
            color = Color.Yellow * 0.55f;

            origin = new Vector2(0, 12);
            lightEffectOrigin = new Vector2(20 - bodyWidth, 48);
            lightCutoutOrigin = new Vector2(1415 - bodyWidth, 1497);
        }

        public override void AddEnhancement(float timer, GameTime gameTime)
        {
            color = Color.Red * 0.5f;
            base.AddEnhancement(timer, gameTime);
        }

        public override void RemoveEnhancement()
        {
            color = Color.Yellow * 0.5f;
            base.RemoveEnhancement();
        }
    
        public override void UseEnhancement(Enemy enemy)
        {
            if (enhanced)
                enemy.Orient(spotOrigin);
            else
                enemy.OrientAway(spotOrigin);
            enemy.Step(enemy.heading * enemy.speed);
        }
    }
}
