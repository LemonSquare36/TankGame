using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace TankGame.Objects.Entities
{
    internal class Tank : Entity
    {             
        public int range;
        public int damage;
        private string texFileDead;
        private Texture2D texDead;

        public Tank(RectangleF CurrentSquare, Point GridLocation) : base(CurrentSquare, GridLocation)
        {
            curSquare = CurrentSquare;
            texFile = "GameSprites/BattleSprites/TankUH";
            texFileDead = "GameSprites/BattleSprites/TankDead";
            type = "tank";
            size = curSquare.Size / spriteSize;
            range = 4;
            damage = 2; 
            HP = 4;
            curHP = 4;
            SetHPBarPos();
            alive = true;
        }
        public override void LoadContent()
        {
            base.LoadContent();
            texDead = Main.GameContent.Load<Texture2D>(texFileDead);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (alive)
            {
                spriteBatch.Draw(tex, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            }
            if (!alive)
            {
                spriteBatch.Draw(texDead, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
            }
            drawHPBar(spriteBatch);            
        }
        public int getHP()
        {
            return HP;
        }
        public static Tank Clone(Tank ItemToClone)
        {
            Tank @new = new Tank(ItemToClone.curSquare, ItemToClone.gridLocation);
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
            @new.texDead = ItemToClone.texDead;
            @new.type = ItemToClone.type;

            return @new;
        }
    }
}
