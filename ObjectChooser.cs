using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sth1edwv
{
    public partial class ObjectChooser : Form
    {
        public bool _exit_ok = false;

        public ObjectChooser()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = comboBox1.SelectedIndex;
            if (n == -1)
                return;
            textBox3.Text = LevelObjectSet.LevelObject.objNames.Keys.ToArray()[n].ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _exit_ok = true;
            this.Close();
        }
    }
}
