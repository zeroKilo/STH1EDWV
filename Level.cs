using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace sth1edwv
{
    public class Level
    {
        //   SP FW FW FH  FH LX LX ??  LW LY LY XH  LH SX SY FL 
        //   FL FS FS BM  BM LA LA 09  SA SA IP CS  CC CP OL OL 
        //   SR UW TL 00  MU
        private byte[] header;
        public readonly byte   solidityIndex;
        public readonly ushort floorWidth;
        public readonly ushort floorHeight;
        public readonly int   floorAddress;
        public readonly ushort floorSize;
        public readonly int   blockMappingAddress;
        private readonly ushort levelXOffset;
        private readonly byte   levelWidth;
        private readonly ushort levelYOffset;
        private readonly byte   levelExtHeight;
        private readonly byte   levelHeight;
        private readonly ushort offsetArt;
        public readonly ushort offsetObjectLayout;
        private readonly byte   initPalette;
        public TileSet TileSet { get; }
        public Floor Floor { get; }
        public LevelObjectSet ObjSet { get; }

        public BlockMapping BlockMapping { get; }
        private readonly string _label;

        public Level(Cartridge cartridge, int offset, int artBanksTableOffset, IList<Palette> palettes, string label)
        {
            _label = label;
            var address = BitConverter.ToUInt16(cartridge.Memory, offset);
            header = new byte[37];
            Array.Copy(cartridge.Memory, address + 0x15580, header, 0, header.Length);

            solidityIndex = header[0];
            floorWidth = BitConverter.ToUInt16(header, 1);
            floorHeight = BitConverter.ToUInt16(header, 3);
            if (address == 666)
                floorHeight /= 2;
            floorAddress = BitConverter.ToUInt16(header, 15) + 0x14000;
            floorSize = BitConverter.ToUInt16(header, 17);
            blockMappingAddress = BitConverter.ToUInt16(header, 19) + 0x10000;
            levelXOffset = BitConverter.ToUInt16(header, 5);
            levelWidth = header[8];
            levelYOffset = BitConverter.ToUInt16(header, 9);
            levelExtHeight = header[11];
            levelHeight = header[12];
            offsetArt = BitConverter.ToUInt16(header, 21);
            offsetObjectLayout = BitConverter.ToUInt16(header, 30);
            initPalette = header[26];
            if (artBanksTableOffset > 0)
            {
                var levelIndex = (offset - 0x15580) / 2;
                var artBank = cartridge.Memory[artBanksTableOffset + levelIndex];
                TileSet = cartridge.GetTileSet(offsetArt + artBank * 0x4000, palettes[initPalette]);
            }
            else
            {
                TileSet = cartridge.GetTileSet(offsetArt + 0x30000, palettes[initPalette]);
            }
            Floor = cartridge.GetFloor(floorAddress, floorSize);
            BlockMapping = cartridge.GetBlockMapping(blockMappingAddress, solidityIndex, TileSet);
            ObjSet = new LevelObjectSet(cartridge, 0x15580 + offsetObjectLayout);
        }

        public override string ToString()
        {
            return _label;
        }

        public TreeNode ToNode()
        {
            TreeNode result = new TreeNode("Header");
            result.Nodes.Add($"Floor Size           = ({floorWidth} x {floorHeight})");
            result.Nodes.Add($"Floor Data           = (@0x{floorAddress:X} Size: 0x{floorSize:X})");
            result.Nodes.Add($"Level Size           = ({levelWidth} x {levelHeight})");
            result.Nodes.Add($"Level Offset         = (dx:{levelXOffset} dy:{levelYOffset})");
            result.Nodes.Add($"Extended Height      = {levelExtHeight}");
            result.Nodes.Add($"Offset Art           = 0x{offsetArt:X8}");
            result.Nodes.Add($"Offset Object Layout = 0x{offsetObjectLayout:X8}");
            result.Nodes.Add($"Initial Palette      = {initPalette}");
            result.Nodes.Add(ObjSet.ToNode());
            result.Expand();
            return result;
        }

        public Tile GetTile(int index)
        {
            return TileSet.Tiles[index];
        }

        public Bitmap Render(bool withObjects, bool blockGaps, bool tileGaps, bool blockLabels)
        {
            var blockSize = 32;
            var tileSize = 8;
            if (tileGaps)
            {
                blockSize += 4;
                ++tileSize;
            }

            if (blockGaps)
            {
                ++blockSize;
            }

            const int labelWidth = 13;
            const int labelHeight = 9;

            var result = new Bitmap(floorWidth * blockSize, floorHeight * blockSize);
            using (var f = new Font("Consolas", 8, FontStyle.Bold))
            using (var g = Graphics.FromImage(result)) 
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.Clear(Color.White);

                for (var blockX = 0; blockX < floorWidth; ++blockX)
                for (var blockY = 0; blockY < floorHeight; ++blockY)
                {
                    var blockIndex = Floor.BlockIndices[blockX + blockY * floorWidth];
                    var block = BlockMapping.Blocks[blockIndex];
                    for (var tileX = 0; tileX < 4; ++tileX)
                    for (var tileY = 0; tileY < 4; ++tileY)
                    {
                        var tileIndex = block.TileIndices[tileX + tileY * 4];
                        var tile = TileSet.Tiles[tileIndex];
                        var x = blockX * blockSize + tileX * tileSize;
                        var y = blockY * blockSize + tileY * tileSize;
                        g.DrawImageUnscaled(tile.Image, x, y);
                    }

                    if (blockLabels)
                    {
                        // Draw a rect over it for the label
//                        g.FillRectangle(Brushes.White, blockX * blockSize, blockY * blockSize, labelWidth, labelHeight);
                        g.DrawString(blockIndex.ToString("X2"), f, Brushes.Black, blockX * blockSize, blockY * blockSize - 1);
                        g.DrawString(blockIndex.ToString("X2"), f, Brushes.White, blockX * blockSize - 1, blockY * blockSize - 2);
                    }
                }

                if (withObjects)
                {
                    var image = Properties.Resources.package;
                    // Draw objects
                    foreach (var obj in ObjSet.objs)
                    {
                        var x = obj.x * blockSize;
                        var y = obj.y * blockSize;
                        g.DrawRectangle(Pens.Blue, x, y, blockSize, blockSize);
                        g.DrawImageUnscaled(image, x + blockSize / 2 - image.Width / 2, y + blockSize / 2 - image.Height / 2);

                        x += blockSize - labelWidth;
                        y += blockSize - labelHeight;
                        g.FillRectangle(Brushes.Blue, x, y, labelWidth, labelHeight);
                        g.DrawString(obj.type.ToString("X2"), f, Brushes.White, x - 1, y - 2);
                    }
                }
            }

            return result;
        }
    }
}
