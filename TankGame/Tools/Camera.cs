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
using System.ComponentModel.Design;

namespace TankGame.Tools
{
    internal class Camera
    {
        //the current resolution of the game
        private static Vector2 Resolution;
        public static Vector2 resolution
        {
            get { return Resolution; } set { Resolution = value; }          
        }
        private static Vector2 bounds;
        private static Vector2 Bounds
        {
            get { return bounds; }  set { bounds = value; }
        }
        public static Vector2 ResolutionScale
        {
            get { return ScaleToResolution(); }           
        }
        private static Vector2 camPosition;
        private static Vector2 CamPosition
        {
            get { return camPosition; }  set { camPosition = value; }
        }

        ///<summary>Scales the batch to the proper resolution within the bounds. Call setBounds() First </summary>
        /// <param name="scaleX">The Scale on the X axis for drawing</param>
        /// <param name="scaleY">The Scale on the Y axis for drawing</param>
        /// <returns></returns>  
        public static Matrix getScalingMatrix(float scaleX, float scaleY)
        {
            if (scaleX == 0)
            {
                scaleX = 1;
            }
            if (scaleY == 0)
            {
                scaleY = 1;
            }

            var translationMatrix = Matrix.CreateTranslation(new Vector3(CamPosition.X, CamPosition.Y, 0));
            var rotationMatrix = Matrix.CreateRotationZ(0);
            var scaleMatrix = Matrix.CreateScale(new Vector3(scaleX, scaleY, 0));
            var originMatrix = Matrix.CreateTranslation(new Vector3(Vector2.Zero.X, Vector2.Zero.Y, 0));

            return translationMatrix * rotationMatrix * scaleMatrix * originMatrix; ;
        }
        ///<summary>Scales the batch to the proper resolution within the bounds. Call setBounds() First </summary>
        /// <param name="scaleX">The Scale on the X axis for drawing</param>
        /// <param name="scaleY">The Scale on the Y axis for drawing</param>
        /// <param name="scale">Additional Scaling for special cases</param>
        public static Matrix getScalingMatrix(float scaleX, float scaleY, float scale)
        {
            if (scaleX == 0)
            {
                scaleX = 1;
            }
            if (scaleY == 0)
            {
                scaleY = 1;
            }
            scaleX *= scale;
            scaleY *= scale;
            var translationMatrix = Matrix.CreateTranslation(new Vector3(CamPosition.X, CamPosition.Y, 0));
            var rotationMatrix = Matrix.CreateRotationZ(0);
            var scaleMatrix = Matrix.CreateScale(new Vector3(scaleX, scaleY, 0));
            var originMatrix = Matrix.CreateTranslation(new Vector3(Vector2.Zero.X, Vector2.Zero.Y, 0));

            return translationMatrix * rotationMatrix * scaleMatrix * originMatrix; ;
        }


        /// <summary>gets the game scale, resolution vs bounds</summary>
        private static Vector2 ScaleToResolution()
        {
            return new Vector2((float)Main.gameWindow.ClientBounds.Width / (float)resolution.X, (float)Main.gameWindow.ClientBounds.Height / (float)resolution.Y);
        }
        /// <summary>gets the bounds of the current active viewport</summary>
        public static void setBound(Viewport view)
        {
            Bounds = new Vector2(view.Width, view.Height);
            CamPosition = new Vector2(view.X, view.Y);
        }
    }
}
