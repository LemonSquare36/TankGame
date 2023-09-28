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
    internal class Tank : Entity
    {
        Vector2 size = new Vector2(50, 50);               
        public int range;        

        public Tank(RectangleF CurrentSquare, Point GridLocation) : base(CurrentSquare, GridLocation)
        {
            curSquare = CurrentSquare;
            texFile = "GameSprites/BattleSprites/TankUH";
            type = "tank";
            size = curSquare.Size / size;
            range = 4;

            HP = 4;
            curHP = 4;
            SetHPBarPos();
            alive = true;
        }
        public override void LoadContent()
        {
            base.LoadContent();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            drawHPBar(spriteBatch);
            

        }
        public int getHP()
        {
            return HP;
        }
    }
}
