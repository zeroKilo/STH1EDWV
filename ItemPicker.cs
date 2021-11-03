using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace sth1edwv
{
    public interface IDrawableBlock
    {
        public Bitmap Image { get; }
    }

    public sealed partial class ItemPicker : UserControl
    {
        private int _tileSize;
        private List<IDrawableBlock> _items;

        public ItemPicker()
        {
            InitializeComponent();
        }

        public List<IDrawableBlock> Items
        {
            get => _items;
            set
            {
                _items = value;
                CheckDrawingSettings();
                Invalidate();
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            if (FixedItemsPerRow && ItemsPerRow > 0 && Items is { Count: > 0 })
            {
                // We want to auto-size
                var rowCount = Items.Count / ItemsPerRow + (Items.Count % ItemsPerRow == 0 ? 0 : 1);
                var pixelsPerItem = Items[0].Image.Width + 1;
                return new Size(ItemsPerRow * pixelsPerItem + 1, rowCount * pixelsPerItem + 1);
            }

            return proposedSize;
        }

        public bool FixedItemsPerRow { get; set; }
        public int ItemsPerRow { get; set; }

        private void OnResize(object sender, EventArgs e)
        {
            CheckDrawingSettings();
        }

        private void CheckDrawingSettings()
        {
            // We either change the items per row or item size
            if (FixedItemsPerRow)
            {
                // We compute the tile size
                if (ItemsPerRow > 0)
                {
                    var tileSize = (Width - ItemsPerRow - 1) / ItemsPerRow;

                    if (tileSize != _tileSize)
                    {
                        _tileSize = tileSize;
                        Invalidate();
                    }
                }
            }
            else if (Items is { Count: > 0 })
            {
                // We compute the items per row
                var newItemsPerRow = (Width - 1) / Items[0].Image.Width;
                if (newItemsPerRow != ItemsPerRow)
                {
                    ItemsPerRow = newItemsPerRow;
                    Invalidate();
                }
            }
        }

        private void TilePicker_Paint(object sender, PaintEventArgs e)
        {
            // Clear the area first
            e.Graphics.FillRectangle(SystemBrushes.Window, e.ClipRectangle);
            if (Items == null)
            {
                e.Graphics.DrawString("No items", Font, SystemBrushes.WindowText, 0, 0);
                return;
            }
            // Draw all tiles overlapping the rect
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            for (var index = 0; index < Items.Count; ++index)
            {
                // Is this tile in the clip rectangle?
                var tileRect = ScreenRectFromIndex(index);
                if (!tileRect.IntersectsWith(e.ClipRectangle))
                {
                    continue;
                }
                var tile = Items[index];
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
            if (Items == null || index < 0 || index >= Items.Count)
            {
                return new Rectangle();
            }
            var x = index % ItemsPerRow  * (_tileSize + 1) + 1;
            var y = index / ItemsPerRow * (_tileSize + 1) + 1;
            return new Rectangle(x, y, _tileSize, _tileSize);
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            var newSelection = -1;
            try
            {
                if (Items == null)
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
                var tileIndex = x + y * ItemsPerRow;
                if (tileIndex >= Items.Count)
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
                InvalidateItem(previousSelection);
                InvalidateItem(SelectedIndex);
                SelectionChanged?.Invoke(this, SelectedItem);
            }
        }

        private void InvalidateItem(int index)
        {
            if (index != -1)
            {
                // Clear the old selection
                var rect = ScreenRectFromIndex(index);
                rect.Inflate(2, 2);
                Invalidate(rect);
            }
        }

        public IDrawableBlock SelectedItem => SelectedIndex == -1 ? null : Items[SelectedIndex];

        public int SelectedIndex { get; set; } = -1;

        public event EventHandler<IDrawableBlock> SelectionChanged;

        private void OnKeyUp(object sender, KeyEventArgs e)
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
            }

            newSelection = Math.Max(0, Math.Min(Items.Count - 1, newSelection));
            ChangeSelection(newSelection);
        }
    }
}
