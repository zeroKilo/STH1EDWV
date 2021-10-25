using System;
using System.Linq;
using System.Windows.Forms;

namespace sth1edwv
{
    public partial class ObjectChooser : Form
    {
        public bool exitOk = false;

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
            exitOk = true;
            this.Close();
        }
    }
}
