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
        int oldScrollValue;
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
            cutOff = new Rectangle(Convert.ToInt16((rectangle.X - BorderThickness) * Camera.ResolutionScale.X), Convert.ToInt16((rectangle.Y - BorderThickness) * Camera.ResolutionScale.Y),
                Convert.ToInt16((rectangle.Width + BorderThickness*2) * Camera.ResolutionScale.X), Convert.ToInt16((rectangle.Height + BorderThickness*2) * Camera.ResolutionScale.Y));
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
            cutOff = new Rectangle(Convert.ToInt16((rectangle.X)* Camera.ResolutionScale.X), Convert.ToInt16((rectangle.Y) * Camera.ResolutionScale.Y), 
                Convert.ToInt16((rectangle.Width)* Camera.ResolutionScale.X), Convert.ToInt16((rectangle.Height) * Camera.ResolutionScale.Y));
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
            //check for the mouse inside the box - before checking scroll values
            if (rectangle.Contains(WorldPos))
            {
                //only scroll if the box is overfull
                if (selections.Length > numSelections)
                {
                    //scroll up
                    if (Mouse.ScrollWheelValue < oldScrollValue)
                    {

                        //make sure the last button isnt at the bottom. If it is dont scroll more
                        if (ButtonsList[ButtonsList.Count - 1].rectangle.Location.Y != (rectangle.Y + rectangle.Height) - buttonSize.Y)
                        {
                            for (int i = 0; i < ButtonsList.Count; i++)
                            {
                                ButtonsList[i].rectangle.Y -= ButtonsList[i].rectangle.Height;
                            }
                        }

                    }
                    //scroll down
                    else if (Mouse.ScrollWheelValue > oldScrollValue)
                    {
                        //make sure the top button isnt at the top. If it is dont scroll more
                        if (ButtonsList[0].rectangle.Location.Y != rectangle.Y)
                        {
                            for (int i = 0; i < ButtonsList.Count; i++)
                            {
                                ButtonsList[i].rectangle.Y += ButtonsList[i].rectangle.Height;
                            }
                        }
                    }
                }
            }
            //get the old value for comparisons
            oldScrollValue = Mouse.ScrollWheelValue;
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
            //draw the text
            for (int i = 0; i < selections.Length; i++)
            {
                //offset the text so it fits in the buttons                
                spriteBatch.DrawString(font, selections[i], ButtonsList[i].rectangle.Location + new Vector2(5, buttonSize.Y * 0.1F), textColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
            //draw the borders
            float borderOffSet = 0;
            for (int i = 0; i < numSelections; i++)
            {
                
                if (i == 0)
                {
                    spriteBatch.Draw(tex, Borders[0].Location - new Vector2(borderThickness, borderThickness), null, borderColor, 0, Vector2.Zero, Borders[0].Size, SpriteEffects.None, 0);
                    spriteBatch.Draw(tex, Borders[1].Location - new Vector2(borderThickness, 0), null, borderColor, 0, Vector2.Zero, Borders[1].Size, SpriteEffects.None, 0);
                    spriteBatch.Draw(tex, Borders[2].Location, null, borderColor, 0, Vector2.Zero, Borders[2].Size, SpriteEffects.None, 0);
                    spriteBatch.Draw(tex, Borders[3].Location - new Vector2(borderThickness, 0), null, borderColor, 0, Vector2.Zero, Borders[3].Size, SpriteEffects.None, 0);
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
            Borders[0] = new RectangleF(pos.X, pos.Y, buttonSize.X + borderThickness*2, borderThickness);
            //get the left border
            Borders[1] = new RectangleF(pos.X, pos.Y, borderThickness, size.Y);
            //get the right border             
            Borders[2] = new RectangleF(pos.X + buttonSize.X, pos.Y, borderThickness, size.Y);
            //get the bottom border
            Borders[3] = new RectangleF(pos.X, pos.Y + size.Y, buttonSize.X + borderThickness * 2, borderThickness);
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
