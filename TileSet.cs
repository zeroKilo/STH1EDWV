using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace sth1edwv
{
    public class TileSet
    {
        private readonly ushort _magic;
        private readonly ushort _dupRows;
        private readonly ushort _artData;
        private readonly ushort _rowCount;
        public List<Tile> Tiles { get; } = new List<Tile>();

        public TileSet(Cartridge cartridge, int offset, Palette pal)
        {
            var address = offset;
            _magic = BitConverter.ToUInt16(cartridge.Memory, address);
            _dupRows = BitConverter.ToUInt16(cartridge.Memory, address + 2);
            _artData = BitConverter.ToUInt16(cartridge.Memory, address + 4);
            _rowCount = BitConverter.ToUInt16(cartridge.Memory, address + 6);
            int bitmaskPos = address + 8;
            int artPos = address + _artData;
            int dupPos = address + _dupRows;
            var tileCount = _rowCount / 8;
            for (int i = 0; i < tileCount; i++)
            {
                var bitmask = cartridge.Memory[bitmaskPos++];
                Tiles.Add(new Tile(cartridge, bitmask, address + _artData, ref artPos, ref dupPos, pal));
            }
        }

        public TreeNode ToNode()
        {
            TreeNode result = new TreeNode("Tile Set");
            TreeNode t = new TreeNode("Header");
            t.Nodes.Add($"Magic           = 0x{_magic:X4}");
            t.Nodes.Add($"Duplicate Rows  = 0x{_dupRows:X4}");
            t.Nodes.Add($"Art Data Offset = 0x{_artData:X4}");
            t.Nodes.Add($"Row Count       = 0x{_rowCount:X4}");
            result.Nodes.Add(t);
            return result;
        }
    }
}
