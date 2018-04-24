using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
    public class Input : Pin
    {
        public Input()
            : base(DesktopServerLogical.Enums.PinTypes.Input)
        {

        }
    }
}
