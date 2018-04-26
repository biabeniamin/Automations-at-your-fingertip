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
    public class Notifications : INotifyPropertyChanged
    {
        private NotificationInput _notification1;
        private NotificationInput _notification2;
        private NotificationInput _notification3;

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

        public NotificationInput Notification3
        {
            get { return _notification3; }
            set { _notification3 = value; }
        }

        public NotificationInput Notification2
        {
            get { return _notification2; }
            set { _notification2 = value; }
        }

        public NotificationInput Notification1
        {
            get { return _notification1; }
            set { _notification1 = value; }
        }


        public Notifications()
        {
            _notification1 = new NotificationInput(Notification1Triggered);
            _notification2 = new NotificationInput(Notification2Triggered);
            _notification3 = new NotificationInput(Notification3Triggered);
            Update(0);
        }

        private void Notification1Triggered()
        {
            Update(1);
        }

        private void Notification2Triggered()
        {
            Update(2);
        }

        private void Notification3Triggered()
        {
            Update(3);
        }

        private void Update(int id)
        {
            ImageSource = $"{System.IO.Directory.GetCurrentDirectory()}\\Images\\not{id}.png";
            //_imageControl.Source = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\not{id}.png"));
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
