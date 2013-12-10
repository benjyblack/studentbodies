using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/***Adapted from stackoverflow***/

namespace Horror
{
    public class Camera
    {
        public int screenWidth;
        public int screenHeight;
        public float scaleX;
        public float scaleY;
        GraphicsDeviceManager graphics;
        Player player;
        public Camera(int width, int height, Player plyr, GraphicsDeviceManager grafx)
        {
            graphics = grafx;
            player = plyr;
            Zoom = 1f;
            screenWidth = 800;
            screenHeight = 480;

            Position = Vector2.Zero;

            scaleX = (float)graphics.GraphicsDevice.Viewport.Width / 800f;
            scaleY = (float)graphics.GraphicsDevice.Viewport.Height / 480f;
        }

        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public float Zoom { get; set; }

        public Matrix TransformMatrix
        {
            get
            {
                return Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(scaleX, scaleY, 1) *
                       Matrix.CreateTranslation(Position.X, Position.Y, 0);
            }
        }

        public void Update()
        {
            Position = new Vector2(scaleX, scaleY) * new Vector2(-((player.position.X + player.width * (-(scaleX - 1))) + player.width / 2 - screenWidth/2),
                -((player.position.Y + player.height * (-(scaleY - 1))) + player.height / 2 - screenHeight/2));

        }
    }
}
