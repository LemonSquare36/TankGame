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
        public Wall(RectangleF CurrentSquare, Point GridLocation) : base(CurrentSquare, GridLocation)
        {
            curSquare = CurrentSquare;
            texFile = "GameSprites/Wall";
            type = "wall";
            size = curSquare.Size / spriteSize;

            HP = 3;
            curHP = 3;
            showHealth = false;
            SetHPBarPos();
            alive = true;
        }
        public override void Initialize(RectangleF newRectangle)
        {
            curSquare = newRectangle;
            size = curSquare.Size / spriteSize;
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
        public static Wall Clone(Wall ItemToClone)
        {
            Wall @new = new Wall(ItemToClone.curSquare, ItemToClone.gridLocation);
            @new.Active = ItemToClone.Active;
            @new.alive = ItemToClone.alive;
            @new.curHP = ItemToClone.curHP;
            @new.HP = ItemToClone.HP;
            @new.hpBar = ItemToClone.hpBar;
            @new.hpBarLoc = ItemToClone.hpBarLoc;
            @new.hpBarLocStart = ItemToClone.hpBarLocStart;
            @new.hpBarSize = ItemToClone.hpBarSize;
            @new.showHealth = ItemToClone.showHealth;
            @new.tex = ItemToClone.tex;
            @new.texFile = ItemToClone.texFile;
            @new.type = ItemToClone.type;

            return @new;
        }
    }
}
