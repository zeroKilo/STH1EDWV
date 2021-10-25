using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace sth1edwv
{
    public class BlockMapping
    {
        private readonly uint _address;
        public uint blockCount;
        public List<byte[]> blocks;
        public List<Color[,]> imagedata;
        
        public BlockMapping(Cartridge cartridge, uint address, byte solidityIndex, TileSet tileSet)
        {
            this._address = address;
            switch (this._address)
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
            m.Write(cartridge.Memory, (int)this._address, (int)blockCount * 16);
            m.Seek(0, 0);
            MemoryStream m2 = new MemoryStream();
            ushort offset = BitConverter.ToUInt16(cartridge.Memory, 0x3A65 + solidityIndex * 2);
            m2.Write(cartridge.Memory, offset, (int)blockCount);
            m2.Seek(0, 0);
            blocks = new List<byte[]>();
            for (int i = 0; i < blockCount; i++)
            {
                byte[] block = new byte[17];
                m.Read(block, 0, 16);
                block[16] = (byte)m2.ReadByte();
                blocks.Add(block);
            }
            imagedata = new List<Color[,]>();
            for (int i = 0; i < blockCount; i++)
            {
                Color[,] data = new Color[32, 32];
                for(int by =0;by<4;by++)
                    for (int bx = 0; bx < 4; bx++)
                        for (int ty = 0; ty < 8; ty++)
                            for (int tx = 0; tx < 8; tx++)
                                data[bx * 8 + tx, by * 8 + ty] = tileSet.Tiles[blocks[i][bx + by * 4]][tx, ty];
                imagedata.Add(data);
            }
        }
    }
}
