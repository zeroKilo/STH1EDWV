using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Equin.ApplicationFramework;
using Microsoft.VisualBasic;
using sth1edwv.Properties;

namespace sth1edwv
{
    public sealed partial class Form1 : Form
    {
        private int _lastX, _lastY;
        private Cartridge _cartridge;
        private readonly ImageList _solidityImages;
        private int _blockEditorTileSize;

        public Form1()
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
            _solidityImages = new ImageList
            {
                ColorDepth = ColorDepth.Depth16Bit,
                ImageSize = new Size(32, 32)
            };
            _solidityImages.Images.AddStrip(Resources.SolidityImages);

            typeof(DataGridView).InvokeMember("DoubleBuffered", 
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, 
                null,
                dataGridViewBlocks, 
                new object[] { true });
        }

        private void openROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var d = new OpenFileDialog{Filter = "*.sms|*.sms"};
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                _cartridge?.Dispose();
                LoadFile(d.FileName);
            }
        }

        private void LoadFile(string filename)
        {
            _cartridge = new Cartridge(filename);
            listBoxLevels.Items.Clear();
            listBoxLevels.Items.AddRange(_cartridge.Levels.ToArray<object>());
            listBoxPalettes.Items.Clear();
            listBoxPalettes.Items.AddRange(_cartridge.Palettes.ToArray<object>());
            listBoxGameText.Items.Clear();
            listBoxGameText.Items.AddRange(_cartridge.GameText.ToArray<object>());
            listBoxScreens.Items.Clear();
            listBoxScreens.Items.AddRange(_cartridge.Screens.ToArray<object>());

            // Add or replace filename in title bar
            Text = $"{Regex.Replace(Text, " \\[.+\\]$", "")} [{Path.GetFileName(filename)}]";

            // Trigger the selected level changed event
            SelectedLevelChanged(null, null);

            UpdateFloorSpace();
            UpdateTileSetSpace();
        }

        private void ListBoxPalettesSelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBoxPalette.Image?.Dispose();
            if (listBoxPalettes.SelectedItem is not Palette palette)
            {
                pictureBoxPalette.Image = null;
                return;
            }

            pictureBoxPalette.Image = palette.ToImage(512);
        }

        private void SelectedLevelChanged(object sender, EventArgs e)
        {
            RenderLevel();

            var level = listBoxLevels.SelectedItem as Level;

            tilePicker1.Items = level?.TileSet.Tiles.Cast<IDrawableBlock>().ToList();

            LoadLevelData();

            propertyGridLevel.SelectedObject = level;

            level?.BlockMapping.UpdateUsageForLevel(level);
            level?.BlockMapping.UpdateGlobalUsage(_cartridge.Levels);

            dataGridViewBlocks.DataSource = level == null ? null : new BindingListView<Block>(level.BlockMapping.Blocks);
            dataGridViewBlocks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        }

        private void LoadLevelData()
        {
            treeViewLevelData.Nodes.Clear();
            if (listBoxLevels.SelectedItem is not Level level)
            {
                return;
            }
            var t = new TreeNode($"{level}");
            t.Nodes.Add(level.ToNode());
            t.Nodes.Add(level.TileSet.ToNode());
            t.Expand();
            treeViewLevelData.Nodes.Add(t);
        }

        private void LevelRenderModeChanged(object sender, EventArgs e)
        {
            RenderLevel();
        }

        private void RenderLevel()
        {
            if (tabControlLevel.SelectedTab == tabPageLayout && listBoxLevels.SelectedItem is Level level)
            {
                pictureBoxRenderedLevel.Image?.Dispose();
                pictureBoxRenderedLevel.Image = level.Render(buttonShowObjects.Checked, buttonBlockGaps.Checked,
                    buttonTileGaps.Checked, buttonBlockNumbers.Checked, buttonLevelBounds.Checked);
            }
            else
            {
                pictureBoxRenderedLevel.Image = null;
            }
        }

        private void tilePicker1_SelectionChanged(object sender, IDrawableBlock b)
        {
            var tile = b as Tile;
            pictureBoxTilePreview.Image?.Dispose();
            if (tile == null)
            {
                pictureBoxTilePreview.Image = null;
                return;
            }
            var shortestSize = Math.Min(pictureBoxTilePreview.Width, pictureBoxTilePreview.Height);
            var zoom = shortestSize / 8.0f;
            var bmp = new Bitmap(shortestSize, shortestSize);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.ScaleTransform(zoom, zoom);
                g.DrawImageUnscaled(tile.Image, 0, 0);
            }

            pictureBoxTilePreview.Image = bmp;

            pictureBoxTileUsedIn.Image?.Dispose();
            pictureBoxTileUsedIn.Image = null;
            if (listBoxLevels.SelectedItem is not Level level)
            {
                return;
            }

            var blocks = level.BlockMapping.Blocks
                .Where(block => block.TileIndices.Contains((byte)tile.Index))
                .ToList();
            if (blocks.Count == 0)
            {
                return;
            }

            var image = new Bitmap(blocks.Count * 33 - 1, 48);
            var x = 0;
            using (var g = Graphics.FromImage(image))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.Clear(SystemColors.Window);
                foreach (var block in blocks)
                {
                    g.DrawImageUnscaled(block.Image, x, 0);
                    g.DrawString(block.Index.ToString("X2"), Font, SystemBrushes.WindowText, 
                        new RectangleF(x, 32, 32, 16),
                        new StringFormat
                        {
                            Alignment = StringAlignment.Center
                        });
                    x += 33;
                }
            }

            pictureBoxTileUsedIn.Image = image;
        }

        private Block GetSelectedBlock()
        {
            return dataGridViewBlocks.SelectedRows.Count == 0 
                ? null 
                : (dataGridViewBlocks.SelectedRows[0].DataBoundItem as ObjectView<Block>)?.Object;
        }

        private void SelectedBlockChanged(object sender, EventArgs e)
        {
            DrawBlockEditor();
        }

        private void DrawBlockEditor()
        {
            pictureBoxBlockEditor.Image?.Dispose();
            pictureBoxBlockEditor.Image = null;
            var block = GetSelectedBlock();
            if (block == null)
            {
                return;
            }

            var scale = (pictureBoxBlockEditor.Width - 3) / 32;
            var bmp = new Bitmap(32 * scale + 3, 32 * scale + 3);
            _blockEditorTileSize = scale * 8 + 1;
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                for (var i = 0; i < 16; ++i)
                {
                    var tile = block.TileSet.Tiles[block.TileIndices[i]];
                    var x = i % 4 * _blockEditorTileSize - 1;
                    var y = i / 4 * _blockEditorTileSize - 1;
                    g.DrawImage(tile.Image, x, y, _blockEditorTileSize - 1, _blockEditorTileSize - 1);
                }
            }

            pictureBoxBlockEditor.Image = bmp;
        }

        private void BlockEditorMouseClick(object sender, MouseEventArgs e)
        {
            var block = GetSelectedBlock();
            if (block == null)
            {
                return;
            }
            var x = e.X / _blockEditorTileSize;
            var y = e.Y / _blockEditorTileSize;
            if (x > 3 || y > 3)
            {
                return;
            }
            var subBlockIndex = x + y * 4;
            var tileIndex = block.TileIndices[subBlockIndex];
            using var tc = new TileChooser(block.TileSet, tileIndex);
            if (tc.ShowDialog(this) == DialogResult.OK)
            {
                // Apply to the block object
                block.TileIndices[subBlockIndex] = (byte)tc.SelectedTile.Index;
                // Invalidate its cached image
                block.ResetImage();
                // Trigger a redraw of the editor
                DrawBlockEditor();
                // And the grid
                dataGridViewBlocks.InvalidateRow(dataGridViewBlocks.SelectedRows[0].Index);
                // And the rendered level
                RenderLevel();
            }
        }

        private void saveROMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog { Filter = "*.sms|*.sms" };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _cartridge.SaveTo(dialog.FileName);
            }
        }

        private void TreeViewLevelDataItemSelected(object sender, TreeViewEventArgs e)
        {
            var node = treeViewLevelData.SelectedNode;
            if (node.Tag is not LevelObject levelObject)
            {
                return;
            }

            using var chooser = new ObjectChooser(levelObject);
            if (chooser.ShowDialog(this) == DialogResult.OK)
            {
                levelObject.X = Convert.ToByte(chooser.textBoxX.Text);
                levelObject.Y = Convert.ToByte(chooser.textBoxY.Text);
                levelObject.Type = Convert.ToByte(chooser.textBoxType.Text);

                // Refresh the level data
                LoadLevelData();

                // And the level map may be different
                RenderLevel();
            }
        }

        private void GameTextDoubleClicked(object sender, EventArgs e)
        {
            if (listBoxGameText.SelectedItem is not GameText text)
            {
                return;
            }
            var input = Interaction.InputBox(
                "Please enter new game text, format X;Y;TEXT\n(TEXT can be 'A'-'Z', ' ' and '©')", "Edit game text",
                $"{text.X};{text.Y};{text.Text}");
            if (input != "")
            {
                try
                {
                    var match = Regex.Match(input, "^(?<x>\\d+);(?<y>\\d+);(?<text>.+)$");
                    if (!match.Success)
                    {
                        throw new Exception("Invalid text entered");
                    }

                    text.Text = match.Groups["text"].Value;
                    text.X = Convert.ToByte(match.Groups["x"].Value);
                    text.Y = Convert.ToByte(match.Groups["y"].Value);
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

            using var d = new SaveFileDialog { Filter = "*.png|*.png" };
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                pictureBoxRenderedLevel.Image.Save(d.FileName);
            }
        }

        private void LevelMapMouseDown(object sender, MouseEventArgs e)
        {
            _lastX = e.X;
            _lastY = e.Y;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length == 2 && File.Exists(args[1]))
            {
                LoadFile(args[1]);
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
                var rect = dataGridViewBlocks.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                e.Paint(e.CellBounds, e.PaintParts & ~DataGridViewPaintParts.ContentForeground);

                var index = (int)e.Value;
                DrawSolidityItem(e.Graphics, index, rect);

                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void DrawSolidityItem(Graphics g, int index, Rectangle rect)
        {
            g.DrawImageUnscaled(_solidityImages.Images[index], rect);
            g.DrawString($"{index}", Font, SystemBrushes.WindowText, rect.Left + _solidityImages.ImageSize.Width + 4, rect.Top + 8);
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
                DrawSolidityItem(e.Graphics, e.Index, e.Bounds);
            }
        }

        private void dataGridViewBlocks_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void tabControlLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlLevel.SelectedTab == tabPageLayout &&
                pictureBoxRenderedLevel.Image == null)
            {
                RenderLevel();
            }
        }

        private void quickTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filename = Path.Combine(Path.GetTempPath(), "test.sms");
            _cartridge.SaveTo(filename);
            Process.Start(filename);
        }

        private void buttonCopyFloor_Click(object sender, EventArgs e)
        {
            if (listBoxLevels.SelectedItem is not Level level)
            {
                return;
            }
            Clipboard.SetText(Convert.ToBase64String(level.Floor.BlockIndices));
        }

        private void buttonPasteFloor_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBoxLevels.SelectedItem is not Level level)
                {
                    return;
                }

                var text = Clipboard.GetText();
                var data = Convert.FromBase64String(text);
                if (data.Length != level.Floor.BlockIndices.Length)
                {
                    return;
                }
                data.CopyTo(level.Floor.BlockIndices, 0);
                // Need to redraw
                RenderLevel();
            }
            catch (Exception)
            {
                // Ignore it
            }
        }

        private void propertyGridLevel_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // Invalidate the level map
            RenderLevel();
        }

        private void pb3_MouseUp(object sender, MouseEventArgs e)
        {
            if (listBoxLevels.SelectedItem is not Level level)
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
            var selectedBlock = -1;
            if (minX == maxX && minY == maxY)
            {
                selectedBlock = level.Floor.BlockIndices[x + y * level.Floor.Width];
            }
            using var bc = new BlockChooser(level.BlockMapping.Blocks, selectedBlock);
            if (bc.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            // Apply the change
            for (var row = minY; row <= maxY; row++)
            for (var col = minX; col <= maxX; col++)
            {
                level.Floor.BlockIndices[col + row * level.Floor.Width] = (byte)bc.SelectedBlockIndex;
            }

            // Redraw the level
            RenderLevel();

            // Update counts
            level.BlockMapping.UpdateUsageForLevel(level);
            level.BlockMapping.UpdateGlobalUsage(_cartridge.Levels);
            dataGridViewBlocks.ResetBindings();

            LoadLevelData();

            UpdateFloorSpace();
        }

        private void UpdateFloorSpace()
        {
            var used = _cartridge.GetFloorSpace();
            floorStatus.Text = $"Floor space: {used.Used}/{used.Total} ({(double)used.Used/used.Total:P})";
        }

        private void buttonSaveTileset_Click(object sender, EventArgs e)
        {
            if (listBoxLevels.SelectedItem is not Level level)
            {
                return;
            }

            using var d = new SaveFileDialog { Filter = "PNG images|*.png" };
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                using var image = level.TileSet.ToImage();
                image.Save(d.FileName, ImageFormat.Png);
            }
        }

        private void buttonLoadTileset_Click(object sender, EventArgs e)
        {
            if (listBoxLevels.SelectedItem is not Level level)
            {
                return;
            }

            using var d = new OpenFileDialog { Filter = "Images|*.png" };
            if (d.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            using var image = (Bitmap) new ImageConverter().ConvertFrom(File.ReadAllBytes(d.FileName));
            if (image == null)
            {
                MessageBox.Show(this, "Failed to load image");
                return;
            }

            try
            {
                level.TileSet.FromImage(image);

                // Invalidate the tileset picker
                tilePicker1.Invalidate();

                // Mark the level as needing a redraw
                RenderLevel();

                UpdateTileSetSpace();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void buttonBlankUnusedTiles_Click(object sender, EventArgs e)
        {
            if (listBoxLevels.SelectedItem is not Level level)
            {
                return;
            }

            var tilesUsed = new HashSet<int>(level.BlockMapping.Blocks.SelectMany(b => b.TileIndices).Select(b => (int)b));

            foreach (var tile in level.TileSet.Tiles.Where(t => !tilesUsed.Contains(t.Index)))
            {
                // Tile is unused
                tile.CopyFrom(level.TileSet.Tiles[0]);
            }

            // Invalidate the tileset picker
            tilePicker1.Invalidate();

            UpdateTileSetSpace();
        }

        private void listBoxScreens_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = listBoxScreens.SelectedItem;
            pictureBox1.Image = (listBoxScreens.SelectedItem as Screen)?.Image;
        }

        private void UpdateTileSetSpace()
        {
            var used = _cartridge.GetTileSetSpace();
            tileSetStatus.Text = $"Tile set space: {used.Used}/{used.Total} ({(double)used.Used/used.Total:P})";
        }

    }
}