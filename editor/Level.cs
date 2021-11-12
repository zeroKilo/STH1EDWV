using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace sth1edwv
{
    public class Level : IDataItem
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global

        [Category("Level bounds")]
        [Description("Pixel location of the left of the level")]
        public int LeftPixels { get; set; }

        [Category("Level bounds")]
        [Description("Pixel location of the top of the level")]
        public int TopPixels { get; set; }

        [Category("Level bounds")]
        [Description("Right side of the level in blocks, divided by 8. The screen shows 256px more than this.")] // TODO: encapsulate this?
        public int RightEdgeFactor { get; set; }

        [Category("Level bounds")]
        [Description("Bottom edge of the level in blocks, divided by 8. The screen shows 192px more than this.")] // TODO: encapsulate this?
        public int BottomEdgeFactor { get; set; }

        [Category("Level bounds")]
        [Description("Extra pixels to add to the level height")] // TODO: encapsulate this?
        public int ExtraHeight { get; set; }

        [Category("Start location")] 
        [Description("Block location of Sonic's start position")]
        public int StartX { get; set; }
        
        [Category("Start location")] 
        [Description("Block location of Sonic's start position")]
        public int StartY { get; set; }

        [Category("Level flags")]
        [Description("The level automatically scrolls to the right (like Bridge Act 2)")]
        public bool AutoScrollRight { get; set; }

        [Category("Level flags")]
        [Description("After a pause, the level automatically scrolls upwards! If you get caught at the bottom of the screen, you die")]
        public bool AutoScrollUp { get; set; }

        [Category("Level flags")]
        [Description("The demo play data controls Sonic")]
        public bool DemoMode { get; set; }

        [Category("Level flags")]
        [Description("Locks the screen, no scrolling occurs")]
        public bool DisableScrolling { get; set; }

        [Category("Level flags")]
        [Description("Uses the lightning effect. This overrides the level's own palette")]
        public bool HasLightning { get; set; }

        [Category("Level flags")]
        [Description("Controls the under-water effect (slow movement / water colour / drowning)")]
        public bool HasWater { get; set; }

        [Category("Level flags")]
        [Description("Screen does not scroll down (like Jungle Act 2). If you get caught at the bottom of the screen, you die")]
        public bool NoScrollDown { get; set; }

        [Category("Level flags")]
        [Description("Shows ring count in HUD and rings are displayed. When turned off, no rings are visible, but the sparkle effect still occurs when you collect them")]
        public bool ShowRings { get; set; }

        [Category("Level flags")]
        [Description("Displays the time")]
        public bool ShowTime { get; set; }

        [Category("Level flags")]
        [Description("The screen scrolls smoothly, allowing you to get ahead of it")]
        public bool SlowScroll { get; set; }

        [Category("Level flags")]
        [Description("Centers the time display when on a special stage. Outside of the special stage causes the game to switch to the special stage")]
        public bool SpecialStageTimer { get; set; }

        [Category("Level flags")]
        [Description("Slow up and down wave effect (like Sky Base Act 2)")]
        public bool WaveScrollY { get; set; }

        [Category("Palette")]
        [Description("Number of frames between palette changes, 0 to disable")]
        public int PaletteCycleRate { get; set; }

        [Category("Palette")]
        [Description("Use the boss underwater palette (like Labyrinth Act 3)")]
        public bool UseUnderwaterBossPalette { get; set; }

        [Category("General")] 
        [Description("Which music track to play")]
        //[TypeConverter(typeof(MusicConverter))]
        public int MusicIndex { get; set; }

        /*
        public class MusicConverter : StringConverter
        {
            // Enable a combo
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            // Disable free-form text
            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;
            }

            // Get values
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                return new StandardValuesCollection(_musicTracks);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                return (value as MusicItem)?.ToString();
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (value is int i && destinationType == typeof(string))
                {
                    return _musicTracks.First(x => x.Index == i).ToString();
                }

                if (value is MusicItem mi && destinationType == typeof(string))
                {
                    return mi.ToString();
                }
                return null;
            }
        }

        private class MusicItem
        {
            public int Index { get; }
            private readonly string _name;

            public MusicItem(int index, string name)
            {
                Index = index;
                _name = name;
            }

            public override string ToString()
            {
                return $"{Index}: {_name}";
            }
        }

        private static List<MusicItem> _musicTracks = new()
        {
            new(0, "Green Hill"),
            new(1, "Bridge"),
            new(2, "Jungle"),
            new(3, "Labyrinth"),
            new(4, "Scrap Brain"),
            new(5, "Sky Base"),
            new(6, "Title"),
            new(8, "Invincibility"),
            new(9, "Level Complete"),
            new(10, "Death"),
            new(11, "Boss"),
        };
*/

        // Objects representing referenced data
        [Category("General")] public TileSet TileSet { get; }
        [Category("General")] public TileSet SpriteTileSet { get; }
        [Category("General")] public Floor Floor { get; }
        [Category("General")] public LevelObjectSet Objects { get; }
        [Category("General")] public BlockMapping BlockMapping { get; }
        [Category("General")] public Palette Palette { get; }
        [Category("General")] public Palette CyclingPalette { get; }

        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper restore MemberCanBePrivate.Global

        [Category("General")] public int Offset { get; }

        private readonly string _label;
        private readonly int _floorWidth;
        private readonly int _floorHeight;
        private readonly int _floorAddress;
        private readonly int _floorSize;
        private readonly int _offsetArt;
        private readonly int _offsetObjectLayout;
        private readonly int _initPalette;

        // These should be encapsulated by a sprite art object
        private readonly int _spriteArtAddress;
        private readonly int _spriteArtPage;

        // These should be encapsulated by a cycling palette object
        private readonly int _paletteCycleCount;
        private readonly int _paletteCycleIndex;

        private readonly int _solidityIndex;

        private readonly byte _unknownByte;

        public Level(Cartridge cartridge, int offset, IList<Palette> palettes, string label)
        {
            _label = label;
            Offset = offset;
            int blockMappingOffset;

            using (var stream = cartridge.Memory.GetStream(offset, 37))
            {
                using (var reader = new BinaryReader(stream))
                {
                    //   SP FW FW FH  FH LX LX ??  LW LY LY XH  LH SX SY FL 
                    //   FL FS FS BM  BM LA LA 09  SA SA IP CS  CC CP OL OL 
                    //   SR UW TL 00  MU
                    _solidityIndex = reader.ReadByte(); // SP
                    _floorWidth = reader.ReadUInt16(); // FW FW
                    _floorHeight = reader.ReadUInt16(); // FH FH
                    LeftPixels = reader.ReadUInt16(); // LX LX
                    _unknownByte = reader.ReadByte();  // ??
                    RightEdgeFactor = reader.ReadByte();  // LW
                    TopPixels = reader.ReadUInt16(); // LY LY
                    ExtraHeight = reader.ReadByte(); // XH
                    BottomEdgeFactor = reader.ReadByte(); // LH
                    StartX = reader.ReadByte(); // SX
                    StartY = reader.ReadByte(); // SY
                    _floorAddress = reader.ReadUInt16(); // FL FL: relative to 0x14000
                    _floorSize = reader.ReadUInt16(); // FS FS: compressed size in bytes
                    blockMappingOffset = reader.ReadUInt16(); // BM BM: relative to 0x10000
                    _offsetArt = reader.ReadUInt16(); // LA LA: Relative to 0x30000
                    _spriteArtPage = reader.ReadByte(); // 09: Page for the below, using slot 1
                    _spriteArtAddress = reader.ReadUInt16(); // SA SA: offset from start of above bank
                    _initPalette = reader.ReadByte(); // IP: Index of palette
                    PaletteCycleRate = reader.ReadByte(); // CS: Number of frames between palette cycles
                    _paletteCycleCount = reader.ReadByte(); // CC: Number of palette cycles in a loop
                    _paletteCycleIndex = reader.ReadByte(); // CP: Which cycling palette to use
                    _offsetObjectLayout = reader.ReadUInt16(); // OL OL: relative to 0x15580
                    var flags = reader.ReadByte(); // SR
                    // Nothing for bit 0
                    DemoMode = (flags & (1 << 1)) != 0;
                    ShowRings = (flags & (1 << 2)) != 0;
                    AutoScrollRight = (flags & (1 << 3)) != 0;
                    AutoScrollUp = (flags & (1 << 4)) != 0;
                    SlowScroll = (flags & (1 << 5)) != 0;
                    WaveScrollY = (flags & (1 << 6)) != 0;
                    NoScrollDown = (flags & (1 << 7)) != 0;
                    flags = reader.ReadByte(); // UW
                    // Nothing for bits 0..6
                    HasWater = (flags & (1 << 7)) != 0;
                    flags = reader.ReadByte(); // TL
                    SpecialStageTimer = (flags & (1 << 0)) != 0;
                    HasLightning = (flags & (1 << 1)) != 0;
                    // Nothing for bits 2, 3
                    UseUnderwaterBossPalette = (flags & (1 << 4)) != 0;
                    ShowTime = (flags & (1 << 5)) != 0;
                    DisableScrolling = (flags & (1 << 6)) != 0;
                    // Nothing for bit 7
                    // Skip unknown byte
                    reader.ReadByte(); // 00
                    MusicIndex = reader.ReadByte(); // MU
                }       
            }

            if (Offset == 0x15580 + 666)
            {
                // Scrap Brain 2 (BallHog area) has only enough data to fill 2KB 
                _floorHeight /= 2;
            }

            Palette = palettes[_initPalette];
            CyclingPalette = palettes[_paletteCycleIndex + 8];

            // The tile palette is effectively the first cycling palette entry...
            TilePalette = CyclingPalette.GetSubPalette(0, 16);
            SpritePalette = Palette.GetSubPalette(16, 16);

            TileSet = cartridge.GetTileSet(_offsetArt + 0x30000, true, false);

            SpriteTileSet = cartridge.GetTileSet(_spriteArtAddress + _spriteArtPage * 0x4000, false, true);

            Floor = cartridge.GetFloor(
                _floorAddress + 0x14000, 
                _floorSize, 
                _floorWidth);
            BlockMapping = cartridge.GetBlockMapping(blockMappingOffset + 0x10000, _solidityIndex, TileSet);
            Objects = new LevelObjectSet(cartridge, 0x15580 + _offsetObjectLayout);
        }

        [Browsable(false)]
        public Palette SpritePalette { get; }
        [Browsable(false)]
        public Palette TilePalette { get; }


        public override string ToString()
        {
            return _label;
        }

        public TreeNode ToNode()
        {
            var uncompressedSize = _floorWidth * _floorHeight;
            var result = new TreeNode("Header")
            {
                Nodes =
                {
                    new TreeNode($"Floor Size           = {_floorWidth} x {_floorHeight} ({uncompressedSize}B)"),
                    new TreeNode($"Floor Data           = @0x{_floorAddress:X} Size: {_floorSize}B ({(double)(uncompressedSize - _floorSize) / uncompressedSize:P})"),
                    new TreeNode($"Level Limits         = ({RightEdgeFactor}, {BottomEdgeFactor})"),
                    new TreeNode($"Level Offset         = ({LeftPixels}, {TopPixels})"),
                    new TreeNode($"Extended Height      = {ExtraHeight}"),
                    new TreeNode($"Offset Art           = 0x{_offsetArt:X8}"),
                    new TreeNode($"Offset Object Layout = 0x{_offsetObjectLayout:X8}"),
                    new TreeNode($"Initial Palette      = {_initPalette}"),
                    new TreeNode($"Cycles Palette       = {_paletteCycleIndex}"),
                    new TreeNode($"Palette cycles       = {_paletteCycleCount}"),
                    Objects.ToNode()
                }
            };
            result.Expand();
            return result;
        }

        public IList<byte> GetData()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write((byte)_solidityIndex);
            writer.Write((ushort)_floorWidth);
            writer.Write((ushort)_floorHeight);
            writer.Write((ushort)LeftPixels);
            writer.Write(_unknownByte);
            writer.Write((byte)RightEdgeFactor);
            writer.Write((ushort)TopPixels);
            writer.Write((byte)ExtraHeight);
            writer.Write((byte)BottomEdgeFactor);
            writer.Write((byte)StartX);
            writer.Write((byte)StartY);
            writer.Write((ushort)(Floor.Offset - 0x14000)); // relative to 0x14000
            writer.Write((ushort)Floor.GetData().Count); // compressed size in bytes
            writer.Write((ushort)(BlockMapping.Blocks[0].Offset - 0x10000)); // relative to 0x10000
            writer.Write((ushort)(TileSet.Offset - 0x30000)); // Relative to 0x30000
            writer.Write((byte)_spriteArtPage); // Page for the above, using slot 2
            writer.Write((ushort)_spriteArtAddress); // CPU address when paged in
            writer.Write((byte)_initPalette); // Index of palette
            writer.Write((byte)PaletteCycleRate); // Number of frames between palette cycles
            writer.Write((byte)_paletteCycleCount); // Number of palette cycles in a loop
            writer.Write((byte)_paletteCycleIndex); // Which cycling palette to use
            writer.Write((ushort)_offsetObjectLayout); // relative to 0x15580
            var flags = 0;
            if (DemoMode) flags |= 1 << 1;
            if (ShowRings) flags |= 1 << 2;
            if (AutoScrollRight) flags |= 1 << 3;
            if (AutoScrollUp) flags |= 1 << 4;
            if (SlowScroll) flags |= 1 << 5;
            if (WaveScrollY) flags |= 1 << 6;
            if (NoScrollDown) flags |= 1 << 7;
            writer.Write((byte)flags);
            flags = 0;
            if (HasWater) flags |= 1 << 7;
            writer.Write((byte)flags);
            flags = 0;
            if (SpecialStageTimer) flags |= 1 << 0;
            if (HasLightning) flags |= 1 << 1;
            if (UseUnderwaterBossPalette) flags |= 1 << 4;
            if (ShowTime) flags |= 1 << 5;
            if (DisableScrolling) flags |= 1 << 6;
            writer.Write((byte)flags);
            writer.Write((byte)0); // Always 0
            writer.Write((byte)MusicIndex);
            return stream.ToArray();
        }
    }
}