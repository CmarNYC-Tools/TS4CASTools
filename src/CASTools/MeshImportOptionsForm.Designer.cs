namespace XMODS
{
    partial class MeshImportOptionsForm
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
            this.meshImportOptionsDefaultRef_radioButton = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.meshImportOptionsDefaultMale_radioButton = new System.Windows.Forms.RadioButton();
            this.meshImportOptionsDefaultFemale_radioButton = new System.Windows.Forms.RadioButton();
            this.meshImportOptionsDefaultChild_radioButton = new System.Windows.Forms.RadioButton();
            this.meshImportOptionsSelectRef_radioButton = new System.Windows.Forms.RadioButton();
            this.meshImportOptionsReferenceFilename = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.meshImportOptionsNoRef_radioButton = new System.Windows.Forms.RadioButton();
            this.meshImportOptionsGo_button = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // meshImportOptionsDefaultRef_radioButton
            // 
            this.meshImportOptionsDefaultRef_radioButton.AutoSize = true;
            this.meshImportOptionsDefaultRef_radioButton.Checked = true;
            this.meshImportOptionsDefaultRef_radioButton.Location = new System.Drawing.Point(6, 32);
            this.meshImportOptionsDefaultRef_radioButton.Name = "meshImportOptionsDefaultRef_radioButton";
            this.meshImportOptionsDefaultRef_radioButton.Size = new System.Drawing.Size(266, 21);
            this.meshImportOptionsDefaultRef_radioButton.TabIndex = 0;
            this.meshImportOptionsDefaultRef_radioButton.TabStop = true;
            this.meshImportOptionsDefaultRef_radioButton.Text = "Use default body mesh as reference: ";
            this.meshImportOptionsDefaultRef_radioButton.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.meshImportOptionsDefaultChild_radioButton);
            this.panel2.Controls.Add(this.meshImportOptionsDefaultFemale_radioButton);
            this.panel2.Controls.Add(this.meshImportOptionsDefaultMale_radioButton);
            this.panel2.Location = new System.Drawing.Point(278, 31);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(382, 35);
            this.panel2.TabIndex = 1;
            // 
            // meshImportOptionsDefaultMale_radioButton
            // 
            this.meshImportOptionsDefaultMale_radioButton.AutoSize = true;
            this.meshImportOptionsDefaultMale_radioButton.Location = new System.Drawing.Point(3, 3);
            this.meshImportOptionsDefaultMale_radioButton.Name = "meshImportOptionsDefaultMale_radioButton";
            this.meshImportOptionsDefaultMale_radioButton.Size = new System.Drawing.Size(59, 21);
            this.meshImportOptionsDefaultMale_radioButton.TabIndex = 0;
            this.meshImportOptionsDefaultMale_radioButton.TabStop = true;
            this.meshImportOptionsDefaultMale_radioButton.Text = "Male";
            this.meshImportOptionsDefaultMale_radioButton.UseVisualStyleBackColor = true;
            // 
            // meshImportOptionsDefaultFemale_radioButton
            // 
            this.meshImportOptionsDefaultFemale_radioButton.AutoSize = true;
            this.meshImportOptionsDefaultFemale_radioButton.Location = new System.Drawing.Point(83, 3);
            this.meshImportOptionsDefaultFemale_radioButton.Name = "meshImportOptionsDefaultFemale_radioButton";
            this.meshImportOptionsDefaultFemale_radioButton.Size = new System.Drawing.Size(75, 21);
            this.meshImportOptionsDefaultFemale_radioButton.TabIndex = 1;
            this.meshImportOptionsDefaultFemale_radioButton.TabStop = true;
            this.meshImportOptionsDefaultFemale_radioButton.Text = "Female";
            this.meshImportOptionsDefaultFemale_radioButton.UseVisualStyleBackColor = true;
            // 
            // meshImportOptionsDefaultChild_radioButton
            // 
            this.meshImportOptionsDefaultChild_radioButton.AutoSize = true;
            this.meshImportOptionsDefaultChild_radioButton.Location = new System.Drawing.Point(179, 3);
            this.meshImportOptionsDefaultChild_radioButton.Name = "meshImportOptionsDefaultChild_radioButton";
            this.meshImportOptionsDefaultChild_radioButton.Size = new System.Drawing.Size(60, 21);
            this.meshImportOptionsDefaultChild_radioButton.TabIndex = 2;
            this.meshImportOptionsDefaultChild_radioButton.TabStop = true;
            this.meshImportOptionsDefaultChild_radioButton.Text = "Child";
            this.meshImportOptionsDefaultChild_radioButton.UseVisualStyleBackColor = true;
            // 
            // meshImportOptionsSelectRef_radioButton
            // 
            this.meshImportOptionsSelectRef_radioButton.AutoSize = true;
            this.meshImportOptionsSelectRef_radioButton.Location = new System.Drawing.Point(6, 78);
            this.meshImportOptionsSelectRef_radioButton.Name = "meshImportOptionsSelectRef_radioButton";
            this.meshImportOptionsSelectRef_radioButton.Size = new System.Drawing.Size(175, 21);
            this.meshImportOptionsSelectRef_radioButton.TabIndex = 2;
            this.meshImportOptionsSelectRef_radioButton.Text = "Select reference mesh:";
            this.meshImportOptionsSelectRef_radioButton.UseVisualStyleBackColor = true;
            // 
            // meshImportOptionsReferenceFilename
            // 
            this.meshImportOptionsReferenceFilename.Location = new System.Drawing.Point(187, 77);
            this.meshImportOptionsReferenceFilename.Name = "meshImportOptionsReferenceFilename";
            this.meshImportOptionsReferenceFilename.Size = new System.Drawing.Size(473, 22);
            this.meshImportOptionsReferenceFilename.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.meshImportOptionsNoRef_radioButton);
            this.groupBox1.Controls.Add(this.meshImportOptionsReferenceFilename);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Controls.Add(this.meshImportOptionsSelectRef_radioButton);
            this.groupBox1.Controls.Add(this.meshImportOptionsDefaultRef_radioButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 26);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(666, 157);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Reference Mesh Options:";
            // 
            // meshImportOptionsNoRef_radioButton
            // 
            this.meshImportOptionsNoRef_radioButton.AutoSize = true;
            this.meshImportOptionsNoRef_radioButton.Location = new System.Drawing.Point(6, 124);
            this.meshImportOptionsNoRef_radioButton.Name = "meshImportOptionsNoRef_radioButton";
            this.meshImportOptionsNoRef_radioButton.Size = new System.Drawing.Size(294, 21);
            this.meshImportOptionsNoRef_radioButton.TabIndex = 4;
            this.meshImportOptionsNoRef_radioButton.Text = "Do not use a reference mesh (no morphs)";
            this.meshImportOptionsNoRef_radioButton.UseVisualStyleBackColor = true;
            // 
            // meshImportOptionsGo_button
            // 
            this.meshImportOptionsGo_button.Location = new System.Drawing.Point(257, 211);
            this.meshImportOptionsGo_button.Name = "meshImportOptionsGo_button";
            this.meshImportOptionsGo_button.Size = new System.Drawing.Size(185, 40);
            this.meshImportOptionsGo_button.TabIndex = 2;
            this.meshImportOptionsGo_button.Text = "Import Mesh";
            this.meshImportOptionsGo_button.UseVisualStyleBackColor = true;
            // 
            // MeshImportOptionsForm
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 276);
            this.Controls.Add(this.meshImportOptionsGo_button);
            this.Controls.Add(this.groupBox1);
            this.Name = "MeshImportOptionsForm";
            this.Text = "MeshImportOptionsForm";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton meshImportOptionsDefaultRef_radioButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton meshImportOptionsDefaultChild_radioButton;
        private System.Windows.Forms.RadioButton meshImportOptionsDefaultFemale_radioButton;
        private System.Windows.Forms.RadioButton meshImportOptionsDefaultMale_radioButton;
        private System.Windows.Forms.RadioButton meshImportOptionsSelectRef_radioButton;
        private System.Windows.Forms.TextBox meshImportOptionsReferenceFilename;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton meshImportOptionsNoRef_radioButton;
        private System.Windows.Forms.Button meshImportOptionsGo_button;
    }
}