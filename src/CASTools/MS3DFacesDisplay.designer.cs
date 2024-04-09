namespace XMODS
{
    partial class MS3DFacesDisplay
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.MS3DFacesDisplay_dataGridView = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.MS3DFacesDisplay_dataGridView)).BeginInit();
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
            // MS3DFacesDisplay_dataGridView
            // 
            this.MS3DFacesDisplay_dataGridView.AllowUserToAddRows = false;
            this.MS3DFacesDisplay_dataGridView.AllowUserToDeleteRows = false;
            this.MS3DFacesDisplay_dataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.MS3DFacesDisplay_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.MS3DFacesDisplay_dataGridView.DefaultCellStyle = dataGridViewCellStyle1;
            this.MS3DFacesDisplay_dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MS3DFacesDisplay_dataGridView.Location = new System.Drawing.Point(3, 26);
            this.MS3DFacesDisplay_dataGridView.Name = "MS3DFacesDisplay_dataGridView";
            this.MS3DFacesDisplay_dataGridView.ReadOnly = true;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.MS3DFacesDisplay_dataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.MS3DFacesDisplay_dataGridView.RowTemplate.Height = 75;
            this.MS3DFacesDisplay_dataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.MS3DFacesDisplay_dataGridView.Size = new System.Drawing.Size(982, 556);
            this.MS3DFacesDisplay_dataGridView.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.MS3DFacesDisplay_dataGridView, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(988, 585);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // MS3DFacesDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(988, 585);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MS3DFacesDisplay";
            this.Text = "GEOM Faces Display";
            this.Load += new System.EventHandler(this.MS3DFacesDisplay_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MS3DFacesDisplay_dataGridView)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView MS3DFacesDisplay_dataGridView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}