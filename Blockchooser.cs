using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace sth1edwv
{
    public partial class BlockChooser : Form
    {
        public int SelectedBlockIndex => itemPicker1.SelectedIndex;
        private readonly List<Block> _blocks;

        private const int BlocksPerRow = 8;

        public BlockChooser(List<Block> blocks, int selectedBlockIndex)
        {
            _blocks = blocks;
            InitializeComponent();
            itemPicker1.Items = blocks.Cast<IDrawableBlock>().ToList();
            itemPicker1.SelectedIndex = selectedBlockIndex;
        }

        private void itemPicker1_SelectionChanged(object sender, IDrawableBlock e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}