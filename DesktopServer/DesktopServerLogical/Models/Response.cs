using DesktopServerLogical.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical.Models
{
    public class Response
    {
        private int _toAddress;
        private ResponseTypes _responseType;
        private DeviceTypes _typeOfDevice;
        private int _fromAddress;
        private int _firstByte;
        private int _secondByte;
        private int _fromPin;
        private int _toPin;
        private int _pinNumber;
        private PinTypes _pinType;
        private int _value;

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public PinTypes PinType
        {
            get { return _pinType; }
            set { _pinType = value; }
        }

        public int PinNumber
        {
            get { return _pinNumber; }
            set { _pinNumber = value; }
        }

        public int SecondByte
        {
            get { return _secondByte; }
            set { _secondByte = value; }
        }

        public int FirstByte
        {
            get { return _firstByte; }
            set { _firstByte = value; }
        }

        public int FromAddress
        {
            get { return _fromAddress; }
            set { _fromAddress = value; }
        }
        public DeviceTypes DeviceType
        {
            get { return _typeOfDevice; }
            set { _typeOfDevice = value; }
        }

        public ResponseTypes ResponseType
        {
            get { return _responseType; }
            set { _responseType = value; }
        }

        public int ToAddress
        {
            get { return _toAddress; }
            set { _toAddress = value; }
        }
        public Response(int toAddress, ResponseTypes typeOfResponse)
        {
            _toAddress = toAddress;
            _responseType = typeOfResponse;
        }
        public Response(int toAddress, ResponseTypes typeOfResponse, int firstByte, int secondByte)
        {
            _toAddress = toAddress;
            _responseType = typeOfResponse;
            _firstByte = firstByte;
            _secondByte = secondByte;
        }
    }
}
