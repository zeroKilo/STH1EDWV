﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;
using Microsoft.VisualBasic;

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
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                _cartridge = new Cartridge(d.FileName);
                rtb1.Text = _cartridge.MakeSummary();
                RefreshAll();
            }
        }

        void RefreshAll()
        {
            listBox1.Items.Clear();
            foreach (Cartridge.MemMapEntry e in _cartridge.Labels)
                listBox1.Items.Add($"${e.Offset:X5} {e.Label}");
            listBoxLevels.Items.Clear();
            listBoxLevels.Items.AddRange(_cartridge.LevelList.ToArray<object>());
            hb1.ByteProvider = new DynamicByteProvider(_cartridge.Memory);
            listBoxPalettes.Items.Clear();
            listBoxPalettes.Items.AddRange(_cartridge.Palettes.ToArray<object>());
            listBox6.Items.Clear();
            foreach (GameText text in _cartridge.GameText)
                listBox6.Items.Add(text.text);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = listBox1.SelectedIndex;
            if (n == -1)
                return;
            hb1.SelectionStart = _cartridge.Labels[n].Offset;
            hb1.SelectionLength = 1;
            hb1.ScrollByteIntoView();
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(listBoxPalettes.SelectedItem is Palette palette))
            {
                return;
            }

            pb2.Image = palette.ToImage(256, 128);
        }

        private void saveFloorAsBitmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pb3.Image == null)
                return;
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "*.bmp|*.bmp";
            if (d.ShowDialog(this) == DialogResult.OK)
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
            listBoxLevels.SelectedIndex = -1;
        }

        private void blockGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            blockGridToolStripMenuItem.Checked = true;
            dontRenderToolStripMenuItem.Checked =
                seamlessToolStripMenuItem.Checked =
                    tileGridToolStripMenuItem.Checked = false;
            listBoxLevels.SelectedIndex = -1;
        }

        private void tileGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tileGridToolStripMenuItem.Checked = true;
            dontRenderToolStripMenuItem.Checked =
                blockGridToolStripMenuItem.Checked =
                    seamlessToolStripMenuItem.Checked = false;
            listBoxLevels.SelectedIndex = -1;
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = listBoxLevels.SelectedIndex;
            if (n == -1)
                return;
            byte mode = 0;
            if (blockGridToolStripMenuItem.Checked) mode = 1;
            if (tileGridToolStripMenuItem.Checked) mode = 2;
            if (dontRenderToolStripMenuItem.Checked) mode = 3;
            if (mode != 3)
                pb3.Image = _cartridge.LevelList[n].Render(mode, pbar1);
            else
                pb3.Image = null;
            Level l = _cartridge.LevelList[n];
            listBox2.Items.Clear();
            for (int i = 0; i < l.tileset.Tiles.Count; i++)
            {
                listBox2.Items.Add($"{i:X2}"); // TODO  : {l.tileset.UniqueRows[i]:X2}");
            }

            tv1.Nodes.Clear();
            TreeNode t = new TreeNode($"{n:X2} : {_cartridge.LevelList[n]}");
            t.Nodes.Add(l.ToNode());
            t.Nodes.Add(l.tileset.ToNode());
            t.Expand();
            tv1.Nodes.Add(t);
            listBox5.Items.Clear();
            for (int i = 0; i < l.blockMapping.blockCount; i++)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{i:X2} : ");
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
            int n = listBoxLevels.SelectedIndex;
            if (n == -1)
                return;
            int m = listBox2.SelectedIndex;
            if (m == -1)
                return;
            var level = _cartridge.LevelList[n];
            var tile = level.GetTile(m);
            int zoom = 32;
            var bmp = new Bitmap(8 * zoom, 8 * zoom);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(tile.Image, 0, 0, bmp.Width, bmp.Height);
            }

            pb1.Image = bmp;
        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = listBoxLevels.SelectedIndex;
            if (n == -1)
                return;
            int m = listBox5.SelectedIndex;
            if (m == -1)
                return;
            Level l = _cartridge.LevelList[n];
            byte[] blockdata = l.blockMapping.blocks[m];
            Bitmap bmp = new Bitmap(36, 36);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                for (int bx = 0; bx < 4; bx++)
                for (int by = 0; by < 4; by++)
                {
                    var tile = l.GetTile(blockdata[bx + by * 4]);
                    g.DrawImage(tile.Image, new Rectangle(bx * 9, by * 9, 8, 8));
                }
            }

            pb4.Image = bmp;
            textBox1.Text = blockdata[16].ToString("X2");
        }

        private void pb4_MouseClick(object sender, MouseEventArgs e)
        {
            int n = listBoxLevels.SelectedIndex;
            if (n == -1)
                return;
            int m = listBox5.SelectedIndex;
            if (m == -1)
                return;
            Level l = _cartridge.LevelList[n];
            byte[] blockdata = l.blockMapping.blocks[m];
            byte x = (byte)(e.X / 9);
            byte y = (byte)(e.Y / 9);
            TileChooser tc = new TileChooser();
            tc.levelIndex = n;
            tc.blockIndex = m;
            tc.subBlockIndex = x + y * 4;
            tc.tileIndex = blockdata[tc.subBlockIndex];
            int maxY = 16;
            if (l.tileset.Tiles.Count == 128)
                maxY = 8;
            var bmp = new Bitmap(256, 256);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                for (int ty = 0; ty < maxY; ty++)
                for (int tx = 0; tx < 16; tx++)
                {
                    // TODO store this in the tileset?
                    var tile = l.tileset.Tiles[tx + ty * 16];
                    g.DrawImage(tile.Image, new Rectangle(tx * 16, ty * 16, 16, 16));
                }
            }

            tc.pb1.Image = bmp;
            tc.ShowDialog(this);
            _cartridge.Memory[l.blockMappingAddress + m * 16 + tc.subBlockIndex] = (byte)tc.tileIndex;
            _cartridge.ReadLevels();
            RefreshAll();
            listBoxLevels.SelectedIndex = n;
            listBox5.SelectedIndex = m;
        }

        private void dontRenderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dontRenderToolStripMenuItem.Checked = true;
            seamlessToolStripMenuItem.Checked =
                blockGridToolStripMenuItem.Checked =
                    tileGridToolStripMenuItem.Checked = false;
            listBoxLevels.SelectedIndex = -1;
        }

        private void saveROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog();
            d.Filter = "*.sms|*.sms";
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                File.WriteAllBytes(d.FileName, _cartridge.Memory);
                MessageBox.Show("Done");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int n = listBoxLevels.SelectedIndex;
            if (n == -1)
                return;
            int m = listBox5.SelectedIndex;
            if (m == -1)
                return;
            Level l = _cartridge.LevelList[n];
            byte[] blockdata = l.blockMapping.blocks[m];
            ushort offset = (ushort)(BitConverter.ToUInt16(_cartridge.Memory, 0x3A65 + l.solidityIndex * 2) + m);
            byte data = Convert.ToByte(textBox1.Text, 16);
            _cartridge.Memory[offset] = data;
            _cartridge.ReadLevels();
            RefreshAll();
            listBoxLevels.SelectedIndex = n;
            listBox5.SelectedIndex = m;
        }

        private void tv1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode t = tv1.SelectedNode;
            int n = listBoxLevels.SelectedIndex;
            if (t == null || t.Parent == null || t.Parent.Text != "Objects" || n == -1)
                return;
            ObjectChooser objc = new ObjectChooser();
            objc.comboBox1.Items.Clear();
            objc.comboBox1.Items.AddRange(LevelObjectSet.LevelObject.objNames.Values.ToArray<object>());
            LevelObjectSet.LevelObject obj = _cartridge.LevelList[n].objSet.objs[t.Index];
            if (LevelObjectSet.LevelObject.objNames.Keys.Contains(obj.type))
            {
                string s = LevelObjectSet.LevelObject.objNames[obj.type];
                for (int i = 0; i < objc.comboBox1.Items.Count; i++)
                    if (objc.comboBox1.Items[i].ToString() == s)
                    {
                        objc.comboBox1.SelectedIndex = i;
                        break;
                    }
            }

            objc.textBox1.Text = obj.x.ToString();
            objc.textBox2.Text = obj.y.ToString();
            objc.textBox3.Text = obj.type.ToString();
            objc.ShowDialog(this);
            if (objc.exitOk)
            {
                int offset = _cartridge.LevelList[n].offsetObjectLayout + 0x15581;
                offset += t.Index * 3;
                try
                {
                    byte x, y, tp;
                    x = Convert.ToByte(objc.textBox1.Text);
                    y = Convert.ToByte(objc.textBox2.Text);
                    tp = Convert.ToByte(objc.textBox3.Text);
                    _cartridge.Memory[offset] = tp;
                    _cartridge.Memory[offset + 1] = x;
                    _cartridge.Memory[offset + 2] = y;
                    _cartridge.ReadLevels();
                    RefreshAll();
                    listBoxLevels.SelectedIndex = n;
                }
                catch
                {
                }
            }
        }

        private void listBox6_DoubleClick(object sender, EventArgs e)
        {
            int n = listBox6.SelectedIndex;
            if (n == -1)
                return;
            GameText text = _cartridge.GameText[n];
            string input = Interaction.InputBox(
                "Please enter new game text, format X;Y;TEXT\n(TEXT can be 'A'-'Z', ' ' and '©')", "Edit game text",
                $"{text.x};{text.y};{text.text}");
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
                    text.WriteToMemory(_cartridge, 0x122D + n * 0xF, x, y, parts[2].Trim());
                }
                else
                {
                    text.WriteToMemory(_cartridge, 0x197E + (n - 6) * 0x10, x, y, parts[2].Trim(), 13);
                }

                _cartridge.ReadGameText();
                _cartridge.ReadLevels();
                RefreshAll();
            }
        }

        int _lastX, _lastY;
        private Cartridge _cartridge;

        private void pb3_MouseDown(object sender, MouseEventArgs e)
        {
            int n = listBoxLevels.SelectedIndex;
            if (n == -1)
                return;
            if (!blockGridToolStripMenuItem.Checked)
            {
                MessageBox.Show("Please select block as render mode to edit blocks");
                return;
            }

            _lastX = e.X / 33;
            _lastY = e.Y / 33;
        }

        private void pb3_MouseUp(object sender, MouseEventArgs e)
        {
            int n = listBoxLevels.SelectedIndex;
            if (n == -1)
                return;
            if (!blockGridToolStripMenuItem.Checked)
            {
                MessageBox.Show("Please select block as render mode to edit blocks");
                return;
            }

            int x = e.X / 33;
            int y = e.Y / 33;
            int minX = x < _lastX ? x : _lastX;
            int maxX = x > _lastX ? x : _lastX;
            int minY = y < _lastY ? y : _lastY;
            int maxY = y > _lastY ? y : _lastY;
            Level l = _cartridge.LevelList[n];
            Blockchooser bc = new Blockchooser(_cartridge);
            bc.levelIndex = n;
            int selection = bc.selectedBlock = l.floor.data[x + y * l.floorWidth];
            bc.ShowDialog(this);
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

                for (int i = 0; i < l.floorSize; i++)
                    if (i < newData.Length)
                        _cartridge.Memory[l.floorAddress + i] = newData[i];
                    else
                        _cartridge.Memory[l.floorAddress + i] = 1;
                _cartridge.ReadLevels();
                RefreshAll();
                listBoxLevels.SelectedIndex = n;
            }
        }
    }
}