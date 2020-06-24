using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            eIPLib = new EIPLib();
        }
    }
}
