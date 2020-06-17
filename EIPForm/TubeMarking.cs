using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;

namespace EIPForm
{
    public partial class TubeMarking : Form
    {
        public TubeMarking()
        {
            InitializeComponent();
        }

        private void TubeMarking_Load(object sender, EventArgs e)
        {
            DataSource.DataSource = new SqlConnection(ConfigurationManager.AppSettings["Datasource"]);
        }
    }
}
