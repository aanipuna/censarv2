using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;


namespace ConsoleApplication3
{
    class Greating
    {
        public const int Hazel = 0;
        public const int Heera = 1;
        public const int David = 2;
        public const int Zira = 3;
        public const int Haruka = 4;
        public const int Heami = 5;
        public const int Huihui = 6;
        public const int Tracy = 7;
        public const int Hanhan = 8;
        
        string[] voices = { "Microsoft Hazel Desktop",
                     "Microsoft Heera Desktop",
                     "Microsoft David Desktop",
                     "Microsoft Zira Desktop",
                     "Microsoft Haruka Desktop",
                     "Microsoft Heami Desktop",
                     "Microsoft Huihui Desktop",
                     "Microsoft Tracy Desktop",
                     "Microsoft Hanhan Desktop"};
        private int volume = 100;
        SpeechSynthesizer synth = null;
        //public int volume {set;get;}
        public Greating()
        {
            // Initialize a new instance of the SpeechSynthesizer.  
            synth = new SpeechSynthesizer();
            // Configure the audio output.   
            synth.SetOutputToDefaultAudioDevice();
            // Speak a string
           
        }
        public void great()// greating according to time
        {
            DateTime currentTime = DateTime.Now;
            if (currentTime.Hour < 12 && currentTime.Hour >= 5)
            {
                Console.WriteLine("Good Morning");
                this.speak("Good Morning");
            }
            else if (currentTime.Hour >= 12)
            {
                Console.WriteLine("Good Afternoon");
                this.speak("Good Afternoon");
            }
            else if (currentTime.Hour >= 16)
            {
                Console.WriteLine("Good Evening");
                this.speak("Good Evening");

            }
            else
            {
                Console.WriteLine("Good Night");
                this.speak("Good Night");

            }

        }
        public void setvolume(int volume) {
            
            this.volume = volume;
        }
        public void setVoice(int voice) {
            String defaultVoice = synth.Voice.Name;
            try { synth.SelectVoice(voices[voice]); }
            catch (Exception )
            {
                synth.SelectVoice(defaultVoice);
                
            }

        }
        public void voidEditVoice(VoiceGender gender,VoiceAge age){
            synth.SelectVoiceByHints(gender, age);
        }

        public void speak(String inText)//speech text
        {
            
            synth.Volume = volume;           
            synth.Speak(inText);
            

        }

    }
}

