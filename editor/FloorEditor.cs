using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using sth1edwv.Properties;

namespace sth1edwv
{
    public sealed partial class FloorEditor : ScrollableControl
    {
        private Floor _floor;
        private Palette _palette;
        private BlockMapping _blockMapping;
        private LevelObjectSet _objects;
        private int _blockSize;
        private int _width;
        private int _height;
        private int _tileSize;
        private TileSet _tileSet;
        private Level _level;
        private bool _levelBounds;
        private bool _withObjects;
        private bool _blockNumbers;
        private bool _blockGaps;
        private bool _tileGaps;

        public FloorEditor()
        {
            InitializeComponent();
            DoubleBuffered = true;

            Paint += OnPaint;
            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
        }

        public void SetData(Level level)
        {
            _level = level;
            if (level == null)
            {
                return;
            }
            _floor = level.Floor;
            _palette = level.TilePalette;
            _tileSet = level.TileSet;
            _blockMapping = level.BlockMapping;
            _objects = level.Objects;

            _width = _floor.Width;
            _height = _floor.BlockIndices.Length / _width;

            UpdateSize();
        }

        private void UpdateSize()
        {
            if (_blockMapping == null || _blockMapping.Blocks.Count == 0)
            {
                return;
            }
            // Set bounds for scrolling
            _tileSize = 8 + (TileGaps ? 1 : 0);
            _blockSize = _tileSize * 4 + (BlockGaps ? 1 : 0);
            AutoScrollMinSize = new Size(_width * _blockSize, _height * _blockSize);
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            // We apply the scroll offset to the graphics to make it draw in the right place
            e.Graphics.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
            Draw(e.Graphics, e.ClipRectangle);
        }

        public void Draw(Graphics g, Rectangle clipRectangle)
        {
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            using var f = new Font(SystemFonts.MessageBoxFont.FontFamily, 8.0f);
            g.Clear(SystemColors.Window);

            if (_level == null)
            {
                g.DrawString("No level selected", f, SystemBrushes.WindowText, 0, 0);
                return;
            }

            // We render only the blocks intersecting with the area in question
            var minX = (clipRectangle.Left - AutoScrollPosition.X)/ _blockSize;
            var maxX = Math.Min((clipRectangle.Right - AutoScrollPosition.X - 1) / _blockSize, _width - 1);
            var minY = (clipRectangle.Top - AutoScrollPosition.Y) / _blockSize;
            var maxY = Math.Min((clipRectangle.Bottom - AutoScrollPosition.Y - 1) / _blockSize, _height - 1);
            for (var blockY = minY; blockY <= maxY; ++blockY)
            for (var blockX = minX; blockX <= maxX; ++blockX)
            {
                var blockIndex = _floor.BlockIndices[blockX + blockY * _width];
                if (blockIndex < _blockMapping.Blocks.Count)
                {
                    var block = _blockMapping.Blocks[blockIndex];
                    for (var tileX = 0; tileX < 4; ++tileX)
                    for (var tileY = 0; tileY < 4; ++tileY)
                    {
                        var tileIndex = block.TileIndices[tileX + tileY * 4];
                        if (tileIndex < _tileSet.Tiles.Count)
                        {
                            var tile = _tileSet.Tiles[tileIndex];
                            var x = blockX * _blockSize + tileX * _tileSize;
                            var y = blockY * _blockSize + tileY * _tileSize;
                            g.DrawImageUnscaled(tile.GetImage(_palette), x, y);
                        }
                    }
                }
                else
                {
                    g.DrawIcon(SystemIcons.Warning, new Rectangle(blockX * _blockSize, blockY * _blockSize, 32, 32));
                }

                if (BlockNumbers)
                {
                    g.DrawString(blockIndex.ToString("X2"), f, Brushes.Black, blockX * _blockSize,
                        blockY * _blockSize - 1);
                    g.DrawString(blockIndex.ToString("X2"), f, Brushes.White, blockX * _blockSize - 1,
                        blockY * _blockSize - 2);
                }
            }

            if (WithObjects)
            {
                var image = Resources.package;
                // Draw objects
                void DrawObject(int x, int y, string label)
                {
                    x *= _blockSize;
                    y *= _blockSize;
                    g.DrawRectangle(Pens.Blue, x, y, _blockSize, _blockSize);
                    g.DrawImageUnscaled(image, x + _blockSize / 2 - image.Width / 2,
                        y + _blockSize / 2 - image.Height / 2);

                    var dims = g.MeasureString(label, f).ToSize();

                    y += _blockSize;
                    if (y + dims.Height > AutoScrollMinSize.Height)
                    {
                        y -= _blockSize + dims.Height;
                    }
                    g.FillRectangle(Brushes.Blue, x, y, dims.Width, dims.Height);
                    g.DrawString(label, f, Brushes.White, x, y);
                }

                foreach (var levelObject in _objects)
                {
                    DrawObject(
                        levelObject.X, 
                        levelObject.Y,
                        LevelObject.Names.TryGetValue(levelObject.Type, out var name)
                            ? name
                            : levelObject.Type.ToString("X2"));
                }

                DrawObject(_level.StartX, _level.StartY, "Sonic");
            }

            if (LevelBounds)
            {
                var left = _level.LeftPixels + _level.LeftPixels / 32 * (_blockSize - 32) + 8;
                var top = _level.TopPixels + _level.TopPixels / 32 * (_blockSize - 32);
                var right = _level.RightEdgeFactor * _blockSize * 8 + 256 / 32 * _blockSize;
                var bottom = _level.BottomEdgeFactor * _blockSize * 8 + 192 / 32 * _blockSize + _level.ExtraHeight;
                var rect = new Rectangle(left, top, right - left, bottom - top);
                // Draw the grey region
                using var brush = new SolidBrush(Color.FromArgb(128, Color.Black));
                g.SetClip(rect, CombineMode.Exclude);
                g.FillRectangle(brush, 0, 0, AutoScrollMinSize.Width, AutoScrollMinSize.Height);
                // Draw the red border, a bit bigger so it's on the outside
                rect.Width += 1;
                rect.Height += 1;
                g.DrawRectangle(Pens.Red, rect);
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || BlockChooser == null)
            {
                return;
            }

            var index = GetClickedBlockIndex(e.X, e.Y);
            if (index == -1)
            {
                return;
            }

            // Ctrl key pressed forces select mode
            if (DrawingMode == Modes.Select || (ModifierKeys & Keys.Control) != 0)
            {
                BlockChooser.SelectedIndex = _floor.BlockIndices[index];
            }
            else if (DrawingMode == Modes.Draw && BlockChooser.SelectedIndex >= 0)
            {
                SetBlockIndex(index, BlockChooser.SelectedIndex);
            }
        }

