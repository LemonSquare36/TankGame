using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TankGame.Objects.Entities
{
    internal class ItemBox : Entity
    {
        public ItemBox(RectangleF CurrentSquare, Point GridLocation) : base(CurrentSquare, GridLocation)
        {
            curSquare = CurrentSquare;
            texFile = "GameSprites/ItemBox";
            type = "itembox";
            size = curSquare.Size / spriteSize;
            alive = true;
        }
        public override void Initialize(RectangleF newRectangle)
        {            
            curSquare = newRectangle;
            size = curSquare.Size / spriteSize;
        }
        public override void LoadContent()
        {
            tex = Main.GameContent.Load<Texture2D>(texFile);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (alive)
            spriteBatch.Draw(tex, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
        }
        public static ItemBox Clone(ItemBox ItemToClone)
        {
            ItemBox @new = new ItemBox(ItemToClone.curSquare, ItemToClone.gridLocation);
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