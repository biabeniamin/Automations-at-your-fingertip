using DesktopServerLogical.Enums;
using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical
{
    public class Serial
    {
        private SerialPort _port;
        private Action<Response> _receivedAction;
        //private int _lastBytes[]
        public Serial(Action<Response> receivedAction)
        {
            _receivedAction = receivedAction;
            _port = new SerialPort("COM6");
            _port.DtrEnable = true;
            _port.RtsEnable = true;
            _port.Open();
            _port.DataReceived += _port_DataReceived;
            //Write(new Request(RequestTypes.Register, 1));--led panel
            //Write(new Request(RequestTypes.Register, 2));
        }
        public void Write(Request request)
        {
            if (request.Type == RequestTypes.ValueChange)
            {
                if(request.PinAction.Type==ActionTypes.Delay)
                {
                    System.Threading.Thread.Sleep(request.PinAction.Value);
                }
                else
                {
                    _port.Write(request.ToAddress.ToString());
                    _port.Write(((int)request.Type).ToString());
                    _port.Write(request.Pin.PinNumber.ToString());
                    _port.Write(((int)request.PinAction.Type).ToString());
                    _port.Write(0.ToString());
                }
            }
            else if(request.Type==RequestTypes.Register)
            {
                _port.Write(request.ToAddress.ToString());
                _port.Write(((int)request.Type).ToString());
                _port.Write("000");
            }
        }
        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int[] dataReceived=new int[5];
            while (_port.BytesToRead>0)
            {
                string line = _port.ReadLine();
                for (int i = 0; i < 5; i++)
                {
                    dataReceived[i] = line[i] - 48;
                }
                ResponseTypes ResponseType = (ResponseTypes)Enum.Parse(typeof(ResponseTypes), dataReceived[1].ToString());
                Response response = new Response(dataReceived[0], ResponseType);
                response.FromAddress = dataReceived[2];
                if (ResponseType == ResponseTypes.DeviceRegister)
                {
                    response.DeviceType = (DeviceTypes)Enum.Parse(typeof(DeviceTypes), dataReceived[3].ToString());
                }
                else if (ResponseType == ResponseTypes.PinRegister)
                {
                    response.PinNumber = dataReceived[3];
                    response.PinType = (PinTypes)Enum.Parse(typeof(PinTypes), dataReceived[4].ToString());
                }
                else if (ResponseType == ResponseTypes.ValueChanged)
                {
                    response.PinNumber = dataReceived[3];
                    response.Value = dataReceived[4];
                }
                _receivedAction(response);
            }
        }
    }
}
