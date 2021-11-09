using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace sth1edwv
{
    public class Tile: IDisposable, IDrawableBlock
    {
        private readonly byte[] _data = new byte[8 * 8];
        private readonly Palette _palette;
        private Bitmap _image;
        private Bitmap _ringImage;

        public int Index { get; }
        public Bitmap Image
        {
            get
            {
                if (_ringImage != null)
                {
                    return _ringImage;
                }
                if (_image != null)
                {
                    return _image;
                }
                // We render only when needed. We do stick to paletted images though..
                _image = new Bitmap(8, 8, PixelFormat.Format8bppIndexed);
                // We have to duplicate the palette to make it pick it up. This gets us a copy...
                var palette = _image.Palette;
                // We have to clear the whole palette to non-transparent colours to avoid (1) the default palette and (2) an image that GDI+ transforms to 32bpp on load
                for (int i = 0; i < 256; ++i)
                {
                    palette.Entries[i] = Color.Magenta;
                }
                // Then we apply the real palette to the start
                _palette.Colors.CopyTo(palette.Entries, 0);
                // And apply it back to the image
                _image.Palette = palette;
                var data = _image.LockBits(
                    new Rectangle(0, 0, 8, 8), 
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format8bppIndexed);
                for (int row = 0; row < 8; ++row)
                {
                    // Copy indices one row at a time
                    Marshal.Copy(_data, row*8, data.Scan0 + row * data.Stride, 8);
                }
                _image.UnlockBits(data);
                return _image;
            }
        }

        public Bitmap ImageWithoutRings => _image;

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
            _ringImage?.Dispose();
        }

        public void SetRingImage(Bitmap image)
        {
            _ringImage = (Bitmap)image.Clone();
        }

        public void SetData(int x, int y, int index)
        {
            _data[x + y * 8] = (byte)index;
            _image = null;
        }

        public void CopyFrom(Tile other)
        {
            Array.Copy(other._data, _data, _data.Length);
            _image = null;
        }
    }
}