using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TankGame.GameInfo;
using TankGame.Tools;
using Microsoft.Xna.Framework.Audio;

namespace TankGame.Objects.Entities.Items
{
    internal class Sweeper : Items
    {
        private SoundEffectInstance itemSound;
        private bool playAnimation = false;
        public bool AnimationActive { get { return playAnimation; } }
       

        public Sweeper(string SoundFile, string AnimationFile) : base(SoundFile, AnimationFile)
        {
            itemSound = SoundManager.CreateSound(SoundFile);
        }

        public override void UseItem(BoardState boardState, Board curBoard, Pathfinder pathfinder, Point curGridLocation, bool drawTankInfo, int activeTankNum, ButtonState curLeftClick, ButtonState oldLeftClick)
        {
            
            //play the sound when it gets used
            playItemSound();
            playAnimation = true;
            RectangleF curRectangle = curBoard.getGridSquare(curGridLocation.X, curGridLocation.Y);
            locationOfAnimation = curRectangle.Location - (curRectangle.Size);
            //check for mines
            for (int i = 0; i < UITiles.GetUpperBound(0); i++)
            {
                for (int j = 0; j < UITiles.GetUpperBound(1); j++)
                {
                    for (int k = 0; k < boardState.playerList.Count; k++)
                    {
                        if (k != boardState.curPlayerNum) //means its an enemy player
                        {
                            foreach (Mine mine in boardState.playerList[k].mines)
                            {
                                if (curBoard.getGridSquare(mine.gridLocation.X, mine.gridLocation.Y).Location == UITiles[i, j].Location)
                                {
                                    //update mines for the player who found them
                                    boardState.playerList[boardState.curPlayerNum].mines.Add(mine);
                                    GameScreensManager.UpdatePathFinderWithMines(boardState, pathfinder);
                                }
                            }
                        }
                    }
                }
            }
        }
        public override void GetUIUpdates(Board curBoard, Point curGridLocation)
        {
            UITiles = curBoard.getSubGrid(new Vector2(curGridLocation.X - 1, curGridLocation.Y - 1), new Vector2(4, 4));
        }
        public override void DrawItemUI(SpriteBatch spriteBatch, Texture2D UITexture)
        {
            if (UITiles.LongLength > 0)
            {
                for (int i = 0; i < UITiles.GetUpperBound(0); i++)
                {
                    for (int j = 0; j < UITiles.GetUpperBound(1); j++)
                    {
                        spriteBatch.Draw(UITexture, UITiles[i, j].Location, null, Color.LightSlateGray, 0, Vector2.Zero, UITiles[i, j].Size, SpriteEffects.None, 0);
                    }
                }
            }

        }
        public override Animation CreateNewAnimation(Vector2 location)
        {
            return new Animation("GameSprites/SpriteSheets/BattleSprites/radar", 12, 3, 36, 1, new Rectangle(new Point(), new Point(150, 150)), 1100, location);
        }
        public override Animation CreateNewAnimation()
        {
            return new Animation("GameSprites/SpriteSheets/BattleSprites/radar", 12, 3, 36, 1, new Rectangle(new Point(), new Point(150, 150)), 1100);
        }
        protected override void playItemSound()
        {
            itemSound.Stop();
            itemSound.Play();
        }
    }
}
