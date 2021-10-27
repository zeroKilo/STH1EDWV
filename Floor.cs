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
            BlockIndices = Compression.DecompressRle(cartridge, address, size);
        }

        public byte[] CompressData()
        {
            return Compression.CompressRle(BlockIndices);
        }

        public int Offset { get; }
        public int LengthConsumed { get; }
        public IList<byte> GetData()
        {
            return Compression.CompressRle(BlockIndices);
        }
    }
}
