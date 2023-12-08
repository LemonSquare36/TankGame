using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using TankGame.Objects;
using Microsoft.Xna.Framework.Audio;
using TankGame.Tools;
using System.Linq.Expressions;
using TankGame.GameInfo;
using System.Text.RegularExpressions;

namespace TankGame.Tools
{
    internal class Keybind
    {
        public Button button;
        public RectangleF rectangle;
        string fileLocation;
        public string name;
        float scale;

        Keys curBind, newBind;
        public Keys orginalBind;
        public string bindDisplayed;

        public bool curChangingSetting;
        public bool SettingChanged, settingJustChanged;


        public Keybind(Vector2 pos, float width, float height, string FileLocation, string KeyBindingName, Keys KeyBind)
        {
            rectangle = new RectangleF(pos, new Vector2(width, height));
            fileLocation = FileLocation;
            name = KeyBindingName;
            curBind = KeyBind;
            orginalBind = KeyBind;

            scale = height / 60;           
        }
        public void Initialize()
        {

        }
        public void LoadContent()
        {
            button = new Button(rectangle.Location, rectangle.Width, rectangle.Height, fileLocation, name);
            button.ButtonClicked += ButtonClicked;
            newBind = Keys.None;
        }
        public void Update(MouseState mouse, Vector2 worldPos, KeyboardState keyState, KeyboardState oldKeyState)
        {
            button.rectangle = rectangle;
            if (settingJustChanged)
            {
                settingJustChanged = false;
                curBind = (Keys)Enum.Parse(typeof(Keys), newBind.ToString());
                newBind = Keys.None;
            }

            if (!curChangingSetting)
            {
                button.Update(mouse, worldPos);
            }
            else
            {
                if (keyState.GetPressedKeyCount() > 0 && keyState.GetPressedKeyCount() != oldKeyState.GetPressedKeyCount())
                {
                    newBind = keyState.GetPressedKeys()[keyState.GetPressedKeyCount() - 1];
                    curChangingSetting = false;                    
                    settingJustChanged = true;
                }
                if (newBind == Keys.Escape)
                {                    
                    newBind = Keys.None;
                    settingJustChanged = false;
                }
                else if (newBind == orginalBind)
                {
                    SettingChanged = false;
                    newBind = Keys.None;
                    curBind = orginalBind;
                    settingJustChanged = false;
                }
                else if (newBind == curBind)
                {
                    newBind = Keys.None;
                    settingJustChanged = false;
                }
                else if (settingJustChanged)
                {
                    SettingChanged = true;
                }
                
            }
            bindDisplayed = "";
            if (newBind != Keys.None)
            {
                bindDisplayed = newBind.ToString();
            }
            else
            {
                bindDisplayed = curBind.ToString();
            }
        }
        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            Color color = Color.Black;
            if (SettingChanged)
            {
                color = Color.DarkRed;
            }            
            button.Draw(spriteBatch);
            Vector2 messageSize;
            bool alreadyDrawn = false;
            if (bindDisplayed.Length == 2)
            {
                if (bindDisplayed[0] == 'D')
                {
                    messageSize = font.MeasureString(Regex.Match(bindDisplayed, @"\d+").Value);
                    spriteBatch.DrawString(font, Regex.Match(bindDisplayed, @"\d+").Value, rectangle.Location + (rectangle.Size / 2) - (messageSize / 2), color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                    alreadyDrawn = true;
                }
            }
            if (!alreadyDrawn)
            {
                messageSize = font.MeasureString(bindDisplayed);
                spriteBatch.DrawString(font, bindDisplayed, rectangle.Location + (rectangle.Size / 2) - (messageSize / 2), color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }
        public void ChangeSetting()
        {
            Settings.keyBinds[name] = curBind.ToString();
            SettingChanged = false;
            orginalBind = (Keys)Enum.Parse(typeof(Keys) , curBind.ToString());
            newBind = Keys.None;
        }
        public void ButtonClicked(object sender, EventArgs eventArgs)
        {
            if (!curChangingSetting)
            {
                curChangingSetting = true;
            }
        }
        public void SetToNone()
        {
            newBind = Keys.None;
            curBind = Keys.None;
            bindDisplayed = "None";
            if (orginalBind != Keys.None)
            {
                SettingChanged = true;
            }
            else
                SettingChanged = false;
        }
        public void ResetToOldBind()
        {
            curBind = orginalBind;
            newBind = Keys.None;
            SettingChanged = false;
        }
    }
}
