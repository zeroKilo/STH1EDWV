using System.Windows.Forms;

namespace sth1edwv
{
    public partial class TileChooser : Form
    {
        public TileChooser(TileSet tileSet, int selectedIndex, Palette palette)
        {
            InitializeComponent();
            tilePicker1.SetData(tileSet.Tiles, palette);
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
