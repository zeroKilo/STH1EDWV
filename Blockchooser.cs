using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace sth1edwv
{
    public partial class BlockChooser : Form
    {
        public int SelectedBlock { get; set; }
        private readonly Level _level;

        public BlockChooser(Level level)
        {
            _level = level;
            InitializeComponent();
        }

        private void Blockchooser_Load(object sender, EventArgs e)
        {
            var bmp = new Bitmap(528, 528);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                for (int i = 0; i < _level.blockMapping.Blocks.Count; ++i)
                {
                    var x = i % 16;
                    var y = i / 16;
                    g.DrawImageUnscaled(_level.blockMapping.Blocks[i].Image, x*33, y*33);
                }
            }

            pb1.Image = bmp;
        }

        private void pb1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / 33;
            int y = e.Y / 33;
            int index = x + y * 16;
            if (index < _level.blockMapping.Blocks.Count)
            {
                SelectedBlock = index;
            }
            Close();
        }
    }
}