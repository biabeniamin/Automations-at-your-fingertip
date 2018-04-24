using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
    public abstract class Output : Pin
    {
        private Boolean _state;

        public void TurnOn()
        {
            _state = true;
            UpdateStatus();
        }

        public void TurnOff()
        {
            _state = false;
            UpdateStatus();
        }

        public void Switch()
        {
            _state = !_state;
            UpdateStatus();
        }

        public Boolean GetStatus()
        {
            return _state;
        }

        public virtual void UpdateStatus()
        {
            int asdfg = 55;
            asdfg++;
        }
        public Output()
            : base(DesktopServerLogical.Enums.PinTypes.Output)
        {
               
        }
    }
}
