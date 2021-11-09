using System.Collections.Generic;

namespace sth1edwv
{
    public class Floor: IDataItem
    {
        public byte[] BlockIndices { get; }

        public Floor(Cartridge cartridge, int offset, int compressedSize, int width)
        {
            Offset = offset;
            BlockIndices = Compression.DecompressRle(cartridge, offset, compressedSize);
            Width = width;
        }

        public int Offset { get; set; }
        public int Width { get; }

        public IList<byte> GetData()
        {
            return Compression.CompressRle(BlockIndices);
        }

        public override string ToString()
        {
            return $"{BlockIndices.Length} blocks ({Width}x{BlockIndices.Length / Width}) @ {Offset:X}";
        }
    }
}
