using System.Collections.Generic;

namespace sth1edwv
{
    public class Floor: IDataItem
    {
        public byte[] BlockIndices { get; }

        public Floor(Cartridge cartridge, int offset, int compressedSize, int maximumCompressedSize, int width)
        {
            Offset = offset;
            MaximumCompressedSize = maximumCompressedSize;
            BlockIndices = Compression.DecompressRle(cartridge, offset, compressedSize);
            Width = width;
        }

        public int Offset { get; set; }
        public int MaximumCompressedSize { get; }
        public int Width { get; }

        public IList<byte> GetData()
        {
            return Compression.CompressRle(BlockIndices);
        }
    }
}
