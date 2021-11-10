using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace sth1edwv
{
    public class Tile: IDisposable, IDrawableBlock
    {
        private readonly byte[] _data = new byte[8 * 8];

        public int Index { get; }

        public int Size => 8;

        // Images are per-palette
        private readonly Dictionary<Palette, Bitmap> _images = new();

        private Tile _ringTile;

        public Bitmap GetImage(Palette palette)
        {
            return GetImage(palette, true);
        }

        public Bitmap GetImage(Palette palette, bool withRings)
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
            image.Palette = palette.ImagePalette;
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

        public Tile(byte[] data, int offset, int index)
        {
            Index = index;
            Array.Copy(data, offset, _data, 0, 64);
        }

        public void WriteTo(MemoryStream ms)
        {
            ms.Write(_data, 0, _data.Length);
        }

        public void Dispose()
        {
            DisposeImages();
        }

        private void DisposeImages()
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
            DisposeImages();
        }

        public void CopyFrom(Tile other)
        {
            Array.Copy(other._data, _data, _data.Length);
            DisposeImages();
        }
    }
}