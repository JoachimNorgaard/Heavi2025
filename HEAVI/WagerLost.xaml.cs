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
    /// Interaction logic for WagerLost.xaml
    /// </summary>
    public partial class WagerLost : Window
    {
        DispatcherTimer timer;
        public WagerLost(int secondsToShow)
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, secondsToShow);
            timer.Tick += Timer_Tick;
            timer.Start();
            //Display screen and sound effects
            //While waiting rewind the crane
            //Return (Go to RoundLost)
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
