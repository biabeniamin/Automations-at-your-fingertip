using DesktopServerLogical.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical.Models
{
    public class Device
    {
        private int _address;
        private DeviceTypes _type;
        private ObservableCollection<Pin> _pins;
        public ObservableCollection<Pin> OutputPins
        {
            get
            {
                return new ObservableCollection<Pin>(_pins.Where<Pin>((p) =>
                {
                    if (p.Type == PinTypes.Output)
                        return true;
                    return false;
                }).ToList<Pin>());
            }
        }
        public ObservableCollection<Pin> InputPins
        {
            get
            {
                return new ObservableCollection<Pin>(_pins.Where<Pin>((p) =>
                {
                    if (p.Type == PinTypes.Input || p.Type == PinTypes.Analog)
                        return true;
                    return false;
                }).ToList<Pin>());
            }
        }
        public ObservableCollection<Pin> Pins
        {
            get { return _pins; }
            set { _pins = value; }
        }

        public DeviceTypes Type
        {
            get { return _type; }
        }

        public int Address
        {
            get { return _address; }
        }
        public Device(int address,DeviceTypes type)
        {
            _address = address;
            _type = type;
            _pins = new ObservableCollection<Pin>();
        }
    }
}
