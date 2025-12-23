namespace XMODS
{
    partial class MS3DVertexDisplay
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
            this.MS3DVertexDisplay_dataGridView = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.MS3DVertexDisplay_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // MS3DVertexDisplay_dataGridView
            // 
            this.MS3DVertexDisplay_dataGridView.AllowUserToAddRows = false;
            this.MS3DVertexDisplay_dataGridView.AllowUserToDeleteRows = false;
            this.MS3DVertexDisplay_dataGridView.AllowUserToOrderColumns = true;
            this.MS3DVertexDisplay_dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.MS3DVertexDisplay_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MS3DVertexDisplay_dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MS3DVertexDisplay_dataGridView.Location = new System.Drawing.Point(0, 0);
            this.MS3DVertexDisplay_dataGridView.Name = "MS3DVertexDisplay_dataGridView";
            this.MS3DVertexDisplay_dataGridView.ReadOnly = true;
            this.MS3DVertexDisplay_dataGridView.RowTemplate.Height = 24;
            this.MS3DVertexDisplay_dataGridView.Size = new System.Drawing.Size(962, 585);
            this.MS3DVertexDisplay_dataGridView.TabIndex = 0;
            this.MS3DVertexDisplay_dataGridView.Scroll += new System.Windows.Forms.ScrollEventHandler(this.MS3DVertexDisplaygrid_Scroll);
            this.MS3DVertexDisplay_dataGridView.SizeChanged += new System.EventHandler(this.MS3DVertexDisplaygrid_Resize);
            this.MS3DVertexDisplay_dataGridView.Paint += new System.Windows.Forms.PaintEventHandler(this.MS3DVertexDisplaygrid_Paint);
            // 
            // MS3DVertexDisplay
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(962, 585);
            this.Controls.Add(this.MS3DVertexDisplay_dataGridView);
            this.Name = "MS3DVertexDisplay";
            this.Text = "MS3D Vertex Listing";
            this.Load += new System.EventHandler(this.MS3DVertexDisplay_onLoad);
            ((System.ComponentModel.ISupportInitialize)(this.MS3DVertexDisplay_dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView MS3DVertexDisplay_dataGridView;

    }
}