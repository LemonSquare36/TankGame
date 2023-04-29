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

namespace TankGame.Objects.Entities
{
    internal class Entity
    {
        public RectangleF curSquare { get; set; }

        protected Texture2D tex;
        protected string texFile;
        protected int hp;

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
        }
        public virtual void Initialize()
        {

        }
        public virtual void LoadContent()
        {
            
        }
        public virtual void Update()
        {

        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(tex, curSquare.Location, null, Color.White, 0, Vector2.Zero, curSquare.Size, SpriteEffects.None, 0);
        }
    }
}
