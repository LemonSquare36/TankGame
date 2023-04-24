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

namespace TankGame
{
    internal class ScreenManager
    {
        //protected SoundManager Music;
        //Random rand = new Random();

        protected KeyboardState Key;
        protected SpriteBatch spriteBatch;
        //gets sent to GameState to inform the manager which screen to load
        protected string nextScreen;   
        //puases the game when true
        protected bool pause = false;

        

        //Holds Initialize
        public virtual void Initialize()
        {

        }
        //Holds LoadContent and the font if called
        public virtual void LoadContent(SpriteBatch spriteBatchmain)
        {

        }
        //Holds Update
        public virtual void Update()
        {

        }
        //Holds Draw
        public virtual void Draw()
        {

        }
        public void getKey()
        {
            Key = Keyboard.GetState();
        }

        //Holds the Function
        public virtual void ButtonReset()
        {

        }
        //Gets the next screen
        public string getNextScreen()
        {
            return nextScreen;
        }

        #region Game Screen Manager
        ///////////////////////////////GAME SCREEN MANAGER STUFF//////////////////////////////////

        
        #endregion

        #region MenuManager
        ///////////////////////////////MENU MANAGER STUFF////////////////////////
        protected MouseState mouse;

        //ButtonCLicked leads Here
        /*protected void ButtonClicked(object sender, EventArgs e)
        {
            //Sets next screen to button name and calls the event.
            switch (((Button)sender).purpose)
            {
                case 0:
                    nextScreen = ((Button)sender).bName;
                    OnScreenChanged();
                    break;
            }
        }*/
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
        protected Vector2 MousePos()
        {
            Vector2 worldPosition = Vector2.Zero;
            mouse = Mouse.GetState();
            try
            {
                worldPosition.X = mouse.X / (float)(Main.gameWindow.ClientBounds.Width / 1920.0);
                worldPosition.Y = mouse.Y / (float)(Main.gameWindow.ClientBounds.Height / 1080.0);
            }
            catch { }
            return worldPosition;
        }
        #endregion

    }
}
