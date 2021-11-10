﻿using System;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.splitContainer6 = new System.Windows.Forms.SplitContainer();
            this.listBoxLevels = new System.Windows.Forms.ListBox();
            this.tabControlLevel = new System.Windows.Forms.TabControl();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.propertyGridLevel = new System.Windows.Forms.PropertyGrid();
            this.treeViewLevelData = new System.Windows.Forms.TreeView();
            this.tabPagePalettes = new System.Windows.Forms.TabPage();
            this.tabPage7 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tilePicker1 = new sth1edwv.ItemPicker();
            this.splitContainer5 = new System.Windows.Forms.SplitContainer();
            this.pictureBoxTilePreview = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBoxTileUsedIn = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.buttonSaveTileset = new System.Windows.Forms.ToolStripButton();
            this.buttonLoadTileset = new System.Windows.Forms.ToolStripButton();
            this.buttonBlankUnusedTiles = new System.Windows.Forms.ToolStripButton();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.dataGridViewBlocks = new System.Windows.Forms.DataGridView();
            this.Image = new System.Windows.Forms.DataGridViewImageColumn();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Solidity = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Foreground = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Used = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UsedGlobal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pictureBoxBlockEditor = new System.Windows.Forms.PictureBox();
            this.tabPageLayout = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBoxRenderedLevel = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonShowObjects = new System.Windows.Forms.ToolStripButton();
            this.buttonBlockNumbers = new System.Windows.Forms.ToolStripButton();
            this.buttonBlockGaps = new System.Windows.Forms.ToolStripButton();
            this.buttonTileGaps = new System.Windows.Forms.ToolStripButton();
            this.buttonLevelBounds = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSaveRenderedLevel = new System.Windows.Forms.ToolStripButton();
            this.buttonCopyFloor = new System.Windows.Forms.ToolStripButton();
            this.buttonPasteFloor = new System.Windows.Forms.ToolStripButton();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.listBoxPalettes = new System.Windows.Forms.ListBox();
            this.pictureBoxPalette = new System.Windows.Forms.PictureBox();
            this.tabPage9 = new System.Windows.Forms.TabPage();
            this.listBoxGameText = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageScreens = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBoxScreens = new System.Windows.Forms.ListBox();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.floorStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tileSetStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer7 = new System.Windows.Forms.SplitContainer();
            this.layoutBlockChooser = new sth1edwv.ItemPicker();
            this.menuStrip1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).BeginInit();
            this.splitContainer6.Panel1.SuspendLayout();
            this.splitContainer6.Panel2.SuspendLayout();
            this.splitContainer6.SuspendLayout();
            this.tabControlLevel.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.tabPage7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
            this.splitContainer5.Panel1.SuspendLayout();
            this.splitContainer5.Panel2.SuspendLayout();
            this.splitContainer5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTilePreview)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTileUsedIn)).BeginInit();
            this.toolStrip2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBlocks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBlockEditor)).BeginInit();
            this.tabPageLayout.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRenderedLevel)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPalette)).BeginInit();
            this.tabPage9.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPageScreens.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).BeginInit();
            this.splitContainer7.Panel1.SuspendLayout();
            this.splitContainer7.Panel2.SuspendLayout();
            this.splitContainer7.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(962, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openROMToolStripMenuItem,
            this.saveROMToolStripMenuItem,
            this.quickTestToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openROMToolStripMenuItem
            // 
            this.openROMToolStripMenuItem.Name = "openROMToolStripMenuItem";
            this.openROMToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openROMToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.openROMToolStripMenuItem.Text = "Open ROM...";
            this.openROMToolStripMenuItem.Click += new System.EventHandler(this.openROMToolStripMenuItem_Click);
            // 
            // saveROMToolStripMenuItem
            // 
            this.saveROMToolStripMenuItem.Name = "saveROMToolStripMenuItem";
            this.saveROMToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveROMToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveROMToolStripMenuItem.Text = "Save as...";
            this.saveROMToolStripMenuItem.Click += new System.EventHandler(this.saveROMToolStripMenuItem_Click);
            // 
            // quickTestToolStripMenuItem
            // 
            this.quickTestToolStripMenuItem.Name = "quickTestToolStripMenuItem";
            this.quickTestToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quickTestToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.quickTestToolStripMenuItem.Text = "Quick test";
            this.quickTestToolStripMenuItem.Click += new System.EventHandler(this.quickTestToolStripMenuItem_Click);
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.splitContainer6);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(954, 537);
            this.tabPage5.TabIndex = 5;
            this.tabPage5.Text = "Levels";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // splitContainer6
            // 
            this.splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer6.Location = new System.Drawing.Point(3, 3);
            this.splitContainer6.Name = "splitContainer6";
            // 
            // splitContainer6.Panel1
            // 
            this.splitContainer6.Panel1.Controls.Add(this.listBoxLevels);
            // 
            // splitContainer6.Panel2
            // 
            this.splitContainer6.Panel2.Controls.Add(this.tabControlLevel);
            this.splitContainer6.Size = new System.Drawing.Size(948, 531);
            this.splitContainer6.SplitterDistance = 218;
            this.splitContainer6.TabIndex = 0;
            // 
            // listBoxLevels
            // 
            this.listBoxLevels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxLevels.FormattingEnabled = true;
            this.listBoxLevels.IntegralHeight = false;
            this.listBoxLevels.Location = new System.Drawing.Point(0, 0);
            this.listBoxLevels.Name = "listBoxLevels";
            this.listBoxLevels.Size = new System.Drawing.Size(218, 531);
            this.listBoxLevels.TabIndex = 1;
            this.listBoxLevels.SelectedIndexChanged += new System.EventHandler(this.SelectedLevelChanged);
            // 
            // tabControlLevel
            // 
            this.tabControlLevel.Controls.Add(this.tabPage6);
            this.tabControlLevel.Controls.Add(this.tabPagePalettes);
            this.tabControlLevel.Controls.Add(this.tabPage7);
            this.tabControlLevel.Controls.Add(this.tabPage3);
            this.tabControlLevel.Controls.Add(this.tabPageLayout);
            this.tabControlLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlLevel.Location = new System.Drawing.Point(0, 0);
            this.tabControlLevel.Name = "tabControlLevel";
            this.tabControlLevel.SelectedIndex = 0;
            this.tabControlLevel.Size = new System.Drawing.Size(726, 531);
            this.tabControlLevel.TabIndex = 1;
            this.tabControlLevel.SelectedIndexChanged += new System.EventHandler(this.tabControlLevel_SelectedIndexChanged);
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.propertyGridLevel);
            this.tabPage6.Controls.Add(this.treeViewLevelData);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(718, 505);
            this.tabPage6.TabIndex = 0;
            this.tabPage6.Text = "Level metadata";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // propertyGridLevel
            // 
            this.propertyGridLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGridLevel.Location = new System.Drawing.Point(449, 6);
            this.propertyGridLevel.Name = "propertyGridLevel";
            this.propertyGridLevel.Size = new System.Drawing.Size(263, 493);
            this.propertyGridLevel.TabIndex = 2;
            this.propertyGridLevel.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridLevel_PropertyValueChanged);
            // 
            // treeViewLevelData
            // 
            this.treeViewLevelData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewLevelData.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.treeViewLevelData.Location = new System.Drawing.Point(3, 6);
            this.treeViewLevelData.Name = "treeViewLevelData";
            this.treeViewLevelData.Size = new System.Drawing.Size(440, 493);
            this.treeViewLevelData.TabIndex = 1;
            this.treeViewLevelData.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewLevelDataItemSelected);
            // 
            // tabPagePalettes
            // 
            this.tabPagePalettes.Location = new System.Drawing.Point(4, 22);
            this.tabPagePalettes.Name = "tabPagePalettes";
            this.tabPagePalettes.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePalettes.Size = new System.Drawing.Size(718, 505);
            this.tabPagePalettes.TabIndex = 4;
            this.tabPagePalettes.Text = "Palettes";
            this.tabPagePalettes.UseVisualStyleBackColor = true;
            // 
            // tabPage7
            // 
            this.tabPage7.Controls.Add(this.splitContainer2);
            this.tabPage7.Controls.Add(this.toolStrip2);
            this.tabPage7.Location = new System.Drawing.Point(4, 22);
            this.tabPage7.Name = "tabPage7";
            this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage7.Size = new System.Drawing.Size(718, 505);
            this.tabPage7.TabIndex = 1;
            this.tabPage7.Text = "Tiles";
            this.tabPage7.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 28);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tilePicker1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer5);
            this.splitContainer2.Size = new System.Drawing.Size(712, 474);
            this.splitContainer2.SplitterDistance = 288;
            this.splitContainer2.TabIndex = 1;
            // 
            // tilePicker1
            // 
            this.tilePicker1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tilePicker1.FixedItemsPerRow = true;
            this.tilePicker1.ItemsPerRow = 16;
            this.tilePicker1.Location = new System.Drawing.Point(0, 0);
            this.tilePicker1.Name = "tilePicker1";
            this.tilePicker1.Scaling = 1;
            this.tilePicker1.SelectedIndex = -1;
            this.tilePicker1.Size = new System.Drawing.Size(286, 472);
            this.tilePicker1.TabIndex = 0;
            this.tilePicker1.SelectionChanged += new System.EventHandler<sth1edwv.IDrawableBlock>(this.tilePicker1_SelectionChanged);
            // 
            // splitContainer5
            // 
            this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer5.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer5.Location = new System.Drawing.Point(0, 0);
            this.splitContainer5.Name = "splitContainer5";
            this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer5.Panel1
            // 
            this.splitContainer5.Panel1.Controls.Add(this.pictureBoxTilePreview);
            // 
            // splitContainer5.Panel2
            // 
            this.splitContainer5.Panel2.Controls.Add(this.panel2);
            this.splitContainer5.Panel2.Controls.Add(this.label1);
            this.splitContainer5.Size = new System.Drawing.Size(418, 472);
            this.splitContainer5.SplitterDistance = 353;
            this.splitContainer5.TabIndex = 1;
            // 
            // pictureBoxTilePreview
            // 
            this.pictureBoxTilePreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxTilePreview.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxTilePreview.Name = "pictureBoxTilePreview";
            this.pictureBoxTilePreview.Size = new System.Drawing.Size(418, 353);
            this.pictureBoxTilePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxTilePreview.TabIndex = 0;
            this.pictureBoxTilePreview.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.pictureBoxTileUsedIn);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 13);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(418, 102);
            this.panel2.TabIndex = 1;
            // 
            // pictureBoxTileUsedIn
            // 
            this.pictureBoxTileUsedIn.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxTileUsedIn.Name = "pictureBoxTileUsedIn";
            this.pictureBoxTileUsedIn.Size = new System.Drawing.Size(142, 82);
            this.pictureBoxTileUsedIn.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxTileUsedIn.TabIndex = 0;
            this.pictureBoxTileUsedIn.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Used in blocks:";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSaveTileset,
            this.buttonLoadTileset,
            this.buttonBlankUnusedTiles});
            this.toolStrip2.Location = new System.Drawing.Point(3, 3);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(712, 25);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // buttonSaveTileset
            // 
            this.buttonSaveTileset.Image = ((System.Drawing.Image)(resources.GetObject("buttonSaveTileset.Image")));
            this.buttonSaveTileset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSaveTileset.Name = "buttonSaveTileset";
            this.buttonSaveTileset.Size = new System.Drawing.Size(60, 22);
            this.buttonSaveTileset.Text = "Save...";
            this.buttonSaveTileset.Click += new System.EventHandler(this.buttonSaveTileset_Click);
            // 
            // buttonLoadTileset
            // 
            this.buttonLoadTileset.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoadTileset.Image")));
            this.buttonLoadTileset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonLoadTileset.Name = "buttonLoadTileset";
            this.buttonLoadTileset.Size = new System.Drawing.Size(62, 22);
            this.buttonLoadTileset.Text = "Load...";
            this.buttonLoadTileset.Click += new System.EventHandler(this.buttonLoadTileset_Click);
            // 
            // buttonBlankUnusedTiles
            // 
            this.buttonBlankUnusedTiles.Image = ((System.Drawing.Image)(resources.GetObject("buttonBlankUnusedTiles.Image")));
            this.buttonBlankUnusedTiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonBlankUnusedTiles.Name = "buttonBlankUnusedTiles";
            this.buttonBlankUnusedTiles.Size = new System.Drawing.Size(98, 22);
            this.buttonBlankUnusedTiles.Text = "Blank unused";
            this.buttonBlankUnusedTiles.Click += new System.EventHandler(this.buttonBlankUnusedTiles_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.splitContainer4);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(718, 505);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Blocks";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(3, 3);
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
            this.splitContainer4.Size = new System.Drawing.Size(712, 499);
            this.splitContainer4.SplitterDistance = 439;
            this.splitContainer4.TabIndex = 2;
            // 
            // dataGridViewBlocks
            // 
            this.dataGridViewBlocks.AllowUserToAddRows = false;
            this.dataGridViewBlocks.AllowUserToDeleteRows = false;
            this.dataGridViewBlocks.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridViewBlocks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewBlocks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewBlocks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Image,
            this.Index,
            this.Solidity,
            this.Foreground,
            this.Used,
            this.UsedGlobal});
            this.dataGridViewBlocks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewBlocks.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewBlocks.MultiSelect = false;
            this.dataGridViewBlocks.Name = "dataGridViewBlocks";
            this.dataGridViewBlocks.RowHeadersVisible = false;
            this.dataGridViewBlocks.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewBlocks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewBlocks.Size = new System.Drawing.Size(439, 499);
            this.dataGridViewBlocks.TabIndex = 2;
            this.dataGridViewBlocks.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridViewBlocks_CellPainting);
            this.dataGridViewBlocks.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridViewBlocks_DataError);
            this.dataGridViewBlocks.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridViewBlocks_EditingControlShowing);
            this.dataGridViewBlocks.SelectionChanged += new System.EventHandler(this.SelectedBlockChanged);
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
            this.Solidity.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Solidity.HeaderText = "Solidity";
            this.Solidity.Name = "Solidity";
            this.Solidity.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
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
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.Used.DefaultCellStyle = dataGridViewCellStyle5;
            this.Used.HeaderText = "Used";
            this.Used.Name = "Used";
            this.Used.ReadOnly = true;
            // 
            // UsedGlobal
            // 
            this.UsedGlobal.DataPropertyName = "GlobalUsageCount";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.UsedGlobal.DefaultCellStyle = dataGridViewCellStyle6;
            this.UsedGlobal.HeaderText = "Total used";
            this.UsedGlobal.Name = "UsedGlobal";
            this.UsedGlobal.ReadOnly = true;
            // 
            // pictureBoxBlockEditor
            // 
            this.pictureBoxBlockEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxBlockEditor.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxBlockEditor.Name = "pictureBoxBlockEditor";
            this.pictureBoxBlockEditor.Size = new System.Drawing.Size(269, 499);
            this.pictureBoxBlockEditor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxBlockEditor.TabIndex = 3;
            this.pictureBoxBlockEditor.TabStop = false;
            this.pictureBoxBlockEditor.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BlockEditorMouseClick);
            // 
            // tabPageLayout
            // 
            this.tabPageLayout.Controls.Add(this.panel1);
            this.tabPageLayout.Controls.Add(this.toolStrip1);
            this.tabPageLayout.Location = new System.Drawing.Point(4, 22);
            this.tabPageLayout.Name = "tabPageLayout";
            this.tabPageLayout.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLayout.Size = new System.Drawing.Size(718, 505);
            this.tabPageLayout.TabIndex = 2;
            this.tabPageLayout.Text = "Layout";
            this.tabPageLayout.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.splitContainer7);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(712, 474);
            this.panel1.TabIndex = 4;
            // 
            // pictureBoxRenderedLevel
            // 
            this.pictureBoxRenderedLevel.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxRenderedLevel.Name = "pictureBoxRenderedLevel";
            this.pictureBoxRenderedLevel.Size = new System.Drawing.Size(330, 243);
            this.pictureBoxRenderedLevel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxRenderedLevel.TabIndex = 2;
            this.pictureBoxRenderedLevel.TabStop = false;
            this.pictureBoxRenderedLevel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LevelMapMouseDown);
            this.pictureBoxRenderedLevel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pb3_MouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonShowObjects,
            this.buttonBlockNumbers,
            this.buttonBlockGaps,
            this.buttonTileGaps,
            this.buttonLevelBounds,
            this.toolStripSeparator1,
            this.toolStripButtonSaveRenderedLevel,
            this.buttonCopyFloor,
            this.buttonPasteFloor});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(712, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonShowObjects
            // 
            this.buttonShowObjects.Checked = true;
            this.buttonShowObjects.CheckOnClick = true;
            this.buttonShowObjects.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonShowObjects.Image = global::sth1edwv.Properties.Resources.package;
            this.buttonShowObjects.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonShowObjects.Name = "buttonShowObjects";
            this.buttonShowObjects.Size = new System.Drawing.Size(67, 22);
            this.buttonShowObjects.Text = "Objects";
            this.buttonShowObjects.CheckedChanged += new System.EventHandler(this.LevelRenderModeChanged);
            // 
            // buttonBlockNumbers
            // 
            this.buttonBlockNumbers.Checked = true;
            this.buttonBlockNumbers.CheckOnClick = true;
            this.buttonBlockNumbers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.buttonBlockNumbers.Image = ((System.Drawing.Image)(resources.GetObject("buttonBlockNumbers.Image")));
            this.buttonBlockNumbers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonBlockNumbers.Name = "buttonBlockNumbers";
            this.buttonBlockNumbers.Size = new System.Drawing.Size(106, 22);
            this.buttonBlockNumbers.Text = "Block numbers";
            this.buttonBlockNumbers.CheckedChanged += new System.EventHandler(this.LevelRenderModeChanged);
            // 
            // buttonBlockGaps
            // 
            this.buttonBlockGaps.CheckOnClick = true;
            this.buttonBlockGaps.Image = ((System.Drawing.Image)(resources.GetObject("buttonBlockGaps.Image")));
            this.buttonBlockGaps.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonBlockGaps.Name = "buttonBlockGaps";
            this.buttonBlockGaps.Size = new System.Drawing.Size(84, 22);
            this.buttonBlockGaps.Text = "Block gaps";
            this.buttonBlockGaps.CheckedChanged += new System.EventHandler(this.LevelRenderModeChanged);
            // 
            // buttonTileGaps
            // 
            this.buttonTileGaps.CheckOnClick = true;
            this.buttonTileGaps.Image = ((System.Drawing.Image)(resources.GetObject("buttonTileGaps.Image")));
            this.buttonTileGaps.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonTileGaps.Name = "buttonTileGaps";
            this.buttonTileGaps.Size = new System.Drawing.Size(73, 22);
            this.buttonTileGaps.Text = "Tile gaps";
            this.buttonTileGaps.CheckedChanged += new System.EventHandler(this.LevelRenderModeChanged);
            // 
            // buttonLevelBounds
            // 
            this.buttonLevelBounds.CheckOnClick = true;
            this.buttonLevelBounds.Image = ((System.Drawing.Image)(resources.GetObject("buttonLevelBounds.Image")));
            this.buttonLevelBounds.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonLevelBounds.Name = "buttonLevelBounds";
            this.buttonLevelBounds.Size = new System.Drawing.Size(97, 22);
            this.buttonLevelBounds.Text = "Level bounds";
            this.buttonLevelBounds.CheckedChanged += new System.EventHandler(this.LevelRenderModeChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSaveRenderedLevel
            // 
            this.toolStripButtonSaveRenderedLevel.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveRenderedLevel.Image")));
            this.toolStripButtonSaveRenderedLevel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveRenderedLevel.Name = "toolStripButtonSaveRenderedLevel";
            this.toolStripButtonSaveRenderedLevel.Size = new System.Drawing.Size(60, 22);
            this.toolStripButtonSaveRenderedLevel.Text = "Save...";
            this.toolStripButtonSaveRenderedLevel.Click += new System.EventHandler(this.toolStripButtonSaveRenderedLevel_Click);
            // 
            // buttonCopyFloor
            // 
            this.buttonCopyFloor.Image = ((System.Drawing.Image)(resources.GetObject("buttonCopyFloor.Image")));
            this.buttonCopyFloor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCopyFloor.Name = "buttonCopyFloor";
            this.buttonCopyFloor.Size = new System.Drawing.Size(55, 22);
            this.buttonCopyFloor.Text = "Copy";
            this.buttonCopyFloor.Click += new System.EventHandler(this.buttonCopyFloor_Click);
            // 
            // buttonPasteFloor
            // 
            this.buttonPasteFloor.Image = ((System.Drawing.Image)(resources.GetObject("buttonPasteFloor.Image")));
            this.buttonPasteFloor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPasteFloor.Name = "buttonPasteFloor";
            this.buttonPasteFloor.Size = new System.Drawing.Size(55, 22);
            this.buttonPasteFloor.Text = "Paste";
            this.buttonPasteFloor.Click += new System.EventHandler(this.buttonPasteFloor_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitContainer3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(954, 537);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "Palettes";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.listBoxPalettes);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.pictureBoxPalette);
            this.splitContainer3.Size = new System.Drawing.Size(948, 531);
            this.splitContainer3.SplitterDistance = 314;
            this.splitContainer3.TabIndex = 1;
            // 
            // listBoxPalettes
            // 
            this.listBoxPalettes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxPalettes.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.listBoxPalettes.FormattingEnabled = true;
            this.listBoxPalettes.IntegralHeight = false;
            this.listBoxPalettes.Location = new System.Drawing.Point(0, 0);
            this.listBoxPalettes.Name = "listBoxPalettes";
            this.listBoxPalettes.Size = new System.Drawing.Size(314, 531);
            this.listBoxPalettes.TabIndex = 0;
            this.listBoxPalettes.SelectedIndexChanged += new System.EventHandler(this.ListBoxPalettesSelectedIndexChanged);
            // 
            // pictureBoxPalette
            // 
            this.pictureBoxPalette.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxPalette.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxPalette.Name = "pictureBoxPalette";
            this.pictureBoxPalette.Size = new System.Drawing.Size(630, 531);
            this.pictureBoxPalette.TabIndex = 1;
            this.pictureBoxPalette.TabStop = false;
            // 
            // tabPage9
            // 
            this.tabPage9.Controls.Add(this.listBoxGameText);
            this.tabPage9.Location = new System.Drawing.Point(4, 22);
            this.tabPage9.Name = "tabPage9";
            this.tabPage9.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage9.Size = new System.Drawing.Size(954, 537);
            this.tabPage9.TabIndex = 7;
            this.tabPage9.Text = "Game Text";
            this.tabPage9.UseVisualStyleBackColor = true;
            // 
            // listBoxGameText
            // 
            this.listBoxGameText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxGameText.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.listBoxGameText.FormattingEnabled = true;
            this.listBoxGameText.IntegralHeight = false;
            this.listBoxGameText.Location = new System.Drawing.Point(3, 3);
            this.listBoxGameText.Name = "listBoxGameText";
            this.listBoxGameText.Size = new System.Drawing.Size(948, 531);
            this.listBoxGameText.TabIndex = 1;
            this.listBoxGameText.DoubleClick += new System.EventHandler(this.GameTextDoubleClicked);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage9);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPageScreens);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(962, 563);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPageScreens
            // 
            this.tabPageScreens.Controls.Add(this.splitContainer1);
            this.tabPageScreens.Location = new System.Drawing.Point(4, 22);
            this.tabPageScreens.Name = "tabPageScreens";
            this.tabPageScreens.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageScreens.Size = new System.Drawing.Size(954, 537);
            this.tabPageScreens.TabIndex = 8;
            this.tabPageScreens.Text = "Screens";
            this.tabPageScreens.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBoxScreens);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.splitContainer1.Size = new System.Drawing.Size(948, 531);
            this.splitContainer1.SplitterDistance = 316;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBoxScreens
            // 
            this.listBoxScreens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxScreens.FormattingEnabled = true;
            this.listBoxScreens.Location = new System.Drawing.Point(0, 0);
            this.listBoxScreens.Name = "listBoxScreens";
            this.listBoxScreens.Size = new System.Drawing.Size(316, 531);
            this.listBoxScreens.TabIndex = 0;
            this.listBoxScreens.SelectedIndexChanged += new System.EventHandler(this.listBoxScreens_SelectedIndexChanged);
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(372, 531);
            this.propertyGrid1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.pictureBox1.Location = new System.Drawing.Point(372, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(256, 531);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.floorStatus,
            this.tileSetStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 587);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(962, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // floorStatus
            // 
            this.floorStatus.Name = "floorStatus";
            this.floorStatus.Size = new System.Drawing.Size(70, 17);
            this.floorStatus.Text = "Floor space:";
            // 
            // tileSetStatus
            // 
            this.tileSetStatus.Name = "tileSetStatus";
            this.tileSetStatus.Size = new System.Drawing.Size(76, 17);
            this.tileSetStatus.Text = "Tileset space:";
            // 
            // splitContainer7
            // 
            this.splitContainer7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer7.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer7.Location = new System.Drawing.Point(0, 0);
            this.splitContainer7.Name = "splitContainer7";
            // 
            // splitContainer7.Panel1
            // 
            this.splitContainer7.Panel1.AutoScroll = true;
            this.splitContainer7.Panel1.Controls.Add(this.pictureBoxRenderedLevel);
            // 
            // splitContainer7.Panel2
            // 
            this.splitContainer7.Panel2.Controls.Add(this.layoutBlockChooser);
            this.splitContainer7.Size = new System.Drawing.Size(712, 474);
            this.splitContainer7.SplitterDistance = 528;
            this.splitContainer7.TabIndex = 3;
            // 
            // layoutBlockChooser
            // 
            this.layoutBlockChooser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutBlockChooser.FixedItemsPerRow = false;
            this.layoutBlockChooser.ItemsPerRow = 0;
            this.layoutBlockChooser.Location = new System.Drawing.Point(0, 0);
            this.layoutBlockChooser.Name = "layoutBlockChooser";
            this.layoutBlockChooser.Scaling = 1;
            this.layoutBlockChooser.SelectedIndex = -1;
            this.layoutBlockChooser.Size = new System.Drawing.Size(180, 474);
            this.layoutBlockChooser.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(962, 609);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "STH1 Editor by WV :: extended by Maxim";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.splitContainer6.Panel1.ResumeLayout(false);
            this.splitContainer6.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).EndInit();
            this.splitContainer6.ResumeLayout(false);
            this.tabControlLevel.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.tabPage7.ResumeLayout(false);
            this.tabPage7.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer5.Panel1.ResumeLayout(false);
            this.splitContainer5.Panel2.ResumeLayout(false);
            this.splitContainer5.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
            this.splitContainer5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTilePreview)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTileUsedIn)).EndInit();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewBlocks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBlockEditor)).EndInit();
            this.tabPageLayout.ResumeLayout(false);
            this.tabPageLayout.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRenderedLevel)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPalette)).EndInit();
            this.tabPage9.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPageScreens.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer7.Panel1.ResumeLayout(false);
            this.splitContainer7.Panel1.PerformLayout();
            this.splitContainer7.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).EndInit();
            this.splitContainer7.ResumeLayout(false);
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
        private TabPage tabPageScreens;
        private SplitContainer splitContainer1;
        private ListBox listBoxScreens;
        private PropertyGrid propertyGrid1;
        private PictureBox pictureBox1;
        private SplitContainer splitContainer7;
        private ItemPicker layoutBlockChooser;
    }
}
