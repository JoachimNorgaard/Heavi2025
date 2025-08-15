using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AlertLED
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class StatusLED : UserControl
    {
        bool mPulse = false;
        LEDCol mLedColor;
        Storyboard storyboardPulse;
        public enum LEDCol: int { Red=1,Yellow=2,Green = 3,Black=4 }

            

        public StatusLED()
        {
            InitializeComponent();
            storyboardPulse = (Storyboard)FindResource("PulseRed");
            Pulse = false;
            //LEDColor = LEDCol.Green;
        }
        private void UpdateControls(LEDCol l)
        {
            //elipseHotSpot
            //ellipseLED
            //ellipseBracketBurnOff

            switch (l) 
            {
                case LEDCol.Green:
                {
                        RadialGradientBrush rb = new RadialGradientBrush();
                        GradientStop gs = new GradientStop();
                        Color color = (Color)ColorConverter.ConvertFromString("#FF005C00");
                        gs.Color = color;
                        gs.Offset = 1;
                        rb.GradientStops.Add(gs);
                        gs = new GradientStop();
                        color = Colors.Green;
                        gs.Color = color;
                        gs.Offset = 0.819;
                        rb.GradientStops.Add(gs);
                        ellipseLED.Fill = rb;

                        rb = new RadialGradientBrush();
                        gs = new GradientStop();
                        color = (Color)ColorConverter.ConvertFromString("#FFFCFCFC");
                        gs.Color = color;
                        gs.Offset = 0.517;
                        rb.GradientStops.Add(gs);
                        gs = new GradientStop();
                        color = Colors.Green;
                        gs.Color = color;
                        gs.Offset = 0.81;
                        rb.GradientStops.Add(gs);
                        elipseHotSpot.Fill = rb;

                        rb = new RadialGradientBrush();
                        gs = new GradientStop();
                        color = Colors.Green;
                        gs.Color = color;
                        gs.Offset = 0.735;
                        rb.GradientStops.Add(gs);
                        gs = new GradientStop();
                        color = (Color)ColorConverter.ConvertFromString("#C3838282");
                        gs.Color = color;
                        gs.Offset = 1;
                        rb.GradientStops.Add(gs);
                        ellipseBracketBurnOff.Fill = rb;

                        break;
                }
                case LEDCol.Yellow:
                    {
                        RadialGradientBrush rb = new RadialGradientBrush();
                        GradientStop gs = new GradientStop();
                        Color color = (Color)ColorConverter.ConvertFromString("#FF8C8C00");
                        gs.Color = color;
                        gs.Offset = 1;
                        rb.GradientStops.Add(gs);
                        gs = new GradientStop();
                        color = Colors.Yellow;
                        gs.Color = color;
                        gs.Offset = 0.819;
                        rb.GradientStops.Add(gs);
                        ellipseLED.Fill = rb;

                        rb = new RadialGradientBrush();
                        gs = new GradientStop();
                        color = (Color)ColorConverter.ConvertFromString("#FFFCFCFC");
                        gs.Color = color;
                        gs.Offset = 0.517;
                        rb.GradientStops.Add(gs);
                        gs = new GradientStop();
                        color = Colors.Yellow;
                        gs.Color = color;
                        gs.Offset = 0.81;
                        rb.GradientStops.Add(gs);
                        elipseHotSpot.Fill = rb;

                        rb = new RadialGradientBrush();
                        gs = new GradientStop();
                        color = Colors.Yellow;
                        gs.Color = color;
                        gs.Offset = 0.735;
                        rb.GradientStops.Add(gs);
                        gs = new GradientStop();
                        color = (Color)ColorConverter.ConvertFromString("#C3838282");
                        gs.Color = color;
                        gs.Offset = 1;
                        rb.GradientStops.Add(gs);
                        ellipseBracketBurnOff.Fill = rb;


                        break;
                    }
                case LEDCol.Red:
                    {
                        RadialGradientBrush rb = new RadialGradientBrush();
                        GradientStop gs = new GradientStop();
                        Color color = (Color)ColorConverter.ConvertFromString("#FF8C0000");
                        gs.Color = color;
                        gs.Offset = 1;
                        rb.GradientStops.Add(gs);
                        gs = new GradientStop();
                        color = Colors.Red;
                        gs.Color = color;
                        gs.Offset = 0.819;
                        rb.GradientStops.Add(gs);
                        ellipseLED.Fill = rb;

                        rb = new RadialGradientBrush();
                        gs = new GradientStop();
                        color = (Color)ColorConverter.ConvertFromString("#FFFCFCFC");
                        gs.Color = color;
                        gs.Offset = 0.517;
                        rb.GradientStops.Add(gs);
                        gs = new GradientStop();
                        color = Colors.Red;
                        gs.Color = color;
                        gs.Offset = 0.81;
                        rb.GradientStops.Add(gs);
                        elipseHotSpot.Fill = rb;

                        rb = new RadialGradientBrush();
                        gs = new GradientStop();
                        color = Colors.Red;
                        gs.Color = color;
                        gs.Offset = 0.735;
                        rb.GradientStops.Add(gs);
                        gs = new GradientStop();
                        color = (Color)ColorConverter.ConvertFromString("#C3838282");
                        gs.Color = color;
                        gs.Offset = 1;
                        rb.GradientStops.Add(gs);
                        ellipseBracketBurnOff.Fill = rb;

                        break;
                    }
                case LEDCol.Black:
                    {
                        RadialGradientBrush rb = new RadialGradientBrush();
                        GradientStop gs = new GradientStop();

                        ellipseLED.Fill = new SolidColorBrush(Colors.Black);

                        rb = new RadialGradientBrush();
                        gs = new GradientStop();
                        Color color = (Color)ColorConverter.ConvertFromString("#FFFCFCFC");
                        gs.Color = color;
                        gs.Offset = 0.517;
                        rb.GradientStops.Add(gs);
                        gs = new GradientStop();
                        color = Colors.Black;
                        gs.Color = color;
                        gs.Offset = 0.81;
                        rb.GradientStops.Add(gs);
                        elipseHotSpot.Fill = rb;

                        break;
                    }
            }
        }
        public LEDCol LEDColor
        {
            get
            {
                return mLedColor;
            }
            set
            {
                mLedColor = value;
                UpdateControls(value);
            }
        }
        public bool Pulse
        {
            get
            {
                
                return mPulse;
            }
            set
            {
                if (value)
                {
                    if (value != mPulse)
                    {
                        ellipseBracketBurnOff.Visibility = Visibility.Visible;
                        storyboardPulse.Begin();
                        
                    }
                    mPulse = value;
                }
                else
                {
                    //LEDColor = LEDCol.Red;
                    storyboardPulse.Seek(new TimeSpan(0));
                    storyboardPulse.Stop();
                    ellipseBracketBurnOff.Visibility = Visibility.Hidden;
                }
                mPulse = value;
            }
        }
    }
}
