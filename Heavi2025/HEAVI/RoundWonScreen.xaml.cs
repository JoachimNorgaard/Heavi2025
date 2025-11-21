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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HEAVI
{
    /// <summary>
    /// Interaction logic for RoundWonScreen.xaml
    /// </summary>
    public partial class RoundWonScreen : Window
    {
        DispatcherTimer timer;
        public RoundWonScreen(int secondsBeforeHide,int roundsLeft)
        {
            InitializeComponent();
            this.Cursor = Cursors.None;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0,0,secondsBeforeHide);
            timer.Tick += Timer_Tick;
            timer.Start();
            if (roundsLeft == 1)
            {
                //Storyboard animateWarning = (Storyboard)this.FindResource("AnimateWarning");
                //grdWaltherKortDescriptor.Visibility = Visibility.Visible;
                //animateWarning.Begin();
                textBlockTheCaseOfBeers.Text = "Der er kun ET ENESTE trin før at de kolde øller kan nåes";
            }
            else
            {
                textBlockTheCaseOfBeers.Text = "Der er " + roundsLeft + " trin tilbage før øllerne er jeres";
                //grdWaltherKortDescriptor.Visibility = Visibility.Hidden;
            }

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
