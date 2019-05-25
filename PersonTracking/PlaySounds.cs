using System;

namespace PersonTracking
{
    class PlaySounds
    {
        private static System.Media.SoundPlayer player = new System.Media.SoundPlayer();

        public static void playTone(String toneFile)
        {
            player.SoundLocation = toneFile;
            player.Play();
        }
    }
}
