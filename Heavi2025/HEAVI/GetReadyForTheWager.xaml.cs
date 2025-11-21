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
    /// Interaction logic for GetReadyForTheWager.xaml
    /// </summary>
    public partial class GetReadyForTheWager : Window
    {
        DispatcherTimer t;
        public GetReadyForTheWager()
        {
            InitializeComponent();
            t = new DispatcherTimer();
            t.Interval = new TimeSpan(0, 0, 7);
            t.Tick += T_Tick;
            t.Start();
        }

        private void T_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtForBattle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
