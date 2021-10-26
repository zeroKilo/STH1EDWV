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

        public TileSet(Cartridge cartridge, int offset, Palette pal)
        {
            Offset = offset;
            _magic = BitConverter.ToUInt16(cartridge.Memory, offset);
            _dupRows = BitConverter.ToUInt16(cartridge.Memory, offset + 2);
            _artData = BitConverter.ToUInt16(cartridge.Memory, offset + 4);
            _rowCount = BitConverter.ToUInt16(cartridge.Memory, offset + 6);
            int bitmaskPos = offset + 8;
            int artPos = offset + _artData;
            int dupPos = offset + _dupRows;
            var tileCount = _rowCount / 8;
            for (int i = 0; i < tileCount; i++)
            {
                var bitmask = cartridge.Memory[bitmaskPos++];
                Tiles.Add(new Tile(cartridge, bitmask, offset + _artData, ref artPos, ref dupPos, pal));
            }

            LengthConsumed = Math.Max(artPos, dupPos) - Offset;
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
            return GetDataInternal().ToList();
        }

        private IEnumerable<byte> GetDataInternal()
        {
            var artData = new List<byte[]>();
            var duplicates = new List<int>();
            var bitmasks = new List<byte>();
            using (var ms = new MemoryStream())
            {
                // We gather our tiles back into a big buffer...
                foreach (var tile in Tiles)
                {
                    tile.WriteTo(ms);
                }
                // Then we work through it one row at a time...
                ms.Position = 0;
                byte bitmask = 0;
                int linesConsumed = 0;
                while (ms.Position < ms.Length)
                {
                    // Read a line
                    var line = new byte[8];
                    ms.Read(line, 0, 8);
                    ++linesConsumed;
                    bitmask <<= 1;
                    // See if we already have it
                    var index = artData.IndexOf(line);

                    if (index == -1)
                    {
                        // If not found, add to art data
                        artData.Add(line);
                    }
                    else
                    {
                        // If found, add the reference
                        duplicates.Add(index);
                        // And set the bit
                        bitmask |= 1;
                    }
                    // If we have finished a tile, emit the bitmask
                    if (linesConsumed == 8)
                    {
                        bitmasks.Add(bitmask);
                        linesConsumed = 0;
                    }
                }
            }
            // Now we emit it back...
            // Header
            foreach (var c in Encoding.ASCII.GetBytes("YH")) yield return c;
            // Duplicates offset = 8 + bitmask count + art size
            var duplicatesOffset = 8 + bitmasks.Count + artData.Count * 4;
            foreach (var b in BitConverter.GetBytes((ushort)duplicatesOffset)) yield return b;
            // Art offset = 8 + bitmask count
            var artOffset = 8 + bitmasks.Count;
            foreach (var b in BitConverter.GetBytes((ushort)artOffset)) yield return b;
            // Next the bitmasks
            foreach (var b in bitmasks) yield return b;
            // And the art data, converting to chunky
            foreach (var b in artData.SelectMany(ChunkyToPlanar)) yield return b;
            // Finally the duplicates data
            foreach (var index in duplicates)
            {
                // 1-2 bytes encoding
                if (index < 0xf0)
                {
                    yield return (byte)index;
                }
                else
                {
                    yield return (byte)(0xf0 | (index >> 8));
                    yield return (byte)(index & 0xff);
                }
            }
        }

        private IEnumerable<byte> ChunkyToPlanar(byte[] data)
        {
            for (int plane = 0; plane < 4; ++plane)
            {
                var b = 0;
                for (int i = 0; i < 8; ++i)
                {
                    var bit = (data[i] >> plane) & 1;
                    b |= bit << (7 - i);
                }

                yield return (byte)b;
            }
        }
    }
}
