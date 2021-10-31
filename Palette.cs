using System;
using System.Collections.Generic;
using System.Drawing;

namespace sth1edwv
{
    public class Palette
    {
        private readonly string _label;
        public IList<Color> Colors { get; } = new List<Color>();

        private Palette(byte[] mem, int offset, string label)
        {
            _label = label;
            for (var i = 0; i < 32; i++)
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
            return _label;
        }

        private static int ScaleColor(int c)
        {
            switch (c)
            {
                case 0:
                    return 0;
                case 1:
                    return 80;
                case 2:
                    return 175;
                case 3:
                    return 255;
            }
            return 0;
        }

        public static IEnumerable<Palette> ReadPalettes(byte[] mem, int offset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var address = BitConverter.ToUInt16(mem, offset);
                yield return new Palette(mem, address, $"{i} @ {offset:X}");
                offset += 2;
            }
        }

        public Image ToImage(int width, int height)
        {
            var bmp = new Bitmap(width, height);
            var rectWidth = width / 8;
            var rectHeight = height / 4;
            using var g = Graphics.FromImage(bmp);
            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    var index = x + y * 8;
                    using var b = new SolidBrush(Colors[index]);
                    g.FillRectangle(b, rectWidth*x, rectHeight*y, rectWidth, rectHeight);
                }
            }

            return bmp;
        }
    }
}