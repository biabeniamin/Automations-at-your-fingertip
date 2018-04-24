using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Simulator
{
    public class Notifications
    {
        private Image _imageControl;
        private NotificationInput _notification1;
        private NotificationInput _notification2;
        private NotificationInput _notification3;

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


        public Notifications(Image imageControl)
        {
            _imageControl = imageControl;
            _notification1 = new NotificationInput(Notification1Triggered);
        }

        private void Notification1Triggered()
        {
            Update(1);
        }

        private void Update(int id)
        {
            _imageControl.Source = new BitmapImage(new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\not{id}.png"));
        }
        
    }
}
