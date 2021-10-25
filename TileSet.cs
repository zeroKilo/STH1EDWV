using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sth1edwv
{
    public class TileSet
    {
        public uint address;
        public ushort magic;
        public ushort dupRows;
        public ushort artData;
        public ushort rowCount;
        public byte[] uniRows;
        public List<Color[,]> tiles;
        public Color[] palette;

        public TileSet(uint offset, Color[] pal)
        {
            address = offset;
            palette = pal;
            magic = BitConverter.ToUInt16(Cartridge.memory, (int)address);
            dupRows = BitConverter.ToUInt16(Cartridge.memory, (int)address + 2);
            artData = BitConverter.ToUInt16(Cartridge.memory, (int)address + 4);
            rowCount = BitConverter.ToUInt16(Cartridge.memory, (int)address + 6);
            uniRows = new byte[rowCount / 8];
            for (int i = 0; i < rowCount / 8; i++)
                uniRows[i] = Cartridge.memory[address + i + 8];
            tiles = new List<Color[,]>();
            uint artPos = address + artData;
            uint dupPos = address + dupRows;
            for (int i = 0; i < uniRows.Length; i++)
                tiles.Add(readTile(i, ref artPos, ref dupPos));
        }


        public TreeNode ToNode()
        {
            TreeNode result = new TreeNode("Tile Set");
            TreeNode t = new TreeNode("Header");
            t.Nodes.Add("Magic           = 0x" + magic.ToString("X4"));
            t.Nodes.Add("Duplicate Rows  = 0x" + dupRows.ToString("X4"));
            t.Nodes.Add("Art Data Offset = 0x" + artData.ToString("X4"));
            t.Nodes.Add("Row Count       = 0x" + rowCount.ToString("X4"));
            result.Nodes.Add(t);
            return result;
        }

        private Color[,] readTile(int index, ref uint artPos, ref uint dupPos)
        {
            Color[,] result = new Color[8, 8];
            byte b = uniRows[index];
            for (int y = 0; y < 8; y++)
            {
                Color[] row;
                if ((b & (1 << y)) != 0)
                {
                    ushort r = Cartridge.memory[dupPos++];
                    if (r >= 0xF0)
                        r = (ushort)(((r - 0xF0) << 8) + Cartridge.memory[dupPos++]);
                    row = readArt(address + artData + r * 4u);
                }
                else
                {
                    row = readArt(artPos);
                    artPos += 4;
                }
                for (int x = 0; x < 8; x++)
                    result[x, y] = row[x];
            }
            return result;
        }

        public Color[] readArt(uint offset)
        {
            Color[] result = new Color[8];
            byte byte1 = Cartridge.memory[offset];
            byte byte2 = Cartridge.memory[offset + 1];
            byte byte3 = Cartridge.memory[offset + 2];
            byte byte4 = Cartridge.memory[offset + 3];
            byte[] pixel = new byte[8];
            for (int bit = 0; bit < 8; bit++)
            {
                if ((byte1 & (1 << (7 - bit))) != 0) pixel[bit] |= 1;
                if ((byte2 & (1 << (7 - bit))) != 0) pixel[bit] |= 2;
                if ((byte3 & (1 << (7 - bit))) != 0) pixel[bit] |= 4;
                if ((byte4 & (1 << (7 - bit))) != 0) pixel[bit] |= 8;
                result[bit] = palette[pixel[bit]];
            }
            return result;
        }
    }
}
