using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TankGame.Objects;
using Microsoft.Xna.Framework.Audio;
using TankGame.Tools;
using System.Linq.Expressions;
using TankGame.GameInfo;

namespace TankGame.Tools
{
    internal class ScrollBox
    {
        public List<List<Button>> buttons = new();
        public List<List<Keybind>> keybinds = new();
        public List<List<Texture2D>> textures = new();
        public List<List<Vector2>> texturePositions = new();
        RectangleF innerBox, fullBox, scrollBar, scroller;
        public float scrollSpeed;
        public bool scrollingDown, scrollingUP, updateItems = true;

        bool scrollerClicked = false;
        float scale;
        RasterizerState r = new RasterizerState();
        Rectangle cutOff;
        MouseState oldMouse;
        Vector2 oldWorldPos;
        Texture2D scrollerTex;

        public ScrollBox(RectangleF displayRectangle, RectangleF fullSize, int scrollerWidth)
        {
            innerBox = displayRectangle;
            fullBox = fullSize;

            scrollSpeed = 15;

            cutOff = new Rectangle(Convert.ToInt16(innerBox.X * Camera.ResolutionScale.X) + Convert.ToInt16(Main.graphicsDevice.Viewport.X),
                Convert.ToInt16(innerBox.Y * Camera.ResolutionScale.Y) + Convert.ToInt16(Main.graphicsDevice.Viewport.Y),
                Convert.ToInt16(innerBox.Width * Camera.ResolutionScale.X), Convert.ToInt16(innerBox.Height * Camera.ResolutionScale.Y));
            Main.gameWindow.ClientSizeChanged += recalcRasterizer;

            scrollerTex = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
            //create the scroller based on the size of the 2 boxes
            scale = innerBox.Height / fullBox.Height;
            float scrollerHeight = scale * innerBox.Height;
            //get the position of the scoller/scroll bar
            Vector2 position = innerBox.Location + new Vector2(innerBox.Width - scrollerWidth, 5);
            scroller = new RectangleF(position, new Vector2(scrollerWidth, scrollerHeight));
            //make the bar for the scroller to be in
            scrollBar = new RectangleF(position + new Vector2(-2, -2), new Vector2(scrollerWidth + 2, innerBox.Height - 10));

            r.ScissorTestEnable = true;
        }
        public void LoadContent()
        {

        }
        public void Update(MouseState mouse, Vector2 worldPos, KeyboardState keyState, KeyboardState oldKeyState)
        {
            bool scrollerUsed = false;
            //amount moved is used when the scroller gets used
            float AmountMoved = 0;
            //check if the scroller is being used
            if (scroller.Contains(worldPos) || scrollerClicked)
            {
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    scrollerUsed = true;
                    scrollerClicked = true;

                    //scale should be less than 0
                    AmountMoved = -(oldWorldPos.Y - worldPos.Y) / scale;
                    if (AmountMoved > 0)
                    {
                        //if the amount moved is positive (down) check to see if it would go past the full box floor
                        if (fullBox.Y + fullBox.Height - AmountMoved < innerBox.Y + innerBox.Height)
                        {
                            AmountMoved = (fullBox.Y + fullBox.Height) - (innerBox.Y + innerBox.Height);
                            fullBox.Y -= AmountMoved;
                            scroller.Y += AmountMoved * scale;
                            //scrollerUsed = false;
                        }
                        else
                        {
                            fullBox.Y -= AmountMoved;
                            scroller.Y -= oldWorldPos.Y - worldPos.Y;
                        }
                    }
                    else if (AmountMoved < 0)
                    {
                        //if the amount moved is negative (down) check to see if it would go past the full box cieling
                        if (fullBox.Y - AmountMoved > innerBox.Y)
                        {
                            AmountMoved = (fullBox.Y) - (innerBox.Y);
                            fullBox.Y -= AmountMoved;
                            scroller.Y += AmountMoved * scale;
                        }
                        else
                        {
                            fullBox.Y -= AmountMoved;
                            scroller.Y -= oldWorldPos.Y - worldPos.Y;
                        }
                    }
                }
                else
                {
                    scrollerClicked = false;
                }
            }
            //if the scroller isnt being used look for mouse wheel
            if (!scrollerUsed && !scrollerClicked)
            {
                if (innerBox.Contains(worldPos))
                {
                    if (mouse.ScrollWheelValue > oldMouse.ScrollWheelValue)
                    {
                        scrollingUP = true;
                    }
                    else if (mouse.ScrollWheelValue < oldMouse.ScrollWheelValue)
                    {
                        scrollingDown = true;
                    }
                }
                if (scrollingDown)
                {
                    if (fullBox.Y + fullBox.Height <= innerBox.Y + innerBox.Height)
                    {
                        scrollingDown = false;
                    }
                    else
                    {
                        fullBox.Y -= scrollSpeed;
                        scroller.Y += scrollSpeed * scale;
                    }
                }
                if (scrollingUP)
                {
                    if (fullBox.Y >= innerBox.Y)
                    {
                        scrollingUP = false;
                    }
                    else
                    {
                        fullBox.Y += scrollSpeed;
                        scroller.Y -= scrollSpeed * scale;
                    }
                }
            }

