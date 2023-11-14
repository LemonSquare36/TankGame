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

namespace TankGame.Objects.Entities.Items
{
    internal class Items
    {
        //tells what item is currently selected
        private string selectedItem;
        public string SelectedItem
        {
            get { return selectedItem; }
        }

        //UI For when you need a warning in game (no items)
        protected bool ActiveItemWarning;
        protected Vector2 warningLocation;
        Stopwatch WarningSW = new Stopwatch();

        public void UseItem()
        {

        }
        public void DrawItem()
        {

        }
        /// <summary> This handles all the code the warning system needs for every item, besides for </summary>
        /// <param name="warningMessage">this string is what will be written to the current mouse location (doesnt update position)</param>
        /// <param name="warningLength">time in milliseconds to have the message on screen</param>
        private void ItemWarningSystem(SpriteBatch spriteBatch, string warningMessage, SpriteFont font, Color color, int warningLength, Vector2 messageLocation)
        {
            if (!WarningSW.IsRunning)
            {
                WarningSW.Start();
                warningLocation = messageLocation;
            }
            else
            {
                spriteBatch.DrawString(font, warningMessage, warningLocation, color);
            }
            if (WarningSW.ElapsedMilliseconds > warningLength)
            {
                WarningSW.Stop();
                WarningSW.Reset();
                ActiveItemWarning = false;
            }
        }
    }
}
