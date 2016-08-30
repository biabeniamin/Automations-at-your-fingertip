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
            SaveActionCommand = new DelegateCommand(SaveAction);
            LoadActionCommand = new DelegateCommand(LoadAction);
            _saves = new Saves();
        }
        private void SaveAction()
        {
            _saves.AddSave(SelectedPin.Actions, "test");
        }
        private void LoadAction()
        {
            SelectedPin.Actions = _saves.LoadActions("test", _controller.Devices);
        }
        private void AddAction()
        {
            SelectedPin.Actions.Add(new RemoteAction(SelectedPin,ActionTypes.TurnOn));
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            //_controller.Devices.Clear();
            Request r = new Request(RequestTypes.ValueChange, 1);
            Device d = new Device(1, DeviceTypes.Relay);
            Pin p = new Pin(d, 8, PinTypes.Output);
            r.Pin = p;
            RemoteAction ra = new RemoteAction(p, ActionTypes.Switch);
            r.PinAction = ra;
            _controller._serial.Write(r);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
