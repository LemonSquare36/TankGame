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
        Vector2 hpBarSize = new Vector2(10, 5);
        private Vector2 hpBarLoc, hpBarLocStart;
        public int range;
        private int HP = 4;
        public bool alive = true;
        Texture2D hpBar; 
        

        public Tank(RectangleF CurrentSquare, Point GridLocation) : base(CurrentSquare, GridLocation)
        {
            curSquare = CurrentSquare;
            texFile = "GameSprites/BattleSprites/TankUH";
            type = "tank";
            size = curSquare.Size / size;
            range = 4;

            hpBarLocStart = new Vector2(curSquare.Location.X - 10, curSquare.Location.Y + 60);
            hpBarLoc = new Vector2(hpBarLocStart.X, hpBarLocStart.Y);
        }
        public override void LoadContent()
        {
            tex = Main.GameContent.Load<Texture2D>(texFile);
            hpBar = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            for (int i = 0; i < HP; i++)
            {
                if (i < 4)
                {
                    spriteBatch.Draw(hpBar, hpBarLoc, null, Color.Red, 0, Vector2.Zero, hpBarSize, SpriteEffects.None, 0);
                    hpBarLoc = new Vector2(hpBarLoc.X + 20, hpBarLoc.Y);
                }
                else if (i == 4)
                {
                    hpBarLoc = new Vector2(hpBarLocStart.X, hpBarLocStart.Y);
                    spriteBatch.Draw(hpBar, hpBarLoc, null, Color.DeepSkyBlue, 0, Vector2.Zero, hpBarSize, SpriteEffects.None, 0);
                    hpBarLoc = new Vector2(hpBarLoc.X + 20, hpBarLoc.Y);
                }
                else if (i >= 5)
                {
                    spriteBatch.Draw(hpBar, hpBarLoc, null, Color.DeepSkyBlue, 0, Vector2.Zero, hpBarSize, SpriteEffects.None, 0);
                    hpBarLoc = new Vector2(hpBarLoc.X + 20, hpBarLoc.Y);
                }                
            }
            hpBarLocStart = new Vector2(curSquare.Location.X - 10, curSquare.Location.Y + 60);
            hpBarLoc = new Vector2(hpBarLocStart.X, hpBarLocStart.Y);

        }
        public int getHP()
        {
            return HP;
        }
        public void alterHP(int HPChange)
        {
            HP += HPChange;
            if (HP > 8)
            {
                HP = 8;
            }
            if (HP <= 0)
            {
                alive = false;
            }
        }

    }
}
