﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
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
        protected bool anyObjectActive = false, escapePressed = false, justPaused, justUnPuased;
        public static bool popupActive = false;
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
        protected RuleSet rules;

        #region game information 
        protected Board curBoard;
        //the rows and columns
        protected int RowsCol;
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
            rules = new RuleSet(2);
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
            if (!popupActive)
            {
                getKeyState(out keyHeldState);
                worldPosition = MousePos();
                //find out the left and right mouse clicks
                oldLeftClick = curLeftClick;
                curLeftClick = mouse.LeftButton;
                oldRightClick = curRightClick;
                curRightClick = mouse.RightButton;
            }
            else if (justPaused)
            {
                keyState = new KeyboardState();
                keyHeldState = new KeyboardState();
                curLeftClick = new ButtonState();
                oldLeftClick = new ButtonState();
                curRightClick = new ButtonState();
                oldRightClick = new ButtonState();
                justPaused = false;
                justUnPuased = true;
            }

            //current rectange the mouse is in
            if (curBoard != null)
            {
                if (curBoard.gridArrayAccessible)
                curBoard.getGridSquare(worldPosition, out curGridLocation);
            }
            //this handles what happens when escape gets pressed
            if (justUnPuased && !popupActive)
            {
                justUnPuased = false;
                //keyHeldState = keyState;
            }
            else
                EscapeKeyManager();

            //popupActive is used for other popups including the puasescreen in gamestate
            if (GameState.Paused)
                popupActive = true;
        }
        //Holds Draw
        public virtual void Draw()
        {

        }
        protected void DrawWalls()
        {
            foreach (Wall wall in boardState.walls)
            {
                wall.Draw(spriteBatch);
            }
            //this prevents overlapping opaque textures when drawing the shadows
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Main.DefualtMatrix());
            foreach (Wall wall in boardState.walls)
            {
                wall.DrawShadows(spriteBatch);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.DefualtMatrix());
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
        private void EscapeKeyManager()
        {
            //escape isnt currently "pressed" for anything that needs that info
            escapePressed = false;
            //if escape is pressed then
            if (keyState.IsKeyDown(Keys.Escape) && !keyHeldState.IsKeyDown(Keys.Escape) && !popupActive && !justUnPuased)
            {
                //if it "wasnt" but is
                if (!escapePressed)
                {
                    //and a button is active
                    if (anyObjectActive)
                    {
                        //set escape to pressed and button active to false
                        anyObjectActive = false;
                        escapePressed = true;
                    }
                    //if no active buttons, and the game is paused, then we will unpuase the game
                    else if (GameState.Paused)
                    {
                        
                    }
                    //if escape gets pressed without any active buttons, then pause the game
                    else
                    {
                        //puase code
                        escapePressed = true;
                        GameState.Paused = true;
                        justPaused = true;
                    }
                }
            }
            anyObjectActive = false;
        }
    }
}
