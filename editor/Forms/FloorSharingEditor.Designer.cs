namespace sth1edwv.Forms
{
    sealed partial class FloorSharingEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.label1 = new System.Windows.Forms.Label();
            this.sharedLevelsList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.uniqueFloorsCombo = new System.Windows.Forms.ComboBox();
            this.ShareWithOtherButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.UnshareButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "This floor is shared with:";
            // 
            // sharedLevelsList
            // 
            this.sharedLevelsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sharedLevelsList.FormattingEnabled = true;
            this.sharedLevelsList.Location = new System.Drawing.Point(12, 25);
            this.sharedLevelsList.Name = "sharedLevelsList";
            this.sharedLevelsList.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.sharedLevelsList.Size = new System.Drawing.Size(417, 134);
            this.sharedLevelsList.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Change to data from level(s)";
            // 
            // uniqueFloorsCombo
            // 
            this.uniqueFloorsCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uniqueFloorsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.uniqueFloorsCombo.FormattingEnabled = true;
            this.uniqueFloorsCombo.Location = new System.Drawing.Point(6, 19);
            this.uniqueFloorsCombo.Name = "uniqueFloorsCombo";
            this.uniqueFloorsCombo.Size = new System.Drawing.Size(397, 21);
            this.uniqueFloorsCombo.TabIndex = 14;
            // 
            // ShareWithOtherButton
            // 
            this.ShareWithOtherButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ShareWithOtherButton.Location = new System.Drawing.Point(328, 134);
            this.ShareWithOtherButton.Name = "ShareWithOtherButton";
            this.ShareWithOtherButton.Size = new System.Drawing.Size(75, 23);
            this.ShareWithOtherButton.TabIndex = 15;
            this.ShareWithOtherButton.Text = "Change";
            this.ShareWithOtherButton.UseVisualStyleBackColor = true;
            this.ShareWithOtherButton.Click += new System.EventHandler(this.ShareWithOtherButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(311, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Note: you will lose this floor data if it is unique to this level";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(347, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "This will make this level use its own unique copy of the floor data.";
            // 
            // UnshareButton
            // 
            this.UnshareButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.UnshareButton.Location = new System.Drawing.Point(328, 134);
            this.UnshareButton.Name = "UnshareButton";
            this.UnshareButton.Size = new System.Drawing.Size(75, 23);
            this.UnshareButton.TabIndex = 19;
            this.UnshareButton.Text = "Unshare";
            this.UnshareButton.UseVisualStyleBackColor = true;
            this.UnshareButton.Click += new System.EventHandler(this.UnshareButton_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 165);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(417, 189);
            this.tabControl1.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.uniqueFloorsCombo);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.ShareWithOtherButton);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(409, 163);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Share with another level";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.UnshareButton);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(409, 163);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Split to unique";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // FloorSharingEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 366);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.sharedLevelsList);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FloorSharingEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Floor data sharing";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox sharedLevelsList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox uniqueFloorsCombo;
        private System.Windows.Forms.Button ShareWithOtherButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button UnshareButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
    }
}