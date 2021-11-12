using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace sth1edwv
{
    public partial class TileSetViewer : UserControl
    {
        private Palette _palette;

        public TileSetViewer()
        {
            InitializeComponent();
        }

        public event Action<TileSet> Changed;

        public void SetData(TileSet tileSet, Palette palette, bool stackedMode,
            Func<int, IList<Block>> getUsedInBlocks)
        {
            _tileSet = tileSet;
            _palette = palette;
            tilePicker.SetData(tileSet?.Tiles, palette);
            tilePicker.SelectedIndex = -1;
            _getUsedInBlocks = getUsedInBlocks;
            buttonBlankUnusedTiles.Visible = getUsedInBlocks != null;
        }

        private void tilePicker_SelectionChanged(object sender, IDrawableBlock b)
        {
            var tile = b as Tile;
            pictureBoxTilePreview.Image = null;
            pictureBoxTilePreview.Image?.Dispose();
            pictureBoxTileUsedIn.Image = null;
            pictureBoxTileUsedIn.Image?.Dispose();
            if (tile == null)
            {
                return;
            }
            var zoom = Math.Min((float)pictureBoxTilePreview.Width / tile.Width, (float)pictureBoxTilePreview.Height / tile.Height);
            var bmp = new Bitmap((int)(tile.Width * zoom), (int)(tile.Height * zoom));
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.ScaleTransform(zoom, zoom);
                g.DrawImageUnscaled(tile.GetImage(_palette), 0, 0);
            }

            pictureBoxTilePreview.Image = bmp;


            if (_getUsedInBlocks == null)
            {
                return;
            }
            var blocks = _getUsedInBlocks((byte)tile.Index);
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
                    g.DrawImageUnscaled(block.GetImage(_palette), x, 0);
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

        private Func<int, IList<Block>> _getUsedInBlocks;
        private TileSet _tileSet;

        private void buttonSaveTileset_Click(object sender, EventArgs e)
        {
            using var d = new SaveFileDialog { Filter = "PNG images|*.png" };
            if (d.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
            using var image = _tileSet.ToImage(_palette);
            image.Save(d.FileName, ImageFormat.Png);
        }

        private void buttonLoadTileset_Click(object sender, EventArgs e)
        {
            using var d = new OpenFileDialog { Filter = "Images|*.png" };
            if (d.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            // This way avoids locking the file
            using var image = (Bitmap) new ImageConverter().ConvertFrom(File.ReadAllBytes(d.FileName));
            if (image == null)
            {
                MessageBox.Show(this, "Failed to load image");
                return;
            }

            try
            {
                _tileSet.FromImage(image);
                tilePicker.Invalidate();
                Changed?.Invoke(_tileSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void buttonBlankUnusedTiles_Click(object sender, EventArgs e)
        {
            if (_getUsedInBlocks == null)
            {
                return;
            }
            foreach (var tile in _tileSet.Tiles.Where(t => _getUsedInBlocks(t.Index).Count == 0))
            {
                tile.Blank();
            }
            tilePicker.Invalidate();
            tilePicker_SelectionChanged(tilePicker, tilePicker.SelectedItem);
            Changed?.Invoke(_tileSet);
        }
    }
}
