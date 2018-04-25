using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Simulator
{
    public class Switch : Input
    {
        private Button _buttonControl;
        private bool _status;

        public Switch(Button buttonControl)
            : base()
        {
            _buttonControl = buttonControl;
            Update();
        }
        private void Update()
        {
            BitmapImage bitimg = new BitmapImage();
            bitimg.BeginInit();

            if(_status)
            {
                bitimg.UriSource = new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\switch.png");
            }
            else
            {
                bitimg.UriSource = new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\switchOff.png");
            }
            
            bitimg.EndInit();

            Image img = new Image();
            img.Stretch = Stretch.Uniform;
            img.Source = bitimg;

            // Set Button.Content
            _buttonControl.Content = img;

            // Set Button.Background
            _buttonControl.Background = new ImageBrush(bitimg);
            /*if (true == GetStatus())
            {
                _buttonControl.Source = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\open.png"));
            }
            else
            {
                _buttonControl.Source = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\close.png"));
            }*/
        }

        public void SwitchButton()
        {
            _status = !_status;
            Update();
        }
    }
}
