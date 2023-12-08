using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TankGame.GameInfo;

namespace TankGame.Tools
{
    internal class SoundManager
    {
        private static float MasterVolume, SoundEffectVolume, MusicVolume;

        public SoundManager()
        {

        }
        public static void LoadSoundSettings()
        {
            MasterVolume = Settings.masterVolume;
            SoundEffectVolume = Settings.soundEffectVolume;
            MusicVolume = Settings.musicVolume;
        }
        public static void SaveSoundSettings()
        {

        }
        public static void SetToDefualtSoundSettings()
        {

        }
        public static SoundEffectInstance CreateSound(string FileLocation)
        {
            SoundEffect sound = Main.GameContent.Load<SoundEffect>(FileLocation);
            SoundEffectInstance instance = sound.CreateInstance();

            instance.Volume = (SoundEffectVolume / 100) * (MasterVolume / 100);
            return instance;
        }
        /// <summary>
        /// This checks the volume of a sound instance to make sure that its volume is correct
        /// </summary>
        /// <param name="sound">Sound Instance to check volume for</param>
        public static void VolumeChecker(SoundEffectInstance sound)
        {
            if (sound.Volume != (SoundEffectVolume / 100) * (MasterVolume / 100))
            {
                sound.Volume = (SoundEffectVolume / 100) * (MasterVolume / 100);
            }
        }
        /// <summary>
        /// this checks the volume of the media player to make sure music volume is correct
        /// </summary>
        public static void VolumeChecker()
        {
            if (MediaPlayer.Volume != (MusicVolume / 100) * (MasterVolume / 100))
            {
                MediaPlayer.Volume = (MusicVolume / 100) * (MasterVolume / 100);
            }
        }
        public static void SoundVolumeChanged(object sender, EventArgs e)
        {
            VolumeChecker();
        }
        public static void MusicVolumeChanged(object sender, EventArgs e)
        {
            VolumeChecker((SoundEffectInstance)sender);
        }

    }
}
