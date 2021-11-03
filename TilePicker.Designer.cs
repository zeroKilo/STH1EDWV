
namespace sth1edwv
{
    sealed partial class TilePicker
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TilePicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.Name = "TilePicker";
            this.Size = new System.Drawing.Size(227, 217);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TilePicker_Paint);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TilePicker_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TilePicker_MouseClick);
            this.Resize += new System.EventHandler(this.TilePicker_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
