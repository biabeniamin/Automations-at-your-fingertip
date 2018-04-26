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
    public class Door : Output, INotifyPropertyChanged
    {

        private string _imageSource;

        public string ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                OnPropertyChanged("ImageSource");
            }
        }

        public Door()
            : base()
        {
        }
        public override void UpdateStatus()
        {
            if (true == GetStatus())
            {
                ImageSource = $"{System.IO.Directory.GetCurrentDirectory()}\\Images\\open.png";
            }
            else
            {
                ImageSource = $"{System.IO.Directory.GetCurrentDirectory()}\\Images\\close.png";
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
