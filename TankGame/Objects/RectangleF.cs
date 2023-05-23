using Microsoft.Xna.Framework;

namespace TankGame.Objects
{
    internal struct RectangleF
    {
        //Properties that make up the rectangle
        //Location of the rectangle, the topleft corner
        public float X, Y;
        //the size of the rectangle measure out from the topleft corner
        public float Width, Height;
        public bool Null = false;

        //get the location of the rectagle
        public Vector2 Location
        {
            get
            {
                return new Vector2(this.X, this.Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        //get the size of the rectangle
        public Vector2 Size
        {
            get
            {
                return new Vector2(this.Width, this.Height);
            }
            set
            {
                Width = value.X;
                Height = value.Y;
            }
        }
        //get the center of the rectangle
        public Vector2 Center
        {
            get
            {
                return new Vector2(this.X + (this.Width / 2), this.Y + (this.Height / 2));
            }
        }
        //get the sides of the rectangles
        public float Left
        {
            get { return this.X; }
        }

        public float Right
        {
            get { return (this.X + this.Width); }
        }

        public float Top
        {
            get { return this.Y; }
        }

        public float Bottom
        {
            get { return (this.Y + this.Height); }
        }
        //constructors
        /// <summary>
        /// Creates a new instance of <see cref="Rectangle"/> struct, with the specified
        /// position, width, and height.
        /// </summary>
        /// <param name="x">The x coordinate of the top-left corner of the created <see cref="Rectangle"/>.</param>
        /// <param name="y">The y coordinate of the top-left corner of the created <see cref="Rectangle"/>.</param>
        /// <param name="width">The width of the created <see cref="Rectangle"/>.</param>
        /// <param name="height">The height of the created <see cref="Rectangle"/>.</param>
        public RectangleF(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
        /// <summary>
        /// Creates a new instance of <see cref="Rectangle"/> struct, with the specified
        /// position, width, and height.
        /// </summary>
        /// <param name="x">The x coordinate of the top-left corner of the created <see cref="Rectangle"/>.</param>
        /// <param name="y">The y coordinate of the top-left corner of the created <see cref="Rectangle"/>.</param>
        /// <param name="width">The width of the created <see cref="Rectangle"/>.</param>
        /// <param name="height">The height of the created <see cref="Rectangle"/>.</param>
        public RectangleF(Vector2 location, Vector2 size)
        {
            this.X = location.X;
            this.Y = location.Y;
            this.Width = size.X;
            this.Height = size.Y;

        }
        /// <summary>
        /// Creates a new instance of <see cref="Rectangle"/> struct, with the specified
        /// position, width, and height. NO PARAMS CREATES ALL 0's AND SETS BOOL 'Null' TO TRUE
        /// </summary>
        public RectangleF()
        {
            this.X = 0;
            this.Y = 0;
            this.Width = 0;
            this.Height = 0;
            Null = true;
        }

        //contains. Used to see if something is in the rectangle
        //returns a bool based on the answer
        public bool Contains(Vector2 value)
        {
            return ((((this.X <= value.X) && (value.X < (this.X + this.Width))) && (this.Y <= value.Y)) && (value.Y < (this.Y + this.Height)));
        }
        public bool Contains(RectangleF value)
        {
            return ((((this.X <= value.X) && ((value.X + value.Width) <= (this.X + this.Width))) && (this.Y <= value.Y)) && ((value.Y + value.Height) <= (this.Y + this.Height)));
        }


    }
}
