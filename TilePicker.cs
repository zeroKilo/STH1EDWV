using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace sth1edwv
{
    public sealed partial class TilePicker : UserControl
    {
        private int _tileSize;
        private TileSet _tileSet;
        private const int TilesPerRow = 16;

        public TilePicker()
        {
            InitializeComponent();
        }

        private void TilePicker_Resize(object sender, EventArgs e)
        {
            // We compute the tile size
            var tileSize = (Width - TilesPerRow - 1) / TilesPerRow;
            if (tileSize != _tileSize)
            {
                _tileSize = tileSize;
                Invalidate();
            }
        }

        private void TilePicker_Paint(object sender, PaintEventArgs e)
        {
            // Clear the area first
            e.Graphics.FillRectangle(SystemBrushes.Window, e.ClipRectangle);
            if (TileSet == null)
            {
                e.Graphics.DrawString("No tiles", SystemFonts.MessageBoxFont, SystemBrushes.WindowText, 0, 0);
                return;
            }
            // Draw all tiles overlapping the rect
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            for (var index = 0; index < TileSet.Tiles.Count; ++index)
            {
                // Is this tile in the clip rectangle?
                var tileRect = ScreenRectFromIndex(index);
                if (!tileRect.IntersectsWith(e.ClipRectangle))
                {
                    continue;
                }
                var tile = TileSet.Tiles[index];
                e.Graphics.DrawImage(tile.Image, tileRect);
            }

            if (SelectedIndex > -1)
            {
                // Is this tile in the clip rectangle?
                var tileRect = ScreenRectFromIndex(SelectedIndex);
                if (tileRect.IntersectsWith(e.ClipRectangle))
                {
                    tileRect.Width += 1;
                    tileRect.Height += 1;
                    e.Graphics.DrawRectangle(SystemPens.Highlight, tileRect);
                }
            }
        }

        // Returns the bounding rect for the given metatile, in screen coordinates.
        private Rectangle ScreenRectFromIndex(int index)
        {
            if (TileSet == null || index < 0 || index >= TileSet.Tiles.Count)
            {
                return new Rectangle();
            }
            var x = index % 16 * (_tileSize + 1) + 1;
            var y = index / 16 * (_tileSize + 1) + 1;
            return new Rectangle(x, y, _tileSize, _tileSize);
        }

        private void TilePicker_MouseClick(object sender, MouseEventArgs e)
        {
            var newSelection = -1;
            try
            {
                if (_tileSet == null)
                {
                    return;
                }

                // Determine the clicked tile
                var x = e.X / (_tileSize + 1);
                if (x > 15)
                {
                    return;
                }

                var y = e.Y / (_tileSize + 1);
                var tileIndex = x + y * 16;
                if (tileIndex >= TileSet.Tiles.Count)
                {
                    return;
                }

                newSelection = tileIndex;
            }
            finally
            {
                ChangeSelection(newSelection);
            }
        }

        private void ChangeSelection(int newSelection)
        {
            if (SelectedIndex != newSelection)
            {
                var previousSelection = SelectedIndex;
                SelectedIndex = newSelection;
                InvalidateTile(previousSelection);
                InvalidateTile(SelectedIndex);
                SelectionChanged?.Invoke(this, SelectedTile);
            }
        }

        private void InvalidateTile(int tileIndex)
        {
            if (tileIndex != -1)
            {
                // Clear the old selection
                var rect = ScreenRectFromIndex(tileIndex);
                rect.Inflate(2, 2);
                Invalidate(rect);
            }
        }

        public Tile SelectedTile => SelectedIndex == -1 ? null : TileSet?.Tiles[SelectedIndex];

        public TileSet TileSet
        {
            get => _tileSet;
            set
            {
                _tileSet = value;
                OnResize(EventArgs.Empty);
                Invalidate();
            }
        }

        public int SelectedIndex { get; set; } = -1;

        public event EventHandler<Tile> SelectionChanged;
    }
}
