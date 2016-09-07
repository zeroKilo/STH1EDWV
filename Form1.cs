using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace sth1edwv
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.Filter = "*.sms|*.sms";
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Cartridge.Load(d.FileName);
                RefreshAll();
            }
        }

        void RefreshAll()
        {
            listBox1.Items.Clear();
            foreach (Cartridge.MemMapEntry e in Cartridge.labels)
                listBox1.Items.Add("$" + e.offset.ToString("X5") + " " + e.label);
            listBox4.Items.Clear();
            for (int i = 0; i < Cartridge.level_list.Count; i++)
                listBox4.Items.Add(Cartridge.levels[i].label);
            hb1.ByteProvider = new DynamicByteProvider(Cartridge.memory);
            listBox3.Items.Clear();
            for (int i = 0; i < 8; i++)
                listBox3.Items.Add(i);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = listBox1.SelectedIndex;
            if (n == -1)
                return;
            hb1.SelectionStart = Cartridge.labels[n].offset;
            hb1.SelectionLength = 1;
            hb1.ScrollByteIntoView();
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = listBox3.SelectedIndex;
            if (n == -1)
                return;
            Color[] list = Palettes.palettes[n];
            Bitmap bmp = new Bitmap(256, 256);
            for (int y = 0; y < 4; y++)
                for (int x = 0; x < 8; x++)
                    for (int a = 0; a < 16; a++)
                        for (int b = 0; b < 16; b++)
                            bmp.SetPixel(x * 16 + a, y * 16 + b, list[x + y * 8]);
            pb2.Image = bmp;
        }

        private void saveFloorAsBitmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pb3.Image == null)
                return;
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "*.bmp|*.bmp";
            if (d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pb3.Image.Save(d.FileName);
                MessageBox.Show("Done.");
            }
        }

        private void seamlessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            seamlessToolStripMenuItem.Checked = true;
            dontRenderToolStripMenuItem.Checked =
            blockGridToolStripMenuItem.Checked =
            tileGridToolStripMenuItem.Checked = false;
            listBox4.SelectedIndex = -1;
        }

        private void blockGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            blockGridToolStripMenuItem.Checked = true;
            dontRenderToolStripMenuItem.Checked =
            seamlessToolStripMenuItem.Checked =
            tileGridToolStripMenuItem.Checked = false;
            listBox4.SelectedIndex = -1;
        }

        private void tileGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tileGridToolStripMenuItem.Checked = true;
            dontRenderToolStripMenuItem.Checked =
            blockGridToolStripMenuItem.Checked =
            seamlessToolStripMenuItem.Checked = false;
            listBox4.SelectedIndex = -1;
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = listBox4.SelectedIndex;
            if (n == -1)
                return;
            byte mode = 0;
            if (blockGridToolStripMenuItem.Checked) mode = 1;
            if (tileGridToolStripMenuItem.Checked) mode = 2;
            if (dontRenderToolStripMenuItem.Checked) mode = 3;
            if (mode != 3)
                pb3.Image = Cartridge.level_list[n].Render(mode, ref pbar1);
            else
                pb3.Image = null;
            Level l = Cartridge.level_list[n];
            listBox2.Items.Clear();
            for (int i = 0; i < l.tileset.uniRows.Length; i++)
                listBox2.Items.Add(i.ToString("X2") + " : " + l.tileset.uniRows[i].ToString("X2"));
            tv1.Nodes.Clear();
            TreeNode t = new TreeNode(n.ToString("X2") + " : " + Cartridge.levels[n].label);
            t.Nodes.Add(l.ToNode());
            t.Nodes.Add(l.tileset.ToNode());
            t.Expand();
            tv1.Nodes.Add(t);
            listBox5.Items.Clear();
            for (int i = 0; i < l.blockMapping.blockCount; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(i.ToString("X2") + " : ");
                byte[] blockdata = l.blockMapping.blocks[i];
                foreach (byte b in blockdata)
                    sb.Append(b.ToString("X2"));
                listBox5.Items.Add(sb.ToString());
            }
        }

        private void listBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            int n = listBox4.SelectedIndex;
            if (n == -1)
                return;
            int m = listBox2.SelectedIndex;
            if (m == -1)
                return;
            Level l = Cartridge.level_list[n];
            Color[,] tile = l.getTile(m);
            int zoom = 4;
            Bitmap bmp = new Bitmap(8 * zoom, 8 * zoom);
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                    for (int a = 0; a < zoom; a++)
                        for (int b = 0; b < zoom; b++)
                            bmp.SetPixel(x * zoom + a, y * zoom + b, tile[x, y]);
            pb1.Image = bmp;
        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = listBox4.SelectedIndex;
            if (n == -1)
                return;
            int m = listBox5.SelectedIndex;
            if (m == -1)
                return;
            Level l = Cartridge.level_list[n];
            byte[] blockdata = l.blockMapping.blocks[m];
            Bitmap bmp = new Bitmap(36, 36);
            for (int bx = 0; bx < 4; bx++)
                for (int by = 0; by < 4; by++)
                {
                    Color[,] tile = l.getTile(blockdata[bx + by * 4]);
                    for (int x = 0; x < 8; x++)
                        for (int y = 0; y < 8; y++)
                            bmp.SetPixel(bx * 9 + x, by * 9 + y, tile[x, y]);
                }
            pb4.Image = bmp;

        }

        private void pb4_MouseClick(object sender, MouseEventArgs e)
        {
            int n = listBox4.SelectedIndex;
            if (n == -1)
                return;
            int m = listBox5.SelectedIndex;
            if (m == -1)
                return;
            Level l = Cartridge.level_list[n];
            byte[] blockdata = l.blockMapping.blocks[m];
            byte x = (byte)(e.X / 9);
            byte y = (byte)(e.Y / 9);
            TileChooser tc = new TileChooser();
            tc.levelIndex = n;
            tc.blockIndex = m;
            tc.subBlockIndex = x + y * 4;
            tc.tileIndex = blockdata[tc.subBlockIndex];
            int maxY = 16;
            if (l.tileset.tiles.Count == 128)
                maxY = 8;
            Bitmap bmp = new Bitmap(256, 256);
            for (int ty = 0; ty < maxY; ty++)
                for (int tx = 0; tx < 16; tx++)
                {
                    Color[,] tile = l.tileset.tiles[tx + ty * 16];
                    for (int dy = 0; dy < 8; dy++)
                        for (int dx = 0; dx < 8; dx++)
                        {
                            bmp.SetPixel(tx * 16 + dx * 2, ty * 16 + dy * 2, tile[dx, dy]);
                            bmp.SetPixel(tx * 16 + dx * 2 + 1, ty * 16 + dy * 2, tile[dx, dy]);
                            bmp.SetPixel(tx * 16 + dx * 2, ty * 16 + dy * 2 + 1, tile[dx, dy]);
                            bmp.SetPixel(tx * 16 + dx * 2 + 1, ty * 16 + dy * 2 + 1, tile[dx, dy]);
                        }
                }
            tc.pb1.Image = bmp;
            tc.ShowDialog();
            Cartridge.memory[l.blockMappingAddress + m * 16 + tc.subBlockIndex] = (byte)tc.tileIndex;
            Cartridge.ReadLevels();
            RefreshAll();
            listBox4.SelectedIndex = n;
            listBox5.SelectedIndex = m;
        }

        private void dontRenderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dontRenderToolStripMenuItem.Checked = true;
            seamlessToolStripMenuItem.Checked = 
            blockGridToolStripMenuItem.Checked =
            tileGridToolStripMenuItem.Checked = false;
            listBox4.SelectedIndex = -1;
        }

        private void saveROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "*.sms|*.sms";
            if(d.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllBytes(d.FileName, Cartridge.memory);
                MessageBox.Show("Done");
            }
        }
    }
}
