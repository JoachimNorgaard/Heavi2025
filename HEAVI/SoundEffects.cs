using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HEAVI
{
    class SoundEffects
    {
        MediaPlayer mPlayer;
        private string effectsPath = @"\Sound\Rendered\";
        private string applicationPath;
        private double vol;
        public string EffectName { get; set; }
        public DateTime timePlayed { get; set; }
        public double Volume
        {
            get
            {
                return (vol);
            }
            set
            {
                vol = value;
                mPlayer.Volume = vol;
            }
        }
        public SoundEffects(string appPath)
        {
            mPlayer = new MediaPlayer();
            applicationPath = appPath + effectsPath;

        }
        public void Play()
        {
            try
            {
                mPlayer.Open(new Uri(applicationPath + EffectName));
                mPlayer.Play();
                timePlayed = DateTime.Now;
            }
            catch (Exception e)
            {

            }
        }
        public void Stop()
        {
            mPlayer.Stop();
        }
    }
}
