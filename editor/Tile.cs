using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace sth1edwv
{
    public class Tile: IDisposable, IDrawableBlock
    {
        private readonly byte[] _data = new byte[8 * 8];

        public int Index { get; }

        // Default image is the default palette and rings included (if set)
        public Bitmap Image => GetImage(_defaultPalette);

        // Images are per-palette
        private readonly Dictionary<Palette, Bitmap> _images = new();
        private readonly Palette _defaultPalette;

        private Tile _ringTile;

        public Bitmap GetImage(Palette palette, bool withRings = true)
        {
            if (withRings && _ringTile != null)
            {
                return _ringTile.GetImage(palette);
            }

            if (_images.TryGetValue(palette, out var image))
            {
                return image;
            }

            // We render only when needed. We do stick to paletted images though..
            image = new Bitmap(8, 8, PixelFormat.Format8bppIndexed);
            // We have to duplicate the palette to make it pick it up. This gets us a copy...
            var imagePalette = image.Palette;
            // We have to clear the whole palette to non-transparent colours to avoid (1) the default palette and (2) an image that GDI+ transforms to 32bpp on load
            for (int i = 0; i < 256; ++i)
            {
                imagePalette.Entries[i] = Color.Magenta;
            }

            // Then we apply the real palette to the start
            palette.Colors.CopyTo(imagePalette.Entries, 0);
            // And apply it back to the image
            image.Palette = imagePalette;
            var data = image.LockBits(
                new Rectangle(0, 0, 8, 8),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);
            for (int row = 0; row < 8; ++row)
            {
                // Copy indices one row at a time
                Marshal.Copy(_data, row * 8, data.Scan0 + row * data.Stride, 8);
            }

            image.UnlockBits(data);
            _images.Add(palette, image);
            return image;
        }

        public Tile(byte[] data, Palette palette, int offset, int index)
        {
            Index = index;
            Array.Copy(data, offset, _data, 0, 64);
            _defaultPalette = palette;
        }

        public void WriteTo(MemoryStream ms)
        {
            ms.Write(_data, 0, _data.Length);
        }

        public void Dispose()
        {
            disposeImages();
        }

        private void disposeImages()
        {
            foreach (var image in _images.Values)
            {
                image.Dispose();
            }
            _images.Clear();
        }

        public void SetRingVersion(Tile tile)
        {
            _ringTile = tile;
        }

        public void SetData(int x, int y, int index)
        {
            _data[x + y * 8] = (byte)index;
            disposeImages();
        }

        public void CopyFrom(Tile other)
        {
            Array.Copy(other._data, _data, _data.Length);
            disposeImages();
        }
    }
}