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
    /// Interaction logic for RoundLostScreen.xaml
    /// </summary>
    public partial class RoundLostScreen : Window
    {
        DispatcherTimer timer;
        public RoundLostScreen(int secondsBeforeHide)
        {
            InitializeComponent();
            this.Cursor = Cursors.None;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, secondsBeforeHide);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void textBlockThereWillBe_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
