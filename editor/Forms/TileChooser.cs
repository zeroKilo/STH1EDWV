using System.Windows.Forms;
using sth1edwv.Controls;
using sth1edwv.GameObjects;

namespace sth1edwv.Forms
{
    public partial class TileChooser : Form
    {
        private readonly bool _inConstructor;

        public TileChooser(TileSet tileSet, int selectedIndex, Palette palette)
        {
            InitializeComponent();
            tilePicker1.SetData(tileSet.Tiles, palette);
            _inConstructor = true;
            tilePicker1.SelectedIndex = selectedIndex;
            _inConstructor = false;
        }

        public Tile SelectedTile => tilePicker1.SelectedItem as Tile;

        private void tilePicker1_SelectionChanged(object sender, IDrawableBlock e)
        {
            if (_inConstructor)
            {
                return;
            }
            DialogResult = e == null ? DialogResult.Cancel : DialogResult.OK;
            Close();
        }
    }
}
