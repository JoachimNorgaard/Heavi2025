using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace HEAVI
{
    /// <summary>
    /// Interaction logic for IdleScreen1.xaml
    /// </summary>
    public partial class IdleScreen1 : Window
    {
        DispatcherTimer secondsTimer;
        int secondsBeforeNext;
        int roundsToGo;
        int roundsTotalBeforeBeerRelease;
        Storyboard onlyOneRoundLeft;
        Storyboard beerArrowIn;
        Settings settings;
        public IdleScreen1(int secondsBeforeNextGame,int roundstogo, int roundstotal)
        {
            InitializeComponent();
            settings = new Settings(System.AppDomain.CurrentDomain.BaseDirectory);
            settings.LoadSettings();
            this.Cursor = Cursors.None;
            secondsBeforeNext = secondsBeforeNextGame;
            secondsTimer = new DispatcherTimer();
            secondsTimer.Interval = new TimeSpan(0,0,1);
            secondsTimer.Tick += SecondsTimer_Tick;
            secondsTimer.Start();
            roundsToGo = roundstogo;
            roundsTotalBeforeBeerRelease = roundstotal;
            onlyOneRoundLeft = (Storyboard)this.FindResource("RevielOnlyOneRoundLeft");
            beerArrowIn = (Storyboard)this.FindResource("BeerArrowIn");
            gridOneRoundLeft.Visibility = Visibility.Hidden;
            renderRig();
        }

        private void SecondsTimer_Tick(object sender, EventArgs e)
        {
            secondsBeforeNext--;

            TimeSpan t = new TimeSpan(0, 0, secondsBeforeNext);

            string formatted = t.Minutes.ToString("D2") + ":" + t.Seconds.ToString("D2");
            textBlockTimeToNextGameNumber.Content = formatted;
            if (secondsBeforeNext == 0)
                this.Close();
            if (roundsToGo==1)
            {
                if (secondsBeforeNext % 11 == 0)
                {
                    onlyOneRoundLeft.Begin(this, true);
                }
            }
            if (secondsBeforeNext % 10 == 0)
                beerArrowIn.Begin(this, true);
            
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void textBlockHelpGetToTheBeers_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
           
        }
        private void renderRig()
        {
            double pixelsToUse = 880 - imageCrate.Height;
            double stepSize = pixelsToUse / roundsTotalBeforeBeerRelease;            
            Thickness t = imageCrate.Margin;
            int crateOrigin = -150;
            
            t.Top = crateOrigin + stepSize * (roundsTotalBeforeBeerRelease - roundsToGo);
            imageCrate.Margin = t;
            imageCrate.UpdateLayout();            

        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            


        }
            
            
    }
}
