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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HEAVI
{
    /// <summary>
    /// Interaction logic for GlareStar.xaml
    /// </summary>
    public partial class GlareStar : UserControl
    {
        Storyboard glareStoryboard;
        public GlareStar()
        {
            InitializeComponent();
            canvasGlare.Opacity = 0;

        }
        public void Animate()
        {
            glareStoryboard.Begin();
            
        }

        private void GlareControl_Loaded(object sender, RoutedEventArgs e)
        {

            glareStoryboard = (Storyboard)FindResource("Animate");
        }

        private void GlareControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            imgGlare.Width = e.NewSize.Width;
            imgGlare.Height = e.NewSize.Height;
        }
    }
}
