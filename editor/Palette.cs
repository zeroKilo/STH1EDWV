using System.Collections.Generic;
using System.Drawing;

namespace sth1edwv
{
    public class Palette
    {
        private readonly int _offset;
        public IList<Color> Colors { get; } = new List<Color>();

        public Palette(IReadOnlyList<byte> mem, int offset, int paletteCount)
        {
            _offset = offset;
            for (var i = 0; i < 16*paletteCount; i++)
            {
                var color = mem[offset + i];
                var r = ScaleColor((color >> 0) & 0b11);
                var g = ScaleColor((color >> 2) & 0b11);
                var b = ScaleColor((color >> 4) & 0b11);
                Colors.Add(Color.FromArgb(r, g, b));
            }
        }

        public override string ToString()
        {
            return $"{Colors.Count} colours @ {_offset:X}";
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
            var rows = Colors.Count / 16;
            var rectSize = width / 16;
            var bmp = new Bitmap(width, rows * rectSize);
            using var g = Graphics.FromImage(bmp);
            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < 16; x++)
                {
                    var index = x + y * 16;
                    using var b = new SolidBrush(Colors[index]);
                    g.FillRectangle(b, rectSize*x, rectSize*y, rectSize, rectSize);
                }
            }

            return bmp;
        }
    }
}