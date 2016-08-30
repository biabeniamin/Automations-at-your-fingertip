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
        public Serial(Action<Response> receivedAction)
        {
            _receivedAction = receivedAction;
            _port = new SerialPort("COM6");
            _port.Open();
            _port.DataReceived += _port_DataReceived;
        }
        public void Write(Response response)
        {
            _port.Write(response.ToAddress.ToString());
            _port.Write(((int)response.TypeOfResponse).ToString());
            if (response.TypeOfResponse == TypesOfResponses.Register)
            {
                _port.Write(response.FromAddress.ToString());
                _port.Write(((int)response.TypeOfDevice).ToString());
            }
        }
        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int[] dataReceived=new int[4];
            string line = _port.ReadLine();
            for (int i = 0; i < 4; i++)
            {
                dataReceived[i] = line[i] - 48;
            }
            TypesOfResponses typeOfResponse = (TypesOfResponses)Enum.Parse(typeof(TypesOfResponses), dataReceived[1].ToString());
            Response response = new Response(dataReceived[0], typeOfResponse);
            if(typeOfResponse==TypesOfResponses.Register)
            {
                response.FromAddress = dataReceived[2];
                response.TypeOfDevice = (TypesOfDevice)Enum.Parse(typeof(TypesOfDevice), dataReceived[3].ToString());
            }
            _receivedAction(response);
        }
    }
}
