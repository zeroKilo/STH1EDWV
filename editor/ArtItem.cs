using sth1edwv.GameObjects;

namespace sth1edwv
{
    public class ArtItem
    {
        // TODO: maybe add screens in here too?
        // Add info on reference locations?
        public TileSet TileSet { get; set; }
        public Palette Palette { get; set; }
        public bool PaletteEditable { get; set; }
        public string Name { get; set; }
        public int Width { get; set; }
        public bool IsSprites { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}