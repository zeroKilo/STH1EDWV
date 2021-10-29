using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Be.Windows.Forms;
using Equin.ApplicationFramework;
using Microsoft.VisualBasic;

namespace sth1edwv
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _solidityImages = new ImageList
            {
                ColorDepth = ColorDepth.Depth4Bit,
                ImageSize = new Size(32, 32),
            };
            _solidityImages.Images.AddStrip(Properties.Resources.SolidityImages);
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

        private void SelectedLevelChanged(object sender, EventArgs e)
        {
            if (!(listBoxLevels.SelectedItem is Level level))
            {
                return;
            }

            RenderLevel();

            tilePicker1.TileSet = level.TileSet;

            treeViewLevelData.Nodes.Clear();
            var t = new TreeNode($"{level}");
            t.Nodes.Add(level.ToNode());
            t.Nodes.Add(level.TileSet.ToNode());
            t.Expand();
            treeViewLevelData.Nodes.Add(t);
            dataGridViewBlocks.DataSource = new BindingListView<Block>(level.BlockMapping.Blocks);
        }

        private void LevelRenderModeChanged(object sender, EventArgs e)
        {
            RenderLevel();
        }

        private void RenderLevel()
        {
            if (listBoxLevels.SelectedItem is Level level)
            {
                pictureBoxRenderedLevel.Image = level.Render(buttonShowObjects.Checked, buttonBlockGaps.Checked, buttonTileGaps.Checked, buttonBlockNumbers.Checked);
            }
        }

        private void tilePicker1_SelectionChanged(object sender, Tile tile)
        {
            pictureBoxTilePreview.Image?.Dispose();
            if (tile == null)
            {
                pictureBoxTilePreview.Image = null;
                return;
            }
            var zoom = pictureBoxTilePreview.Width / 8.0f;
            var bmp = new Bitmap(pictureBoxTilePreview.Width, pictureBoxTilePreview.Width);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.ScaleTransform(zoom, zoom);
                g.DrawImageUnscaled(tile.Image, 0, 0);
            }

            pictureBoxTilePreview.Image = bmp;
        }

        private Block GetSelectedBlock()
        {
            if (listBoxLevels.SelectedItem is Level level && 
                dataGridViewBlocks.SelectedRows.Count != 0)
            {
                return level.BlockMapping.Blocks[dataGridViewBlocks.SelectedRows[0].Index];
            }
            return null;
        }

        private void BlockGridCellEdited(object sender, DataGridViewCellEventArgs e)
        {
            if (!(listBoxLevels.SelectedItem is Level level))
            {
                return;
            }
            var block = GetSelectedBlock();
            if (block == null)
            {
                return;
            }
            // Solidity
            ushort offset = (ushort)(BitConverter.ToUInt16(_cartridge.Memory, 0x3A65 + level.solidityIndex * 2) + block.Index);
            byte data = block.Data;
            _cartridge.Memory[offset] = data;
            _cartridge.ReadLevels();
            var n = listBoxLevels.SelectedIndex;
            RefreshAll();
            listBoxLevels.SelectedIndex = n;
        }

        private void SelectedBlockChanged(object sender, EventArgs e)
        {
            pictureBoxBlockEditor.Image?.Dispose();
            pictureBoxBlockEditor.Image = null;
            var block = GetSelectedBlock();
            if (block == null)
            {
                return;
            }
            const int scale = 4;
            var bmp = new Bitmap(36*scale, 36*scale);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.ScaleTransform(scale, scale);
                g.DrawImageUnscaled(block.Image, 0, 0);
            }

            pictureBoxBlockEditor.Image = bmp;
        }

        private void BlockEditorMouseClick(object sender, MouseEventArgs e)
        {
            if (!(listBoxLevels.SelectedItem is Level level))
            {
                return;
            }
            var block = GetSelectedBlock();
            if (block == null)
            {
                return;
            }
            var x = e.X / 4 / 9;
            var y = e.Y / 4 / 9;
            var subBlockIndex = x + y * 4;
            var tileIndex = block.TileIndices[subBlockIndex];
            using (var tc = new TileChooser(level.TileSet, tileIndex))
            {
                tc.ShowDialog(this);
                _cartridge.Memory[level.blockMappingAddress + level.BlockMapping.Blocks.IndexOf(block) * 16 + subBlockIndex] = (byte)(level.TileSet.Tiles.IndexOf(tc.SelectedTile));
            }

            _cartridge.ReadLevels();
            var n = listBoxLevels.SelectedIndex;
            RefreshAll();
            listBoxLevels.SelectedIndex = n;
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

        private void GameTextDoubleClicked(object sender, EventArgs e)
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

        private void toolStripButtonSaveRenderedLevel_Click(object sender, EventArgs e)
        {
            if (pictureBoxRenderedLevel.Image == null)
            {
                return;
            }

            using (var d = new SaveFileDialog() { Filter = "*.png|*.png" })
            {
                if (d.ShowDialog(this) == DialogResult.OK)
                {
                    pictureBoxRenderedLevel.Image.Save(d.FileName);
                }
            }
        }

        private int _lastX, _lastY;
        private Cartridge _cartridge;
        private readonly ImageList _solidityImages;

        private void pb3_MouseDown(object sender, MouseEventArgs e)
        {
            _lastX = e.X;
            _lastY = e.Y;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists("C:\\Users\\maxim\\Documents\\code\\SMS\\sonic-genesis\\source\\sonic.sms"))
            {
                _cartridge = new Cartridge("C:\\Users\\maxim\\Documents\\code\\SMS\\sonic-genesis\\source\\sonic.sms");
                _richTextBoxGeneralSummary.Text = _cartridge.MakeSummary();
                RefreshAll();
                tabControl1.SelectedTab = tabPage5;
            }

            // We do some grid setup here...
            dataGridViewBlocks.AutoGenerateColumns = false;
            foreach (DataGridViewColumn column in dataGridViewBlocks.Columns)
            {
                switch (column.DataPropertyName)
                {
                    case nameof(Block.SolidityIndex):
                    {
                        column.SortMode = DataGridViewColumnSortMode.Automatic;
                        if (column is DataGridViewComboBoxColumn c)
                        {
                            c.DataSource = Enumerable
                                .Range(0, _solidityImages.Images.Count)
                                .ToList();
                        }
                        break;
                    }
                }
            }
        }

        private void dataGridViewBlocks_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (dataGridViewBlocks.Columns[e.ColumnIndex].DataPropertyName == nameof(Block.SolidityIndex) && e.RowIndex >= 0 && e.Value != null)
            {
                var g = e.Graphics;
                var rect = dataGridViewBlocks.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                e.Paint(e.CellBounds, e.PaintParts & ~DataGridViewPaintParts.ContentForeground);

                var index = (int)e.Value;
                g.DrawImageUnscaled(_solidityImages.Images[index], rect);

                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void dataGridViewBlocks_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is ComboBox combo)
            {
                combo.DrawMode = DrawMode.OwnerDrawFixed;
                combo.ItemHeight = _solidityImages.ImageSize.Height + 1;
                combo.DrawItem -= DrawSolidityComboItem;
                combo.DrawItem += DrawSolidityComboItem;
            }
        }

        private void DrawSolidityComboItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                e.Graphics.DrawImageUnscaled(_solidityImages.Images[e.Index], e.Bounds);
            }
        }

        private void dataGridViewBlocks_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void pb3_MouseUp(object sender, MouseEventArgs e)
        {
            if (!(listBoxLevels.SelectedItem is Level level))
            {
                return;
            }

            var x = e.X;
            var y = e.Y;
            level.AdjustPixelsToTile(ref x, ref y);
            level.AdjustPixelsToTile(ref _lastX, ref _lastY);

            var minX = Math.Min(x, _lastX);
            var maxX = Math.Max(x, _lastX);
            var minY = Math.Min(y, _lastY);
            var maxY = Math.Max(y, _lastY);
            using (var bc = new BlockChooser(level))
            {
                var selection = bc.SelectedBlock = level.Floor.BlockIndices[x + y * level.floorWidth];
                bc.ShowDialog(this);

                // Make a backup
                var temp = new byte[level.Floor.BlockIndices.Length];
                Array.Copy(level.Floor.BlockIndices, temp, level.Floor.BlockIndices.Length);

                if (bc.SelectedBlock != selection && bc.SelectedBlock >= 0)
                {
                    for (var row = minY; row <= maxY; row++)
                    for (var col = minX; col <= maxX; col++)
                    {
                        level.Floor.BlockIndices[col + row * level.floorWidth] = (byte)bc.SelectedBlock;
                    }

                    var newData = level.Floor.CompressData();
                    if (level.floorSize < newData.Length)
                    {
                        MessageBox.Show(this, "Cannot compress level enough to fit into ROM.");
                        level.Floor.BlockIndices = temp;
                        return;
                    }

                    for (var i = 0; i < level.floorSize; i++)
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