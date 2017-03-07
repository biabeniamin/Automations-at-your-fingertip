using DesktopServerLogical.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DesktopServerLogical.Models
{
    public class Pin:INotifyPropertyChanged
    {
        private Device _owner;
        private int _pinNumber;
        private PinTypes _type;
        private ObservableCollection<RemoteAction> _actions;
        private ObservableCollection<RemoteAction> _activeLowActions;
        private int _repeats=1;
        private int _triggeredValue=5;
        public int TriggeredValue
        {
            get { return _triggeredValue; }
            set { _triggeredValue = value; }
        }
        public int Repeats
        {
            get { return _repeats; }
            set { _repeats = value; }
        }
        public bool AllowAction
        {
            get { return PinTypes.Input==_type; }
        }
        public bool AllowTriggeredValue
        {
            get
            {
                return PinTypes.Analog == _type;
            }
        }
        public ObservableCollection<RemoteAction> Actions
        {
            get { return _actions; }
            set
            {
                _actions = value;
            }
        }
        public ObservableCollection<RemoteAction> ActiveLowActions
        {
            get { return _activeLowActions; }
            set
            {
                _activeLowActions = value;
            }
        }
        public PinTypes Type
        {
            get { return _type; }
        }
        public int PinNumber
        {
            get { return _pinNumber; }
        }
        public Device Owner
        {
            get { return _owner; }
        }
        public Pin(Device owner,int pinNumber,PinTypes type)
        {
            _owner = owner;
            _pinNumber = pinNumber;
            _type = type;
            _actions = new ObservableCollection<RemoteAction>();
            _activeLowActions = new ObservableCollection<RemoteAction>();
        }
        public void ClearActions()
        {
            _actions.Clear();
            _activeLowActions.Clear();
        }
        public override string ToString()
        {
            return $"{_owner.Address}-{PinNumber}";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
