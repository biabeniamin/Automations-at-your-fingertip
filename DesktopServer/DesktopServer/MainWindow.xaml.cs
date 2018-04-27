#define ADD_DEVICES
#define RUN_SIMULATOR

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
        private bool _wasBlockMoved = false;
        private bool _isDown = false;
        private BlockControl _movedBlock;
        private DelegateCommand _addBlockCommand;
        private string _trashClosedPath = "Images/recyclePart.png";
        private string _trashOpenPath = "Images/openRecyclePart.png";
        private string _trashPath;
        private ObservableCollection<string> _presetConfig;
        private string _selectedSave;
        private string _saveName;
        private DelegateCommand _showHideSavesCommand;
        private Visibility _areSavesShowed=Visibility.Hidden;
        private DelegateCommand _reinitiliazingCommand;
        private Simulator.SimulatorWindow _simulatorWindow;

        public DelegateCommand ReinitializingCommand
        {
            get { return _reinitiliazingCommand; }
            set { _reinitiliazingCommand = value; }
        }
        

        public Visibility AreSavesShowed
        {
            get { return _areSavesShowed; }
            set
            {
                _areSavesShowed = value;
                OnPropertyChanged("AreSavedShowed");
            }
        }

        public DelegateCommand ShowHideSavesCommand
        {
            get { return _showHideSavesCommand; }
            set { _showHideSavesCommand = value; }
        }

        public string SaveName
        {
            get { return _saveName; }
            set
            {
                _saveName = value;
                OnPropertyChanged("SaveName");
            }
        }

        public string SelectedSave
        {
            get { return _selectedSave; }
            set
            {
                _selectedSave = value;
                OnPropertyChanged("SelectedSave");
            }
        }

        public ObservableCollection<string> PresetConfig
        {
            get { return _presetConfig; }
            set { _presetConfig = value; }
        }

        public string TrashPath
        {
            get { return _trashPath; }
            set
            {
                _trashPath = value;
                OnPropertyChanged("TrashPath");
            }
        }
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
        public ObservableCollection<Pin> AnalogPins
        {
            get
            {
                return _controller.AnalogPins;
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
                    /*if(_wasModifiedUsingBlocks)
                        AnalyzeBlocksForPin(_selectedPin);*/
                }
                _wasBlockMoved = false;
                _selectedPin = value;
                if (_selectedPin != null)
                {
                    _blockControls.Clear();
                    grid.Children.Clear();
                    GenerateBlocksForPin(_selectedPin);
                }
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
        public ObservableCollection<ConditionType> ConditionTypes
        {
            get
            {
                ObservableCollection<ConditionType> list = new ObservableCollection<ConditionType>();
                foreach (ConditionType type in Enum.GetValues(typeof(ConditionType)))
                    list.Add(type);
                return list;
            }
        }
        private Point GetDefaultBlockPosition()
        {
            return (new Point(600, -150));
        }
        public MainWindow()
        {
            InitializeComponent();
            _controller = new Controller(App.Current.Dispatcher);
            DataContext = this;
            TrashPath = _trashClosedPath;
            AddActionCommand = new DelegateCommand(AddAction);
            AddActiveLowActionCommand= new DelegateCommand(AddActiveLowAction);
            SaveActionCommand = new DelegateCommand(SaveAction);
            LoadActionCommand = new DelegateCommand(LoadAction);
            LoadDevicesCommand = new DelegateCommand(LoadDevices);
            ProgramActionsCommand = new DelegateCommand(ProgramActions);
            AddBlockCommand = new DelegateCommand(AddBlockAction);
            ShowHideSavesCommand = new DelegateCommand(ShowHideSavesAction);
            ReinitializingCommand = new DelegateCommand(Reinitializing);
            _saves = new Saves();
            _blockControls = new List<BlockControl>();
            LoadDevices();
            LoadSaves();

#if RUN_SIMULATOR
            _simulatorWindow = new Simulator.SimulatorWindow(_controller.Devices);
            _simulatorWindow.Show();
#endif
        }

        private void LoadSaves()
        {
            PresetConfig = _saves.GetActions();
        }

        private void ProgramActions()
        {
            if (_selectedPin != null)
            {
                if (_wasBlockMoved)
                    AnalyzeBlocksForPin(_selectedPin);
            }
            
#if RUN_SIMULATOR
            _simulatorWindow.UpdateConfiguration(_controller.Devices);
#else
            _controller.ProgramMaster();
#endif
            MessageBox.Show("Programming is done!");
        }
        private void LoadDevices()
        {
#if ADD_DEVICES
            _controller.Devices.Clear(); ;
            //Network interface
            _controller.Devices.Add(new Device(1, DeviceTypes.Network));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0], 1, PinTypes.Input));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0], 2, PinTypes.Input));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0], 3, PinTypes.Input));
            //add notification in network
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0], 6, PinTypes.Output));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0], 7, PinTypes.Output));
            _controller.Devices[0].Pins.Add(new Pin(_controller.Devices[0], 8, PinTypes.Output));

            //relay module
            _controller.Devices.Add(new Device(2, DeviceTypes.Relay));
            _controller.Devices[1].Pins.Add(new Pin(_controller.Devices[1], 5, PinTypes.Analog));
            Pin oPin = new Pin(_controller.Devices[1], 9, PinTypes.Output);
            _controller.Devices[1].Pins.Add(oPin);
            Pin pin = new Pin(_controller.Devices[1], 8, PinTypes.Input);
            pin.Actions.Add(new RemoteAction(oPin, ActionTypes.Switch, pin));
            //RemoteAction del = new RemoteAction(oPin, ActionTypes.Delay, null);
            //del.Value = 5;
            //pin.Actions.Add(del);
            _controller.Devices[1].Pins.Add(pin);
            
            //keypad module
            _controller.Devices.Add(new Device(3, DeviceTypes.Keyboard));
            _controller.Devices[2].Pins.Add(new Pin(_controller.Devices[2], 7, PinTypes.Input));

            //motor controller
            _controller.Devices.Add(new Device(4, DeviceTypes.Motor));
            _controller.Devices[3].Pins.Add(new Pin(_controller.Devices[3], 7, PinTypes.Output));
            _controller.Devices[3].Pins.Add(new Pin(_controller.Devices[3], 9, PinTypes.Output));

            //voice assistance
            _controller.Devices.Add(new Device(5, DeviceTypes.VoiceAssistance));
            _controller.Devices[4].Pins.Add(new Pin(_controller.Devices[4], 1, PinTypes.Input));
            _controller.Devices[4].Pins.Add(new Pin(_controller.Devices[4], 2, PinTypes.Input));
            _controller.Devices[4].Pins.Add(new Pin(_controller.Devices[4], 3, PinTypes.Input));

            //facial recognition
            _controller.Devices.Add(new Device(6, DeviceTypes.FacialRecognition));
            _controller.Devices[5].Pins.Add(new Pin(_controller.Devices[5], 1, PinTypes.Input));
            _controller.Devices[5].Pins.Add(new Pin(_controller.Devices[5], 2, PinTypes.Input));
            _controller.Devices[5].Pins.Add(new Pin(_controller.Devices[5], 3, PinTypes.Input));

            SelectedDevice = _controller.Devices[1];
            SelectedPin = _controller.Devices[1].Pins[2];
