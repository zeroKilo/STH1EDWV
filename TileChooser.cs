using System.Windows.Forms;

namespace sth1edwv
{
    public partial class TileChooser : Form
    {
        private const int TileScale = 2;
        public int TileIndex { get; set; }

        public TileChooser(TileSet tileSet)
        {
            InitializeComponent();

            // Draw the tiles in a grid
            pb1.Image = tileSet.getImage(pb1.Width);
        }

        private void pb1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / 16;
            int y = e.Y / 16;
            TileIndex = x + y * 16;
            Close();
        }
    }
}
