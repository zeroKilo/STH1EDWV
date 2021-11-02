using System;
using System.Collections.Generic;
using System.Linq;

namespace sth1edwv
{
    public class BlockMapping: IDisposable, IDataItem
    {
        public List<Block> Blocks { get; } = new();
    
        public BlockMapping(Cartridge cartridge, int address, byte solidityIndex, TileSet tileSet)
        {
            // Hard-coded block counts...
            // TODO: make these safer
            var blockCount = address switch
            {
                0x10000 => 184,
                0x10B80 => 144,
                0x11480 => 160,
                0x11E80 => 176,
                0x12980 => 192,
                0x13580 => 216,
                0x14300 => 104,
                0x14980 => 128,
                _ => 0
            };
            var solidityOffset = BitConverter.ToUInt16(cartridge.Memory, 0x3A65 + solidityIndex * 2);
            for (var i = 0; i < blockCount; ++i)
            {
                Blocks.Add(new Block(cartridge.Memory, address + i * 16, solidityOffset + i, tileSet, i));
            }

            Offset = address;
            LengthConsumed = blockCount * 16;
        }

        public void Dispose()
        {
            foreach (var block in Blocks)
            {
                block.Dispose();
            }
            Blocks.Clear();
        }

        public int Offset { get; }
        public int LengthConsumed { get; }

        public IList<byte> GetData()
        {
            throw new NotImplementedException();
        }

        public void UpdateUsageForLevel(Level level)
        {
            foreach (var block in Blocks)
            {
                block.UsageCount = 0;
            }

            foreach (var index in level.Floor.BlockIndices.Where(x => x < Blocks.Count))
            {
                ++Blocks[index].UsageCount;
            }
        }
    }
}
