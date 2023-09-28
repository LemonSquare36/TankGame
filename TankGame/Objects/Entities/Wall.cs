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
        Vector2 size = new Vector2(50, 50);
        public Wall(RectangleF CurrentSquare, Point GridLocation) : base(CurrentSquare, GridLocation)
        {
            curSquare = CurrentSquare;
            texFile = "GameSprites/Wall";
            type = "wall";
            size = curSquare.Size / size;

            HP = 4;
            curHP = 4;
            showHealth = false;
            SetHPBarPos();
            alive = true;
        }
        public override void Initialize(RectangleF newRectangle)
        {
            size = new Vector2(50, 50);
            curSquare = newRectangle;
            size = curSquare.Size / size;
            showHealth = false;
        }
        public override void LoadContent()
        {
            base.LoadContent();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            if (showHealth)
            {
                drawHPBar(spriteBatch);
            }
           
        }
    }
}
