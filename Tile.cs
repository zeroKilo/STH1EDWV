using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace sth1edwv
{
    public class Tile
    {
        private readonly byte[,] _data = new byte[8, 8];
        private readonly Palette _palette;
        private Bitmap _image;

        public Bitmap Image
        {
            get
            {
                if (_image != null)
                {
                    return _image;
                }
                // Lazy rendering
                _image = new Bitmap(8, 8);
                for (int y = 0; y < 8; ++y)
                {
                    for (int x = 0; x < 8; ++x)
                    {
                        _image.SetPixel(x, y, _palette.Colors[_data[x, y]]);
                    }
                }
                return _image;
            }
        }

        public Tile(Cartridge cartridge, byte bitmask, int artBase, ref int artPos, ref int dupPos, Palette palette)
        {
            _palette = palette;
            for (var y = 0; y < 8; ++y)
            {
                IList<int> row;
                var bit = bitmask & 1;
                bitmask >>= 1;
                if (bit == 1)
                {
                    // Duplicate
                    var duplicateOffset = (int)cartridge.Memory[dupPos++];
                    if ((duplicateOffset & 0xf0) == 0xf0)
                    {
                        // 0xfX 0xYY means index 0xXYY
                        duplicateOffset &= 0xf;
                        duplicateOffset <<= 8;
                        duplicateOffset |= cartridge.Memory[dupPos++];
                    }

                    row = ReadArt(cartridge, artBase + duplicateOffset * 4).ToList();
                }
                else
                {
                    row = ReadArt(cartridge, artPos).ToList();
                    artPos += 4;
                }

                // Apply row to tile
                for (int x = 0; x < 8; x++)
                {
                    _data[x, y] = (byte)row[x];
                }
            }
        }

        private IEnumerable<int> ReadArt(Cartridge cartridge, int offset)
        {
            // Read a row
            var data = new byte[4];
            Array.Copy(cartridge.Memory, offset, data, 0, 4); 
            // This is planar, we convert to chunky
            for (int i = 0; i < 8; ++i)
            {
                var value = 0;
                for (int b = 0; b < 4; ++b)
                {
                    var bit = (data[b] >> (7-i)) & 1;
                    value |= bit << b;
                }

                yield return value;
            }
        }

        public Color getColor(int x, int y)
        {
            return Image.GetPixel(x, y);
        }
    }
}