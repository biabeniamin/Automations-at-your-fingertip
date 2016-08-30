using DesktopServerLogical.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical.Models
{
    public class RemoteAction:INotifyPropertyChanged
    {
        private ActionTypes _type;
        private Pin _pin;
        private int _value;

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Pin Pin
        {
            get { return _pin; }
            set { _pin = value; }
        }

        public ActionTypes Type
        {
            get { return _type; }
            set
            {
                _type = value;
            }
        }
        public RemoteAction(Pin pin,ActionTypes type)
        {
            _pin = pin;
            _type = type;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
