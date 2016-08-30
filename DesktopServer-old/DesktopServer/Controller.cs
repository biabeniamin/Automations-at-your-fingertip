using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical
{
    public class Controller
    {
        private Serial _serial;
        private List<Device> _devices;
        public Controller()
        {
            _serial = new Serial(ResponseReceived);
            _devices = new List<Device>();
        }
        private void ResponseReceived(Response response)
        {
            if (response.TypeOfResponse == TypesOfResponses.Register)
                NewDevice(response);
        }
        private void NewDevice(Response response)
        {
            Device newDevice = new Device(response.FromAddress, response.TypeOfDevice);
            _devices.Add(newDevice);
        }
    }
}
