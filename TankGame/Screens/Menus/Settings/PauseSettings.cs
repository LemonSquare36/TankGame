using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankGame.Tools;
using TankGame.GameInfo;
using System;
using Microsoft.Xna.Framework.Input;

namespace TankGame.Screens.Menus.Settings
{
    internal class PauseSettings : PauseManager
    {
        MainSettings mainSettings;
        public override void Initialize()
        {
            base.Initialize();
            mainSettings = new MainSettings();
            mainSettings.Initialize();

            mainSettings.SettingsSaved += OnSettingsSave;
        }
        public override void LoadContent(SpriteBatch spriteBatchmain)
        {
            base.LoadContent(spriteBatchmain);
            mainSettings.LoadContent(spriteBatch);
        }
        public override void Update()
        {
            base.Update();
            mainSettings.Update(mouse, worldPosition, keyState, keyHeldState);
            anyObjectActive = mainSettings.anyObjectActive;
        }
        public override void Draw()
        {
            base.Draw();
            mainSettings.Draw(spriteBatch);
        }
        public override void ButtonReset()
        {

        }
    }
}

