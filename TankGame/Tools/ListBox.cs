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
        RectangleF[] Borders;

        Color bgColor, textColor, offSetColor = new Color(20,20,20), borderColor;

        Vector2 pos, size;
        int numSelections;
        RectangleF rectangle;
        string[] selections;
        public string curSelection = "";
        float scale;
        int borderThickness;
        //information for the buttons that make up the list
        List<Button> ButtonsList = new List<Button>();
        Vector2 buttonPos, buttonSize;

        //used to keep the text in the box
        Rectangle cutOff;
        RasterizerState r = new RasterizerState();

        public ListBox(Vector2 Position, Vector2 Size, int NumSelections, Color backgroundColor, Color TextColor, Color BorderColor, int BorderThickness)
        {
            pos = Position;
            size = Size;
            rectangle = new RectangleF(pos, size);
            numSelections = NumSelections;
            bgColor = backgroundColor;
            textColor = TextColor;
            borderThickness = BorderThickness;
            cutOff = new Rectangle(Convert.ToInt16(rectangle.X), Convert.ToInt16(rectangle.Y), Convert.ToInt16(rectangle.Width), Convert.ToInt16(rectangle.Height));
            borderColor = BorderColor;
        }
        public ListBox(RectangleF Rectangle, int NumSelections, Color backgroundColor, Color TextColor, Color BorderColor, int BorderThickness)
        {
            pos = Rectangle.Location;
            size = Rectangle.Size;
            rectangle = new RectangleF(pos, size);
            numSelections = NumSelections;
            bgColor = backgroundColor;
            textColor = TextColor;
            borderThickness = BorderThickness;
            cutOff = new Rectangle(Convert.ToInt16(rectangle.X - BorderThickness), Convert.ToInt16(rectangle.Y - BorderThickness), 
                Convert.ToInt16(rectangle.Width + BorderThickness), Convert.ToInt16(rectangle.Height + BorderThickness));
            borderColor = BorderColor;
        }
        public void Initialize()
        {
            
        }
        public void LoadContent(string[] Selections)
        {
            ButtonsList.Clear();
            selections = Selections;
            buttonSize = new Vector2(size.X ,size.Y / numSelections);            
            buttonPos = pos;
            //load the font
            font = Main.GameContent.Load<SpriteFont>("Fonts/DefualtFont");

            //create buttons for each selection
            for (int i = 0; i < selections.Length; i++)
            {
                Button tempButton = new Button(buttonPos, buttonSize.X, buttonSize.Y, "GameSprites/WhiteDot", "", "toggleOneTex");
                tempButton.ChangeButtonColor(bgColor);
                tempButton.ChangeOffSetColor(offSetColor);
                tempButton.ButtonClicked += ButtonPressed;
                buttonPos.Y += buttonSize.Y;
                ButtonsList.Add(tempButton);
            }

            scale = buttonSize.Y / 50;
            try { r.ScissorTestEnable = true; }
            catch { }
           

            tex = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
            CreateBorder();
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
            //draw the full box
            spriteBatch.Draw(tex, pos, null, bgColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            //draw the buttons under the text
            foreach (Button b in ButtonsList)
            {
                b.Draw(spriteBatch, true);
            }
            //reset the text location
            Vector2 textpos = rectangle.Location;
            //draw the text
            for (int i = 0; i < selections.Length; i++)
            {
                //offset the text so it fits in the buttons                
                spriteBatch.DrawString(font, selections[i], textpos + new Vector2(5, buttonSize.Y * 0.1F), textColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                textpos.Y += buttonSize.Y;
            }
            //draw the borders
            float borderOffSet = 0;
            for (int i = 0; i < numSelections; i++)
            {
                
                if (i == 0)
                {
                    spriteBatch.Draw(tex, Borders[0].Location, null, borderColor, 0, Vector2.Zero, Borders[0].Size, SpriteEffects.None, 0);
                    spriteBatch.Draw(tex, Borders[1].Location, null, borderColor, 0, Vector2.Zero, Borders[1].Size, SpriteEffects.None, 0);
                    spriteBatch.Draw(tex, Borders[2].Location - new Vector2(borderThickness, 0), null, borderColor, 0, Vector2.Zero, Borders[2].Size, SpriteEffects.None, 0);
                    spriteBatch.Draw(tex, Borders[3].Location - new Vector2(0, borderThickness), null, borderColor, 0, Vector2.Zero, Borders[3].Size, SpriteEffects.None, 0);
                }
                    if (i < ButtonsList.Count)
                    {
                        borderOffSet += buttonSize.Y;
                    }
                    spriteBatch.Draw(tex, new Vector2(Borders[0].Location.X, Borders[0].Location.Y + borderOffSet), null, borderColor, 0, Vector2.Zero, new Vector2(Borders[0].Size.X, Borders[0].Size.Y/2), SpriteEffects.None, 0);              
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
        private void CreateBorder()
        {
            Borders = new RectangleF[4];
            //get the top border
            Borders[0] = new RectangleF(pos.X, pos.Y, buttonSize.X, borderThickness);
            //get the left border
            Borders[1] = new RectangleF(pos.X, pos.Y, borderThickness, size.Y);
            //get the right border             
            Borders[2] = new RectangleF(pos.X + buttonSize.X, pos.Y, borderThickness, size.Y);
            //get the bottom border
            Borders[3] = new RectangleF(pos.X, pos.Y + size.Y, buttonSize.X, borderThickness);
        }
        private void ButtonPressed(object sender, EventArgs e)
        {
            //gets the current string selected (from the button)
            //as well as unselects all other buttons
            for (int i = 0; i < ButtonsList.Count;i++)
            {
                if (ButtonsList[i] == sender as Button)
                {
                    if (ButtonsList[i].Texture == ButtonsList[i].UnPressed)
                    {
                        //empty string if no buttons are active
                        curSelection = "";
                    }
                    else { curSelection = selections[i]; }
                }
                else
                {
                    if (ButtonsList[i].OneTexPressed)
                    {
                        ButtonsList[i].toggleTexture();
                    }                    
                }
            }
        }
    }
}
