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
    /// Interaction logic for ApplicationClosed.xaml
    /// </summary>
    public partial class ApplicationClosed : Window
    {
        DispatcherTimer heartbeat;
        public ApplicationClosed()
        {
            InitializeComponent();
            this.Cursor = Cursors.None;
            heartbeat = new DispatcherTimer();
            heartbeat.Interval = new TimeSpan(0, 0, 10);
            heartbeat.Tick += Heartbeat_Tick;
            heartbeat.Start();
        }

        private void Heartbeat_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.TimeOfDay.Hours>12 && DateTime.Now.TimeOfDay.Hours<23)
            {
                this.Close();
            }
            
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
