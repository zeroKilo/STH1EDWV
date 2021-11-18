using System;
using System.Collections.Generic;
using System.Drawing;

namespace sth1edwv.GameObjects
{
    // Wrapper class for use in the grid
    public class BlockRow
    {
        private readonly Palette _palette;

        public BlockRow(Block block, Palette palette)
        {
            Block = block;
            _palette = palette;
        }

        // ReSharper disable UnusedMember.Global
        public Image Image => Block.GetImage(_palette);

        public int Index => Block.Index;

        public bool IsForeground
        {
            get => Block.IsForeground;
            set => Block.IsForeground = value;
        }

        public int SolidityIndex
        {
            get => Block.SolidityIndex;
            set => Block.SolidityIndex = value;
        }

        public int UsageCount => Block.UsageCount;
        public int GlobalUsageCount => Block.GlobalUsageCount;
        // ReSharper restore UnusedMember.Global

        public Block Block { get; }
    }

    public class Block: IDisposable, IDataItem, IDrawableBlock
    {
        public TileSet TileSet { get; }
        private readonly Dictionary<Palette, Bitmap> _images = new();

        public byte[] TileIndices { get; } = new byte[16];

        public Bitmap GetImage(Palette palette)
        {
            if (_images.TryGetValue(palette, out var image))
            {
                return image;
            }

            // Lazy rendering
            image = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(image))
            {
                for (var i = 0; i < 16; ++i)
                {
                    var x = i % 4 * 8;
                    var y = i / 4 * 8;
                    var tileIndex = TileIndices[i];
                    var tile = TileSet.Tiles[tileIndex];
                    g.DrawImageUnscaled(tile.GetImage(palette), x, y);
                }
            }

            _images.Add(palette, image);
            return image;
        }

        public int Width => 32;
        public int Height => 32;
        
        public int Index { get; }

        public int SolidityIndex { get; set; }
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        public bool IsForeground { get; set; }

        public byte Data => (byte)(SolidityIndex | (IsForeground ? 0x80 : 0));

        public Block(IReadOnlyList<byte> cartridgeMemory, int tilesOffset, int solidityOffset, TileSet tileSet, int index)
        {
            TileSet = tileSet;
            Offset = tilesOffset;
            SolidityOffset = solidityOffset;
            Index = index;
            for (var i = 0; i < 16; ++i)
            {
                TileIndices[i] = cartridgeMemory[tilesOffset + i];
            }
            var solidityData = cartridgeMemory[solidityOffset];
            SolidityIndex = solidityData & 0b00111111;
            IsForeground = (solidityData & 0b10000000) != 0;
        }

        public void Dispose()
        {
            ResetImages();
        }

        public int Offset { get; }
        public int UsageCount { get; set; }
        public int GlobalUsageCount { get; set; }

        public int SolidityOffset { get; }

        public IList<byte> GetData()
        {
            return TileIndices;
        }

        public void ResetImages()
        {
            foreach (var bitmap in _images.Values)
            {
                bitmap.Dispose();
            }
            _images.Clear();
        }
    }
}