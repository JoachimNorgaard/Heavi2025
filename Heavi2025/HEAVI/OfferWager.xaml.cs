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
using WeightGameWPF.Helpers;

namespace HEAVI
{
    /// <summary>
    /// Interaction logic for OfferWager.xaml
    /// </summary>
    public partial class OfferWager : Window
    {
        private DispatcherTimer timeOut;
        private int timeOutCount;
        private Rs232Helper rs232;

        public bool Accepted { get; set; }
        public OfferWager(int timeoutCount, Rs232Helper rs)
        {
            rs232 = rs;
            Random r = new Random(DateTime.Now.Millisecond);
            InitializeComponent();
            
            int select = r.Next(0, 3);
            string text = "";
           
            switch (select)
            {
                case 0:
                    text = "HOLY shit!!";
                    textBlockGreet.FontSize = 250;
                    break;
                case 1:
                    text = "Du milde Gerhard";
                    textBlockGreet.FontSize = 200;
                    break;
                case 2:
                    text = "Den sad lige i skabet";
                    textBlockGreet.FontSize = 180;
                    break;

            }
            textBlockGreet.Text = text;
            timeOut = new DispatcherTimer();
            timeOut.Interval = new TimeSpan(0, 0, 1);
            timeOut.Tick += TimeOut_Tick;
            this.timeOutCount = timeoutCount;
            timeOut.Start();
            //Omit CraneLoweringScreen
            //Display offer to get chips included. Warn that if the loose the crane will raise 5 steps
            //Wait for accept
            //  Timeout->Cranelowering
            //  Accept -> MainWindow (logic for running on wager must be inserted)
            //      Win:WagerWon
            //      Loose: WagerLost
        }

        private void TimeOut_Tick(object sender, EventArgs e)
        {
            if(timeOutCount<=0)
            {
                Accepted = false;
                this.Close();
            }
            else
            {
                timeOutCount--;
                txtCountDown.Text =timeOutCount.ToString();
                if (Accepted == true)
                    this.Close();
                

            }
        }

        private void btnAcceptWager_Click(object sender, RoutedEventArgs e)
        {
            Accepted = true;
            this.Close();

        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
                btnAcceptWager_Click(this, null);
        }
    }
}
