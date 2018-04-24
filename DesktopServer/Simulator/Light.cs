using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Simulator
{
    public class Light : Output
    {
        private Image _lightImageControl;

        public Light(Image lightImageControl)
            :base()
        {
            _lightImageControl = lightImageControl;
        }
        public override void UpdateStatus()
        {
            if(true == GetStatus())
            {
                _lightImageControl.Source = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\bulbOn.png"));
            }
            else
            {
                _lightImageControl.Source = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\bulb.png"));
            }
        }
    }
}
