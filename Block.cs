using System;
using System.Drawing;
using System.Linq;

namespace sth1edwv
{
    public class Block
    {
        private readonly TileSet _tileSet;
        private Bitmap _image;
        public byte[] TileIndices { get; } = new byte[16];
        public byte SolidityIndex { get; }
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
                    for (int i = 0; i < 16; ++i)
                    {
                        var x = i % 4 * 8;
                        var y = i / 4 * 8;
                        var tileIndex = TileIndices[i];
                        var tile = _tileSet.Tiles[tileIndex];
                        g.DrawImageUnscaled(tile.Image, x, y);
                    }
                }
                return _image;
            }
        }

        public Block(byte[] cartridgeMemory, int tilesOffset, int solidityOffset, TileSet tileSet)
        {
            _tileSet = tileSet;
            Array.Copy(cartridgeMemory, tilesOffset, TileIndices, 0, 16);
            SolidityIndex = cartridgeMemory[solidityOffset];
        }

        public override string ToString()
        {
            return string.Join("", TileIndices.Select(x => x.ToString("X2"))) + " - " + Convert.ToString(SolidityIndex, 2).PadLeft(8, '0');
        }
    }
}