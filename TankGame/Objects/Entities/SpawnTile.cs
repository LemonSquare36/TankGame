using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankGame.Objects.Entities
{
    internal class SpawnTile : Entity
    {
        public SpawnTile(RectangleF CurrentSquare, Point GridLocation) : base(CurrentSquare, GridLocation)
        {
            curSquare = CurrentSquare;
            texFile = "GameSprites/spawnHighlight";
            type = "spawn";
            size = curSquare.Size / spriteSize;
        }
        public override void LoadContent()
        {
            base.LoadContent();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, curSquare.Location, null, Color.Gray, 0, Vector2.Zero, curSquare.Size, SpriteEffects.None, 0);
        }
        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(tex, curSquare.Location, null, color, 0, Vector2.Zero, curSquare.Size, SpriteEffects.None, 0);
        }
    }
}
