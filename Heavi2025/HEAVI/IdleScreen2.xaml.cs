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
using System.Windows.Input.StylusPlugIns;

using System.Windows.Ink;

namespace HEAVI
{
   
    /// <summary>
    /// Interaction logic for IdleScreen2.xaml
    /// </summary>
    public partial class IdleScreen2 : Window
    {
        public IdleScreen2()
        {
            InitializeComponent();
            double width = grid.Width-40;
            double height = grid.Height-40;
            int thickness = 3;
            int countOfBars = 4;
           
            for (int j = 0; j < countOfBars; j++)
            {
                byte c = Convert.ToByte(255-(j * 50));
                Rectangle r = CreateRectangle(width+(j*thickness), height+(j*thickness), Color.FromRgb(c,c,c), (j * thickness), (j * thickness), (j * thickness), (j * thickness));
                grid.Children.Add(r);
            }
            for (int j = countOfBars-1; j > 0; j--)
            {
                byte c = Convert.ToByte(255-(j * 50));
                Rectangle r = CreateRectangle(width - (j * thickness), height - (j * thickness), Color.FromRgb(c, c, c), -(j * thickness), -(j * thickness), -(j * thickness), -(j * thickness));
                grid.Children.Add(r);
                if (j==0)
                    r.Fill = new SolidColorBrush(Colors.DarkGray);
            }
            //r = CreateRectangle(width+10, height+10, Colors.LightGray,-5,-5,-5,-5);
            //grid.Children.Add(r);
            //r = CreateRectangle(width + 20, height + 20, Colors.LightGray, -10, -10, -10, -10);
            //grid.Children.Add(r);


        }
        private Rectangle CreateRectangle(double width,double height,Color col,int offsetX,int offsetY,int offsetX2,int offsetY2)
        {
            Rectangle r = new Rectangle();
            r.Width = width;
            r.Height = height;
            r.Margin = new Thickness(offsetX, offsetY, offsetX2, offsetY2);
            r.RadiusX = 20;
            r.RadiusY = 20;
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = col;
            r.StrokeThickness = 3;
            r.Stroke = new SolidColorBrush(col);
           
            return r;
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
