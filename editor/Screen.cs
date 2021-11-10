using System.Drawing;
using System.Drawing.Drawing2D;

namespace sth1edwv
{
    public class Screen
    {
        private Bitmap _image;

        public string Name { get; }

        public TileSet TileSet { get; }

        public Palette Palette { get; }

        public byte[] Tilemap { get; }

        public Screen(Cartridge cartridge, string name, int tileSetReferenceOffset,
            int tileSetBankOffset, int paletteReferenceOffset, int tileMapReferenceOffset, int tileMapSizeOffset,
            int tileMapBankOffset)
        {
            Name = name;
            var paletteOffset = cartridge.Memory.Word(paletteReferenceOffset);
            Palette = cartridge.GetPalette(paletteOffset, 1);
            // Tile set reference is relative to the start of its bank
            var tileSetOffset = cartridge.Memory.Word(tileSetReferenceOffset) + cartridge.Memory[tileSetBankOffset] * 0x4000;
            TileSet = cartridge.GetTileSet(tileSetOffset, Palette, false);
            // Tile map offset is as pages in slot 1 (TODO always?)
            var tileMapOffset = cartridge.Memory.Word(tileMapReferenceOffset) + cartridge.Memory[tileMapBankOffset] * 0x4000 - 0x4000;
            var tileMapSize = cartridge.Memory.Word(tileMapSizeOffset);
            Tilemap = Compression.DecompressRle(cartridge, tileMapOffset, tileMapSize);
        }

        public override string ToString()
        {
            return Name;
        }

        public Bitmap Image
        {
            get
            {
                if (_image == null)
                {
                    _image = new Bitmap(256, 192);
                    using var g = Graphics.FromImage(_image);
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    for (var i = 0; i < Tilemap.Length; ++i)
                    {
                        var x = i % 32 * 8;
                        var y = i / 32 * 8;
                        var index = Tilemap[i];
                        if (index != 0xff)
                        {
                            g.DrawImageUnscaled(TileSet.Tiles[index].Image, x, y);
                        }
                    }
                }

                return _image;
            }
        }
    }
}