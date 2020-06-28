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
        EIPLib eIPLib;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: このコード行はデータを 'e_LineDataSet.Table_1' テーブルに読み込みます。必要に応じて移動、または削除をしてください。
            this.table_1TableAdapter.Fill(this.e_LineDataSet.Table_1);
            eIPLib = new EIPLib();

            //DBに登録された品番リストを表示
            //configuretion stringはapp.comfiguに記述
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataSource"].ConnectionString);
            connection.Open();

            string cmd = "SELECT TOP 1000 [ID],[PROCEEDED] FROM [E_Line].[dbo].[Table_1]";
            DataSet dataSet = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand(cmd, connection);
            adapter.SelectCommand.CommandTimeout = 0;
            adapter.Fill(dataSet);
            dataGridView1.DataSource = dataSet.Tables[0];

            connection.Close();
            connection.Dispose();
            //insertのてすや
            //↓でうごくや
            //string cmd2 = "INSERT INTO Table_1 (ID, PROCEEDED) VALUES ('BA660-01300', 16)";
            //new SqlCommand(cmd2, connection).ExecuteNonQuery();

            var sql = new EIP_DataWrite_On_SQLDB();
            sql.SQLWrite(new object[] { "ID", "PROCEEDED" }, new object[] { "BA660-01400", 14 });
            
        }
    }
}
