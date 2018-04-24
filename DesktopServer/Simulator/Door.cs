using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Simulator
{
    public class Door : Output
    {
        private Image _imageControl;

        public Door(Image imageControl)
            : base()
        {
            _imageControl = imageControl;
        }
        public override void UpdateStatus()
        {
            if (true == GetStatus())
            {
                _imageControl.Source = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\open.png"));
            }
            else
            {
                _imageControl.Source = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\close.png"));
            }
        }
    }
}
