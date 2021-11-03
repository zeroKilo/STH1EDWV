using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace sth1edwv
{
    public partial class BlockChooser : Form
    {
        public int SelectedBlockIndex { get; private set; }
        private readonly List<Block> _blocks;

        private const int BlocksPerRow = 8;

        public BlockChooser(List<Block> blocks, int selectedBlockIndex)
        {
            _blocks = blocks;
            SelectedBlockIndex = selectedBlockIndex;
            InitializeComponent();
            var rows = _blocks.Count / BlocksPerRow;
            if (_blocks.Count % BlocksPerRow != 0)
            {
                ++rows;
            }
            var bmp = new Bitmap(33*BlocksPerRow+1, 33*rows+1);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                for (var i = 0; i < _blocks.Count; ++i)
                {
                    var x = i % BlocksPerRow;
                    var y = i / BlocksPerRow;
                    g.DrawImageUnscaled(_blocks[i].Image, x*33+1, y*33+1);
                }

                if (SelectedBlockIndex >= 0 && SelectedBlockIndex < _blocks.Count)
                {
                    var x = SelectedBlockIndex % BlocksPerRow * 33;
                    var y = SelectedBlockIndex / BlocksPerRow * 33;
                    g.DrawRectangle(SystemPens.Highlight, x, y, 34, 34);
                }
            }

            pb1.Image = bmp;
            pb1.Disposed += (_, _) => pb1.Image.Dispose();
        }

        private void pb1_MouseClick(object sender, MouseEventArgs e)
        {
            var x = e.X / 33;
            var y = e.Y / 33;
            var index = x + y * BlocksPerRow;
            if (index < _blocks.Count)
            {
                SelectedBlockIndex = index;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}