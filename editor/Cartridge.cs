using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace sth1edwv
{
    public interface IDataItem
    {
        /// <summary>
        /// Offset data was read from
        /// </summary>
        int Offset { get; }

        /// <summary>
        /// Get raw data from the item
        /// </summary>
        IList<byte> GetData();
    }

    public class Cartridge: IDisposable
    {
        private class MemoryItem
        {
            public int Offset { get; set; }
            public int Length { get; set; }
        }

        private class Game
        {
            public class Level : MemoryItem
            {
                public string Name { get; set; }
            }

            public List<Level> Levels { get; set; }

            public class Palette: MemoryItem
            {
                public string Name { get; set; }
            }

            public List<Palette> Palettes { get; set; }

            public class TileSet : MemoryItem
            {}

            public List<TileSet> TileSets { get; set; }

            public class Floor : MemoryItem
            {}

            public List<Floor> Floors { get; set; }
        }

        private static readonly Game Sonic1MasterSystem = new()
        {
            Levels = new List<Game.Level>
            {
                new() {
                    Name = "Green Hill Act 1", Offset = 0x15580 + 0x4a, Length = 37,
                }, new() {
                    Name = "Green Hill Act 2", Offset = 0x15580 + 0x6f, Length = 37,
                }, new() {
                    Name = "Green Hill Act 3", Offset = 0x15580 + 0x94, Length = 37,
                }, new() {
                    Name = "Bridge Act 1", Offset = 0x15580 + 0xde, Length = 37,
                }, new() {
                    Name = "Bridge Act 2", Offset = 0x15580 + 0x103, Length = 37,
                }, new() {
                    Name = "Bridge Act 3", Offset = 0x15580 + 0x128, Length = 37,
                }, new() {
                    Name = "Jungle Act 1", Offset = 0x15580 + 0x14d, Length = 37,
                }, new() {
                    Name = "Jungle Act 2", Offset = 0x15580 + 0x172, Length = 37,
                }, new() {
                    Name = "Jungle Act 3", Offset = 0x15580 + 0x197, Length = 37,
                }, new() {
                    Name = "Labyrinth Act 1", Offset = 0x15580 + 0x1bc, Length = 37,
                }, new() {
                    Name = "Labyrinth Act 2", Offset = 0x15580 + 0x1e1, Length = 37,
                }, new() {
                    Name = "Labyrinth Act 3", Offset = 0x15580 + 0x206, Length = 37,
                }, new() {
                    Name = "Scrap Brain Act 1", Offset = 0x15580 + 0x22b, Length = 37,
                }, new() {
                    Name = "Scrap Brain Act 2", Offset = 0x15580 + 0x250, Length = 37,
                }, new() {
                    Name = "Scrap Brain Act 3", Offset = 0x15580 + 0x2bf, Length = 37,
                }, new() {
                    Name = "Sky Base Act 1", Offset = 0x15580 + 0x378, Length = 37,
                }, new() {
                    Name = "Sky Base Act 2", Offset = 0x15580 + 0x39d, Length = 37,
                }, new() {
                    Name = "Sky Base Act 3", Offset = 0x15580 + 0x3c2, Length = 37,
                }, new() {
                    Name = "Ending Sequence", Offset = 0x15580 + 0xb9, Length = 37,
                }, new() {
                    Name = "Scrap Brain Act 2 (Emerald Maze), from corridor", Offset = 0x15580 + 0x275, Length = 37,
                }, new() {
                    Name = "Scrap Brain Act 2 (Ballhog Area)", Offset = 0x15580 + 0x29a, Length = 37,
                }, new() {
                    Name = "Scrap Brain Act 2, from transporter", Offset = 0x15580 + 0x32e, Length = 37,
                }, new() {
                    Name = "Scrap Brain Act 2, from Emerald Maze", Offset = 0x15580 + 0x2e4, Length = 37,
                }, new() {
                    Name = "Scrap Brain Act 2, from Ballhog Area", Offset = 0x15580 + 0x309, Length = 37,
                }, new() {
                    Name = "Sky Base Act 2 (Interior)", Offset = 0x15580 + 0x3e7, Length = 37,
                }, new() {
                    Name = "Special Stage 1", Offset = 0x15580 + 0x40c, Length = 37,
                }, new() {
                    Name = "Special Stage 2", Offset = 0x15580 + 0x431, Length = 37,
                }, new() {
                    Name = "Special Stage 3", Offset = 0x15580 + 0x456, Length = 37,
                }, new() {
                    Name = "Special Stage 4", Offset = 0x15580 + 0x47b, Length = 37,
                }, new() {
                    Name = "Special Stage 5", Offset = 0x15580 + 0x4a0, Length = 37,
                }, new() {
                    Name = "Special Stage 6", Offset = 0x15580 + 0x4c5, Length = 37,
                }, new() {
                    Name = "Special Stage 7", Offset = 0x15580 + 0x4ea, Length = 37,
                }, new() {
                    Name = "Special Stage 8", Offset = 0x15580 + 0x50f, Length = 37,
                }
            },
            Palettes = new List<Game.Palette>
            {
                new() {
                    Name = "Green Hill", Length = 16*5, Offset = 0x629e, 
                }, new() {
                    Name = "Bridge", Length = 16*5, Offset = 0x62ee, 
                }, new() {
                    Name = "Jungle", Length = 16*5, Offset = 0x633e, 
                }, new() {
                    Name = "Labyrinth", Length = 16*5, Offset = 0x638e, 
                }, new() {
                    Name = "Scrap Brain", Length = 16*6, Offset = 0x63de, 
                }, new() {
                    Name = "Sky Base 1/2", Length = 16*6, Offset = 0x643e, 
                }, new() {
                    Name = "Sky Base 3/2 interior", Length = 16*2, Offset = 0x658e, 
                }, new() {
                    Name = "Special stage", Length = 16*2, Offset = 0x655e, 
                }, new() {
                    Name = "Green Hill cycle", Length = 16*3, Offset = 0x62be, 
                }, new() {
                    Name = "Bridge cycle", Length = 16*3, Offset = 0x630e, 
                }, new() {
                    Name = "Jungle cycle", Length = 16*3, Offset = 0x635e, 
                }, new() {
                    Name = "Labyrinth cycle", Length = 16*3, Offset = 0x63ae, 
                }, new() {
                    Name = "Scrap Brain cycle", Length = 16*4, Offset = 0x63fe, 
                }, new() {
                    Name = "Sky Base 1 cycle", Length = 16*4, Offset = 0x645e, 
                }, new() {
                    Name = "Sky Base lightning 1", Length = 16*4, Offset = 0x649e, 
                }, new() {
                    Name = "Sky Base lightning 2", Length = 16*4, Offset = 0x64de, 
                }, new() {
                    Name = "Sky Base interior cycle", Length = 16*4, Offset = 0x65ae, 
                }, new() {
                    Name = "Special stage cycle", Length = 16*1, Offset = 0x657e, 
                },  new() {
                    Name = "Sky Base 2 cycle", Length = 16*4, Offset = 0x651e, 
                },  new() {
                    Name = "Underwater", Length = 16*2, Offset = 0x024b, 
                },  new() {
                    Name = "Underwater (boss)", Length = 16*2, Offset = 0x024b, 
                },  new() {
                    Name = "Map screen 1", Length = 16*2, Offset = 0x0f0e, 
                },  new() {
                    Name = "Map screen 2", Length = 16*2, Offset = 0x0f2e, 
                },  new() {
                    Name = "Title screen", Length = 16*2, Offset = 0x13e1, 
                },  new() {
                    Name = "Act complete screen", Length = 16*2, Offset = 0x1b8d, 
                },  new() {
                    Name = "Credits screen", Length = 16*2, Offset = 0x2ad6, 
                },  new() {
                    Name = "End sign sprites", Length = 16, Offset = 0x626c, 
                },  new() {
                    Name = "Boss sprites #1", Length = 16, Offset = 0x731c, 
                },  new() {
                    Name = "Act complete", Length = 16*2, Offset = 0x14fc, 
                }
                // TODO: there is a "regular palette" table and a "palette cycle" table implied by the above, it'd be better to make those tables flexible?
                // TODO: palette editing at all :)
            }
        };

        public Memory Memory { get; }
        public IList<Level> Levels { get; } = new List<Level>();
        public IList<GameText> GameText { get; } = new List<GameText>();
        public IList<Palette> Palettes { get; }

        private readonly Dictionary<int, TileSet> _tileSets = new();
        private readonly Dictionary<int, Floor> _floors = new();
        private readonly Dictionary<int, BlockMapping> _blockMappings = new();

        public Cartridge(string path)
        {
            Memory = new Memory(File.ReadAllBytes(path));
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
                Levels.Add(new Level(this, level.Offset, Palettes, level.Name));
            }
        }

        public TileSet GetTileSet(int offset, Palette palette, bool addRings)
        {
            return GetItem(_tileSets, offset, () => new TileSet(this, offset, palette, addRings));
        }

        public Floor GetFloor(int offset, int compressedSize, int maximumCompressedSize, int width)
        {
            return GetItem(_floors, offset, () => new Floor(this, offset, compressedSize, maximumCompressedSize, width));
        }

        public BlockMapping GetBlockMapping(int offset, int solidityIndex, TileSet tileSet)
        {
            return GetItem(_blockMappings, offset, () => new BlockMapping(this, offset, solidityIndex, tileSet));
        }

        private static T GetItem<T>(IDictionary<int, T> dictionary, int offset, Func<T> generatorFunc) 
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

        private static string RomSizes(int size)
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

        private static string Regions(int region)
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
            sb.AppendLine("Cartridge Header");
            sb.AppendLine($"Magic        : \"{Memory.String(0x7ff0, 8)}\"");
            sb.AppendLine($"Reserved     : 0x{Memory[0x7ff8]:X2}{Memory[0x7ff8]:X2}");
            sb.AppendLine($"Checksum     : 0x{Memory.Word(0x7ffa)}");
            sb.AppendLine($"Product code : {Memory[0x7ffe] >> 4:X1}{Memory[0x7ffd]:X2}{Memory[0x7ffc]:X2}");
            sb.AppendLine($"Version      : 0x{Memory[0x7ffe] & 0xf:X1}");
            sb.AppendLine($"Region       : 0x{Memory[0x7fff] >> 4:X1}{Regions(Memory[0x7fff] >> 4)}");
            sb.AppendLine($"ROM Size     : 0x{Memory[0x7fff] & 0xf:X1}{RomSizes(Memory[0x7fff] & 0xf)}");
            sb.AppendLine($"ROM message  : \"{Memory.String(0x3B, 0x2A)}\"");
            return sb.ToString();
        }

        public void Dispose()
        {
            DisposeAll(_tileSets);
            DisposeAll(_blockMappings);
        }

        private static void DisposeAll<T>(Dictionary<int, T> collection) where T: IDisposable
        {
            foreach (var item in collection.Values)
            {
                item.Dispose();
            }
            collection.Clear();
        }

        public void SaveTo(string filename)
        {
            // We clone the memory to a memory stream
            var memory = Memory.GetStream(0, Memory.Count).ToArray();

            // We work through the data types...
            // - Game text (at original offsets)
            foreach (var gameText in GameText)
            {
                gameText.GetData().CopyTo(memory, gameText.Offset);
            }
            // - Floors (filling space)
            // TODO: 16000-18de9 is tilemaps, I should repack them and then append (when I add an editor?)
            var offset = 0x16dea;
            foreach (var floor in Levels.Select(l => l.Floor).Distinct())
            {
                var data = floor.GetData();
                data.CopyTo(memory, offset);
                floor.Offset = offset;
                offset += data.Count;
            }

            if (offset >= 0x20000)
            {
                throw new Exception("Floor layouts out of space");
            }

            // - Tile sets (at original offsets)
            // TODO: make them fit, along with everything else
            // - all art from 26000
            // - level art from 32FE6
            // - end at $3da28 where some more assets are placed in the way... repack them too? "Contains sprite art and/or sprite mappings"
            offset = 0x32FE6;
            foreach (var tileSet in Levels.Select(l => l.TileSet).Distinct())
            {
                var data = tileSet.GetData();
                data.CopyTo(memory, offset);
                tileSet.Offset = offset;
                offset += data.Count;
            }

            if (offset >= 0x3da28)
            {
                throw new Exception("Tilesets out of space");
            }

            // - Block mappings (at original offsets)
            // TODO make these flexible if I make it possible to change sizes
            foreach (var blockMapping in Levels.Select(l => l.BlockMapping).Distinct())
            {
                // We need to place both the block data and solidity data
                foreach (var block in blockMapping.Blocks)
                {
                    block.GetData().CopyTo(memory, block.Offset);

                    memory[block.SolidityOffset] = block.Data;
                }
            }
            // - Level objects (at original offsets)
            foreach (var obj in Levels.Select(l => l.Objects).Distinct().SelectMany(x => x))
            {
                obj.GetData().CopyTo(memory, obj.Offset);
            }
            // - Level headers (at original offsets). We do these last so they pick up info from the contained objects.
            foreach (var level in Levels)
            {
                level.GetData().CopyTo(memory, level.Offset);
            }
            File.WriteAllBytes(filename, memory);
        }

        public class Space
        {
            public int Total { get; set; }
            public int Used { get; set; }
        }

        public Space GetFloorSpace() =>
            new()
            {
                Total = 0x20000 - 0x16dea,
                Used = _floors.Values.Sum(x => x.GetData().Count)
            };
        public Space GetTileSetSpace() =>
            new()
            {
                Total = 0x3da28 - 0x32FE6,
                Used = _tileSets.Values.Sum(x => x.GetData().Count)
            };
    }
}
