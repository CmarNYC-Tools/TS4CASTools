namespace XMODS
{
    partial class MeshLodRegionAssignmentForm
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
            this.MeshAssignFile = new System.Windows.Forms.TextBox();
            this.MeshAssignFile_button = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.MeshAssignLOD_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.MeshAssignRegion_checkedListBox = new System.Windows.Forms.CheckedListBox();
            this.MeshAssignFullHair_button = new System.Windows.Forms.Button();
            this.MeshAssignHatStraight_button = new System.Windows.Forms.Button();
            this.MeshAssignHatTilted_button = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.MeshAssignGo_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.MeshAssignCancel_button = new System.Windows.Forms.Button();
            this.MeshAssignLayer_textBox = new System.Windows.Forms.TextBox();
            this.LayerHelp_button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.MeshAssignLOD_numericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Mesh:";
            // 
            // MeshAssignFile
            // 
            this.MeshAssignFile.Location = new System.Drawing.Point(46, 10);
            this.MeshAssignFile.Margin = new System.Windows.Forms.Padding(2);
            this.MeshAssignFile.Name = "MeshAssignFile";
            this.MeshAssignFile.Size = new System.Drawing.Size(476, 20);
            this.MeshAssignFile.TabIndex = 1;
            // 
            // MeshAssignFile_button
            // 
            this.MeshAssignFile_button.Location = new System.Drawing.Point(526, 4);
            this.MeshAssignFile_button.Margin = new System.Windows.Forms.Padding(2);
            this.MeshAssignFile_button.Name = "MeshAssignFile_button";
            this.MeshAssignFile_button.Size = new System.Drawing.Size(75, 29);
            this.MeshAssignFile_button.TabIndex = 2;
            this.MeshAssignFile_button.Text = "Select";
            this.MeshAssignFile_button.UseVisualStyleBackColor = true;
            this.MeshAssignFile_button.Click += new System.EventHandler(this.MeshAssignFile_button_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 41);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "LOD:";
            // 
            // MeshAssignLOD_numericUpDown
            // 
            this.MeshAssignLOD_numericUpDown.Location = new System.Drawing.Point(87, 39);
            this.MeshAssignLOD_numericUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.MeshAssignLOD_numericUpDown.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.MeshAssignLOD_numericUpDown.Name = "MeshAssignLOD_numericUpDown";
            this.MeshAssignLOD_numericUpDown.Size = new System.Drawing.Size(61, 20);
            this.MeshAssignLOD_numericUpDown.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(168, 41);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Region:";
            // 
            // MeshAssignRegion_checkedListBox
            // 
            this.MeshAssignRegion_checkedListBox.FormattingEnabled = true;
            this.MeshAssignRegion_checkedListBox.Location = new System.Drawing.Point(215, 41);
            this.MeshAssignRegion_checkedListBox.Margin = new System.Windows.Forms.Padding(2);
            this.MeshAssignRegion_checkedListBox.Name = "MeshAssignRegion_checkedListBox";
            this.MeshAssignRegion_checkedListBox.Size = new System.Drawing.Size(219, 214);
            this.MeshAssignRegion_checkedListBox.TabIndex = 6;
            // 
            // MeshAssignFullHair_button
            // 
            this.MeshAssignFullHair_button.Location = new System.Drawing.Point(443, 57);
            this.MeshAssignFullHair_button.Margin = new System.Windows.Forms.Padding(2);
            this.MeshAssignFullHair_button.Name = "MeshAssignFullHair_button";
            this.MeshAssignFullHair_button.Size = new System.Drawing.Size(79, 28);
            this.MeshAssignFullHair_button.TabIndex = 7;
            this.MeshAssignFullHair_button.Text = "Full Hair";
            this.MeshAssignFullHair_button.UseVisualStyleBackColor = true;
            this.MeshAssignFullHair_button.Click += new System.EventHandler(this.MeshAssignFullHair_button_Click);
            // 
            // MeshAssignHatStraight_button
            // 
            this.MeshAssignHatStraight_button.Location = new System.Drawing.Point(443, 90);
            this.MeshAssignHatStraight_button.Margin = new System.Windows.Forms.Padding(2);
            this.MeshAssignHatStraight_button.Name = "MeshAssignHatStraight_button";
            this.MeshAssignHatStraight_button.Size = new System.Drawing.Size(79, 49);
            this.MeshAssignHatStraight_button.TabIndex = 8;
            this.MeshAssignHatStraight_button.Text = "Hat Cut\r\nStraight";
            this.MeshAssignHatStraight_button.UseVisualStyleBackColor = true;
            this.MeshAssignHatStraight_button.Click += new System.EventHandler(this.MeshAssignHatStraight_button_Click);
            // 
            // MeshAssignHatTilted_button
            // 
            this.MeshAssignHatTilted_button.Location = new System.Drawing.Point(443, 144);
            this.MeshAssignHatTilted_button.Margin = new System.Windows.Forms.Padding(2);
            this.MeshAssignHatTilted_button.Name = "MeshAssignHatTilted_button";
            this.MeshAssignHatTilted_button.Size = new System.Drawing.Size(79, 49);
            this.MeshAssignHatTilted_button.TabIndex = 9;
            this.MeshAssignHatTilted_button.Text = "Hat Cut\r\nTilted";
            this.MeshAssignHatTilted_button.UseVisualStyleBackColor = true;
            this.MeshAssignHatTilted_button.Click += new System.EventHandler(this.MeshAssignHatTilted_button_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(441, 41);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Hair Presets:";
            // 
            // MeshAssignGo_button
            // 
            this.MeshAssignGo_button.Location = new System.Drawing.Point(168, 274);
            this.MeshAssignGo_button.Margin = new System.Windows.Forms.Padding(2);
            this.MeshAssignGo_button.Name = "MeshAssignGo_button";
            this.MeshAssignGo_button.Size = new System.Drawing.Size(128, 30);
            this.MeshAssignGo_button.TabIndex = 11;
            this.MeshAssignGo_button.Text = "Save";
            this.MeshAssignGo_button.UseVisualStyleBackColor = true;
            this.MeshAssignGo_button.Click += new System.EventHandler(this.MeshAssignGo_button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(43, 90);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Layer:";
            // 
            // MeshAssignCancel_button
            // 
            this.MeshAssignCancel_button.Location = new System.Drawing.Point(316, 274);
            this.MeshAssignCancel_button.Margin = new System.Windows.Forms.Padding(2);
            this.MeshAssignCancel_button.Name = "MeshAssignCancel_button";
            this.MeshAssignCancel_button.Size = new System.Drawing.Size(128, 30);
            this.MeshAssignCancel_button.TabIndex = 14;
            this.MeshAssignCancel_button.Text = "Cancel";
            this.MeshAssignCancel_button.UseVisualStyleBackColor = true;
            this.MeshAssignCancel_button.Click += new System.EventHandler(this.MeshAssignCancel_button_Click);
            // 
            // MeshAssignLayer_textBox
            // 
            this.MeshAssignLayer_textBox.Location = new System.Drawing.Point(87, 87);
            this.MeshAssignLayer_textBox.Margin = new System.Windows.Forms.Padding(2);
            this.MeshAssignLayer_textBox.Name = "MeshAssignLayer_textBox";
            this.MeshAssignLayer_textBox.Size = new System.Drawing.Size(61, 20);
            this.MeshAssignLayer_textBox.TabIndex = 15;
            this.MeshAssignLayer_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LayerHelp_button
            // 
            this.LayerHelp_button.Location = new System.Drawing.Point(153, 82);
            this.LayerHelp_button.Name = "LayerHelp_button";
            this.LayerHelp_button.Size = new System.Drawing.Size(39, 28);
            this.LayerHelp_button.TabIndex = 16;
            this.LayerHelp_button.Text = "?";
            this.LayerHelp_button.UseVisualStyleBackColor = true;
            this.LayerHelp_button.Click += new System.EventHandler(this.LayerHelp_button_Click);
            // 
            // MeshLodRegionAssignmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 310);
            this.Controls.Add(this.LayerHelp_button);
            this.Controls.Add(this.MeshAssignLayer_textBox);
            this.Controls.Add(this.MeshAssignCancel_button);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.MeshAssignGo_button);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.MeshAssignHatTilted_button);
            this.Controls.Add(this.MeshAssignHatStraight_button);
            this.Controls.Add(this.MeshAssignFullHair_button);
            this.Controls.Add(this.MeshAssignRegion_checkedListBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.MeshAssignLOD_numericUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MeshAssignFile_button);
            this.Controls.Add(this.MeshAssignFile);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MeshLodRegionAssignmentForm";
            this.Text = "Mesh Information";
            ((System.ComponentModel.ISupportInitialize)(this.MeshAssignLOD_numericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox MeshAssignFile;
        private System.Windows.Forms.Button MeshAssignFile_button;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown MeshAssignLOD_numericUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckedListBox MeshAssignRegion_checkedListBox;
        private System.Windows.Forms.Button MeshAssignFullHair_button;
        private System.Windows.Forms.Button MeshAssignHatStraight_button;
        private System.Windows.Forms.Button MeshAssignHatTilted_button;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button MeshAssignGo_button;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button MeshAssignCancel_button;
        private System.Windows.Forms.TextBox MeshAssignLayer_textBox;
        private System.Windows.Forms.Button LayerHelp_button;
    }
}