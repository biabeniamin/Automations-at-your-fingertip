using DesktopServerLogical.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical
{
    public static class PinName
    {
        private static String GetPinNameForOutput(DeviceTypes deviceType, int pinNumber)
        {
            if (DeviceTypes.Relay == deviceType && 9 == pinNumber)
            {
                return "Relay";
            }
            else if (DeviceTypes.Network == deviceType)
            {
                switch (pinNumber)
                {
                    case 6:
                        return "Notification 1";
                        break;
                    case 7:
                        return "Notification 2";
                        break;
                    case 8:
                        return "Notification 3";
                        break;
                    case 9:
                        return "Notification 4";
                        break;
                }
            }
            else if (DeviceTypes.Motor == deviceType)
            {
                switch (pinNumber)
                {
                    case 7:
                        return "Mot. 1 c.clockwise";
                        break;
                    case 9:
                        return "Mot. 1 clockwise";
                        break;
                    case 5:
                        return "Mot. 2 clockwise";
                        break;
                    case 6:
                        return "Mot. 2 c.clockwise";
                        break;
                }
            }
            return pinNumber.ToString();
        }
        private static String GetPinNameForInput(DeviceTypes deviceType, int pinNumber)
        {
            if (DeviceTypes.Relay == deviceType && 8 == pinNumber)
            {
                return "Switch";
            }
            else if (DeviceTypes.Network == deviceType)
            {
                switch(pinNumber)
                {
                    case 0:
                        return "Notification Received";
                        break;
                    case 1:
                        return "Button 1";
                        break;
                    case 2:
                        return "Button 2";
                        break;
                    case 3:
                        return "Button 3";
                        break;
                    case 4:
                        return "Face 1";
                        break;
                    case 5:
                        return "Face 2";
                        break;
                }
            }
            else if (DeviceTypes.Keyboard == deviceType)
            {
                return "Pin entered";
            }
            return pinNumber.ToString();
        }
        public static String GetPinName(DeviceTypes deviceType, PinTypes pinType, int pinNumber)
        {
            switch(pinType)
            {
                case PinTypes.Output:
                    return GetPinNameForOutput(deviceType, pinNumber);
                    break;
                case PinTypes.Input:
                    return GetPinNameForInput(deviceType, pinNumber);
                    break;
            }
            return pinNumber.ToString();
        }
    }
}
