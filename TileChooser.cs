using System.Windows.Forms;

namespace sth1edwv
{
    public partial class TileChooser : Form
    {
        public TileChooser(TileSet tileSet, int selectedIndex)
        {
            InitializeComponent();
            tilePicker1.TileSet = tileSet;
            tilePicker1.SelectedIndex = selectedIndex;
        }

        public Tile SelectedTile => tilePicker1.SelectedTile;

        private void tilePicker1_SelectionChanged(object sender, Tile e)
        {
            Close();
        }
    }
}
