namespace XMODS
{
    partial class StitchForm
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
            this.Stitch_dataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.Stitch_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // Stitch_dataGridView
            // 
            this.Stitch_dataGridView.AllowUserToAddRows = false;
            this.Stitch_dataGridView.AllowUserToDeleteRows = false;
            this.Stitch_dataGridView.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.Stitch_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Stitch_dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Stitch_dataGridView.Location = new System.Drawing.Point(0, 0);
            this.Stitch_dataGridView.Name = "Stitch_dataGridView";
            this.Stitch_dataGridView.ReadOnly = true;
            this.Stitch_dataGridView.RowTemplate.Height = 24;
            this.Stitch_dataGridView.Size = new System.Drawing.Size(1112, 525);
            this.Stitch_dataGridView.TabIndex = 0;
            // 
            // StitchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1112, 525);
            this.Controls.Add(this.Stitch_dataGridView);
            this.Name = "StitchForm";
            this.Text = "StitchForm";
            ((System.ComponentModel.ISupportInitialize)(this.Stitch_dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView Stitch_dataGridView;
    }
}