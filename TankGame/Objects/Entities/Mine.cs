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

namespace TankGame.Objects.Entities
{
    internal class Mine : Entity
    {
        private static SoundEffectInstance explosion;

        public Mine(RectangleF CurrentSquare, Point GridLocation) : base(CurrentSquare, GridLocation)
        {
            curSquare = CurrentSquare;
            texFile = "GameSprites/BattleSprites/MineUH";
            type = "mine";
            size = curSquare.Size / spriteSize;
        }
        public override void LoadContent()
        {
            tex = Main.GameContent.Load<Texture2D>(texFile);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
        }
        public static Mine Clone(Mine ItemToClone)
        {
            Mine @new = new Mine(ItemToClone.curSquare, ItemToClone.gridLocation);
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
        public static void SetMineSoundEffects(string explosionFileLocation)
        {
            explosion = SoundManager.CreateSound(explosionFileLocation);
        }
        public static void PlayMineExplosion()
        {
            explosion.Stop();
            explosion.Play();
        }
    }
}
