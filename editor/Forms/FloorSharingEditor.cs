using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using sth1edwv.GameObjects;

namespace sth1edwv.Forms
{
    public sealed partial class FloorSharingEditor : Form
    {
        private readonly Level _level;
        private readonly Cartridge _cartridge;

        private class ComboItem
        {
            public Floor Floor { get; set; }
            public string Label { get; set; }
            public override string ToString()
            {
                return Label;
            }
        }

        public FloorSharingEditor(Level level, Cartridge cartridge)
        {
            _level = level;
            _cartridge = cartridge;
            InitializeComponent();
            Font = SystemFonts.MessageBoxFont;

            sharedLevelsList.Items.AddRange(cartridge.Levels.Where(x => x.Floor == level.Floor).ToArray<object>());

            var comboItems = cartridge.Levels
                .GroupBy(l => l.Floor)
                .Select(g => new ComboItem{Floor = g.Key, Label = string.Join(", ", g.Select(l => l.ToString()))})
                .ToList();
            uniqueFloorsCombo.Items.AddRange(comboItems.ToArray<object>());
            uniqueFloorsCombo.SelectedItem = comboItems.First(x => x.Floor == level.Floor);
        }

        private void ShareWithOtherButton_Click(object sender, System.EventArgs e)
        {
            // We change the level to point at the selected floor...
            if (uniqueFloorsCombo.SelectedItem is not ComboItem selected || selected.Floor == _level.Floor)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            // We update the floor dimensions to match other levels using it
            var otherLevel = _cartridge.Levels.FirstOrDefault(x => x.Floor == selected.Floor);
            _level.Floor = selected.Floor;
            if (otherLevel != null)
            {
                _level.FloorHeight = otherLevel.FloorHeight;
                _level.FloorWidth = otherLevel.FloorWidth;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void UnshareButton_Click(object sender, System.EventArgs e)
        {
            // We clone the floor...
            _level.Floor = _level.Floor.Clone();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
