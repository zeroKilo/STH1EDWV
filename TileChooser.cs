using System.Windows.Forms;

namespace sth1edwv
{
    public partial class TileChooser : Form
    {
        public int levelIndex;
        public int blockIndex;
        public int subBlockIndex;
        public int tileIndex;

        public TileChooser()
        {
            InitializeComponent();
        }

        private void pb1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / 16;
            int y = e.Y / 16;
            tileIndex = x + y * 16;
            this.Close();
        }
    }
}
