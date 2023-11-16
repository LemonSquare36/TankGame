using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TankGame.Objects;
using System.Linq.Expressions;
using TankGame.Objects.Entities;
using System.Threading;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using TankGame.GameInfo;

namespace TankGame.Tools
{
    internal class Selector
    {
        public string text = "";
        public int max, min;

        Button leftButton, rightButton;
        Vector2 position, size;
        RectangleF rectangle;
        Rectangle cutOff;
        RasterizerState r = new RasterizerState();
        int value;
        string type, buttonType;
        List<string> selection = new List<string>();
        Color textColor, bgColor;
        Texture2D bgTexture;
        float scale;

        private event EventHandler valueChanged;
        public event EventHandler ValueChanged
        {
            add { valueChanged += value; }
            remove { valueChanged -= value; }
        }

        /// <summary>
        /// Creates a field with a button on either side. Used to select stuff with side buttons
        /// </summary>
        /// <param name="ButtonTypes">"Arrows" for arrows || "PlusMinus" for -, +</param>
        /// <param name="Max">Max number allowed</param>
        /// <param name="Min">min number allowed</param>
        public Selector(Vector2 Position, Vector2 RectangleSize, string ButtonTypes, int Max, int Min, Color TextColor)
        {
            //the type tells furture functions how to do various tasks. 
            type = "Increment";
            //the minimum number and max number for the field
            max = Max;
            min = Min;
            value = 0;
            if (value < min)
                value = min;
            else if (value > max)
                value = max;

            //get rectangle information for the field that stores the information
            position = Position;
            size = RectangleSize;
            rectangle = new RectangleF(position, size);

            //create the rasterizer states cutoff rectangle adjusted for camera resultion
            cutOff = new Rectangle(Convert.ToInt16(rectangle.X * Camera.ResolutionScale.X) + Convert.ToInt16(Main.graphicsDevice.Viewport.X),
                Convert.ToInt16(rectangle.Y * Camera.ResolutionScale.Y) + Convert.ToInt16(Main.graphicsDevice.Viewport.Y),
                Convert.ToInt16(rectangle.Width * Camera.ResolutionScale.X), Convert.ToInt16(rectangle.Height * Camera.ResolutionScale.Y));
            //create buttons
            buttonType = ButtonTypes;

            textColor = TextColor;
        }
        /// <summary>
        /// Creates a field with a button on either side. Used to select stuff with side buttons
        /// </summary>
        /// <param name="ButtonTypes">"Arrows" for arrows || "PlusMinus" for -, +</param>
        /// <param name="Selection">List of strings used as the selector values to cycle through</param>
        public Selector(Vector2 Position, Vector2 RectangleSize, string ButtonTypes, List<string> Selection, Color BGColor, Color TextColor)
        {
            //the type tells furture functions how to do various tasks. 
            type = "Selector";
            //the strings to choose from in the field
            selection = Selection;

            //get rectangle information for the field that stores the information
            position = Position;
            size = RectangleSize;
            rectangle = new RectangleF(position, size);

            //create the rasterizer states cutoff rectangle adjusted for camera resultion
            cutOff = new Rectangle(Convert.ToInt16(rectangle.X * Camera.ResolutionScale.X) + Convert.ToInt16(Main.graphicsDevice.Viewport.X),
                Convert.ToInt16(rectangle.Y * Camera.ResolutionScale.Y) + Convert.ToInt16(Main.graphicsDevice.Viewport.Y),
                Convert.ToInt16(rectangle.Width * Camera.ResolutionScale.X), Convert.ToInt16(rectangle.Height * Camera.ResolutionScale.Y));
            //create buttons
            buttonType = ButtonTypes;

            textColor = TextColor;
            bgColor = BGColor;
        }
        /// <summary>
        /// creates the buttons with the appropriate textures
        /// </summary>
        /// <param name="ButtonType">"Arrows" or "PlusMinus" for the two button types<</param>
        private void CreateButtons(string ButtonType)
        {
            float buttonHeight = ((15 * scale) / 2) - (scale + (scale/2));

            switch (ButtonType)
            {
                case "Arrows":
                    //create two arrow buttons on the sides of the information field
                    leftButton = new Button(position + new Vector2(-((30 * scale) + 5), buttonHeight), 50, 50, 30 * scale, 30 * scale, "Buttons/Editor/ArrowLeft", "leftbutton");
                    rightButton = new Button(position + new Vector2((size.X) + 5, buttonHeight), 50, 50, 30 * scale, 30 * scale, "Buttons/Editor/ArrowRight", "rightbutton");

                    leftButton.ButtonClicked += ButtonPressed;
                    rightButton.ButtonClicked += ButtonPressed;
                    break;
                case "PlusMinus":
                    //create a minus and plus buttons on the sides of the information field
                    leftButton = new Button(position + new Vector2(-((30 * scale) + 5), buttonHeight), 50, 50, 30 * scale, 30 * scale, "Buttons/Editor/Minus", "leftbutton");
                    rightButton = new Button(position + new Vector2((size.X) + 5, buttonHeight), 50, 50, 30 * scale, 30 * scale, "Buttons/Editor/Plus", "rightbutton");

                    leftButton.ButtonClicked += ButtonPressed;
                    rightButton.ButtonClicked += ButtonPressed;
                    break;

            }
        }
        public void LoadContent()
        {
            //make sure the text scales with the size given
            scale = size.Y / 60;

            //loads the buttons data
            CreateButtons(buttonType);

            //load bg texture for selector type
            if (type == "Selector")
            {
                bgTexture = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
            }
            //redo the rasterizer calculations when the window changes size
            Main.gameWindow.ClientSizeChanged += recalcRasterizer;

            //unfortunatly I dont know if I need this so for now it will stay until further tested
            try { r.ScissorTestEnable = true; }
            catch { }
        }
        public void Update(MouseState Mouse, Vector2 WorldPos)
        {
            leftButton.Update(Mouse, WorldPos);
            rightButton.Update(Mouse, WorldPos);
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            leftButton.Draw(spriteBatch);
            rightButton.Draw(spriteBatch);

            //activate a new spritebatch.begin with the rasterizer cutoff
            spriteBatch.End();
            spriteBatch.GraphicsDevice.RasterizerState = r;
            spriteBatch.GraphicsDevice.ScissorRectangle = cutOff;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, r, null, Main.DefualtMatrix());
            #region inside rasterizer draw

