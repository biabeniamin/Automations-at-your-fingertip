using DesktopServerLogical.Enums;
using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical
{
    public class Saves
    {
        private DatabaseOperations _dbOper;
        public Saves()
        {
            _dbOper = new DatabaseOperations();
        }
        private void AddActions(ObservableCollection<RemoteAction> actions, int pinId)
        {
            foreach (RemoteAction action in actions)
            {
                _dbOper.ExecuteQuery($@"insert into actions(pinId,Type,AValue,DeviceNumber,PinNumber) 
                    values({pinId},{(int)action.Type},{action.Value},{action.Pin.Owner.Address},{action.Pin.PinNumber})");
                //int actionid = _dbOper.ReadOneValue<int>($"select top 1 PinId from pins where Deviceid={deviceid} and PinNumber={pin.PinNumber} order by deviceid desc");
            }
        }
        private void AddPins(ObservableCollection<Pin> pins, int deviceid)
        {
            foreach (Pin pin in pins)
            {
                _dbOper.ExecuteQuery($"insert into pins(deviceId,pinNumber,Repeats,TriggeredValue) values({deviceid},{pin.PinNumber},{pin.Repeats},{pin.TriggeredValue})");
                int pinId = _dbOper.ReadOneValue<int>($"select top 1 PinId from pins where Deviceid={deviceid} and PinNumber={pin.PinNumber} order by deviceid desc");
                AddActions(pin.Actions, pinId);
            }
        }
        private void AddDevices(ObservableCollection<Device> devices, int saveid)
        {
            foreach (Device device in devices)
            {
                _dbOper.ExecuteQuery($"insert into device(saveId,DeviceNumber) values({saveid},{device.Address})");
                int deviceId = _dbOper.ReadOneValue<int>($"select top 1 DeviceId from device where DeviceNumber={device.Address} and saveId={saveid} order by deviceid desc");
                AddPins(device.Pins, deviceId);
            }
        }
        public void AddSave(ObservableCollection<Device> devices, string name)
        {
            _dbOper.ExecuteQuery($"insert into saves(nume) values('{name}')");
            int saveId = _dbOper.ReadOneValue<int>($"select top 1 id from saves where nume='{name}' order by id desc");
            AddDevices(devices, saveId);
            /*for (int i = 0; i < actions.Count; i++)
            {
                _dbOper.ExecuteQuery($"insert into actions(saveId,deviceId,pinId,Type,AValue) values({saveId},{actions[i].Pin.Owner.Address},{actions[i].Pin.PinNumber},{(int)actions[i].Type},{actions[i].Value})");
            }*/
        }
        public ObservableCollection<RemoteAction> LoadActions(string name,ObservableCollection<Device> devices)
        {
            ObservableCollection<RemoteAction> actions = new ObservableCollection<RemoteAction>();
            OleDbDataReader reader=_dbOper.GetReader($"select deviceId,pinId,Type,AValue from actions where saveId={_dbOper.ReadOneValue<int>($"select id from saves where nume='{name}'")}");
            while(reader.Read())
            {
                int deviceId = Convert.ToInt32(reader[0]);
                int pinId = Convert.ToInt32(reader[1]);
                Pin pin = Helpers.GetPin(Helpers.GetDevice(devices, deviceId), pinId);
                RemoteAction action = new RemoteAction(pin,(ActionTypes)Enum.Parse(typeof(ActionTypes),reader[2].ToString()),pin);
                action.Value = Convert.ToInt32(reader[3]);
                actions.Add(action);
            }
            return actions;
        }
    }
}
