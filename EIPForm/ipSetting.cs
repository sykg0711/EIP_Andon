using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;


namespace EIPForm
{
    public partial class IPAddressSetting : Form
    {
        public EIP_Andon parent;
        public EIPLib lib;

        public IPAddressSetting()
        {
            InitializeComponent();

        }

        private void IPAddressSetting_Load(object sender, EventArgs e)
        {
            string[] array = lib.DeviceList.Select(d => d.SocketAddress.SIN_Address.ToString()).ToArray();
            string ipString = "";
            foreach(string ipserial in array)
            {
                uint ip = uint.Parse(ipserial);
                //上位から順番にビット積を計算
                ipString += byte.Parse((ip >> 24 & 0xFF).ToString()) + ".";
                ipString += byte.Parse((ip >> 16 & 0xFF).ToString()) + ".";
                ipString += byte.Parse((ip >> 8 & 0xFF).ToString()) + ".";
                ipString += byte.Parse((ip & 0xFF).ToString());
                comboBox1.Items.Add(ipString);
                ipString = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            parent.destination = comboBox1.SelectedIndex;
            this.Close();
        }
    }
}
