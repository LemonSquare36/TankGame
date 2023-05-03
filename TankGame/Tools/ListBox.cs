using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TankGame.Objects;

namespace TankGame.Tools
{
    internal class ListBox
    {
        Texture2D tex;
        SpriteFont font;

        Color bgColor, textColor, offSetColor = new Color(20,20,20);

        Vector2 pos, size, textSize, textPos;
        int numSelections;
        RectangleF rectangle;
        string[] selections;
        float scale;
        int borderThickness;
        //information for the buttons that make up the list
        List<Button> ButtonsList = new List<Button>();
        Vector2 buttonPos, buttonSize;

        //mouse stuff for clicking the textbox (selecting it)
        private MouseState mouse;
        public ButtonState oldClick;
        public ButtonState curClick;

        //used to keep the text in the box
        Rectangle cutOff;
        RasterizerState r = new RasterizerState();

        public ListBox(Vector2 Position, Vector2 Size, int NumSelections, Color backgroundColor, Color TextColor, int BorderThickness)
        {
            pos = Position;
            size = Size;
            rectangle = new RectangleF(pos, size);
            numSelections = NumSelections;
            bgColor = backgroundColor;
            textColor = TextColor;
            borderThickness = BorderThickness;
            cutOff = new Rectangle(Convert.ToInt16(rectangle.X), Convert.ToInt16(rectangle.Y), Convert.ToInt16(rectangle.Width), Convert.ToInt16(rectangle.Height));
        }
        public ListBox(RectangleF Rectangle, int NumSelections, Color backgroundColor, Color TextColor, int BorderThickness)
        {
            pos = Rectangle.Location;
            size = Rectangle.Size;
            rectangle = new RectangleF(pos, size);
            numSelections = NumSelections;
            bgColor = backgroundColor;
            textColor = TextColor;
            borderThickness = BorderThickness;
            cutOff = new Rectangle(Convert.ToInt16(rectangle.X), Convert.ToInt16(rectangle.Y), Convert.ToInt16(rectangle.Width), Convert.ToInt16(rectangle.Height));
        }
        public void Initialize()
        {
            
        }
        public void LoadContent(string[] Selections)
        {
            selections = Selections;
            buttonSize = size / numSelections;
            buttonPos = Vector2.Zero;
            //load the font
            font = Main.GameContent.Load<SpriteFont>("Fonts/DefualtFont");

            //create buttons for each selection
            for (int i = 0; i < selections.Length; i++)
            {
                Button tempButton = new Button(buttonPos, buttonSize.X, buttonSize.Y, "GameSprites/WhiteDot", "", "toggleOneTex");
                tempButton.ChangeButtonColor(bgColor);
                tempButton.ChangeOffSetColor(offSetColor);
                buttonPos.Y += buttonSize.Y;
                ButtonsList.Add(tempButton);
            }

            scale = buttonSize.Y / 50;
            r.ScissorTestEnable = true;
        }
        public void Update(MouseState Mouse, Vector2 WorldPos)
        {
            foreach (Button b in ButtonsList)
            {
                b.Update(Mouse, WorldPos);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //activate a new spritebatch.begin with the rasterizer cutoff
            spriteBatch.End();
            spriteBatch.GraphicsDevice.RasterizerState = r;
            spriteBatch.GraphicsDevice.ScissorRectangle = cutOff;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, r, null, Main.DefualtMatrix());
            //draw the buttons under the text
            foreach (Button b in ButtonsList)
            {
                b.Draw(spriteBatch);
            }
            //reset the text location
            pos = rectangle.Location;
            //draw the text
            for (int i = 0; i < selections.Length; i++)
            {
                //offset the text so it fits in the buttons
                pos += ButtonsList[i].Pos;
                spriteBatch.DrawString(font, selections[i], pos + new Vector2(5, size.Y * 0.1F), textColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }

            //reset how it draws back to normal
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = Main.gameWindow.ClientBounds;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.DefualtMatrix());

        }
        public void SetSelections(string[] Selections)
        {
            selections = Selections;
        }
    }
}
