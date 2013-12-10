using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Horror
{
    public class Door : GameObject
    {
        public int index;
        public String doorTo, doorFrom;

        public Door(Vector2 pos, int i, String dT, String dF) : base(pos)
        {
            width = 32;
            height = 32;
            origin = Vector2.Zero;
            doorTo = dT;
            doorFrom = dF;
            index = i;
        }
    }
}
