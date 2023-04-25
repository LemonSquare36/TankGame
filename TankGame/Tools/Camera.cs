using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Threading;

namespace TankGame.Tools
{
    internal class Camera
    {
        private Viewport viewport;
        public Viewport Viewport
        {
            get { return viewport; }
        }
        Vector2 Pos;
        Vector2 Size;

        float scaleX, scaleY; 
       
        public Camera()
        {
            
        }
        /// <param name="pos">Top left position of the viewport</param>
        /// <param name="size">Size of the viewport</param>
        public Camera(Point pos, Point size)
        {
            viewport = new Viewport(new Rectangle(pos, size));
            Pos = new Vector2(pos.X, pos.Y);
            Size = new Vector2(size.X, size.Y);
        }
        public Rectangle getBounds()
        {
            return viewport.Bounds;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale">Set to 1 or 0 for defualt scaling</param>
        /// <returns></returns>
        public Matrix getScalingMatrix(float scale)
        {
            if (scale == 0)
            {
                scale = 1;
            }
            getScale();
            //game is scaled to these amounts yo
             scaleX = (float)viewport.Width / Main.gameWindow.ClientBounds.Height;
             scaleY = (float)viewport.Height / Main.gameWindow.ClientBounds.Height;

            var translationMatrix = Matrix.CreateTranslation(new Vector3(Pos.X, Pos.Y, 0));
            var rotationMatrix = Matrix.CreateRotationZ(0);
            var scaleMatrix = Matrix.CreateScale(new Vector3(scaleX, scaleY, scale));
            var originMatrix = Matrix.CreateTranslation(new Vector3(Vector2.Zero.X, Vector2.Zero.Y, 0));

            return translationMatrix * rotationMatrix * scaleMatrix * originMatrix; ;
        }
        public void scaleCameraStart()
        {
            getScale();
            viewport.X *= Convert.ToInt16(scaleX);
            viewport.Y *= Convert.ToInt16(scaleY);
        }
        private Vector2 getScale()
        {
            scaleX = (float)viewport.Width / Main.gameWindow.ClientBounds.Width;
            scaleY = (float)viewport.Height / Main.gameWindow.ClientBounds.Height;
            return new Vector2(scaleX, scaleY);
        }
    }
}
