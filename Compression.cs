using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace sth1edwv
{
    public static class Compression
    {
        public static byte[] DecompressRle(Cartridge cartridge, int address, int size)
        {
            // Read into a memory stream
            // Decompress into another one
            using var m = new MemoryStream(cartridge.Memory, address, size);
            using var result = new MemoryStream();
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

        public static byte[] CompressRle(byte[] data)
        {
            using var m = new MemoryStream(data);
            using var result = new MemoryStream();
            while (m.Position < m.Length)            
            {
                // Look for a run of bytes
                var b = m.ReadByte();
                var counter = 1;
                for (; counter < 257; ++counter)
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
                    --counter; // stored as n-1
                    result.WriteByte((byte)counter);
                }
            }
            return result.ToArray();
        }

        public static byte[] DecompressArt(byte[] data, int offset, out int lengthConsumed)
        {
            var magic = Encoding.ASCII.GetString(data, offset, 2);
            if (magic != "HY")
            {
                throw new Exception("Invalid magic number");
            }
            var duplicateRowsOffset = offset + BitConverter.ToUInt16(data, offset + 2);
            var artDataOffset = offset + BitConverter.ToUInt16(data, offset + 4);
            var rowCount = BitConverter.ToUInt16(data, offset + 6);
            if (rowCount % 8 != 0)
            {
                throw new Exception($"Row count {rowCount} is not a multiple of 8");
            }

            var nextArtDataOffset = artDataOffset;
            // We work through the data based on these...
            using var bitMasks = new MemoryStream(data, offset+8, rowCount / 8);
            using var duplicatesData = new MemoryStream(data, duplicateRowsOffset, rowCount);
            using var result = new MemoryStream();
            while (bitMasks.Position != bitMasks.Length)
            {
                var bitmask = bitMasks.ReadByte();
                for (var i = 0; i < 8; ++i)
                {
                    var bit = bitmask & 1;
                    bitmask >>= 1;
                    byte[] row;
                    if (bit == 1)
                    {
                        // Duplicate
                        var duplicateIndex = duplicatesData.ReadByte();
                        if ((duplicateIndex & 0xf0) == 0xf0)
                        {
                            // Higher indices are two bytes
                            duplicateIndex &= 0xf;
                            duplicateIndex <<= 8;
                            duplicateIndex |= duplicatesData.ReadByte();
                        }

                        row = ReadArt(data, artDataOffset + duplicateIndex * 4);
                    }
                    else
                    {
                        row = ReadArt(data, nextArtDataOffset);
                        nextArtDataOffset += 4;
                    }
                    result.Write(row, 0, row.Length);
                }
            }

            // We assume that the data is contiguous from the header to the end of whichever of the art data and duplicates data comes last.
            lengthConsumed = Math.Max(duplicateRowsOffset + (int)duplicatesData.Position, nextArtDataOffset) - offset;

            return result.ToArray();
        }

        private static byte[] ReadArt(IReadOnlyList<byte> data, int offset)
        {
            var result = new byte[8];
            // We convert to chunky
            for (var i = 0; i < 8; ++i)
            {
                var value = 0;
                for (var b = 0; b < 4; ++b)
                {
                    var bit = (data[offset + b] >> (7-i)) & 1;
                    value |= bit << b;
                }

                result[i] = (byte)value;
            }

            return result;
        }

        public static byte[] CompressArt(MemoryStream source)
        {
            var artData = new List<byte[]>();
            var duplicates = new List<int>();
            var bitMasks = new List<byte>();
            
            // We work through it one row at a time...
            source.Position = 0;
            byte bitmask = 0;
            int linesConsumed = 0;
            while (source.Position < source.Length)
            {
                // Read a line
                var line = new byte[8];
                source.Read(line, 0, 8);
                ++linesConsumed;
                bitmask <<= 1;
                // See if we already have it
                var index = artData.IndexOf(line);

                if (index == -1)
                {
                    // If not found, add to art data
                    artData.Add(line);
                }
                else
                {
                    // If found, add the reference
                    duplicates.Add(index);
                    // And set the bit
                    bitmask |= 1;
                }

                // If we have finished a tile, emit the bitmask
                if (linesConsumed == 8)
                {
                    bitMasks.Add(bitmask);
                    linesConsumed = 0;
                }
            }

            // Now we emit it back...
            using var result = new MemoryStream();
            using var resultWriter = new BinaryWriter(result, Encoding.ASCII);
            // Header - magic chars
            resultWriter.Write("HY");
            // Art offset = 8 + bitmask count
            // Duplicates offset = 8 + bitmask count + art size
            var artOffset = 8 + bitMasks.Count;
            var duplicatesOffset = artOffset + artData.Count * 4;
            var rowCount = bitMasks.Count / 8;

            resultWriter.Write((ushort)duplicatesOffset);
            resultWriter.Write((ushort)artOffset);
            resultWriter.Write((ushort)rowCount);

            // Next the bitmasks
            resultWriter.Write(bitMasks.ToArray());
            // And the art data, converting to chunky
            resultWriter.Write(artData.SelectMany(ChunkyToPlanar).ToArray());
            // Finally the duplicates data
            foreach (var index in duplicates)
            {
                // 1-2 bytes encoding
                if (index < 0xf0)
                {
                    resultWriter.Write((byte)index);
                }
                else
                {
                    resultWriter.Write((byte)(0xf0 | (index >> 8)));
                    resultWriter.Write((byte)(index & 0xff));
                }
            }
            return result.ToArray();
        }

        private static IEnumerable<byte> ChunkyToPlanar(byte[] data)
        {
            for (int plane = 0; plane < 4; ++plane)
            {
                var b = 0;
                for (int i = 0; i < 8; ++i)
                {
                    var bit = (data[i] >> plane) & 1;
                    b |= bit << (7 - i);
                }

                yield return (byte)b;
            }
        }


    }
}