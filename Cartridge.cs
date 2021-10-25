using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace sth1edwv
{
    public class Cartridge
    {
        public class MemMapEntry
        {
            public uint Offset { get; }
            public string Label { get; }

            public MemMapEntry(uint offset, string label)
            {
                Offset = offset;
                Label = label;
            }
        }

        public byte[] Memory { get; }
        public IList<MemMapEntry> Labels { get; }
        public IList<Level> LevelList { get; } = new List<Level>();
        public IList<GameText> GameText { get; } = new List<GameText>();
        public IList<Palette> Palettes { get; }

        private readonly IList<MemMapEntry> _levelOffsets;
        private readonly uint _artBanksTableOffset;

        public Cartridge(string path)
        {
            Memory = File.ReadAllBytes(path);
            Labels = ReadList(Properties.Resources.map);
            _levelOffsets = ReadList(Properties.Resources.levels);

            _artBanksTableOffset = 0;
            var symbolsFilePath = Path.ChangeExtension(path, "sym");
            if (File.Exists(symbolsFilePath))
            {
                // As a hack, let'label read it in and find the ArtTilesTable label
                var regex = new Regex("(?<bank>[0-9]{2}):(?<offset>[0-9]{4}) ArtTilesTable");
                var line = File.ReadAllLines(symbolsFilePath)
                    .Select(x => regex.Match(x))
                    .FirstOrDefault(x => x.Success);
                if (line != null)
                {
                    // Compute the art banks table offset
                    _artBanksTableOffset = Convert.ToUInt32(line.Groups["bank"].Value, 16) * 0x4000 + Convert.ToUInt32(line.Groups["offset"].Value, 16) % 0x4000;
                }
            }

            Palettes = Palette.ReadPalettes(Memory, 0x627C, 8).ToList();
            ReadLevels();
            ReadGameText();
        }

        public void ReadLevels()
        {
            foreach (MemMapEntry e in _levelOffsets)
            {
                LevelList.Add(new Level(this, e.Offset, _artBanksTableOffset, Palettes, e.Label));
            }
        }

        public void ReadGameText()
        {
            for (int i = 0; i < 6; i++)
            {
                MemoryStream m = new MemoryStream();
                m.Write(Memory, 0x122D + i * 0xF, 0xF);
                GameText.Add(new GameText(m.ToArray(), i < 3));
            }
            for (int i = 0; i < 3; i++)
            {
                MemoryStream m = new MemoryStream();
                m.Write(Memory, 0x197E + i * 0x10, 0x10);
                GameText.Add(new GameText(m.ToArray(), true));
            }
        }

        public List<MemMapEntry> ReadList(string s)
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

        public string RomSizes(int size)
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

        public string Regions(int region)
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

        public string MakeSummary()
        {
            StringBuilder sb = new StringBuilder();
            MemoryStream m = new MemoryStream(Memory);
            m.Seek(0x7FF0, 0);
            byte[] header = new byte[16];
            m.Read(header, 0, 16);
            sb.AppendLine("Cartridge Header");
            sb.Append("Magic        : \"");
            for (int i = 0; i < 8; i++)
                sb.Append((char)header[i]);
            sb.AppendLine("\"");
            sb.AppendLine($"Reserved     : 0x{header[8]:X2}{header[9]:X2}");
            sb.AppendLine($"Checksum     : 0x{header[10]:X2}{header[11]:X2}");
            sb.AppendLine($"Product code : {(header[14] >> 4):X1}{header[13]:X2}{header[12]:X2}");
            sb.AppendLine($"Version      : 0x{(header[14] & 7):X1}");
            sb.AppendLine($"Region       : 0x{(header[15] >> 4):X1}{Regions(header[15] >> 4)}");
            sb.AppendLine($"ROM Size     : 0x{(header[15] & 7):X1}{RomSizes(header[15] & 7)}");
            sb.AppendLine($"ROM Header   : \"{Encoding.UTF7.GetString(Memory, 0x3B, 0x2A)}\"");
            return sb.ToString();
        }
    }
}
