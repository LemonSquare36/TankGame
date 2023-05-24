using Microsoft.Xna.Framework;
using TankGame.Objects;

namespace TankGame.Tools
{
    internal class Line
    {
        public float XMin { get; }
        public float XMax { get; }
        public float YMin { get; }
        public float YMax { get; }
        float slope;
        float yOffset;

        public Line(Vector2 p1, Vector2 p2)
        {
            XMin = (p1.X <= p2.X) ? p1.X : p2.X;
            XMax = (p1.X >= p2.X) ? p1.X : p2.X;

            YMin = (p1.Y <= p2.Y) ? p1.Y : p2.Y;
            YMax = (p1.Y >= p2.Y) ? p1.Y : p2.Y;

            slope = (p2.Y - p1.Y) / (p2.X - p1.X);

            yOffset = -((slope * p1.X) - p1.Y);
        }

        public float CalculateY(float x)
        {
            return (slope * x) + yOffset;
        }

        public bool LineSegmentIntersectsRectangle(RectangleF rect)
        {
            //find out if a rectangle is in the path of a line
            //rule out if the rectangle is further left or right of the lines min and max points for X
            if (rect.Left > XMax || rect.Right < XMin)
            {
                return false;
            }
            //rule out if the rectangle is further up or down of the lines min and max points for Y (monogame reverses Y cords)
            if (rect.Bottom < YMin || rect.Top > YMax)
            {
                return false;
            }

            //if the rectangle is near the line and doesnt get ruled out, but still isnt intersecting find the Y for the left and right X
            float yAtRectLeft = float.Parse((CalculateY(rect.Left)).ToString("0.00"));
            float yAtRectRight = float.Parse((CalculateY(rect.Right)).ToString("0.00"));

            float roundedTop = float.Parse(rect.Top.ToString("0.00"));
            float roundedBottom = float.Parse(rect.Bottom.ToString("0.00"));

            if (roundedTop >= yAtRectLeft && roundedTop >= yAtRectRight)
            {
                return false;
            }

            if (roundedBottom <= yAtRectLeft && roundedBottom <= yAtRectRight)
            {
                return false;
            } 

            return true;
        }
    }
}
