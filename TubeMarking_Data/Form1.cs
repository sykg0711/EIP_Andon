using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using EIPForm;

namespace TubeMarking_Data
{ 
    public partial class Form1 : Form
    {
        TubeLib eIPLib = new TubeLib();
        bool tick = false;
        EIPLib.EIP_Status read_request = new EIPLib.EIP_Status();
        private delegate void Form1Update();
        EIP_DataWrite_On_SQLDB.SQLtarget target;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: このコード行はデータを 'e_LineDataSet.Table_1' テーブルに読み込みます。必要に応じて移動、または削除をしてください。
            //this.table_1TableAdapter.Fill(this.e_LineDataSet.Table_1);

            //DBに登録された品番リストを表示
            //configuretion stringはapp.comfiguに記述

            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataSource"].ConnectionString);
            connection.Open();

            string cmd = "SELECT TOP 1000 [DATE],[ID],[PROCEEDED] FROM [E_Line].[dbo].[RECENT]";
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand(cmd, connection);
            adapter.SelectCommand.CommandTimeout = 0;
            adapter.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];

            connection.Close();
            connection.Dispose();

            //SQLRead用のやつ
            target.column = "YS_ID";
            target.database = "ID";
            target.filter = "CUSTOMER_ID";

            timer1.Interval = 1000;

            eIPLib.GetForm = this;
            eIPLib.SearchDevice();
            comboBox1.Items.AddRange(eIPLib.DeviceList.Select(d => d.ProductName1).ToArray());
            //comboBox1.SelectedIndex = 0;
            /*
            var sql = new EIP_DataWrite_On_SQLDB();
            sql.SQLWrite(new object[] { "ID", "PROCEEDED" }, new object[] { "BA660-01400", 14 });
            */
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!tick) { tick = true; }
            else { tick = false; }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           //eIPLib.IpAddressList.Add(eIPLib.DeviceList[comboBox1.SelectedIndex].SocketAddress.SIN_Address);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if (tick)
            {
               
                DateTime d = DateTime.Now;

                EIPLib.EIP_Status status1 = eIPLib.ReadInstance(0x64, EIPLib.DataType.DM, true, destination: comboBox1.SelectedIndex);
                if (status1.code != 0)
                {
                    tick = false;
                    MessageBox.Show(status1.message);
                }
                //前のtickの時と違う品番が返ってきたときにDBに登録する
                //それ以外はbreak
                if (target.value != status1.value)
                {
                    target.value = status1.value;
                    string tmp1 = new EIP_DataWrite_On_SQLDB().SQLRead(target);

                    EIPLib.EIP_Status status2 = eIPLib.ReadInstance(0x65, EIPLib.DataType.DM, false, destination: comboBox1.SelectedIndex);
                    if (status2.code != 0)
                    {
                        tick = false;
                        MessageBox.Show(status2.message);
                    }
                    string tmp2 = status2.value;

                    new EIP_DataWrite_On_SQLDB().SQLWrite(new object[] { "DATE", "ID", "PROCEEDED" }, new object[] { d.ToString(), tmp1, int.Parse(tmp2) });

                    Invoke(new Form1Update(() => { table_1TableAdapter.Update(e_LineDataSet); }));
                }
                
            }            
           
        }
    }
}
