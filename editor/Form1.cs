using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            if (d.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            _cartridge?.Dispose();
            LoadFile(d.FileName);
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
            var level = listBoxLevels.SelectedItem as Level;

            floorEditor1.SetData(level);
            LevelRenderModeChanged(null, EventArgs.Empty);

            tileSetViewer.SetData(level?.TileSet, level?.TilePalette, false, GetTileUsedInBlocks);
            spriteTileSetViewer.SetData(level?.SpriteTileSet, level?.SpritePalette, true, null);

            LoadLevelData();

            propertyGridLevel.SelectedObject = level;

            level?.BlockMapping.UpdateUsageForLevel(level);
            level?.BlockMapping.UpdateGlobalUsage(_cartridge.Levels);

            dataGridViewBlocks.DataSource = level == null 
                ? null 
                : new BindingListView<BlockRow>(level.BlockMapping.Blocks.Select(x => new BlockRow(x, level.TilePalette)).ToList());
            dataGridViewBlocks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            layoutBlockChooser.SetData(level?.BlockMapping.Blocks, level?.TilePalette);
        }

        private IList<Block> GetTileUsedInBlocks(int index)
        {
            if (listBoxLevels.SelectedItem is not Level level)
            {
                return new List<Block>();
            }

            return level.BlockMapping.Blocks
                .Where(block => block.TileIndices.Contains((byte)index))
                .ToList();
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
            floorEditor1.TileGaps = buttonTileGaps.Checked;
            floorEditor1.BlockGaps = buttonBlockGaps.Checked;
            floorEditor1.LevelBounds = buttonLevelBounds.Checked;
            floorEditor1.BlockNumbers = buttonBlockNumbers.Checked;
            floorEditor1.WithObjects = buttonShowObjects.Checked;
        }

        private Block GetSelectedBlock()
        {
            return dataGridViewBlocks.SelectedRows.Count == 0 
                ? null 
                : (dataGridViewBlocks.SelectedRows[0].DataBoundItem as ObjectView<BlockRow>)?.Object.Block;
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

            if (listBoxLevels.SelectedItem is not Level level)
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
                    g.DrawImage(tile.GetImage(level.TilePalette), x, y, _blockEditorTileSize - 1, _blockEditorTileSize - 1);
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
            if (listBoxLevels.SelectedItem is not Level level)
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
            using var tc = new TileChooser(block.TileSet, tileIndex, level.TilePalette);
            if (tc.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            // Apply to the block object
            block.TileIndices[subBlockIndex] = (byte)tc.SelectedTile.Index;
            // Invalidate its cached image
            block.ResetImages();
            // Trigger a redraw of the editor
            DrawBlockEditor();
            // And the grid
            dataGridViewBlocks.InvalidateRow(dataGridViewBlocks.SelectedRows[0].Index);
            // And the rendered level
            floorEditor1.Invalidate();
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
            if (chooser.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            levelObject.X = Convert.ToByte(chooser.textBoxX.Text);
            levelObject.Y = Convert.ToByte(chooser.textBoxY.Text);
            levelObject.Type = Convert.ToByte(chooser.textBoxType.Text);

            // Refresh the level data
            LoadLevelData();

            // And the level map may be different
            if (floorEditor1.WithObjects)
            {
                floorEditor1.Invalidate();
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
            if (input == "")
            {
                return;
            }
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

        private void toolStripButtonSaveRenderedLevel_Click(object sender, EventArgs e)
        {
            using var bmp = new Bitmap(floorEditor1.AutoScrollMinSize.Width, floorEditor1.AutoScrollMinSize.Height);
            using var g = Graphics.FromImage(bmp);
            floorEditor1.Draw(g, new Rectangle(0, 0, bmp.Width, bmp.Height));
            using var d = new SaveFileDialog { Filter = "*.png|*.png" };
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                bmp.Save(d.FileName);
            }
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
            if (e.Control is not ComboBox combo)
            {
                return;
            }
            combo.DrawMode = DrawMode.OwnerDrawFixed;
            combo.ItemHeight = _solidityImages.ImageSize.Height + 1;
            combo.DrawItem -= DrawSolidityComboItem;
            combo.DrawItem += DrawSolidityComboItem;
        }

        private void DrawSolidityComboItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            DrawSolidityItem(e.Graphics, e.Index, e.Bounds);
        }

        private void dataGridViewBlocks_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
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
                floorEditor1.Invalidate();
            }
            catch (Exception)
            {
                // Ignore it
            }
        }

        private void propertyGridLevel_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // Invalidate the level map
            floorEditor1.Invalidate();
        }

        private void UpdateFloorSpace()
        {
            var used = _cartridge.GetFloorSpace();
            floorStatus.Text = $"Floor space: {used.Used}/{used.Total} ({(double)used.Used/used.Total:P})";
        }

        private void listBoxScreens_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = listBoxScreens.SelectedItem;
            pictureBox1.Image = (listBoxScreens.SelectedItem as Screen)?.Image;
        }

        private void DrawingButtonCheckedChanged(object sender, EventArgs e)
        {
            foreach (var button in new[]{buttonDraw, buttonSelect}.Where(x => x.Checked && x != sender))
            {
                button.Checked = false;
            }

            floorEditor1.DrawingMode = buttonDraw.Checked ? FloorEditor.Modes.Draw : FloorEditor.Modes.Select;
        }

        private void floorEditor1_FloorChanged()
        {
            UpdateFloorSpace();
        }

        private void UpdateTileSetSpace()
        {
            var backgrounds = _cartridge.GetFloorTileSetSpace();
            var sprites = _cartridge.GetSpriteTileSetSpace();
            tileSetStatus.Text = $"Tile set space: backgrounds {backgrounds.Used}/{backgrounds.Total} ({(double)backgrounds.Used/backgrounds.Total:P}), sprites {sprites.Used}/{sprites.Total} ({(double)sprites.Used/sprites.Total:P})";
        }

        private void spriteTileSetViewer_Changed(TileSet tileSet)
        {
            UpdateTileSetSpace();
        }

        private void tileSetViewer_Changed(TileSet obj)
        {
            UpdateTileSetSpace();
        }
    }
}