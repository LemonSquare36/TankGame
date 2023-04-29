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
using TankGame.Tools;

namespace TankGame
{
    internal class ScreenManager
    {
        //protected SoundManager Music;
        //Random rand = new Random();

        //gets sent to GameState to inform the manager which screen to load
        protected string nextScreen;   
        //puases the game when true
        protected bool pause = false;
        //mouse that every screen uses
        protected MouseState mouse;
        protected Vector2 worldPosition;
        //keystate every screen uses and keystate to calculate if a key is held
        protected KeyboardState keyState;
        protected KeyboardState keyHeldState;
        //spriteBatch for all the screens
        protected SpriteBatch spriteBatch;

        #region Held functions

        //Holds Initialize
        public virtual void Initialize()
        {
            keyState = new KeyboardState();
        }
        //Holds LoadContent and the font if called
        public virtual void LoadContent(SpriteBatch spriteBatchmain)
        {
            spriteBatch = spriteBatchmain;
        }
        //Holds Update
        public virtual void Update()
        {
            getKeyState(out keyHeldState);
            worldPosition = MousePos();
        }
        //Holds Draw
        public virtual void Draw()
        {

        }

        //Holds the Function
        public virtual void ButtonReset()
        {

        }
        #endregion

        //Gets the next screen
        public string getNextScreen()
        {
            return nextScreen;
        }

        #region Manager


        //ButtonCLicked leads Here
        protected void ButtonClicked(object sender, EventArgs e)
        {
            //Sets next screen to button name and calls the event.
            switch (((Button)sender).purpose)
            {
                case 0:
                    nextScreen = ((Button)sender).bName;
                    OnScreenChanged();
                    break;
            }
        }
        //Event for Changing the Screen
        public event EventHandler ChangeScreen;
        public event EventHandler buttonPressed;
        public void OnScreenChanged()
        {
            ChangeScreen?.Invoke(this, EventArgs.Empty);
        }
        public void OnButtonPressed()
        {
            buttonPressed?.Invoke(this, EventArgs.Empty);
        }

        //calculates the accurate mouse position so the screen can scale
        private Vector2 MousePos()
        {
            Vector2 pos = Vector2.Zero;
            mouse = Mouse.GetState();
            try
            {
                pos.X = mouse.X / ((float)(Main.graphicsDevice.Viewport.Width / Camera.resolution.X));
                pos.Y = mouse.Y / ((float)(Main.graphicsDevice.Viewport.Height / Camera.resolution.Y));
            }
            catch { }
            return pos;
        }

        private void getKeyState(out KeyboardState oldState)
        {
            oldState = keyState;
            keyState = Keyboard.GetState();
        }
        #endregion

    }
}
