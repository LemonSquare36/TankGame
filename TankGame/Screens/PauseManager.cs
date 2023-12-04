using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TankGame.Tools;
using TankGame.Objects.Entities;
using TankGame.Objects;
using TankGame.GameInfo;

namespace TankGame.Screens
{
    internal class PauseManager
    {
        //protected SoundManager Music;
        //Random rand = new Random();

        //gets sent to GameState to inform the manager which screen to load
        public string nextScreen;
        //boolean logic
        //mouse that every screen uses
        protected MouseState mouse;
        protected ButtonState curLeftClick, oldLeftClick, curRightClick, oldRightClick;
        protected Vector2 worldPosition;
        //keystate every screen uses and keystate to calculate if a key is held
        protected KeyboardState keyState;
        protected KeyboardState keyHeldState;
        //spriteBatch for all the screens
        protected SpriteBatch spriteBatch;
        bool firstPuase = true;

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
            SoundManager.LoadSoundSettings();
        }
        public virtual void UnloadContent()
        {

        }
        //Holds Update
        public virtual void Update()
        {
            getKeyState(out keyHeldState);
            worldPosition = MousePos();
            //find out the left and right mouse clicks and keeping thier prior state to see if its a new press or held
            oldLeftClick = curLeftClick;
            curLeftClick = mouse.LeftButton;
            oldRightClick = curRightClick;
            curRightClick = mouse.RightButton;

            //this handles what happens when escape gets pressed
            EscapeKeyManager();
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

        #region Manager
        //ButtonCLicked leads Here
        protected void ScreenChangeEvent(object sender, EventArgs e)
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
                //calculate the mouse position to scale and possible offset
                pos.X = (mouse.X / Camera.ResolutionScale.X) - (Main.graphicsDevice.Viewport.X / Camera.ResolutionScale.X);
                pos.Y = (mouse.Y / Camera.ResolutionScale.Y) - (Main.graphicsDevice.Viewport.Y / Camera.ResolutionScale.Y);
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
        private void EscapeKeyManager()
        {
            //first time the game is paused, the conditions for unpausing are true. This keeps the game from unpausing immedietly the first time
            if (firstPuase)
            {
                if (!keyState.IsKeyDown(Keys.Escape))
                {
                    firstPuase = false;
                }
            }
            //if escape is pressed then
            else if (keyState.IsKeyDown(Keys.Escape) && !keyHeldState.IsKeyDown(Keys.Escape))
            {
                GameState.Paused = false;
                ScreenManager.popupActive = false;
            }
        }
    }
}
