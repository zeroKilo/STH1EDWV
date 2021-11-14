using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using sth1edwv.GameObjects;

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
        private class Game
        {
            public class LevelInfo
            {
                public string Name { get; set; }
                public int Offset { get; set; }
            }
            public List<LevelInfo> Levels { get; set; }

            public class ScreenInfo
            {
                public string Name { get; set; }
                public int TileSetBankOffset { get; set; }
                public int TileSetReferenceOffset { get; set; }
                public int TileMapBankOffset { get; set; }
                public int TileMapReferenceOffset { get; set; }
                public int SecondaryTileMapReferenceOffset { get; set; }
                public int TileMapSizeOffset { get; set; }
                public int SecondaryTileMapSizeOffset { get; set; }
                public int PaletteReferenceOffset { get; set; }
            }
            public List<ScreenInfo> Screens { get; set; }
        }

        private static readonly Game Sonic1MasterSystem = new()
        {
            Screens = new List<Game.ScreenInfo>
            {
                new()
                {
                    Name = "Map screen 1", 
                    TileSetReferenceOffset = 0x0c8a, 
                    TileSetBankOffset = 0x0c90, 
                    TileMapBankOffset = 0x0cab,
                    TileMapReferenceOffset = 0x0cb3, 
                    TileMapSizeOffset = 0x0cb6,
                    SecondaryTileMapReferenceOffset = 0x0cc4, 
                    SecondaryTileMapSizeOffset = 0x0cc7,
                    PaletteReferenceOffset = 0x0cd5
                },
                new()
                {
                    Name = "Map screen 2", 
                    TileSetReferenceOffset = 0x0cec, 
                    TileSetBankOffset = 0x0cf2, 
                    PaletteReferenceOffset = 0x0d37, 
                    TileMapReferenceOffset = 0x0d15, 
                    TileMapSizeOffset = 0x0d18,
                    TileMapBankOffset = 0x0d0d,
                    SecondaryTileMapReferenceOffset = 0x0d26, 
                    SecondaryTileMapSizeOffset = 0x0d29
                },
                new()
                {
                    Name = "Title screen", 
                    TileSetReferenceOffset = 0x1297, 
                    TileSetBankOffset = 0x129d, 
                    TileMapBankOffset = 0x12ad,
                    TileMapReferenceOffset = 0x12b5, 
                    TileMapSizeOffset = 0x12bb,
                    PaletteReferenceOffset = 0x12cd
                },
                new()
                {
                    Name = "Game Over",
                    TileSetReferenceOffset = 0x1412,
                    TileSetBankOffset = 0x1418,
                    TileMapBankOffset = 0x141d,
                    TileMapReferenceOffset = 0x1425,
                    TileMapSizeOffset = 0x1428,
                    PaletteReferenceOffset = 0x143d
                },
                new()
                {
                    Name = "Act Complete",
                    TileSetReferenceOffset = 0x1581,
                    TileSetBankOffset = 0x1587,
                    TileMapBankOffset = 0x158c,
                    TileMapReferenceOffset = 0x1594,
                    TileMapSizeOffset = 0x1597,
                    PaletteReferenceOffset = 0x1605
                },
                new()
                {
                    Name = "Special Stage Complete",
                    TileSetReferenceOffset = 0x1581,
                    TileSetBankOffset = 0x1587,
                    TileMapBankOffset = 0x158c,
                    TileMapReferenceOffset = 0x15a4,
                    TileMapSizeOffset = 0x15a7,
                    PaletteReferenceOffset = 0x1605
                },
                new()
                {
                    Name = "Ending Map",
                    PaletteReferenceOffset = 0x25a2,
                    TileSetReferenceOffset = 0x25aa,
                    TileSetBankOffset = 0x25b0,
                    TileMapBankOffset = 0x25b5,
                    TileMapReferenceOffset = 0x25bd,
                    TileMapSizeOffset = 0x25c0
                },
                new()
                {
                    Name = "Ending Map 2",
                    // Uses the tileset from Ending Map...
                    PaletteReferenceOffset = 0x25a2,
                    TileSetReferenceOffset = 0x25aa,
                    TileSetBankOffset = 0x25b0,
                    TileMapBankOffset = 0x26c2,
                    TileMapReferenceOffset = 0x267e,
                    TileMapSizeOffset = 0x2681
                },
                new()
                {
                    Name = "Credits",
                    TileSetReferenceOffset = 0x26ac,
                    TileSetBankOffset = 0x26b2,
                    TileMapBankOffset = 0x26c2,
                    TileMapReferenceOffset = 0x26ca,
                    TileMapSizeOffset = 0x26cd,
                    PaletteReferenceOffset = 0x2703 // TODO: same tileset, different palette... how to resolve this?
                }
            },
            Levels = new List<Game.LevelInfo>
            {
                new() { Name = "Green Hill Act 1", Offset = 0x15580 + 0x4a },
                new() { Name = "Green Hill Act 2", Offset = 0x15580 + 0x6f },
                new() { Name = "Green Hill Act 3", Offset = 0x15580 + 0x94 },
                new() { Name = "Bridge Act 1", Offset = 0x15580 + 0xde },
                new() { Name = "Bridge Act 2", Offset = 0x15580 + 0x103 },
                new() { Name = "Bridge Act 3", Offset = 0x15580 + 0x128 },
                new() { Name = "Jungle Act 1", Offset = 0x15580 + 0x14d },
                new() { Name = "Jungle Act 2", Offset = 0x15580 + 0x172 },
                new() { Name = "Jungle Act 3", Offset = 0x15580 + 0x197 },
                new() { Name = "Labyrinth Act 1", Offset = 0x15580 + 0x1bc },
                new() { Name = "Labyrinth Act 2", Offset = 0x15580 + 0x1e1 },
                new() { Name = "Labyrinth Act 3", Offset = 0x15580 + 0x206 },
                new() { Name = "Scrap Brain Act 1", Offset = 0x15580 + 0x22b },
                new() { Name = "Scrap Brain Act 2", Offset = 0x15580 + 0x250 },
                new() { Name = "Scrap Brain Act 3", Offset = 0x15580 + 0x2bf },
                new() { Name = "Sky Base Act 1", Offset = 0x15580 + 0x378 },
                new() { Name = "Sky Base Act 2", Offset = 0x15580 + 0x39d },
                new() { Name = "Sky Base Act 3", Offset = 0x15580 + 0x3c2 },
                new() { Name = "Ending Sequence", Offset = 0x15580 + 0xb9 },
                new() { Name = "Scrap Brain Act 2 (Emerald Maze), from corridor", Offset = 0x15580 + 0x275 },
                new() { Name = "Scrap Brain Act 2 (Ballhog Area)", Offset = 0x15580 + 0x29a },
                new() { Name = "Scrap Brain Act 2, from transporter", Offset = 0x15580 + 0x32e },
                new() { Name = "Scrap Brain Act 2, from Emerald Maze", Offset = 0x15580 + 0x2e4 },
                new() { Name = "Scrap Brain Act 2, from Ballhog Area", Offset = 0x15580 + 0x309 },
                new() { Name = "Sky Base Act 2 (Interior)", Offset = 0x15580 + 0x3e7 },
                new() { Name = "Special Stage 1", Offset = 0x15580 + 0x40c },
                new() { Name = "Special Stage 2", Offset = 0x15580 + 0x431 },
                new() { Name = "Special Stage 3", Offset = 0x15580 + 0x456 },
                new() { Name = "Special Stage 4", Offset = 0x15580 + 0x47b },
                new() { Name = "Special Stage 5", Offset = 0x15580 + 0x4a0 },
                new() { Name = "Special Stage 6", Offset = 0x15580 + 0x4c5 },
                new() { Name = "Special Stage 7", Offset = 0x15580 + 0x4ea },
                new() { Name = "Special Stage 8", Offset = 0x15580 + 0x50f }
            }
        };

        public Memory Memory { get; }
        public IList<Level> Levels { get; } = new List<Level>();
        public IList<GameText> GameText { get; } = new List<GameText>();
        public IList<Palette> Palettes { get; }
        public IList<Screen> Screens { get; } = new List<Screen>();

        private readonly Dictionary<int, TileSet> _tileSets = new();
        private readonly Dictionary<int, Floor> _floors = new();
        private readonly Dictionary<int, BlockMapping> _blockMappings = new();
        private readonly Dictionary<int, Palette> _palettes = new();

        public Cartridge(string path)
        {
            Memory = new Memory(File.ReadAllBytes(path));
            Palettes = Palette.ReadPalettes(Memory, 0x627C, 8+9).ToList();
            ReadLevels();
            ReadGameText();
            ReadScreens();
        }

        private void ReadScreens()
        {
            foreach (var screenInfo in Sonic1MasterSystem.Screens)
            {
                Screens.Add(new Screen(
                    this, 
                    screenInfo.Name, 
                    screenInfo.TileSetReferenceOffset, 
                    screenInfo.TileSetBankOffset, 
                    screenInfo.PaletteReferenceOffset, 
                    screenInfo.TileMapReferenceOffset, 
                    screenInfo.TileMapSizeOffset,
                    screenInfo.TileMapBankOffset,
                    screenInfo.SecondaryTileMapReferenceOffset,
                    screenInfo.SecondaryTileMapSizeOffset));
            }
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

        public TileSet GetTileSet(int offset, bool addRings, bool isSprites)
        {
            return GetItem(_tileSets, offset, () => new TileSet(this, offset, addRings, isSprites));
        }

        public Floor GetFloor(int offset, int compressedSize, int width)
        {
            return GetItem(_floors, offset, () => new Floor(this, offset, compressedSize, width));
        }

        public BlockMapping GetBlockMapping(int offset, int solidityIndex, TileSet tileSet)
        {
            return GetItem(_blockMappings, offset, () => new BlockMapping(this, offset, solidityIndex, tileSet));
        }

        public Palette GetPalette(int offset, int count)
        {
            return GetItem(_palettes, offset, () => new Palette(Memory, offset, count));
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

            if (offset > 0x20000)
            {
                throw new Exception("Floor layouts out of space");
            }

            // - Tile sets (filling space)
            // TODO: various art from 26000
            // TODO: map art from 30000 - can be more flexible for the bank, levels can't
            // Level sprite art is from 0x2a12a to 0x2f92e (ending sequence includes boss sprites)
            offset = 0x2a12a;
            foreach (var tileSet in Levels.Select(l => l.SpriteTileSet).Distinct())
            {
                var data = tileSet.GetData();
                data.CopyTo(memory, offset);
                tileSet.Offset = offset;
                offset += data.Count;
            }

            if (offset > 0x2f92e)
            {
                throw new Exception("Sprite tile sets out of space");
            }

            // Level background art is from 0x32fe6 to $3da28, where some more assets are placed in the way... repack them too?
            // "Contains sprite art and/or sprite mappings"
            offset = 0x32fe6;
            foreach (var tileSet in Levels.Select(l => l.TileSet).Distinct())
            {
                var data = tileSet.GetData();
                data.CopyTo(memory, offset);
                tileSet.Offset = offset;
                offset += data.Count;
            }

            if (offset > 0x3da28)
            {
                throw new Exception("Level tile sets out of space");
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

        // All the numbers in these need to match what's in SaveTo
        public Space GetFloorSpace() =>
            new()
            {
                Total = 0x20000 - 0x16dea,
                Used = Levels.Select(x => x.Floor).Distinct().Sum(x => x.GetData().Count)
            };
        public Space GetFloorTileSetSpace() =>
            new()
            {
                Total = 0x3da28 - 0x32FE6,
                Used = Levels.Select(x => x.TileSet).Distinct().Sum(x => x.GetData().Count)
            };
        public Space GetSpriteTileSetSpace() =>
            new()
            {
                Total = 0x2f92e - 0x2a12a,
                Used = Levels.Select(x => x.SpriteTileSet).Distinct().Sum(x => x.GetData().Count)
            };
    }
}
