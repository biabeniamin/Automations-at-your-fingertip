using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialReader
{
    public partial class Form1 : Form
    {
        private SerialPort _port;
        public Form1()
        {
            InitializeComponent();
            _port = new SerialPort("COM4");
            _port.Open();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
