using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using sth1edwv.GameObjects;

namespace sth1edwv.Forms
{
    public sealed partial class ObjectEditor : Form
    {
        public ObjectEditor(LevelObject levelObject)
        {
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;

            comboBoxNames.Items.AddRange(LevelObject.Names.OrderBy(x => x.Name).ToArray<object>());
            if (LevelObject.NamesById.TryGetValue(levelObject.Type, out var name))
            {
                comboBoxNames.SelectedItem = name;
            }

            textBoxX.Text = levelObject.X.ToString();
            textBoxY.Text = levelObject.Y.ToString();
            textBoxType.Text = levelObject.Type.ToString();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxNames.SelectedItem is not LevelObject.NamedObject obj)
            {
                return;
            }

            textBoxType.Text = obj.Type.ToString();
        }

        public byte X => Convert.ToByte(textBoxX.Text);
        public byte Y => Convert.ToByte(textBoxY.Text);
        public byte Type => Convert.ToByte(textBoxType.Text);
    }
}
