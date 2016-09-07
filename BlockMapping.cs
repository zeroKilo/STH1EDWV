using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sth1edwv
{
    public class BlockMapping
    {
        public uint address;
        public uint blockCount;
        public List<byte[]> blocks;
        public BlockMapping(uint _address)
        {
            address = _address;
            switch (address)
            {
                case 0x10000:
                    blockCount = 184;
                    break;
                case 0x10B80:
                    blockCount = 144;
                    break;
                case 0x11480:
                    blockCount = 160;
                    break;
                case 0x11E80:
                    blockCount = 176;
                    break;
                case 0x12980:
                    blockCount = 192;
                    break;
                case 0x13580:
                    blockCount = 216;
                    break;
                case 0x14300:
                    blockCount = 104;
                    break;
                case 0x14980:
                    blockCount = 132;
                    break;
            }
            MemoryStream m = new MemoryStream();
            m.Write(Cartridge.memory, (int)address, (int)blockCount * 16);
            m.Seek(0, 0);
            blocks = new List<byte[]>();
            for (int i = 0; i < blockCount; i++)
            {
                byte[] block = new byte[16];
                m.Read(block, 0, 16);
                blocks.Add(block);
            }
        }
    }
}
