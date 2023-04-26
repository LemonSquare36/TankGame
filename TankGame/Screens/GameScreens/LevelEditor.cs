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
using TankGame.Objects;

namespace TankGame
{
    internal class LevelEditor : GameScreenManager
    {
        Texture2D LoadH, LoadUH, SaveH, SaveUH;
        Button Load, Save;
        
        //Initialize
        public override void Initialize()
        {

        }
        //LoadContent
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            #region load Textures
            LoadH = Main.GameContent.Load<Texture2D>("Buttons/Editor/LoadH");
            LoadUH = Main.GameContent.Load<Texture2D>("Buttons/Editor/LoadUH");
            SaveH = Main.GameContent.Load<Texture2D>("Buttons/Editor/SaveH");
            SaveUH = Main.GameContent.Load<Texture2D>("Buttons/Editor/SaveUH");
            #endregion
            #region load buttons
            //Load = new Button()
            #endregion
        }
        //Update
        public override void Update()
        {

        }
        //Draw
        public override void Draw()
        {

        }
    }
}
