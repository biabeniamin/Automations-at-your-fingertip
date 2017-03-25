using DesktopServerLogical.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopServerLogical.Models
{
    public class Condition
    {
        private Pin _pin;
        private ConditionType _type;

        public ConditionType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public Pin Pin
        {
            get { return _pin; }
            set { _pin = value; }
        }
        public Condition(Pin pin,ConditionType type)
        {
            _pin = pin;
            _type = type;
        }
        public static Condition NoCondition()
        {
            return new Condition(null,ConditionType.NoCondition);
        }
    }
}
