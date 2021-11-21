using System;
using System.Collections.Generic;
using System.Drawing;
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
        public class Game
        {
            public class LevelInfo
            {
                public string Name { get; set; }
                public int Offset { get; set; }
            }
            public List<LevelInfo> Levels { get; set; }

            public class ArtInfo
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
            public List<ArtInfo> Screens { get; set; }

            public class Reference
            {
                public int Offset { get; set; }
                public string To { get; set; } // This is a bit nasty... we look things up by name. I guess I need to check for clashes and unreferenced things?
                public enum Types
                {
                    Absolute,
                    Slot1Banked,
                    Slot2Banked,
                    PageNumber,
                    Size
                }
                public Types Type { get; set; }
                public int Delta { get; set; }
            }
            public Dictionary<string, List<Reference>> References { get; set; }

            // The game has various "groups" of assets, with one or more of:
            // - Tileset
            // - Palette
            // - Tilemap
            // - Sprite tileset
            // - Sprite palette (maybe linked to the other palette)
            // - Secondary tilemap for tile priority
            // Each of these items will then have one or more references in the ROM.
            public class Asset
            {
                public int Offset { get; set; }
                public enum Types { TileSet, Palette, TileMap,
                    SpriteTileSet,
                    ForegroundTileMap
                }
                public Types Type { get; set; }
                public List<Reference> References { get; set; }
            }

            public class AssetGroup
            {
                public string Name { get; set; }
                public List<Asset> Assets { get; set; }
            }
            public List<AssetGroup> Assets { get; set; }
        }

        private static readonly Game Sonic1MasterSystem = new()
        {
            Assets = new List<Game.AssetGroup>
            {
                new()
                {
                    Name = "Map screen 1",
                    Assets = new List<Game.Asset>
                    {
                        new() // Tileset
                        {
                            Offset = 0x30000,
                            Type = Game.Asset.Types.TileSet,
                            References = new List<Game.Reference>
                            {
                                new() {Offset = 0x0c89+1, Type = Game.Reference.Types.Slot1Banked, Delta = -0x4000}, // ld     hl,$0000        ; 000C89 21 00 00 
                                new() {Offset = 0x0c8f+1, Type = Game.Reference.Types.PageNumber}, // ld     a,$0c           ; 000C8F 3E 0C 
                            }
                        },
                        new() // Sprite tileset
                        {
                            Offset = 0x2926b,
                            Type = Game.Asset.Types.SpriteTileSet,
                            References = new List<Game.Reference>
                            {
                                new() {Offset = 0x0c94+1, Type = Game.Reference.Types.Slot1Banked, Delta = -0x4000}, // ld     hl,$526b        ; 000C94 21 6B 52 
                                new() {Offset = 0x0c9a+1, Type = Game.Reference.Types.PageNumber}, // ld     a,$09           ; 000C9A 3E 09 
                            }
                        },
                        new() // HUD tileset
                        {
                            Offset = 0x2f92e,
                            Type = Game.Asset.Types.SpriteTileSet,
                            References = new List<Game.Reference>
                            {
                                new() {Offset = 0x0c9f+1, Type = Game.Reference.Types.Slot1Banked, Delta = -0x4000}, // ld     hl,$b92e        ; 000C9F 21 2E B9 
                                new() {Offset = 0x0ca5+1, Type = Game.Reference.Types.PageNumber}, // ld     a,$09           ; 000CA5 3E 09 
                            }
                        },
                        new() // High priority tilemap, loaded first
                        {
                            Offset = 0x1627e,
                            Type = Game.Asset.Types.ForegroundTileMap,
                            References = new List<Game.Reference>
                            {
                                new() {Offset = 0x0caa+1, Type = Game.Reference.Types.PageNumber}, // ld     a,$05           ; 000CAA 3E 05 
                                new() {Offset = 0x0cb2+1, Type = Game.Reference.Types.Slot1Banked}, // ld     hl,$627e        ; 000CB2 21 7E 62 
                                new() {Offset = 0x0cb5+1, Type = Game.Reference.Types.Size}, // ld     bc,$0178        ; 000CB5 01 78 01 
                            }
                        },
                        new() // Low priority tilemap, loaded second with "transparency"
                        {
                            Offset = 0x1627e,
                            Type = Game.Asset.Types.TileMap,
                            References = new List<Game.Reference>
                            {
                                new() {Offset = 0x0caa+1, Type = Game.Reference.Types.PageNumber}, // TODO shared reference with the above means they need to be linked somehow
                                new() {Offset = 0x0cc3+1, Type = Game.Reference.Types.Slot1Banked}, // ld     hl,$63f6        ; 000CC3 21 F6 63 
                                new() {Offset = 0x0cc6+1, Type = Game.Reference.Types.Size}, // ld     bc,$0145        ; 000CC6 01 45 01 
                            }
                        },
                        new() // Palette
                        {
                            Offset = 0x0f0e,
                            Type = Game.Asset.Types.Palette,
                            References = new List<Game.Reference>
                            {
                                new() {Offset = 0x0cd4+1, Type = Game.Reference.Types.Absolute}, // ld     hl,$0f0e        ; 000CD4 21 0E 0F 
                            }
                        }
                    }
                }
            },

            Screens = new List<Game.ArtInfo>
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
                    // Same tileset and palette as above
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
                    PaletteReferenceOffset = 0x2703
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
            },
            References = new Dictionary<string, List<Game.Reference>>
            {
                { 
                    "Monitor Art", // 0x15180
                    new() {
                        new Game.Reference { Offset = 0x5b32, Type = Game.Reference.Types.Slot1Banked }, // 005B31 21 80 51 
                        new Game.Reference { Offset = 0x350a, Type = Game.Reference.Types.Slot1Banked, Delta = 0x5400 - 0x5180 }, // 005F09 21 00 54 
                        new Game.Reference { Offset = 0xbf51, Type = Game.Reference.Types.Slot1Banked, Delta = 0x5400 - 0x5180 }, // 00BF50 21 00 54 
                        new Game.Reference { Offset = 0x5c00, Type = Game.Reference.Types.Slot1Banked, Delta = 0x5200 - 0x5200 }, // 005BFF 21 00 52
                        new Game.Reference { Offset = 0x5cde, Type = Game.Reference.Types.Slot1Banked, Delta = 0x5280 - 0x5200 }, // 005C6D 21 80 52
                        new Game.Reference { Offset = 0x5ca8, Type = Game.Reference.Types.Slot1Banked, Delta = 0x5180 - 0x5200 }, // 005CA7 21 80 51 
                        new Game.Reference { Offset = 0x5cb3, Type = Game.Reference.Types.Slot1Banked, Delta = 0x5280 - 0x5200 }, // 005CB2 21 80 52
                        new Game.Reference { Offset = 0x5cfa, Type = Game.Reference.Types.Slot1Banked, Delta = 0x5300 - 0x5200 }, // 005CF9 21 00 53
                        new Game.Reference { Offset = 0x5d2a, Type = Game.Reference.Types.Slot1Banked, Delta = 0x5380 - 0x5200 }, // 005D29 21 80 53
                        new Game.Reference { Offset = 0x5d7b, Type = Game.Reference.Types.Slot1Banked, Delta = 0x5480 - 0x5200 }, // 005D7A 21 80 54
                        new Game.Reference { Offset = 0x5da3, Type = Game.Reference.Types.Slot1Banked, Delta = 0x5500 - 0x5200 }, // 005DA2 21 00 55
                        new Game.Reference { Offset = 0x0c1f, Type = Game.Reference.Types.PageNumber } // 000C1E 3E 05 
                    }
                }, {
                    "Sonic (right)", // 0x20000 
                    new() {
                        new Game.Reference { Offset = 0x4c85, Type = Game.Reference.Types.Slot1Banked }, // 004C84 01 00 40 
                        new Game.Reference { Offset = 0x012e, Type = Game.Reference.Types.PageNumber }, // 00012D 3E 08 
                        new Game.Reference { Offset = 0x0156, Type = Game.Reference.Types.PageNumber, Delta = 1 }, // 000135 3E 09
                    }
                }, {
                    "Sonic (left)", // 0x23000 TODO this has to be in the same 32KB window as the above, how to express this?
                    new() {
                        new Game.Reference { Offset = 0x4c8e, Type = Game.Reference.Types.Slot1Banked }, // 004C8D 01 00 70
                        new Game.Reference { Offset = 0x012e, Type = Game.Reference.Types.PageNumber }, // 00012D 3E 08 
                        new Game.Reference { Offset = 0x0156, Type = Game.Reference.Types.PageNumber, Delta = 1 }, // 000135 3E 09
                    }
                }
            }
        };

        public Memory Memory { get; }
        public List<Level> Levels { get; } = new();
        public List<GameText> GameText { get; } = new();
        public List<Screen> Screens { get; } = new();
        public List<ArtItem> Art { get; } = new();

        private readonly Dictionary<int, TileSet> _tileSets = new();
        private readonly Dictionary<int, Floor> _floors = new();
        private readonly Dictionary<int, BlockMapping> _blockMappings = new();
        private readonly Dictionary<int, Palette> _palettes = new();
        private TileSet _rings;

        public Cartridge(string path)
        {
            Memory = new Memory(File.ReadAllBytes(path));
            ReadLevels();
            ReadGameText();
            ReadScreens();
            ReadExtraArt();

            // Apply rings to level tilesets
            foreach (var tileSet in Levels.Select(x => x.TileSet).Distinct())
            {
                tileSet.SetRings(_rings.Tiles[0]);
            }
        }

        private void ReadExtraArt()
        {
            TileSet titleAndCreditTileSet;
            Art.AddRange(new[]
            {
                new ArtItem
                {
                    TileSet = new TileSet(Memory, 0x15180, 0x400, 4, TileSet.Groupings.Monitor),
                    Name = "Monitor Art",
                    Palette = Levels[0].SpritePalette,
                    Width = 8,
                    IsSprites = true
                },
                new ArtItem
                {
                    TileSet = new TileSet(Memory, 0x20000, 12 * 24 * 42, 3, TileSet.Groupings.Sonic),
                    Name = "Sonic (right)",
                    Palette = Levels[0].SpritePalette,
                    Width = 8,
                    IsSprites = true
                },
                new ArtItem
                {
                    TileSet = new TileSet(Memory, 0x23000, 12 * 24 * 42, 3, TileSet.Groupings.Sonic),
                    Name = "Sonic (left)",
                    Palette = Levels[0].SpritePalette,
                    Width = 8,
                    IsSprites = true
                },
                new ArtItem
                {
                    TileSet = new TileSet(Memory, 0x28294, TileSet.Groupings.Sprite),
                    Name = "End sign",
                    Palette = GetPalette(0x626C, 1),
                    PaletteEditable = true,
                    Width = 16,
                    IsSprites = true
                },
                new ArtItem
                {
                    TileSet = titleAndCreditTileSet = new TileSet(Memory, 0x28B0A, TileSet.Groupings.Sprite),
                    Name = "Title Screen Sprites",
                    Palette = GetPalette(0x13f1, 1),
                    PaletteEditable = true,
                    Width = 16,
                    IsSprites = true
                },
                new ArtItem
                {
                    TileSet = titleAndCreditTileSet,
                    Name = "Credits Screen Sprites",
                    Palette = GetPalette(0x2ae6, 1),
                    PaletteEditable = true,
                    Width = 16,
                    IsSprites = true
                },
                new ArtItem
                {
                    TileSet = new TileSet(Memory, 0x2926B, TileSet.Groupings.Sprite),
                    Name = "Map Screen Sprites 1",
                    Palette = GetPalette(0x0f1e, 1),
                    PaletteEditable = true,
                    Width = 16,
                    IsSprites = true
                },
                new ArtItem
                {
                    TileSet = new TileSet(Memory, 0x29942, TileSet.Groupings.Sprite),
                    Name = "Map Screen Sprites 2",
                    Palette = GetPalette(0x0f3e, 1),
                    PaletteEditable = true,
                    Width = 16,
                    IsSprites = true
                },
                // This is no longer at a fixed position as it gets relocated when saving levels
                new ArtItem
                {
                    TileSet = Levels[18].SpriteTileSet,
                    Name = "Boss Sprites",
                    Palette = GetPalette(0x731C, 1),
                    PaletteEditable = true,
                    Width = 16,
                    IsSprites = true
                },
                new ArtItem
                {
                    TileSet = new TileSet(Memory, 0x2F92E, TileSet.Groupings.Sprite),
                    Name = "HUD Sprites",
                    Palette = Levels[0].SpritePalette,
                    Width = 16,
                    IsSprites = true
                },
                new ArtItem
                {
                    TileSet = _rings = new TileSet(Memory, 0x2Fcf0, 24 * 32, 4, TileSet.Groupings.Ring),
                    Name = "Rings",
                    Palette = Levels[0].TilePalette,
                    Width = 6
                },
                new ArtItem
                {
                    TileSet = new TileSet(Memory, 0x3da28, TileSet.Groupings.Sprite),
                    Name = "Capsule and animals",
                    Palette = GetPalette(0x731C, 1),
                    PaletteEditable = true,
                    Width = 16,
                    IsSprites = true
                },
                new ArtItem
                {
                    TileSet = new TileSet(Memory, 0x3e508, TileSet.Groupings.Sprite),
                    Name = "Underwater boss",
                    Palette = GetPalette(0x027b, 1),
                    PaletteEditable = true,
                    Width = 16,
                    IsSprites = true
                },
                new ArtItem
                {
                    TileSet = new TileSet(Memory, 0x3ef3f, TileSet.Groupings.Sprite),
                    Name = "Running Robotnik",
                    Palette = GetPalette(0x731C, 1),
                    PaletteEditable = true,
                    Width = 16,
                    IsSprites = true
                }
            });
        }

        private void ReadScreens()
        {
            foreach (var screenInfo in Sonic1MasterSystem.Screens)
            {
                Screens.Add(new Screen(this, screenInfo));
            }
        }

        private void ReadLevels()
        {
            DisposeAll(_blockMappings);
            DisposeAll(_tileSets);
            Levels.Clear();
            foreach (var level in Sonic1MasterSystem.Levels)
            {
                Levels.Add(new Level(this, level.Offset, level.Name));
            }
        }

        public TileSet GetTileSet(int offset, List<Point> grouping)
        {
            return GetItem(_tileSets, offset, () => new TileSet(Memory, offset, grouping));
        }

        public Floor GetFloor(int offset, int compressedSize, int width)
        {
            return GetItem(_floors, offset, () => new Floor(this, offset, compressedSize, width));
        }

        public BlockMapping GetBlockMapping(int offset, int blockCount, int solidityIndex, TileSet tileSet)
        {
            return GetItem(_blockMappings, offset, () => new BlockMapping(this, offset, blockCount, solidityIndex, tileSet));
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

        public byte[] MakeRom()
        {
            // We clone the memory to a memory stream
            var memory = Memory.GetStream(0, Memory.Count).ToArray();

            // Data we read/write and what we can repack...
            // Start    End     Description                         Repacking?
            // -----------------------------------------------------------------------------
            // 0027b    28a     Underwater boss palette             TODO
            //
            // 00f0e    00f1d   Map screen 1 art palette            TODO
            // 00f1e    00f2d   Map screen 1 sprites palette        TODO
            //
            // 00f3e    00f4d   Map screen 2 sprites palette        TODO
            //
            // 0122d    01286   Level titles text                   Original offset/size
            //
            // 013e1    013f0   Title screen art palette            TODO
            //
            // 014fc    0150b   "Game over" art palette             TODO
            //
            // 0197e    019ad   Ending text                         Original offset/size
            //
            // 01b8d    01b9c   "Sonic has passed" art palette      TODO
            //
            // 02ae6    02af5   Credits sprite palette              TODO
            //
            // 0626c    0627b   Sign palette                        TODO
            // 0629e    065ed   Level palettes                      Original offset/size, Sky Base extra palettes not editable
            //
            // 0731c    0732b   Boss/capsule palette                TODO
            //
            // 15180    1557f   Monitor screens                     Original offset/size (uncompressed) TODO change?
            // 15580    155c9   Level header pointers               Untouched
            // 155ca    15ab3   Level headers                       Yes, original offset/size 
            // 15ab4    15fff   Object layouts (+ unused)           Original offsets/sizes TODO change?
            // 16000    1612D   Title screen tilemap
            // 1612E    161E8   "Sonic Has Passed" tilemap
            // 161E9    1627D   Special Stage complete tilemap
            // 1627E    163F5   Map screen 1 high-priority tilemap
            // 163F6    1653A   Map screen 1 low-priority tilemap
            // 1653B    166AA   Map screen 2 high-priority tilemap
            // 166AB    167FD   Map screen 3 low-priority tilemap
            // 167FE    1682F   Game Over tilemap
            // 16830    169A8   End screen polluted tilemap
            // 169A9    16AED   End screen clean tilemap
            // 16AED    16c60   Unused credits screen tilemap       Can be removed for some free space
            // 16C61    16DE9   Credits screen tilemap
            // 16dea    1ffff   Floors (+unused)                    Yes, level header pointers are rewritten. Can be in range 14000..23fff
            // 20000    22fff   Sonic sprites (left) (+ unused)     Original offset/size (uncompressed) TODO change?
            // 23000    25fff   Sonic sprites (right) (+ unused)    Original offset/size (uncompressed) TODO change?
            // 26000    2751e   Title screen art                    (compressed) TODO
            // 2751f    28293   "Sonic has passed", "game over" art (compressed) TODO
            // 28294    28b09   Sign sprites                        (compressed) TODO
            // 28b0a    2926a   Title screen/credits sprites        (compressed) TODO
            // 2926b    29941   Map screen sprites 1                (compressed) TODO
            // 29942    2a129   Map screen sprites 2                (compressed) TODO
            // 2a12a    2f92d   Level sprites                       Yes, level header pointers are rewritten. Can be in range 24000..33fff TODO: boss sprites are in here but with a bad palette. Could exclude?
            // 2f92e    2fcef   HUD sprites                         (compressed) TODO
            // 2fcf0    2ffff   Rings (+ unused)                    Original offset/size (uncompressed) TODO change?
            // 30000    31800   Map screen 1/ending art             TODO
            // 31801    32fe5   Map screen 2 art                    TODO
            // 32fe6    3da27   Level backgrounds                   Yes, level header pointers are rewritten. Can be in range 30000..3ffff
            // 3da28    3e507   Capsule art                         (compressed) TODO
            // 3e508    3ef3e   Underwater boss art                 (compressed) TODO
            // 3ef3f    3ff21?  Running Robotnik art                (compressed) TODO

            // We work through the data types...
            // - Game text (at original offsets)
            // 122d..1286 inclusive
            // 197e..19ad inclusive
            foreach (var gameText in GameText)
            {
                gameText.GetData().CopyTo(memory, gameText.Offset);
            }
            // - Level palettes (at original offsets)
            // 629e..65ed inclusive, with Sky Base lightning not covered
            foreach (var palette in Levels.SelectMany(x => new[]{x.Palette, x.CyclingPalette}))
            {
                palette.GetData().CopyTo(memory, palette.Offset);
            }

            // Static screen tilemaps
            // 16000..16de9 inclusive, so we combine with Floors below
            // TODO these could be placed anywhere... so we could place them later
            var offset = 0x16000;
            foreach (var tileMap in Screens.Select(x => x.TileMap).Distinct())
            {
                var data = tileMap.GetData();
                data.CopyTo(memory, offset);
                tileMap.Offset = offset;
                offset += data.Count;
            }

            // - Floors (filling space)
            // 16dea..1ffff inclusive
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

            // - Sprite tile sets (filling space)
            // TODO: various art from 26000
            // TODO: map art from 30000 - can be more flexible for the bank, levels can't
            // 2a12a..2EEB0 inclusive
            // Game engine expects data in the range 24000..33fff
            offset = 0x2a12a;
            foreach (var tileSet in Levels.Select(l => l.SpriteTileSet).Distinct())
            {
                if (tileSet.Offset == 0x2EEB1)
                {
                     // Skip boss tiles TODO fix this
                    continue;
                }
                var data = tileSet.GetData();
                data.CopyTo(memory, offset);
                tileSet.Offset = offset;
                offset += data.Count;
            }

            if (offset > 0x2EEB1)
            {
                throw new Exception("Sprite tile sets out of space");
            }

            // - Lavel background art (filling space)
            // 32fe6..3da27
            // Game engine expects data in the range 30000..3ffff
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

            // Uncompressed art at its original offsets
            foreach (var tileSet in Art.Select(x => x.TileSet).Where(x => !x.Compressed))
            {
                tileSet.GetData().CopyTo(memory, tileSet.Offset);
            }
            // TODO other art...

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
            // Fix up screen pointers - this also waits for all the offsets to be sorted
            foreach (var screen in Screens)
            {
                screen.FixPointers(memory);
            }

            return memory;
        }

        public class Space
        {
            public int Total { get; set; }
            public int Used { get; set; }
        }

        // All the numbers in these need to match what's in MakeRom
        public Space GetFloorSpace() =>
            new()
            {
                Total = 0x20000 - 0x16000,
                Used = Levels.Select(x => x.Floor).Distinct().Sum(x => x.GetData().Count) +
                       Screens.Select(x => x.TileMap).Distinct().Sum(x => x.GetData().Count)
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
                Total = 0x2EEB1 - 0x2a12a,
                Used = Levels.Select(x => x.SpriteTileSet).Distinct().Where(x => x.Offset != 0x2EEB1).Sum(x => x.GetData().Count)
            };
    }
}
