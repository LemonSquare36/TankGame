using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TankGame.Objects;
using System.Text.Json.Serialization;

namespace TankGame.Tools
{
    internal class ListBox
    {
        Texture2D tex;
        SpriteFont font;
        RectangleF[] Borders;

        Color bgColor, textColor, borderColor;
        Vector3 offSetColor = new Vector3(20, 20, 20);
        public Vector3 OffSetColor
        {
            get { return offSetColor; }
            set { offSetColor = value; ChangeOffSet(); }

        }

        Vector2 pos, size;
        int numSelections, topButton = 0;
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

        //this event allows for outside objects to tell when an event is fired internally
        public event EventHandler GetEvent;

        /// <summary>
        /// Creates a listbox that holds buttons that return thier string. Takes an array of strings to populate
        /// </summary>
        /// <param name="Position">Top left corner position</param>
        /// <param name="Size">How big / bottom right corner in regards to the position</param>
        /// <param name="NumSelections">the number of selections fit into the hieght of the box</param>
        /// <param name="backgroundColor"></param>
        /// <param name="TextColor"></param>
        /// <param name="BorderColor"></param>
        /// <param name="BorderThickness">Outside border normal, inside is half of outside</param>
        public ListBox(Vector2 Position, Vector2 Size, int NumSelections, Color backgroundColor, Color TextColor, Color BorderColor, int BorderThickness)
        {
            pos = Position;
            size = Size;
            rectangle = new RectangleF(pos, size);
            numSelections = NumSelections;
            bgColor = backgroundColor;
            textColor = TextColor;
            borderThickness = BorderThickness;
            cutOff = new Rectangle(Convert.ToInt16((rectangle.X - borderThickness) * Camera.ResolutionScale.X) + Convert.ToInt16(Main.graphicsDevice.Viewport.X),
                Convert.ToInt16((rectangle.Y - borderThickness) * Camera.ResolutionScale.Y) + Convert.ToInt16(Main.graphicsDevice.Viewport.Y),
                Convert.ToInt16((rectangle.Width + borderThickness * 2) * Camera.ResolutionScale.X), Convert.ToInt16((rectangle.Height + borderThickness * 2) * Camera.ResolutionScale.Y));
            borderColor = BorderColor;
        }
        /// <summary>
        /// Creates a listbox that holds buttons that return thier string. Takes an array of strings to populate
        /// </summary>
        /// <param name="Rectangle">The size and location for the box</param>
        /// <param name="NumSelections">the number of selections fit into the hieght of the box</param>
        /// <param name="backgroundColor"></param>
        /// <param name="TextColor"></param>
        /// <param name="BorderColor"></param>
        /// <param name="BorderThickness">Outside border normal, inside is half of outside</param>
        public ListBox(RectangleF Rectangle, int NumSelections, Color backgroundColor, Color TextColor, Color BorderColor, int BorderThickness)
        {
            pos = Rectangle.Location;
            size = Rectangle.Size;
            rectangle = new RectangleF(pos, size);
            numSelections = NumSelections;
            bgColor = backgroundColor;
            textColor = TextColor;
            borderThickness = BorderThickness;
            cutOff = new Rectangle(Convert.ToInt16((rectangle.X - borderThickness) * Camera.ResolutionScale.X) + Convert.ToInt16(Main.graphicsDevice.Viewport.X),
                Convert.ToInt16((rectangle.Y - borderThickness) * Camera.ResolutionScale.Y) + Convert.ToInt16(Main.graphicsDevice.Viewport.Y),
                Convert.ToInt16((rectangle.Width + borderThickness * 2) * Camera.ResolutionScale.X), Convert.ToInt16((rectangle.Height + borderThickness * 2) * Camera.ResolutionScale.Y));
            borderColor = BorderColor;
        }
        public void Initialize()
        {

        }
        public void LoadContent(string[] Selections)
        {
            if (ButtonsList.Count > 0)
            {
                ButtonsList[topButton].rectangle.Y += ButtonsList[topButton].rectangle.Height;
                findTopButton();
            }
            //seeing if a button was removed
            bool subtraction = false;

            if (ButtonsList.Count > Selections.Length)
            {
                subtraction = true;
            }

            ButtonsList.Clear();
            selections = Selections;
            buttonSize = new Vector2(size.X, size.Y / numSelections);
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
            //font scale for size 12. 12 is good for 50 pixel scaling
            scale = buttonSize.Y / 50;
            try { r.ScissorTestEnable = true; }
            catch { }


            tex = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
            CreateBorder();

            //listen in on the window size change to recaluclate the cutoff rectangle for the rasterizer 
            Main.gameWindow.ClientSizeChanged += recalcRasterizer;

            if (topButton > 0)
            {
                if (subtraction)
                {
                    //lower the top button by one since we removed a button
                    topButton--;
                }
                for (int i = 0; i < ButtonsList.Count; i++)
                {
                    ButtonsList[i].rectangle.Y -= (ButtonsList[i].rectangle.Height) * topButton;
                }
            }
        }
        public void Update(MouseState Mouse, Vector2 WorldPos, KeyboardState curKeyState, KeyboardState oldKeyState)
        {
            buttonUpdate(Mouse, WorldPos);
            bool scrolldown = false;
            bool scrollup = false;
            if (numSelections < selections.Length)
            {
                if (ButtonsList[topButton].ButtonActive)
                {
                    scrollup = true;
                }
                else if (ButtonsList[topButton + numSelections - 1].ButtonActive)
                {
                    scrolldown = true;
                }
            }

            if (curKeyState.IsKeyDown(Keys.Up) && !oldKeyState.IsKeyDown(Keys.Up))
            {
                int j = ButtonsList.FindIndex(X => X.ButtonActive == true);
                if (j > 0)
                {
                    ButtonsList[j].ButtonReset();
                    j--;
                    ButtonsList[j].OneTexPressed = true;
                    curSelection = selections[j];
                    //only scroll if the box is overfull
                    if (selections.Length > numSelections && scrollup)
                    {

                        //make sure the top button isnt at the top. If it is dont scroll more
                        if (Convert.ToInt16(ButtonsList[0].rectangle.Location.Y) != Convert.ToInt16((rectangle.Y)))
                        {
                            //top button tracks which button in the list should currently be the top one
                            //used in draw and update logic for buttons
                            topButton--;
                            for (int i = 0; i < ButtonsList.Count; i++)
                            {
                                ButtonsList[i].rectangle.Y += ButtonsList[i].rectangle.Height;
                            }
                        }

                    }
                }
            }
            else if (curKeyState.IsKeyDown(Keys.Down) && !oldKeyState.IsKeyDown(Keys.Down))
            {
                int j = ButtonsList.FindIndex(X => X.ButtonActive == true);
                if (j < ButtonsList.Count - 1 && j != -1)
                {
                    ButtonsList[j].ButtonReset();
                    j++;
                    ButtonsList[j].OneTexPressed = true;
                    curSelection = selections[j];
                    //only scroll if the box is overfull
                    if (selections.Length > numSelections && scrolldown)
                    {
                        //make sure the last button isnt at the bottom. If it is dont scroll more
                        if (Convert.ToInt16(ButtonsList[ButtonsList.Count - 1].rectangle.Location.Y) != Convert.ToInt16((rectangle.Y + rectangle.Height) - buttonSize.Y))
                        {
                            //top button tracks which button in the list should currently be the top one
                            //used in draw and update logic for buttons
                            topButton++;
                            for (int i = 0; i < ButtonsList.Count; i++)
                            {
                                ButtonsList[i].rectangle.Y -= ButtonsList[i].rectangle.Height;
                            }
                        }

                    }
                }
            }
            //check for the mouse inside the box - before checking scroll values
            if (rectangle.Contains(WorldPos))
            {
                //only scroll if the box is overfull
                if (selections.Length > numSelections)
                {
                    //scroll down
                    if (Mouse.ScrollWheelValue < oldScrollValue)
                    {

                        //make sure the last button isnt at the bottom. If it is dont scroll more
                        if (Convert.ToInt16(ButtonsList[ButtonsList.Count - 1].rectangle.Location.Y) != Convert.ToInt16((rectangle.Y + rectangle.Height) - buttonSize.Y))
                        {
                            //top button tracks which button in the list should currently be the top one
                            //used in draw and update logic for buttons
                            topButton++;
                            for (int i = 0; i < ButtonsList.Count; i++)
                            {
                                ButtonsList[i].rectangle.Y -= ButtonsList[i].rectangle.Height;
                            }
                        }

                    }
                    //scroll up
                    else if (Mouse.ScrollWheelValue > oldScrollValue)
                    {
                        //make sure the top button isnt at the top. If it is dont scroll more
                        if (Convert.ToInt16(ButtonsList[0].rectangle.Location.Y) != Convert.ToInt16((rectangle.Y)))
                        {
                            //top button tracks which button in the list should currently be the top one
                            //used in draw and update logic for buttons
                            topButton--;
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
            buttonDraw(spriteBatch);
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
                spriteBatch.Draw(tex, new Vector2(Borders[0].Location.X - borderThickness, Borders[0].Location.Y + borderOffSet), null, borderColor, 0, Vector2.Zero, new Vector2(Borders[0].Size.X, Borders[0].Size.Y / 2), SpriteEffects.None, 0);
            }
            //reset how it draws back to normal
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = Main.gameWindow.ClientBounds;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.DefualtMatrix());

        }
        private void CreateBorder()
        {
            Borders = new RectangleF[4];
            //get the top border
            Borders[0] = new RectangleF(pos.X, pos.Y, buttonSize.X + borderThickness * 2, borderThickness);
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
            for (int i = 0; i < ButtonsList.Count; i++)
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
            if (GetEvent != null)
            {
                GetEvent?.Invoke(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Only call after LoadContent() - Changes the Offset amount of the bgcolor when a button is selected
        /// </summary>
        private void ChangeOffSet()
        {
            foreach (Button b in ButtonsList)
            {
                b.ChangeOffSetColor(offSetColor);
            }
        }
        private void recalcRasterizer(object sender, EventArgs e)
        {
            cutOff = new Rectangle(Convert.ToInt16((rectangle.X - borderThickness) * Camera.ResolutionScale.X) + Convert.ToInt16(Main.graphicsDevice.Viewport.X),
                Convert.ToInt16((rectangle.Y - borderThickness) * Camera.ResolutionScale.Y) + Convert.ToInt16(Main.graphicsDevice.Viewport.Y),
                Convert.ToInt16((rectangle.Width + borderThickness * 2) * Camera.ResolutionScale.X), Convert.ToInt16((rectangle.Height + borderThickness * 2) * Camera.ResolutionScale.Y));
        }
        private void buttonDraw(SpriteBatch spriteBatch)
        {
            //only draws the buttons that should be showing
            if (ButtonsList.Count > numSelections)
            {
                //top button tracks which button in the list should currently be the top one
                for (int i = topButton; i < numSelections + topButton; i++)
                {
                    try
                    {
                        ButtonsList[i].Draw(spriteBatch, true);
                    }
                    catch { }
                }
            }
            else
            {
                foreach (Button b in ButtonsList)
                {
                    b.Draw(spriteBatch, true);
                }
            }
        }
        public void UnselectButtons()
        {
            foreach (Button button in ButtonsList)
            {
                button.OneTexPressed = false;
            }
        }
        private void buttonUpdate(MouseState Mouse, Vector2 WorldPos)
        {
            //only updates the buttons currently showing
            if (ButtonsList.Count > numSelections)
            {
                //int difference = (ButtonsList.Count - numSelections);
                //top button tracks which button in the list should currently be the top one
                for (int i = topButton; i < numSelections + topButton; i++)
                {
                    try
                    {
                        ButtonsList[i].Update(Mouse, WorldPos);
                    }
                    catch { }
                }
            }
            else
            {
                foreach (Button b in ButtonsList)
                {
                    b.Update(Mouse, WorldPos);
                }
            }
        }
        private void findTopButton()
        {
            for (int i = 0; i < ButtonsList.Count; i++)
            {
                if (ButtonsList[i].rectangle.Location == this.rectangle.Location)
                {
                    topButton = i;
                }
            }
        }
    }
}
