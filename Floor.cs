using System.IO;

namespace sth1edwv
{
    public class Floor
    {
        public uint address;
        public uint size;
        public byte[] data;

        public Floor(Cartridge cartridge, uint address, uint size)
        {
            this.address = address;
            this.size = size;
            DecompressData(cartridge);
        }

        private void DecompressData(Cartridge cartridge)
        {
            MemoryStream m = new MemoryStream();
            m.Write(cartridge.Memory, (int)address, (int)size);
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

        public byte[] CompressData(Level l)
        {
            MemoryStream m = new MemoryStream(data);
            int len = (l.floorHeight * l.floorWidth) - 1;
            MemoryStream result = new MemoryStream();
            while (m.Position < len)            
            {
                int b1 = m.ReadByte();
                int b2 = m.ReadByte();
                if (b1 != b2 || b2 == -1 || b1 == -1)
                {
                    if (b1 != -1)
                        result.WriteByte((byte)b1);
                    if (b2 != -1)
                        m.Seek(-1, SeekOrigin.Current);
                }
                else
                {
                    int count = 1;
                    while (true)                    
                    {
                        int read = m.ReadByte();
                        if (read == -1)
                        {
                            count++;
                            break;
                        }
                        if (read != b1)
                        {
                            m.Seek(-1, SeekOrigin.Current);
                            break;
                        }
                        count++;
                        if (count == 256)
                            break;
                    }
                    result.WriteByte((byte)b1);
                    result.WriteByte((byte)b2);
                    result.WriteByte((byte)count);
                }
            }
            return result.ToArray();
        }
    }
}
