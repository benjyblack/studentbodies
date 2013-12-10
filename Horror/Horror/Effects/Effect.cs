using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    abstract public class Effect
    {
        protected Camera camera;
        protected Player player;

        public int stagger;

        public Effect(Camera cmr, Player plyr) 
        {
            camera = cmr;
            player = plyr;
            stagger = 0;
        }

        virtual public void Update() { }
    }
}
