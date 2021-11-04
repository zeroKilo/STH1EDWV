using System.Linq;
using System.Windows.Forms;

namespace sth1edwv
{
    public partial class TileChooser : Form
    {
        public TileChooser(TileSet tileSet, int selectedIndex)
        {
            InitializeComponent();
            tilePicker1.Items = tileSet.Tiles.Cast<IDrawableBlock>().ToList();
            tilePicker1.SelectedIndex = selectedIndex;
        }

        public Tile SelectedTile => tilePicker1.SelectedItem as Tile;

        private void tilePicker1_SelectionChanged(object sender, IDrawableBlock e)
        {
            DialogResult = e == null ? DialogResult.Cancel : DialogResult.OK;
            Close();
        }
    }
}
