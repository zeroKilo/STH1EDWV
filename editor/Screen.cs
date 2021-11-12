using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace sth1edwv
{
    public class Screen
    {
        private Bitmap _image;

        // ReSharper disable MemberCanBePrivate.Global
        public string Name { get; }

        public TileSet TileSet { get; }

        public Palette Palette { get; }

        public List<ushort> TileMap { get; }
        // ReSharper restore MemberCanBePrivate.Global

        public Screen(Cartridge cartridge, string name, int tileSetReferenceOffset,
            int tileSetBankOffset, int paletteReferenceOffset, int tileMapReferenceOffset, int tileMapSizeOffset,
            int tileMapBankOffset, int secondaryTileMapReferenceOffset, int secondaryTileMapSizeOffset)
        {
            Name = name;
            var paletteOffset = cartridge.Memory.Word(paletteReferenceOffset);
            Palette = cartridge.GetPalette(paletteOffset, 1);
            // Tile set reference is relative to the start of its bank
            var tileSetOffset = cartridge.Memory.Word(tileSetReferenceOffset) + cartridge.Memory[tileSetBankOffset] * 0x4000;
            TileSet = cartridge.GetTileSet(tileSetOffset, false);
            // Tile map offset is as pages in slot 1 (TODO always?)
            var tileMapOffset = cartridge.Memory.Word(tileMapReferenceOffset) + cartridge.Memory[tileMapBankOffset] * 0x4000 - 0x4000;
            var tileMapSize = cartridge.Memory.Word(tileMapSizeOffset);
            TileMap = Compression.DecompressRle(cartridge, tileMapOffset, tileMapSize)
                .Select(b => (ushort)b)
                .ToList();
            if (secondaryTileMapReferenceOffset != 0)
            {
                tileMapOffset = cartridge.Memory.Word(secondaryTileMapReferenceOffset) + cartridge.Memory[tileMapBankOffset] * 0x4000 - 0x4000;
                tileMapSize = cartridge.Memory.Word(secondaryTileMapSizeOffset);
                var secondaryTileMap = Compression.DecompressRle(cartridge, tileMapOffset, tileMapSize);

                // Apply to the tilemap
                for (var i = 0; i < secondaryTileMap.Length; i++)
                {
                    var index = secondaryTileMap[i];
                    if (index == 0xff)
                    {
                        //  Assume the primary one is therefore foreground
                        TileMap[i] |= 0x1000;
                    }
                    else
                    {
                        TileMap[i] = secondaryTileMap[i];
                    }
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public Bitmap Image
        {
            get
            {
                if (_image != null)
                {
                    return _image;
                }
                _image = new Bitmap(256, 192);
                using var g = Graphics.FromImage(_image);
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                for (var i = 0; i < TileMap.Count; ++i)
                {
                    var x = i % 32 * 8;
                    var y = i / 32 * 8;
                    var index = TileMap[i];
                    if (index != 0xff)
                    {
                        g.DrawImageUnscaled(TileSet.Tiles[index].GetImage(Palette), x, y);
                    }
                }

                return _image;
            }
        }
    }
}