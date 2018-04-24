using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator
{
   public  class NotificationInput : Output
    {
        private Action _action;
        public NotificationInput(Action action)
            :base()
        {
            _action = action;
        }

        public override void UpdateStatus()
        {
            _action();
        }
    }
}
