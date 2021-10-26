using System.IO;

namespace sth1edwv
{
    public static class RleCompressor
    {
        public static byte[] Decompress(Cartridge cartridge, int address, int size)
        {
            // Read into a memory stream
            // Decompress into another one
            using (var m = new MemoryStream(cartridge.Memory, address, size))
            using (var result = new MemoryStream())
            {
                m.Seek(0, SeekOrigin.Begin);
                while (m.Position < m.Length)
                {
                    var b1 = (byte)m.ReadByte();
                    result.WriteByte(b1);
                    if (m.Position == m.Length)
                    {
                        // Done
                        break;
                    }
                    var b2 = (byte)m.ReadByte();
                    if (b1 != b2)
                    {
                        // Not RLE, rewind and loop
                        m.Seek(-1, SeekOrigin.Current);
                        continue;
                    }

                    // Duplicate indicates RLE
                    // Next byte is a count
                    var count = m.ReadByte();
                    // 0 means 256
                    if (count == 0)
                    {
                        count = 256;
                    }
                    // Emit that many
                    for (var i = 0; i < count; ++i)
                    {
                        result.WriteByte(b1);
                    }
                }
                return result.ToArray();
            }
        }

        public static byte[] Compress(byte[] data)
        {
            using (var m = new MemoryStream(data))
            using (var result = new MemoryStream())
            {
                while (m.Position < m.Length)            
                {
                    // Look for a run of bytes
                    var b = m.ReadByte();
                    var counter = 1;
                    for (; counter < 256; ++counter)
                    {
                        var next = m.ReadByte();
                        if (next == -1)
                        {
                            // End of stream
                            break;
                        }

                        if (next != b)
                        {
                            // Different byte, put it back and stop counting
                            m.Seek(-1, SeekOrigin.Current);
                            break;
                        }
                    }
                    // And emit
                    result.WriteByte((byte)b);
                    if (counter > 1)
                    {
                        result.WriteByte((byte)b);
                        result.WriteByte((byte)counter);
                    }
                }
                return result.ToArray();
            }
        }

    }
}