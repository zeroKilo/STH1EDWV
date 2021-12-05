using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace sth1edwv.Controls
{
    public partial class SpaceVisualizer : UserControl
    {
        private FreeSpace _space;

        public SpaceVisualizer()
        {
            InitializeComponent();
        }

        public void SetData(FreeSpace space)
        {
            _space = space;
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            Invalidate();
            base.OnSizeChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_space == null)
            {
                base.OnPaint(e);
                return;
            }
            
            const int padding = 4;
            const int banksGap = 1;

            // Clear the background to the "control" colour
            e.Graphics.Clear(SystemColors.Control);

            // Add a border
            e.Graphics.DrawRectangle(SystemPens.ControlDark, new Rectangle(padding, padding, Width - padding * 2 - 1, Height - padding * 2 - 1));

            // The pixel width of the area we want to fill
            var totalWidth = Width - padding * 2 - 2;
            var topLeft = padding + 1;
            var height = Height - padding * 2 - 2;
            var banks = _space.Maximum / (16 * 1024);
            var pixelsPerByte = (float)(totalWidth - (banks - 1) * banksGap) / _space.Maximum;

            int BankStart(int bank)
            {
                return topLeft + banksGap * bank + (int)(bank * 0x4000 * pixelsPerByte);
            }
            int OffsetToPixel(int offset)
            {
                // We have bank-1 gaps to the left
                var bank = offset / 0x4000;
                var bankLeft = BankStart(bank);
                var bankWidth = BankStart(bank + 1) - bankLeft - banksGap;
                var offsetInBank = offset % 0x4000;
                return bankLeft + offsetInBank * bankWidth / 0x4000;
            }

            // Draw in the whole lot as "full"
            e.Graphics.FillRectangle(SystemBrushes.Highlight, topLeft, topLeft, totalWidth, height);

            // Draw free space over it
            foreach (var span in _space.Spans)
            {
                var x = OffsetToPixel(span.Start);
                var width = OffsetToPixel(span.End - 1) - x + 1;
                e.Graphics.FillRectangle(SystemBrushes.Control, x, topLeft, width, height);
            }

            // Draw in banks boundaries
            for (var bank = 0; bank < banks; ++bank)
            {
                var x = OffsetToPixel(0x4000 * bank);
                e.Graphics.FillRectangle(SystemBrushes.ControlDark, x-banksGap, topLeft, banksGap, height);
            }

            // Draw some text on the top
            var freeBytes = _space.Spans.Sum(x => x.Size);
            e.Graphics.DrawString($"{freeBytes} bytes ({freeBytes/1024.0:F1} KB) free of {_space.Maximum / 1024} KB", Font, SystemBrushes.ControlText, new RectangleF(padding, 0, totalWidth, Height), new StringFormat {Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center});
        }
    }
}
