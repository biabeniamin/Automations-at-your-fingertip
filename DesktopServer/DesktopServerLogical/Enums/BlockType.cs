using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical.Enums
{
    public enum BlockType
    {
        PinTriggered = 0,
        For = 1,
        SwitchAction,
        TurnOffAction,
        TurnOnAction,
        DelayAction,
        PositiveAnalogTriggered,
        NegativeAnalogTriggered,
    }
}
