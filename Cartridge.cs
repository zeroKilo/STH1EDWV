using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using sth1edwv.Properties;

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
            public int RelativeTo { get; set; } // for RelativeOffset
            public int Bank { get; set; } // for OffsetInBank
        }

        private class MemoryItem
        {
            public int Offset { get; set; }
            public int Length { get; set; }

            public List<Reference> References { get; } = new();

            public enum PlacementOptions
            {
                Fixed,
                SameBank,
                Free
            }
            public PlacementOptions Placement { get; set; }
        }

        private class Game
        {
            public class Level : MemoryItem
            {
                public string Name { get; set; }
            }

            public List<Level> Levels;

            public class Palette: MemoryItem
            {
                public string Name { get; set; }
            }

            public List<Palette> Palettes;

            public class TileSet : MemoryItem
            {}

            public List<TileSet> TileSets;

            public class Floor : MemoryItem
            {}

            public List<Floor> Floors;
        }

        private static readonly Game Sonic1MasterSystem = new()
        {
            Levels = new List<Game.Level>
            {
                new() {
                    Name = "Green Hill Act 1", Offset = 0x15580 + 0x4a, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15580 } }
                }, new() {
                    Name = "Green Hill Act 2", Offset = 0x15580 + 0x6f, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15582 } }
                }, new() {
                    Name = "Green Hill Act 3", Offset = 0x15580 + 0x94, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15584 } }
                }, new() {
                    Name = "Bridge Act 1", Offset = 0x15580 + 0xde, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15586 } }
                }, new() {
                    Name = "Bridge Act 2", Offset = 0x15580 + 0x103, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15588 } }
                }, new() {
                    Name = "Bridge Act 3", Offset = 0x15580 + 0x128, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x1558a } }
                }, new() {
                    Name = "Jungle Act 1", Offset = 0x15580 + 0x14d, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x1558c } }
                }, new() {
                    Name = "Jungle Act 2", Offset = 0x15580 + 0x172, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x1558e } }
                }, new() {
                    Name = "Jungle Act 3", Offset = 0x15580 + 0x197, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15590 } }
                }, new() {
                    Name = "Labyrinth Act 1", Offset = 0x15580 + 0x1bc, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15592 } }
                }, new() {
                    Name = "Labyrinth Act 2", Offset = 0x15580 + 0x1e1, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15594 } }
                }, new() {
                    Name = "Labyrinth Act 3", Offset = 0x15580 + 0x206, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15596 } }
                }, new() {
                    Name = "Scrap Brain Act 1", Offset = 0x15580 + 0x22b, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x15598 } }
                }, new() {
                    Name = "Scrap Brain Act 2", Offset = 0x15580 + 0x250, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x1559a } }
                }, new() {
                    Name = "Scrap Brain Act 3", Offset = 0x15580 + 0x2bf, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x1559c } }
                }, new() {
                    Name = "Sky Base Act 1", Offset = 0x15580 + 0x378, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x1559e } }
                }, new() {
                    Name = "Sky Base Act 2", Offset = 0x15580 + 0x39d, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155a0 } }
                }, new() {
                    Name = "Sky Base Act 3", Offset = 0x15580 + 0x3c2, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155a2 } }
                }, new() {
                    Name = "Ending Sequence", Offset = 0x15580 + 0xb9, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155a4 } }
                }, new() {
                    Name = "Scrap Brain Act 2 (Emerald Maze), from corridor", Offset = 0x15580 + 0x275, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155a8 } }
                }, new() {
                    Name = "Scrap Brain Act 2 (Ballhog Area)", Offset = 0x15580 + 0x29a, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155aa } }
                }, new() {
                    Name = "Scrap Brain Act 2, from transporter", Offset = 0x15580 + 0x32e, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155ac } }
                }, new() {
                    Name = "Scrap Brain Act 2, from Emerald Maze", Offset = 0x15580 + 0x2e4, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155b0 } }
                }, new() {
                    Name = "Scrap Brain Act 2, from Ballhog Area", Offset = 0x15580 + 0x309, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155b2 } }
                }, new() {
                    Name = "Sky Base Act 2 (Interior)", Offset = 0x15580 + 0x3e7, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155b4 } }
                }, new() {
                    Name = "Special Stage 1", Offset = 0x15580 + 0x40c, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155b8 } }
                }, new() {
                    Name = "Special Stage 2", Offset = 0x15580 + 0x431, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155ba } }
                }, new() {
                    Name = "Special Stage 3", Offset = 0x15580 + 0x456, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155bc } }
                }, new() {
                    Name = "Special Stage 4", Offset = 0x15580 + 0x47b, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155be } }
                }, new() {
                    Name = "Special Stage 5", Offset = 0x15580 + 0x4a0, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155c0 } }
                }, new() {
                    Name = "Special Stage 6", Offset = 0x15580 + 0x4c5, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155c2 } }
                }, new() {
                    Name = "Special Stage 7", Offset = 0x15580 + 0x4ea, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155c4 } }
                }, new() {
                    Name = "Special Stage 8", Offset = 0x15580 + 0x50f, Length = 37, Placement = MemoryItem.PlacementOptions.Fixed,
                    References = { new Reference { Type = Reference.Types.RelativeOffset, RelativeTo = 0x15580, Location = 0x155c6 } }
                }
            },
            Palettes = new List<Game.Palette>
            {
                new() {
                    Name = "Green Hill", Length = 16*5, Offset = 0x629e, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x627c } }
                }, new() {
                    Name = "Bridge", Length = 16*5, Offset = 0x62ee, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x627e } }
                }, new() {
                    Name = "Jungle", Length = 16*5, Offset = 0x633e, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x6280 } }
                }, new() {
                    Name = "Labyrinth", Length = 16*5, Offset = 0x638e, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References =
                    {
                        new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x6282 }, // Table
                        new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x01EA }, // Raster split
                    }
                }, new() {
                    Name = "Scrap Brain", Length = 16*6, Offset = 0x63de, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x6284 } }
                }, new() {
                    Name = "Sky Base 1/2", Length = 16*6, Offset = 0x643e, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x6286 } }
                }, new() {
                    Name = "Sky Base 3/2 interior", Length = 16*2, Offset = 0x658e, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x6288 } }
                }, new() {
                    Name = "Special stage", Length = 16*2, Offset = 0x655e, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x628a } }
                }, new() {
                    Name = "Green Hill cycle", Length = 16*3, Offset = 0x62be, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x628c } }
                }, new() {
                    Name = "Bridge cycle", Length = 16*3, Offset = 0x630e, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x628e } }
                }, new() {
                    Name = "Jungle cycle", Length = 16*3, Offset = 0x635e, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x6290 } }
                }, new() {
                    Name = "Labyrinth cycle", Length = 16*3, Offset = 0x63ae, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x6292 } }
                }, new() {
                    Name = "Scrap Brain cycle", Length = 16*4, Offset = 0x63fe, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x6294 } }
                }, new() {
                    Name = "Sky Base 1 cycle", Length = 16*4, Offset = 0x645e, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References =
                    {
                        new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x6296 },
                        new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x1f9f }
                    }
                }, new() {
                    Name = "Sky Base lightning 1", Length = 16*4, Offset = 0x649e, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x1fa3 } }
                }, new() {
                    Name = "Sky Base lightning 2", Length = 16*4, Offset = 0x64de, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x1fa7 } }
                }, new() {
                    Name = "Sky Base interior cycle", Length = 16*4, Offset = 0x65ae, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x6298 } }
                }, new() {
                    Name = "Special stage cycle", Length = 16*1, Offset = 0x657e, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x629a } }
                },  new() {
                    Name = "Sky Base 2 cycle", Length = 16*4, Offset = 0x651e, Placement = MemoryItem.PlacementOptions.SameBank, 
                    References = { new Reference { Type = Reference.Types.OffsetInBank, Bank = 1, Location = 0x629c } }
                },
                // TODO: there is a "regular palette" table and a "palette cycle" table implied by the above, it'd be better to make those tables flexible?
                // TODO: palette editing at all :)
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

        private readonly int _artBanksTableOffset;

        private readonly Dictionary<int, TileSet> _tileSets = new();
        private readonly Dictionary<int, Floor> _floors = new();
        private readonly Dictionary<int, BlockMapping> _blockMappings = new();

        public Cartridge(string path)
        {
            Memory = File.ReadAllBytes(path);

            _originalMemory = (byte[])Memory.Clone();

            Labels = ReadList(Resources.map);

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

            Palettes = Palette.ReadPalettes(Memory, 0x627C, 8+9).ToList();
            ReadLevels();
            ReadGameText();
        }

        private void ReadLevels()
        {
            DisposeAll(_blockMappings);
            DisposeAll(_tileSets);
            Levels.Clear();
            foreach (var level in Sonic1MasterSystem.Levels)
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
            var space = Sonic1MasterSystem.Levels
                .Cast<MemoryItem>()
                .Concat(Sonic1MasterSystem.Palettes)
                .Concat(Sonic1MasterSystem.Floors)
                .Concat(Sonic1MasterSystem.TileSets)
                .Select(x => new FreeSpace{Start = x.Offset, End = x.Offset + x.Length})
                .OrderBy(x => x.Start)
                .ToList();

            // We then merge consecutive ones together
            for (int i = 0; i < space.Count - 1; /* increment in loop */)
            {
                if (space[i].End == space[i + 1].Start)
                {
                    space[i].End = space[i + 1].End;
                    space.RemoveAt(i + 1);
                }
                else
                {
                    ++i;
                }
            }

            // TODO more here...
            // - Get data from each item
            // - Allocate space (with constraints?)
            // - Rewrite pointers and bank numbers
            // - Iteratively solve outstanding calculations
            // - Emit ROM!
        }
    }
}
