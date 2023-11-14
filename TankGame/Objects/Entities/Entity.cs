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
using System.Runtime.CompilerServices;
using TankGame.Tools;

namespace TankGame.Objects.Entities
{
    internal class Entity
    {
        public RectangleF curSquare { get; set; }
        protected Vector2 spriteSize = new Vector2(50, 50);
        protected Vector2 size;

        protected Texture2D tex, hpBar;
        protected string texFile;
        public bool Active = false;

        protected Vector2 hpBarLoc, hpBarLocStart;
        protected Vector2 hpBarSize = new Vector2(10, 5);
        protected int HP, curHP;
        public bool alive = true;
        public bool showHealth;

        protected string type;
        public string Type
        {
            get { return type; }
        }
        public Point gridLocation { get; set; }

        public Entity(RectangleF CurrentSquare, Point GridLocation)
        {
            curSquare = CurrentSquare;
            gridLocation = GridLocation;
            size = curSquare.Size / spriteSize;
        }
        public virtual void Initialize(RectangleF newRectangle)
        {
            
        }
        public virtual void LoadContent()
        {
            tex = Main.GameContent.Load<Texture2D>(texFile);
            hpBar = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
        }
        public virtual void Update()
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            if (showHealth)
            {
                drawHPBar(spriteBatch);
            }
        }
        public static Entity Clone(Entity ItemToClone)
        {
            Entity @new = new Entity(ItemToClone.curSquare, ItemToClone.gridLocation);
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

        protected void SetHPBarPos()
        {
            hpBarLocStart = new Vector2(curSquare.Location.X - 10, curSquare.Location.Y + 60);
            hpBarLoc = new Vector2(hpBarLocStart.X, hpBarLocStart.Y);
        }
        protected void drawHPBar(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < curHP; i++)
            {
                if (i < HP)
                {
                    spriteBatch.Draw(hpBar, hpBarLoc, null, Color.Red, 0, Vector2.Zero, hpBarSize, SpriteEffects.None, 0);
                    hpBarLoc = new Vector2(hpBarLoc.X + 20, hpBarLoc.Y);
                }
                else if (i == HP)
                {
                    hpBarLoc = new Vector2(hpBarLocStart.X, hpBarLocStart.Y);
                    spriteBatch.Draw(hpBar, hpBarLoc, null, Color.DeepSkyBlue, 0, Vector2.Zero, hpBarSize, SpriteEffects.None, 0);
                    hpBarLoc = new Vector2(hpBarLoc.X + 20, hpBarLoc.Y);
                }
                else if (i >= HP + 1)
                {
                    spriteBatch.Draw(hpBar, hpBarLoc, null, Color.DeepSkyBlue, 0, Vector2.Zero, hpBarSize, SpriteEffects.None, 0);
                    hpBarLoc = new Vector2(hpBarLoc.X + 20, hpBarLoc.Y);
                }
            }
            SetHPBarPos();
        }
        public void alterHP(int HPChange)
        {
            curHP += HPChange;
            if (curHP > HP * 2)
            {
                curHP = HP * 2;
            }
            if (curHP <= 0)
            {
                alive = false;
                curHP = 0;
            }
        }
    }
}
