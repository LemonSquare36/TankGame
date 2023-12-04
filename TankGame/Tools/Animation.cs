using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Tools
{
    internal class Animation
    {
        public Texture2D texture;
        private float timerPerFrame, time;
        private int frame, row, col, totalFrames, curCycle, totalCycles;
        private Rectangle imageBox;
        private Rectangle[] animation;
        public bool cycleComplete = false, stopped = false;
        public Vector2 AnimationLocation = Vector2.Zero;

        /// <summary>
        /// Create a new Animation Class to handle spritesheet animations
        /// </summary>
        /// <param name="fileLocation">this is the spritesheet file location</param>
        /// <param name="Row">how many rows is the spritesheet</param>
        /// <param name="Col">how many columns is the spritesheet</param>
        /// <param name="TotalFrames">how many frames are there total</param>
        /// <param name="NumOfCycles">how many cycles of the full sheet to animate. -1 is forever</param>
        /// <param name="ImageBox">height and width of indivual sprites in the sheet</param>
        /// <param name="FullAnimationTime">how fast to play the animation, in milliseconds</param>
        public Animation(string fileLocation, int Row, int Col, int TotalFrames, int NumOfCycles, Rectangle ImageBox, float FullAnimationTime)
        {
            texture = Main.GameContent.Load<Texture2D>(fileLocation);
            row = Row;
            col = Col;
            totalFrames = TotalFrames;
            imageBox = ImageBox;
            //get the amount of time between each frames based on fps asked
            timerPerFrame = FullAnimationTime / totalFrames;
            time = 0;
            frame = 0;
            totalCycles = NumOfCycles;
            //create the 2d array of rectangles that stores the source retancles
            animation = new Rectangle[totalFrames];
            int curRow = 0;
            int curCol = 0;
            for (int i = 0; i < totalFrames; i++)
            {

                animation[i] = new Rectangle(new Point(imageBox.Width * curCol, imageBox.Height * curRow), imageBox.Size);

                curCol++;
                if (curCol == col)
                {
                    curRow++;
                    curCol = 0;
                }
            }
        }
        /// <summary>
        /// Create a new Animation Class to handle spritesheet animations
        /// </summary>
        /// <param name="fileLocation">this is the spritesheet file location</param>
        /// <param name="Row">how many rows is the spritesheet</param>
        /// <param name="Col">how many columns is the spritesheet</param>
        /// <param name="TotalFrames">how many frames are there total</param>
        /// /// <param name="NumOfCycles">how many cycles of the full sheet to animate. -1 is forever</param>
        /// <param name="ImageBox">height and width of indivual sprites in the sheet</param>
        /// <param name="FullAnimationTime">how fast to play the animation, in milliseconds</param>
        public Animation(string fileLocation, int Row, int Col, int TotalFrames, int NumOfCycles, Rectangle ImageBox, float FullAnimationTime, Vector2 animationLocation)
        {
            texture = Main.GameContent.Load<Texture2D>(fileLocation);
            row = Row;
            col = Col;
            totalFrames = TotalFrames;
            imageBox = ImageBox;
            //get the amount of time between each frames based on fps asked
            timerPerFrame = FullAnimationTime / totalFrames;
            time = 0;
            frame = 0;
            AnimationLocation = animationLocation;
            totalCycles = NumOfCycles;
            //create the 2d array of rectangles that stores the source retancles
            animation = new Rectangle[totalFrames];
            int curRow = 0;
            int curCol = 0;
            for (int i = 0; i < totalFrames; i++)
            {

                animation[i] = new Rectangle(new Point(imageBox.Width * curCol, imageBox.Height * curRow), imageBox.Size);

                curCol++;
                if (curCol == col)
                {
                    curRow++;
                    curCol = 0;
                }
            }
        }
        public Animation()
        {
        }
        /// <summary>
        /// plays an animation a specified number of times. Requires an animation reset between uses
        /// </summary>
        /// <param name="location">where the draw</param>
        /// <param name="NumOfCycles">how many times to cycle</param>
        public void PlayAnimation(SpriteBatch spriteBatch, Vector2 location, float scale)
        {
            if (totalCycles != -1)
            {
                if (curCycle < totalCycles)
                {
                    if (!cycleComplete)
                    {
                        frameTimer();
                        spriteBatch.Draw(texture, location, animation[frame], Color.White);
                    }
                    else
                    {
                        curCycle++;
                        cycleComplete = false;
                    }
                }
                else
                {
                    stopped = true;
                }
            }
            else
            {
                frameTimer();
                spriteBatch.Draw(texture, location, animation[frame], Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }
        /// <summary>
        /// plays an animation a specified number of times. Requires an animation reset between uses
        /// </summary>
        /// <param name="location">where the draw</param>
        /// <param name="NumOfCycles">how many times to cycle</param>
        public void PlayAnimation(SpriteBatch spriteBatch, float scale)
        {
            if (totalCycles != -1)
            {
                if (curCycle < totalCycles)
                {
                    if (!cycleComplete)
                    {
                        frameTimer();
                        spriteBatch.Draw(texture, AnimationLocation, animation[frame], Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    }
                    else
                    {
                        curCycle++;
                        cycleComplete = false;
                    }
                }
                else
                {
                    stopped = true;
                }
            }
            else
            {
                frameTimer();
                spriteBatch.Draw(texture, AnimationLocation, animation[frame], Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }
        /// <summary>
        /// used to cycle frames based on the time required by the animation
        /// </summary>
        private void frameTimer()
        {
            if (time > timerPerFrame)
            {
                frame++;
                if (frame == totalFrames)
                {
                    frame = 0;
                    cycleComplete = true;
                }
                timerPerFrame = 0;
            }
            else
            {
                time += (float)Main.gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }
        /// <summary>
        /// sets all the values used to track animation progress to defualt / 0
        /// </summary>
        public void ResetAnimation()
        {
            cycleComplete = false;
            curCycle = 0;
            stopped = false;
        }
    }
}
