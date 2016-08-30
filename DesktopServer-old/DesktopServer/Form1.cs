using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopServerLogical
{
    public partial class Form1 : Form
    {
        private Controller _controller;
        public Form1()
        {
            InitializeComponent();
            _controller = new Controller();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            /*Response resp = new Response(1, TypesOfResponses.Register);
            resp.FromAddress = 0;
            resp.TypeOfDevice = TypesOfDevices.Master;
            _serial.Write(resp);*/
        }
    }
}
