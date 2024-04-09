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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Xmods.DataLib;

namespace XMODS
{
    public partial class FNVhashForm : Form
    {
        public FNVhashForm()
        {
            InitializeComponent();
        }

        private void FNVhashButton_Click(object sender, EventArgs e)
        {
            FNV24hexStr.Text = Convert.ToString(FNVhash.FNV24(hashStr.Text), 16).ToUpper().PadLeft(8,'0');
            FNV32hexStr.Text = Convert.ToString(FNVhash.FNV32(hashStr.Text), 16).ToUpper().PadLeft(8, '0');
            ulong tmp = FNVhash.FNV64(hashStr.Text);
            uint low64, hi64;
            unchecked
            {
                low64 = (uint)tmp;
                hi64 = (uint)(tmp >> 32);
            }
            FNV64hexStr.Text = Convert.ToString(hi64, 16).ToUpper().PadLeft(8, '0') + Convert.ToString(low64, 16).ToUpper().PadLeft(8, '0');
        }

        private void Copy24_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(FNV24hexStr.Text);
        }

        private void Copy32_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(FNV32hexStr.Text);
        }

        private void Copy64_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(FNV64hexStr.Text);
        }

    }
}
