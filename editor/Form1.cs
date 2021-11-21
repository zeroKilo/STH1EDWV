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
using sth1edwv.Controls;
using sth1edwv.Forms;
using sth1edwv.GameObjects;
using sth1edwv.Properties;
using Screen = sth1edwv.GameObjects.Screen;

namespace sth1edwv
{
    public sealed partial class Form1 : Form
    {
        private Cartridge _cartridge;
        private readonly ImageList _solidityImages;
        private int _blockEditorTileSize;
        private byte[] _lastSaved;

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
            _cartridge = new Cartridge(filename, Log);
            listBoxLevels.Items.Clear();
            listBoxLevels.Items.AddRange(_cartridge.Levels.ToArray<object>());
            listBoxGameText.Items.Clear();
            listBoxGameText.Items.AddRange(_cartridge.GameText.ToArray<object>());
            listBoxArt.Items.Clear();
            listBoxArt.Items.AddRange(_cartridge.Art.ToArray<object>());
            listBoxArt.Items.AddRange(_cartridge.Screens.ToArray<object>());
            listBoxArt.Sorted = true;

            // Add or replace filename in title bar
            Text = $"{Regex.Replace(Text, " \\[.+\\]$", "")} [{Path.GetFileName(filename)}]";

            // Trigger the selected level changed event
            SelectedLevelChanged(null, null);

            UpdateFloorSpace();
            UpdateTileSetSpace();

            // Take an image at the point of loading
            _lastSaved = _cartridge.MakeRom();
        }

        private void SelectedLevelChanged(object sender, EventArgs e)
        {
            var level = listBoxLevels.SelectedItem as Level;

            floorEditor1.SetData(level);
            LevelRenderModeChanged(null, EventArgs.Empty);

            tileSetViewer.SetData(level?.TileSet, level?.TilePalette, GetTileUsedInBlocks);
            spriteTileSetViewer.SetData(level?.SpriteTileSet, level?.SpritePalette);

            LoadLevelData();

            foreach (Control control in PalettesLayout.Controls)
            {
                control.Dispose();
            }
            PalettesLayout.Controls.Clear();
            if (level != null)
            {
                PalettesLayout.Controls.Add(new PaletteEditor(level.Palette, "Base palette (only sprites part is used)", OnPaletteChanged));
                PalettesLayout.Controls.Add(new PaletteEditor(level.CyclingPalette, "Colour cycling", OnPaletteChanged));
            }

            propertyGridLevel.SelectedObject = level;

            level?.BlockMapping.UpdateUsageForLevel(level);
            level?.BlockMapping.UpdateGlobalUsage(_cartridge.Levels);

            dataGridViewBlocks.DataSource = level == null 
                ? null 
                : new BindingListView<BlockRow>(level.BlockMapping.Blocks.Select(x => new BlockRow(x, level.TilePalette)).ToList());
            dataGridViewBlocks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            layoutBlockChooser.SetData(level?.BlockMapping.Blocks, level?.TilePalette);
        }

