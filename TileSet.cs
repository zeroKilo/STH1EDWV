using System;
using System.Collections.Generic;
using System.IO;
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

        public TileSet(Cartridge cartridge, int offset, Palette palette)
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