            Vector2 length = font.MeasureString(text);
            switch (type)
            {
                case "Increment":
                    spriteBatch.DrawString(font, text, new Vector2((position.X + (size.X / 2)) - ((length.X * scale) / 2.5F), position.Y), textColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    break;
                case "Selector":
                    spriteBatch.Draw(bgTexture, position, null, bgColor, 0, Vector2.Zero, size, SpriteEffects.None, 0);
                    spriteBatch.DrawString(font, text, new Vector2((position.X + (size.X / 2)) - (length.X * scale / 2), position.Y), textColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    break;
            }

            #endregion
            //reset how it draws back to normal
            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = Main.gameWindow.ClientBounds;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.DefualtMatrix());
        }
        private void ButtonPressed(Object sender, EventArgs e)
        {
            switch (type)
            {
                //add or subtract one per button press but do not exceed limits then assign the number value to the text string
                case "Increment":
                    if (((Button)sender).bName == "leftbutton")
                    {
                        value--;
                        if (value < min)
                            value = min;
                        text = Convert.ToString(value);
                    }
                    else if (((Button)sender).bName == "rightbutton")
                    {
                        value++;
                        if (value > max)
                            value = max;
                        text = Convert.ToString(value);
                    }
                    break;
                //cycle through the selection list, going to either side when limit reached (instead of just walling)
                //then get the text from the selection list using the value (i)
                case "Selector":
                    if (((Button)sender).bName == "leftbutton")
                    {
                        value--;
                        if (value < 0)
                            value = selection.Count - 1;
                        text = selection[value];
                    }
                    else if (((Button)sender).bName == "rightbutton")
                    {
                        value++;
                        if (value >= selection.Count)
                            value = 0;
                        text = selection[value];
                    }
                    break;
            }
            OnValueChanged();
        }

        private void recalcRasterizer(object sender, EventArgs e)
        {
            cutOff = new Rectangle(Convert.ToInt16(rectangle.X * Camera.ResolutionScale.X) + Convert.ToInt16(Main.graphicsDevice.Viewport.X),
                Convert.ToInt16(rectangle.Y * Camera.ResolutionScale.Y) + Convert.ToInt16(Main.graphicsDevice.Viewport.Y),
                Convert.ToInt16(rectangle.Width * Camera.ResolutionScale.X), Convert.ToInt16(rectangle.Height * Camera.ResolutionScale.Y));
        }

        private void OnValueChanged()
        {
            valueChanged?.Invoke(this, EventArgs.Empty);
        }
        public int Value
        {
            get { return this.value; }
            set
            {   //make sure the text updates with external value changes
                switch (type)
                {
                    case "Increment":
                        if (value < min)
                            this.value = min;
                        else if (value > max)
                            this.value = max;
                        else
                            this.value = value;

                        text = Convert.ToString(this.value);
                        break;
                    case "Selector":
                        if (value < 0)
                            this.value = selection.Count - 1;
                        else if (value >= selection.Count)
                            this.value = 0;
                        else
                            this.value = value;
                        text = selection[this.value];
                        break;
                }
                OnValueChanged();
            }
        }
    }
}
