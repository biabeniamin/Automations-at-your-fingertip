using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical
{
    public static class Helpers
    {
        public static Pin GetPin(Device owner, int pinNumber)
        {
            Pin pin = null;
            try
            {
                pin = owner.Pins.Where<Pin>((p) =>
                {
                    if (p.PinNumber == pinNumber)
                        return true;
                    else
                        return false;
                }).ToList<Pin>()[0];
            }
            catch
            {
                //
            }
            return pin;
        }
        public static Device GetDevice(ObservableCollection<Device> devices,int address)
        {
            Device device = null;
            try
            {
                device = devices.Where<Device>((dev) =>
                {
                    if (dev.Address == address)
                        return true;
                    else
                        return false;
                }).ToList<Device>()[0];
            }
            catch
            {
                //
            }
            return device;
        }
    }
}
