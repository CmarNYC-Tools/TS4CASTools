namespace XMODS
{
    partial class MapResizer
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
            this.ImageFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PartType_comboBox = new System.Windows.Forms.ComboBox();
            this.Go_button = new System.Windows.Forms.Button();
            this.Select_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select Normal or Emission Map:";
            // 
            // ImageFile
            // 
            this.ImageFile.Location = new System.Drawing.Point(29, 61);
            this.ImageFile.Name = "ImageFile";
            this.ImageFile.Size = new System.Drawing.Size(278, 20);
            this.ImageFile.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Select Part Type:";
            // 
            // PartType_comboBox
            // 
            this.PartType_comboBox.FormattingEnabled = true;
            this.PartType_comboBox.Items.AddRange(new object[] {
            "Top/Bottom/Body",
            "Shoes",
            "Hair",
            "Hat",
            "Accessory"});
            this.PartType_comboBox.Location = new System.Drawing.Point(125, 107);
            this.PartType_comboBox.Name = "PartType_comboBox";
            this.PartType_comboBox.Size = new System.Drawing.Size(182, 21);
            this.PartType_comboBox.TabIndex = 3;
            // 
            // Go_button
            // 
            this.Go_button.Location = new System.Drawing.Point(79, 164);
            this.Go_button.Name = "Go_button";
            this.Go_button.Size = new System.Drawing.Size(187, 38);
            this.Go_button.TabIndex = 4;
            this.Go_button.Text = "Convert to Correct Size";
            this.Go_button.UseVisualStyleBackColor = true;
            this.Go_button.Click += new System.EventHandler(this.Go_button_Click);
            // 
            // Select_button
            // 
            this.Select_button.Location = new System.Drawing.Point(188, 29);
            this.Select_button.Name = "Select_button";
            this.Select_button.Size = new System.Drawing.Size(119, 26);
            this.Select_button.TabIndex = 5;
            this.Select_button.Text = "Select";
            this.Select_button.UseVisualStyleBackColor = true;
            this.Select_button.Click += new System.EventHandler(this.Select_button_Click);
            // 
            // MapResizer
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 231);
            this.Controls.Add(this.Select_button);
            this.Controls.Add(this.Go_button);
            this.Controls.Add(this.PartType_comboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ImageFile);
            this.Controls.Add(this.label1);
            this.Name = "MapResizer";
            this.Text = "MapResizer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ImageFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox PartType_comboBox;
        private System.Windows.Forms.Button Go_button;
        private System.Windows.Forms.Button Select_button;
    }
}