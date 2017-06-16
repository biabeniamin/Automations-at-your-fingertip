using DesktopServerLogical.Enums;
using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.OleDb;
using System.Data.SqlClient;
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
        private void AddActions(ObservableCollection<RemoteAction> actions, int pinId,int isNegativeTriggered)
        {
            foreach (RemoteAction action in actions)
            {
                _dbOper.ExecuteQuery($@"insert into actions(pinId,Type,Value,DeviceNumber,PinNumber,IsNegativeTriggered) 
                    values({pinId},{(int)action.Type},{action.Value},{action.Pin.Owner.Address},{action.Pin.PinNumber},{isNegativeTriggered})");
                //int actionid = _dbOper.ReadOneValue<int>($"select top 1 PinId from pins where Deviceid={deviceid} and PinNumber={pin.PinNumber} order by deviceid desc");
            }
        }
        private void AddPins(ObservableCollection<Pin> pins, int deviceid)
        {
            foreach (Pin pin in pins)
            {
                _dbOper.ExecuteQuery($"insert into pins(deviceId,pinNumber,Repeats,TriggeredValue) values({deviceid},{pin.PinNumber},{pin.Repeats},{pin.TriggeredValue})");
                int pinId = _dbOper.ReadOneValue<int>($"select top 1 PinId from pins where Deviceid={deviceid} and PinNumber={pin.PinNumber} order by deviceid desc");
                AddActions(pin.Actions, pinId, 0);
                AddActions(pin.ActiveLowActions, pinId, 1);
            }
        }
        private void AddDevices(ObservableCollection<Device> devices, int saveid)
        {
            foreach (Device device in devices)
            {
                _dbOper.ExecuteQuery($"insert into devices(saveId,DeviceNumber) values({saveid},{device.Address})");
                int deviceId = _dbOper.ReadOneValue<int>($"select top 1 DeviceId from devices where DeviceNumber={device.Address} and saveId={saveid} order by deviceid desc");
                AddPins(device.Pins, deviceId);
            }
        }
        public void AddSave(ObservableCollection<Device> devices, string name)
        {
            _dbOper.ExecuteQuery($"insert into saves(name) values('{name}')");
            int saveId = _dbOper.ReadOneValue<int>($"select top 1 id from saves where name='{name}' order by id desc");
            AddDevices(devices, saveId);
            /*for (int i = 0; i < actions.Count; i++)
            {
                _dbOper.ExecuteQuery($"insert into actions(saveId,deviceId,pinId,Type,AValue) values({saveId},{actions[i].Pin.Owner.Address},{actions[i].Pin.PinNumber},{(int)actions[i].Type},{actions[i].Value})");
            }*/
        }
        private void ClearActions(ref ObservableCollection<Device> devices)
        {
            foreach(Device device in devices)
            {
                foreach(Pin pin in device.Pins)
                {
                    pin.Actions.Clear();
                    pin.ActiveLowActions.Clear();
                }
            }
        }
        public ObservableCollection<string> GetActions()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();
            ObservableCollection<RemoteAction> actions = new ObservableCollection<RemoteAction>();
            SqlDataReader reader = _dbOper.GetReader($@"SELECT Name from Saves");
            while (reader.Read())
            {
                list.Add(reader[0].ToString());
            }
            reader.Dispose();
            return list;
        }
        public ObservableCollection<Device> LoadActions(string name,ObservableCollection<Device> devices)
        {
            ObservableCollection<RemoteAction> actions = new ObservableCollection<RemoteAction>();
            ClearActions(ref devices);
            SqlDataReader reader=_dbOper.GetReader($@"SELECT devices.DeviceNumber,Pins.PinNumber, Pins.TriggeredValue, Actions.DeviceNumber, Actions.PinNumber, Actions.Type, Actions.Value,Actions.IsNegativeTriggered
FROM Actions
INNER JOIN Pins ON Pins.PinId=Actions.PinId
INNER JOIN Devices ON Devices.Deviceid=Pins.DeviceId
INNER JOIN Saves ON Saves.Id=Devices.SaveId
WHERE Saves.Id IN(SELECT TOP 1 Id FROM Saves ORDER BY Id DESC)");
            while (reader.Read())
            {
                int ownerDeviceId = Convert.ToInt32(reader[0]);
                int ownerPinId = Convert.ToInt32(reader[1]);
                int triggeredValue = Convert.ToInt32(reader[2]);
                int deviceId = Convert.ToInt32(reader[3]);
                int pinId = Convert.ToInt32(reader[4]);
                string type = reader[5].ToString();
                int value = Convert.ToInt32(reader[6]);
                int isNegativeTriggered = Convert.ToInt32(reader[7]);
                Pin pin = Helpers.GetPin(Helpers.GetDevice(devices, deviceId), pinId);
                Pin ownerPin = Helpers.GetPin(Helpers.GetDevice(devices, ownerDeviceId), ownerPinId);
                RemoteAction action = new RemoteAction(pin, (ActionTypes)Enum.Parse(typeof(ActionTypes), type), pin);
                action.Value = value;
                if (isNegativeTriggered == 0)
                {
                    ownerPin.Actions.Add(action);
                }
                else
                {
                    ownerPin.ActiveLowActions.Add(action);
                }
            }
            reader.Dispose();
            return devices;
        }
    }
}
