using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TankGame.GameInfo;
using TankGame.Tools;
using System.Collections.Generic;

namespace TankGame.Objects.Entities.Items
{
    internal class Items
    {
        protected RectangleF[,] UITiles;
        public Vector2 locationOfAnimation;

        public Items(string SoundFile, string AnimationFile)
        {

        }

        public virtual void UseItem(BoardState boardState, Board curBoard, Pathfinder pathfinder, Point curGridLocation, bool drawTankInfo, int activeTankNum, ButtonState curLeftClick, ButtonState oldLeftClick)
        {
            
        }
        public virtual void GetUIUpdates(Board curBoard, Point curGridLocation)
        {

        }
        public virtual void DrawItemUI(SpriteBatch spriteBatch, Texture2D UITexture)
        {
            
        }
        public virtual void ItemAnimation(SpriteBatch spriteBatch)
        {

        }
        public virtual Animation CreateNewAnimation(Vector2 location)
        {
            return new Animation();
        }
        public virtual Animation CreateNewAnimation()
        {
            return new Animation();
        }
        protected virtual void playItemSound()
        {

        }
    }
}
