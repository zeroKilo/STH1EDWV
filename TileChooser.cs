using System.Drawing;
using System.Drawing.Drawing2D;
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
            var bmp = new Bitmap(pb1.Width, pb1.Height);
            var tilesPerRow = pb1.Width / (8 * TileScale);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.ScaleTransform(TileScale, TileScale);
                for (int i = 0; i < tileSet.Tiles.Count; ++i)
                {
                    var x = i % tilesPerRow * 8;
                    var y = i / tilesPerRow * 8;
                    g.DrawImageUnscaled(tileSet.Tiles[i].Image, x, y);
                }
            }
            pb1.Image = bmp;
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
