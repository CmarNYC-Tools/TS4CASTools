namespace XMODS
{
    partial class GEOMFacesDisplay
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
            this.GEOMFacesDisplay_dataGridView = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.GEOMFacesDisplay_dataGridView)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.label1.Size = new System.Drawing.Size(50, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Faces:";
            // 
            // GEOMFacesDisplay_dataGridView
            // 
            this.GEOMFacesDisplay_dataGridView.AllowUserToAddRows = false;
            this.GEOMFacesDisplay_dataGridView.AllowUserToDeleteRows = false;
            this.GEOMFacesDisplay_dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.GEOMFacesDisplay_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.GEOMFacesDisplay_dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GEOMFacesDisplay_dataGridView.Location = new System.Drawing.Point(3, 26);
            this.GEOMFacesDisplay_dataGridView.Name = "GEOMFacesDisplay_dataGridView";
            this.GEOMFacesDisplay_dataGridView.ReadOnly = true;
            this.GEOMFacesDisplay_dataGridView.RowTemplate.Height = 24;
            this.GEOMFacesDisplay_dataGridView.Size = new System.Drawing.Size(541, 556);
            this.GEOMFacesDisplay_dataGridView.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.GEOMFacesDisplay_dataGridView, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(547, 585);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // GEOMFacesDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 585);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "GEOMFacesDisplay";
            this.Text = "GEOM Faces Display";
            this.Load += new System.EventHandler(this.GEOMFacesDisplay_Load);
            ((System.ComponentModel.ISupportInitialize)(this.GEOMFacesDisplay_dataGridView)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView GEOMFacesDisplay_dataGridView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}