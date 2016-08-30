using DesktopServerLogical.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical.Models
{
    public class Request
    {
        private RequestTypes _type;
        private int _toAddress;
        private Pin _pin;
        private RemoteAction _pinAction;

        public RemoteAction PinAction
        {
            get { return _pinAction; }
            set { _pinAction = value; }
        }

        public Pin Pin
        {
            get { return _pin; }
            set { _pin = value; }
        }

        public int ToAddress
        {
            get { return _toAddress; }
            set { _toAddress = value; }
        }

        public RequestTypes Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public Request(RequestTypes type,int toAddress)
        {
            _type = type;
            _toAddress = toAddress;
        }
    }
}
