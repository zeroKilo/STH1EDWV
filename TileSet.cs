using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace sth1edwv
{
    public class TileSet: IDataItem
    {
        private readonly ushort _magic;
        private readonly ushort _dupRows;
        private readonly ushort _artData;
        private readonly ushort _rowCount;
        public List<Tile> Tiles { get; } = new List<Tile>();

        public TileSet(Cartridge cartridge, int offset, Palette palette)
        {
            Offset = offset;
            _magic = BitConverter.ToUInt16(cartridge.Memory, offset);
            _dupRows = BitConverter.ToUInt16(cartridge.Memory, offset + 2);
            _artData = BitConverter.ToUInt16(cartridge.Memory, offset + 4);
            _rowCount = BitConverter.ToUInt16(cartridge.Memory, offset + 6);
            var decompressed = Compression.DecompressArt(cartridge.Memory, offset);
            for (int i = 0; i < decompressed.Length; i += 64)
            {
                Tiles.Add(new Tile(decompressed, i, palette));
            }

            //LengthConsumed = Math.Max(artPos, dupPos) - Offset;
        }

        public TreeNode ToNode()
        {
            TreeNode result = new TreeNode("Tile Set");
            TreeNode t = new TreeNode("Header");
            t.Nodes.Add($"Magic           = 0x{_magic:X4}");
            t.Nodes.Add($"Duplicate Rows  = 0x{_dupRows:X4}");
            t.Nodes.Add($"Art BlockIndices Offset = 0x{_artData:X4}");
            t.Nodes.Add($"Row Count       = 0x{_rowCount:X4}");
            result.Nodes.Add(t);
            return result;
        }

        public int Offset { get; }
        public int LengthConsumed { get; }

        public IList<byte> GetData()
        {
            using (var ms = new MemoryStream())
            {
                // We gather our tiles back into a big buffer...
                foreach (var tile in Tiles)
                {
                    tile.WriteTo(ms);
                }

                // Then we compress it...
                return Compression.CompressArt(ms.ToArray());
            }
        }
    }
}
