using System.Collections.Generic;

namespace sth1edwv
{
    public class Floor: IDataItem
    {
        public byte[] BlockIndices { get; set; }

        public Floor(Cartridge cartridge, int address, int size)
        {
            Offset = address;
            LengthConsumed = size;
            BlockIndices = RleCompressor.Decompress(cartridge, address, size);
        }

        public byte[] CompressData(Level l)
        {
            return RleCompressor.Compress(BlockIndices);
        }

        public int Offset { get; }
        public int LengthConsumed { get; }
        public IList<byte> GetData()
        {
            return RleCompressor.Compress(BlockIndices);
        }
    }
}
