using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace sth1edwv
{
    public class TileSet: IDataItem, IDisposable
    {
        private readonly ushort _magic;
        private readonly ushort _dupRows;
        private readonly ushort _artData;
        private readonly ushort _rowCount;
        private readonly double _compression;
        public List<Tile> Tiles { get; } = new();

        public TileSet(Cartridge cartridge, int offset, Palette palette, bool addRings)
        {
            Offset = offset;
            _magic = BitConverter.ToUInt16(cartridge.Memory, offset);
            _dupRows = BitConverter.ToUInt16(cartridge.Memory, offset + 2);
            _artData = BitConverter.ToUInt16(cartridge.Memory, offset + 4);
            _rowCount = BitConverter.ToUInt16(cartridge.Memory, offset + 6);
            var decompressed = Compression.DecompressArt(cartridge.Memory, offset, out var lengthConsumed);
            for (var i = 0; i < decompressed.Length; i += 64)
            {
                Tiles.Add(new Tile(decompressed, i, palette, i / 64));
            }
            LengthConsumed = lengthConsumed;
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
                    using var ringTile = new Tile(buffer, 0, palette, index);
                    Tiles[index].SetRingImage(ringTile.Image);
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

        public int Offset { get; }
        public int LengthConsumed { get; }

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

        public List<RomBuilder.DataChunk.Reference> GetReferences()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            foreach (var tile in Tiles)
            {
                tile.Dispose();
            }
            Tiles.Clear();
        }
    }
}
