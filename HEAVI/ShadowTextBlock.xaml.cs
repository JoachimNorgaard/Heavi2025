using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HEAVI
{
	/// <summary>
	/// Interaction logic for ShadowTextBlock.xaml
	/// </summary>
	public partial class ShadowTextBlock : UserControl
	{
        string text;
        int shadowOffset;
		public ShadowTextBlock()
		{
			this.InitializeComponent();
            ShadowOffset = 2;
           
		}
        

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                textBlockMain.Text = text;
                textBlockMainShadow.Text = text;
            }
        }
        public int ShadowOffset
        {
            get
            {
                return shadowOffset;
            }
            set
            {
                shadowOffset = value;
                textBlockMain.Margin = new Thickness(shadowOffset+1, shadowOffset, 0, 0);
            }
        }
	}
}