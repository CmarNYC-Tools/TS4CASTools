namespace XMODS
{
    partial class RenumberSortOrder
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
            this.Renumber_radioButton = new System.Windows.Forms.RadioButton();
            this.Add_radioButton = new System.Windows.Forms.RadioButton();
            this.Value = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Increment = new System.Windows.Forms.TextBox();
            this.Go_button = new System.Windows.Forms.Button();
            this.Cancel_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Renumber_radioButton
            // 
            this.Renumber_radioButton.AutoSize = true;
            this.Renumber_radioButton.Location = new System.Drawing.Point(23, 28);
            this.Renumber_radioButton.Name = "Renumber_radioButton";
            this.Renumber_radioButton.Size = new System.Drawing.Size(224, 17);
            this.Renumber_radioButton.TabIndex = 0;
            this.Renumber_radioButton.TabStop = true;
            this.Renumber_radioButton.Text = "Renumber using start value and increment";
            this.Renumber_radioButton.UseVisualStyleBackColor = true;
            this.Renumber_radioButton.CheckedChanged += new System.EventHandler(this.Renumber_radioButton_CheckedChanged);
            // 
            // Add_radioButton
            // 
            this.Add_radioButton.AutoSize = true;
            this.Add_radioButton.Location = new System.Drawing.Point(23, 52);
            this.Add_radioButton.Name = "Add_radioButton";
            this.Add_radioButton.Size = new System.Drawing.Size(159, 17);
            this.Add_radioButton.TabIndex = 1;
            this.Add_radioButton.TabStop = true;
            this.Add_radioButton.Text = "Add a value to all sort orders";
            this.Add_radioButton.UseVisualStyleBackColor = true;
            this.Add_radioButton.CheckedChanged += new System.EventHandler(this.Add_radioButton_CheckedChanged);
            // 
            // Value
            // 
            this.Value.Location = new System.Drawing.Point(87, 102);
            this.Value.Name = "Value";
            this.Value.Size = new System.Drawing.Size(100, 20);
            this.Value.TabIndex = 2;
            this.Value.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Start value:";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Increment:";
            this.label2.Visible = false;
            // 
            // Increment
            // 
            this.Increment.Location = new System.Drawing.Point(87, 134);
            this.Increment.Name = "Increment";
            this.Increment.Size = new System.Drawing.Size(100, 20);
            this.Increment.TabIndex = 5;
            this.Increment.Visible = false;
            // 
            // Go_button
            // 
            this.Go_button.Location = new System.Drawing.Point(87, 173);
            this.Go_button.Name = "Go_button";
            this.Go_button.Size = new System.Drawing.Size(100, 34);
            this.Go_button.TabIndex = 6;
            this.Go_button.Text = "Renumber";
            this.Go_button.UseVisualStyleBackColor = true;
            this.Go_button.Visible = false;
            this.Go_button.Click += new System.EventHandler(this.Go_button_Click);
            // 
            // Cancel_button
            // 
            this.Cancel_button.Location = new System.Drawing.Point(87, 213);
            this.Cancel_button.Name = "Cancel_button";
            this.Cancel_button.Size = new System.Drawing.Size(100, 34);
            this.Cancel_button.TabIndex = 7;
            this.Cancel_button.Text = "Cancel";
            this.Cancel_button.UseVisualStyleBackColor = true;
            this.Cancel_button.Click += new System.EventHandler(this.Cancel_button_Click);
            // 
            // RenumberSortOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.Cancel_button);
            this.Controls.Add(this.Go_button);
            this.Controls.Add(this.Increment);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Value);
            this.Controls.Add(this.Add_radioButton);
            this.Controls.Add(this.Renumber_radioButton);
            this.Name = "RenumberSortOrder";
            this.Text = "RenumberSortOrder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton Renumber_radioButton;
        private System.Windows.Forms.RadioButton Add_radioButton;
        private System.Windows.Forms.TextBox Value;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Increment;
        private System.Windows.Forms.Button Go_button;
        private System.Windows.Forms.Button Cancel_button;

    }
}