using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
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

        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper restore MemberCanBePrivate.Global

        // Objects representing referenced data
        [Category("General")] public TileSet TileSet { get; }
        [Category("General")] public Floor Floor { get; }
        [Category("General")] private LevelObjectSet Objects { get; }
        [Category("General")] public BlockMapping BlockMapping { get; }

        [Category("General")] public int Offset { get; }

        //   SP FW FW FH  FH LX LX ??  LW LY LY XH  LH SX SY FL 
        //   FL FS FS BM  BM LA LA 09  SA SA IP CS  CC CP OL OL 
        //   SR UW TL 00  MU
        private readonly string _label;
        private readonly int _floorWidth;
        private readonly int _floorHeight;
        private readonly int _floorAddress;
        private readonly int _floorSize;
        private readonly int _offsetArt;
        private readonly int _offsetObjectLayout;
        private readonly int _initPalette;

        // Size of blocks used when last rendered, used for turning clicks back into block locations
        private int _blockSize;

        // These should be encapsulated by a sprite art object
        private readonly int _spriteArtAddress;
        private readonly int _spriteArtPage;

        // These should be encapsulated by a cycling palette object
        private readonly int _paletteCycleCount;
        private readonly int _paletteCycleIndex;

        private readonly int _solidityIndex;
        private readonly int _blockMappingAddress;

        // TODO: make these come from the cartridge layout?
        private readonly Dictionary<int, int> _originalSizes = new()
        {
            { 0x2dea, 0x83e }, // Green Hill Act 1/Ending Sequence
            { 0x3628, 0x661 }, // Green Hill Act 2
            { 0x3c89, 0x32d }, // Green Hill Act 3
            { 0x3fb6, 0xaac }, // Jungle Act 1
            { 0x4a62, 0x8db }, // Jungle Act 2/Special Stage 4/Special Stage 8
            { 0x533d, 0x69a }, // Scrap Brain Act 1
            { 0x59d7, 0x904 }, // Scrap Brain Act 2
            { 0x62db, 0x8f8 }, // Scrap Brain Act 2 (Emerald Maze), from corridor
            { 0x6bd3, 0x6af }, // Scrap Brain Act 2 (Ballhog Area)
            { 0x7282, 0x8b2 }, // Scrap Brain Act 3
            { 0x7b34, 0x39d }, // Sky Base Act 2
            { 0x7ed1, 0x694 }, // Bridge Act 1
            { 0x8565, 0x8c2 }, // Labyrinth Act 1
            { 0x8e27, 0xd13 }, // Labyrinth Act 2
            { 0x9b3a, 0x76e }, // Sky Base Act 1
            { 0xa2a8, 0x499 }, // Bridge Act 2
            { 0xa741, 0x4c0 }, // Sky Base Act 2 (Interior)/Sky Base Act 3
            { 0xac01, 0x3f5 }, // Jungle Act 3
            { 0xaff6, 0x30b }, // Labyrinth Act 3
            { 0xb301, 0x140 }, // Bridge Act 3
            { 0xb441, 0x760 }, // Special Stage 1/2/3/5/6/7
        };

        public Level(Cartridge cartridge, int offset, int artBanksTableOffset, IList<Palette> palettes, string label)
        {
            _label = label;
            Offset = offset;

            using (var stream = new MemoryStream(cartridge.Memory, offset, 37, false))
            using (var reader = new BinaryReader(stream))
            {
                _solidityIndex = reader.ReadByte();
                _floorWidth = reader.ReadUInt16();
                _floorHeight = reader.ReadUInt16();
                LeftPixels = reader.ReadUInt16();
                // Skip unknown byte
                reader.ReadByte();
                RightEdgeFactor = reader.ReadByte();
                TopPixels = reader.ReadUInt16();
                ExtraHeight = reader.ReadByte();
                BottomEdgeFactor = reader.ReadByte();
                StartX = reader.ReadByte();
                StartY = reader.ReadByte();
                _floorAddress = reader.ReadUInt16(); // relative to 0x14000
                _floorSize = reader.ReadUInt16(); // compressed size in bytes
                _blockMappingAddress = reader.ReadUInt16(); // relative to 0x10000
                _offsetArt = reader.ReadUInt16(); // Relative to 0x30000
                _spriteArtAddress = reader.ReadUInt16(); // CPU address when paged in
                _spriteArtPage = reader.ReadByte(); // Page for the above, using slot 2
                _initPalette = reader.ReadByte(); // Index of palette
                PaletteCycleRate = reader.ReadByte(); // Number of frames between palette cycles
                _paletteCycleCount = reader.ReadByte(); // Number of palette cycles in a loop
                _paletteCycleIndex = reader.ReadByte(); // Which cycling palette to use
                _offsetObjectLayout = reader.ReadUInt16(); // relative to 0x15580
                var flags = reader.ReadByte();
                // Nothing for bit 0
                DemoMode = (flags & (1 << 1)) != 0;
                ShowRings = (flags & (1 << 2)) != 0;
                AutoScrollRight = (flags & (1 << 3)) != 0;
                AutoScrollUp = (flags & (1 << 4)) != 0;
                SlowScroll = (flags & (1 << 5)) != 0;
                WaveScrollY = (flags & (1 << 6)) != 0;
                NoScrollDown = (flags & (1 << 7)) != 0;
                flags = reader.ReadByte();
                // Nothing for bits 0..6
                HasWater = (flags & (1 << 7)) != 0;
                flags = reader.ReadByte();
                SpecialStageTimer = (flags & (1 << 0)) != 0;
                HasLightning = (flags & (1 << 1)) != 0;
                // Nothing for bits 2, 3
                UseUnderwaterBossPalette = (flags & (1 << 4)) != 0;
                ShowTime = (flags & (1 << 5)) != 0;
                DisableScrolling = (flags & (1 << 6)) != 0;
                // Nothing for bit 7
                // Skip unknown byte
                reader.ReadByte();
                MusicIndex = reader.ReadByte();
            }

            if (Offset == 0x15580 + 666)
            {
                // Scrap Brain 2 (BallHog area) has only enough data to fill 2KB 
                _floorHeight /= 2;
            }

            if (artBanksTableOffset > 0)
            {
                var levelIndex = (offset - 0x15580) / 2;
                var artBank = cartridge.Memory[artBanksTableOffset + levelIndex];
                TileSet = cartridge.GetTileSet(_offsetArt + artBank * 0x4000, palettes[_initPalette], true);
            }
            else
            {
                TileSet = cartridge.GetTileSet(_offsetArt + 0x30000, palettes[_initPalette], true);
            }

            if (!_originalSizes.TryGetValue(_floorAddress, out var originalSize))
            {
                originalSize = 0;
            }

            Floor = cartridge.GetFloor(
                _floorAddress + 0x14000, 
                _floorSize, 
                originalSize,
                _floorWidth);
            BlockMapping = cartridge.GetBlockMapping(_blockMappingAddress + 0x10000, _solidityIndex, TileSet);
            Objects = new LevelObjectSet(cartridge, 0x15580 + _offsetObjectLayout);
        }

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
                    new TreeNode($"Floor Data           = @0x{_floorAddress:X} Size: {_floorSize}B ({(double)(uncompressedSize - _floorSize) / uncompressedSize:P}), max {Floor.MaximumCompressedSize}B"),
                    new TreeNode($"Level Limits         = ({RightEdgeFactor}, {BottomEdgeFactor})"),
                    new TreeNode($"Level Offset         = ({LeftPixels}, {TopPixels})"),
                    new TreeNode($"Extended Height      = {ExtraHeight}"),
                    new TreeNode($"Offset Art           = 0x{_offsetArt:X8}"),
                    new TreeNode($"Offset Object Layout = 0x{_offsetObjectLayout:X8}"),
                    new TreeNode($"Initial Palette      = {_initPalette}"),
                    new TreeNode($"Palette cycles       = {_initPalette}"),
                    Objects.ToNode()
                }
            };
            result.Expand();
            return result;
        }

        public Bitmap Render(bool withObjects, bool blockGaps, bool tileGaps, bool blockLabels, bool levelBounds)
        {   
            _blockSize = 32;
            var tileSize = 8;
            if (tileGaps)
            {
                _blockSize += 4;
                ++tileSize;
            }

            if (blockGaps)
            {
                ++_blockSize;
            }

            var result = new Bitmap(_floorWidth * _blockSize, _floorHeight * _blockSize);
            using var f = new Font(SystemFonts.MessageBoxFont.FontFamily, 8.0f);
            using var g = Graphics.FromImage(result);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.Clear(SystemColors.Window);

            for (var blockX = 0; blockX < _floorWidth; ++blockX)
            for (var blockY = 0; blockY < _floorHeight; ++blockY)
            {
                var blockIndex = Floor.BlockIndices[blockX + blockY * _floorWidth];
                if (blockIndex < BlockMapping.Blocks.Count)
                {
                    var block = BlockMapping.Blocks[blockIndex];
                    for (var tileX = 0; tileX < 4; ++tileX)
                    for (var tileY = 0; tileY < 4; ++tileY)
                    {
                        var tileIndex = block.TileIndices[tileX + tileY * 4];
                        if (tileIndex < TileSet.Tiles.Count)
                        {
                            var tile = TileSet.Tiles[tileIndex];
                            var x = blockX * _blockSize + tileX * tileSize;
                            var y = blockY * _blockSize + tileY * tileSize;
                            g.DrawImageUnscaled(tile.Image, x, y);
                        }
                    }
                }
                else
                {
                    g.DrawIcon(SystemIcons.Error, new Rectangle(blockX * _blockSize, blockY * _blockSize, 32, 32));
                }

                if (blockLabels)
                {
                    g.DrawString(blockIndex.ToString("X2"), f, Brushes.Black, blockX * _blockSize,
                        blockY * _blockSize - 1);
                    g.DrawString(blockIndex.ToString("X2"), f, Brushes.White, blockX * _blockSize - 1,
                        blockY * _blockSize - 2);
                }
            }

            if (withObjects)
            {
                var image = Properties.Resources.package;
                // Draw objects
                void DrawObject(int x, int y, string label)
                {
                    x *= _blockSize;
                    y *= _blockSize;
                    g.DrawRectangle(Pens.Blue, x, y, _blockSize, _blockSize);
                    g.DrawImageUnscaled(image, x + _blockSize / 2 - image.Width / 2,
                        y + _blockSize / 2 - image.Height / 2);

                    var dims = g.MeasureString(label, f).ToSize();

                    y += _blockSize;
                    if (y + dims.Height > result.Height)
                    {
                        y -= _blockSize + dims.Height;
                    }
                    g.FillRectangle(Brushes.Blue, x, y, dims.Width, dims.Height);
                    g.DrawString(label, f, Brushes.White, x, y);
                }

                foreach (var levelObject in Objects)
                {
                    if (LevelObject.Names.TryGetValue(levelObject.Type, out var name))
                    {
                        DrawObject(levelObject.X, levelObject.Y, name);
                    }
                    else
                    {
                        DrawObject(levelObject.X, levelObject.Y, levelObject.Type.ToString("X2"));
                    }
                }

                DrawObject(StartX, StartY, "Sonic");
            }

            if (levelBounds)
            {
                var left = LeftPixels + LeftPixels / 32 * (_blockSize - 32) + 8;
                var top = TopPixels + TopPixels / 32 * (_blockSize - 32);
                var right = RightEdgeFactor * _blockSize * 8 + (256 / 32 * _blockSize);
                var bottom = BottomEdgeFactor * _blockSize * 8 + (192 / 32 * _blockSize) + ExtraHeight;
                var rect = new Rectangle(left, top, right - left, bottom - top);
                // Draw the grey region
                using var brush = new SolidBrush(Color.FromArgb(128, Color.Black));
                g.SetClip(rect, CombineMode.Exclude);
                g.FillRectangle(brush, 0, 0, result.Width, result.Height);
                // Draw the red border, a bit bigger so it's on the outside
                rect.Width += 1;
                rect.Height += 1;
                g.DrawRectangle(Pens.Red, rect);
            }

            return result;
        }

        public void AdjustPixelsToTile(ref int x, ref int y)
        {
            x /= _blockSize;
            y /= _blockSize;
        }

        public IList<byte> GetData()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            writer.Write((byte)_solidityIndex);
            writer.Write((ushort)_floorWidth);
            writer.Write((ushort)_floorHeight);
            writer.Write((ushort)LeftPixels);
            // Unknown byte
            writer.Write((byte)0);
            writer.Write((byte)RightEdgeFactor);
            writer.Write((ushort)TopPixels);
            writer.Write((byte)ExtraHeight);
            writer.Write((byte)BottomEdgeFactor);
            writer.Write((byte)StartX);
            writer.Write((byte)StartY);
            writer.Write((ushort)_floorAddress); // relative to 0x14000
            writer.Write((ushort)_floorSize); // compressed size in bytes
            writer.Write((ushort)_blockMappingAddress); // relative to 0x10000
            writer.Write((ushort)_offsetArt); // Relative to 0x30000
            writer.Write((ushort)_spriteArtAddress); // CPU address when paged in
            writer.Write((byte)_spriteArtPage); // Page for the above, using slot 2
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
            writer.Write((byte)MusicIndex);
            return stream.ToArray();
        }

        public List<RomBuilder.DataChunk.Reference> GetReferences()
        {
            // Level headers have lots of references!
            return new List<RomBuilder.DataChunk.Reference>();
        }
    }
}