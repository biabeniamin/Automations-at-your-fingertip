using DesktopServerLogical.Enums;
using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            //b.Margin = new Thickness(location.X, location.Y, 0, 0);
            TranslateTransform t = new TranslateTransform();
            t.X = location.X;
            t.Y = location.Y;
            b.RenderTransform = t;
            b.VerticalAlignment = VerticalAlignment.Top;
            b.HorizontalAlignment = HorizontalAlignment.Left;
            return b;
        }
        public static Polygon GenerateEndConnector()
        {
            Polygon pol = new Polygon();
            pol.Points.Add(new Point(100, 0));
            pol.Points.Add(new Point(150, 0));
            pol.Points.Add(new Point(100, 50));
            pol.Points.Add(new Point(150, 100));
            pol.Points.Add(new Point(100, 100));
            pol.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            return pol;
        }
        public static Polygon GenerateMarginBeginConnector()
        {
            int thick = 5;
            Polygon margin = new Polygon();
            margin.Points.Add(new Point(0, 0));
            margin.Points.Add(new Point(-50, 50));
            margin.Points.Add(new Point(0, 100));
            margin.Points.Add(new Point(thick, 100));
            margin.Points.Add(new Point(-50+ thick, 50));
            margin.Points.Add(new Point(thick, 0));
            margin.Fill = new SolidColorBrush(Color.FromRgb(255,240,36));
            return margin;
        }
        public static Polygon GenerateBeginConnector()
        {
            Polygon pol = new Polygon();
            pol.Points.Add(new Point(0, 0));
            pol.Points.Add(new Point(-50,50));
            pol.Points.Add(new Point(0, 100));
            pol.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            return pol;
        }
        public static Image GeneratePositioningIcon()
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri($"{System.IO.Directory.GetCurrentDirectory()}\\Images\\arrows.png");
            bitmap.EndInit();
            Image image = new Image();
            image.Source=bitmap;
            TranslateTransform t = new TranslateTransform(-40, 32);
            image.RenderTransform = t;
            return image;
        }
        public static BlockControl GeneratePinTriggeredBlock(Point location)
        {
            BlockControl control;
            Canvas b = (Canvas)GenerateBlock(location, new Size(150, 100), Color.FromRgb(255, 0, 0));
            Label label = GenerateBlockControlItem<Label>(new Point(10, 10), new Size(80, 80));
            label.Content = "Pin triggered";
            b.Children.Add(label);
            ComboBox comboBox = GenerateBlockControlItem<ComboBox>(new Point(5, 35), new Size(90, 25));
            Binding binding = new Binding("InputPins");
            comboBox.SetBinding(ComboBox.ItemsSourceProperty, binding);
            b.Children.Add(comboBox);
            b.Children.Add(GenerateEndConnector());
            b.Children.Add(GenerateBeginConnector());
            b.Children.Add(GenerateMarginBeginConnector());
            control = new BlockControl(b, BlockType.PinTriggered);
            return control;
        }
        public static BlockControl GenerateAnalogTriggeredBlock(Point location,BlockType type)
        {
            BlockControl control;
            Canvas b = (Canvas)GenerateBlock(location, new Size(150, 100), Color.FromRgb(255, 0, 0));
            Label label = GenerateBlockControlItem<Label>(new Point(10, 10), new Size(80, 80));
            if (type == BlockType.PositiveAnalogTriggered)
                label.Content = "Pos An. trigg.";
            else if (type == BlockType.NegativeAnalogTriggered)
                label.Content = "Neg An. trigg.";
            else
            {
                System.Diagnostics.Debug.WriteLine($"Trying to create an analog pin with diff. type!{type}");
            }
            b.Children.Add(label);
            ComboBox comboBox = GenerateBlockControlItem<ComboBox>(new Point(5, 35), new Size(90, 25));
            Binding binding = new Binding("InputPins");
            comboBox.SetBinding(ComboBox.ItemsSourceProperty, binding);
            b.Children.Add(comboBox);
            TextBox textBox = GenerateBlockControlItem<TextBox>(new Point(5, 65), new Size(90, 30));
            b.Children.Add(textBox);
            b.Children.Add(GenerateEndConnector());
            b.Children.Add(GenerateBeginConnector());
            b.Children.Add(GenerateMarginBeginConnector());
            control = new BlockControl(b, type);
            return control;
        }
        public static BlockControl GenerateForBlock(Point location)
        {
            BlockControl control;
            Canvas b = (Canvas)GenerateBlock(location, new Size(150, 100), Color.FromRgb(0, 0, 255));
            Label label = GenerateBlockControlItem<Label>(new Point(5, 5), new Size(90, 25));
            label.Content = "Repeats";
            b.Children.Add(label);
            TextBox textBox = GenerateBlockControlItem<TextBox>(new Point(5, 35), new Size(90, 60));
            textBox.Text = "1";
            b.Children.Add(textBox);
            b.Children.Add(GenerateEndConnector());
            b.Children.Add(GenerateBeginConnector());
            b.Children.Add(GenerateMarginBeginConnector());
            b.Children.Add(GeneratePositioningIcon());
            control = new BlockControl(b, BlockType.For);
            return control;
        }
        public static BlockControl GenerateDelayBlock(Point location)
        {
            BlockControl control;
            Canvas b = (Canvas)GenerateBlock(location, new Size(150, 100), Color.FromRgb(0, 0, 255));
            Label label = GenerateBlockControlItem<Label>(new Point(5, 5), new Size(90, 25));
            label.Content = "Delay";
            b.Children.Add(label);
            TextBox textBox = GenerateBlockControlItem<TextBox>(new Point(5, 35), new Size(90, 60));
            b.Children.Add(textBox);
            b.Children.Add(GenerateEndConnector());
            b.Children.Add(GenerateBeginConnector());
            b.Children.Add(GenerateMarginBeginConnector());
            b.Children.Add(GeneratePositioningIcon());
            control = new BlockControl(b, BlockType.DelayAction);
            return control;
        }
        public static BlockControl GeneratePinActionBlock(Point location,BlockType type)
        {
            BlockControl control;
            Canvas b = (Canvas)GenerateBlock(location, new Size(150, 100), Color.FromRgb(0, 0, 255));
            Label label = GenerateBlockControlItem<Label>(new Point(5, 5), new Size(90, 25));
            if (type == BlockType.SwitchAction)
                label.Content = "Switch";
            else if (type == BlockType.TurnOnAction)
                label.Content = "Turn On";
            else if (type == BlockType.TurnOffAction)
                label.Content = "Turn OFF";
            else
            {
                System.Diagnostics.Debug.WriteLine($"Trying to create an pin action with diff. type!{type}");
            }
            b.Children.Add(label);
            ComboBox comboBox = GenerateBlockControlItem<ComboBox>(new Point(5, 35), new Size(90, 25));
            Binding binding = new Binding("OutputPins");
            comboBox.SetBinding(ComboBox.ItemsSourceProperty, binding);
            b.Children.Add(comboBox);
            b.Children.Add(GenerateEndConnector());
            b.Children.Add(GenerateBeginConnector());
            b.Children.Add(GenerateMarginBeginConnector());
            b.Children.Add(GeneratePositioningIcon());
            control = new BlockControl(b, type);
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
                case BlockType.TurnOnAction:
                case BlockType.TurnOffAction:
                    element = GeneratePinActionBlock(point,type);
                    break;
                case BlockType.DelayAction:
                    element = GenerateDelayBlock(point);
                    break;
                case BlockType.PositiveAnalogTriggered:
                case BlockType.NegativeAnalogTriggered:
                    element = GenerateAnalogTriggeredBlock(point,type);
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
