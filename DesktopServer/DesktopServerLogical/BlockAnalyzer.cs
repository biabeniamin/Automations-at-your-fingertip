using DesktopServerLogical.Enums;
using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical
{
    public static class BlockAnalyzer
    {
        private static void AnalyzeBlock(Pin ownerPin,BlockControl ownerBlock,BlockControl blockControl,ref int repetitions)
        {
            RemoteAction action = null;
            Pin pin = null;
            repetitions = 1;
            int value;
            try
            {
                switch (blockControl.Type)
                {
                    case BlockType.PositiveAnalogTriggered:
                    case BlockType.NegativeAnalogTriggered:
                        ownerPin.TriggeredValue = Convert.ToInt32(blockControl.GetSecondValue());
                        break;
                    case BlockType.For:
                        if (blockControl.Parent == ownerBlock)
                            ownerPin.Repeats = Convert.ToInt32(blockControl.GetValue());
                        else
                            repetitions = Convert.ToInt32(blockControl.GetValue());
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
                        if(ownerBlock.Type == BlockType.NegativeAnalogTriggered)
                            ownerPin.ActiveLowActions.Add(action);
                        else
                            ownerPin.Actions.Add(action);
                        break;
                    /*case BlockType.If:
                        if (blockControl.Childs.Count() < 1)
                            return;
                        BlockControl
                        pin = (Pin)blockControl.GetValue();
                        action = new RemoteAction(pin, ActionTypes., ownerPin);
                        if (ownerBlock.Type == BlockType.NegativeAnalogTriggered)
                            ownerPin.ActiveLowActions.Add(action);
                        else
                            ownerPin.Actions.Add(action);
                        break;*/
                }
            }
            catch(Exception ee)
            {
                Debug.WriteLine($"Was an error when tryed to analyze this block{ee.Message}");
            }
            //return action;
        }
        public static void Analyze(BlockControl blockControl)
        {
            try
            {
                Pin pin = (Pin)blockControl.GetValue();
                Analyze(blockControl, blockControl, pin,1);
            }
            catch(Exception ee)
            {
                Debug.WriteLine($"Error when trying to analyze a parent block.{ee.Message}");
            }
        }
        private static void Analyze(BlockControl ownerBlock,BlockControl blockControl,Pin pin,int repetitions)
        {
            for (int j = 0; j < repetitions; j++)
            {
                int blockRepetitions = 1;
                AnalyzeBlock(pin, ownerBlock, blockControl,ref repetitions);
                for (int i = 0; i < blockControl.Childs.Count; i++)
                {
                    Analyze(ownerBlock, blockControl.Childs[i], pin, blockRepetitions);
                }
            }
        }
    }
}
