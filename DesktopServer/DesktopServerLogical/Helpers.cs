using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DesktopServerLogical
{
    public static class Helpers
    {
        public static Pin GetPin(Device owner, int pinNumber)
        {
            Pin pin = null;
            try
            {
                pin = owner.Pins.Where<Pin>((p) =>
                {
                    if (p.PinNumber == pinNumber)
                        return true;
                    else
                        return false;
                }).ToList<Pin>()[0];
            }
            catch
            {
                //
            }
            return pin;
        }
        public static Device GetDevice(ObservableCollection<Device> devices,int address)
        {
            Device device = null;
            try
            {
                device = devices.Where<Device>((dev) =>
                {
                    if (dev.Address == address)
                        return true;
                    else
                        return false;
                }).ToList<Device>()[0];
            }
            catch
            {
                //
            }
            return device;
        }
        public static Point GetPositionOfControl(Visual v, UIElement c)
        {
            return c.TransformToAncestor(v).Transform(new Point(0, 0));
        }
        public static System.Drawing.Point ConvertWindowsToDrawingPoint(Point p)
        {
            return new System.Drawing.Point((int)p.X, (int)p.Y);
        }
        public static System.Drawing.Size ConvertWindowsToDrawingSize(Size s)
        {
            return new System.Drawing.Size((int)s.Width, (int)s.Height);
        }
        public static System.Drawing.Rectangle GetRectangleOfButtonControl(Visual v, UIElement b)
        {
            Point location = GetPositionOfControl(v, b);
            Size size = new Size(GetWidthOfElement(b), GetHeightOfElement(b));
            System.Drawing.Rectangle r1 = new System.Drawing.Rectangle(ConvertWindowsToDrawingPoint(location), ConvertWindowsToDrawingSize(size));
            return r1;
        }
        public static BlockControl GetBlockControl(UIElement b, List<BlockControl> controls)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (b == controls[i].Block)
                    return controls[i];
                BlockControl subButtonControl = GetBlockControl(b, controls[i].Childs);
                if (subButtonControl != null)
                    return subButtonControl;
            }
            return null;
        }
        public static BlockControl GetIndexOfIntersectedItem(Visual v, BlockControl b, List<BlockControl> controls)
        {
            if (b == null)
                return null;
            for (int i = 0; i < controls.Count; i++)
            {
                if (b == controls[i])
                    continue;
                if (b.doesIntersectsWith(controls[i], v))
                    return controls[i];
                BlockControl subItems = GetIndexOfIntersectedItem(v, b, controls[i].Childs);
                if (subItems != null)
                    return subItems;
            }
            return null;
        }
        public static double GetWidthOfElement(UIElement element)
        {
            return ((Canvas)element).Width;
        }
        public static double GetHeightOfElement(UIElement element)
        {
            return ((Canvas)element).Height;
        }
    }
}
