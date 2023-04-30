﻿using System;
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
using System.Diagnostics.Tracing;

namespace TankGame.Tools
{
    internal class InputBox
    {
        Texture2D tex;
        SpriteFont font;
        Color bgColor, textColor, selectedColor;
        new Vector2 pos, size;
        RectangleF rectangle;
        //mouse stuff for clicking the textbox (selecting it)
        private MouseState mouse;
        public ButtonState oldClick;
        public ButtonState curClick;

        bool active = false;


        KeyStrokeHandler KeyHandler = new KeyStrokeHandler();

        public string text = "";

        public InputBox(Color backgroundColor, Color TextColor, Vector2 Pos, Vector2 Size)
        {
            bgColor = backgroundColor;
            selectedColor = new Color(backgroundColor.R - 50, backgroundColor.G - 50, backgroundColor.B - 50);
            textColor = TextColor;
            pos = Pos;
            size = Size;
            rectangle = new RectangleF(pos, size);
        }
        public void Initialize()
        {

        }
        public void LoadContent()
        {
            //load the texture for the BG
            tex = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
            //load the font
            font = Main.GameContent.Load<SpriteFont>("Fonts/DefualtFont");
            //prepare the dictionary
            KeyStrokeHandler.populateAlphabet();
        }
        public void Update(MouseState Mouse, Vector2 worldMousePosition, KeyboardState keystate, KeyboardState keyHeldState)
        {
            mouse = Mouse;
            oldClick = curClick;
            curClick = mouse.LeftButton;
            //checks to see if the mouse is in the textbox
            if (rectangle.Contains(worldMousePosition))
            {
                //if the mouse clicks from in the box set it to active
                if (curClick == ButtonState.Pressed && oldClick == ButtonState.Released)
                {
                    active = true;
                }
            }
            //if the mouse isnt in the box
            else if (!rectangle.Contains(worldMousePosition))
            {
                //click to deactivate it
                if (curClick == ButtonState.Pressed && oldClick == ButtonState.Released)
                {
                    active = false;
                }
            }
            //Typing affects the string builder if the box is active
            if (active)
            {
                text = KeyHandler.TypingOnKeyBoard(keystate, keyHeldState);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {
                spriteBatch.Draw(tex, pos, null, selectedColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(tex, pos, null, bgColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            }
            
            spriteBatch.DrawString(font, text, pos + new Vector2(5, size.Y * 0.1F), textColor);
        }
    }
}

