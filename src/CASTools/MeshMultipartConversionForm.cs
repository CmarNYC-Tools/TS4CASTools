/* TS4 CAS Mesh Tools, a tool for creating custom content for The Sims 4,
    Copyright (C) 2014  C. Marinetti

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>. 
    The author may be contacted at modthesims.info, username cmarNYC. */

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
    public partial class MeshMultipartConversionForm : Form
    {
        public bool ConvertAllToOne
        {
            get { return MeshMultipartAll2One_radioButton.Checked; }
        }

        public MeshMultipartConversionForm(bool setTrue)
        {
            InitializeComponent();
            MeshMultipartAll2One_radioButton.Checked = setTrue;
            MeshMultipartOne2One_radioButton.Checked = !setTrue;
        }

        private void MeshMultipartGo_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void MeshMultipartCancel_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
