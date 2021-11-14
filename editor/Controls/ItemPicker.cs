using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using sth1edwv.GameObjects;

namespace sth1edwv.Controls
{
    public interface IDrawableBlock
    {
        public Bitmap GetImage(Palette palette);
        public int Width { get; }
        public int Height { get; }
    }

    public sealed partial class ItemPicker : ScrollableControl
    {
        private int _tileWidth;
        private int _tileHeight;
        private List<IDrawableBlock> _items;
        private Palette _palette;
        private int _selectedIndex = -1;

        public ItemPicker()
        {
            DoubleBuffered = true;
            InitializeComponent();
        }

        public void SetData(IEnumerable<IDrawableBlock> items, Palette palette)
        {
            _palette = palette;
            _items = items?.ToList();
            if (AutoSize && FixedItemsPerRow && ItemsPerRow > 0 && _items is { Count: > 0 })
            {
                // We want to auto-size
                var rowCount = _items.Count / ItemsPerRow + (_items.Count % ItemsPerRow == 0 ? 0 : 1);
                var itemWidth = _items[0].Width * Math.Max(1, Scaling) + 1;
                var itemHeight = _items[0].Height * Math.Max(1, Scaling) + 1;
                Size = new Size(ItemsPerRow * itemWidth + 1, rowCount * itemHeight + 1);
            }
            CheckDrawingSettings();
            Invalidate();
        }

        public bool FixedItemsPerRow { get; set; }
        public int Scaling { get; set; } = 1;
        public int ItemsPerRow { get; set; }

        protected override void OnResize(EventArgs e)
        {
            CheckDrawingSettings();
            base.OnResize(e);
        }

        private void CheckDrawingSettings()
        {
            // We either change the items per row or item size
            if (FixedItemsPerRow)
            {
                // We compute the tile size
                if (ItemsPerRow > 0)
                {
                    var tileWidth = (Width - ItemsPerRow - 1) / ItemsPerRow;
                    var tileHeight = _items is { Count: > 0 } 
                        ? tileWidth * _items[0].Height / _items[0].Width 
                        : tileWidth;

                    if (tileWidth != _tileWidth || tileHeight != _tileHeight)
                    {
                        _tileWidth = tileWidth;
                        _tileHeight = tileHeight;
                        Invalidate();
                    }
                }
            }
            else if (_items is { Count: > 0 })
            {
                // We compute the items per row
                _tileWidth = _items[0].Width;
                _tileHeight = _items[0].Height;
                var newItemsPerRow = (Width - 1 - SystemInformation.VerticalScrollBarWidth) / _tileWidth;
                if (newItemsPerRow != ItemsPerRow)
                {
                    ItemsPerRow = newItemsPerRow;

                    // Set scroll bounds
                    var numRows = _items.Count / ItemsPerRow + (_items.Count % ItemsPerRow == 0 ? 0 : 1);
                    AutoScrollMinSize = new Size(ItemsPerRow * _tileWidth, numRows * _tileHeight);

                    Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Clear the area first
            e.Graphics.FillRectangle(SystemBrushes.Window, e.ClipRectangle);
            if (_items == null)
            {
                e.Graphics.DrawString("No items", Font, SystemBrushes.WindowText, 0, 0);
                return;
            }

            if (_palette == null)
            {
                e.Graphics.DrawString("No palette`", Font, SystemBrushes.WindowText, 0, 0);
                return;
            }
            // Draw all tiles overlapping the rect
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            for (var index = 0; index < _items.Count; ++index)
            {
                // Is this tile in the clip rectangle?
                var tileRect = ScreenRectFromIndex(index);
                if (!tileRect.IntersectsWith(e.ClipRectangle))
                {
                    continue;
                }
                var tile = _items[index];
                e.Graphics.DrawImage(tile.GetImage(_palette), tileRect);
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
            if (_items == null || index < 0 || index >= _items.Count)
            {
                return new Rectangle();
            }

            var x = index % ItemsPerRow * (_tileWidth + 1) + 1 + AutoScrollPosition.X;
            var y = index / ItemsPerRow * (_tileHeight + 1) + 1 + AutoScrollPosition.Y;
            return new Rectangle(x, y, _tileWidth, _tileHeight);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var newSelection = -1;
            try
            {
                if (_items == null)
                {
                    return;
                }

                // Determine the clicked tile
                var x = (e.X - AutoScrollPosition.X) / (_tileWidth + 1);
                if (x > 15)
                {
                    return;
                }

                var y = (e.Y - AutoScrollPosition.Y) / (_tileHeight + 1);
                var tileIndex = x + y * ItemsPerRow;

                if (tileIndex >= _items.Count)
                {
                    return;
                }

                newSelection = tileIndex;
                
                // Focus on click
                Select();
            }
            finally
            {
                SelectedIndex = newSelection;
            }
        }

        private void InvalidateItem(int index)
        {
            if (index == -1)
            {
                return;
            }
            // Clear the old selection
            var rect = ScreenRectFromIndex(index);
            rect.Inflate(2, 2);
            Invalidate(rect);
        }

        public IDrawableBlock SelectedItem => SelectedIndex == -1 ? null : _items[SelectedIndex];

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex == value)
                {
                    return;
                }
                var previousSelection = _selectedIndex;
                _selectedIndex = value;
                InvalidateItem(previousSelection);
                InvalidateItem(_selectedIndex);
                SelectionChanged?.Invoke(this, SelectedItem);
            }
        }

        public event EventHandler<IDrawableBlock> SelectionChanged;

        protected override void OnKeyUp(KeyEventArgs e)
        {
            var newSelection = SelectedIndex;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    newSelection -= ItemsPerRow;
                    break;
                case Keys.Down:
                    newSelection += ItemsPerRow;
                    break;
                case Keys.Left:
                    --newSelection;
                    break;
                case Keys.Right:
                    ++newSelection;
                    break;
                default:
                    return;
            }

            SelectedIndex = Math.Max(0, Math.Min(_items.Count - 1, newSelection));
        }
    }
}
