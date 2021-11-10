using System.ComponentModel;

namespace sth1edwv
{
    partial class BlockChooser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.itemPicker1 = new sth1edwv.ItemPicker();
            this.SuspendLayout();
            // 
            // itemPicker1
            // 
            this.itemPicker1.AutoSize = true;
            this.itemPicker1.FixedItemsPerRow = true;
            this.itemPicker1.ItemsPerRow = 8;
            this.itemPicker1.Location = new System.Drawing.Point(3, 3);
            this.itemPicker1.Name = "itemPicker1";
            this.itemPicker1.SelectedIndex = -1;
            this.itemPicker1.Size = new System.Drawing.Size(100, 100);
            this.itemPicker1.TabIndex = 0;
            this.itemPicker1.SelectionChanged += new System.EventHandler<sth1edwv.IDrawableBlock>(this.itemPicker1_SelectionChanged);
            // 
            // BlockChooser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(376, 392);
            this.Controls.Add(this.itemPicker1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BlockChooser";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Block Chooser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ItemPicker itemPicker1;
    }
}