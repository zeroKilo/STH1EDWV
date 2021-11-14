using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace sth1edwv.GameObjects
{
    public class TileSet: IDataItem, IDisposable
    {
        private readonly bool _isSprites;
        private readonly ushort _magic;
        private readonly ushort _dupRows;
        private readonly ushort _artData;
        private readonly ushort _rowCount;
        private readonly double _compression;
        private readonly int _tileHeight;
        public List<Tile> Tiles { get; } = new();

        public TileSet(Cartridge cartridge, int offset, bool addRings, bool isSprites)
        {
            _isSprites = isSprites;
            Offset = offset;
            _magic = cartridge.Memory.Word(offset);
            _dupRows = cartridge.Memory.Word(offset + 2);
            _artData = cartridge.Memory.Word(offset + 4);
            _rowCount = cartridge.Memory.Word(offset + 6);
            var decompressed = Compression.DecompressArt(cartridge.Memory, offset, out var lengthConsumed);
            _tileHeight = isSprites ? 16 : 8;
            var tileSizeBytes = _tileHeight * 8;
            for (var i = 0; i < decompressed.Length; i += tileSizeBytes)
            {
                Tiles.Add(new Tile(decompressed, i, i / tileSizeBytes, _tileHeight));
            }
            _compression = (double)(decompressed.Length - lengthConsumed) / decompressed.Length;

            if (addRings)
            {
                // We replace the last 4 tiles' image
                for (var i = 0; i < 4; ++i)
                {
                    var index = 252 + i;
                    // We want to convert the data from raw VDP to one byte per pixel
                    const int ringOffset = 0x2FD70 + 32 * 4 * 3; // Frame 3 of the animation looks good
                    var buffer = Compression.PlanarToChunky(cartridge.Memory, ringOffset + i * 32, 8).ToArray();
                    var ringTile = new Tile(buffer, 0, index, 8);
                    Tiles[index].SetRingVersion(ringTile);
                }
            }
        }

        public TreeNode ToNode()
        {
            return new TreeNode($"Tile Set @ {Offset:X6}")
            {
                Nodes =
                {
                    new TreeNode("Header")
                    {
                        Nodes =
                        {
                            new TreeNode($"Magic           = 0x{_magic:X4}"),
                            new TreeNode($"Duplicate Rows  = 0x{_dupRows:X4}"),
                            new TreeNode($"Art Data Offset = 0x{_artData:X4}"),
                            new TreeNode($"Row Count       = 0x{_rowCount:X4}")
                        }
                    },
                    new TreeNode($"{Tiles.Count} tiles"),
                    new TreeNode($"{_compression:P} compression")
                }
            };
        }

        public int Offset { get; set; }

        public IList<byte> GetData()
        {
            using var ms = new MemoryStream();
            // We gather our tiles back into a big buffer...
            foreach (var tile in Tiles)
            {
                tile.WriteTo(ms);
            }

            // Then we compress it...
            ms.Position = 0;
            return Compression.CompressArt(ms);
        }

        public void Dispose()
        {
            foreach (var tile in Tiles)
            {
                tile.Dispose();
            }
            Tiles.Clear();
        }

        public Bitmap ToImage(Palette palette)
        {
            // We write the tileset to an image
            // Keeping it all in 8bpp is a pain!
            var rows = Tiles.Count / 16;
            // Caller disposes
            var image = new Bitmap(128, rows * _tileHeight, PixelFormat.Format8bppIndexed);
            image.Palette = palette.ImagePalette;
            var data = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);
            foreach (var tile in Tiles)
            {
                var x = tile.Index % 16 * 8;
                var y = tile.Index / 16 * _tileHeight;
                // We copy the data from the source image one row at a time
                var sourceImage = tile.GetImage(palette, false);
                var sourceData = sourceImage.LockBits(
                    new Rectangle(0, 0, 8, _tileHeight),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format8bppIndexed);
                var rowData = new byte[8];
                for (var row = 0; row < _tileHeight; ++row)
                {
                    Marshal.Copy(
                        sourceData.Scan0 + row * sourceData.Stride,
                        rowData, 
                        0, 
                        8);
                    Marshal.Copy(
                        rowData, 
                        0, 
                        data.Scan0 + (row + y) * data.Stride + x,
                        8);
                }
                sourceImage.UnlockBits(sourceData);
            }
            image.UnlockBits(data);
            return image; // Caller disposes
        }

        public void FromImage(Bitmap image)
        {
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new Exception("Image is not paletted!");
            }

            var expectedHeight = Tiles.Count / 16 * _tileHeight;
            if (image.Width != 128 || image.Height != expectedHeight)
            {
                throw new Exception($"Clipboard image is {image.Width}x{image.Height}, we need 128x{expectedHeight}");
            }

            // We walk over the image and extract each tile...
            var data = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height), 
                ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            // Make a buffer
            var rowData = new byte[8];
            foreach (var tile in Tiles)
            {
                // First get the tile's source coordinates
                var tileX = tile.Index % 16 * 8;
                var tileY = tile.Index / 16 * _tileHeight;

                for (var row = 0; row < _tileHeight; ++row)
                {
                    // Copy source data into the buffer
                    Marshal.Copy(
                        data.Scan0 + (tileY + row) * data.Stride + tileX,
                        rowData,
                        0,
                        8);

                    // Then into the tile
                    for (var x = 0; x < 8; ++x)
                    {
                        tile.SetData(x, row, rowData[x] & 0xf);
                    }
                }
            }
            image.UnlockBits(data);
        }

        public override string ToString()
        {
            return $"{Tiles.Count} tiles @ {Offset:X}";
        }
    }
}
