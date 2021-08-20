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
                rtb1.Text = Cartridge.MakeSummary();
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
            listBox6.Items.Clear();
            foreach (GameText text in Cartridge.gameText)
                listBox6.Items.Add(text.text);
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
                int count = 0;
                foreach (byte b in blockdata)
                    if (count++ < 16)
                        sb.Append(b.ToString("X2"));
                sb.Append(" - ");
                sb.Append(Convert.ToString(blockdata[16], 2).PadLeft(8, '0'));
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
            textBox1.Text = blockdata[16].ToString("X2");

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

        private void button1_Click(object sender, EventArgs e)
        {
            int n = listBox4.SelectedIndex;
            if (n == -1)
                return;
            int m = listBox5.SelectedIndex;
            if (m == -1)
                return;
            Level l = Cartridge.level_list[n];
            byte[] blockdata = l.blockMapping.blocks[m];
            ushort offset = (ushort)(BitConverter.ToUInt16(Cartridge.memory, 0x3A65 + l.solidityIndex * 2) + m);
            byte data = (byte)Convert.ToByte(textBox1.Text, 16);
            Cartridge.memory[offset] = data;
            Cartridge.ReadLevels();
            RefreshAll();
            listBox4.SelectedIndex = n;
            listBox5.SelectedIndex = m;
        }

        private void tv1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode t = tv1.SelectedNode;
            int n = listBox4.SelectedIndex;
            if (t == null || t.Parent == null || t.Parent.Text != "Objects" || n == -1)
                return;
            ObjectChooser objc = new ObjectChooser();
            objc.comboBox1.Items.Clear();
            objc.comboBox1.Items.AddRange(LevelObjectSet.LevelObject.objNames.Values.ToArray());
            LevelObjectSet.LevelObject obj = Cartridge.level_list[n].objSet.objs[t.Index];
            if (LevelObjectSet.LevelObject.objNames.Keys.Contains(obj.type))
            {
                string s = LevelObjectSet.LevelObject.objNames[obj.type];
                for(int i=0;i<objc.comboBox1.Items.Count;i++)
                    if (objc.comboBox1.Items[i].ToString() == s)
                    {
                        objc.comboBox1.SelectedIndex = i;
                        break;
                    }
            }
            objc.textBox1.Text = obj.X.ToString();
            objc.textBox2.Text = obj.Y.ToString();
            objc.textBox3.Text = obj.type.ToString();
            objc.ShowDialog();
            if (objc._exit_ok)
            {
                int offset = Cartridge.level_list[n].offsetObjectLayout + 0x15581;
                offset += t.Index * 3;
                try
                {
                    byte x, y, tp;
                    x = Convert.ToByte(objc.textBox1.Text);
                    y = Convert.ToByte(objc.textBox2.Text);
                    tp = Convert.ToByte(objc.textBox3.Text);
                    Cartridge.memory[offset] = tp;
                    Cartridge.memory[offset + 1] = x;
                    Cartridge.memory[offset + 2] = y;
                    Cartridge.ReadLevels();
                    RefreshAll();
                    listBox4.SelectedIndex = n;
                }
                catch { }
            }
        }

        private void listBox6_DoubleClick(object sender, EventArgs e)
        {
            int n = listBox6.SelectedIndex;
            if (n == -1)
                return;
            GameText text = Cartridge.gameText[n];
            string input = Microsoft.VisualBasic.Interaction.InputBox("Please enter new game text, format X;Y;TEXT\n(TEXT can be 'A'-'Z', ' ' and '©')", "Edit game text", text.X + ";" + text.Y + ";" + text.text);
            if (input != "")
            {
                input = input.ToUpper();
                string[] parts = input.Split(';');
                if (parts.Length != 3)
                {
                    MessageBox.Show("Please check your input, there should be 3 parts seperated by a ';' !");
                    return;
                }
                bool allOk = true;
                foreach (char c in parts[2])
                    if (!GameText.lowChars.ContainsValue(c))
                    {
                        allOk = false;
                        break;
                    }
                if (!allOk)
                {
                    MessageBox.Show("Please check your input, there was an invalid character!");
                    return;
                }
                if ((n < 6 && parts[2].Trim().Length > 12) ||
                    (n >= 6 && parts[2].Trim().Length > 13)) 
                {
                    MessageBox.Show("Your input is too long!");
                    return;
                }
                byte x, y;
                try
                {
                    x = Convert.ToByte(parts[0].Trim());
                    y = Convert.ToByte(parts[1].Trim());
                }
                catch
                {
                    MessageBox.Show("Your input has wrong coordinates!");
                    return;
                }
                if (n < 6)
                {
                    text.WriteToMemory(0x122D + n * 0xF, x, y, parts[2].Trim());
                }
                else
                {
                    text.WriteToMemory(0x197E + (n - 6) * 0x10, x, y, parts[2].Trim(), 13);
                }
                Cartridge.ReadGameText();
                Cartridge.ReadLevels();
                RefreshAll();
            }
        }

        int lastX, lastY;
        private void pb3_MouseDown(object sender, MouseEventArgs e)
        {

            int n = listBox4.SelectedIndex;
            if (n == -1)
                return;
            if (!blockGridToolStripMenuItem.Checked)
            {
                MessageBox.Show("Please select block as render mode to edit blocks");
                return;
            }
            lastX = e.X / 33;
            lastY = e.Y / 33;
        }

        private void pb3_MouseUp(object sender, MouseEventArgs e)
        {
            int n = listBox4.SelectedIndex;
            if (n == -1)
                return;
            if (!blockGridToolStripMenuItem.Checked)
            {
                MessageBox.Show("Please select block as render mode to edit blocks");
                return;
            }
            int x = e.X / 33;
            int y = e.Y / 33;
            int minX = x < lastX ? x : lastX;
            int maxX = x > lastX ? x : lastX;
            int minY = y < lastY ? y : lastY;
            int maxY = y > lastY ? y : lastY;
            Level l = Cartridge.level_list[n];
            Blockchooser bc = new Blockchooser();
            bc.levelIndex = n;
            int selection = bc.selectedBlock = l.floor.data[x + y * l.floorWidth];
            bc.ShowDialog();
            byte[] temp = new byte[l.floor.data.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = l.floor.data[i];
            if (bc.selectedBlock != selection)
            {
                for (int row = minY; row <= maxY; row++)
                    for (int col = minX; col <= maxX; col++)
                        l.floor.data[col + row * l.floorWidth] = (byte)bc.selectedBlock;
                byte[] newData = l.floor.CompressData(l);
                if (l.floorSize < newData.Length)
                {
                    MessageBox.Show("Cannot compress level enough to fit into ROM.");
                    l.floor.data = temp;
                    return;
                }
                else
                {
                    for (int i = 0; i < l.floorSize; i++)
                        if (i < newData.Length)
                            Cartridge.memory[l.floorAddress + i] = newData[i];
                        else
                            Cartridge.memory[l.floorAddress + i] = 1;
                    Cartridge.ReadLevels();
                    RefreshAll();
                    listBox4.SelectedIndex = n;
                }
            }
        }
    }
}
