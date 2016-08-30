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
using System.Windows.Threading;

namespace DesktopServerLogical
{
    public class Controller:INotifyPropertyChanged
    {
        public Serial _serial;
        private Dispatcher _dispatcher;
        private ObservableCollection<Device> _devices;
        public ObservableCollection<Device> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        public Controller(Dispatcher dispatcher)
        {
            _serial = new Serial(ResponseReceived);
            _devices = new ObservableCollection<Device>();
            _dispatcher = dispatcher;
            //AddNetworkDevice();
        }

        private void AddNetworkDevice()
        {
            Device networkDev = new Device(0, DeviceTypes.Network);
            for (int i = 0; i < 10; i++)
            {
                networkDev.Pins.Add(new Pin(networkDev, i, PinTypes.Input));
            }
            _devices.Add(networkDev);
        }

        private void ResponseReceived(Response response)
        {
            if (response.ResponseType == ResponseTypes.DeviceRegister)
                NewDevice(response);
            if (response.ResponseType == ResponseTypes.PinRegister)
                NewPin(response);
            if (response.ResponseType == ResponseTypes.ValueChanged)
                PinValueChanged(response);
            if (response.ResponseType == ResponseTypes.TransmissionEnded)
                LoadNextDevice(response);
        }
        private void PinValueChanged(Response response)
        {
            Pin pin = GetPin(GetDevice(response.FromAddress), response.PinNumber);
            for (int j = 0; j < pin.Repeats; j++)
            {
                for (int i = 0; i < pin.Actions.Count; i++)
                {
                    Request request = new Request(RequestTypes.ValueChange, pin.Actions[i].Pin.Owner.Address);
                    request.Pin = pin.Actions[i].Pin;
                    request.PinAction = pin.Actions[i];
                    _serial.Write(request);
                }
            }
        }
        private void NewDevice(Response response)
        {
            Device newDevice = new Device(response.FromAddress, response.DeviceType);
            if (GetDevice(response.FromAddress) == null)
            {
                _dispatcher.Invoke(() =>
                {
                    _devices.Add(newDevice);
                });
            }
            
        }
        private void NewPin(Response response)
        {
            Device ownerDevice = GetDevice(response.FromAddress);
            if (ownerDevice != null && GetPin(ownerDevice,response.PinNumber)==null)
            {
                Pin pin = new Pin(ownerDevice, response.PinNumber, response.PinType);
                _dispatcher.Invoke(() =>
                {
                    for (int i = 0; i < _devices.Count; i++)
                    {
                        if (_devices[i] == ownerDevice)
                            _devices[i].Pins.Add(pin);
                    }
                });
            }
        }
        private void LoadNextDevice(Response response)
        {
            if(response.FromAddress<4)
                LoadDevice(response.FromAddress + 1);
        }
        public void LoadDevices()
        {
            LoadDevice(1);
        }
        private void LoadDevice(int id)
        {
            _serial.Write(new Request(RequestTypes.Register, id));
        }
        public  Pin GetPin(Device owner,int pinNumber)
        {
            return Helpers.GetPin(owner, pinNumber);
        }
        private Device GetDevice(int address)
        {
            return Helpers.GetDevice(_devices, address);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
