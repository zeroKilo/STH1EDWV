using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace sth1edwv
{
    public interface IDataItem
    {
        /// <summary>
        /// Offset data was read from
        /// </summary>
        int Offset { get; }
        /// <summary>
        /// Length of data read from there
        /// </summary>
        int LengthConsumed { get; }

        /// <summary>
        /// Get raw data from the item
        /// </summary>
        IList<byte> GetData();
        /*
        /// <summary>
        /// Addresses of pointers to this data, along with the slot used for the pointer
        /// </summary>
        IEnumerable<Tuple<int, int>> Pointers { get; }
        /// <summary>
        /// Addresses of bank numbers used to access this data
        /// </summary>
        IEnumerable<int> BankReferences { get; }
        /// <summary>
        /// Addresses of relative references to this data, along with the reference point they are relative to
        /// </summary>
        IEnumerable<Tuple<int, int>> RelativeReferences { get; }
        */
    }

    public class Cartridge: IDisposable
    {
        private class Reference
        {
            public int Location { get; set; }

            public enum Types
            {
                Bank,
                OffsetInBank,
                RelativeOffset
            }

            public Types Type { get; set; }
            public int RelativeTo { get; set; }
        }

        private class MemoryItem
        {
            public int Offset { get; set; }
            public int Length { get; set; }

            public List<Reference> References { get; } = new();
        }

        private class Game
        {
            public class Level : MemoryItem
            {
                public string Name { get; set; }
            }

            public List<Level> Levels;

            public class Palette: MemoryItem
            {}

            public List<Palette> Palettes;

            public class TileSet : MemoryItem
            {}

            public List<TileSet> TileSets;

            public class Floor : MemoryItem
            {}

            public List<Floor> Floors;
        }

        private static Game _sonic1MasterSystem = new()
        {
            Levels = new List<Game.Level>
            {
                new() {
                    Name = "Green Hill Act 1", Offset = 0x15580 + 0x4a, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15580 } }
                }, new() {
                    Name = "Green Hill Act 2", Offset = 0x15580 + 0x6f, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15582 } }
                }, new() {
                    Name = "Green Hill Act 3", Offset = 0x15580 + 0x94, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15584 } }
                }, new() {
                    Name = "Bridge Act 1",
                    Offset = 0x15580 + 0xde,
                    Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15586 } }
                }, new() {
                    Name = "Bridge Act 2",
                    Offset = 0x15580 + 0x103,
                    Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15588 } }
                }, new() {
                    Name = "Bridge Act 3", Offset = 0x15580 + 0x128, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x1558a } }
                }, new() {
                    Name = "Jungle Act 1", Offset = 0x15580 + 0x14d, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x1558c } }
                }, new() {
                    Name = "Jungle Act 2", Offset = 0x15580 + 0x172, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x1558e } }
                }, new() {
                    Name = "Jungle Act 3", Offset = 0x15580 + 0x197, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15590 } }
                }, new() {
                    Name = "Labyrinth Act 1", Offset = 0x15580 + 0x1bc, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15592 } }
                }, new() {
                    Name = "Labyrinth Act 2", Offset = 0x15580 + 0x1e1, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15594 } }
                }, new() {
                    Name = "Labyrinth Act 3", Offset = 0x15580 + 0x206, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15596 } }
                }, new() {
                    Name = "Scrap Brain Act 1", Offset = 0x15580 + 0x22b, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15598 } }
                }, new() {
                    Name = "Scrap Brain Act 2", Offset = 0x15580 + 0x250, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x1559a } }
                }, new() {
                    Name = "Scrap Brain Act 3", Offset = 0x15580 + 0x2bf, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x1559c } }
                }, new() {
                    Name = "Sky Base Act 1", Offset = 0x15580 + 0x378, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x1559e } }
                }, new() {
                    Name = "Sky Base Act 2", Offset = 0x15580 + 0x39d, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155a0 } }
                }, new() {
                    Name = "Sky Base Act 3", Offset = 0x15580 + 0x3c2, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155a2 } }
                }, new() {
                    Name = "Ending Sequence", Offset = 0x15580 + 0xb9, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155a4 } }
                }, new() {
                    Name = "Scrap Brain Act 2 (Emerald Maze), from corridor", Offset = 0x15580 + 0x275, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155a8 } }
                }, new() {
                    Name = "Scrap Brain Act 2 (Ballhog Area)", Offset = 0x15580 + 0x29a, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155aa } }
                }, new() {
                    Name = "Scrap Brain Act 2, from transporter", Offset = 0x15580 + 0x32e, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155ac } }
                }, new() {
                    Name = "Scrap Brain Act 2, from Emerald Maze", Offset = 0x15580 + 0x2e4, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155b0 } }
                }, new() {
                    Name = "Scrap Brain Act 2, from Ballhog Area", Offset = 0x15580 + 0x309, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155b2 } }
                }, new() {
                    Name = "Sky Base Act 2 (Interior)", Offset = 0x15580 + 0x3e7, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155b4 } }
                }, new() {
                    Name = "Special Stage 1", Offset = 0x15580 + 0x40c, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155b8 } }
                }, new() {
                    Name = "Special Stage 2", Offset = 0x15580 + 0x431, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155ba } }
                }, new() {
                    Name = "Special Stage 3", Offset = 0x15580 + 0x456, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155bc } }
                }, new() {
                    Name = "Special Stage 4", Offset = 0x15580 + 0x47b, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155be } }
                }, new() {
                    Name = "Special Stage 5", Offset = 0x15580 + 0x4a0, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155c0 } }
                }, new() {
                    Name = "Special Stage 6", Offset = 0x15580 + 0x4c5, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155c2 } }
                }, new() {
                    Name = "Special Stage 7", Offset = 0x15580 + 0x4ea, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155c4 } }
                }, new() {
                    Name = "Special Stage 8", Offset = 0x15580 + 0x50f, Length = 37,
                    References = { new Reference() { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155c6 } }
                }
            }
        };

        public class MemMapEntry
        {
            public int Offset { get; set; }
            public string Label { get; set; }
            public override string ToString()
            {
                return $"${Offset:X5} {Label}";
            }
        }

        private readonly byte[] _originalMemory;

        public byte[] Memory { get; }
        public IList<MemMapEntry> Labels { get; }
        public IList<Level> Levels { get; } = new List<Level>();
        public IList<GameText> GameText { get; } = new List<GameText>();
        public IList<Palette> Palettes { get; }

        private readonly IList<MemMapEntry> _levelOffsets;
        private readonly int _artBanksTableOffset;

        private readonly Dictionary<int, TileSet> _tileSets = new();
        private readonly Dictionary<int, Floor> _floors = new();
        private readonly Dictionary<int, BlockMapping> _blockMappings = new();

        public Cartridge(string path)
        {
            Memory = File.ReadAllBytes(path);

            _originalMemory = (byte[])Memory.Clone();

            Labels = ReadList(Properties.Resources.map);
            _levelOffsets = ReadList(Properties.Resources.levels);

            _artBanksTableOffset = 0;
            var symbolsFilePath = Path.ChangeExtension(path, "sym");
            if (File.Exists(symbolsFilePath))
            {
                // As a hack, let's read it in and find the ArtTilesTable label
                var regex = new Regex("(?<bank>[0-9]{2}):(?<offset>[0-9]{4}) ArtTilesTable");
                var line = File.ReadAllLines(symbolsFilePath)
                    .Select(x => regex.Match(x))
                    .FirstOrDefault(x => x.Success);
                if (line != null)
                {
                    // Compute the art banks table offset
                    _artBanksTableOffset = Convert.ToInt32(line.Groups["bank"].Value, 16) * 0x4000 + Convert.ToInt32(line.Groups["offset"].Value, 16) % 0x4000;
                }
            }

            Palettes = Palette.ReadPalettes(Memory, 0x627C, 8).ToList();
            ReadLevels();
            ReadGameText();
        }

        public void ReadLevels()
        {
            DisposeAll(_blockMappings);
            DisposeAll(_tileSets);
            Levels.Clear();
            foreach (var level in _sonic1MasterSystem.Levels)
            {
                Levels.Add(new Level(this, level.Offset, _artBanksTableOffset, Palettes, level.Name));
            }
        }

        public TileSet GetTileSet(int offset, Palette palette)
        {
            return GetItem(_tileSets, offset, () => new TileSet(this, offset, palette));
        }

        public Floor GetFloor(int offset, int size, int width)
        {
            return GetItem(_floors, offset, () => new Floor(this, offset, size, width));
        }

        public BlockMapping GetBlockMapping(int offset, byte solidityIndex, TileSet tileSet)
        {
            return GetItem(_blockMappings, offset, () => new BlockMapping(this, offset, solidityIndex, tileSet));
        }

        private T GetItem<T>(Dictionary<int, T> dictionary, int offset, Func<T> generatorFunc) 
        {
            if (!dictionary.TryGetValue(offset, out var result))
            {
                result = generatorFunc();
                dictionary.Add(offset, result);
            }

            return result;
        }

        private void ReadGameText()
        {
            GameText.Clear();
            for (var i = 0; i < 6; i++)
            {
                GameText.Add(new GameText(this, 0x122D + i * 0xF, i < 3));
            }
            for (var i = 0; i < 3; i++)
            {
                GameText.Add(new GameText(this, 0x197E + i * 0x10, true));
            }
        }

        private static List<MemMapEntry> ReadList(string s)
        {
            return s.Split(new []{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => Regex.Match(line, "^\\$(?<offset>[0-9A-Fa-f]+) (?<label>.+)$"))
                .Where(m => m.Success)
                .Select(m => new MemMapEntry { Offset = Convert.ToInt32(m.Groups["offset"].Value, 16), Label = m.Groups["label"].Value})
                .ToList();
        }

        private string RomSizes(int size)
        {
            var sb = new StringBuilder();
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

        private string Regions(int region)
        {
            var sb = new StringBuilder();
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
            var sb = new StringBuilder();
            var m = new MemoryStream(Memory);
            m.Seek(0x7FF0, 0);
            var header = new byte[16];
            m.Read(header, 0, 16);
            sb.AppendLine("Cartridge Header");
            sb.Append("Magic        : \"");
            for (var i = 0; i < 8; i++)
                sb.Append((char)header[i]);
            sb.AppendLine("\"");
            sb.AppendLine($"Reserved     : 0x{header[8]:X2}{header[9]:X2}");
            sb.AppendLine($"Checksum     : 0x{header[10]:X2}{header[11]:X2}");
            sb.AppendLine($"Product code : {header[14] >> 4:X1}{header[13]:X2}{header[12]:X2}");
            sb.AppendLine($"Version      : 0x{header[14] & 7:X1}");
            sb.AppendLine($"Region       : 0x{header[15] >> 4:X1}{Regions(header[15] >> 4)}");
            sb.AppendLine($"ROM Size     : 0x{header[15] & 7:X1}{RomSizes(header[15] & 7)}");
            sb.AppendLine($"ROM Header   : \"{Encoding.UTF7.GetString(Memory, 0x3B, 0x2A)}\"");
            return sb.ToString();
        }

        public void Dispose()
        {
            DisposeAll(_tileSets);
            DisposeAll(_blockMappings);
        }

        private void DisposeAll<T>(Dictionary<int, T> collection) where T: IDisposable
        {
            foreach (var item in collection.Values)
            {
                item.Dispose();
            }
            collection.Clear();
        }

        private class FreeSpace
        {
            public int Start { get; set; }
            public int End { get; set; }
        }

        private void Rebuild()
        {
            // We clone the original memory...
            var data = (byte[])_originalMemory.Clone();

            // We walk through the data and "de-allocate" all the parts that we have read into objects
            var space = _tileSets.Values
                .Cast<IDataItem>()
                .Concat(_floors.Values)
                .Concat(_blockMappings.Values)
                .Select(x => new FreeSpace{Start = x.Offset, End = x.Offset + x.LengthConsumed})
                .OrderBy(x => x.Start)
                .ToList();

            // TODO more here...
            // - Get data from each item
            // - Allocate space (with constraints?)
            // - Rewrite pointers and bank numbers
            // - Iteratively solve outstanding calculations
            // - Emit ROM!
        }
    }
}
