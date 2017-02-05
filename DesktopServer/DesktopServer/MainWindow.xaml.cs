using DesktopServerLogical;
using DesktopServerLogical.Enums;
using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace DesktopServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        private Controller _controller;
        private DelegateCommand _addActionCommand;
        private Pin _selectedPin;
        private Device _selectedDevice;
        private DelegateCommand _saveActionCommand;
        private DelegateCommand _loadActionCommand;
        private Saves _saves;
        private DelegateCommand _loadDevicesCommand;
        private DelegateCommand _programActionsCommand;
        private DelegateCommand _activeLowActionCommand;

        public DelegateCommand AddActiveLowActionCommand
        {
            get { return _activeLowActionCommand; }
            set { _activeLowActionCommand = value; }
        }

        public DelegateCommand ProgramActionsCommand
        {
            get { return _programActionsCommand; }
            set { _programActionsCommand = value; }
        }

        public DelegateCommand LoadDevicesCommand
        {
            get { return _loadDevicesCommand; }
            set { _loadDevicesCommand = value; }
        }

        public DelegateCommand LoadActionCommand
        {
            get { return _loadActionCommand; }
            set { _loadActionCommand = value; }
        }
        public ObservableCollection<Pin> OutputPins
        {
            get
            {
                List<Pin> pins = new List<Pin>();
                for (int i = 0; i < _controller.Devices.Count; i++)
                {
                    pins.AddRange(_controller.Devices[i].OutputPins.ToList<Pin>());
                }
                return new ObservableCollection<Pin>(pins);
            }
        }
        public DelegateCommand SaveActionCommand
        {
            get { return _saveActionCommand; }
            set { _saveActionCommand = value; }
        }

        public Pin SelectedPin
        {
            get { return _selectedPin; }
            set
            {
                _selectedPin = value;
                OnPropertyChanged("SelectedPin");
            }
        }
        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        public DelegateCommand AddActionCommand
        {
            get { return _addActionCommand; }
            set { _addActionCommand = value; }
        }




        public ObservableCollection<Device> Devices
        {
            get { return _controller.Devices; }
        }

        public Array AvailableActions
        {
            get
            {
                return Enum.GetValues(typeof(ActionTypes));
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            _controller = new Controller(App.Current.Dispatcher);
            DataContext = this;
            AddActionCommand = new DelegateCommand(AddAction);
            AddActiveLowActionCommand= new DelegateCommand(AddActiveLowAction);
            SaveActionCommand = new DelegateCommand(SaveAction);
            LoadActionCommand = new DelegateCommand(LoadAction);
            LoadDevicesCommand = new DelegateCommand(LoadDevices);
            ProgramActionsCommand = new DelegateCommand(ProgramActions);
            _saves = new Saves();
            LoadDevices();
        }
        private void ProgramActions()
        {
            _controller.ProgramMaster();
            MessageBox.Show("Programare terminata!");
        }
        private void LoadDevices()
        {
            //_controller.LoadDevices();
            _controller.Devices.Add(new Device(2, DeviceTypes.Relay));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0], 5, PinTypes.Analog));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0],8, PinTypes.Input));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0], 7, PinTypes.Output));
        }
        private void SaveAction()
        {
            if (SelectedPin != null)
                _saves.AddSave(SelectedPin.Actions, "test");
        }
        private void LoadAction()
        {
            if (SelectedPin != null)
            {
                SelectedPin.Actions = _saves.LoadActions("test", _controller.Devices);
                for (int i = 0; i < SelectedPin.Actions.Count; i++)
                {
                    SelectedPin.Actions[i].RemoveAction = _controller.RemoveAction;
                    SelectedPin.Actions[i].OwnerPin = SelectedPin;
                }
            }
        }
        private void AddAction()
        {
            if (SelectedPin != null)
            {
                RemoteAction action = new RemoteAction(SelectedPin, ActionTypes.TurnOn, SelectedPin);
                action.RemoveAction = _controller.RemoveAction;
                SelectedPin.Actions.Add(action);
            }
        }
        private void AddActiveLowAction()
        {
            if (SelectedPin != null)
            {
                RemoteAction action = new RemoteAction(SelectedPin, ActionTypes.TurnOn, SelectedPin);
                action.RemoveAction = _controller.RemoveAction;
                SelectedPin.ActiveLowActions.Add(action);
            }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            //_controller.Devices.Clear();
            /*Request r = new Request(RequestTypes.ValueChange, 1);
            Device d = new Device(1, DeviceTypes.Relay);
            Pin p = new Pin(d, 8, PinTypes.Output);
            r.Pin = p;
            RemoteAction ra = new RemoteAction(p, ActionTypes.Switch,p);
            r.PinAction = ra;
            _controller._serial.Write(r);*/
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        
    }
    public class DoubleApproximator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueAsString = value.ToString();
            if (valueAsString.Length > 1)
                valueAsString = valueAsString.Substring(0, 1);
            return valueAsString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 2.2;
        }
    }
}
