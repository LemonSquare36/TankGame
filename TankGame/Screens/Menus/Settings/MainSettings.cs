using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankGame.Tools;
using TankGame.GameInfo;
using System;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using TankGame.Objects;

namespace TankGame.Screens.Menus.Settings
{
    internal class MainSettings
    {
        Texture2D bgTex;
        Button SaveButton, MainMenu;
        Keybind conflictedBind, Multi, Destroy, SpawnTile, Eraser, Hole, Wall, ItemBox, Load, Save;
        List<Button> buttons = new();
        List<Keybind> editorKeybinds = new();
        SpriteFont font;
        bool notSaved, missingKeyBind, conflictingBinds, otherActiveBind;
        public bool anyObjectActive;
        ScrollBox settingsBox;
        public EventHandler SettingsSaved;

        public void Initialize()
        {

        }
        public void LoadContent(SpriteBatch spriteBatchmain)
        {
            bgTex = Main.GameContent.Load<Texture2D>("Backgrounds/PauseBG");
            font = Main.GameContent.Load<SpriteFont>("Fonts/DefualtFont");

            #region buttons
            //clear list
            buttons.Clear();
            //load buttons
            SaveButton = new Button(new Vector2(700, 100), 100, 50, "Buttons/Settings/Save", "save");
            //events
            SaveButton.ButtonClicked += SaveKeyBinds;
            //list add
            buttons.Add(SaveButton);
            //noise
            foreach (Button button in buttons)
            {
                button.addSoundEffect("Sounds/click");
            }
            #endregion

            #region keybinds
            
            //loading the keybinds
            Multi = new Keybind(new Vector2(1100, 100), 120, 60, "Buttons/Settings/Keybinds", "multiWallKey", (Keys)Enum.Parse(typeof(Keys), GameInfo.Settings.keyBinds["multiWallKey"]));
            Destroy = new Keybind(new Vector2(1100, 200), 120, 60, "Buttons/Settings/Keybinds", "destroyableKey", (Keys)Enum.Parse(typeof(Keys), GameInfo.Settings.keyBinds["destroyableKey"]));
            SpawnTile = new Keybind(new Vector2(1100, 300), 120, 60, "Buttons/Settings/Keybinds", "spawnTileKey", (Keys)Enum.Parse(typeof(Keys), GameInfo.Settings.keyBinds["spawnTileKey"]));
            Eraser = new Keybind(new Vector2(1100, 400), 120, 60, "Buttons/Settings/Keybinds", "eraserKey", (Keys)Enum.Parse(typeof(Keys), GameInfo.Settings.keyBinds["eraserKey"]));
            ItemBox = new Keybind(new Vector2(1100, 500), 120, 60, "Buttons/Settings/Keybinds", "itemBoxKey", (Keys)Enum.Parse(typeof(Keys), GameInfo.Settings.keyBinds["itemBoxKey"]));
            Hole = new Keybind(new Vector2(1100, 600), 120, 60, "Buttons/Settings/Keybinds", "holeKey", (Keys)Enum.Parse(typeof(Keys), GameInfo.Settings.keyBinds["holeKey"]));
            Wall = new Keybind(new Vector2(1100, 700), 120, 60, "Buttons/Settings/Keybinds", "wallKey", (Keys)Enum.Parse(typeof(Keys), GameInfo.Settings.keyBinds["wallKey"]));            
            Load = new Keybind(new Vector2(1100, 1200), 120, 60, "Buttons/Settings/Keybinds", "loadKey", (Keys)Enum.Parse(typeof(Keys), GameInfo.Settings.keyBinds["loadKey"]));
            Save = new Keybind(new Vector2(1100, 1500), 120, 60, "Buttons/Settings/Keybinds", "saveKey", (Keys)Enum.Parse(typeof(Keys), GameInfo.Settings.keyBinds["saveKey"]));
            //clear list
            editorKeybinds.Clear();
            //list add
            editorKeybinds.Add(Multi);
            editorKeybinds.Add(Destroy);
            editorKeybinds.Add(SpawnTile);
            editorKeybinds.Add(Eraser);
            editorKeybinds.Add(Hole);
            editorKeybinds.Add(Wall);
            editorKeybinds.Add(ItemBox);
            editorKeybinds.Add(Load);
            editorKeybinds.Add(Save);


            //give the buttons a click noise
            foreach (Keybind keybind in editorKeybinds)
            {
                keybind.LoadContent();
                keybind.button.addSoundEffect("Sounds/click");
            }
            #endregion

            settingsBox = new ScrollBox(new RectangleF(560, 80, 800, 900), new RectangleF(560, 80, 800, 2000), 10);
            settingsBox.updateItems = false;
            settingsBox.AddList(buttons);
            settingsBox.AddList(editorKeybinds);
        }
        public void Update(MouseState mouse, Vector2 worldPosition, KeyboardState keyState, KeyboardState oldKeyState)
        {
            settingsBox.Update(mouse, worldPosition, keyState, oldKeyState);
            //update buttons in the menu and let the game know which ones are active
            foreach (Button button in buttons)
            {
                button.Update(mouse, worldPosition);
                if (button.ButtonActive)
                {
                    anyObjectActive = true;
                }
            }
            //if a bind is active no others can be active
            if (editorKeybinds.Any(x => x.curChangingSetting))
            {
                otherActiveBind = true;
                //let the manager know there is an active object
                anyObjectActive = true;
            }
            else
            {
                otherActiveBind = false;
                anyObjectActive = false;
            }


            foreach (Keybind keybind in editorKeybinds)
            {
                if (otherActiveBind)
                {//if a bind is active only update that bind
                    if (keybind.curChangingSetting)                    
                        keybind.Update(mouse, worldPosition, keyState, oldKeyState);                    
                }
                else //if no bind is active update all binds
                {
                    keybind.Update(mouse, worldPosition, keyState, oldKeyState);
                }
                //if the keybind has had a setting changed
                if (keybind.settingJustChanged)
                {
                    //see if it was changed to any other currently set ones
                    List<Keybind> conflict = editorKeybinds.FindAll(x => x.bindDisplayed == keybind.bindDisplayed);
                    if (conflict.Count() > 1)
                    {
                        foreach (Keybind key in conflict)
                        {
                            if (!key.settingJustChanged)
                            {
                                //find the old key conflicting with the new one
                                conflictedBind = key;
                                conflictingBinds = true;
                            }
                        }
                    }
                    conflict.Clear();
                }
            }
            if (conflictingBinds)
            {
                //set the conflicted key to no bind
                conflictedBind.SetToNone();

                //dont changed the bind. Just reset the one changed to what it was
                /*foreach (Keybind keybind in keybinds)
                {
                    if (keybind.bindDisplayed == conflictedBind.bindDisplayed && keybind.orginalBind != conflictedBind.orginalBind)
                    {
                        keybind.ResetToOldBind();
                        break;
                    }
                }*/
                conflictingBinds = false;
            }

        }
        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(bgTex, new Rectangle(560, 40, 800, 1000), Color.White);
            settingsBox.Draw(spriteBatch, font);
            /*foreach (Button button in buttons)
            {
                button.Draw(spriteBatch);
            }
            foreach (Keybind keybind in editorKeybinds)
            {
                keybind.Draw(spriteBatch, font);
            }*/
        }
        public void ButtonReset()
        {

        }
        public void ResetAllActiveObjects()
        {
            foreach (Button button in buttons)
            {
                button.ButtonReset();
            }
            foreach (Keybind keybind in editorKeybinds)
            {
                keybind.button.ButtonReset();
            }
        }
        public void SaveKeyBinds(object sender, EventArgs e)
        {
            foreach (Keybind keybind in editorKeybinds)
            {
                if (keybind.SettingChanged)
                {
                    keybind.ChangeSetting();
                    GameInfo.Settings.keyBinds[keybind.name] = keybind.orginalBind.ToString();
                }
            }
            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }
    }
}
