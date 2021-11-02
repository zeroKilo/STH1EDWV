﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace sth1edwv
{
    public class Level: IDataItem
    {
        //   SP FW FW FH  FH LX LX ??  LW LY LY XH  LH SX SY FL 
        //   FL FS FS BM  BM LA LA 09  SA SA IP CS  CC CP OL OL 
        //   SR UW TL 00  MU
        private byte[] _header;
        private readonly byte   _solidityIndex;
        private readonly ushort _floorWidth;
        private readonly ushort _floorHeight;
        private readonly int   _floorAddress;
        private readonly ushort _floorSize;
        private readonly int   _blockMappingAddress;
        private readonly ushort _levelXOffset;
        private readonly byte   _levelWidth;
        private readonly ushort _levelYOffset;
        private readonly byte   _levelExtHeight;
        private readonly byte   _levelHeight;
        private readonly ushort _offsetArt;
        private readonly ushort _offsetObjectLayout;
        private readonly byte   _initPalette;
        public TileSet TileSet { get; }
        public Floor Floor { get; }
        private LevelObjectSet Objects { get; }

        public BlockMapping BlockMapping { get; }
        private readonly string _label;
        private int _blockSize;

        public Level(Cartridge cartridge, int offset, int artBanksTableOffset, IList<Palette> palettes, string label)
        {
            _label = label;
            Offset = offset;
            LengthConsumed = 37;
            _header = new byte[37];
            Array.Copy(cartridge.Memory, offset, _header, 0, _header.Length);


            _solidityIndex = _header[0];
            _floorWidth = BitConverter.ToUInt16(_header, 1);
            _floorHeight = BitConverter.ToUInt16(_header, 3);
            if (Offset == 0x15580 + 666)
            {
                // Scrap Brain 2 (BallHog area) has only enough data to fill 2KB 
                _floorHeight /= 2;
            }

            _floorAddress = BitConverter.ToUInt16(_header, 15) + 0x14000;
            _floorSize = BitConverter.ToUInt16(_header, 17);
            _blockMappingAddress = BitConverter.ToUInt16(_header, 19) + 0x10000;
            _levelXOffset = BitConverter.ToUInt16(_header, 5);
            _levelWidth = _header[8];
            _levelYOffset = BitConverter.ToUInt16(_header, 9);
            _levelExtHeight = _header[11];
            _levelHeight = _header[12];
            _offsetArt = BitConverter.ToUInt16(_header, 21);
            _offsetObjectLayout = BitConverter.ToUInt16(_header, 30);
            _initPalette = _header[26];
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
            Floor = cartridge.GetFloor(_floorAddress, _floorSize, _floorWidth);
            BlockMapping = cartridge.GetBlockMapping(_blockMappingAddress, _solidityIndex, TileSet);
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
                    new TreeNode($"Floor Data           = @0x{_floorAddress:X} Size: 0x{_floorSize:X} ({(double)(uncompressedSize-_floorSize)/uncompressedSize:P})"),
                    new TreeNode($"Level Size           = ({_levelWidth} x {_levelHeight})"),
                    new TreeNode($"Level Offset         = (dx:{_levelXOffset} dy:{_levelYOffset})"),
                    new TreeNode($"Extended Height      = {_levelExtHeight}"),
                    new TreeNode($"Offset Art           = 0x{_offsetArt:X8}"),
                    new TreeNode($"Offset Object Layout = 0x{_offsetObjectLayout:X8}"),
                    new TreeNode($"Initial Palette      = {_initPalette}"),
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

            const int labelWidth = 13;
            const int labelHeight = 9;

            var result = new Bitmap(_floorWidth * _blockSize, _floorHeight * _blockSize);
            using var f = new Font("Consolas", 8, FontStyle.Bold);
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
                    g.DrawIcon(SystemIcons.Error, new Rectangle(blockX*_blockSize, blockY*_blockSize, 32, 32));
                }

                if (blockLabels)
                {
                    g.DrawString(blockIndex.ToString("X2"), f, Brushes.Black, blockX * _blockSize, blockY * _blockSize - 1);
                    g.DrawString(blockIndex.ToString("X2"), f, Brushes.White, blockX * _blockSize - 1, blockY * _blockSize - 2);
                }
            }

            if (withObjects)
            {
                var image = Properties.Resources.package;
                // Draw objects
                foreach (var levelObject in Objects)
                {
                    var x = levelObject.X * _blockSize;
                    var y = levelObject.Y * _blockSize;
                    g.DrawRectangle(Pens.Blue, x, y, _blockSize, _blockSize);
                    g.DrawImageUnscaled(image, x + _blockSize / 2 - image.Width / 2, y + _blockSize / 2 - image.Height / 2);

                    x += _blockSize - labelWidth;
                    y += _blockSize - labelHeight;
                    g.FillRectangle(Brushes.Blue, x, y, labelWidth, labelHeight);
                    g.DrawString(levelObject.Type.ToString("X2"), f, Brushes.White, x - 1, y - 2);
                }
            }

            if (levelBounds)
            {
                var x = _levelXOffset + _levelXOffset / 32 * (_blockSize - 32);
                var y = _levelYOffset + _levelYOffset / 32 * (_blockSize - 32);
                var w = (_levelWidth * 8 + 14) * _blockSize;
                var h = (_levelHeight * 8 + 6) * _blockSize + _levelExtHeight;
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
