using System.Collections.Generic;
using System.Windows.Forms;

namespace sth1edwv.GameObjects
{
    public class Floor : IDataItem
    {
        public byte[] BlockIndices { get; private set; }

        public Floor(Cartridge cartridge, int offset, int compressedSize, int width)
        {
            Offset = offset;
            BlockIndices = Compression.DecompressRle(cartridge, offset, compressedSize);
            Width = width;
        }

        private Floor(Floor other)
        {
            Width = other.Width;
            BlockIndices = (byte[])other.BlockIndices.Clone();
        }

        public int Offset { get; set; }
        internal int Width { get; private set; }

        public IList<byte> GetData()
        {
            return Compression.CompressRle(BlockIndices);
        }

        public override string ToString()
        {
            return $"{BlockIndices.Length} blocks ({Width}x{BlockIndices.Length / Width}) @ {Offset:X}";
        }

        public void Resize(Padding padding)
        {
            // We need to rebuild the byte[]
            var newWidth = Width + padding.Horizontal;
            var oldHeight = BlockIndices.Length / Width;
            var newHeight = oldHeight + padding.Vertical;
            var newData = new byte[newWidth * newHeight];
            // We copy the old data in... leaving any new blocks set to 0
            for (var y = 0; y < oldHeight; ++y)
            {
                var newY = y + padding.Top;
                if (newY < 0 || newY >= newHeight)
                {
                    // No longer present
                    continue;
                }

                for (var x = 0; x < Width; ++x)
                {
                    var newX = x + padding.Left;
                    if (newX < 0 || newX >= newWidth)
                    {
                        // No longer present
                        continue;
                    }

                    newData[newX + newWidth * newY] = BlockIndices[x + y * Width];
                }
            }

            // Apply it
            BlockIndices = newData;
            Width = newWidth;
        }

        public Floor Clone()
        {
            return new Floor(this);
        }
    }
}