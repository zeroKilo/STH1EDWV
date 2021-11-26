using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace sth1edwv.GameObjects
{
    public class TileSet: IDataItem, IDisposable
    {
        private readonly int _bitPlanes;
        public bool Compressed { get; }
        public List<Tile> Tiles { get; }

        public static class Groupings
        {
            /// <summary>
            /// A single tile
            /// </summary>
            public static readonly List<Point> Single = new() { new Point(0, 0) };

            /// <summary>
            /// A sprite tile in 8x16 mode
            /// </summary>
            public static readonly List<Point> Sprite = new() { new Point(0, 0), new Point(0, 8) };

            /// <summary>
            /// Rings are ordered
            /// <code>
            /// AB
            /// CD
            /// </code>
            /// </summary>
            public static readonly List<Point> Ring = new()
            {
                new Point(0, 0), new Point(8, 0),
                new Point(0, 8), new Point(8, 8)
            };


            /// <summary>
            /// Monitor screens are ordered
            /// <code>
            /// AC
            /// BD
            /// </code>
            /// </summary>
            public static readonly List<Point> Monitor = new()
            {
                new Point(0, 0), new Point(0, 8),
                new Point(8, 0), new Point(8, 8)
            };

            /// <summary>
            /// Sonic is built from six 8x16 sprites:
            /// <code>
            /// ACE
            /// BDF
            /// GIK
            /// HJL
            /// </code>
            /// </summary>
            public static readonly List<Point> Sonic = new()
            {
                new Point(0, 0), new Point(0, 8),
                new Point(8, 0), new Point(8, 8),
                new Point(16, 0), new Point(16, 8),
                new Point(0, 16), new Point(0, 24),
                new Point(8, 16), new Point(8, 24),
                new Point(16, 16), new Point(16, 24)
            };
        }

        /// <summary>
        /// Uncompressed data version
        /// </summary>
        public TileSet(Memory memory, int offset, int length, int bitPlanes = 4, List<Point> grouping = null, int tilesPerRow = 16)
        {
            // Raw VRAM data
            Offset = offset;
            TilesPerRow = tilesPerRow;
            Compressed = false;
            _bitPlanes = bitPlanes;

            // Default grouping
            grouping ??= Groupings.Single;

            // Read the data and convert to tiles
            Tiles = new EnumerableMemoryStream(memory.GetStream(offset, length))
                .PlanarToChunky(bitPlanes)
                .ToChunks(64 * grouping.Count)
                .Select((x, index) => new Tile(x, grouping, index))
                .ToList();
        }

        /// <summary>
        /// Compressed data version
        /// </summary>
        public TileSet(Memory memory, int offset, List<Point> grouping, int tilesPerRow = 16)
        {
            Offset = offset;
            TilesPerRow = tilesPerRow;
            Compressed = true;

            // Default grouping
            grouping ??= Groupings.Single;

            var decompressed = Compression.DecompressArt(memory, offset, out _);
            Tiles = decompressed
                .ToChunks(64 * grouping.Count)
                .Select((x, index) => new Tile(x, grouping, index))
                .ToList();
        }

        public TileSet(Bitmap image, TileSet baseTileSet)
        {
            if (image.Width % 8 != 0 || image.Height % 8 != 0)
            {
                throw new Exception($"Image is {image.Width}x{image.Height}, both dimensions must be a multiple of 8");
            }
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new Exception("Image is not 8bpp");
            }

            var tiles = new List<byte[]>();

            // We split the image into 8x8 tiles and build the unique set
            var data = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

            // We walk over the image and get one tile at a time
            for (var y = 0; y < image.Height; y += 8)
            for (var x = 0; x < image.Width; x += 8)
            {
                // Get the tile at x, y
                var buffer = new byte[64];
                for (var row = 0; row < 8; ++row)
                {
                    Marshal.Copy(data.Scan0 + (y + row) * data.Stride + x, buffer, row*8, 8);
                }
                // Check if we already have it
                if (!tiles.Any(x => x.SequenceEqual(buffer)))
                {
                    // No, so add it
                    tiles.Add(buffer);
                }
            }
            image.UnlockBits(data);

            // We have a maximum of 255 distinct tiles
            if (tiles.Count > 255)
            {
                throw new Exception($"Image has {tiles.Count} unique tiles, the limit is 255");
            }

            // And they must all be in the range 0..15
            if (tiles.SelectMany(x => x).Any(x => x > 15))
            {
                throw new Exception("Image has pixels with index higher than 15");
            }

            // If we get this far then we are good to go. 
            Offset = -1;
            TilesPerRow = 16;
            // We copy these from the base to ensure we match formats on serialization.
            Compressed = baseTileSet.Compressed;
            _bitPlanes = baseTileSet._bitPlanes;
            Tiles = tiles.Select((x, index) => new Tile(x, TileSet.Groupings.Single, index)).ToList();
        }

        public int Offset { get; set; }
        public int TilesPerRow { get; }

        public IList<byte> GetData()
        {
            if (Compressed)
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
            else
            {
                // Uncompressed, so we convert chunky to planar and then deal with bitplane removal.
                // First we get all the sprites...
                using var ms = new MemoryStream();
                foreach (var tile in Tiles)
                {
                    tile.WriteTo(ms);
                }
                // Then we convert to planar and return it
                ms.Position = 0;
                return new EnumerableMemoryStream(ms).ChunkyToPlanar(_bitPlanes).ToList();
            }
        }

        public void Dispose()
        {
            foreach (var tile in Tiles)
            {
                tile.Dispose();
            }
            Tiles.Clear();
        }

        public Bitmap ToImage(Palette palette, int tilesPerRow)
        {
            // We write the tileset to an image
            // Keeping it all in 8bpp is a pain!
            var blockWidth = Tiles[0].Width;
            var blockHeight = Tiles[0].Height;
            var columns = tilesPerRow;
            var rows = Tiles.Count / columns + (Tiles.Count % columns == 0 ? 0 : 1);
            // Caller disposes
            var image = new Bitmap(columns * blockWidth, rows * blockHeight, PixelFormat.Format8bppIndexed);
            image.Palette = palette.ImagePalette;
            var data = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);
            var index = 0;
            foreach (var tile in Tiles)
            {
                var x = index % columns * blockWidth;
                var y = index / columns * blockHeight;
                ++index;
                // We copy the data from the source image one row at a time
                var sourceImage = tile.GetImage(palette);
                var sourceData = sourceImage.LockBits(
                    new Rectangle(0, 0, blockWidth, blockHeight),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format8bppIndexed);
                var rowData = new byte[blockWidth];
                for (var row = 0; row < blockHeight; ++row)
                {
                    Marshal.Copy(
                        sourceData.Scan0 + row * sourceData.Stride,
                        rowData, 
                        0, 
                        blockWidth);
                    Marshal.Copy(
                        rowData, 
                        0, 
                        data.Scan0 + (row + y) * data.Stride + x,
                        blockWidth);
                }
                sourceImage.UnlockBits(sourceData);
            }
            image.UnlockBits(data);
            return image; // Caller disposes
        }

        public void FromImage(Bitmap image)
        {
            var blocks = Tiles.ToList();
            var blockWidth = blocks[0].Width;
            var blockHeight = blocks[0].Height;

            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
            {
                throw new Exception("Image is not paletted!");
            }

            if (image.Width % blockWidth != 0 || image.Height % blockHeight != 0)
            {
                throw new Exception($"Image is {image.Width}x{image.Height}, must be a multiple of {blockWidth}x{blockHeight}");
            }

            var columns = image.Width / blockWidth;
            var imageTileCount =  columns * (image.Height / blockHeight);
            if (imageTileCount < Tiles.Count)
            {
                throw new Exception($"Image defines {imageTileCount} tiles, we need {Tiles.Count}");
            }

            // We walk over the image and extract each tile...
            var data = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height), 
                ImageLockMode.ReadOnly,
                PixelFormat.Format8bppIndexed);
            // Make a buffer
            var tileData = new byte[Tiles[0].Width * Tiles[0].Height];

            foreach (var tile in Tiles)
            {
                // First get the tile's source coordinates
                var tileX = tile.Index % columns * tile.Width;
                var tileY = tile.Index / columns * tile.Height;

                // Then we extract the data for each row of the tile
                for (var row = 0; row < tile.Height; ++row)
                {
                    // Copy source data into the buffer
                    Marshal.Copy(
                        data.Scan0 + (tileY + row) * data.Stride + tileX,
                        tileData,
                        row * tile.Width,
                        tile.Width);
                }

                // Finally we send it to the tile
                tile.SetData(tileData);
            }

            image.UnlockBits(data);
        }

        public override string ToString()
        {
            return $"{Tiles.Count} tiles @ {Offset:X}..{Offset+GetData().Count-1:X}";
        }

        public void SetRings(Tile ring)
        {
            Tiles[252].SetRingTile(ring, 0, 0);
            Tiles[253].SetRingTile(ring, 8, 0);
            Tiles[254].SetRingTile(ring, 0, 8);
            Tiles[255].SetRingTile(ring, 8, 8);
        }
    }
}
