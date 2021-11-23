using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly Action<string> _logger;

        public class Game
        {
            public class LevelHeader
            {
                public string Name { get; set; }
                public int Offset { get; set; }
            }
            public List<LevelHeader> Levels { get; set; }

            public class Reference
            {
                public int Offset { get; set; }
                public enum Types
                {
                    Absolute,
                    Slot1,
                    PageNumber,
                    Size
                }
                public Types Type { get; set; }
                public int Delta { get; set; }
                public override string ToString()
                {
                    return $"{Type}@{Offset:X} ({Delta:X})";
                }
            }

            public class Asset
            {
                //public int Offset { get; set; }
                public enum Types { TileSet, Palette, TileMap, SpriteTileSet, ForegroundTileMap }
                public Types Type { get; set; }
                public List<Reference> References { get; set; }
                public int FixedSize { get; set; }
                public int BitPlanes { get; set; }
                public List<Point> TileGrouping { get; set; }
                public int TilesPerRow { get; set; } = 16; // 16 is often the best default
                public bool Hidden { get; set; }

                public int GetOffset(Memory memory)
                {
                    // The offset is implied by the references
                    // First see if there is an absolute one
                    var absoluteReference = References.FirstOrDefault(x => x.Type == Reference.Types.Absolute);
                    if (absoluteReference != null)
                    {
                        return memory.Word(absoluteReference.Offset) - absoluteReference.Delta;
                    }
                    // Next try for a paged one
                    var pageNumberReference = References.FirstOrDefault(x => x.Type == Reference.Types.PageNumber);
                    if (pageNumberReference == null)
                    {
                        throw new Exception("Unable to compute offset");
                    }
                    var pagedReference = References.FirstOrDefault(x => x.Type == Reference.Types.Slot1);
                    if (pagedReference == null)
                    {
                        throw new Exception("Unable to compute offset");
                    }

                    var offset = memory.Word(pagedReference.Offset) - pagedReference.Delta;
                    var page = memory[pageNumberReference.Offset] - pageNumberReference.Delta;
                    switch (pagedReference.Type)
                    {
                        case Reference.Types.Slot1:
                            page -= 1;
                            break;
                    }
                    return page * 0x4000 + offset;
                }

                public int GetLength(Memory memory)
                {
                    if (FixedSize > 0)
                    {
                        return FixedSize;
                    }
                    // We must have a reference to our length
                    var reference = References.FirstOrDefault(x => x.Type == Reference.Types.Size);
                    if (reference == null)
                    {
                        throw new Exception("No length reference");
                    }

                    return memory.Word(reference.Offset) - reference.Delta;
                }
            }

            public Dictionary<string, Asset> Assets { get; set; }

            public Dictionary<string, IEnumerable<string>> AssetGroups { get; set; }
        }

        private static readonly Game Sonic1MasterSystem = new()
        {
            Assets = new Dictionary<string, Game.Asset> {
                {
                    "Monitor Art", new Game.Asset { 
                        // Offset = 0x15180, 
                        Type = Game.Asset.Types.SpriteTileSet, 
                        FixedSize = 0x400,
                        BitPlanes = 4,
                        TileGrouping = TileSet.Groupings.Monitor,
                        TilesPerRow = 8,
                        References = new List<Game.Reference> 
                        {
                            new() { Offset = 0x5B31 + 1, Type = Game.Reference.Types.Slot1 }, // ld hl, $5180 ; 005B31 21 80 51 
                            new() { Offset = 0x5F09 + 1, Type = Game.Reference.Types.Slot1, Delta = 0x5400 - 0x5180 }, // ld hl, $5400 ; 005F09 21 00 54
                            new() { Offset = 0xBF50 + 1, Type = Game.Reference.Types.Slot1, Delta = 0x5400 - 0x5180 }, // ld hl, $5400 ; 00BF50 21 00 54
                            new() { Offset = 0x5BFF + 1, Type = Game.Reference.Types.Slot1, Delta = 0x5200 - 0x5180 }, // ld hl, $5200 ; 005BFF 21 00 52
                            new() { Offset = 0x5C6D + 1, Type = Game.Reference.Types.Slot1, Delta = 0x5280 - 0x5180 }, // ld hl, $5280 ; 005C6D 21 80 52
                            new() { Offset = 0x5CA7 + 1, Type = Game.Reference.Types.Slot1, Delta = 0x5180 - 0x5180 }, // ld hl, $5100 ; 005CA7 21 80 51
                            new() { Offset = 0x5CB2 + 1, Type = Game.Reference.Types.Slot1, Delta = 0x5280 - 0x5180 }, // ld hl, $5200 ; 005CB2 21 80 52
                            new() { Offset = 0x5CF9 + 1, Type = Game.Reference.Types.Slot1, Delta = 0x5300 - 0x5180 }, // ld hl, $5300 ; 005CF9 21 00 53
                            new() { Offset = 0x5D29 + 1, Type = Game.Reference.Types.Slot1, Delta = 0x5380 - 0x5180 }, // ld hl, $5380 ; 005D29 21 80 53
                            new() { Offset = 0x5D7A + 1, Type = Game.Reference.Types.Slot1, Delta = 0x5480 - 0x5180 }, // ld hl, $5480 ; 005D7A 21 80 54
                            new() { Offset = 0x5DA2 + 1, Type = Game.Reference.Types.Slot1, Delta = 0x5500 - 0x5180 }, // ld hl, $5500 ; 005DA2 21 00 55
                            new() { Offset = 0x0c1e + 1, Type = Game.Reference.Types.PageNumber } // ld a,$05 ; 000C1E 3E 05 
                        }
                    }
                }, {
                    "Sonic (right)", new Game.Asset 
                    { 
                        // Offset = 0x20000,
                        Type = Game.Asset.Types.SpriteTileSet, 
                        BitPlanes = 3,
                        TileGrouping = TileSet.Groupings.Sonic,
                        FixedSize = 42 * 24 * 32 * 3/8, // 42 frames, each 24x32, each pixel is 3 bits
                        TilesPerRow = 8,
                        References = new List<Game.Reference> 
                        {
                            new() { Offset = 0x4c84 + 1, Type = Game.Reference.Types.Slot1 }, // ld bc,$4000 ; 004C84 01 00 40 
                            new() { Offset = 0x012d + 1, Type = Game.Reference.Types.PageNumber }, // ld a,$08 ; 00012D 3E 08 
                            new() { Offset = 0x0135 + 1, Type = Game.Reference.Types.PageNumber, Delta = 1 } // ld a,$09 ; 000135 3E 09 
                        }
                    }
                }, {
                    "Sonic (left)", new Game.Asset 
                    { 
                        // Offset = 0x23000,
                        Type = Game.Asset.Types.SpriteTileSet, 
                        BitPlanes = 3,
                        TileGrouping = TileSet.Groupings.Sonic,
                        FixedSize = 42 * 24 * 32 * 3/8, // 42 frames, each 24x32, each pixel is 3 bits
                        TilesPerRow = 8,
                        References = new List<Game.Reference> 
                        {
                            // TODO this has to be in the same 32KB window as the above, how to express this?
                            new() { Offset = 0x4c8e, Type = Game.Reference.Types.Slot1 }, // ld bc,$7000 ; 004C8D 01 00 70
                            new() { Offset = 0x012d + 1, Type = Game.Reference.Types.PageNumber }, // ld a,$08 ; 00012D 3E 08 
                            new() { Offset = 0x0135 + 1, Type = Game.Reference.Types.PageNumber, Delta = 1 } // ld a,$09 ; 000135 3E 09 
                        }
                    }
                }, {
                    "Map screen 1 tileset", new Game.Asset 
                    { 
                        // Offset = 0x30000,
                        Type = Game.Asset.Types.TileSet, 
                        References = new List<Game.Reference> 
                        {
                            // Map screen
                            // ld hl,$0000 ; 000C89 21 00 00
                            // ld a,$0c    ; 000C8F 3E 0C 
                            new() {Offset = 0x0c89+1, Type = Game.Reference.Types.Slot1, Delta = -0x4000}, 
                            new() {Offset = 0x0c8f+1, Type = Game.Reference.Types.PageNumber}, 
                            // Ending screens
                            // ld hl,$0000 ; 0025A9 21 00 00
                            // ld a,$0c    ; 0025AF 3E 0C 
                            new() {Offset = 0x25a9+1, Type = Game.Reference.Types.Slot1, Delta = -0x4000}, 
                            new() {Offset = 0x25af+1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "Map screen 1 tilemap 1", new Game.Asset 
                    { 
                        // Offset = 0x1627E,
                        Type = Game.Asset.Types.ForegroundTileMap, 
                        References = new List<Game.Reference>
                        {
                            // Map screen
                            // ld a,$05    ; 000CAA 3E 05 
                            // ld hl,$627e ; 000CB2 21 7E 62 
                            // ld bc,$0178 ; 000CB5 01 78 01 
                            new() {Offset = 0x0caa + 1, Type = Game.Reference.Types.PageNumber},
                            new() {Offset = 0x0cb2 + 1, Type = Game.Reference.Types.Slot1},
                            new() {Offset = 0x0cb5 + 1, Type = Game.Reference.Types.Size}
                        }
                    }
                }, {
                    "Map screen 1 tilemap 2", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.TileMap, 
                        References = new List<Game.Reference>
                        {
                            // Shares page with the above
                            // ld hl,$63f6 ; 000CC3 21 F6 63 
                            // ld bc,$0145 ; 000CC6 01 45 01 
                            new() {Offset = 0x0caa + 1, Type = Game.Reference.Types.PageNumber}, // TODO can this duplication act to make us know to tie them together?
                            new() {Offset = 0x0cc3 + 1, Type = Game.Reference.Types.Slot1},
                            new() {Offset = 0x0cc6 + 1, Type = Game.Reference.Types.Size}
                        }
                    }
                }, {
                    "Map screen 1 palette", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.Palette,
                        FixedSize = 32,
                        References = new List<Game.Reference>
                        {
                            new() {Offset = 0x0cd4 + 1, Type = Game.Reference.Types.Absolute} // ld hl,$0f0e ; 000CD4 21 0E 0F 
                        }
                    }
                }, {
                    "Map screen 1 sprite tiles", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.SpriteTileSet,
                        TileGrouping = TileSet.Groupings.Sprite,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$526b ; 000C94 21 6B 52 
                            // ld a,$09    ; 000C9A 3E 09 
                            new() {Offset = 0x0c94 + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x0c9a + 1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "Map screen 2 tileset", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.TileSet, 
                        References = new List<Game.Reference> 
                        {
                            // Map screen
                            // ld hl,$1801 ; 000CEB 21 01 18 
                            // ld a,$0c    ; 000CF1 3E 0C 
                            new() {Offset = 0x0ceb+1, Type = Game.Reference.Types.Slot1, Delta = -0x4000}, 
                            new() {Offset = 0x0cf1+1, Type = Game.Reference.Types.PageNumber}, 
                            // Credits
                            // ld hl,$1801 ; 0026AB 21 01 18
                            // ld a,$0c    ; 0026B1 3E 0C 
                            new() {Offset = 0x26ab+1, Type = Game.Reference.Types.Slot1, Delta = -0x4000}, 
                            new() {Offset = 0x26b1+1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "Map screen 2 tilemap 1", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.ForegroundTileMap, 
                        References = new List<Game.Reference>
                        {
                            // Map screen
                            // ld a,$05     ; 000D0C 3E 05 
                            // ld hl,$653b  ; 000D14 21 3B 65 
                            // ld bc,$0170  ; 000D17 01 70 01 
                            new() {Offset = 0x0d0c + 1, Type = Game.Reference.Types.PageNumber},
                            new() {Offset = 0x0d14 + 1, Type = Game.Reference.Types.Slot1},
                            new() {Offset = 0x0d17 + 1, Type = Game.Reference.Types.Size}
                        }
                    }
                }, {
                    "Map screen 2 tilemap 2", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.TileMap, 
                        References = new List<Game.Reference>
                        {
                            // Shares page with the above
                            // ld hl,$66ab ; 000D25 21 AB 66 
                            // ld bc,$0153 ; 000D28 01 53 01 
                            new() {Offset = 0x0d0c + 1, Type = Game.Reference.Types.PageNumber},
                            new() {Offset = 0x0d25 + 1, Type = Game.Reference.Types.Slot1},
                            new() {Offset = 0x0d28 + 1, Type = Game.Reference.Types.Size}
                        }
                    }
                }, {
                    "Map screen 2 palette", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.Palette,
                        FixedSize = 32,
                        References = new List<Game.Reference>
                        {
                            new() {Offset = 0x0d36 + 1, Type = Game.Reference.Types.Absolute} // ld hl,$0f2e ; 000D36 21 2E 0F 
                        }
                    }
                }, {
                    "Map screen 2 sprite tiles", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.SpriteTileSet,
                        TileGrouping = TileSet.Groupings.Sprite,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$5942 ; 000CF6 21 42 59 
                            // ld a,$09    ; 000CFC 3E 09 
                            new() {Offset = 0x0cf6 + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x0cfc + 1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "HUD sprite tiles", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.SpriteTileSet,
                        TileGrouping = TileSet.Groupings.Sprite,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$b92e ; 000C9F 21 2E B9 
                            // ld a,$09    ; 000CA5 3E 09 
                            new() {Offset = 0x0c9f + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x0ca5 + 1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "Title screen tiles", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.TileSet,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$2000 ; 001296 21 00 20 
                            // ld a,$09    ; 00129C 3E 09 
                            new() {Offset = 0x1296 + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x129c + 1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "Title screen sprites", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.SpriteTileSet,
                        TileGrouping = TileSet.Groupings.Sprite,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$4b0a ; 0012A1 21 0A 4B 
                            // ld a,$09    ; 0012A7 3E 09 
                            new() {Offset = 0x12a1 + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x12a7 + 1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "Title screen tilemap", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.TileMap,
                        References = new List<Game.Reference>
                        {
                            // ld a,$05    ; 0012AC 3E 05 
                            // ld hl,$6000 ; 0012B4 21 00 60 
                            // ld bc,$012e ; 0012BA 01 2E 01 
                            new() {Offset = 0x12ac + 1, Type = Game.Reference.Types.PageNumber},
                            new() {Offset = 0x12b4 + 1, Type = Game.Reference.Types.Slot1},
                            new() {Offset = 0x12ba + 1, Type = Game.Reference.Types.Size}
                        }
                    }
                }, {
                    "Title screen palette", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.Palette,
                        FixedSize = 32,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$13e1 ; 0012CC 21 E1 13 
                            new() {Offset = 0x12cc + 1, Type = Game.Reference.Types.Absolute}
                        }
                    }
                }, {
                    "Act Complete tiles", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.TileSet,
                        References = new List<Game.Reference>
                        {
                            // Game Over
                            // ld hl,$351f ; 001411 21 1F 35 
                            // ld a,$09    ; 001417 3E 09 
                            new() {Offset = 0x1411 + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x1417 + 1, Type = Game.Reference.Types.PageNumber},
                            // Act Complete
                            // ld hl,$351f ; 001580 21 1F 35 
                            // ld a,$09    ; 001586 3E 09 
                            new() {Offset = 0x1580 + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x1586 + 1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "Game Over tilemap", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.TileMap,
                        References = new List<Game.Reference>
                        {
                            // ld a,$05    ; 00141C 3E 05 
                            // ld hl,$67fe ; 001424 21 FE 67 
                            // ld bc,$0032 ; 001427 01 32 00 
                            new() {Offset = 0x141c + 1, Type = Game.Reference.Types.PageNumber},
                            new() {Offset = 0x1424 + 1, Type = Game.Reference.Types.Slot1},
                            new() {Offset = 0x1427 + 1, Type = Game.Reference.Types.Size}
                        }
                    }
                }, {
                    "Game Over palette", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.Palette,
                        FixedSize = 32,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$14fc ; 00143C 21 FC 14 
                            new() {Offset = 0x143c + 1, Type = Game.Reference.Types.Absolute}
                        }
                    }
                }, {
                    "Act Complete tilemap", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.TileMap,
                        References = new List<Game.Reference>
                        {
                            // ld a,$05    ; 00158B 3E 05 
                            // ld hl,$612e ; 001593 21 2E 61 
                            // ld bc,$00bb ; 001596 01 BB 00 
                            new() {Offset = 0x1588 + 1, Type = Game.Reference.Types.PageNumber},
                            new() {Offset = 0x1593 + 1, Type = Game.Reference.Types.Slot1},
                            new() {Offset = 0x1596 + 1, Type = Game.Reference.Types.Size}
                        }
                    }
                }, {
                    "Special Stage Complete tilemap", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.TileMap,
                        References = new List<Game.Reference>
                        {
                            // ld a,$05    ; 00158B 3E 05 // Shared with Act Complete tilemap
                            // ld hl,$61e9 ; 0015A3 21 E9 61 
                            // ld bc,$0095 ; 0015A6 01 95 00 
                            new() {Offset = 0x1588 + 1, Type = Game.Reference.Types.PageNumber},
                            new() {Offset = 0x15A3 + 1, Type = Game.Reference.Types.Slot1},
                            new() {Offset = 0x15A6 + 1, Type = Game.Reference.Types.Size}
                        }
                    }
                }, {
                    "Act Complete palette", new Game.Asset 
                    { 
                        Type = Game.Asset.Types.Palette,
                        FixedSize = 32, 
                        References = new List<Game.Reference>
                        {
                            // ld hl,$1b8d ; 001604 21 8D 1B 
                            new() {Offset = 0x1604 + 1, Type = Game.Reference.Types.Absolute}
                        }
                    }
                }, {
                    "Ending palette", new Game.Asset
                    {
                        Type = Game.Asset.Types.Palette,
                        FixedSize = 32,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$2828 ; 0025A1 21 28 28 
                            new() { Offset = 0x25a1 + 1, Type = Game.Reference.Types.Absolute},
                            // ld hl,$2828 ; 00268D 21 28 28 
                            new() { Offset = 0x268d + 1, Type = Game.Reference.Types.Absolute}
                        }
                    }
                }, {
                    "Ending 1 tilemap", new Game.Asset
                    {
                        Type = Game.Asset.Types.TileMap,
                        References = new List<Game.Reference>
                        {
                            // ld a,$05    ; 0025B4 3E 05 
                            // ld hl,$6830 ; 0025BC 21 30 68 
                            // ld bc,$0179 ; 0025BF 01 79 01 
                            new() {Offset = 0x25B4 + 1, Type = Game.Reference.Types.PageNumber},
                            new() {Offset = 0x25BC + 1, Type = Game.Reference.Types.Slot1},
                            new() {Offset = 0x25BF + 1, Type = Game.Reference.Types.Size}
                        }
                    }
                }, {
                    "Ending 2 tilemap", new Game.Asset
                    {
                        Type = Game.Asset.Types.TileMap,
                        References = new List<Game.Reference>
                        {
                            // ld a,$05    ; 002675 3E 05 
                            // ld hl,$69a9 ; 00267D 21 A9 69 
                            // ld bc,$0145 ; 002680 01 45 01 
                            new() {Offset = 0x2675 + 1, Type = Game.Reference.Types.PageNumber},
                            new() {Offset = 0x267D + 1, Type = Game.Reference.Types.Slot1},
                            new() {Offset = 0x2680 + 1, Type = Game.Reference.Types.Size}
                        }
                    }
                }, {
                    "Credits tilemap", new Game.Asset
                    {
                        Type = Game.Asset.Types.TileMap,
                        References = new List<Game.Reference>
                        {
                            // ld a,$05    ; 0026C1 3E 05 
                            // ld hl,$6c61 ; 0026C9 21 61 6C 
                            // ld bc,$0189 ; 0026CC 01 89 01 
                            new() {Offset = 0x26C1 + 1, Type = Game.Reference.Types.PageNumber},
                            new() {Offset = 0x26C9 + 1, Type = Game.Reference.Types.Slot1},
                            new() {Offset = 0x26CC + 1, Type = Game.Reference.Types.Size}
                        }
                    }
                }, {
                    "Credits palette", new Game.Asset
                    {
                        Type = Game.Asset.Types.Palette,
                        FixedSize = 32,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$2ad6 ; 002702 21 D6 2A 
                            new() {Offset = 0x2702 + 1, Type = Game.Reference.Types.Absolute}
                        }
                    }
                }, {
                    "End sign tileset", new Game.Asset
                    {
                        Type = Game.Asset.Types.SpriteTileSet,
                        TileGrouping = TileSet.Groupings.Sprite,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$4294 ; 005F2D 21 94 42 // This is actually into page 10
                            // ld a,$09    ; 005F33 3E 09 
                            new() {Offset = 0x5f2d + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x5f33 + 1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "End sign palette", new Game.Asset
                    {
                        Type = Game.Asset.Types.Palette,
                        FixedSize = 16, // Sprite palette only
                        References = new List<Game.Reference>
                        {
                            // ld hl,$626c ; 005F38 21 6C 62 
                            new() {Offset = 0x5F38 + 1, Type = Game.Reference.Types.Absolute}
                        }
                    }
                }, {
                    "Rings", new Game.Asset
                    {
                        Type = Game.Asset.Types.TileSet,
                        TileGrouping = TileSet.Groupings.Ring,
                        TilesPerRow = 6,
                        BitPlanes = 4,
                        FixedSize = 6 * 16 * 16 * 4/8, // 6 16x16 frames at 4bpp
                        References = new List<Game.Reference>
                        {
                            // ld de,$7cf0 ; 0023AD 11 F0 7C 
                            new() {Offset = 0x23AD + 1, Type = Game.Reference.Types.Slot1},
                            // ld a,$0b ; 001D55 3E 0B 
                            new() {Offset = 0x1D55 + 1, Type = Game.Reference.Types.PageNumber},
                            // ld a,$0b ; 001DB5 3E 0B 
                            new() {Offset = 0x1DB5 + 1, Type = Game.Reference.Types.PageNumber},
                            // ld a,$0b ; 001eb1 3E 0B 
                            new() {Offset = 0x1eb1 + 1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "Boss sprites palette", new Game.Asset
                    {
                        Type = Game.Asset.Types.Palette,
                        FixedSize = 16,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$731c ; 00703C 21 1C 73 
                            // ld hl,$731c ; 00807F 21 1C 73 
                            // ld hl,$731c ; 0084C7 21 1C 73 
                            // ld hl,$731c ; 00929C 21 1C 73 
                            // ld hl,$731c ; 00A821 21 1C 73 
                            // ld hl,$731c ; 00BE07 21 1C 73 
                            new() {Offset = 0x703C + 1, Type = Game.Reference.Types.Absolute},
                            new() {Offset = 0x807F + 1, Type = Game.Reference.Types.Absolute},
                            new() {Offset = 0x84C7 + 1, Type = Game.Reference.Types.Absolute},
                            new() {Offset = 0x929C + 1, Type = Game.Reference.Types.Absolute},
                            new() {Offset = 0xA821 + 1, Type = Game.Reference.Types.Absolute},
                            new() {Offset = 0xBE07 + 1, Type = Game.Reference.Types.Absolute}
                        }
                    }
                }, {
                    "Boss sprites 1", new Game.Asset
                    {
                        Type = Game.Asset.Types.SpriteTileSet,
                        TileGrouping = TileSet.Groupings.Sprite,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$aeb1 ; 007031 21 B1 AE 
                            // ld a,$09    ; 007037 3E 09 
                            new() {Offset = 0x7031 + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x7037 + 1, Type = Game.Reference.Types.PageNumber},
                            // ld hl,$aeb1 ; 008074 21 B1 AE 
                            // ld a,$09    ; 00807A 3E 09 
                            new() {Offset = 0x8074 + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x807A + 1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "Boss sprites 2", new Game.Asset
                    {
                        Type = Game.Asset.Types.SpriteTileSet,
                        TileGrouping = TileSet.Groupings.Sprite,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$e508 ; 0084BC 21 08 E5 
                            // ld a,$0c    ; 0084C2 3E 0C 
                            new() {Offset = 0x84BC + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x84C2 + 1, Type = Game.Reference.Types.PageNumber},
                            // ld hl,$e508 ; 009291 21 08 E5 
                            // ld a,$0c    ; 009297 3E 0C 
                            new() {Offset = 0x9291 + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x9297 + 1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "Boss sprites 3", new Game.Asset
                    {
                        Type = Game.Asset.Types.SpriteTileSet,
                        TileGrouping = TileSet.Groupings.Sprite,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$ef3f ; 00A816 21 3F EF 
                            // ld a,$0c    ; 00A81C 3E 0C 
                            new() {Offset = 0xA816 + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0xA81C + 1, Type = Game.Reference.Types.PageNumber},
                            // ld hl,$ef3f ; 00BB94 21 3F EF 
                            // ld a,$0c    ; 00BB9A 3E 0C 
                            new() {Offset = 0xBB94 + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0xBB9A + 1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                }, {
                    "Capsule sprites", new Game.Asset
                    {
                        Type = Game.Asset.Types.SpriteTileSet,
                        TileGrouping = TileSet.Groupings.Sprite,
                        References = new List<Game.Reference>
                        {
                            // ld hl,$da28 ; 007916 21 28 DA 
                            // ld a,$0c    ; 00791C 3E 0C 
                            new() {Offset = 0x7916 + 1, Type = Game.Reference.Types.Slot1, Delta = -0x4000},
                            new() {Offset = 0x791C + 1, Type = Game.Reference.Types.PageNumber}
                        }
                    }
                },
                {
                    "Green Hill palette", new Game.Asset // We add this just so we can use it below
                    {
                        Type = Game.Asset.Types.Palette,
                        FixedSize = 32,
                        Hidden = true,
                        References = new List<Game.Reference>
                        {
                            new() { Offset = 0x627C, Type = Game.Reference.Types.Absolute}
                        }
                    }
                }
            },
            // These all need to match strings above. This is a bit nasty but I don't see a better way.
            AssetGroups = new Dictionary<string, IEnumerable<string>>
            {
                { "Map screen 1", new [] { "Map screen 1 tileset", "Map screen 1 tilemap 1", "Map screen 1 tilemap 2", "Map screen 1 palette", "Map screen 1 sprite tiles", "HUD sprite tiles" } }, // HUD sprites only used for life counter
                { "Map screen 2", new [] { "Map screen 2 tileset", "Map screen 2 tilemap 1", "Map screen 2 tilemap 2", "Map screen 2 palette", "Map screen 2 sprite tiles", "HUD sprite tiles" } },
                { "Sonic", new[] { "Sonic (right)", "Sonic (left)", "HUD sprite tiles", "Green Hill palette" } }, // HUD sprites contain the spring jump toes, the ones in the art seem unused...
                { "Monitors", new [] { "Monitor Art", "HUD sprite tiles", "Green Hill palette"  } }, // Monitor bases are in the HUD sprites
                { "Title screen", new [] { "Title screen tiles", "Title screen sprites", "Title screen palette", "Title screen tilemap" } },
                { "Game Over", new [] { "Act Complete tiles", "Game Over palette", "Game Over tilemap" } },
                { "Act Complete", new [] { "Act Complete tiles", "Act Complete palette", "Act Complete tilemap", "HUD sprite tiles" } },
                { "Special Stage Complete", new [] { "Act Complete tiles", "Act Complete palette", "Special Stage Complete tilemap", "HUD sprite tiles" } },
                { "Ending 1", new [] { "Map screen 1 tileset", "Ending palette", "Ending 1 tilemap" } },
                { "Ending 2", new [] { "Map screen 1 tileset", "Ending palette", "Ending 2 tilemap" } },
                { "Credits", new [] { "Map screen 2 tileset", "Title screen sprites", "Credits tilemap", "Credits palette" } },
                { "End sign", new [] { "End sign tileset", "End sign palette" } },
                { "Rings", new [] { "Rings", "Green Hill palette" } },
                { "Dr. Robotnik", new [] { "Boss sprites 1", "Boss sprites 2", "Boss sprites 3", "Boss sprites palette" } },
                { "Capsule", new [] { "Capsule sprites", "Boss sprites palette" } }
            },
            Levels = new List<Game.LevelHeader>
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
        public List<Level> Levels { get; } = new();
        public List<GameText> GameText { get; } = new();
        public List<ArtItem> Art { get; } = new();

        private readonly Dictionary<int, TileSet> _tileSets = new();
        private readonly Dictionary<int, Floor> _floors = new();
        private readonly Dictionary<int, BlockMapping> _blockMappings = new();
        private readonly Dictionary<int, Palette> _palettes = new();
        private TileSet _rings;
        private readonly Dictionary<Game.Asset, IDataItem> _assetsLookup = new();

        public Cartridge(string path, Action<string> logger)
        {
            _logger = logger;
            logger($"Loading {path}...");
            var sw = Stopwatch.StartNew();
            Memory = new Memory(File.ReadAllBytes(path));
            ReadLevels();
            ReadGameText();
            ReadAssets();

            // Apply rings to level tilesets
            foreach (var tileSet in Levels.Select(x => x.TileSet).Distinct())
            {
                tileSet.SetRings(_rings.Tiles[0]);
            }

            _logger($"Load complete in {sw.Elapsed}");
        }

        private void ReadAssets()
        {
            _rings = new TileSet(Memory, 0x2Fcf0, 24 * 32, 4, TileSet.Groupings.Ring, 8); // TODO this
            _assetsLookup.Clear();

            _logger("Loading art...");

            foreach (var kvp in Sonic1MasterSystem.AssetGroups)
            {
                var item = new ArtItem{Name = kvp.Key};
                foreach (var asset in kvp.Value.Select(x => Sonic1MasterSystem.Assets[x]))
                {
                    var offset = asset.GetOffset(Memory);

                    switch (asset.Type)
                    {
                        case Game.Asset.Types.TileSet:
                            _assetsLookup[asset] = item.TileSet = asset.BitPlanes > 0 
                                ? GetTileSet(offset, asset.GetLength(Memory), asset.BitPlanes, asset.TileGrouping, asset.TilesPerRow) 
                                : GetTileSet(offset, asset.TileGrouping, asset.TilesPerRow);
                            break;
                        case Game.Asset.Types.Palette:
                            _assetsLookup[asset] = item.Palette = GetPalette(offset, asset.FixedSize / 16);
                            item.PaletteEditable = !asset.Hidden; // Only applies to palettes...
                            break;
                        case Game.Asset.Types.ForegroundTileMap:
                            // We assume these are set first
                            _assetsLookup[asset] = item.TileMap = new TileMap(Memory, offset, asset.GetLength(Memory));
                            item.TileMap.SetAllForeground();
                            break;
                        case Game.Asset.Types.TileMap:
                        {
                            // We assume these are set second so we have to check if it's a set or overlay
                            var tileMap = new TileMap(Memory, offset, asset.GetLength(Memory));
                            if (tileMap.IsOverlay())
                            {
                                item.TileMap.OverlayWith(tileMap);
                                _assetsLookup[asset] = item.TileMap; // Point at the same object for both
                            }
                            else
                            {
                                _assetsLookup[asset] = item.TileMap = tileMap;
                            }
                            break;
                        }
                        case Game.Asset.Types.SpriteTileSet:
                            var tileSet = asset.BitPlanes > 0 
                                ? GetTileSet(offset, asset.GetLength(Memory), asset.BitPlanes, asset.TileGrouping, asset.TilesPerRow) 
                                : GetTileSet(offset, asset.TileGrouping, asset.TilesPerRow);
                            _assetsLookup[asset] = tileSet;
                            item.SpriteTileSets.Add(tileSet);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                Art.Add(item);
            }
        }

        private void ReadLevels()
        {
            _logger("Loading levels...");
            DisposeAll(_blockMappings);
            DisposeAll(_tileSets);
            Levels.Clear();
            foreach (var level in Sonic1MasterSystem.Levels)
            {
                _logger($"- Loading level {level.Name} at offset ${level.Offset:X}...");
                Levels.Add(new Level(this, level.Offset, level.Name));
            }
        }

        public TileSet GetTileSet(int offset, List<Point> grouping, int tilesPerRow)
        {
            return GetItem(_tileSets, offset, () => new TileSet(Memory, offset, grouping, tilesPerRow));
        }

        private TileSet GetTileSet(int offset, int length, int bitPlanes, List<Point> tileGrouping, int tilesPerRow)
        {
            return GetItem(_tileSets, offset, () => new TileSet(Memory, offset, length, bitPlanes, tileGrouping, tilesPerRow));
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
            _logger("Loading game text...");
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
            _logger("Building ROM...");
            var sw = Stopwatch.StartNew();
            // We clone the memory to a memory stream
            var memory = Memory.GetStream(0, Memory.Count).ToArray();

            // Data we read/write and what we can repack...
            // Start    End     Description                         Repacking?
            // -----------------------------------------------------------------------------
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
            // 2fcf0    2ffff   Rings (+ unused)                    TODO
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
                var data = gameText.GetData();
                data.CopyTo(memory, gameText.Offset);
                _logger($"- Wrote game text \"{gameText.Text}\" at offset ${gameText.Offset:X}, length {data.Count} bytes");
            }
            // - Level palettes (at original offsets)
            // 629e..65ed inclusive, with Sky Base lightning not covered
            foreach (var palette in Levels.SelectMany(x => new[]{x.Palette, x.CyclingPalette}).Distinct())
            {
                var data = palette.GetData();
                data.CopyTo(memory, palette.Offset);
                _logger($"- Wrote palette at offset ${palette.Offset:X}, length {data.Count} bytes");
            }

            // - Floors (filling space)
            // 16dea..1ffff inclusive
            var offset = 0x16dea;
            foreach (var group in Levels.GroupBy(l => l.Floor))
            {
                var floor = group.Key;
                var data = floor.GetData();
                data.CopyTo(memory, offset);
                floor.Offset = offset;
                offset += data.Count;
                _logger($"- Wrote floor for level(s) {string.Join(", ", group)} at offset ${floor.Offset:X}, length {data.Count} bytes");
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
            foreach (var group in Levels.GroupBy(l => l.SpriteTileSet))
            {
                var tileSet = group.Key;
                if (tileSet.Offset == 0x2EEB1)
                {
                     // Skip boss tiles TODO fix this
                    continue;
                }
                var data = tileSet.GetData();
                data.CopyTo(memory, offset);
                tileSet.Offset = offset;
                offset += data.Count;
                _logger($"- Wrote sprite tileset for level(s) {string.Join(", ", group)} at offset ${tileSet.Offset:X}, length {data.Count} bytes");
            }

            if (offset > 0x2EEB1)
            {
                throw new Exception("Sprite tile sets out of space");
            }

            // - Lavel background art (filling space)
            // 32fe6..3da27
            // Game engine expects data in the range 30000..3ffff
            offset = 0x32fe6;
            foreach (var group in Levels.GroupBy(l => l.TileSet))
            {
                var tileSet = group.Key;
                var data = tileSet.GetData();
                data.CopyTo(memory, offset);
                tileSet.Offset = offset;
                offset += data.Count;
                _logger($"- Wrote background tileset for level(s) {string.Join(", ", group)} at offset ${tileSet.Offset:X}, length {data.Count} bytes");
            }

            if (offset > 0x3da28)
            {
                throw new Exception("Level tile sets out of space");
            }

            // Next we pack the other assets.
            var assetsToPack = Sonic1MasterSystem.AssetGroups.Values
                .SelectMany(x => x)
                .Distinct()
                .Select(x => new { Name = x, Asset = Sonic1MasterSystem.Assets[x]})
                .ToList();
            foreach (var item in assetsToPack)
            {
                var dataItem = _assetsLookup[item.Asset];
                var data = dataItem.GetData();
                offset = dataItem.Offset;
                if (item.Asset.References.Any(r => r.Type == Game.Reference.Types.Absolute))
                {
                    // First we do the ones with absolute positions. There's nothing to gain by relocating them, so we just copy the data 
                    data.CopyTo(memory, offset);
                    _logger($"- Wrote data for asset {item.Name} at its original location {offset:X}, length {data.Count} bytes");
                }
                else
                {
                    switch (item.Asset.Type)
                    {
                        case Game.Asset.Types.TileMap:
                        case Game.Asset.Types.ForegroundTileMap:
                            // Skip for now
                            continue;
                        case Game.Asset.Types.Palette:
                        case Game.Asset.Types.TileSet:
                        case Game.Asset.Types.SpriteTileSet:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    // For now, we write the rest at their original offsets. We don't check sizes.
                    data.CopyTo(memory, offset);
                    _logger($"- Wrote data for asset {item.Name} at {offset:X}, length {data.Count} bytes");

                    foreach (var reference in item.Asset.References)
                    {
                        switch (reference.Type)
                        {
                            case Game.Reference.Types.PageNumber:
                                var pageNumber = (byte)(offset / 0x4000 + reference.Delta);
                                memory[reference.Offset] = pageNumber;
                                _logger($" - Wrote page number ${pageNumber:X} for offset {offset:X} at reference at {reference.Offset:X}");
                                break;
                            case Game.Reference.Types.Size:
                                var size = (uint)data.Count;
                                memory[reference.Offset + 0] = (byte)(size & 0xff);
                                memory[reference.Offset + 1] = (byte)(size >> 8);
                                _logger($" - Wrote size ${size:X} at reference at {reference.Offset:X}");
                                break;
                            case Game.Reference.Types.Slot1:
                                var value = (uint)(offset % 0x4000 + 0x4000 + reference.Delta);
                                memory[reference.Offset + 0] = (byte)(value & 0xff);
                                memory[reference.Offset + 1] = (byte)(value >> 8);
                                _logger($" - Wrote slot 1 location ${value:X} for offset {offset:X} at reference at {reference.Offset:X}");
                                break;
                        }
                    }
                }
            }

            // - Block mappings (at original offsets)
            // TODO make these flexible if I make it possible to change sizes
            foreach (var group in Levels.GroupBy(l => l.BlockMapping))
            {
                // We need to place both the block data and solidity data
                foreach (var block in group.Key.Blocks)
                {
                    block.GetData().CopyTo(memory, block.Offset);

                    memory[block.SolidityOffset] = block.Data;
                }

                _logger($"- Wrote block mapping for level(s) {string.Join(", ", group)} at offset ${group.Key.Blocks[0].Offset:X}");
            }
            // - Level objects (at original offsets)
            foreach (var group in Levels.GroupBy(l => l.Objects))
            {
                foreach (var obj in group.Key)
                {
                    obj.GetData().CopyTo(memory, obj.Offset);
                }
                _logger($"- Wrote {group.Key.Count()} level objects for level(s) {string.Join(", ", group)} at offset ${group.Key.First().Offset:X}");
            }
            // - Level headers (at original offsets). We do these last so they pick up info from the contained objects.
            foreach (var level in Levels)
            {
                level.GetData().CopyTo(memory, level.Offset);
                _logger($"- Wrote level header for {level} at offset ${level.Offset:X}");
            }

            _logger($"Built ROM image in {sw.Elapsed}");

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
                Total = 0x2EEB1 - 0x2a12a,
                Used = Levels.Select(x => x.SpriteTileSet).Distinct().Where(x => x.Offset != 0x2EEB1).Sum(x => x.GetData().Count)
            };
    }
}
