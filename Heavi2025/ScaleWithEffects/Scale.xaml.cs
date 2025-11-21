using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScaleWithEffects
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class Scale : UserControl
    {
        private int soundPressure;
        public Scale()
        {
            InitializeComponent();
        }
        public int defaultVolume;
        public int SoundPressure
        {
            get
            {
                return soundPressure;
            }
            set
            {
                soundPressure = value;
                txtSoundPresureRight.Text = soundPressure.ToString();
                if (soundPressure>= 0 && soundPressure <= defaultVolume)
                {
                    rectSoundPressureLeft.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
                    rectSoundPressureRight.Fill = rectSoundPressureLeft.Fill;
                }
                else if (soundPressure <= defaultVolume && soundPressure < 60)
                {
                    rectSoundPressureLeft.Fill = new SolidColorBrush(System.Windows.Media.Colors.Yellow);
                    rectSoundPressureRight.Fill = rectSoundPressureLeft.Fill;
                }
                else if (soundPressure>60)
                {
                    rectSoundPressureLeft.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    rectSoundPressureRight.Fill = rectSoundPressureLeft.Fill;
                }


            }
        }
    }
}
