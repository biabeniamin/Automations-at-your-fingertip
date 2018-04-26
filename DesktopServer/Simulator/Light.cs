using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Simulator
{
    public class Light : Output, INotifyPropertyChanged
    {
        private Image _lightImageControl;

        private string _imageSource;

        public string ImageSource
        {
            get { return _imageSource; }
            set { _imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }


        public Light(Image lightImageControl)
            :base()
        {
            _lightImageControl = lightImageControl;
            
        }
        public override void UpdateStatus()
        {

            if(true == GetStatus())
            {
                ImageSource = $"{System.IO.Directory.GetCurrentDirectory()}\\Images\\bulbOn.png";
                //_lightImageControl.Source = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\bulbOn.png"));
            }
            else
            {
                ImageSource = $"{System.IO.Directory.GetCurrentDirectory()}\\Images\\bulb.png";
                //_lightImageControl.Source = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\bulb.png"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
