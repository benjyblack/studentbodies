using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Horror
{
    public class TimedHazard : Hazard
    {
        const float HAZARD_DEPTH = 0.92f;
        public float creationTime;

        public TimedHazard(Vector2 position, Player plyr, List<Enemy> enmies, GameTime gameTime)
            : base(position, plyr, enmies)
        {
            creationTime = gameTime.TotalGameTime.Seconds;
        }
    }
}
