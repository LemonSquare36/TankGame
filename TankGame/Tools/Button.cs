﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TankGame.Objects;

namespace TankGame
{
    internal class Button
    {
        RectangleF rectangle;
        public Vector2 Pos = new Vector2();
        private Texture2D unPressed, pressed;
        public Texture2D Texture;
        private MouseState mouse;
        public ButtonState oldClick;
        public ButtonState curClick;

        //Holds the Name or Function the button does
        private string Bname;
        public string bName
        {
            get { return Bname; }
        }
        private int Purpose;
        public int purpose
        {
            get { return Purpose; }
        }
        //Event for when you click the button
        private event EventHandler buttonClicked;
        public event EventHandler ButtonClicked
        {
            add { buttonClicked += value; }
            remove { buttonClicked -= value; }
        }
        //Resets the Current and Old Click for the buttons
        public void ButtonReset()
        {
            curClick = ButtonState.Pressed;
            oldClick = ButtonState.Pressed;
        }

        /// <summary>Create the Image, HitBox, and eventInformation when calling the button in this Constructer</summary>
        /// <param name="pos">Where the button is in the game world</param>
        /// <param name="width">How wide is the TEX</param>
        /// <param name="height">How tall is the TEX</param>
        /// <param name="Unpressed">Its not pressed (highlighted) TEX</param>
        /// <param name="Pressed">Its  pressed (highlighted) TEX</param>
        /// <param name="ButtonName">For a switch case. Name tells it what event to call</param>
        /// <param name="ButtonPurpose">0 for changeScreen, 1 for other</param>
        public Button(Vector2 pos, float width, float height, Texture2D Unpressed, Texture2D Pressed, string ButtonName, int ButtonPurpose)//Button Name is super important becuase it determines what it does
        {
            curClick = ButtonState.Pressed;
            oldClick = ButtonState.Pressed;
            Pos = pos;
            rectangle = new RectangleF(pos.X, pos.Y, width, height);
            unPressed = Unpressed;
            pressed = Pressed;
            Bname = ButtonName;
            Texture = unPressed;
            Purpose = ButtonPurpose;
        }
        //Reads for inputs of the mouse in correspondence for the button

        public void Update(MouseState Mouse, Vector2 worldMousePosition)
        {
            mouse = Mouse;
            Texture = unPressed;
            oldClick = curClick;
            curClick = mouse.LeftButton;
            if (rectangle.Contains(worldMousePosition))
            {
                Texture = pressed;
                //Edge Detection
                if (curClick == ButtonState.Pressed && oldClick == ButtonState.Released)
                {
                    OnButtonClicked();
                }
            }
        }
        //Draws the Buttons
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Pos, Color.White);
        }

        //Button Event
        private void OnButtonClicked()
        {
            buttonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