        private void OnPaletteChanged(Palette palette)
        {
            // We invalidate everything based on it...
            foreach (var level in _cartridge.Levels.Where(x => x.Palette == palette))
            {
                level.UpdateRenderingPalettes();
            }

            if (listBoxLevels.SelectedItem is Level selectedLevel)
            {
                tileSetViewer.SetData(selectedLevel.TileSet, selectedLevel.TilePalette, GetTileUsedInBlocks);
                spriteTileSetViewer.SetData(selectedLevel.SpriteTileSet, selectedLevel.SpritePalette);
            }
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
                    g.DrawImage(tile.GetImageWithRings(level.TilePalette), x, y, _blockEditorTileSize - 1, _blockEditorTileSize - 1);
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
                try
                {
                    var data = _cartridge.MakeRom();
                    File.WriteAllBytes(dialog.FileName, data);
                    _lastSaved = data;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"There was an error while saving:\n\n{ex.Message}");
                }
            }
        }

        private void TreeViewLevelDataItemSelected(object sender, TreeViewEventArgs e)
        {
            var node = treeViewLevelData.SelectedNode;
            if (node.Tag is not LevelObject levelObject)
            {
                return;
            }

            using var chooser = new ObjectEditor(levelObject);
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
            try
            {
                var filename = Path.Combine(Path.GetTempPath(), "test.sms");
                File.WriteAllBytes(filename, _cartridge.MakeRom());
                Process.Start(filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"There was an error while saving:\n\n{ex.Message}");
            }
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
            UpdateSpace(floorStatus, "Floor/screen tile map", _cartridge.GetFloorSpace());
        }

        private void DrawingButtonCheckedChanged(object sender, EventArgs e)
        {
            foreach (var button in new[]{buttonDraw, buttonSelect, buttonFloodFill})
            {
                button.Checked = button == sender;
            }

            floorEditor1.DrawingMode = buttonDraw.Checked ? FloorEditor.Modes.Draw 
                : buttonSelect.Checked ? FloorEditor.Modes.Select 
                : FloorEditor.Modes.FloodFill;
        }

        private void floorEditor1_FloorChanged()
        {
            UpdateFloorSpace();
        }

        private void UpdateTileSetSpace()
        {
            UpdateSpace(tileSetStatus, "Tile set", _cartridge.GetFloorTileSetSpace());
            UpdateSpace(spriteTileSetStatus, "Sprite tile set", _cartridge.GetSpriteTileSetSpace());
        }

        private void UpdateSpace(ToolStripStatusLabel label, string prefix, Cartridge.Space space)
        {
            label.Text = $"{prefix} space: {space.Used}/{space.Total} ({(double)space.Used/space.Total:P})";
            label.ForeColor = space.Used > space.Total ? Color.White: SystemColors.ControlText;
            label.BackColor = space.Used > space.Total ? Color.DarkRed : SystemColors.Control;
        }


        private void spriteTileSetViewer_Changed(TileSet tileSet)
        {
            UpdateTileSetSpace();
        }

        private void tileSetViewer_Changed(TileSet tileSet)
        {
            // We need to reset any cached images in block mappings using this tile set
            foreach (var block in _cartridge.Levels
                .Where(x => x.TileSet == tileSet)
                .Select(x => x.BlockMapping)
                .Distinct()
                .SelectMany(x => x.Blocks))
            {
                block.ResetImages();
            }

            UpdateTileSetSpace();
        }

        private void ResizeFloorButtonClick(object sender, EventArgs e)
        {
            if (listBoxLevels.SelectedItem is not Level selectedLevel)
            {
                return;
            }

            using var form = new FloorSizeEditor(selectedLevel);
            if (form.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            // Floor size changed!
            // We need to change stuff for every level using this floor...
            foreach (var level in _cartridge.Levels.Where(x => x.Floor == selectedLevel.Floor))
            {
                // - The level header
                level.FloorWidth += form.Result.Horizontal;
                level.FloorHeight += form.Result.Vertical;
                // - The level bounds - top/left only
                level.LeftPixels = Math.Max(0, level.LeftPixels + form.Result.Left * 32);
                level.TopPixels = Math.Max(0, level.TopPixels + form.Result.Top * 32);
                // - Start position
                level.StartX += form.Result.Left;
                level.StartY += form.Result.Top;
                // - Object positions
                foreach (var levelObject in level.Objects)
                {
                    levelObject.X = (byte)Math.Min(level.FloorWidth - 1, Math.Max(0, levelObject.X + form.Result.Left));
                    levelObject.Y = (byte)Math.Min(level.FloorHeight - 1, Math.Max(0, levelObject.Y + form.Result.Top));
                }
            }
            // Then the floor itself
            selectedLevel.Floor.Resize(form.Result);

            // Reset the floor editor so it picks up the new size
            floorEditor1.SetData(selectedLevel);
            // Update space counts
            UpdateFloorSpace();
        }

        private void SharingButton_Click(object sender, EventArgs e)
        {
            if (listBoxLevels.SelectedItem is not Level selectedLevel)
            {
                return;
            }

            using var d = new FloorSharingEditor(selectedLevel, _cartridge);
            if (d.ShowDialog(this) == DialogResult.OK)
            {
                // Invalidate stuff
                floorEditor1.SetData(selectedLevel);
                UpdateFloorSpace();
            }
        }

        private void selectBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBoxLevels.SelectedItem is not Level level)
            {
                return;
            }
            layoutBlockChooser.SelectedIndex = level.Floor.BlockIndices[floorEditor1.LastClickedBlockIndex];
        }

        private void editObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBoxLevels.SelectedItem is not Level level)
            {
                return;
            }
            var index = floorEditor1.LastClickedBlockIndex;
            var x = index % level.FloorWidth;
            var y = index / level.FloorWidth;
            // See if we have an object here
            var levelObject = level.Objects.FirstOrDefault(o => o.X == x && o.Y == y);
            if (levelObject != null)
            {
                using var editor = new ObjectEditor(levelObject);
                if (editor.ShowDialog(this) == DialogResult.OK)
                {
                    levelObject.X = Convert.ToByte(editor.textBoxX.Text);
                    levelObject.Y = Convert.ToByte(editor.textBoxY.Text);
                    levelObject.Type = Convert.ToByte(editor.textBoxType.Text);

                    // Refresh the level data
                    LoadLevelData();

                    // And the level map may be different
                    if (floorEditor1.WithObjects)
                    {
                        floorEditor1.Invalidate();
                    }
                }
            }
        }

        private void listBoxArt_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Control control in tabPageArtPalette.Controls)
            {
                control.Dispose();
            }
            tabPageArtPalette.Controls.Clear();
            otherArtTileSetViewer.SetData(null, null);

            propertyGrid1.SelectedObject = listBoxArt.SelectedItem;

            switch (listBoxArt.SelectedItem)
            {
                case ArtItem artItem:
                    Show(artItem);
                    return;
                case Screen screen:
                    Show(screen);
                    break;
            }
        }

        private void Show(Screen screen)
        {
            otherArtTileSetViewer.TilesPerRow = 16;
            otherArtTileSetViewer.SetData(screen.TileSet, screen.Palette);

            var paletteEditor = new PaletteEditor(screen.Palette, "Palette", _ =>
            {
                foreach (var tile in screen.TileSet.Tiles)
                {
                    tile.ResetImages();
                }
                otherArtTileSetViewer.Invalidate();
            });
            tabPageArtPalette.Controls.Add(paletteEditor);
            paletteEditor.Dock = DockStyle.Fill;

            pictureBoxArtLayout.Image = screen.TileMap.GetImage(screen.TileSet, screen.Palette);
            if (!tabControlArt.TabPages.Contains(tabPageArtLayout))
            {
                tabControlArt.TabPages.Insert(0, tabPageArtLayout);
            }
        }

        private void Show(ArtItem artItem)
        {
            otherArtTileSetViewer.TilesPerRow = artItem.Width;
            otherArtTileSetViewer.SetData(artItem.TileSet, artItem.Palette, null, artItem.IsSprites);

            if (artItem.PaletteEditable)
            {
                var paletteEditor = new PaletteEditor(artItem.Palette, "Palette", _ =>
                {
                    foreach (var tile in artItem.TileSet.Tiles)
                    {
                        tile.ResetImages();
                    }
                    otherArtTileSetViewer.Invalidate();
                });
                tabPageArtPalette.Controls.Add(paletteEditor);
                paletteEditor.Dock = DockStyle.Fill;
                if (!tabControlArt.TabPages.Contains(tabPageArtPalette))
                {
                    tabControlArt.TabPages.Add(tabPageArtPalette);
                }
            }
            else
            {
                tabControlArt.TabPages.Remove(tabPageArtPalette);
            }
            // No layouts for these
            tabControlArt.TabPages.Remove(tabPageArtLayout);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && _lastSaved != null)
            {
                try
                {
                    var current = _cartridge.MakeRom();
                    if (!current.SequenceEqual(_lastSaved))
                    {
                        if (MessageBox.Show(
                                this, 
                                "You have unsaved changes. Do you want to discard them?",
                                "Unsaved changes", 
                                MessageBoxButtons.YesNo, 
                                MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            e.Cancel = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show(
                            this,
                            $"There may be unsaved changes, however there is currently an error:\n\n{ex.Message}",
                            "Unsaved changes", 
                            MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void Log(string text)
        {
            logTextBox.BeginInvoke(new Action(() =>
            {
                logTextBox.AppendText(text);
                logTextBox.AppendText("\r\n");
            }));
        }
    }
}