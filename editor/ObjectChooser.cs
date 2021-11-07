using System;
using System.Linq;
using System.Windows.Forms;

namespace sth1edwv
{
    public partial class ObjectChooser : Form
    {
        public ObjectChooser(LevelObject levelObject)
        {
            InitializeComponent();

            comboBoxNames.Items.AddRange(LevelObject.Names.Values.ToArray<object>());
            if (LevelObject.Names.TryGetValue(levelObject.Type, out var name))
            {
                comboBoxNames.SelectedItem = name;
            }

            textBoxX.Text = levelObject.X.ToString();
            textBoxY.Text = levelObject.Y.ToString();
            textBoxType.Text = levelObject.Type.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var n = comboBoxNames.SelectedIndex;
            if (n == -1)
                return;
            textBoxType.Text = LevelObject.Names.Keys.ToArray()[n].ToString();
        }
    }
}
