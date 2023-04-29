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
using Microsoft.VisualBasic.Devices;
using System.Drawing;

namespace TankGame.Tools
{
    internal class InputBox
    {
        Texture2D tex;
        Color color;
        new Vector2 pos, size;
        string curText, titleText;
        //mouse stuff for clicking the textbox (selecting it)
        private MouseState mouse;
        public ButtonState oldClick;
        public ButtonState curClick;

        public InputBox(Color color, Vector2 Pos, Vector2 Size)
        {
            this.color = color;
            this.pos = Pos;
            this.size = Size;
        }
        public void Initialize()
        {
            
        }
        public void LoadContent()
        {
            tex = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
        }
        public void Update(MouseState Mouse, Vector2 worldMousePosition)
        {
            mouse = Mouse;
            oldClick = curClick;
            curClick = mouse.LeftButton;
            if (rectangle.Contains(worldMousePosition))
            {
                //Edge Detection
                if (curClick == ButtonState.Pressed && oldClick == ButtonState.Released)
                {
                    
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, pos, null, color, 0, Vector2.Zero, size, SpriteEffects.None, 0);
        }
    }
}
