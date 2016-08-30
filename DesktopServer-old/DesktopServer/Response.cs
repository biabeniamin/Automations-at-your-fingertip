using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical
{
    public class Response
    {
        private int _toAddress;
        private TypesOfResponses _typeOfResponse;
        private TypesOfDevice _typeOfDevice;
        private int _fromAddress;
        private int _firstByte;
        private int _secondByte;

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
        public TypesOfDevice TypeOfDevice
        {
            get { return _typeOfDevice; }
            set { _typeOfDevice = value; }
        }

        public TypesOfResponses TypeOfResponse
        {
            get { return _typeOfResponse; }
            set { _typeOfResponse = value; }
        }

        public int ToAddress
        {
            get { return _toAddress; }
            set { _toAddress = value; }
        }
        public Response(int toAddress, TypesOfResponses typeOfResponse)
        {
            _toAddress = toAddress;
            _typeOfResponse = typeOfResponse;
        }
        public Response(int toAddress, TypesOfResponses typeOfResponse, int firstByte, int secondByte)
        {
            _toAddress = toAddress;
            _typeOfResponse = typeOfResponse;
            _firstByte = firstByte;
            _secondByte = secondByte;
        }
    }
}
