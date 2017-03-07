using DesktopServerLogical;
using DesktopServerLogical.Enums;
using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace DesktopServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        private Controller _controller;
        private DelegateCommand _addActionCommand;
        private Pin _selectedPin;
        private Device _selectedDevice;
        private DelegateCommand _saveActionCommand;
        private DelegateCommand _loadActionCommand;
        private Saves _saves;
        private DelegateCommand _loadDevicesCommand;
        private DelegateCommand _programActionsCommand;
        private DelegateCommand _activeLowActionCommand;
        private List<BlockControl> _blockControls;
        bool isDown = false;
        private DelegateCommand _addBlockCommand;
        public DelegateCommand AddBlockCommand
        {
            get { return _addBlockCommand; }
            set { _addBlockCommand = value; }
        }
        public DelegateCommand AddActiveLowActionCommand
        {
            get { return _activeLowActionCommand; }
            set { _activeLowActionCommand = value; }
        }
        public DelegateCommand ProgramActionsCommand
        {
            get { return _programActionsCommand; }
            set { _programActionsCommand = value; }
        }
        public DelegateCommand LoadDevicesCommand
        {
            get { return _loadDevicesCommand; }
            set { _loadDevicesCommand = value; }
        }
        public DelegateCommand LoadActionCommand
        {
            get { return _loadActionCommand; }
            set { _loadActionCommand = value; }
        }
        public ObservableCollection<Pin> OutputPins
        {
            get
            {
                return _controller.OutputPins;
            }
        }
        public ObservableCollection<Pin> InputPins
        {
            get
            {
                return _controller.InputPins;
            }
        }
        public DelegateCommand SaveActionCommand
        {
            get { return _saveActionCommand; }
            set { _saveActionCommand = value; }
        }
        public Pin SelectedPin
        {
            get { return _selectedPin; }
            set
            {
                if (_selectedPin != null)
                {
                    _selectedPin.BlockControls = _blockControls;
                    _selectedPin.Blocks = new List<UIElement>();
                    foreach (UIElement element in grid.Children)
                        _selectedPin.Blocks.Add(element);
                    AnalyzeBlocksForPin(_selectedPin);
                }
                _selectedPin = value;
                if (_selectedPin != null)
                {
                    //_blockControls = _selectedPin.BlockControls;
                    _blockControls.Clear();
                    grid.Children.Clear();
                    /*foreach (UIElement element in _selectedPin.Blocks)
                        grid.Children.Add(element);*/
                }
                GenerateBlocksForPin(_selectedPin);
                OnPropertyChanged("SelectedPin");
            }
        }
        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }
        public DelegateCommand AddActionCommand
        {
            get { return _addActionCommand; }
            set { _addActionCommand = value; }
        }
        public ObservableCollection<Device> Devices
        {
            get { return _controller.Devices; }
        }
        public Array AvailableActions
        {
            get
            {
                return Enum.GetValues(typeof(ActionTypes));
            }
        }
        public ObservableCollection<string> BlockTypes
        {
            get
            {
                ObservableCollection<string> list = new ObservableCollection<string>();
                foreach (BlockType type in Enum.GetValues(typeof(BlockType)))
                    list.Add(type.ToString());
                return list;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            _controller = new Controller(App.Current.Dispatcher);
            DataContext = this;
            AddActionCommand = new DelegateCommand(AddAction);
            AddActiveLowActionCommand= new DelegateCommand(AddActiveLowAction);
            SaveActionCommand = new DelegateCommand(SaveAction);
            LoadActionCommand = new DelegateCommand(LoadAction);
            LoadDevicesCommand = new DelegateCommand(LoadDevices);
            ProgramActionsCommand = new DelegateCommand(ProgramActions);
            AddBlockCommand = new DelegateCommand(AddBlockAction);
            _saves = new Saves();
            _blockControls = new List<BlockControl>();
            LoadDevices();
        }
        private void ProgramActions()
        {
            _controller.ProgramMaster();
            MessageBox.Show("Programare terminata!");
        }
        private void LoadDevices()
        {
            //_controller.LoadDevices();
            _controller.Devices.Add(new Device(2, DeviceTypes.Relay));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0], 5, PinTypes.Analog));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0],8, PinTypes.Input));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0], 7, PinTypes.Output));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0], 9, PinTypes.Output));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0], 4, PinTypes.Output));
        }
        private void SaveAction()
        {
            if (SelectedPin != null)
                _saves.AddSave(SelectedPin.Actions, "test");
        }
        private void LoadAction()
        {
            if (SelectedPin != null)
            {
                SelectedPin.Actions = _saves.LoadActions("test", _controller.Devices);
                for (int i = 0; i < SelectedPin.Actions.Count; i++)
                {
                    SelectedPin.Actions[i].RemoveAction = _controller.RemoveAction;
                    SelectedPin.Actions[i].OwnerPin = SelectedPin;
                }
            }
        }
        private void AddAction()
        {
            if (SelectedPin != null)
            {
                RemoteAction action = new RemoteAction(SelectedPin, ActionTypes.TurnOn, SelectedPin);
                action.RemoveAction = _controller.RemoveAction;
                SelectedPin.Actions.Add(action);
            }
        }
        private void AddActiveLowAction()
        {
            if (SelectedPin != null)
            {
                RemoteAction action = new RemoteAction(SelectedPin, ActionTypes.TurnOn, SelectedPin);
                action.RemoveAction = _controller.RemoveAction;
                SelectedPin.ActiveLowActions.Add(action);
            }
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            //_controller.Devices.Clear();
            /*Request r = new Request(RequestTypes.ValueChange, 1);
            Device d = new Device(1, DeviceTypes.Relay);
            Pin p = new Pin(d, 8, PinTypes.Output);
            r.Pin = p;
            RemoteAction ra = new RemoteAction(p, ActionTypes.Switch,p);
            r.PinAction = ra;
            _controller._serial.Write(r);*/
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        private BlockControl GenerateNewBlock(Point location, BlockType type)
        {
            BlockControl block = BlockGenerator.GenerateNewBlock(location, type);
            block.Block.PreviewMouseDown += button_PreviewMouseDown;
            block.Block.PreviewMouseMove += button_PreviewMouseMove;
            block.Block.PreviewMouseUp += button_PreviewMouseUp;
            return block;
        }
        private BlockControl GenerateBlockForAction(RemoteAction action,Point location)
        {
            BlockControl block=null;
            switch (action.Type)
            {
                case ActionTypes.Switch:
                    block = GenerateNewBlock(location, BlockType.SwitchAction);
                    ((ComboBox)((Canvas)block.Block).Children[1]).SelectedItem = action.Pin;
                    break;
            }
            return block;
        }
        private BlockControl GenerateSubBlocksForAction(RemoteAction action,BlockControl parent,Point location)
        {
            BlockControl block = GenerateBlockForAction(action, location);
            grid.Children.Add(block.Block);
            block.Parent = parent;
            parent.AddSubBlockControl(block);
            return block;
        }
        private void GenerateBlocksForPin(Pin pin)
        {
            switch(pin.Type)
            {
                case PinTypes.Input:
                    Point location = new Point(-25, 0);
                    BlockControl parent=AddNewButton(location, BlockType.PinTriggered,pin);
                    BlockControl last = parent;
                    foreach (RemoteAction action in pin.Actions)
                    {
                        location = new Point(location.X + ((Canvas)parent.Block).Width, location.Y);
                        last=GenerateSubBlocksForAction(action,last, location);
                    }
                    break;
                case PinTypes.Analog:
                    AddNewButton(new Point(-25, 0), BlockType.PositiveAnalogTriggered);
                    AddNewButton(new Point(-25, 150), BlockType.NegativeAnalogTriggered);
                    break;
            }
        }
        private BlockControl AddNewButton(Point location, BlockType type)
        {
            BlockControl b = GenerateNewBlock(location, type);
            grid.Children.Add(b.Block);
            _blockControls.Add(b);
            return b;
        }
        private BlockControl AddNewButton(Point location, BlockType type,object arg)
        {
            BlockControl b = GenerateNewBlock(location, type);
            grid.Children.Add(b.Block);
            _blockControls.Add(b);
            switch(type)
            {
                case BlockType.PinTriggered:
                    ((ComboBox)((Canvas)b.Block).Children[1]).SelectedItem = arg;
                    break;
            }
            return b;
        }
        private void AddBlockAction(object parameter)
        {
            BlockType type = (BlockType)Enum.Parse(typeof(BlockType), parameter.ToString());
            AddNewButton(new Point(0, 0), type);
        }
        private void button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            isDown = true;

        }
        private int MoveSubBlocks(BlockControl block,Point point)
        {
            TranslateTransform t = new TranslateTransform();
            t.X = point.X;
            t.Y = point.Y;
            block.Block.RenderTransform = t;
            int count = 0;
            for (int i = 0; i < block.Childs.Count; i++)
            {
                TranslateTransform tt = new TranslateTransform();
                tt.X = t.X + Helpers.GetWidthOfElement(block.Block) * (i + count);
                tt.Y = t.Y;
                block.Childs[i].Block.RenderTransform = tt;
                count++;
                count+=MoveSubBlocks(block.Childs[i], new Point(tt.X, tt.Y));
            }
            return count;
        }
        private void button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isDown == true)
            {
                UIElement b = sender as UIElement;
                BlockControl bC = Helpers.GetBlockControl(b, _blockControls);
                TranslateTransform t = new TranslateTransform();
                Point pp = Mouse.GetPosition(grid);
                MoveSubBlocks(bC, new Point(pp.X-25, pp.Y-25));
            }
        }
        private void button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            UIElement b = sender as UIElement;
            BlockControl bC = Helpers.GetBlockControl(b, _blockControls);
            BlockControl overButton = Helpers.GetIntersectedBlock(grid, bC, _blockControls);
            if (overButton != null)
            {
                if(bC.Parent!=null)
                bC.Parent.Childs.Remove(bC);
                bC.Parent = overButton;
                overButton.AddSubBlockControl(bC);
                _blockControls.Remove(bC);
            }
            else if(bC.Parent!=null)
            {
                bC.Parent.Childs.Remove(bC);
                _blockControls.Add(bC);
            }
            if (bC != null)
                label.Content = bC.Childs.Count;
            isDown = false;
        }

        private void AnalyzeBlocksForPin(Pin pin)
        {
            pin.ClearActions();
            for (int i = 0; i < pin.BlockControls.Count; i++)
            {
                BlockAnalyzer.Analyze(pin.BlockControls[i]);
                //MessageBox.Show(_blockControls[i].GetValue().ToString());
            }
        }
        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            foreach (Device device in Devices)
            {
                foreach (Pin pin in device.InputPins)
                {
                    pin.ClearActions();
                    for (int i = 0; i < pin.BlockControls.Count; i++)
                    {
                        BlockAnalyzer.Analyze(pin.BlockControls[i]);
                        //MessageBox.Show(_blockControls[i].GetValue().ToString());
                    }
                }
            }
        }
    }
    public class DoubleApproximator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueAsString = value.ToString();
            if (valueAsString.Length > 1)
                valueAsString = valueAsString.Substring(0, 1);
            return valueAsString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 2.2;
        }
    }
}
