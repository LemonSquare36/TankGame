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
    internal class Wall : Entity
    {
        public Wall(RectangleF CurrentSquare) : base(CurrentSquare)
        {
            curSquare = CurrentSquare;
            texFile = "GameSprites/WhiteDot";          
        }
        public override void LoadContent()
        {
            tex = Main.GameContent.Load<Texture2D>(texFile);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, curSquare.Location, null, Color.Black, 0, Vector2.Zero, curSquare.Size, SpriteEffects.None, 0);
        }
    }
}
