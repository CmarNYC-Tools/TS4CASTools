namespace XMODS
{
    partial class GEOMDataDisplay
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
            this.GEOMDataDisplay_dataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.GEOMDataDisplay_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // GEOMDataDisplay_dataGridView
            // 
            this.GEOMDataDisplay_dataGridView.AllowUserToAddRows = false;
            this.GEOMDataDisplay_dataGridView.AllowUserToDeleteRows = false;
            this.GEOMDataDisplay_dataGridView.AllowUserToOrderColumns = true;
            this.GEOMDataDisplay_dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.GEOMDataDisplay_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GEOMDataDisplay_dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GEOMDataDisplay_dataGridView.Location = new System.Drawing.Point(0, 0);
            this.GEOMDataDisplay_dataGridView.Name = "GEOMDataDisplay_dataGridView";
            this.GEOMDataDisplay_dataGridView.ReadOnly = true;
            this.GEOMDataDisplay_dataGridView.RowTemplate.Height = 24;
            this.GEOMDataDisplay_dataGridView.Size = new System.Drawing.Size(797, 585);
            this.GEOMDataDisplay_dataGridView.TabIndex = 0;
            this.GEOMDataDisplay_dataGridView.Scroll += new System.Windows.Forms.ScrollEventHandler(this.GEOMDataDisplaygrid_Scroll);
            this.GEOMDataDisplay_dataGridView.SizeChanged += new System.EventHandler(this.GEOMDataDisplaygrid_Resize);
            this.GEOMDataDisplay_dataGridView.Paint += new System.Windows.Forms.PaintEventHandler(this.GEOMDataDisplaygrid_Paint);
            // 
            // GEOMDataDisplay
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 585);
            this.Controls.Add(this.GEOMDataDisplay_dataGridView);
            this.Name = "GEOMDataDisplay";
            this.Text = "Mesh Data Listing";
            this.Load += new System.EventHandler(this.GEOMDataDisplay_onLoad);
            ((System.ComponentModel.ISupportInitialize)(this.GEOMDataDisplay_dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView GEOMDataDisplay_dataGridView;

    }
}