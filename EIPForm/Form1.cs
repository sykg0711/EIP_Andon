using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EIPForm
{
    public partial class Form1 : Form
    {
        public delegate void Form1Update();
        private bool start;
        EIPLib EIPLib = new EIPLib();
        List<CheckBox> checkBoxArray = new List<CheckBox>();
        List<TextBox> dataAreaArray = new List<TextBox>();
        Task waitTask;

        public Form1()
        {
            InitializeComponent();

            //データエリア番号を登録
            dataAreaArray.Add(DataArea0);
            dataAreaArray.Add(DataArea1);
            timer1.Interval = 1000;
            timer1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            start = true;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            EIPLib.GetFrom = this;
            EIPLib.SearchDevice();
            comboBox1.Items.AddRange(EIPLib.DeviceList.Select(d => d.ProductName1).ToArray());

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            EIPLib.SetIpAddress = EIPLib.DeviceList[comboBox1.SelectedIndex].SocketAddress.SIN_Address;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            EIPLib.Close();
        }

        public void Form1_DataAreaUpdate(string str, int id)
        {
            dataAreaArray[id].Text = str;
        }

        public void Form1_CheckboxUpdata(bool bit, int id)
        {
            checkBoxArray[id].Checked = bit;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            DateTime d = DateTime.Now;
            label5.Text = string.Format("{0:00}:{1:00}:{2:00}", d.Hour, d.Minute, d.Second);

            if (start)
            {
                int.TryParse(comboBox2.SelectedItem.ToString(), out int waittime);

                EIPLib.EIP_Status status1 = EIPLib.ReadInstance(0, 0x64, EIPLib.DataType.DM, true);
                if (status1.code != 0)
                {
                    start = false;
                    MessageBox.Show(status1.message);
                }
                DataArea0.Refresh();
                EIPLib.EIP_Status status2 = EIPLib.ReadInstance(1, 0x66, EIPLib.DataType.DM, false);
                if(status2.code !=0)
                {
                    start = false;
                    MessageBox.Show(status2.message);
                }
                DataArea1.Refresh();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            byte[] sendData = { 0xFF, 0xFF };
            EIPLib.WriteInstance(0x67, EIPLib.DataType.L, sendData, false);


        }
    }
}
