namespace XMODS
{
    partial class CreatorPrompt
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
            this.CreatorName = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TS4PathString = new System.Windows.Forms.TextBox();
            this.Folder_button = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Folder_button2 = new System.Windows.Forms.Button();
            this.TS4UserPathString = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Prompt_radioButton = new System.Windows.Forms.RadioButton();
            this.Auto_radioButton = new System.Windows.Forms.RadioButton();
            this.NoUpdate_radioButton = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Your creator name: ";
            // 
            // CreatorName
            // 
            this.CreatorName.Location = new System.Drawing.Point(127, 15);
            this.CreatorName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CreatorName.Name = "CreatorName";
            this.CreatorName.Size = new System.Drawing.Size(113, 20);
            this.CreatorName.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(219, 249);
            this.button1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 25);
            this.button1.TabIndex = 2;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 53);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 26);
            this.label2.TabIndex = 3;
            this.label2.Text = "Location of TS4 game\r\ninstallation directory:";
            // 
            // TS4PathString
            // 
            this.TS4PathString.Location = new System.Drawing.Point(127, 58);
            this.TS4PathString.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TS4PathString.Name = "TS4PathString";
            this.TS4PathString.Size = new System.Drawing.Size(287, 20);
            this.TS4PathString.TabIndex = 4;
            // 
            // Folder_button
            // 
            this.Folder_button.Location = new System.Drawing.Point(417, 53);
            this.Folder_button.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Folder_button.Name = "Folder_button";
            this.Folder_button.Size = new System.Drawing.Size(70, 29);
            this.Folder_button.TabIndex = 5;
            this.Folder_button.Text = "Select";
            this.Folder_button.UseVisualStyleBackColor = true;
            this.Folder_button.Click += new System.EventHandler(this.Folder_button_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(127, 82);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(255, 26);
            this.label3.TabIndex = 6;
            this.label3.Text = "Select the Sims 4 game top folder, not a subfolder!\r\nEx: C:\\Program Files (x86)\\O" +
    "rigin Games\\The Sims 4";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(127, 143);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(302, 39);
            this.label4.TabIndex = 10;
            this.label4.Text = "Select the folder for The Sims 4 in your Documents directory!\r\nThis is the same p" +
    "lace where your Mods folder is located.\r\nEx: C:\\users\\MyName\\Documents\\Electroni" +
    "c Arts\\The Sims 4";
            // 
            // Folder_button2
            // 
            this.Folder_button2.Location = new System.Drawing.Point(417, 114);
            this.Folder_button2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Folder_button2.Name = "Folder_button2";
            this.Folder_button2.Size = new System.Drawing.Size(70, 29);
            this.Folder_button2.TabIndex = 9;
            this.Folder_button2.Text = "Select";
            this.Folder_button2.UseVisualStyleBackColor = true;
            this.Folder_button2.Click += new System.EventHandler(this.Folder_button2_Click);
            // 
            // TS4UserPathString
            // 
            this.TS4UserPathString.Location = new System.Drawing.Point(127, 119);
            this.TS4UserPathString.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TS4UserPathString.Name = "TS4UserPathString";
            this.TS4UserPathString.Size = new System.Drawing.Size(287, 20);
            this.TS4UserPathString.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 114);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 26);
            this.label5.TabIndex = 7;
            this.label5.Text = "Location of TS4 user\r\nDocuments directory:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 204);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 26);
            this.label6.TabIndex = 11;
            this.label6.Text = "Update old CASPs\r\nto latest version:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.NoUpdate_radioButton);
            this.panel1.Controls.Add(this.Auto_radioButton);
            this.panel1.Controls.Add(this.Prompt_radioButton);
            this.panel1.Location = new System.Drawing.Point(112, 204);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(375, 35);
            this.panel1.TabIndex = 12;
            // 
            // Prompt_radioButton
            // 
            this.Prompt_radioButton.AutoSize = true;
            this.Prompt_radioButton.Location = new System.Drawing.Point(15, 3);
            this.Prompt_radioButton.Name = "Prompt_radioButton";
            this.Prompt_radioButton.Size = new System.Drawing.Size(58, 17);
            this.Prompt_radioButton.TabIndex = 0;
            this.Prompt_radioButton.TabStop = true;
            this.Prompt_radioButton.Text = "Prompt";
            this.Prompt_radioButton.UseVisualStyleBackColor = true;
            // 
            // Auto_radioButton
            // 
            this.Auto_radioButton.AutoSize = true;
            this.Auto_radioButton.Location = new System.Drawing.Point(96, 3);
            this.Auto_radioButton.Name = "Auto_radioButton";
            this.Auto_radioButton.Size = new System.Drawing.Size(85, 17);
            this.Auto_radioButton.TabIndex = 1;
            this.Auto_radioButton.TabStop = true;
            this.Auto_radioButton.Text = "Auto Update";
            this.Auto_radioButton.UseVisualStyleBackColor = true;
            // 
            // NoUpdate_radioButton
            // 
            this.NoUpdate_radioButton.AutoSize = true;
            this.NoUpdate_radioButton.Location = new System.Drawing.Point(204, 3);
            this.NoUpdate_radioButton.Name = "NoUpdate_radioButton";
            this.NoUpdate_radioButton.Size = new System.Drawing.Size(97, 17);
            this.NoUpdate_radioButton.TabIndex = 2;
            this.NoUpdate_radioButton.TabStop = true;
            this.NoUpdate_radioButton.Text = "Do Not Update";
            this.NoUpdate_radioButton.UseVisualStyleBackColor = true;
            // 
            // CreatorPrompt
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 285);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Folder_button2);
            this.Controls.Add(this.TS4UserPathString);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Folder_button);
            this.Controls.Add(this.TS4PathString);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CreatorName);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "CreatorPrompt";
            this.Text = "TS4 Mesh Tools Setup";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox CreatorName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TS4PathString;
        private System.Windows.Forms.Button Folder_button;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Folder_button2;
        private System.Windows.Forms.TextBox TS4UserPathString;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton NoUpdate_radioButton;
        private System.Windows.Forms.RadioButton Auto_radioButton;
        private System.Windows.Forms.RadioButton Prompt_radioButton;
    }
}