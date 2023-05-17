﻿using System;
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
    internal class Mine : Entity
    {
        Vector2 size = new Vector2(50, 50);
        public Mine(RectangleF CurrentSquare, Point GridLocation) : base(CurrentSquare, GridLocation)
        {
            curSquare = CurrentSquare;
            texFile = "GameSprites/BattleSprites/MineUH";
            type = "mine";
            size = curSquare.Size / size;
        }
        public override void LoadContent()
        {
            tex = Main.GameContent.Load<Texture2D>(texFile);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tex, curSquare.Location, null, Color.White, 0, Vector2.Zero, size, SpriteEffects.None, 0);
        }
    }
}
