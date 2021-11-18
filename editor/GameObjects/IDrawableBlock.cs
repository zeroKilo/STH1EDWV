using System.Drawing;

namespace sth1edwv.GameObjects
{
    public interface IDrawableBlock
    {
        public Bitmap GetImage(Palette palette);
        public void ResetImages();
        public int Width { get; }
        public int Height { get; }
    }
}