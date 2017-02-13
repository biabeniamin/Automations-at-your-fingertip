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
        private static void AnalyzeBlock(Pin ownerPin,BlockControl ownerBlock,BlockControl blockControl)
        {
            RemoteAction action = null;
            Pin pin = null;
            int value;
            switch (blockControl.Type)
            {
                case BlockType.PositiveAnalogTriggered:
                case BlockType.NegativeAnalogTriggered:
                    ownerPin.TriggeredValue = Convert.ToInt32(blockControl.GetSecondValue());
                    break;
                case BlockType.For:
                    pin.Repeats = Convert.ToInt32(blockControl.GetValue());
                    break;
                case BlockType.SwitchAction:
                    pin = (Pin)blockControl.GetValue();
                    action = new RemoteAction(pin, ActionTypes.Switch, ownerPin);
                    if (ownerBlock.Type == BlockType.NegativeAnalogTriggered)
                        ownerPin.ActiveLowActions.Add(action);
                    else
                        ownerPin.Actions.Add(action);
                    break;
                case BlockType.TurnOnAction:
                    pin = (Pin)blockControl.GetValue();
                    action = new RemoteAction(pin, ActionTypes.TurnOn, ownerPin);
                    if (ownerBlock.Type == BlockType.NegativeAnalogTriggered)
                        ownerPin.ActiveLowActions.Add(action);
                    else
                        ownerPin.Actions.Add(action);
                    break;
                case BlockType.TurnOffAction:
                    pin = (Pin)blockControl.GetValue();
                    action = new RemoteAction(pin, ActionTypes.TurnOff, ownerPin);
                    if (ownerBlock.Type == BlockType.NegativeAnalogTriggered)
                        ownerPin.ActiveLowActions.Add(action);
                    else
                        ownerPin.Actions.Add(action);
                    break;
                case BlockType.DelayAction:
                    value = Convert.ToInt32(blockControl.GetValue());
                    action = new RemoteAction(ownerPin, ActionTypes.Delay, null);
                    action.Value = value;
                    pin.Actions.Add(action);
                    break;
            }
            //return action;
        }
        public static void Analyze(BlockControl blockControl)
        {
            Pin pin = (Pin)blockControl.GetValue();
            AnalyzeBlock(pin,null, blockControl);
            for (int i = 0; i < blockControl.Childs.Count; i++)
            {
                AnalyzeBlock(pin,blockControl, blockControl.Childs[i]);
            }
        }
    }
}
