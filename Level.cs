using System;
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
        public ushort Left { get; set; }

        [Category("Level bounds")]
        [Description("Pixel location of the top of the level")]
        public ushort Top { get; set; }

        [Category("Level bounds")]
        [Description("Block width of the level, minus 14 and divided by 8")] // TODO: encapsulate this?
        public byte WidthFactor { get; set; }

        [Category("Level bounds")]
        [Description("Block height of the level, minus 6 and divided by 8")] // TODO: encapsulate this?
        public byte HeightFactor { get; set; }

        [Category("Level bounds")]
        [Description("Extra pixels to add to the level height")] // TODO: encapsulate this?
        public byte ExtraHeight { get; set; }

        [Category("Start location")] 
        [Description("Block location of Sonic's start position")]
        public byte StartX { get; set; }
        
        [Category("Start location")] 
        [Description("Block location of Sonic's start position")]
        public byte StartY { get; set; }

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
        public byte PaletteCycleRate { get; set; }

        [Category("Palette")]
        [Description("Use the boss underwater palette (like Labyrinth Act 3)")]
        public bool UseUnderwaterBossPalette { get; set; }

        [Category("General")] 
        [Description("Which music track to play")]
        public byte MusicIndex { get; set; }

        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper restore MemberCanBePrivate.Global

        // Objects representing referenced data
        [Category("General")] public TileSet TileSet { get; }
        [Category("General")] public Floor Floor { get; }
        [Category("General")] private LevelObjectSet Objects { get; }
        [Category("General")] public BlockMapping BlockMapping { get; }

        //   SP FW FW FH  FH LX LX ??  LW LY LY XH  LH SX SY FL 
        //   FL FS FS BM  BM LA LA 09  SA SA IP CS  CC CP OL OL 
        //   SR UW TL 00  MU
        private readonly string _label;
        private readonly ushort _floorWidth;
        private readonly ushort _floorHeight;
        private readonly int _floorAddress;
        private readonly ushort _floorSize;
        private readonly ushort _offsetArt;
        private readonly ushort _offsetObjectLayout;
        private readonly byte _initPalette;

        // Size of blocks used when last rendered, used for turning clicks back into block locations
        private int _blockSize;

        // These should be encapsulated by a sprite art object
        private readonly ushort _spriteArtAddress;
        private readonly byte _spriteArtPage;

        // These should be encapsulated by a cycling palette object
        private readonly byte _paletteCycleCount;
        private readonly byte _paletteCycleIndex;

        public Level(Cartridge cartridge, int offset, int artBanksTableOffset, IList<Palette> palettes, string label)
        {
            int blockMappingAddress;
            byte solidityIndex;
            _label = label;
            Offset = offset;
            LengthConsumed = 37;

            using (var stream = new MemoryStream(cartridge.Memory, offset, 37, false))
            using (var reader = new BinaryReader(stream))
            {
                solidityIndex = reader.ReadByte();
                _floorWidth = reader.ReadUInt16();
                _floorHeight = reader.ReadUInt16();
                Left = reader.ReadUInt16();
                // Skip unknown byte
                reader.ReadByte();
                WidthFactor = reader.ReadByte();
                Top = reader.ReadUInt16();
                ExtraHeight = reader.ReadByte();
                HeightFactor = reader.ReadByte();
                StartX = reader.ReadByte();
                StartY = reader.ReadByte();
                _floorAddress = reader.ReadUInt16(); // relative to 0x14000
                _floorSize = reader.ReadUInt16(); // compressed size in bytes
                blockMappingAddress = reader.ReadUInt16(); // relative to 0x10000
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

            Floor = cartridge.GetFloor(_floorAddress + 0x14000, _floorSize, _floorWidth);
            BlockMapping = cartridge.GetBlockMapping(blockMappingAddress + 0x10000, solidityIndex, TileSet);
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
                    new TreeNode($"Floor Data           = @0x{_floorAddress:X} Size: 0x{_floorSize:X} ({(double)(uncompressedSize - _floorSize) / uncompressedSize:P})"),
                    new TreeNode($"Level Size           = ({WidthFactor} x {HeightFactor})"),
                    new TreeNode($"Level Offset         = (dx:{Left} dy:{Top})"),
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
                var x = Left + Left / 32 * (_blockSize - 32);
                var y = Top + Top / 32 * (_blockSize - 32);
                var w = (WidthFactor * 8 + 14) * _blockSize;
                var h = (HeightFactor * 8 + 6) * _blockSize + ExtraHeight;
                using var brush = new SolidBrush(Color.FromArgb(128, Color.Black));
                // Top
                g.FillRectangle(brush, 0, 0, result.Width, y);
                // Bottom
                g.FillRectangle(brush, 0, y + h, result.Width, result.Height - y - h);
                // Left chunk
                g.FillRectangle(brush, 0, y, x, h);
                // Right chunk
                g.FillRectangle(brush, x + w, y, result.Width - x - w, h);

                g.DrawRectangle(Pens.Red, x, y, w, h);
            }

            return result;
        }

        public void AdjustPixelsToTile(ref int x, ref int y)
        {
            x /= _blockSize;
            y /= _blockSize;
        }

        public int Offset { get; }
        public int LengthConsumed { get; }

        public IList<byte> GetData()
        {
            throw new NotImplementedException();
        }
    }
}