using DesktopServerLogical.Models;
using FacialRecognition;
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

//using DesktopServerLogical;

namespace Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SimulatorWindow : Window, INotifyPropertyChanged
    {
        private Light _light;
        private Door _door;
        private Switch _switch;
        private Input _networkButton1;
        private Input _networkButton2;
        private Input _networkButton3;
        private Input _keyboard;
        private Executer _executer;
        Notifications _notifications;

        private Input _voice1;
        private Input _voice2;
        private Input _voice3;

        private Input _face1;
        private Input _face2;
        private Input _face3;

        private Input _lightIntensity;

        private FacialRecognitionWindow _facialRecognition;

        private ObservableCollection<Device> _devices;


        public Light Light
        {
            get => _light;
            set
            {
                _light = value;
                OnPropertyChanged("Light");
            }
        }

        


        public SimulatorWindow(ObservableCollection<Device> devices)
        {
            //DesktopServerLogical
            InitializeComponent();

            this.DataContext = this;


            _devices = devices;

            _facialRecognition = new FacialRecognitionWindow(FaceDetected);

            _executer = new Executer();

            _notifications = new Notifications(notification);


            _switch = new Switch(button);
            _networkButton1 = new Input();
            _networkButton2 = new Input();
            _networkButton3 = new Input();
            _keyboard = new Input();

            _voice1 = new Input();
            _voice2 = new Input();
            _voice3 = new Input();

            _face1 = new Input();
            _face2 = new Input();
            _face3 = new Input();

            _lightIntensity = new Input();

            _light = new Light(bulb);
            _light.TurnOff();

            _door = new Door(door);
            _door.TurnOff();

            _executer.AddSimulatedPin(_switch, devices[1].InputPins[1]);
            _executer.AddSimulatedPin(_light, devices[1].OutputPins[0]);
            _executer.AddSimulatedPin(_networkButton1, devices[0].InputPins[0]);
            _executer.AddSimulatedPin(_networkButton2, devices[0].InputPins[1]);
            _executer.AddSimulatedPin(_networkButton3, devices[0].InputPins[2]);

            _executer.AddSimulatedPin(_keyboard, devices[2].InputPins[0]);

            _executer.AddSimulatedPin(_door, devices[3].OutputPins[0]);

            _executer.AddSimulatedPin(_notifications.Notification1, devices[0].OutputPins[0]);
            _executer.AddSimulatedPin(_notifications.Notification2, devices[0].OutputPins[1]);
            _executer.AddSimulatedPin(_notifications.Notification3, devices[0].OutputPins[2]);

            _executer.AddSimulatedPin(_voice1, devices[4].InputPins[0]);
            _executer.AddSimulatedPin(_voice2, devices[4].InputPins[1]);
            _executer.AddSimulatedPin(_voice3, devices[4].InputPins[2]);

            _executer.AddSimulatedPin(_face1, devices[5].InputPins[0]);
            _executer.AddSimulatedPin(_face2, devices[5].InputPins[1]);
            _executer.AddSimulatedPin(_face3, devices[5].InputPins[2]);

            _executer.AddSimulatedPin(_lightIntensity, devices[1].InputPins[0]);
        }

        public void UpdateConfiguration(ObservableCollection<Device> devices)
        {
            MessageBox.Show("COnfiguration updated!");
        }



        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            _executer.ActionTriggered(_switch);
            _switch.SwitchButton();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _executer.ActionTriggered(_networkButton1);
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            _executer.ActionTriggered(_networkButton2);
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            _executer.ActionTriggered(_networkButton3);
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button2_Click_1(object sender, RoutedEventArgs e)
        {
            if(pinTextBox.Text.Equals("0000"))
            {
                MessageBox.Show("Pin correct!");
                _executer.ActionTriggered(_keyboard);
            }
            else
            {
                MessageBox.Show("Pin incorrect! (The pin is 0000)");
            }
        }

        private void button3_Click_1(object sender, RoutedEventArgs e)
        {
            _executer.ActionTriggered(_voice1);
        }

        private void button3_Copy_Click(object sender, RoutedEventArgs e)
        {
            _executer.ActionTriggered(_voice2);
        }

        private void button3_Copy1_Click(object sender, RoutedEventArgs e)
        {
            _executer.ActionTriggered(_voice3);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _facialRecognition.Show();
        }

        private void FaceDetected(int id)
        {
            switch(id)
            {
                case 1:
                    _executer.ActionTriggered(_face1);
                    break;
                case 2:
                    _executer.ActionTriggered(_face2);
                    break;
                case 3:
                    _executer.ActionTriggered(_face3);
                    break;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                if ((int)e.NewValue > _devices[1].InputPins[0].TriggeredValue
                    && (int)e.OldValue <= _devices[1].InputPins[0].TriggeredValue)
                {
                    _executer.ActionTriggered(_lightIntensity);
                }

                if ((int)e.NewValue < _devices[1].InputPins[0].TriggeredValue
                    && (int)e.OldValue >= _devices[1].InputPins[0].TriggeredValue)
                {
                    _executer.ActionTriggered(_lightIntensity, true);
                }
            }
            catch(Exception ee)
            {

            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string name)
        {
            if(null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
