namespace XMODS
{
    partial class SlotRayDataDisplay
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
            this.SlotRay_dataGridView = new System.Windows.Forms.DataGridView();
            this.SlotIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FacePointIndices = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CoordinatesIntersection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DistanceOriginIntersection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OffsetIntersection = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SlotAveragePos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TransformOStoLS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PivotBoneIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.SlotRay_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // SlotRay_dataGridView
            // 
            this.SlotRay_dataGridView.AllowUserToAddRows = false;
            this.SlotRay_dataGridView.AllowUserToDeleteRows = false;
            this.SlotRay_dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.SlotRay_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SlotRay_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SlotIndex,
            this.FacePointIndices,
            this.CoordinatesIntersection,
            this.DistanceOriginIntersection,
            this.OffsetIntersection,
            this.SlotAveragePos,
            this.TransformOStoLS,
            this.PivotBoneIndex});
            this.SlotRay_dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SlotRay_dataGridView.Location = new System.Drawing.Point(0, 0);
            this.SlotRay_dataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.SlotRay_dataGridView.Name = "SlotRay_dataGridView";
            this.SlotRay_dataGridView.ReadOnly = true;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.SlotRay_dataGridView.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.SlotRay_dataGridView.RowTemplate.Height = 24;
            this.SlotRay_dataGridView.Size = new System.Drawing.Size(1345, 333);
            this.SlotRay_dataGridView.TabIndex = 0;
            // 
            // SlotIndex
            // 
            this.SlotIndex.HeaderText = "Slot Index";
            this.SlotIndex.Name = "SlotIndex";
            this.SlotIndex.ReadOnly = true;
            // 
            // FacePointIndices
            // 
            this.FacePointIndices.HeaderText = "Facepoints";
            this.FacePointIndices.Name = "FacePointIndices";
            this.FacePointIndices.ReadOnly = true;
            // 
            // CoordinatesIntersection
            // 
            this.CoordinatesIntersection.HeaderText = "Barycentric Coordinates of Intersection";
            this.CoordinatesIntersection.Name = "CoordinatesIntersection";
            this.CoordinatesIntersection.ReadOnly = true;
            this.CoordinatesIntersection.Width = 200;
            // 
            // DistanceOriginIntersection
            // 
            this.DistanceOriginIntersection.HeaderText = "Distance Origin to Intersection";
            this.DistanceOriginIntersection.Name = "DistanceOriginIntersection";
            this.DistanceOriginIntersection.ReadOnly = true;
            // 
            // OffsetIntersection
            // 
            this.OffsetIntersection.HeaderText = "Offset From Intersection";
            this.OffsetIntersection.Name = "OffsetIntersection";
            this.OffsetIntersection.ReadOnly = true;
            this.OffsetIntersection.Width = 200;
            // 
            // SlotAveragePos
            // 
            this.SlotAveragePos.HeaderText = "Slot Average Position";
            this.SlotAveragePos.Name = "SlotAveragePos";
            this.SlotAveragePos.ReadOnly = true;
            this.SlotAveragePos.Width = 200;
            // 
            // TransformOStoLS
            // 
            this.TransformOStoLS.HeaderText = "Transform from Object Space to Local Space";
            this.TransformOStoLS.Name = "TransformOStoLS";
            this.TransformOStoLS.ReadOnly = true;
            this.TransformOStoLS.Width = 250;
            // 
            // PivotBoneIndex
            // 
            this.PivotBoneIndex.HeaderText = "Pivot Index";
            this.PivotBoneIndex.Name = "PivotBoneIndex";
            this.PivotBoneIndex.ReadOnly = true;
            // 
            // SlotRayDataDisplay
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1345, 333);
            this.Controls.Add(this.SlotRay_dataGridView);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SlotRayDataDisplay";
            this.Text = "SlotRayDataDisplay";
            this.Load += new System.EventHandler(this.SlotRayDataDisplay_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SlotRay_dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView SlotRay_dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn SlotIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn FacePointIndices;
        private System.Windows.Forms.DataGridViewTextBoxColumn CoordinatesIntersection;
        private System.Windows.Forms.DataGridViewTextBoxColumn DistanceOriginIntersection;
        private System.Windows.Forms.DataGridViewTextBoxColumn OffsetIntersection;
        private System.Windows.Forms.DataGridViewTextBoxColumn SlotAveragePos;
        private System.Windows.Forms.DataGridViewTextBoxColumn TransformOStoLS;
        private System.Windows.Forms.DataGridViewTextBoxColumn PivotBoneIndex;
    }
}