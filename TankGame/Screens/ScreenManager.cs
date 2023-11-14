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
using TankGame.Objects.Entities;
using TankGame.Objects;
using TankGame.GameInfo;

namespace TankGame
{
    internal class ScreenManager
    {
        //protected SoundManager Music;
        //Random rand = new Random();

        //gets sent to GameState to inform the manager which screen to load
        public string nextScreen, selectedFile;   
        //puases the game when true
        protected bool pause = false;
        //mouse that every screen uses
        protected MouseState mouse;
        protected ButtonState curLeftClick, oldLeftClick, curRightClick, oldRightClick;
        protected Vector2 worldPosition;
        protected Point curGridLocation;
        //keystate every screen uses and keystate to calculate if a key is held
        protected KeyboardState keyState;
        protected KeyboardState keyHeldState;
        //spriteBatch for all the screens
        protected SpriteBatch spriteBatch;
        protected string relativePath;
        protected LevelManager levelManager;


        #region game information 
        protected Board curBoard;
        protected Point TanksAndMines;
        //the rows and columns
        protected int RowsCol, sweeps;
        //board info declares
        protected bool mouseInBoard = false;

        protected BoardState boardState, previousBoardState;
        #endregion

        #region Held functions

        //Holds Initialize
        public virtual void Initialize()
        {
            keyState = new KeyboardState();
            levelManager = new LevelManager();
            //curBoardState.entities.Clear();
            relativePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
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
            //find out the left and right mouse clicks
            oldLeftClick = curLeftClick;
            curLeftClick = mouse.LeftButton;
            oldRightClick = curRightClick;
            curRightClick = mouse.RightButton;

            //current rectange the mouse is in
            if (curBoard != null)
            {
                curBoard.getGridSquare(worldPosition, out curGridLocation);
            }
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

        protected void getMouseInBoard()
        {
            //find out if the mouse is inside the current board
            if (new RectangleF(curBoard.getInnerRectangle().Location, curBoard.getInnerRectangle().Size).Contains(worldPosition))
            {
                mouseInBoard = true;
            }
            else
            {
                mouseInBoard = false;
            }
        }
        #endregion

    }
}
