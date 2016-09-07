using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sth1edwv
{
    public class Floor
    {
        public uint address;
        public uint size;
        public byte[] data;

        public Floor(uint _address, uint _size)
        {
            address = _address;
            size = _size;
            DecompressData();
        }

        private void DecompressData()
        {
            MemoryStream m = new MemoryStream();
            m.Write(Cartridge.memory, (int)address, (int)size);
            int len = (int)m.Length - 1;
            m.Seek(0, 0);
            MemoryStream result = new MemoryStream();
            while (m.Position < len)
            {
                byte b1 = (byte)m.ReadByte();
                byte b2 = (byte)m.ReadByte();
                if (b1 != b2)
                {
                    result.WriteByte(b1);
                    m.Seek(-1, SeekOrigin.Current);
                }
                else
                {
                    byte count = (byte)m.ReadByte();
                    count--;
                    result.WriteByte(b1);
                    result.WriteByte(b2);
                    for (int i = 0; i < count; i++)
                        result.WriteByte(b1);
                }
            }
            data = result.ToArray();
        }
    }
}
