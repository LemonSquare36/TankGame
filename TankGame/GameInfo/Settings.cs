using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TankGame.Objects.Entities;
using TankGame.Objects;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Input;

namespace TankGame.GameInfo
{

    internal class Settings
    {
        string settingFileLocation;
        public static float masterVolume, soundEffectVolume, musicVolume;
        bool borderless = false;

        public static Dictionary<string, string> keyBinds = new Dictionary<string, string>();

        public Settings()
        {
            settingFileLocation = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\My Games\\TankGame";
            if (!System.IO.Directory.Exists(settingFileLocation))
            {
                System.IO.Directory.CreateDirectory(settingFileLocation);
            }
            if (!System.IO.File.Exists(settingFileLocation + "/settings.ini"))
            {
                //create the file but close the filestream it makes when making the file
                using (System.IO.FileStream fs = System.IO.File.Create(settingFileLocation + "/settings.ini"))
                {
                    fs.Close();
                }
            }
            LoadSettings();
        }
        public void SaveSettings()
        {
            XDocument document = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("Settings",

                new XElement("Audio",
                    new XElement("Master", masterVolume),
                    new XElement("Soundeffects", soundEffectVolume),
                    new XElement("Music", musicVolume)),

                new XElement("Borderless", borderless.ToString()),

                new XElement("Keybinds",
                    from keybind in keyBinds
                    select new XElement(keybind.Key, keybind.Value))));



            document.Save(settingFileLocation + "/settings.ini");
        }
        private void LoadSettings()
        {
            XDocument document = XDocument.Load(settingFileLocation + "/settings.ini");
            XElement parentElemnt = document.Element("Settings").Element("Audio");

            masterVolume = Convert.ToInt16(parentElemnt.Element("Master").Value);
            soundEffectVolume = Convert.ToInt16(parentElemnt.Element("Soundeffects").Value);
            musicVolume = Convert.ToInt16(parentElemnt.Element("Music").Value);

            if (document.Element("Settings").Element("Borderless").Value == "False")
            {
                borderless = false;
            }
            else
            {
                borderless = true;
            }
            parentElemnt = document.Element("Settings").Element("Keybinds");
            foreach (XElement element in parentElemnt.Elements())
            {
                keyBinds.Add(element.Name.ToString(), element.Value);
            }
        }
    }
}
