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
using System.Diagnostics.Tracing;

namespace TankGame.Tools
{
    internal class InputBox
    {
        Texture2D tex;
        SpriteFont font;
        Color bgColor, textColor, selectedColor;
        Vector2 pos, size, textSize;
        RectangleF rectangle;
        //mouse stuff for clicking the textbox (selecting it)
        private MouseState mouse;
        public ButtonState oldClick;
        public ButtonState curClick;
        bool active = false;
        //used to keep the text in the box
        Rectangle cutOff;
        RasterizerState r = new RasterizerState();
        float scale;
        int charLimit = -1;


        KeyStrokeHandler KeyHandler = new KeyStrokeHandler();
        private string text;
        public string Text
        {
            get { return text; }
            set { text = value; KeyHandler.CurText.Clear(); KeyHandler.CurText.Append(value); }
        }

        public InputBox(Color backgroundColor, Color TextColor, Vector2 Pos, Vector2 Size)
        {
            bgColor = backgroundColor;
            selectedColor = new Color(backgroundColor.R + 50, backgroundColor.G + 50, backgroundColor.B + 50);
            textColor = TextColor;
            pos = Pos;
            size = Size;
            rectangle = new RectangleF(pos, size);
            cutOff = new Rectangle(new Point(Convert.ToInt16(pos.X * Camera.ResolutionScale.X) + Convert.ToInt16(Main.graphicsDevice.Viewport.X),
               Convert.ToInt16(pos.Y * Camera.ResolutionScale.Y) + Convert.ToInt16(Main.graphicsDevice.Viewport.Y)),
               new Point(Convert.ToInt16(size.X * Camera.ResolutionScale.X), Convert.ToInt16(size.Y * Camera.ResolutionScale.Y)));
            text = "";
            //scale the text if needed
            scale = Size.Y / 50;            
        }
        public InputBox(Color backgroundColor, Color TextColor, Vector2 Pos, Vector2 Size, int characterLimit)
        {
            bgColor = backgroundColor;
            selectedColor = new Color(backgroundColor.R + 50, backgroundColor.G + 50, backgroundColor.B + 50);
            textColor = TextColor;
            pos = Pos;
            size = Size;
            rectangle = new RectangleF(pos, size);
            cutOff = new Rectangle(new Point(Convert.ToInt16(pos.X * Camera.ResolutionScale.X) + Convert.ToInt16(Main.graphicsDevice.Viewport.X),
               Convert.ToInt16(pos.Y * Camera.ResolutionScale.Y) + Convert.ToInt16(Main.graphicsDevice.Viewport.Y)),
               new Point(Convert.ToInt16(size.X * Camera.ResolutionScale.X), Convert.ToInt16(size.Y * Camera.ResolutionScale.Y)));
            text = "";
            //scale the text if needed
            scale = Size.Y / 50;
            charLimit = characterLimit;
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
            try
            {
                KeyStrokeHandler.populateInformation();
            }
            catch { }
            try
            {
                r.ScissorTestEnable = true;
            }
            catch { }

            //listen in on the window size change to recaluclate the cutoff rectangle for the rasterizer 
            Main.gameWindow.ClientSizeChanged += recalcRasterizer;
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
                text = KeyHandler.TypingOnKeyBoard(keystate, keyHeldState, charLimit);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //activate a new spritebatch.begin with the rasterizer cutoff
            spriteBatch.End();
            spriteBatch.GraphicsDevice.RasterizerState = r;
            spriteBatch.GraphicsDevice.ScissorRectangle = cutOff;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, r, null, Main.DefualtMatrix());

            //draw the box the text is typed in - active color if active
            if (active)
            {
                spriteBatch.Draw(tex, pos, null, selectedColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(tex, pos, null, bgColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            }
            textSize = font.MeasureString(text) * scale;
            if (textSize.X > size.X)
            {
                textSize.X -= size.X-20;
            }
            else
            {
                textSize.X = 0;
            }
            //draw the text
            spriteBatch.DrawString(font, text, pos + new Vector2(5 - textSize.X, size.Y * 0.1F), textColor, 0, Vector2.Zero , scale, SpriteEffects.None, 0);       
            
            //reset how it draws back to normal
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = Main.gameWindow.ClientBounds;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.DefualtMatrix());
        }
        private void recalcRasterizer(object sender, EventArgs e)
        {
            cutOff = new Rectangle(new Point(Convert.ToInt16(pos.X * Camera.ResolutionScale.X) + Convert.ToInt16(Main.graphicsDevice.Viewport.X),
                Convert.ToInt16(pos.Y * Camera.ResolutionScale.Y) + Convert.ToInt16(Main.graphicsDevice.Viewport.Y)),
                new Point(Convert.ToInt16(size.X * Camera.ResolutionScale.X), Convert.ToInt16(size.Y * Camera.ResolutionScale.Y)));
        }
    }
}

