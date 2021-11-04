using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace sth1edwv
{
    public partial class BlockChooser : Form
    {
        public int SelectedBlockIndex => itemPicker1.SelectedIndex;

        public BlockChooser(List<Block> blocks, int selectedBlockIndex)
        {
            InitializeComponent();
            itemPicker1.Items = blocks.Cast<IDrawableBlock>().ToList();
            itemPicker1.SelectedIndex = selectedBlockIndex;
        }

        private void itemPicker1_SelectionChanged(object sender, IDrawableBlock e)
        {
            DialogResult = e == null ? DialogResult.Cancel : DialogResult.OK;
            Close();
        }
    }
}