            //buttons
            foreach (List<Button> buttonList in buttons)
            {
                foreach (Button button in buttonList)
                {
                    if (scrollerUsed)
                    {
                        button.rectangle.Y -= AmountMoved;
                    }
                    else if (scrollingDown)
                    {
                        button.rectangle.Y -= scrollSpeed;
                    }
                    else if (scrollingUP)
                    {
                        button.rectangle.Y += scrollSpeed;
                    }
                    if (updateItems)
                        button.Update(mouse, worldPos);
                }
            }
            //keybinds
            foreach (List<Keybind> keybindLists in keybinds)
            {
                foreach (Keybind keybind in keybindLists)
                {
                    if (scrollerUsed)
                    {
                        keybind.rectangle.Y -= AmountMoved;
                    }
                    else if (scrollingDown)
                    {
                        keybind.rectangle.Y -= scrollSpeed;
                    }
                    else if (scrollingUP)
                    {
                        keybind.rectangle.Y += scrollSpeed;
                    }
                    if (updateItems)
                        keybind.Update(mouse, worldPos, keyState, oldKeyState);
                }
            }
            //textures
            for (int i = 0; i < textures.Count; i++)
            {
                for (int j = 0; j < textures[i].Count; j++)
                {
                    if (scrollerUsed)
                    {
                        texturePositions[i][j] -= new Vector2(0, AmountMoved);
                    }
                    else if (scrollingDown)
                    {
                        texturePositions[i][j] -= new Vector2(0, scrollSpeed);
                    }
                    else if (scrollingUP)
                    {
                        texturePositions[i][j] += new Vector2(0, scrollSpeed);
                    }
                }
            }
            //set values for next loop
            scrollingUP = false;
            scrollingDown = false;
            oldMouse = mouse;
            oldWorldPos = worldPos;
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.End();
            spriteBatch.GraphicsDevice.RasterizerState = r;
            spriteBatch.GraphicsDevice.ScissorRectangle = cutOff;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, r, null, Main.DefualtMatrix());


            //buttons
            foreach (List<Button> buttonList in buttons)
            {
                foreach (Button button in buttonList)
                {
                    button.Draw(spriteBatch);
                }
            }
            //keybinds
            foreach (List<Keybind> keybindLists in keybinds)
            {
                foreach (Keybind keybind in keybindLists)
                {
                    keybind.Draw(spriteBatch, font);
                }
            }
            //textures
            for (int i = 0; i < textures.Count; i++)
            {
                for (int j = 0; j < textures[i].Count; j++)
                {
                    spriteBatch.Draw(textures[i][j], texturePositions[i][j], Color.White);
                }
            }

            //draw the scroller and scroll bar
            spriteBatch.Draw(scrollerTex, scrollBar.Location, null, new Color(Color.DarkGray, 110), 0, Vector2.Zero, scrollBar.Size, SpriteEffects.None, 0);
            spriteBatch.Draw(scrollerTex, scroller.Location, null, new Color(Color.Black, 255), 0, Vector2.Zero, scroller.Size, SpriteEffects.None, 0);


            spriteBatch.End();
            spriteBatch.GraphicsDevice.ScissorRectangle = Main.gameWindow.ClientBounds;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, Main.DefualtMatrix());
        }
        public void AddList(List<Button> ButtonList)
        {
            buttons.Add(ButtonList);
        }
        public void AddList(List<Keybind> KeybindList)
        {
            keybinds.Add(KeybindList);
        }
        public void AddList(List<Texture2D> textureList, List<Vector2> positions)
        {
            textures.Add(textureList);
            texturePositions.Add(positions);
        }
        private void recalcRasterizer(object sender, EventArgs e)
        {
            cutOff = new Rectangle(Convert.ToInt16(innerBox.X * Camera.ResolutionScale.X) + Convert.ToInt16(Main.graphicsDevice.Viewport.X),
                Convert.ToInt16(innerBox.Y * Camera.ResolutionScale.Y) + Convert.ToInt16(Main.graphicsDevice.Viewport.Y),
                Convert.ToInt16(innerBox.Width * Camera.ResolutionScale.X), Convert.ToInt16(innerBox.Width * Camera.ResolutionScale.Y));
        }
    }
}
