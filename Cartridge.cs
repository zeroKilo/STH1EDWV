using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sth1edwv
{
    public static class Cartridge
    {
        public class MemMapEntry
        {
            public uint offset;
            public string label;
            public MemMapEntry(uint off, string s)
            {
                offset = off;
                label = s;
            }
        }
        public static byte[] memory;
        public static List<MemMapEntry> labels;
        public static List<MemMapEntry> levels;
        public static List<Level> level_list;

        public static void Load(string path)
        {
            memory = File.ReadAllBytes(path);
            labels = ReadList(Properties.Resources.map);
            levels = ReadList(Properties.Resources.levels);
            ReadLevels();
        }

        public static void ReadLevels()
        {
            level_list = new List<Level>();
            foreach (MemMapEntry e in levels)
                level_list.Add(new Level(e.offset));
        }

        public static List<MemMapEntry> ReadList(string s)
        {
            List<MemMapEntry> result  = new List<MemMapEntry>();
            using (StringReader sr = new StringReader(s))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] parts = line.Split(' ');
                    int len = parts[0].Length;
                    string offset = parts[0].Substring(1, len - 1);
                    string label = line.Substring(len + 1, line.Length - len - 1);
                    result.Add(new MemMapEntry(Convert.ToUInt32(offset, 16), label));
                }
            }
            return result;
        }

        public static string ROMSizes(int size)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" (");
            switch (size)
            {
                case 0xA:
                    sb.Append("8KB");
                    break;
                case 0xB:
                    sb.Append("16KB");
                    break;
                case 0xC:
                    sb.Append("32KB");
                    break;
                case 0xD:
                    sb.Append("48KB");
                    break;
                case 0xE:
                    sb.Append("64KB");
                    break;
                case 0xF:
                    sb.Append("128KB");
                    break;
                case 0x0:
                    sb.Append("256KB");
                    break;
                case 0x1:
                    sb.Append("512KB");
                    break;
                case 0x2:
                    sb.Append("1MB");
                    break;
            }
            sb.Append(")");
            return sb.ToString();
        }

        public static string Regions(int region)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" (");
            switch (region)
            {
                case 0x3:
                    sb.Append("SMS Japan");
                    break;
                case 0x4:
                    sb.Append("SMS Export");
                    break;
                case 0x5:
                    sb.Append("GG Japan");
                    break;
                case 0x6:
                    sb.Append("GG Export");
                    break;
                case 0x7:
                    sb.Append("GG International");
                    break;
            }
            sb.Append(")");
            return sb.ToString();
        }

        public static string MakeSummary()
        {
            StringBuilder sb = new StringBuilder();
            MemoryStream m = new MemoryStream(memory);
            m.Seek(0x7FF0, 0);
            byte[] header = new byte[16];
            m.Read(header, 0, 16);
            sb.AppendLine("Cartridge Header");
            sb.Append("Magic        : \"");
            for (int i = 0; i < 8; i++)
                sb.Append((char)header[i]);
            sb.AppendLine("\"");
            sb.AppendLine("Reserved     : 0x" + header[8].ToString("X2") + header[9].ToString("X2"));
            sb.AppendLine("Checksum     : 0x" + header[10].ToString("X2") + header[11].ToString("X2"));
            sb.AppendLine("Product code : " + (header[14] >> 4).ToString("X1") + header[13].ToString("X2") + header[12].ToString("X2"));
            sb.AppendLine("Version      : 0x" + (header[14] & 7).ToString("X1"));
            sb.AppendLine("Region       : 0x" + (header[15] >> 4).ToString("X1") + Regions(header[15] >> 4));
            sb.AppendLine("ROM Size     : 0x" + (header[15] & 7).ToString("X1") + ROMSizes(header[15] & 7));
            sb.AppendLine("ROM Header   : \"" + Encoding.UTF7.GetString(memory, 0x3B, 0x2A) + "\"");
            return sb.ToString();
        }
    }
}
