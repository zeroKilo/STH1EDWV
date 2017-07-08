using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sth1edwv
{
    public class Level
    {
        //   SP FW FW FH  FH LX LX ??  LW LY LY XH  LH SX SY FL 
        //   FL FS FS BM  BM LA LA 09  SA SA IP CS  CC CP OL OL 
        //   SR UW TL 00  MU
        public byte[] header;
        public byte   solidityIndex;
        public uint   address;
        public ushort floorWidth;
        public ushort floorHeight;
        public uint   floorAddress;
        public ushort floorSize;
        public uint   blockMappingAddress;
        public ushort levelXOffset;
        public byte   levelWidth;
        public ushort levelYOffset;
        public byte   levelExtHeight;
        public byte   levelHeight;
        public ushort offsetArt;
        public ushort offsetObjectLayout;
        public byte   initPalette;
        public List<Color> palette;
        public TileSet tileset;
        public Floor floor;
        public LevelObjectSet objSet;
        
        public BlockMapping blockMapping;

        public Level(uint offset)
        {
            address = BitConverter.ToUInt16(Cartridge.memory, (int)offset);
            header = new byte[37];
            for (int i = 0; i < 37; i++)
                header[i] = Cartridge.memory[address + i + 0x15580];
            solidityIndex = header[0];
            floorWidth = BitConverter.ToUInt16(header, 1);
            floorHeight = BitConverter.ToUInt16(header, 3);
            if (address == 666)
                floorHeight /= 2;
            floorAddress = BitConverter.ToUInt16(header, 15) + 0x14000u;
            floorSize = BitConverter.ToUInt16(header, 17);
            blockMappingAddress = BitConverter.ToUInt16(header, 19) + 0x10000u;
            levelXOffset = BitConverter.ToUInt16(header, 5);
            levelWidth = header[8];
            levelYOffset = BitConverter.ToUInt16(header, 9);
            levelExtHeight = header[11];
            levelHeight = header[12];
            offsetArt = BitConverter.ToUInt16(header, 21);
            offsetObjectLayout = BitConverter.ToUInt16(header, 30);
            initPalette = header[26];
            Palettes.ReadPallettes(Cartridge.memory);
            tileset = new TileSet(offsetArt, Palettes.palettes[initPalette]);
            floor = new Floor(floorAddress, floorSize);
            blockMapping = new BlockMapping(blockMappingAddress, solidityIndex, tileset);
            objSet = new LevelObjectSet(0x15580 + offsetObjectLayout);
        }

        public TreeNode ToNode()
        {
            TreeNode result = new TreeNode("Header");
            result.Nodes.Add("Floor Size           = (" + floorWidth + " x " + floorHeight + ")");
            result.Nodes.Add("Floor Data           = (@0x" + floorAddress.ToString("X") + " Size: 0x" + floorSize.ToString("X") + ")");
            result.Nodes.Add("Level Size           = (" + levelWidth + " x " + levelHeight + ")");
            result.Nodes.Add("Level Offset         = (dx:" + levelXOffset + " dy:" + levelYOffset + ")");
            result.Nodes.Add("Extended Height      = " + levelExtHeight);
            result.Nodes.Add("Offset Art           = 0x" + offsetArt.ToString("X8"));
            result.Nodes.Add("Offset Object Layout = 0x" + offsetObjectLayout.ToString("X8"));
            result.Nodes.Add("Initial Palette      = " + initPalette);
            result.Nodes.Add(objSet.ToNode());
            result.Expand();
            return result;
        }

        public Color[,] getTile(int index)
        {            
            return tileset.tiles[index];
        }

        public Bitmap Render(byte mode, ref ToolStripProgressBar pb)
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
            Graphics g = Graphics.FromImage(result);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.Clear(Color.White);
            byte block, tile;
            byte[] blockData;
            Color[,] tileData;
            Font f = new Font("Courier New", 8, FontStyle.Bold);
            if (pb != null)
                pb.Maximum = floorWidth;
            if (mode == 2)
                for (int bx = 0; bx < floorWidth; bx++)
                {
                    for (int by = 0; by < floorHeight; by++)
                    {
                        block = floor.data[bx + by * floorWidth];
                        blockData = blockMapping.blocks[block];
                        for (int ty = 0; ty < 4; ty++)
                            for (int tx = 0; tx < 4; tx++)
                            {
                                tile = blockData[tx + ty * 4];
                                tileData = tileset.tiles[tile];
                                for (int y = 0; y < 8; y++)
                                    for (int x = 0; x < 8; x++)
                                        result.SetPixel(bx * bs + tx * ts + x, by * bs + ty * ts + y, tileData[x, y]);
                            }
                    }
                    pb.Value = bx;
                }
            else
                for (int bx = 0; bx < floorWidth; bx++)
                {
                    for (int by = 0; by < floorHeight; by++)
                    {
                        block = floor.data[bx + by * floorWidth];
                        tileData = blockMapping.imagedata[block];
                        if (mode != 1)
                            for (int y = 0; y < 32; y++)
                                for (int x = 0; x < 32; x++)
                                    result.SetPixel(bx * bs + x, by * bs + y, tileData[x, y]);
                        if (mode == 1)
                        {
                            for (int y = 0; y < 32; y++)
                                for (int x = 0; x < 32; x++)
                                    if (y > 10 || x > 12)//white background keep free
                                        result.SetPixel(bx * bs + x, by * bs + y, tileData[x, y]);
                            g.DrawString(block.ToString("X2"), f, Brushes.Black, bx * bs - 2, by * bs - 3);
                        }
                    }
                    pb.Value = bx;
                }
            Pen pen = new Pen(Color.Black, 2);
            pen.Width = 1;
            g.SmoothingMode = SmoothingMode.None;
            g.PixelOffsetMode = PixelOffsetMode.None;
            g.InterpolationMode = InterpolationMode.Default;
            foreach (LevelObjectSet.LevelObject obj in objSet.objs)
            {
                int a, b, c, d;
                a = obj.X * bs - 1;
                b = obj.Y * bs - 1;
                c = a + bs;
                d = b + bs;
                g.DrawLine(pen, a, b, c, d);
                g.DrawLine(pen, c, b, a, d);
                g.DrawLine(pen, a, b, a, d);
                g.DrawLine(pen, c, b, c, d);
                g.DrawLine(pen, a, b, c, b);
                g.DrawLine(pen, a, d, c, d);
            }
            pb.Value = 0;
            return result;
        }
    }
}
