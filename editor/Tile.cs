using System;
using System.Drawing;
using System.IO;

namespace sth1edwv
{
    public class Tile: IDisposable, IDrawableBlock
    {
        private readonly byte[] _data = new byte[8 * 8];
        private readonly Palette _palette;
        private Bitmap _image;
        private Bitmap _imageWithoutRings;

        public int Index { get; }
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
                for (var i = 0; i < _data.Length; ++i)
                {
                    var x = i % 8;
                    var y = i / 8;
                    _image.SetPixel(x, y, _palette.Colors[_data[i]]);
                }
                return _image;
            }
        }

        public Image ImageWithoutRings => _imageWithoutRings ?? Image;

        public Tile(byte[] data, int offset, Palette palette, int index)
        {
            _palette = palette;
            Index = index;
            Array.Copy(data, offset, _data, 0, 64);
        }

        public void WriteTo(MemoryStream ms)
        {
            ms.Write(_data, 0, _data.Length);
        }

        public void Dispose()
        {
            _image?.Dispose();
            _imageWithoutRings?.Dispose();
        }

        public void SetRingImage(Bitmap image)
        {
            _imageWithoutRings = Image;
            _image = (Bitmap)image.Clone();
        }

        public void SetData(int x, int y, int index)
        {
            _data[x + y * 8] = (byte)index;
            _image = null;
        }
    }
}