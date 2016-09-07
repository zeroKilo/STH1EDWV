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
    }
}