#else
            _controller.LoadDevices();
#endif
        }

        private void Reinitializing()
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void SaveAction()
        {
            if (SaveName == null)
                SaveName = DateTime.Now.ToString("hhmmss");
            _saves.AddSave(_controller.Devices, SaveName);
            //LoadSaves();
            PresetConfig.Add(SaveName);
        }
        private void ShowHideSavesAction()
        {
            System.Windows.Forms.MessageBox.Show("Test");
        }
        private void LoadAction()
        {
            if (SelectedSave!=null)
            {
                _saves.LoadActions(SelectedSave, _controller.Devices);
                /*for (int i = 0; i < SelectedPin.Actions.Count; i++)
                {
                    SelectedPin.Actions[i].RemoveAction = _controller.RemoveAction;
                    SelectedPin.Actions[i].OwnerPin = SelectedPin;
                }*/
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
                case ActionTypes.TurnOn:
                    block = GenerateNewBlock(location, BlockType.TurnOnAction);
                    ((ComboBox)((Canvas)block.Block).Children[1]).SelectedItem = action.Pin;
                    break;
                case ActionTypes.TurnOff:
                    block = GenerateNewBlock(location, BlockType.TurnOffAction);
                    ((ComboBox)((Canvas)block.Block).Children[1]).SelectedItem = action.Pin;
                    break;
                case ActionTypes.Delay:
                    block = GenerateNewBlock(location, BlockType.DelayAction);
                    ((TextBox)((Canvas)block.Block).Children[1]).Text = action.Value.ToString();
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
        private void GenerateAllChildBlocks(Pin pin,Point location,BlockControl parent)
        {
            BlockControl last = parent;
            if (parent.Type == BlockType.NegativeAnalogTriggered)
            {
                foreach (RemoteAction action in pin.ActiveLowActions)
                {
                    location = new Point(location.X + ((Canvas)parent.Block).Width, location.Y);
                    last = GenerateSubBlocksForAction(action, last, location);
                }
            }
            else
            {
                foreach (RemoteAction action in pin.Actions)
                {
                    location = new Point(location.X + ((Canvas)parent.Block).Width, location.Y);
                    last = GenerateSubBlocksForAction(action, last, location);
                }
            }
        }
        private BlockControl GenerateForBlock(ref Point location,BlockControl parent, Pin pin)
        {
            location = new Point(location.X + ((Canvas)parent.Block).Width, location.Y);
            BlockControl forBlock= GenerateNewBlock(location, BlockType.For);
            ((TextBox)((Canvas)forBlock.Block).Children[1]).Text = pin.Repeats.ToString();
            grid.Children.Add(forBlock.Block);
            forBlock.Parent = parent;
            parent.AddSubBlockControl(forBlock);
            return forBlock;
        }
        private void GenerateBlocksForPin(Pin pin)
        {
            BlockControl parent;
            BlockControl forBlock;
            switch (pin.Type)
            {
                case PinTypes.Input:
                    Point location = new Point(-25, 0);
                    parent=AddNewBlock(location, BlockType.PinTriggered,pin);
                    forBlock=GenerateForBlock(ref location, parent, pin);
                    GenerateAllChildBlocks(pin, location, forBlock);
                    break;
                case PinTypes.Analog:
                    Point posLocation = new Point(-25, 0);
                    parent = AddNewBlock(posLocation, BlockType.PositiveAnalogTriggered, pin);
                    forBlock = GenerateForBlock(ref posLocation, parent, pin);
                    GenerateAllChildBlocks(pin, posLocation, parent);
                    posLocation = new Point(-25, 150);
                    parent = AddNewBlock(posLocation, BlockType.NegativeAnalogTriggered, pin);
                    forBlock = GenerateForBlock(ref posLocation, parent, pin);
                    GenerateAllChildBlocks(pin, posLocation, parent);
                    break;
            }
        }
        private BlockControl AddNewBlock(Point location, BlockType type)
        {
            BlockControl b = GenerateNewBlock(location, type);
            grid.Children.Add(b.Block);
            _blockControls.Add(b);
            return b;
        }
        private BlockControl AddNewBlock(Point location, BlockType type,object arg)
        {
            BlockControl b = GenerateNewBlock(location, type);
            grid.Children.Add(b.Block);
            _blockControls.Add(b);
            switch(type)
            {
                case BlockType.PinTriggered:
                    ((ComboBox)((Canvas)b.Block).Children[1]).SelectedItem = arg;
                    break;
                case BlockType.PositiveAnalogTriggered:
                case BlockType.NegativeAnalogTriggered:
                    ((ComboBox)((Canvas)b.Block).Children[1]).SelectedItem = arg;
                    ((Slider)((Canvas)b.Block).Children[2]).Value= ((Pin)arg).TriggeredValue;
                    break;
                case BlockType.For:
                    ((TextBox)((Canvas)b.Block).Children[1]).Text = ((Pin)arg).Repeats.ToString();
                    break;
            }
            return b;
        }
        private void AddBlockAction(object parameter)
        {
            BlockType type = (BlockType)Enum.Parse(typeof(BlockType), parameter.ToString());
            AddNewBlock(GetDefaultBlockPosition(), type);
        }
        private void button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _wasBlockMoved = false;
            Point pp = Mouse.GetPosition(grid);
            UIElement b = sender as UIElement;
            BlockControl bC = Helpers.GetBlockControl(b, _blockControls);
            _movedBlock = bC;
            Point locationOnBlock = Helpers.DifferencePoint(pp, bC.GetPosition());
            if(locationOnBlock.X<0)
                _isDown = true;
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
                tt.X = t.X + Helpers.GetWidthOfElement(block.Block) * (i+1 + count);
                tt.Y = t.Y;
                block.Childs[i].Block.RenderTransform = tt;
                count++;
                count+=MoveSubBlocks(block.Childs[i], new Point(tt.X, tt.Y));
            }
            return count;
        }
        private void RemoveBlock(BlockControl block)
        {
            if (block.Parent != null)
            {
                block.Parent.Childs.Remove(block);
                foreach(BlockControl b in block.Childs)
                    block.Parent.Childs.Add(b);
                if(block.Parent.Parent!=null)
                    MoveSubBlocks(block.Parent, block.Parent.Parent.GetPositionOfChild());
            }
            else
            {
                _blockControls.Remove(block);
            }
            grid.Children.Remove(block.Block);
        }
        private void button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown == true)
            {
                _wasBlockMoved = true;
                UIElement b = sender as UIElement;
                BlockControl bC = Helpers.GetBlockControl(b, _blockControls);
                if (bC != _movedBlock)
                {
                    _isDown = false;
                    return;
                }
                TranslateTransform t = new TranslateTransform();
                Point pp = Mouse.GetPosition(grid);
                label2.Content = pp.ToString();
                MoveSubBlocks(bC, new Point(pp.X, pp.Y-50));
                if (Helpers.IsOverTrash(pp))
                    TrashPath = _trashOpenPath;
                else
                    TrashPath = _trashClosedPath;
            }
        }
        private void button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            UIElement b = sender as UIElement;
            BlockControl bC = Helpers.GetBlockControl(b, _blockControls);
            BlockControl overButton = Helpers.GetIntersectedBlock(grid, bC, _blockControls);
            if(Helpers.IsOverTrash(bC.GetPosition()))
            {
                RemoveBlock(bC);
                TrashPath = _trashClosedPath;
            }
            else if (overButton != null)
            {
                if(bC.Parent!=null)
                bC.Parent.Childs.Remove(bC);
                bC.Parent = overButton;
                overButton.AddSubBlockControl(bC);
                _blockControls.Remove(bC);
                MoveSubBlocks(bC, overButton.GetPositionOfChild());
            }
            else if(bC.Parent!=null && _wasBlockMoved) 
            {
                bC.Parent.Childs.Remove(bC);
                _blockControls.Add(bC);
                bC.Parent = null;
            }
            if (bC != null)
                label.Content = bC.Childs.Count;
            _isDown = false;
            AnalyzeBlocksForPin(_selectedPin);
        }

        private void AnalyzeBlocksForPin(Pin pin)
        {
            if(null == pin)
            {
                return;
            }
            pin.ClearActions();
            for (int i = 0; i < _blockControls.Count; i++)
            {
                BlockAnalyzer.Analyze(_blockControls[i]);
                //MessageBox.Show(_blockControls[i].GetValue().ToString());
            }
            //AnalyzeIfBlocks();
        }
        private void AnalyzeIfBlocks()
        {
            for (int i = 0; i < _blockControls.Count; i++)
            {
                BlockAnalyzer.Analyze(_blockControls[i]);
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
                    for (int i = 0; i < _blockControls.Count; i++)
                    {
                        BlockAnalyzer.Analyze(_blockControls[i]);
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
