namespace sth1edwv.Forms
{
    sealed partial class FloorSizeForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.LeftDelta = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.RightDelta = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.TopDelta = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.BottomDelta = new System.Windows.Forms.NumericUpDown();
            this.finalSizeLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.LeftDelta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RightDelta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TopDelta)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BottomDelta)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(189, 177);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 9;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.ButtonClick);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(270, 177);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.ButtonClick);
            // 
            // LeftDelta
            // 
            this.LeftDelta.Location = new System.Drawing.Point(108, 12);
            this.LeftDelta.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.LeftDelta.Name = "LeftDelta";
            this.LeftDelta.Size = new System.Drawing.Size(71, 20);
            this.LeftDelta.TabIndex = 1;
            this.LeftDelta.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Left";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Right";
            // 
            // RightDelta
            // 
            this.RightDelta.Location = new System.Drawing.Point(108, 38);
            this.RightDelta.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.RightDelta.Name = "RightDelta";
            this.RightDelta.Size = new System.Drawing.Size(71, 20);
            this.RightDelta.TabIndex = 3;
            this.RightDelta.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Top";
            // 
            // TopDelta
            // 
            this.TopDelta.Location = new System.Drawing.Point(108, 64);
            this.TopDelta.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.TopDelta.Name = "TopDelta";
            this.TopDelta.Size = new System.Drawing.Size(71, 20);
            this.TopDelta.TabIndex = 5;
            this.TopDelta.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Bottom";
            // 
            // BottomDelta
            // 
            this.BottomDelta.Location = new System.Drawing.Point(108, 90);
            this.BottomDelta.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.BottomDelta.Name = "BottomDelta";
            this.BottomDelta.Size = new System.Drawing.Size(71, 20);
            this.BottomDelta.TabIndex = 7;
            this.BottomDelta.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // finalSizeLabel
            // 
            this.finalSizeLabel.AutoSize = true;
            this.finalSizeLabel.Location = new System.Drawing.Point(10, 132);
            this.finalSizeLabel.Name = "finalSizeLabel";
            this.finalSizeLabel.Size = new System.Drawing.Size(76, 13);
            this.finalSizeLabel.TabIndex = 8;
            this.finalSizeLabel.Text = "Final map size:";
            // 
            // FloorSizeForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(357, 210);
            this.Controls.Add(this.finalSizeLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.BottomDelta);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TopDelta);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.RightDelta);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LeftDelta);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FloorSizeForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Resize floor";
            ((System.ComponentModel.ISupportInitialize)(this.LeftDelta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RightDelta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TopDelta)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BottomDelta)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.NumericUpDown LeftDelta;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown RightDelta;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown TopDelta;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown BottomDelta;
        private System.Windows.Forms.Label finalSizeLabel;
    }
}