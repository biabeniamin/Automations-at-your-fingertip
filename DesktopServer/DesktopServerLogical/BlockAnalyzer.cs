using DesktopServerLogical.Enums;
using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical
{
    public static class BlockAnalyzer
    {
        private static void AnalyzeBlock(Pin ownerPin,BlockControl blockControl)
        {
            RemoteAction action = null;
            Pin pin = null;
            switch (blockControl.Type)
            {
                case BlockType.For:
                    pin.Repeats = Convert.ToInt32(blockControl.GetValue());
                    break;
                case BlockType.SwitchAction:
                    pin = (Pin)blockControl.GetValue();
                    action = new RemoteAction(ownerPin, ActionTypes.Switch, pin);
                    pin.Actions.Add(action);
                    break;
            }
            //return action;
        }
        public static void Analyze(BlockControl blockControl)
        {
            Pin pin = (Pin)blockControl.GetValue();
            AnalyzeBlock(pin, blockControl);
            for (int i = 0; i < blockControl.Childs.Count; i++)
            {
                AnalyzeBlock(pin, blockControl.Childs[i]);
            }
        }
    }
}
