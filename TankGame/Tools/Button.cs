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
        private bool toggle = false;

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
        public Texture2D UnPressed { get { return unPressed; } }
        public Texture2D Pressed { get { return pressed; } }

        /// <summary>Create the Image, HitBox, and eventInformation when calling the button in this Constructer</summary>
        /// <param name="pos">Where the button is in the game world</param>
        /// <param name="width">How wide is the TEX</param>
        /// <param name="height">How tall is the TEX</param>
        /// <param name="fileLocation">The location of the two files that make a button. Ex: load for loadUH and loadH</param>
        /// <param name="ButtonName">For a switch case. Name tells it what event to call</param>
        /// <param name="ButtonPurpose">0 for changeScreen, 1 for other - only need if leaving the class it belongs to</param>
        public Button(Vector2 pos, float width, float height, string fileLocation, string ButtonName, int ButtonPurpose)//Button Name is super important becuase it determines what it does
        {
            curClick = ButtonState.Pressed;
            oldClick = ButtonState.Pressed;
            Pos = pos;
            rectangle = new RectangleF(pos.X, pos.Y, width, height);
            unPressed = Main.GameContent.Load<Texture2D>(fileLocation+"UH");
            pressed = Main.GameContent.Load<Texture2D>(fileLocation+"H");
            Bname = ButtonName;
            Texture = unPressed;
            Purpose = ButtonPurpose;
        }
        /// <summary>Create the Image, HitBox, and eventInformation when calling the button in this Constructer</summary>
        /// <param name="pos">Where the button is in the game world</param>
        /// <param name="width">How wide is the TEX</param>
        /// <param name="height">How tall is the TEX</param>
        /// <param name="fileLocation">The location of the two files that make a button. Ex: load for loadUH and loadH</param>
        /// <param name="ButtonName">For a switch case. Name tells it what event to call</param>
        public Button(Vector2 pos, float width, float height, string fileLocation, string ButtonName)//Button Name is super important becuase it determines what it does
        {
            curClick = ButtonState.Pressed;
            oldClick = ButtonState.Pressed;
            Pos = pos;
            rectangle = new RectangleF(pos.X, pos.Y, width, height);
            unPressed = Main.GameContent.Load<Texture2D>(fileLocation + "UH");
            pressed = Main.GameContent.Load<Texture2D>(fileLocation + "H");
            Bname = ButtonName;
            Texture = unPressed;
        }
        /// <summary>Create the Image, HitBox, and eventInformation when calling the button in this Constructer</summary>
        /// <param name="pos">Where the button is in the game world</param>
        /// <param name="width">How wide is the TEX</param>
        /// <param name="height">How tall is the TEX</param>
        /// <param name="fileLocation">The location of the two files that make a button. Ex: load for loadUH and loadH</param>
        /// <param name="ButtonName">For a switch case. Name tells it what event to call</param>
        /// <param name="buttonBehaviour">Determines abnormal button behavior - (toggle): Second texutre toggles by click / (oneTex): Texture only used one texture. Doesnt ad H\UH</param>
        public Button(Vector2 pos, float width, float height, string fileLocation, string ButtonName, string buttonBehaviour)//Button Name is super important becuase it determines what it does
        {
            curClick = ButtonState.Pressed;
            oldClick = ButtonState.Pressed;
            Pos = pos;
            if(buttonBehaviour == "oneTex")
            {
                unPressed = Main.GameContent.Load<Texture2D>(fileLocation);
                pressed = Main.GameContent.Load<Texture2D>(fileLocation);
            }
            else
            {
                unPressed = Main.GameContent.Load<Texture2D>(fileLocation + "UH");
                pressed = Main.GameContent.Load<Texture2D>(fileLocation + "H");
            }
            if (buttonBehaviour == "toggle")
            {
                toggle = true;
            }
            rectangle = new RectangleF(pos.X, pos.Y, width, height);
            
            Bname = ButtonName;
            Texture = unPressed;
        }
        //Reads for inputs of the mouse in correspondence for the button

        public void Update(MouseState Mouse, Vector2 worldMousePosition)
        {
            mouse = Mouse;       
            oldClick = curClick;
            curClick = mouse.LeftButton;
            if (!toggle)
            {
                Texture = unPressed;
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
            else if (toggle)
            {
                if (rectangle.Contains(worldMousePosition))
                {      
                    //Edge Detection
                    if (curClick == ButtonState.Pressed && oldClick == ButtonState.Released)
                    {
                        if (Texture == pressed)
                            Texture = unPressed;

                        else Texture = pressed;
                        OnButtonClicked();
                    }
                }

            }
        }
        //Draws the Buttons
        public void Draw(SpriteBatch spriteBatch)
        {
            //if a texture is a different size when pressed, offset it approprietly
            if (Texture == pressed)
            {
                if (unPressed.Bounds != pressed.Bounds)
                {
                    Vector2 tempPos = new Vector2();
                    if (unPressed.Bounds.Size.X > pressed.Bounds.Size.X)
                    {
                        tempPos -= new Vector2(unPressed.Bounds.Width - pressed.Bounds.Width, unPressed.Bounds.Height - pressed.Bounds.Height)/2;
                    }
                    else if (unPressed.Bounds.Size.X < pressed.Bounds.Size.X)
                    {
                        tempPos -= new Vector2(pressed.Bounds.Width - unPressed.Bounds.Width, pressed.Bounds.Height - unPressed.Bounds.Height)/2;
                    }
                    spriteBatch.Draw(Texture, Pos + tempPos, Color.White);
                }
                else { spriteBatch.Draw(Texture, Pos, Color.White); }

            }
            //draw normally if not pressed
            else { spriteBatch.Draw(Texture, Pos, Color.White); }
        }

        //Button Event
        private void OnButtonClicked()
        {
            buttonClicked?.Invoke(this, EventArgs.Empty);
        }
        public void toggleTexture()
        {                       
            if (Texture == pressed)
                Texture = unPressed;

            else Texture = pressed;
        }
    }
}
