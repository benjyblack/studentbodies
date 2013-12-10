using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{

    public class Key : GameObject
    {
        public String toWhere;
        public bool taken;
        public Key(String _toWhere, Vector2 position)
            : base(position)
        {
            width = 32;
            height = 32;
            toWhere = _toWhere;
            taken = false;
        }
    }
}
