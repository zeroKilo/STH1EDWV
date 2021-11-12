using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace sth1edwv
{
    public class Palette
    {
        private readonly int _offset;
        private readonly List<Color> _colors = new();
        private ColorPalette _imagePalette;

        public ColorPalette ImagePalette
        {
            get
            {
                if (_imagePalette == null)
                {
                    // We have to extract a ColorPalette from an image as it blocks us from constructing it...
                    using var bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
                    _imagePalette = bmp.Palette;
                    // We have to clear the whole palette to non-transparent colours to avoid (1) the default palette and (2) an image that GDI+ transforms to 32bpp on load
                    for (var i = 0; i < 256; ++i)
                    {
                        _imagePalette.Entries[i] = Color.Magenta;
                    }

                    // Then we apply the real palette to the start
                    _colors.CopyTo(_imagePalette.Entries, 0);
                }
                return _imagePalette;
            }
        }

        public Palette(IReadOnlyList<byte> mem, int offset, int count)
        {
            _offset = offset;
            for (var i = 0; i < 16*count; i++)
            {
                var color = mem[offset + i];
                var r = ScaleColor((color >> 0) & 0b11);
                var g = ScaleColor((color >> 2) & 0b11);
                var b = ScaleColor((color >> 4) & 0b11);
                _colors.Add(Color.FromArgb(r, g, b));
            }
        }

        private Palette(IList<Color> colors, int offset, int count)
        {
            _colors.AddRange(colors.Skip(offset).Take(count));
        }

        public override string ToString()
        {
            if (_offset > 0)
            {
                return $"{_colors.Count} colours @ {_offset:X}";
            }

            return $"{_colors.Count} colours (copy, do not save this)";
        }

        private static int ScaleColor(int c) => c switch
        {
            0 => 0x00,
            1 => 0x55,
            2 => 0xaa,
            _ => 0xff
        };

        public static IEnumerable<Palette> ReadPalettes(Memory mem, int offset, int count)
        {
            var counts = new[]
            {
                2, 2, 2, 2, 2, 2, 2, 2, // Regular tile+sprite palettes
                3, 3, 3, 3, 4, 12, 4, 1, 4 // Cycling palette 
            };

            for (var i = 0; i < count; i++)
            {
                var address = mem.Word(offset);
                var palette = new Palette(mem, address, counts[i]);
                yield return palette;
                offset += 2;
            }
        }

        public Image ToImage(int width)
        {
            var rows = _colors.Count / 16;
            var rectSize = width / 16;
            var bmp = new Bitmap(width, rows * rectSize);
            using var g = Graphics.FromImage(bmp);
            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < 16; x++)
                {
                    var index = x + y * 16;
                    using var b = new SolidBrush(_colors[index]);
                    g.FillRectangle(b, rectSize*x, rectSize*y, rectSize, rectSize);
                }
            }

            return bmp;
        }

        public Palette GetSubPalette(int start, int count)
        {
            return new Palette(_colors, start, count);
        }
    }
}