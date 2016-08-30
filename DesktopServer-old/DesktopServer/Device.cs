using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical
{
    public class Device
    {
        private int _address;
        private TypesOfDevice _type;

        public TypesOfDevice Type
        {
            get { return _type; }
        }

        public int Address
        {
            get { return _address; }
        }
        public Device(int address,TypesOfDevice type)
        {
            _address = address;
            _type = type;
        }
    }
}
