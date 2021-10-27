using System;
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
            using (var d = new OpenFileDialog{Filter = "*.sms|*.sms"})
            {
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    _cartridge?.Dispose();
                    _cartridge = new Cartridge(d.FileName);
                    _richTextBoxGeneralSummary.Text = _cartridge.MakeSummary();
                    RefreshAll();
                }
            }
        }

        private void RefreshAll()
        {
            listBoxMemoryLocations.Items.Clear();
            listBoxMemoryLocations.Items.AddRange(_cartridge.Labels.ToArray<object>());
            listBoxLevels.Items.Clear();
            listBoxLevels.Items.AddRange(_cartridge.Levels.ToArray<object>());
            hexViewer.ByteProvider = new DynamicByteProvider(_cartridge.Memory);
            listBoxPalettes.Items.Clear();
            listBoxPalettes.Items.AddRange(_cartridge.Palettes.ToArray<object>());
            listBoxGameText.Items.Clear();
            foreach (var text in _cartridge.GameText)
            {
                listBoxGameText.Items.Add(text);
            }
        }

        private void ListBoxMemoryLocationsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(listBoxMemoryLocations.SelectedItem is Cartridge.MemMapEntry label))
            {
                return;
            }
            hexViewer.SelectionStart = label.Offset;
            hexViewer.SelectionLength = 1;
            hexViewer.ScrollByteIntoView();
        }

        private void ListBoxPalettesSelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBoxPalette.Image?.Dispose();
            if (!(listBoxPalettes.SelectedItem is Palette palette))
            {
                pictureBoxPalette.Image = null;
                return;
            }

            pictureBoxPalette.Image = palette.ToImage(256, 128);
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
                MessageBox.Show(this, "Done.");
            }
        }

        private void seamlessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem child in renderToolStripMenuItem.DropDownItems)
            {
                child.Checked = child == sender;
            }
            listBoxLevels.SelectedItem = null;
        }

        private void blockGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem child in renderToolStripMenuItem.DropDownItems)
            {
                child.Checked = child == sender;
            }
            listBoxLevels.SelectedItem = null;
        }

        private void tileGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem child in renderToolStripMenuItem.DropDownItems)
            {
                child.Checked = child == sender;
            }
            listBoxLevels.SelectedItem = null;
        }

        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(listBoxLevels.SelectedItem is Level level))
            {
                return;
            }
            byte mode = 0;
            if (blockGridToolStripMenuItem.Checked) mode = 1;
            if (tileGridToolStripMenuItem.Checked) mode = 2;
            if (dontRenderToolStripMenuItem.Checked) mode = 3;
            pb3.Image?.Dispose();
            if (mode == 3)
            {
                pb3.Image = null;
            }
            else
            {
                pb3.Image = level.Render(mode, pbar1);
            }

            tilePicker1.TileSet = level.TileSet;

            treeViewLevelData.Nodes.Clear();
            var t = new TreeNode($"{level}");
            t.Nodes.Add(level.ToNode());
            t.Nodes.Add(level.TileSet.ToNode());
            t.Expand();
            treeViewLevelData.Nodes.Add(t);
            listBox5.Items.Clear();
            for (int i = 0; i < level.BlockMapping.Blocks.Count; i++)
            {
                var sb = new StringBuilder();
                sb.Append($"{i:X2} : ");
                var block = level.BlockMapping.Blocks[i];
                sb.Append(block);
                listBox5.Items.Add(sb.ToString());
            }
        }

        private void tilePicker1_SelectionChanged(object sender, Tile tile)
        {
            pb1.Image?.Dispose();
            if (tile == null)
            {
                pb1.Image = null;
                return;
            }
            float zoom = pb1.Width / 8.0f;
            var bmp = new Bitmap(pb1.Width, pb1.Width);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.ScaleTransform(zoom, zoom);
                g.DrawImageUnscaled(tile.Image, 0, 0);
            }

            pb1.Image = bmp;
        }

        private void listBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(listBoxLevels.SelectedItem is Level level))
            {
                return;
            }
            int m = listBox5.SelectedIndex;
            if (m == -1)
                return;
            var block = level.BlockMapping.Blocks[m];
            var scale = 4;
            var bmp = new Bitmap(36*scale, 36*scale);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.ScaleTransform(scale, scale);
                for (int bx = 0; bx < 4; bx++)
                for (int by = 0; by < 4; by++)
                {
                    var tile = level.GetTile(block.TileIndices[bx + by * 4]);
                    g.DrawImageUnscaled(tile.Image, bx * 9, by * 9);
                }
            }

            pb4.Image = bmp;
            textBox1.Text = block.SolidityIndex.ToString("X2");
        }

        private void pb4_MouseClick(object sender, MouseEventArgs e)
        {
            if (!(listBoxLevels.SelectedItem is Level level))
            {
                return;
            }
            int m = listBox5.SelectedIndex;
            if (m == -1)
                return;
            var block = level.BlockMapping.Blocks[m];
            var x = e.X / 4 / 9;
            var y = e.Y / 4 / 9;
            var subBlockIndex = x + y * 4;
            var tileIndex = block.TileIndices[subBlockIndex];
            using (var tc = new TileChooser(level.TileSet, tileIndex))
            {
                tc.ShowDialog(this);
                _cartridge.Memory[level.blockMappingAddress + m * 16 + subBlockIndex] = (byte)(level.TileSet.Tiles.IndexOf(tc.SelectedTile));
            }

            _cartridge.ReadLevels();
            var n = listBoxLevels.SelectedIndex;
            RefreshAll();
            listBoxLevels.SelectedIndex = n;
            listBox5.SelectedIndex = m;
        }

        private void dontRenderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem child in renderToolStripMenuItem.DropDownItems)
            {
                child.Checked = child == sender;
            }
            listBoxLevels.SelectedItem = null;
        }

        private void saveROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog { Filter = "*.sms|*.sms" })
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    File.WriteAllBytes(dialog.FileName, _cartridge.Memory);
                    MessageBox.Show(this, "Done");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!(listBoxLevels.SelectedItem is Level level))
            {
                return;
            }
            int m = listBox5.SelectedIndex;
            if (m == -1)
                return;
            ushort offset = (ushort)(BitConverter.ToUInt16(_cartridge.Memory, 0x3A65 + level.solidityIndex * 2) + m);
            byte data = Convert.ToByte(textBox1.Text, 16);
            _cartridge.Memory[offset] = data;
            _cartridge.ReadLevels();
            var n = listBoxLevels.SelectedIndex;
            RefreshAll();
            listBoxLevels.SelectedIndex = n;
            listBox5.SelectedIndex = m;
        }

        private void tv1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = treeViewLevelData.SelectedNode;
            if (node?.Parent == null || node.Parent.Text != "Objects")
            {
                return;
            }
            if (!(listBoxLevels.SelectedItem is Level level))
            {
                return;
            }

            using (var objc = new ObjectChooser())
            {
                objc.comboBox1.Items.Clear();
                objc.comboBox1.Items.AddRange(LevelObjectSet.LevelObject.objNames.Values.ToArray<object>());
                LevelObjectSet.LevelObject obj = level.ObjSet.objs[node.Index];
                if (LevelObjectSet.LevelObject.objNames.Keys.Contains(obj.type))
                {
                    string s = LevelObjectSet.LevelObject.objNames[obj.type];
                    for (int i = 0; i < objc.comboBox1.Items.Count; i++)
                    {
                        if (objc.comboBox1.Items[i].ToString() == s)
                        {
                            objc.comboBox1.SelectedIndex = i;
                            break;
                        }
                    }
                }

                objc.textBox1.Text = obj.x.ToString();
                objc.textBox2.Text = obj.y.ToString();
                objc.textBox3.Text = obj.type.ToString();
                objc.ShowDialog(this);
                if (objc.exitOk)
                {
                    int offset = level.offsetObjectLayout + 0x15581;
                    offset += node.Index * 3;
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
                        var n = listBoxLevels.SelectedIndex ;
                        RefreshAll();
                        listBoxLevels.SelectedIndex = n;
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void listBox6_DoubleClick(object sender, EventArgs e)
        {
            if (!(listBoxGameText.SelectedItem is GameText text))
            {
                return;
            }
            string input = Interaction.InputBox(
                "Please enter new game text, format X;Y;TEXT\n(TEXT can be 'A'-'Z', ' ' and '©')", "Edit game text",
                text.AsSerialized);
            if (input != "")
            {
                try
                {
                    text.Deserialize(input.ToUpperInvariant());
                }
                catch (Exception exception)
                {
                    MessageBox.Show(this, exception.Message);
                }

                // This makes the listbox re-get the text
                listBoxGameText.Items[listBoxGameText.SelectedIndex] = text;
            }
        }

        private int _lastX, _lastY;
        private Cartridge _cartridge;

        private void pb3_MouseDown(object sender, MouseEventArgs e)
        {
            if (!blockGridToolStripMenuItem.Checked)
            {
                MessageBox.Show(this, "Please select block as render mode to edit blocks");
                return;
            }

            _lastX = e.X / 33;
            _lastY = e.Y / 33;
        }

        private void pb3_MouseUp(object sender, MouseEventArgs e)
        {
            if (!blockGridToolStripMenuItem.Checked)
            {
                MessageBox.Show(this, "Please select block as render mode to edit blocks");
                return;
            }

            if (!(listBoxLevels.SelectedItem is Level level))
            {
                return;
            }
            int x = e.X / 33;
            int y = e.Y / 33;
            int minX = x < _lastX ? x : _lastX;
            int maxX = x > _lastX ? x : _lastX;
            int minY = y < _lastY ? y : _lastY;
            int maxY = y > _lastY ? y : _lastY;
            using (var bc = new BlockChooser(level))
            {
                int selection = bc.SelectedBlock = level.Floor.BlockIndices[x + y * level.floorWidth];
                bc.ShowDialog(this);
                byte[] temp = new byte[level.Floor.BlockIndices.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = level.Floor.BlockIndices[i];
                }

                if (bc.SelectedBlock != selection)
                {
                    for (int row = minY; row <= maxY; row++)
                    for (int col = minX; col <= maxX; col++)
                    {
                        level.Floor.BlockIndices[col + row * level.floorWidth] = (byte)bc.SelectedBlock;
                    }

                    byte[] newData = level.Floor.CompressData();
                    if (level.floorSize < newData.Length)
                    {
                        MessageBox.Show(this, "Cannot compress level enough to fit into ROM.");
                        level.Floor.BlockIndices = temp;
                        return;
                    }

                    for (int i = 0; i < level.floorSize; i++)
                    {
                        if (i < newData.Length)
                        {
                            _cartridge.Memory[level.floorAddress + i] = newData[i];
                        }
                        else
                        {
                            _cartridge.Memory[level.floorAddress + i] = 1;
                        }
                    }

                    _cartridge.ReadLevels();
                    var n = listBoxLevels.SelectedIndex;
                    RefreshAll();
                    listBoxLevels.SelectedIndex = n;
                }
            }
        }
    }
}