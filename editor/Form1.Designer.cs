using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using sth1edwv.Properties;

namespace sth1edwv
{
    sealed partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            this.menuStrip1 = new MenuStrip();
            this.fileToolStripMenuItem = new ToolStripMenuItem();
            this.openROMToolStripMenuItem = new ToolStripMenuItem();
            this.saveROMToolStripMenuItem = new ToolStripMenuItem();
            this.quickTestToolStripMenuItem = new ToolStripMenuItem();
            this.tabPage5 = new TabPage();
            this.splitContainer6 = new SplitContainer();
            this.listBoxLevels = new ListBox();
            this.tabControlLevel = new TabControl();
            this.tabPage6 = new TabPage();
            this.propertyGridLevel = new PropertyGrid();
            this.treeViewLevelData = new TreeView();
            this.tabPagePalettes = new TabPage();
            this.tabPage7 = new TabPage();
            this.splitContainer2 = new SplitContainer();
            this.tilePicker1 = new ItemPicker();
            this.splitContainer5 = new SplitContainer();
            this.pictureBoxTilePreview = new PictureBox();
            this.panel2 = new Panel();
            this.pictureBoxTileUsedIn = new PictureBox();
            this.label1 = new Label();
            this.toolStrip2 = new ToolStrip();
            this.buttonSaveTileset = new ToolStripButton();
            this.buttonLoadTileset = new ToolStripButton();
            this.buttonBlankUnusedTiles = new ToolStripButton();
            this.tabPage3 = new TabPage();
            this.splitContainer4 = new SplitContainer();
            this.dataGridViewBlocks = new DataGridView();
            this.Image = new DataGridViewImageColumn();
            this.Index = new DataGridViewTextBoxColumn();
            this.Solidity = new DataGridViewComboBoxColumn();
            this.Foreground = new DataGridViewCheckBoxColumn();
            this.Used = new DataGridViewTextBoxColumn();
            this.UsedGlobal = new DataGridViewTextBoxColumn();
            this.pictureBoxBlockEditor = new PictureBox();
            this.tabPageLayout = new TabPage();
            this.panel1 = new Panel();
            this.pictureBoxRenderedLevel = new PictureBox();
            this.toolStrip1 = new ToolStrip();
            this.buttonShowObjects = new ToolStripButton();
            this.buttonBlockNumbers = new ToolStripButton();
            this.buttonBlockGaps = new ToolStripButton();
            this.buttonTileGaps = new ToolStripButton();
            this.buttonLevelBounds = new ToolStripButton();
            this.toolStripSeparator1 = new ToolStripSeparator();
            this.toolStripButtonSaveRenderedLevel = new ToolStripButton();
            this.buttonCopyFloor = new ToolStripButton();
            this.buttonPasteFloor = new ToolStripButton();
            this.tabPage2 = new TabPage();
            this.splitContainer3 = new SplitContainer();
            this.listBoxPalettes = new ListBox();
            this.pictureBoxPalette = new PictureBox();
            this.tabPage9 = new TabPage();
            this.listBoxGameText = new ListBox();
            this.tabControl1 = new TabControl();
            this.statusStrip1 = new StatusStrip();
            this.floorStatus = new ToolStripStatusLabel();
            this.tileSetStatus = new ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((ISupportInitialize)(this.splitContainer6)).BeginInit();
            this.splitContainer6.Panel1.SuspendLayout();
            this.splitContainer6.Panel2.SuspendLayout();
            this.splitContainer6.SuspendLayout();
            this.tabControlLevel.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            ((ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            ((ISupportInitialize)(this.pictureBoxTilePreview)).BeginInit();
            this.panel2.SuspendLayout();
            ((ISupportInitialize)(this.pictureBoxTileUsedIn)).BeginInit();
            this.toolStrip2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((ISupportInitialize)(this.dataGridViewBlocks)).BeginInit();
            ((ISupportInitialize)(this.pictureBoxBlockEditor)).BeginInit();
            this.tabPageLayout.SuspendLayout();
            this.panel1.SuspendLayout();
            ((ISupportInitialize)(this.pictureBoxRenderedLevel)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((ISupportInitialize)(this.pictureBoxPalette)).BeginInit();
            this.tabPage9.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new Size(24, 24);
            this.menuStrip1.Items.AddRange(new ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new Size(962, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            this.openROMToolStripMenuItem,
            this.saveROMToolStripMenuItem,
            this.quickTestToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openROMToolStripMenuItem
            // 
            this.openROMToolStripMenuItem.Name = "openROMToolStripMenuItem";
            this.openROMToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.O)));
            this.openROMToolStripMenuItem.Size = new Size(185, 22);
            this.openROMToolStripMenuItem.Text = "Open ROM...";
            this.openROMToolStripMenuItem.Click += new EventHandler(this.openROMToolStripMenuItem_Click);
            // 
            // saveROMToolStripMenuItem
            // 
            this.saveROMToolStripMenuItem.Name = "saveROMToolStripMenuItem";
            this.saveROMToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.S)));
            this.saveROMToolStripMenuItem.Size = new Size(185, 22);
            this.saveROMToolStripMenuItem.Text = "Save as...";
            this.saveROMToolStripMenuItem.Click += new EventHandler(this.saveROMToolStripMenuItem_Click);
            // 
            // quickTestToolStripMenuItem
            // 
            this.quickTestToolStripMenuItem.Name = "quickTestToolStripMenuItem";
            this.quickTestToolStripMenuItem.ShortcutKeys = ((Keys)((Keys.Control | Keys.Q)));
            this.quickTestToolStripMenuItem.Size = new Size(185, 22);
            this.quickTestToolStripMenuItem.Text = "Quick test";
            this.quickTestToolStripMenuItem.Click += new EventHandler(this.quickTestToolStripMenuItem_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.splitContainer6);
            this.tabPage5.Location = new Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new Padding(3);
            this.tabPage5.Size = new Size(954, 537);
            this.tabPage5.TabIndex = 5;
            this.tabPage5.Text = "Levels";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // splitContainer6
            // 
            this.splitContainer6.Dock = DockStyle.Fill;
            this.splitContainer6.Location = new Point(3, 3);
            this.splitContainer6.Name = "splitContainer6";
            // 
            // splitContainer6.Panel1
            // 
            this.splitContainer6.Panel1.Controls.Add(this.listBoxLevels);
            // 
            // splitContainer6.Panel2
            // 
            this.splitContainer6.Panel2.Controls.Add(this.tabControlLevel);
            this.splitContainer6.Size = new Size(948, 531);
            this.splitContainer6.SplitterDistance = 218;
            this.splitContainer6.TabIndex = 0;
            // 
            // listBoxLevels
            // 
            this.listBoxLevels.Dock = DockStyle.Fill;
            this.listBoxLevels.FormattingEnabled = true;
            this.listBoxLevels.IntegralHeight = false;
            this.listBoxLevels.Location = new Point(0, 0);
            this.listBoxLevels.Name = "listBoxLevels";
            this.listBoxLevels.Size = new Size(218, 531);
            this.listBoxLevels.TabIndex = 1;
            this.listBoxLevels.SelectedIndexChanged += new EventHandler(this.SelectedLevelChanged);
            // 
            // tabControlLevel
            // 
            this.tabControlLevel.Controls.Add(this.tabPage6);
            this.tabControlLevel.Controls.Add(this.tabPagePalettes);
            this.tabControlLevel.Controls.Add(this.tabPage7);
            this.tabControlLevel.Controls.Add(this.tabPage3);
            this.tabControlLevel.Controls.Add(this.tabPageLayout);
            this.tabControlLevel.Dock = DockStyle.Fill;
            this.tabControlLevel.Location = new Point(0, 0);
            this.tabControlLevel.Name = "tabControlLevel";
            this.tabControlLevel.SelectedIndex = 0;
            this.tabControlLevel.Size = new Size(726, 531);
            this.tabControlLevel.TabIndex = 1;
            this.tabControlLevel.SelectedIndexChanged += new EventHandler(this.tabControlLevel_SelectedIndexChanged);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.propertyGridLevel);
            this.tabPage6.Controls.Add(this.treeViewLevelData);
            this.tabPage6.Location = new Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new Padding(3);
            this.tabPage6.Size = new Size(718, 505);
            this.tabPage6.TabIndex = 0;
            this.tabPage6.Text = "Level metadata";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // propertyGridLevel
            // 
            this.propertyGridLevel.Anchor = ((AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Bottom) 
                                                             | AnchorStyles.Right)));
            this.propertyGridLevel.Location = new Point(449, 6);
            this.propertyGridLevel.Name = "propertyGridLevel";
            this.propertyGridLevel.Size = new Size(263, 493);
            this.propertyGridLevel.TabIndex = 2;
            this.propertyGridLevel.PropertyValueChanged += new PropertyValueChangedEventHandler(this.propertyGridLevel_PropertyValueChanged);
            // 
            // treeViewLevelData
            // 
            this.treeViewLevelData.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
                                                              | AnchorStyles.Left) 
                                                             | AnchorStyles.Right)));
            this.treeViewLevelData.Font = new Font("Consolas", 8.25F);
            this.treeViewLevelData.Location = new Point(3, 6);
            this.treeViewLevelData.Name = "treeViewLevelData";
            this.treeViewLevelData.Size = new Size(440, 493);
            this.treeViewLevelData.TabIndex = 1;
            this.treeViewLevelData.AfterSelect += new TreeViewEventHandler(this.TreeViewLevelDataItemSelected);
            // 
            // tabPagePalettes
            // 
            this.tabPagePalettes.Location = new Point(4, 22);
            this.tabPagePalettes.Name = "tabPagePalettes";
            this.tabPagePalettes.Padding = new Padding(3);
            this.tabPagePalettes.Size = new Size(718, 505);
            this.tabPagePalettes.TabIndex = 4;
            this.tabPagePalettes.Text = "Palettes";
            this.tabPagePalettes.UseVisualStyleBackColor = true;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.splitContainer2);
            this.tabPage7.Controls.Add(this.toolStrip2);
            this.tabPage7.Location = new Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new Padding(3);
            this.tabPage7.Size = new Size(718, 505);
            this.tabPage7.TabIndex = 1;
            this.tabPage7.Text = "Tiles";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = BorderStyle.FixedSingle;
            this.splitContainer2.Dock = DockStyle.Fill;
            this.splitContainer2.Location = new Point(3, 28);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tilePicker1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer5);
            this.splitContainer2.Size = new Size(712, 474);
            this.splitContainer2.SplitterDistance = 288;
            this.splitContainer2.TabIndex = 1;
            // 
            // tilePicker1
            // 
            this.tilePicker1.Dock = DockStyle.Fill;
            this.tilePicker1.FixedItemsPerRow = true;
            this.tilePicker1.Items = null;
            this.tilePicker1.ItemsPerRow = 16;
            this.tilePicker1.Location = new Point(0, 0);
            this.tilePicker1.Name = "tilePicker1";
            this.tilePicker1.Scaling = 1;
            this.tilePicker1.SelectedIndex = -1;
            this.tilePicker1.Size = new Size(286, 472);
            this.tilePicker1.TabIndex = 0;
            this.tilePicker1.SelectionChanged += new EventHandler<IDrawableBlock>(this.tilePicker1_SelectionChanged);
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = DockStyle.Fill;
            this.splitContainer5.FixedPanel = FixedPanel.Panel2;
            this.splitContainer5.Location = new Point(0, 0);
            this.splitContainer5.Name = "splitContainer5";
            this.splitContainer5.Orientation = Orientation.Horizontal;
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.pictureBoxTilePreview);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.panel2);
            this.splitContainer5.Panel2.Controls.Add(this.label1);
            this.splitContainer5.Size = new Size(418, 472);
            this.splitContainer5.SplitterDistance = 353;
            this.splitContainer5.TabIndex = 1;
            // 
            // pictureBoxTilePreview
            // 
            this.pictureBoxTilePreview.Dock = DockStyle.Fill;
            this.pictureBoxTilePreview.Location = new Point(0, 0);
            this.pictureBoxTilePreview.Name = "pictureBoxTilePreview";
            this.pictureBoxTilePreview.Size = new Size(418, 353);
            this.pictureBoxTilePreview.SizeMode = PictureBoxSizeMode.CenterImage;
            this.pictureBoxTilePreview.TabIndex = 0;
            this.pictureBoxTilePreview.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.pictureBoxTileUsedIn);
            this.panel2.Dock = DockStyle.Fill;
            this.panel2.Location = new Point(0, 13);
            this.panel2.Name = "panel2";
            this.panel2.Size = new Size(418, 102);
            this.panel2.TabIndex = 1;
            // 
            // pictureBoxTileUsedIn
            // 
            this.pictureBoxTileUsedIn.Location = new Point(3, 3);
            this.pictureBoxTileUsedIn.Name = "pictureBoxTileUsedIn";
            this.pictureBoxTileUsedIn.Size = new Size(142, 82);
            this.pictureBoxTileUsedIn.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureBoxTileUsedIn.TabIndex = 0;
            this.pictureBoxTileUsedIn.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = DockStyle.Top;
            this.label1.Location = new Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Used in blocks:";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new ToolStripItem[] {
            this.buttonSaveTileset,
            this.buttonLoadTileset,
            this.buttonBlankUnusedTiles});
            this.toolStrip2.Location = new Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new Size(712, 25);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // buttonSaveTileset
            // 
            this.buttonSaveTileset.Image = ((Image)(resources.GetObject("buttonSaveTileset.Image")));
            this.buttonSaveTileset.ImageTransparentColor = Color.Magenta;
            this.buttonSaveTileset.Name = "buttonSaveTileset";
            this.buttonSaveTileset.Size = new Size(60, 22);
            this.buttonSaveTileset.Text = "Save...";
            this.buttonSaveTileset.Click += new EventHandler(this.buttonSaveTileset_Click);
            // 
            // buttonLoadTileset
            // 
            this.buttonLoadTileset.Image = ((Image)(resources.GetObject("buttonLoadTileset.Image")));
            this.buttonLoadTileset.ImageTransparentColor = Color.Magenta;
            this.buttonLoadTileset.Name = "buttonLoadTileset";
            this.buttonLoadTileset.Size = new Size(62, 22);
            this.buttonLoadTileset.Text = "Load...";
            this.buttonLoadTileset.Click += new EventHandler(this.buttonLoadTileset_Click);
            // 
            // buttonBlankUnusedTiles
            // 
            this.buttonBlankUnusedTiles.Image = ((Image)(resources.GetObject("buttonBlankUnusedTiles.Image")));
            this.buttonBlankUnusedTiles.ImageTransparentColor = Color.Magenta;
            this.buttonBlankUnusedTiles.Name = "buttonBlankUnusedTiles";
            this.buttonBlankUnusedTiles.Size = new Size(98, 22);
            this.buttonBlankUnusedTiles.Text = "Blank unused";
            this.buttonBlankUnusedTiles.Click += new EventHandler(this.buttonBlankUnusedTiles_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.splitContainer4);
            this.tabPage3.Location = new Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new Padding(3);
            this.tabPage3.Size = new Size(718, 505);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Blocks";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = DockStyle.Fill;
            this.splitContainer4.Location = new Point(3, 3);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.dataGridViewBlocks);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.AutoScroll = true;
            this.splitContainer4.Panel2.Controls.Add(this.pictureBoxBlockEditor);
            this.splitContainer4.Size = new Size(712, 499);
            this.splitContainer4.SplitterDistance = 439;
            this.splitContainer4.TabIndex = 2;
            // 
            // dataGridViewBlocks
            // 
            this.dataGridViewBlocks.AllowUserToAddRows = false;
            this.dataGridViewBlocks.AllowUserToDeleteRows = false;
            this.dataGridViewBlocks.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridViewBlocks.BorderStyle = BorderStyle.None;
            this.dataGridViewBlocks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBlocks.Columns.AddRange(new DataGridViewColumn[] {
            this.Image,
            this.Index,
            this.Solidity,
            this.Foreground,
            this.Used,
            this.UsedGlobal});
            this.dataGridViewBlocks.Dock = DockStyle.Fill;
            this.dataGridViewBlocks.Location = new Point(0, 0);
            this.dataGridViewBlocks.MultiSelect = false;
            this.dataGridViewBlocks.Name = "dataGridViewBlocks";
            this.dataGridViewBlocks.RowHeadersVisible = false;
            this.dataGridViewBlocks.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewBlocks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewBlocks.Size = new Size(439, 499);
            this.dataGridViewBlocks.TabIndex = 2;
            this.dataGridViewBlocks.CellPainting += new DataGridViewCellPaintingEventHandler(this.dataGridViewBlocks_CellPainting);
            this.dataGridViewBlocks.DataError += new DataGridViewDataErrorEventHandler(this.dataGridViewBlocks_DataError);
            this.dataGridViewBlocks.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(this.dataGridViewBlocks_EditingControlShowing);
            this.dataGridViewBlocks.SelectionChanged += new EventHandler(this.SelectedBlockChanged);
            // 
            // Image
            // 
            this.Image.DataPropertyName = "Image";
            this.Image.HeaderText = "Image";
            this.Image.Name = "Image";
            this.Image.ReadOnly = true;
            // 
            // Index
            // 
            this.Index.DataPropertyName = "Index";
            dataGridViewCellStyle4.Format = "X2";
            this.Index.DefaultCellStyle = dataGridViewCellStyle4;
            this.Index.HeaderText = "Index";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            // 
            // Solidity
            // 
            this.Solidity.DataPropertyName = "SolidityIndex";
            this.Solidity.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Solidity.HeaderText = "Solidity";
            this.Solidity.Name = "Solidity";
            this.Solidity.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // Foreground
            // 
            this.Foreground.DataPropertyName = "IsForeground";
            this.Foreground.HeaderText = "Foreground";
            this.Foreground.Name = "Foreground";
            // 
            // Used
            // 
            this.Used.DataPropertyName = "UsageCount";
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.Used.DefaultCellStyle = dataGridViewCellStyle5;
            this.Used.HeaderText = "Used";
            this.Used.Name = "Used";
            this.Used.ReadOnly = true;
            // 
            // UsedGlobal
            // 
            this.UsedGlobal.DataPropertyName = "GlobalUsageCount";
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleRight;
            this.UsedGlobal.DefaultCellStyle = dataGridViewCellStyle6;
            this.UsedGlobal.HeaderText = "Total used";
            this.UsedGlobal.Name = "UsedGlobal";
            this.UsedGlobal.ReadOnly = true;
            // 
            // pictureBoxBlockEditor
            // 
            this.pictureBoxBlockEditor.Dock = DockStyle.Fill;
            this.pictureBoxBlockEditor.Location = new Point(0, 0);
            this.pictureBoxBlockEditor.Name = "pictureBoxBlockEditor";
            this.pictureBoxBlockEditor.Size = new Size(269, 499);
            this.pictureBoxBlockEditor.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureBoxBlockEditor.TabIndex = 3;
            this.pictureBoxBlockEditor.TabStop = false;
            this.pictureBoxBlockEditor.MouseClick += new MouseEventHandler(this.BlockEditorMouseClick);
            // 
            // tabPageLayout
            // 
            this.tabPageLayout.Controls.Add(this.panel1);
            this.tabPageLayout.Controls.Add(this.toolStrip1);
            this.tabPageLayout.Location = new Point(4, 22);
            this.tabPageLayout.Name = "tabPageLayout";
            this.tabPageLayout.Padding = new Padding(3);
            this.tabPageLayout.Size = new Size(718, 505);
            this.tabPageLayout.TabIndex = 2;
            this.tabPageLayout.Text = "Layout";
            this.tabPageLayout.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBoxRenderedLevel);
            this.panel1.Dock = DockStyle.Fill;
            this.panel1.Location = new Point(3, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(712, 474);
            this.panel1.TabIndex = 4;
            // 
            // pictureBoxRenderedLevel
            // 
            this.pictureBoxRenderedLevel.Location = new Point(0, 0);
            this.pictureBoxRenderedLevel.Name = "pictureBoxRenderedLevel";
            this.pictureBoxRenderedLevel.Size = new Size(330, 243);
            this.pictureBoxRenderedLevel.SizeMode = PictureBoxSizeMode.AutoSize;
            this.pictureBoxRenderedLevel.TabIndex = 2;
            this.pictureBoxRenderedLevel.TabStop = false;
            this.pictureBoxRenderedLevel.MouseDown += new MouseEventHandler(this.LevelMapMouseDown);
            this.pictureBoxRenderedLevel.MouseUp += new MouseEventHandler(this.pb3_MouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new ToolStripItem[] {
            this.buttonShowObjects,
            this.buttonBlockNumbers,
            this.buttonBlockGaps,
            this.buttonTileGaps,
            this.buttonLevelBounds,
            this.toolStripSeparator1,
            this.toolStripButtonSaveRenderedLevel,
            this.buttonCopyFloor,
            this.buttonPasteFloor});
            this.toolStrip1.Location = new Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new Size(712, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonShowObjects
            // 
            this.buttonShowObjects.Checked = true;
            this.buttonShowObjects.CheckOnClick = true;
            this.buttonShowObjects.CheckState = CheckState.Checked;
            this.buttonShowObjects.Image = Resources.package;
            this.buttonShowObjects.ImageTransparentColor = Color.Magenta;
            this.buttonShowObjects.Name = "buttonShowObjects";
            this.buttonShowObjects.Size = new Size(67, 22);
            this.buttonShowObjects.Text = "Objects";
            this.buttonShowObjects.CheckedChanged += new EventHandler(this.LevelRenderModeChanged);
            // 
            // buttonBlockNumbers
            // 
            this.buttonBlockNumbers.Checked = true;
            this.buttonBlockNumbers.CheckOnClick = true;
            this.buttonBlockNumbers.CheckState = CheckState.Checked;
            this.buttonBlockNumbers.Image = ((Image)(resources.GetObject("buttonBlockNumbers.Image")));
            this.buttonBlockNumbers.ImageTransparentColor = Color.Magenta;
            this.buttonBlockNumbers.Name = "buttonBlockNumbers";
            this.buttonBlockNumbers.Size = new Size(106, 22);
            this.buttonBlockNumbers.Text = "Block numbers";
            this.buttonBlockNumbers.CheckedChanged += new EventHandler(this.LevelRenderModeChanged);
            // 
            // buttonBlockGaps
            // 
            this.buttonBlockGaps.CheckOnClick = true;
            this.buttonBlockGaps.Image = ((Image)(resources.GetObject("buttonBlockGaps.Image")));
            this.buttonBlockGaps.ImageTransparentColor = Color.Magenta;
            this.buttonBlockGaps.Name = "buttonBlockGaps";
            this.buttonBlockGaps.Size = new Size(84, 22);
            this.buttonBlockGaps.Text = "Block gaps";
            this.buttonBlockGaps.CheckedChanged += new EventHandler(this.LevelRenderModeChanged);
            // 
            // buttonTileGaps
            // 
            this.buttonTileGaps.CheckOnClick = true;
            this.buttonTileGaps.Image = ((Image)(resources.GetObject("buttonTileGaps.Image")));
            this.buttonTileGaps.ImageTransparentColor = Color.Magenta;
            this.buttonTileGaps.Name = "buttonTileGaps";
            this.buttonTileGaps.Size = new Size(73, 22);
            this.buttonTileGaps.Text = "Tile gaps";
            this.buttonTileGaps.CheckedChanged += new EventHandler(this.LevelRenderModeChanged);
            // 
            // buttonLevelBounds
            // 
            this.buttonLevelBounds.CheckOnClick = true;
            this.buttonLevelBounds.Image = ((Image)(resources.GetObject("buttonLevelBounds.Image")));
            this.buttonLevelBounds.ImageTransparentColor = Color.Magenta;
            this.buttonLevelBounds.Name = "buttonLevelBounds";
            this.buttonLevelBounds.Size = new Size(97, 22);
            this.buttonLevelBounds.Text = "Level bounds";
            this.buttonLevelBounds.CheckedChanged += new EventHandler(this.LevelRenderModeChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new Size(6, 25);
            // 
            // toolStripButtonSaveRenderedLevel
            // 
            this.toolStripButtonSaveRenderedLevel.Image = ((Image)(resources.GetObject("toolStripButtonSaveRenderedLevel.Image")));
            this.toolStripButtonSaveRenderedLevel.ImageTransparentColor = Color.Magenta;
            this.toolStripButtonSaveRenderedLevel.Name = "toolStripButtonSaveRenderedLevel";
            this.toolStripButtonSaveRenderedLevel.Size = new Size(60, 22);
            this.toolStripButtonSaveRenderedLevel.Text = "Save...";
            this.toolStripButtonSaveRenderedLevel.Click += new EventHandler(this.toolStripButtonSaveRenderedLevel_Click);
            // 
            // buttonCopyFloor
            // 
            this.buttonCopyFloor.Image = ((Image)(resources.GetObject("buttonCopyFloor.Image")));
            this.buttonCopyFloor.ImageTransparentColor = Color.Magenta;
            this.buttonCopyFloor.Name = "buttonCopyFloor";
            this.buttonCopyFloor.Size = new Size(55, 22);
            this.buttonCopyFloor.Text = "Copy";
            this.buttonCopyFloor.Click += new EventHandler(this.buttonCopyFloor_Click);
            // 
            // buttonPasteFloor
            // 
            this.buttonPasteFloor.Image = ((Image)(resources.GetObject("buttonPasteFloor.Image")));
            this.buttonPasteFloor.ImageTransparentColor = Color.Magenta;
            this.buttonPasteFloor.Name = "buttonPasteFloor";
            this.buttonPasteFloor.Size = new Size(55, 22);
            this.buttonPasteFloor.Text = "Paste";
            this.buttonPasteFloor.Click += new EventHandler(this.buttonPasteFloor_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer3);
            this.tabPage2.Location = new Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new Padding(3);
            this.tabPage2.Size = new Size(954, 537);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "Palettes";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = DockStyle.Fill;
            this.splitContainer3.Location = new Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.listBoxPalettes);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.pictureBoxPalette);
            this.splitContainer3.Size = new Size(948, 531);
            this.splitContainer3.SplitterDistance = 314;
            this.splitContainer3.TabIndex = 1;
            // 
            // listBoxPalettes
            // 
            this.listBoxPalettes.Dock = DockStyle.Fill;
            this.listBoxPalettes.Font = new Font("Consolas", 8.25F);
            this.listBoxPalettes.FormattingEnabled = true;
            this.listBoxPalettes.IntegralHeight = false;
            this.listBoxPalettes.Location = new Point(0, 0);
            this.listBoxPalettes.Name = "listBoxPalettes";
            this.listBoxPalettes.Size = new Size(314, 531);
            this.listBoxPalettes.TabIndex = 0;
            this.listBoxPalettes.SelectedIndexChanged += new EventHandler(this.ListBoxPalettesSelectedIndexChanged);
            // 
            // pictureBoxPalette
            // 
            this.pictureBoxPalette.Dock = DockStyle.Fill;
            this.pictureBoxPalette.Location = new Point(0, 0);
            this.pictureBoxPalette.Name = "pictureBoxPalette";
            this.pictureBoxPalette.Size = new Size(630, 531);
            this.pictureBoxPalette.TabIndex = 1;
            this.pictureBoxPalette.TabStop = false;
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.listBoxGameText);
            this.tabPage9.Location = new Point(4, 22);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Padding = new Padding(3);
            this.tabPage9.Size = new Size(954, 537);
            this.tabPage9.TabIndex = 7;
            this.tabPage9.Text = "Game Text";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // listBoxGameText
            // 
            this.listBoxGameText.Dock = DockStyle.Fill;
            this.listBoxGameText.Font = new Font("Consolas", 8.25F);
            this.listBoxGameText.FormattingEnabled = true;
            this.listBoxGameText.IntegralHeight = false;
            this.listBoxGameText.Location = new Point(3, 3);
            this.listBoxGameText.Name = "listBoxGameText";
            this.listBoxGameText.Size = new Size(948, 531);
            this.listBoxGameText.TabIndex = 1;
            this.listBoxGameText.DoubleClick += new EventHandler(this.GameTextDoubleClicked);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage9);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = DockStyle.Fill;
            this.tabControl1.Location = new Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new Size(962, 563);
            this.tabControl1.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new ToolStripItem[] {
            this.floorStatus,
            this.tileSetStatus});
            this.statusStrip1.Location = new Point(0, 587);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new Size(962, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // floorStatus
            // 
            this.floorStatus.Name = "floorStatus";
            this.floorStatus.Size = new Size(70, 17);
            this.floorStatus.Text = "Floor space:";
            // 
            // tileSetStatus
            // 
            this.tileSetStatus.Name = "tileSetStatus";
            this.tileSetStatus.Size = new Size(76, 17);
            this.tileSetStatus.Text = "Tileset space:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(962, 609);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "STH1 Editor by WV :: extended by Maxim";
            this.Load += new EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.splitContainer6.Panel1.ResumeLayout(false);
            this.splitContainer6.Panel2.ResumeLayout(false);
            ((ISupportInitialize)(this.splitContainer6)).EndInit();
            this.splitContainer6.ResumeLayout(false);
            this.tabControlLevel.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            this.splitContainer5.Panel2.PerformLayout();
            ((ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            ((ISupportInitialize)(this.pictureBoxTilePreview)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((ISupportInitialize)(this.pictureBoxTileUsedIn)).EndInit();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.Panel2.PerformLayout();
            ((ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((ISupportInitialize)(this.dataGridViewBlocks)).EndInit();
            ((ISupportInitialize)(this.pictureBoxBlockEditor)).EndInit();
            this.tabPageLayout.ResumeLayout(false);
            this.tabPageLayout.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((ISupportInitialize)(this.pictureBoxRenderedLevel)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((ISupportInitialize)(this.pictureBoxPalette)).EndInit();
            this.tabPage9.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openROMToolStripMenuItem;
        private ToolStripMenuItem saveROMToolStripMenuItem;
        private ToolStripMenuItem quickTestToolStripMenuItem;
        private TabPage tabPage5;
        private SplitContainer splitContainer6;
        private ListBox listBoxLevels;
        private TabControl tabControlLevel;
        private TabPage tabPage6;
        private PropertyGrid propertyGridLevel;
        private TreeView treeViewLevelData;
        private TabPage tabPagePalettes;
        private TabPage tabPage7;
        private SplitContainer splitContainer2;
        private ItemPicker tilePicker1;
        private SplitContainer splitContainer5;
        private PictureBox pictureBoxTilePreview;
        private Panel panel2;
        private PictureBox pictureBoxTileUsedIn;
        private Label label1;
        private ToolStrip toolStrip2;
        private ToolStripButton buttonSaveTileset;
        private TabPage tabPage3;
        private SplitContainer splitContainer4;
        private DataGridView dataGridViewBlocks;
        private DataGridViewImageColumn Image;
        private DataGridViewTextBoxColumn Index;
        private DataGridViewComboBoxColumn Solidity;
        private DataGridViewCheckBoxColumn Foreground;
        private DataGridViewTextBoxColumn Used;
        private DataGridViewTextBoxColumn UsedGlobal;
        private PictureBox pictureBoxBlockEditor;
        private TabPage tabPageLayout;
        private Panel panel1;
        private PictureBox pictureBoxRenderedLevel;
        private ToolStrip toolStrip1;
        private ToolStripButton buttonShowObjects;
        private ToolStripButton buttonBlockNumbers;
        private ToolStripButton buttonBlockGaps;
        private ToolStripButton buttonTileGaps;
        private ToolStripButton buttonLevelBounds;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonSaveRenderedLevel;
        private ToolStripButton buttonCopyFloor;
        private ToolStripButton buttonPasteFloor;
        private TabPage tabPage2;
        private SplitContainer splitContainer3;
        private ListBox listBoxPalettes;
        private PictureBox pictureBoxPalette;
        private TabPage tabPage9;
        private ListBox listBoxGameText;
        private TabControl tabControl1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel floorStatus;
        private ToolStripStatusLabel tileSetStatus;
        private ToolStripButton buttonLoadTileset;
        private ToolStripButton buttonBlankUnusedTiles;
    }
}

