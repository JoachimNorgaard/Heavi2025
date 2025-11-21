using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for WagerWon.xaml
    /// </summary>
    public partial class WagerWon : Window
    {
        DispatcherTimer timer;
        int countOfImages = 0;
        string appPath;
        string[] fileNames = new string[]
        {
          "LaysYellow.png",
          "LaysBlue.png",
          "LaysBlack.png",
          "LaysRed.png",
          "LaysLightGreen.png",
          "LaysGreen.png"
        };
        int waitCounter = 0;
        Random imgRandom;
        DateTime windowLoadedAt;
        int secondsBeforeHide;
        public WagerWon(int secs)
        {
            InitializeComponent();
            secondsBeforeHide = secs;
            windowLoadedAt = DateTime.Now;
            imgRandom = new Random(DateTime.Now.Millisecond);
            appPath = Directory.GetCurrentDirectory();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(100);
            timer.Tick += Timer_Tick;
            timer.Start();

            //Display Chips falling and fireworks.
            //For 30seconds
            //Return (and wait for reset)
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (waitCounter >= 500) // 600 = 10 sec
            {
                AnimateImage(appPath, countOfImages * 10);
                countOfImages++;
                if (countOfImages > 50)
                {
                    countOfImages = 0;
                    waitCounter = 0;
                }
            }
            else
                waitCounter++;
            TimeSpan t = DateTime.Now - windowLoadedAt;
            if (t.Seconds>secondsBeforeHide)
            {
                timer.Stop();
                this.Close();
            }
        }
        private void MoveTo(Canvas canvas, Image target, int toX, int toY, double time)
        {
            Random random = new Random(DateTime.Now.Millisecond);
            TransformGroup group = new TransformGroup();
            TranslateTransform trans = new TranslateTransform();
            int rotateDeg = imgRandom.Next(1, 50) - 30;
            RotateTransform rotateTransform = new RotateTransform(rotateDeg, 0.5, 0.5);
            //target.RenderTransformOrigin = new Point(0.5, 0.5);
            group.Children.Add(rotateTransform);
            group.Children.Add(trans);

            target.RenderTransform = group;

            DoubleAnimation animX = new DoubleAnimation(0, toX, TimeSpan.FromMilliseconds(time));
            DoubleAnimation animY = new DoubleAnimation(0, toY, TimeSpan.FromMilliseconds(time));

            DoubleAnimation rotate = new DoubleAnimation(0, rotateDeg, TimeSpan.FromMilliseconds(time));
            TimeSpan t = new TimeSpan(random.Next(100, 1000));
            animX.BeginTime = t;
            animY.BeginTime = t;
            animY.AccelerationRatio = 1;
            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotate);
            trans.BeginAnimation(TranslateTransform.XProperty, animX);
            trans.BeginAnimation(TranslateTransform.YProperty, animY);

        }
        private Image getRandomImage()
        {

            int r = imgRandom.Next(0, 5);
            string fileName = appPath + "/GFX/" + fileNames[r];
            BitmapImage bitmapImage = new BitmapImage(new Uri(fileName));
            Image img = new Image();
            img.Source = bitmapImage;
            int size = imgRandom.Next(50, 100);
            img.Width = size;
            img.Height = size;
            img.Margin = new Thickness(0, 0, 0, 0);
            return img;
        }
        private void AnimateImage(string appPath, int xOffset)
        {
            Random timeRandom = new Random(DateTime.Now.Millisecond);
            Random xRandom = new Random(DateTime.Now.Millisecond);

            Image img = getRandomImage();
            canvas.Children.Add(img);
            int x = xRandom.Next(300, 1000);
            int xDirection = x - xRandom.Next(300, 1000);
            Canvas.SetLeft(img, x);
            Canvas.SetTop(img, -100);

            MoveTo(canvas, img, xDirection, 450, timeRandom.Next(300, 1500));
        }
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }

    
    
}
