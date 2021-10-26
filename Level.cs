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
        public Floor Floor1 { get; }
        public LevelObjectSet ObjSet { get; }

        public readonly BlockMapping blockMapping;
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
                TileSet = new TileSet(cartridge, offsetArt + artBank * 0x4000, palettes[initPalette]);
            }
            else
            {
                TileSet = new TileSet(cartridge, offsetArt + 0x30000, palettes[initPalette]);
            }
            Floor1 = new Floor(cartridge, floorAddress, floorSize);
            blockMapping = new BlockMapping(cartridge, blockMappingAddress, solidityIndex, TileSet);
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

        public Bitmap Render(byte mode, ToolStripProgressBar pb)
        {
            int bs = 32;
            int ts = 8;
            if (mode == 1)
                bs = 33;
            if (mode == 2)
            {
                bs = 36;
                ts = 9;
            }
            Bitmap result = new Bitmap(floorWidth * bs, floorHeight * bs);
            using (Graphics g = Graphics.FromImage(result)) 
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.Clear(Color.White);
                pb.Maximum = floorWidth;
                if (mode == 2)
                {
                    // Tile-wise drawing
                    for (int bx = 0; bx < floorWidth; bx++)
                    {
                        for (int by = 0; by < floorHeight; by++)
                        {
                            var blockIndex = Floor1.data[bx + by * floorWidth];
                            var block = blockMapping.Blocks[blockIndex];
                            for (int ty = 0; ty < 4; ty++)
                            for (int tx = 0; tx < 4; tx++)
                            {
                                var tileIndex = block.TileIndices[tx + ty * 4];
                                var tile = TileSet.Tiles[tileIndex];
                                g.DrawImageUnscaled(tile.Image, bx * bs + tx * ts, by * bs + ty * ts);
                            }
                        }
                        pb.Value = bx;
                    }
                }
                else
                {
                    // Block-wise drawing
                    using (var f = new Font("Courier New", 8, FontStyle.Bold))
                    {
                        for (int bx = 0; bx < floorWidth; bx++)
                        {
                            for (int by = 0; by < floorHeight; by++)
                            {
                                var block = Floor1.data[bx + by * floorWidth];
                                var tileData = blockMapping.Blocks[block];

                                g.DrawImageUnscaled(tileData.Image, bx * bs, by * bs);

                                if (mode == 1)
                                {
                                    // Draw a rect over it for the label
                                    g.FillRectangle(Brushes.White, bx*bs, by*bs, 13, 11);
                                    g.DrawString(block.ToString("X2"), f, Brushes.Black, bx * bs - 2, by * bs - 3);
                                }
                            }

                            pb.Value = bx;
                        }
                    }
                }

                using (var pen = new Pen(Color.Black, 1))
                {
                    foreach (var obj in ObjSet.objs)
                    {
                        var a = obj.x * bs - 1;
                        var b = obj.y * bs - 1;
                        var c = a + bs;
                        var d = b + bs;
                        g.DrawLine(pen, a, b, c, d);
                        g.DrawLine(pen, c, b, a, d);
                        g.DrawLine(pen, a, b, a, d);
                        g.DrawLine(pen, c, b, c, d);
                        g.DrawLine(pen, a, b, c, b);
                        g.DrawLine(pen, a, d, c, d);
                    }
                }
            }

            pb.Value = 0;
            return result;
        }
    }
}
