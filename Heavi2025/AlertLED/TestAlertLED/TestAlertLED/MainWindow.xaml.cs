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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestAlertLED
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnPulse_Click(object sender, RoutedEventArgs e)
        {
            alertLED.Pulse = !alertLED.Pulse;
        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            alertLED.LEDColor = AlertLED.StatusLED.LEDCol.Red;
        }

        private void btnYellow_Click(object sender, RoutedEventArgs e)
        {
            alertLED.LEDColor = AlertLED.StatusLED.LEDCol.Yellow;
        }

        private void btnGreen_Click(object sender, RoutedEventArgs e)
        {
            alertLED.LEDColor = AlertLED.StatusLED.LEDCol.Green;
        }

        private void btnOff_Click(object sender, RoutedEventArgs e)
        {
            alertLED.LEDColor = AlertLED.StatusLED.LEDCol.Black;
        }
    }
}
