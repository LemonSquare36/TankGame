using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankGame.Objects.Entities
{
    internal class Hole : Entity
    {
        public Hole(RectangleF CurSquare, Point GridLocation) : base(CurSquare, GridLocation)
        {
            curSquare = CurSquare;
            texFile = "GameSprites/BattleSprites/Hole";
            type = "hole";
            size = curSquare.Size / spriteSize;
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
            spriteBatch.Draw(tex, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
        }
        public static Hole Clone(Hole ItemToClone)
        {
            Hole @new = new Hole(ItemToClone.curSquare, ItemToClone.gridLocation);
            @new.Active = ItemToClone.Active;
            @new.alive = ItemToClone.alive;
            @new.tex = ItemToClone.tex;
            @new.texFile = ItemToClone.texFile;
            @new.type = ItemToClone.type;

            return @new;
        }

    }
}
