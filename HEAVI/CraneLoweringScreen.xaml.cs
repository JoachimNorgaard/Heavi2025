using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HEAVI
{
    /// <summary>
    /// Interaction logic for CraneLoweringScreen.xaml
    /// </summary>
    /// 
    
    public partial class CraneLoweringScreen : Window
    {
        DispatcherTimer secondTimer;
        int secondsElapsed = 0;
        int secondsToShow;
        string appPath;
        public CraneLoweringScreen(int secondsToShow)
        {
            this.secondsToShow = secondsToShow;
            appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            InitializeComponent();
            this.Cursor = Cursors.None;
            secondTimer = new DispatcherTimer();
            secondTimer.Interval = new TimeSpan(0, 0, 1);
            secondTimer.Tick += SecondTimer_Tick;
            secondTimer.Start();
            mediaPlayer.LoadedBehavior = MediaState.Manual;
            mediaPlayer.IsMuted = true;
            mediaPlayer.Source = new Uri(appPath+@"\Fireworks.mp4", UriKind.Relative);

            mediaPlayer.Play();

        }

        private void SecondTimer_Tick(object sender, EventArgs e)
        {
            if (secondsElapsed < secondsToShow)
            {
                if (secondsElapsed == 3)
                    Utils.AssignShaderEffect(shadowTextBlockCheers);
                else if (secondsElapsed==15)
                    Utils.AssignShaderEffect(shadowTextBlockCheers);
            }
            
            else
            {
                this.Visibility = Visibility.Hidden;
                this.Close();
            }
            secondsElapsed++;
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
