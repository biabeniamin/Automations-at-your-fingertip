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
        private int _value1=0;
        private int _value2=0;
        private int _value3=0;
        private int _value4=0;
        private int _value5=0;

        public int Value5
        {
            get { return _value5; }
            set { _value5 = value; }
        }

        public int Value4
        {
            get { return _value4; }
            set { _value4 = value; }
        }

        public int Value3
        {
            get { return _value3; }
            set { _value3 = value; }
        }

        public int Value2
        {
            get { return _value2; }
            set { _value2 = value; }
        }

        public int Value1
        {
            get { return _value1; }
            set { _value1 = value; }
        }

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
