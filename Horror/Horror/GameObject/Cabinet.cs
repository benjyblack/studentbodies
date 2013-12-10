using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Horror
{
    public class Cabinet : GameObject
    {
        public Cabinet()
            : base(Map.coordinatesToPixels(new Vector2(23, 2)))
        {
            width = 109;
            height = 117;
            origin = Vector2.Zero;
        }
    }
}
