using System;
using System.Collections.Generic;

namespace sth1edwv
{
    public class BlockMapping: IDisposable
    {
        public List<Block> Blocks { get; } = new List<Block>();
    
        public BlockMapping(Cartridge cartridge, int address, byte solidityIndex, TileSet tileSet)
        {
            // Hard-coded block counts...
            uint blockCount = 0;
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
            var solidityOffset = BitConverter.ToUInt16(cartridge.Memory, 0x3A65 + solidityIndex * 2);
            for (var i = 0; i < blockCount; ++i)
            {
                Blocks.Add(new Block(cartridge.Memory, address + i * 16, solidityOffset + i, tileSet));
            }
        }

        public void Dispose()
        {
            foreach (var block in Blocks)
            {
                block.Dispose();
            }
            Blocks.Clear();
        }
    }
}
