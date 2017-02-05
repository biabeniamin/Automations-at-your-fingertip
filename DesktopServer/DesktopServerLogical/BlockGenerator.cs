using DesktopServerLogical.Enums;
using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DesktopServerLogical
{
    public static class BlockGenerator
    {
        private static type GenerateBlockControlItem<type>(Point location, Size size)
            where type : Control, new()
        {
            Control b = new type();
            b.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            b.Width = size.Width;
            b.Height = size.Height;
            b.Margin = new Thickness(location.X, location.Y, 0, 0);
            b.VerticalAlignment = VerticalAlignment.Top;
            b.HorizontalAlignment = HorizontalAlignment.Left;
            return (type)b;
        }
        public static UIElement GenerateBlock(Point location, Size size, Color color)
        {
            Canvas b = new Canvas();
            b.Background = new SolidColorBrush(color);
            b.Width = size.Width;
            b.Height = size.Height;
            b.Margin = new Thickness(location.X, location.Y, 0, 0);
            b.VerticalAlignment = VerticalAlignment.Top;
            b.HorizontalAlignment = HorizontalAlignment.Left;
            return b;
        }
        public static BlockControl GeneratePinTriggeredBlock(Point location)
        {
            BlockControl control;
            Canvas b = (Canvas)GenerateBlock(location, new Size(100, 100), Color.FromRgb(255, 0, 0));
            Label label = GenerateBlockControlItem<Label>(new Point(10, 10), new Size(80, 80));
            label.Content = "Pin triggered";
            b.Children.Add(label);
            control = new BlockControl(b, BlockType.PinTriggered);
            return control;
        }
        public static BlockControl GenerateForBlock(Point location)
        {
            BlockControl control;
            Canvas b = (Canvas)GenerateBlock(location, new Size(100, 100), Color.FromRgb(0, 0, 255));
            Label label = GenerateBlockControlItem<Label>(new Point(5, 5), new Size(90, 25));
            label.Content = "Repeats";
            b.Children.Add(label);
            TextBox textBox = GenerateBlockControlItem<TextBox>(new Point(5, 35), new Size(90, 60));
            textBox.Text = "1";
            b.Children.Add(textBox);
            control = new BlockControl(b, BlockType.For);
            return control;
        }
        public static BlockControl GenerateDelayBlock(Point location)
        {
            BlockControl control;
            Canvas b = (Canvas)GenerateBlock(location, new Size(100, 100), Color.FromRgb(0, 0, 255));
            Label label = GenerateBlockControlItem<Label>(new Point(5, 5), new Size(90, 25));
            label.Content = "Delay";
            b.Children.Add(label);
            TextBox textBox = GenerateBlockControlItem<TextBox>(new Point(5, 35), new Size(90, 60));
            b.Children.Add(textBox);
            control = new BlockControl(b, BlockType.DelayAction);
            return control;
        }
        public static BlockControl GenerateSwitchActionBlock(Point location)
        {
            BlockControl control;
            Canvas b = (Canvas)GenerateBlock(location, new Size(100, 100), Color.FromRgb(0, 0, 255));
            Label label = GenerateBlockControlItem<Label>(new Point(5, 5), new Size(90, 25));
            label.Content = "Switch";
            b.Children.Add(label);
            ComboBox comboBox = GenerateBlockControlItem<ComboBox>(new Point(5, 35), new Size(90, 25));
            comboBox.Items.Add("2-7");
            b.Children.Add(comboBox);
            control = new BlockControl(b, BlockType.SwitchAction);
            return control;
        }
        public static BlockControl GenerateNewBlock(Point point, BlockType type)
        {
            BlockControl element;
            switch (type)
            {
                case BlockType.For:
                    element = GenerateForBlock(point);
                    break;
                case BlockType.SwitchAction:
                    element = GenerateSwitchActionBlock(point);
                    break;
                case BlockType.DelayAction:
                    element = GenerateDelayBlock(point);
                    break;
                case BlockType.PinTriggered:
                default:
                    element = GeneratePinTriggeredBlock(point);
                    break;
            }
            return element;
        }
    }
}
