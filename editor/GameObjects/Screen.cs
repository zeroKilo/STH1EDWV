using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace sth1edwv.GameObjects
{
    public class Screen
    {
        private readonly Cartridge.Game.ArtInfo _screenInfo;

        // ReSharper disable MemberCanBePrivate.Global
        public string Name { get; }

        public TileSet TileSet { get; }

        public Palette Palette { get; }

        public TileMap TileMap { get; }
        // ReSharper restore MemberCanBePrivate.Global

        public Screen(Cartridge cartridge, Cartridge.Game.ArtInfo screenInfo)
        {
            _screenInfo = screenInfo;
            Name = screenInfo.Name;
            var paletteOffset = cartridge.Memory.Word(screenInfo.PaletteReferenceOffset);
            Palette = cartridge.GetPalette(paletteOffset, 1);
            // Tile set reference is relative to the start of its bank
            var tileSetOffset = cartridge.Memory.Word(screenInfo.TileSetReferenceOffset) + cartridge.Memory[screenInfo.TileSetBankOffset] * 0x4000;
            TileSet = cartridge.GetTileSet(tileSetOffset, null);
            // Tile map offset is as pages in slot 1
            var tileMapOffset = cartridge.Memory.Word(screenInfo.TileMapReferenceOffset) + cartridge.Memory[screenInfo.TileMapBankOffset] * 0x4000 - 0x4000;
            var tileMapSize = cartridge.Memory.Word(screenInfo.TileMapSizeOffset);
            TileMap = new TileMap(cartridge.Memory, tileMapOffset, tileMapSize);
            TileMapOffset = $"{tileMapOffset:X}-{tileMapOffset + tileMapSize - 1:X}";
            if (screenInfo.SecondaryTileMapReferenceOffset != 0)
            {
                TileMap.SetAllForeground();
                tileMapOffset = cartridge.Memory.Word(screenInfo.SecondaryTileMapReferenceOffset) + cartridge.Memory[screenInfo.TileMapBankOffset] * 0x4000 - 0x4000;
                tileMapSize = cartridge.Memory.Word(screenInfo.SecondaryTileMapSizeOffset);
                var background = new TileMap(cartridge.Memory, tileMapOffset, tileMapSize);
                TileMap.Overlay(background);
                SecondaryTileMapOffset = $"{tileMapOffset:X}-{tileMapOffset + tileMapSize - 1:X}";
            }
        }

        // These are for display only
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string TileMapOffset { get; }
        public string SecondaryTileMapOffset { get; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global

        public override string ToString()
        {
            return Name;
        }

        public void FixPointers(byte[] memory)
        {
            var value = 0x4000 + TileMap.Offset % 0x4000;
            var bank = TileMap.Offset / 0x4000;
            var length = TileMap.TileMap1Size;
            memory[_screenInfo.TileMapReferenceOffset + 0] = (byte)(value & 0xff);
            memory[_screenInfo.TileMapReferenceOffset + 1] = (byte)(value >> 8);
            memory[_screenInfo.TileMapBankOffset] = (byte)bank;
            memory[_screenInfo.TileMapSizeOffset + 0] = (byte)(length & 0xff);
            memory[_screenInfo.TileMapSizeOffset + 1] = (byte)(length >> 8);
            if (TileMap.TileMap2Size > 0)
            {
                value = 0x4000 + (TileMap.Offset + TileMap.TileMap1Size) % 0x4000;
                length = TileMap.TileMap2Size;
                memory[_screenInfo.SecondaryTileMapReferenceOffset + 0] = (byte)(value & 0xff);
                memory[_screenInfo.SecondaryTileMapReferenceOffset + 1] = (byte)(value >> 8);
                memory[_screenInfo.SecondaryTileMapSizeOffset + 0] = (byte)(length & 0xff);
                memory[_screenInfo.SecondaryTileMapSizeOffset + 1] = (byte)(length >> 8);
            }
        }
    }
}