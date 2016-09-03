using DesktopServerLogical.Enums;
using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical
{
    public class Programmer
    {
        private ObservableCollection<Device> _devices;
        private Serial _serial;
        public Programmer(ObservableCollection<Device> devices,Serial serial)
        {
            _devices = devices;
            _serial = serial;
        }
        private void Initializing()
        {
            Request request = new Request(RequestTypes.Program, 0);
            request.Value1 = _devices.Count;
            _serial.Write(request);
        }
        private void RegisterDevices()
        {
            for (int i = 0; i < _devices.Count; i++)
            {
                Request registerPorts = new Request(RequestTypes.DevicePortsRegister, 0);
                registerPorts.Value1 = _devices[i].Address;
                registerPorts.Value2 = _devices[i].InputPins.Count;
                _serial.Write(registerPorts);
                System.Threading.Thread.Sleep(100);
            }
        }
        private void RegisterPins()
        {
            for (int i = 0; i < _devices.Count; i++)
            {
                for (int j = 0; j < _devices[i].InputPins.Count; j++)
                {
                    Request registerPinActions = new Request(RequestTypes.PortActionsRegister, 0);
                    registerPinActions.Value1 = _devices[i].Address;
                    registerPinActions.Value2 = _devices[i].InputPins[j].PinNumber;
                    registerPinActions.Value3 = _devices[i].InputPins[j].Actions.Count;
                    registerPinActions.Value4 = _devices[i].InputPins[j].Repeats;
                    _serial.Write(registerPinActions);
                    System.Threading.Thread.Sleep(100);
                }
            }
            /*Request registerPinActionsEnded = new Request(RequestTypes.PortActionsRegister, 0);
            _serial.Write(registerPinActionsEnded);*/
        }
        private void RegisterActions()
        {
            for (int i = 0; i < _devices.Count; i++)
            {
                for (int j = 0; j < _devices[i].InputPins.Count; j++)
                {
                    for (int k = 0; k < _devices[i].InputPins[j].Actions.Count; k++)
                    {
                        Request registerAction = new Request(RequestTypes.ActionRegister, 0);
                        registerAction.Value1 = _devices[i].InputPins[j].Actions[k].Pin.Owner.Address;
                        registerAction.Value2 = _devices[i].InputPins[j].Actions[k].Pin.PinNumber;
                        registerAction.Value3 = (int)(_devices[i].InputPins[j].Actions[k].Type);
                        registerAction.Value4 = _devices[i].InputPins[j].Actions[k].Value;
                        _serial.Write(registerAction);
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
        }
        private void SendCommandToPrint()
        {
            Request r = new Request(RequestTypes.PrintEeprom, 0);
            _serial.Write(r);
        }
        public void Program()
        {
            Initializing();
            RegisterDevices();
            RegisterPins();
            RegisterActions();
            SendCommandToPrint();
        }

        
    }
}
