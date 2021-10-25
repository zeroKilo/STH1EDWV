using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;

namespace sth1edwv
{
    public class TileSet
    {
        private readonly uint _address;
        private readonly ushort _magic;
        private readonly ushort _dupRows;
        private readonly ushort _artData;
        private readonly ushort _rowCount;
        public byte[] UniqueRows { get; }
        public List<Color[,]> Tiles { get; } = new List<Color[,]>();
        private readonly Palette _palette;

        public TileSet(Cartridge cartridge, uint offset, Palette pal)
        {
            _address = offset;
            _palette = pal;
            _magic = BitConverter.ToUInt16(cartridge.Memory, (int)_address);
            _dupRows = BitConverter.ToUInt16(cartridge.Memory, (int)_address + 2);
            _artData = BitConverter.ToUInt16(cartridge.Memory, (int)_address + 4);
            _rowCount = BitConverter.ToUInt16(cartridge.Memory, (int)_address + 6);
            UniqueRows = new byte[_rowCount / 8];
            for (int i = 0; i < _rowCount / 8; i++)
            {
                UniqueRows[i] = cartridge.Memory[_address + i + 8];
            }

            uint artPos = _address + _artData;
            uint dupPos = _address + _dupRows;
            for (int i = 0; i < UniqueRows.Length; i++)
            {
                Tiles.Add(ReadTile(cartridge, i, ref artPos, ref dupPos));
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

        private Color[,] ReadTile(Cartridge cartridge, int index, ref uint artPos, ref uint dupPos)
        {
            Color[,] result = new Color[8, 8];
            byte b = UniqueRows[index];
            for (int y = 0; y < 8; y++)
            {
                Color[] row;
                if ((b & (1 << y)) != 0)
                {
                    ushort r = cartridge.Memory[dupPos++];
                    if (r >= 0xF0)
                        r = (ushort)(((r - 0xF0) << 8) + cartridge.Memory[dupPos++]);
                    row = ReadArt(cartridge, _address + _artData + r * 4u);
                }
                else
                {
                    row = ReadArt(cartridge, artPos);
                    artPos += 4;
                }
                for (int x = 0; x < 8; x++)
                    result[x, y] = row[x];
            }
            return result;
        }

        private Color[] ReadArt(Cartridge cartridge, uint offset)
        {
            Color[] result = new Color[8];
            byte byte1 = cartridge.Memory[offset];
            byte byte2 = cartridge.Memory[offset + 1];
            byte byte3 = cartridge.Memory[offset + 2];
            byte byte4 = cartridge.Memory[offset + 3];
            byte[] pixel = new byte[8];
            for (int bit = 0; bit < 8; bit++)
            {
                if ((byte1 & (1 << (7 - bit))) != 0) pixel[bit] |= 1;
                if ((byte2 & (1 << (7 - bit))) != 0) pixel[bit] |= 2;
                if ((byte3 & (1 << (7 - bit))) != 0) pixel[bit] |= 4;
                if ((byte4 & (1 << (7 - bit))) != 0) pixel[bit] |= 8;
                result[bit] = _palette.Colors[pixel[bit]];
            }
            return result;
        }
    }
}
