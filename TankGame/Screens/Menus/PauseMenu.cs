using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TankGame.Objects;
using TankGame.Tools;
using TankGame.Objects.Entities;
using TankGame.GameInfo;
using System.Linq;

namespace TankGame.Screens.Menus
{
    internal class PauseMenu : PauseManager
    {
        Texture2D bgTex; 
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);
            bgTex = Main.GameContent.Load<Texture2D>("GameSprites/WhiteDot");
        }
        public override void Update()
        {
            base.Update();
        }
        public override void Draw()
        {
            spriteBatch.Draw(bgTex, new Rectangle(0, 0, Convert.ToInt16(Camera.resolution.X), Convert.ToInt16(Camera.resolution.Y)), Color.Beige);
        }
        public override void ButtonReset()
        {

        }
    }
}
