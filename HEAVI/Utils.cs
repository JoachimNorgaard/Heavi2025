using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace HEAVI
{
    class Utils
    {
        public static void AssignShaderEffect(ShadowTextBlock controlToAssignTo)
        {
            string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            BrightContrastEffect b = new BrightContrastEffect(appPath + @"\BrightContrast.ps");
            b.Brightness = 0.5;
            b.PosOfHotSpot = 0.0;
            controlToAssignTo.Effect = b;
            DoubleAnimation da = new DoubleAnimation(-0.1, 1.0, new Duration(TimeSpan.FromSeconds(2.0)), FillBehavior.HoldEnd);
            da.AccelerationRatio = 0.5;
            da.DecelerationRatio = 0.5;
            //da.Completed += new EventHandler(this.TransitionCompleted);
            b.BeginAnimation(BrightContrastEffect.PosOfHotSpotProperty, da);
        }
    }
}
