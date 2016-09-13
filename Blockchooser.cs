using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sth1edwv
{
    public partial class Blockchooser : Form
    {
        public int levelIndex;
        public int selectedBlock;

        public Blockchooser()
        {
            InitializeComponent();
        }

        private void Blockchooser_Load(object sender, EventArgs e)
        {
            Level l = Cartridge.level_list[levelIndex];
            Bitmap bmp = new Bitmap(528, 528);
            int index;
            for(int by=0; by<16; by++)
                for (int bx = 0; bx < 16; bx++)
                {
                    index = bx + by * 16;
                    if (index >= l.blockMapping.blockCount)
                    {
                        by = 16;
                        break;
                    }
                    for (int y = 0; y < 32; y++)
                        for (int x = 0; x < 32; x++)
                            bmp.SetPixel(bx * 33 + x, by * 33 + y, l.blockMapping.imagedata[index][x, y]);
                }
            pb1.Image = bmp;
        }

        private void pb1_MouseClick(object sender, MouseEventArgs e)
        {
            Level l = Cartridge.level_list[levelIndex];
            int x = e.X / 33;
            int y = e.Y / 33;
            int index = x + y * 16;
            if (index < l.blockMapping.blockCount)
                selectedBlock = index;
            this.Close();
        }
    }
}
