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
        private int _value=0;
        private DelegateCommand _removeCommand;
        private Action<Pin, RemoteAction> _removeAction;
        private Pin _ownerPin;

        public Pin OwnerPin
        {
            get { return _ownerPin; }
            set { _ownerPin = value; }
        }

        public Action<Pin, RemoteAction> RemoveAction
        {
            get
            {
                return _removeAction;
            }
            set
            {
                _removeAction = value;
            }
        }
        public DelegateCommand RemoveCommand
        {
            get { return _removeCommand; }
            set { _removeCommand = value; }
        }

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
        public RemoteAction(Pin pin,ActionTypes type,Pin ownerPin)
        {
            _pin = pin;
            _type = type;
            _ownerPin = ownerPin;
            _removeCommand = new DelegateCommand(Delete);
        }
        private void Delete()
        {
            _removeAction(_ownerPin, this);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
