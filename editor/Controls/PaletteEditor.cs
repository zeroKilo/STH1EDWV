using System;
using System.IO;
using System.Windows.Forms;
using sth1edwv.GameObjects;

namespace sth1edwv.Controls
{
    public partial class PaletteEditor : UserControl
    {
        private readonly Palette _palette;

        public event Action<Palette> Changed;

        public PaletteEditor(Palette palette, string description, Action<Palette> onPaletteChanged)
        {
            _palette = palette;
            InitializeComponent();
            label1.Text = description;
            RefreshImage();

            Changed += onPaletteChanged;

            Disposed += (_, _) =>
            {
                pictureBox1.Image.Dispose();
                Changed -= onPaletteChanged;
            };
        }

        private void RefreshImage()
        {
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = _palette.ToImage(pictureBox1.Width);
        }

        private void saveToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            using var d = new SaveFileDialog
            {
                Filter = "PNG images|*.png|JASC PAL files|*.pal"
            };
            if (d.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            switch (Path.GetExtension(d.FileName.ToLowerInvariant()))
            {
                case ".pal":
                    _palette.SaveAsText(d.FileName);
                    break;
                case ".png":
                    _palette.SaveAsImage(d.FileName);
                    break;
            }
        }

        private void loadToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                using var d = new OpenFileDialog
                {
                    Filter = "PNG images|*.png|JASC PAL files|*.pal"
                };
                if (d.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                switch (Path.GetExtension(d.FileName.ToLowerInvariant()))
                {
                    case ".pal":
                        _palette.LoadFromText(d.FileName);
                        break;
                    case ".png":
                        _palette.LoadFromImage(d.FileName);
                        break;
                    default:
                        throw new Exception("Unsupported extension");
                }

                RefreshImage();
                Changed?.Invoke(_palette);
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, $"Failed to load: {exception.Message}");
            }

        }
    }
}
