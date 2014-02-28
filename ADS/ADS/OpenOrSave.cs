using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADS
{
    public partial class OpenOrSave : Form
    {
        public OpenSave OpenSave { get; set; }

        public OpenOrSave()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenSave = OpenSave.Open;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenSave = OpenSave.Save;
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
