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

namespace TankGame.Objects.Entities
{
    internal class Tank
    {
        int hp;
        public Rectangle curSquare { get; set; }

        Texture2D tex;

        public Tank(Rectangle startingSquare)
        {
            hp = 3;
            curSquare = startingSquare;
            //curSquare.
        }
        public void LoadContent()
        {
            tex = Main.GameContent.Load<Texture2D>("GameSprites/test");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, curSquare, Color.White);
        }
    }
}
