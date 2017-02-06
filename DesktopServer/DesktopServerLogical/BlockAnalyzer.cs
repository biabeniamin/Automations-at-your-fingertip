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
        private static void AnalyzeBlock(Pin pin,BlockControl blockControl)
        {
            RemoteAction action = null;
            switch(blockControl.Type)
            {
                case BlockType.For:
                    pin.Repeats = (int)blockControl.GetValue();
                    break;
            }
            //return action;
        }
        public static void Analyze(Pin pin,BlockControl blockControl)
        {
            AnalyzeBlock(pin,blockControl);
        }
    }
}
