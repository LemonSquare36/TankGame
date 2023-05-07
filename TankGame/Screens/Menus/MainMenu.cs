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
    internal class MainMenu : ScreenManager
    {
        Button Play, Editor, Settings;
        List<Button> buttonList = new List<Button>();

        //Holds Initialize
        public override void Initialize()
        {

        }
        //Holds LoadContent and the font if called
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);

            #region Button Load
            Play = new Button(new Vector2(800, 200), 300, 150, "Buttons/MainMenu/Play", "play", 0);
            Editor = new Button(new Vector2(800, 400), 300, 150, "Buttons/MainMenu/Editor", "editor", 0);
            Settings = new Button(new Vector2(800, 600), 300, 150, "Buttons/MainMenu/Settings", "settings", 0);
            #endregion

            #region Button Add
            buttonList.Clear();
            buttonList.Add(Play);
            buttonList.Add(Editor);
            buttonList.Add(Settings);
            #endregion

            #region Button Event
            Play.ButtonClicked += ScreenChangeEvent;
            Editor.ButtonClicked += ScreenChangeEvent;
            Settings.ButtonClicked += ScreenChangeEvent;
            #endregion

        }
        //Holds Update
        public override void Update()
        {
            base.Update();

            foreach (Button b in buttonList)
            {
                b.Update(mouse, worldPosition);
            }
        }
        //Holds Draw
        public override void Draw()
        {
            foreach (Button b in buttonList)
            {
                b.Draw(spriteBatch);
            }
        }

        //Holds the Function
        public override void ButtonReset()
        {
            //resets everybutton to prevent unwanted button clicks
            foreach (Button b in buttonList)
            {
                b.ButtonReset();
            }
        }
    }
}
