using DesktopServerLogical.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
    public class Pin
    {
        private PinTypes _type;

        public PinTypes Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public Pin(PinTypes type)
        {
            _type = type;
        }
    }
}
