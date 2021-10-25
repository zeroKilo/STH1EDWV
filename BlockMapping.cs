using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace sth1edwv
{
    public class BlockMapping
    {
        private readonly uint _blockCount;
        public readonly List<byte[]> blocks;
        //public readonly List<Color[,]> imagedata;
        public List<Bitmap> Images { get; } = new List<Bitmap>();
        
        public BlockMapping(Cartridge cartridge, uint address, byte solidityIndex, TileSet tileSet)
        {
            // Hard-coded block counts...
            switch (address)
            {
                case 0x10000:
                    _blockCount = 184;
                    break;
                case 0x10B80:
                    _blockCount = 144;
                    break;
                case 0x11480:
                    _blockCount = 160;
                    break;
                case 0x11E80:
                    _blockCount = 176;
                    break;
                case 0x12980:
                    _blockCount = 192;
                    break;
                case 0x13580:
                    _blockCount = 216;
                    break;
                case 0x14300:
                    _blockCount = 104;
                    break;
                case 0x14980:
                    _blockCount = 132;
                    break;
            }
            // Read block mapping (16B per block)
            MemoryStream m = new MemoryStream();
            m.Write(cartridge.Memory, (int)address, (int)_blockCount * 16);
            m.Seek(0, 0);
            // Read solidity pointer
            MemoryStream m2 = new MemoryStream();
            ushort offset = BitConverter.ToUInt16(cartridge.Memory, 0x3A65 + solidityIndex * 2);
            m2.Write(cartridge.Memory, offset, (int)_blockCount);
            m2.Seek(0, 0);
            // Each block is 16B from m and 1 byte from m2
            blocks = new List<byte[]>();
            for (int i = 0; i < _blockCount; i++)
            {
                byte[] block = new byte[17];
                m.Read(block, 0, 16);
                block[16] = (byte)m2.ReadByte();
                blocks.Add(block);
            }

            for (int i = 0; i < _blockCount; i++)
            {
                // Draw the image
                var bm = new Bitmap(32, 32);
                using (var g = Graphics.FromImage(bm))
                {
                    for (int j = 0; j < 16; ++j)
                    {
                        var y = j / 4;
                        var x = j % 4;
                        var tileIndex = blocks[i][j];
                        var tile = tileSet.Tiles[tileIndex];
                        g.DrawImageUnscaled(tile.Image, x*8, y*8);
                    }
                }
                Images.Add(bm);
            }
        }
    }
}
