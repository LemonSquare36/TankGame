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

namespace TankGame
{
    internal class GameBoard : GameScreenManager
    {
        Texture2D Outline;

        int columns;
        int rows;

        Rectangle board; 
        Rectangle[] gridarray;

        public override void Initialize()
        {
            //columns = Main.gameWindow.ClientBounds.Width / 10;
            //rows = Main.gameWindow.ClientBounds.Height / 10;

            

            gridarray = getBoard(new Point(100, 100), new Point(600, 600), 6, 6);



        }
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            spriteBatch = spriteBatchmain; 
            Outline = Main.GameContent.Load<Texture2D>("GameSprites/WhiteRectangle");
        }
        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            //spriteBatch.Draw(Outline,new Rectangle(new Point(100, 100), new Point(600, 600)) , Color.Blue);
            //rectangle - first point is the top left corner, second point is the bottom right corner based on the position of point 1
            foreach (Rectangle r in gridarray)
            {
                spriteBatch.Draw(Outline, r, Color.Black);
            }
            

            
        }

        private Rectangle[] getBoard(Point TopLeft, Point BottomRight, int Col, int Rows)
        {
            board = new Rectangle(TopLeft, BottomRight);

            Point size = new Point(board.Width/Col, board.Height/Rows);
            Point location = TopLeft;

            Rectangle[] rectangles = new Rectangle[Col*Rows];
            for (int i = 0; i < rectangles.Length; i++)
            {
                rectangles[i] = new Rectangle(location, size);
                location.X += size.X-1;
                if (location.X > BottomRight.X)
                {
                    location.X = TopLeft.X;
                    location.Y += size.Y-1;
                }

            }
            return rectangles;
        }
    }
}
