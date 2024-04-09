using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XMODS
{
    public partial class RenumberSortOrder : Form
    {
        public int RenumberValue;
        public int RenumberIncrement;

        public RenumberSortOrder()
        {
            InitializeComponent();
        }

        private void Renumber_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (Renumber_radioButton.Checked) SetOptions();
        }

        private void Add_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (Add_radioButton.Checked) SetOptions();
        }

        private void SetOptions()
        {
            if (Renumber_radioButton.Checked)
            {
                label1.Text = "Start value:";
                label1.Visible = true;
                Value.Visible = true;
                label2.Visible = true;
                Increment.Visible = true;
            }
            if (Add_radioButton.Checked)
            {
                label1.Text = "Value:";
                label1.Visible = true;
                Value.Visible = true;
                label2.Visible = false;
                Increment.Visible = false;
            }
            Go_button.Visible = true;
        }

        private void Go_button_Click(object sender, EventArgs e)
        {
            if ((!Int32.TryParse(Value.Text, out RenumberValue)) || (Math.Abs(RenumberValue) > UInt16.MaxValue))
            {
                MessageBox.Show("Please enter a valid number for the value!");
                return;
            }
            if (Renumber_radioButton.Checked)
            {
                if (!Int32.TryParse(Increment.Text, out RenumberIncrement) || Math.Abs(RenumberIncrement) > UInt16.MaxValue || RenumberIncrement == 0)
                {
                    MessageBox.Show("Please enter a valid number for the increment!");
                    return;
                }
            }
            else { RenumberIncrement = 0; }
            this.DialogResult = DialogResult.OK;
        }

        private void Cancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
