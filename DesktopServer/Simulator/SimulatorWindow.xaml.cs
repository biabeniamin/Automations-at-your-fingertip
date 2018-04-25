using DesktopServerLogical.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class SimulatorWindow : Window
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
        public SimulatorWindow(ObservableCollection<Device> devices)
        {
            //DesktopServerLogical
            InitializeComponent();

            _executer = new Executer();

            _notifications = new Notifications(notification);


            _switch = new Switch(button);
            _networkButton1 = new Input();
            _networkButton2 = new Input();
            _networkButton3 = new Input();
            _keyboard = new Input();

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
                _executer.ActionTriggered(_keyboard);
            }
        }

        private void button3_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
