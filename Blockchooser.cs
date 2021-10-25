using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace sth1edwv
{
    public partial class Blockchooser : Form
    {
        public int SelectedBlock { get; set; }
        private readonly Level _level;

        public Blockchooser(Level level)
        {
            _level = level;
            InitializeComponent();
        }

        private void Blockchooser_Load(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(528, 528);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                for (int i = 0; i < _level.blockMapping.blocks.Count; ++i)
                {
                    var x = i % 16;
                    var y = i / 16;
                    g.DrawImageUnscaled(_level.blockMapping.Images[i], x*33, y*33);
                }
            }

            pb1.Image = bmp;
        }

        private void pb1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / 33;
            int y = e.Y / 33;
            int index = x + y * 16;
            if (index < _level.blockMapping.blocks.Count)
            {
                SelectedBlock = index;
            }
            Close();
        }
    }
}