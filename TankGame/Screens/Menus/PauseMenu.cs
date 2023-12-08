using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TankGame.Screens.Menus
{
    internal class PauseMenu : PauseManager
    {
        Texture2D bgTex;
        Button Resume, Settings, MainMenu;
        List<Button> buttons = new();
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);
            bgTex = Main.GameContent.Load<Texture2D>("Backgrounds/PauseBG");

            //buttons load
            Resume = new Button(new Vector2(800, 200), 300, 150, "Buttons/Pause/Resume", "resume");
            Settings = new Button(new Vector2(800, 400), 300, 150, "Buttons/Pause/Settings", "settings");
            MainMenu = new Button(new Vector2(800, 600), 300, 150, "Buttons/Pause/MainMenu", "mainmenu");

            //button events
            Resume.ButtonClicked += ScreenChangeEvent;
            Settings.ButtonClicked += ScreenChangeEvent;
            MainMenu.ButtonClicked += ScreenChangeEvent;

            //buttons list add
            buttons.Add(Resume);
            buttons.Add(Settings);
            buttons.Add(MainMenu);
            //give the buttons a click noise
            foreach (Button button in buttons)
            {
                button.addSoundEffect("Sounds/click");
            }
        }
        public override void Update()
        {
            base.Update();
            foreach (Button button in buttons)
            {
                button.Update(mouse, worldPosition);
            }
        }
        public override void Draw()
        {
            base.Draw();
            spriteBatch.Draw(bgTex, new Rectangle(560, 40, 800, 1000), Color.White);
            foreach (Button button in buttons)
            {
                button.Draw(spriteBatch);
            }
        }
        public override void ButtonReset()
        {

        }
    }
}
