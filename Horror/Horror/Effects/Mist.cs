using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Horror
{
    class Mist : Effect
    {
        public Texture2D mist1, mist2;
        public Vector2 mist1Position, mist2Position;
        public Vector2 mist1Movement, mist2Movement, mistStart;
        GraphicsDevice view;

        public Mist(Camera camera, Player player, Texture2D mistOne, Texture2D mistTwo, GraphicsDevice viewport) 
            : base(camera, player) 
        {
            view = viewport;
            mist1 = mistOne;
            mist2 = mistTwo;
            mistStart = new Vector2(camera.screenWidth, 0);
            mist1Movement = mistStart;
            mist2Movement = Vector2.Zero;
        }

        /// <summary>
        /// Moves the mist and keeps it centered on the screen
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="player"></param>
        public override void Update()
        {
            mistStart = new Vector2(player.position.X + camera.screenWidth/2, -camera.Position.Y);
            stagger++;

            mist1Position = mistStart - mist1Movement;
            mist2Position = mistStart - mist2Movement;

            if (stagger % 2 == 0)
            {
                mist1Movement.X++;
                mist2Movement.X++;
            }
            
            //if (stagger % 120 == 0)
            //{
            //    Console.WriteLine("--------------------------------------------------------");
            //    Console.WriteLine("Mist Start:" + mistStart);
            //    Console.WriteLine("Mist 1: " + mist1Position + " Movement: " + mist1Movement);
            //    Console.WriteLine("Mist 2: " + mist2Position + " Movement: " + mist2Movement);
            //    Console.WriteLine("Camera: " + camera.Position);
            //    Console.WriteLine("--------------------------------------------------------");
            //    Console.WriteLine();
            //}

            if (mist1Position.X <= player.position.X - camera.screenWidth * 1.5)
            {
                Console.WriteLine("Resetting Mist 1: " + mist1Position + " Movement: " + mist1Movement);
                mist1Movement = Vector2.Zero;
            }
            if (mist2Position.X <= player.position.X - camera.screenWidth * 1.5)
            {
                Console.WriteLine("Resetting Mist 2: " + mist2Position + " Movement: " + mist2Movement);
                mist2Movement = Vector2.Zero;
            }
        }
    }
}
