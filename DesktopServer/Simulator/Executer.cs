using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
    public class Executer
    {
        //1 to one between simulator and data models
        Dictionary<Pin, DesktopServerLogical.Models.Pin> _mapSimToMod;
        Dictionary<DesktopServerLogical.Models.Pin, Pin> _mapModToSim;

        public void AddSimulatedPin(Pin simulatedPin, DesktopServerLogical.Models.Pin pin)
        {
            _mapSimToMod[simulatedPin] = pin;
            _mapModToSim[pin] = simulatedPin;
        }

        public Executer()
        {
            _mapModToSim = new Dictionary<DesktopServerLogical.Models.Pin, Pin>();
            _mapSimToMod = new Dictionary<Pin, DesktopServerLogical.Models.Pin>();
        }

        public void ActionTriggered(Input pin, bool runNegativeTriggeredActions = false)
        {
            DesktopServerLogical.Models.Pin modPin = _mapSimToMod[pin];
            if (null == modPin)
                return;

            var actions = modPin.Actions;
            if(runNegativeTriggeredActions)
            {
                actions = modPin.ActiveLowActions;
            }

            foreach (DesktopServerLogical.Models.RemoteAction action in actions)
            {
                Pin p;
                _mapModToSim.TryGetValue(action.Pin, out p);
                Output output = null;
                if (DesktopServerLogical.Enums.ActionTypes.Delay == action.Type)
                {

                }
                else if (null == p)
                {
                      continue;
                }
                else
                {
                    output = (Output)p;
                }
                switch(action.Type)
                {
                    case DesktopServerLogical.Enums.ActionTypes.TurnOn:
                        output.TurnOn();
                        break;
                    case DesktopServerLogical.Enums.ActionTypes.Switch:
                        output.Switch();
                        break;
                    case DesktopServerLogical.Enums.ActionTypes.TurnOff:
                        output.TurnOff();
                        break;
                    case DesktopServerLogical.Enums.ActionTypes.Delay:
                        System.Threading.Thread.Sleep(action.Value * 1000);
                        break;
                }
            }

        }
    }
}
