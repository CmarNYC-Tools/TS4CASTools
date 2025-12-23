namespace XMODS
{
    partial class MeshMultipartConversionForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.MeshMultipartOne2One_radioButton = new System.Windows.Forms.RadioButton();
            this.MeshMultipartAll2One_radioButton = new System.Windows.Forms.RadioButton();
            this.MeshMultipartGo_button = new System.Windows.Forms.Button();
            this.MeshMultipartCancel_button = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(112, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(340, 68);
            this.label1.TabIndex = 0;
            this.label1.Text = "This mesh contains multiple groups/objects. \r\nDo you want to convert them to ONE " +
    "TS mesh\r\n(as with hair mesh layers) or do you want to \r\nconvert each group to a " +
    "separate TS4 mesh?";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.MeshMultipartOne2One_radioButton);
            this.panel1.Controls.Add(this.MeshMultipartAll2One_radioButton);
            this.panel1.Location = new System.Drawing.Point(115, 114);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(337, 94);
            this.panel1.TabIndex = 1;
            // 
            // MeshMultipartOne2One_radioButton
            // 
            this.MeshMultipartOne2One_radioButton.AutoSize = true;
            this.MeshMultipartOne2One_radioButton.Location = new System.Drawing.Point(0, 41);
            this.MeshMultipartOne2One_radioButton.Name = "MeshMultipartOne2One_radioButton";
            this.MeshMultipartOne2One_radioButton.Size = new System.Drawing.Size(310, 21);
            this.MeshMultipartOne2One_radioButton.TabIndex = 1;
            this.MeshMultipartOne2One_radioButton.TabStop = true;
            this.MeshMultipartOne2One_radioButton.Text = "Convert each group to a separate TS4 mesh";
            this.MeshMultipartOne2One_radioButton.UseVisualStyleBackColor = true;
            // 
            // MeshMultipartAll2One_radioButton
            // 
            this.MeshMultipartAll2One_radioButton.AutoSize = true;
            this.MeshMultipartAll2One_radioButton.Location = new System.Drawing.Point(0, 14);
            this.MeshMultipartAll2One_radioButton.Name = "MeshMultipartAll2One_radioButton";
            this.MeshMultipartAll2One_radioButton.Size = new System.Drawing.Size(256, 21);
            this.MeshMultipartAll2One_radioButton.TabIndex = 0;
            this.MeshMultipartAll2One_radioButton.TabStop = true;
            this.MeshMultipartAll2One_radioButton.Text = "Convert all groups to one TS4 mesh";
            this.MeshMultipartAll2One_radioButton.UseVisualStyleBackColor = true;
            // 
            // MeshMultipartGo_button
            // 
            this.MeshMultipartGo_button.Location = new System.Drawing.Point(212, 214);
            this.MeshMultipartGo_button.Name = "MeshMultipartGo_button";
            this.MeshMultipartGo_button.Size = new System.Drawing.Size(141, 44);
            this.MeshMultipartGo_button.TabIndex = 2;
            this.MeshMultipartGo_button.Text = "Convert!";
            this.MeshMultipartGo_button.UseVisualStyleBackColor = true;
            this.MeshMultipartGo_button.Click += new System.EventHandler(this.MeshMultipartGo_button_Click);
            // 
            // MeshMultipartCancel_button
            // 
            this.MeshMultipartCancel_button.Location = new System.Drawing.Point(212, 273);
            this.MeshMultipartCancel_button.Name = "MeshMultipartCancel_button";
            this.MeshMultipartCancel_button.Size = new System.Drawing.Size(141, 44);
            this.MeshMultipartCancel_button.TabIndex = 3;
            this.MeshMultipartCancel_button.Text = "Cancel";
            this.MeshMultipartCancel_button.UseVisualStyleBackColor = true;
            this.MeshMultipartCancel_button.Click += new System.EventHandler(this.MeshMultipartCancel_button_Click);
            // 
            // MeshMultipartConversionForm
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(565, 329);
            this.Controls.Add(this.MeshMultipartCancel_button);
            this.Controls.Add(this.MeshMultipartGo_button);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Name = "MeshMultipartConversionForm";
            this.Text = "Multipart Mesh Conversion Options";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton MeshMultipartOne2One_radioButton;
        private System.Windows.Forms.RadioButton MeshMultipartAll2One_radioButton;
        private System.Windows.Forms.Button MeshMultipartGo_button;
        private System.Windows.Forms.Button MeshMultipartCancel_button;
    }
}