        private void SetBlockIndex(int index, int value)
        {
            if (_floor.BlockIndices[index] == value)
            {
                return;
            }

            // Set it
            _floor.BlockIndices[index] = (byte)value;

            // Invalidate it
            var rect = new Rectangle(index % _width * _blockSize, index / _width * _blockSize, _blockSize, _blockSize);
            rect.Offset(AutoScrollPosition);
            Invalidate(rect);
            FloorChanged?.Invoke();
        }

        private int GetClickedBlockIndex(int x, int y)
        {
            x = (x - AutoScrollPosition.X) / _blockSize;
            y = (y - AutoScrollPosition.Y) / _blockSize;
            var index = y * _width + x;
            if (x <= _width && y <= _height && index < _floor.BlockIndices.Length)
            {
                return index;
            }

            return -1;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (DrawingMode != Modes.Draw || e.Button != MouseButtons.Left || BlockChooser == null || BlockChooser.SelectedIndex < 0)
            {
                return;
            }

            var index = GetClickedBlockIndex(e.X, e.Y);
            if (index == -1)
            {
                return;
            }

            SetBlockIndex(index, BlockChooser.SelectedIndex);
        }

        public bool LevelBounds
        {
            get => _levelBounds;
            set { _levelBounds = value; Invalidate(); }
        }

        public bool WithObjects
        {
            get => _withObjects;
            set { _withObjects = value; Invalidate(); }
        }

        public bool BlockNumbers
        {
            get => _blockNumbers;
            set { _blockNumbers = value; Invalidate(); }
        }

        public bool BlockGaps
        {
            get => _blockGaps;
            set { _blockGaps = value; UpdateSize(); }
        }

        public bool TileGaps
        {
            get => _tileGaps;
            set { _tileGaps = value; UpdateSize(); }
        }

        public ItemPicker BlockChooser { get; set; }

        public enum Modes
        {
            Draw,
            Select
        }

        public Modes DrawingMode { get; set; }

        public event Action FloorChanged;
    }
}
