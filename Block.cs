using System;
using System.Collections.Generic;
using System.Drawing;

namespace sth1edwv
{
    public class Block: IDisposable, IDataItem, IDrawableBlock
    {
        public TileSet TileSet { get; }
        private Bitmap _image;

        public byte[] TileIndices { get; } = new byte[16];
        public Bitmap Image {
            get
            {
                if (_image != null)
                {
                    return _image;
                }
                // Lazy rendering
                _image = new Bitmap(32, 32);
                using (var g = Graphics.FromImage(_image))
                {
                    for (var i = 0; i < 16; ++i)
                    {
                        var x = i % 4 * 8;
                        var y = i / 4 * 8;
                        var tileIndex = TileIndices[i];
                        var tile = TileSet.Tiles[tileIndex];
                        g.DrawImageUnscaled(tile.Image, x, y);
                    }
                }
                return _image;
            }
        }

        public int Index { get; }
        public int SolidityOffset { get; }

        public int SolidityIndex { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public bool IsForeground { get; set; }

        public byte Data => (byte)(SolidityIndex | (IsForeground ? 0x80 : 0));

        public Block(byte[] cartridgeMemory, int tilesOffset, int solidityOffset, TileSet tileSet, int index)
        {
            TileSet = tileSet;
            Offset = tilesOffset;
            LengthConsumed = 16;
            SolidityOffset = solidityOffset;
            Index = index;
            Array.Copy(cartridgeMemory, tilesOffset, TileIndices, 0, 16);
            var solidityData = cartridgeMemory[solidityOffset];
            SolidityIndex = solidityData & 0x3f;

            IsForeground = (solidityData & 0b10000000) != 0;
        }

        public void Dispose()
        {
            _image?.Dispose();
        }

        public int Offset { get; }
        public int LengthConsumed { get; }
        public int UsageCount { get; set; }
        public int GlobalUsageCount { get; set; }

        public IList<byte> GetData()
        {
            return TileIndices;
        }

        public void ResetImage()
        {
            _image?.Dispose();
            _image = null;
        }
    }
}