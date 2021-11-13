using System;
using System.Drawing;
using System.Windows.Forms;

namespace sth1edwv
{
    public sealed partial class FloorSizeForm : Form
    {
        private readonly Level _level;

        public FloorSizeForm()
        {
            // For the designer
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;
        }

        public FloorSizeForm(Level level)
        {
            _level = level;
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;
            ValueChanged(null, EventArgs.Empty);
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            if (_level == null)
            {
                return;
            }

            Result = new Padding((int)LeftDelta.Value, (int)TopDelta.Value, (int)RightDelta.Value, (int)BottomDelta.Value);

            // Compute the result
            var newWidth = _level.FloorWidth + Result.Horizontal;
            var newHeight = _level.FloorHeight + Result.Vertical;

            // Set label
            finalSizeLabel.Text = $"Final map size: {newWidth} x {newHeight}";

            okButton.Enabled = true;

            if (newHeight < 192 / 32)
            {
                okButton.Enabled = false;
                finalSizeLabel.Text += " (too short)";
            }

            if (newWidth * newHeight > 4096)
            {
                okButton.Enabled = false;
                finalSizeLabel.Text += " (too large)";
            }

            // Width can only be 256, 128, 64, 32, 16
            switch (newWidth)
            {
                case 256:
                case 128:
                case 64:
                case 32:
                case 16:
                    break;
                default:
                    okButton.Enabled = false;
                    finalSizeLabel.Text += " (width must be 16, 32, 64, 128 or 256)";
                    break;
            }
        }

        public Padding Result { get; private set; }

        private void ButtonClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
