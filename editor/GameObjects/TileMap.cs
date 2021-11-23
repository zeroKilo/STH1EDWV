using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace sth1edwv.GameObjects
{
    public class TileMap: IDataItem
    {
        private readonly List<ushort> _data;
        private Bitmap _image;

        public TileMap(Memory memory, int offset, int size)
        {
            Offset = offset;
            _data = Compression.DecompressRle(memory, offset, size)
                .Select(b => (ushort)b)
                .ToList();
        }

        public void SetAllForeground()
        {
            for (var i = 0; i < _data.Count; i++)
            {
                _data[i] |= 0x1000;
            }
        }

        public void OverlayWith(TileMap other)
        {
            for (var i = 0; i < _data.Count; i++)
            {
                if (other._data[i] != 0xff)
                {
                    _data[i] = other._data[i];
                }
            }
        }

        public Bitmap GetImage(TileSet tileSet, Palette palette)
        {
            if (_image != null)
            {
                return _image;
            }
            _image = new Bitmap(256, 192);
            using var g = Graphics.FromImage(_image);
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            for (var i = 0; i < _data.Count; ++i)
            {
                var x = i % 32 * 8;
                var y = i / 32 * 8;
                var index = _data[i];
                if (index != 0xff)
                {
                    // Mask off high bits
                    index &= 0xff;
                    g.DrawImageUnscaled(tileSet.Tiles[index].GetImage(palette), x, y);
                }
            }

            return _image;
        }


        public int Offset { get; set; }
        public int TileMap1Size { get; private set; }
        public int TileMap2Size { get; private set; }

        public IList<byte> GetData()
        {
            if (!_data.Any(x => x > 0xff))
            {
                // Single tilemap mode
                var tileMap = Compression.CompressRle(_data.Select(x => (byte)x).ToArray());
                TileMap1Size = tileMap.Length;
                return tileMap;
            }
            // Two tilemaps
            var tileMap1 = Compression.CompressRle(_data.Select(x => x > 0xff ? (byte)(x & 0xff) : (byte)0xff).ToArray());
            var tileMap2 = Compression.CompressRle(_data.Select(x => x > 0xff ? (byte)0xff : (byte)(x & 0xff)).ToArray());
            TileMap1Size = tileMap1.Length;
            TileMap2Size = tileMap2.Length;
            return tileMap1.Concat(tileMap2).ToList();
        }

        public bool IsOverlay()
        {
            return _data.Any(x => (x & 0xff) == 0xff);
        }
    }
}