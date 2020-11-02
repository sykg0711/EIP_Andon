using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EIPForm
{
    public partial class EIP_Andon : Form
    {
        public delegate void Form1Update();
        private bool start;
        EIPLib EIPLib = new EIPLib();
        List<CheckBox> checkBoxArray = new List<CheckBox>();
        List<TextBox> dataAreaArray = new List<TextBox>();
        Task waitTask;

        public EIP_Andon()
        {
            InitializeComponent();

            //データエリア番号を登録
            dataAreaArray.Add(DataArea0);
            dataAreaArray.Add(DataArea1);
            timer1.Interval = 1000;
            timer1.Enabled = true;
        }
        /// <summary>
        /// ボタンを押すとtimer1のtickでreadincetanceを実行する再度押した場合止める
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (start) start = false;
            else start = true;

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            EIPLib.GetFrom = this;
            EIPLib.SearchDevice();
            comboBox1.Items.AddRange(EIPLib.DeviceList.Select(d => d.ProductName1).ToArray());
            //comboBox1.SelectedIndex = 0;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //EIPLib.IpAddressList.Add(EIPLib.DeviceList[comboBox1.SelectedIndex].SocketAddress.SIN_Address);
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

                EIPLib.EIP_Status status1 = EIPLib.ReadInstance(0x64, EIPLib.DataType.DM, true, false, destination:comboBox1.SelectedIndex);
                if (status1.code != 0)
                {
                    start = false;
                    MessageBox.Show(status1.message);
                }
                DataArea0.Text = status1.value;
                EIPLib.EIP_Status status2 = EIPLib.ReadInstance(0x66, EIPLib.DataType.DM, false, false, destination:comboBox1.SelectedIndex);
                if(status2.code !=0)
                {
                    start = false;
                    MessageBox.Show(status2.message);
                }
                DataArea1.Text = status2.value;
                EIPLib.EIP_Status status3 = EIPLib.ReadInstance(0x67, EIPLib.DataType.DM, false, true, destination: comboBox1.SelectedIndex);
                if (status3.code != 0)
                {
                    start = false;
                    MessageBox.Show(status3.message);
                }
                DataArea2.Text = status3.value;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            byte[] sendData = { 0xFF, 0xFF };
            EIPLib.WriteInstance(0x67, EIPLib.DataType.L, sendData, false);


        }
    }
}
