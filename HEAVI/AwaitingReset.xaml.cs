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
    /// Interaction logic for AwaitingReset.xaml
    /// </summary>
    public partial class AwaitingReset : Window
    {
        DispatcherTimer timer;
        Settings settings;
        public AwaitingReset()
        {
            InitializeComponent();
            this.Cursor = Cursors.None;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
            settings = new Settings(System.AppDomain.CurrentDomain.BaseDirectory);
            settings.LoadSettings();

        }
        public bool ResetHasHappened=false;

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (ResetHasHappened)
                this.Close();
            else if(settings.GamesWon == settings.GamesInADay )
            {
                ApplicationClosed app = new ApplicationClosed();
                settings.GamesWon = 0;
                settings.SaveSetings();
                app.ShowDialog();
            }


        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
