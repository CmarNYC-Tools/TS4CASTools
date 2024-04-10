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
using System.IO;
using Ookii.Dialogs.Wpf;

namespace XMODS
{
    public partial class CreatorPrompt : Form
    {
        public CreatorPrompt()
        {
            InitializeComponent();
        }

        public CreatorPrompt(string myName, string path, string userpath, uint CASPUpdate)
        {
            InitializeComponent();
            CreatorName.Text = myName;
            TS4PathString.Text = path;
            TS4UserPathString.Text = userpath;
            if (CASPUpdate == 0) Prompt_radioButton.Checked = true;
            else if (CASPUpdate == 1) Auto_radioButton.Checked = true;
            else NoUpdate_radioButton.Checked = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.Compare(CreatorName.Text, " ") > 0)
            {
                Properties.Settings.Default.Creator = CreatorName.Text;
            }
            else
            {
                Properties.Settings.Default.Creator = "anon";
            }
            Properties.Settings.Default.TS4Path = TS4PathString.Text;
            Properties.Settings.Default.TS4UserPath = TS4UserPathString.Text;
            if (Prompt_radioButton.Checked) Properties.Settings.Default.CASPupdateOption = 0;
            else if (Auto_radioButton.Checked) Properties.Settings.Default.CASPupdateOption = 1;
            else Properties.Settings.Default.CASPupdateOption = 2;
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void Folder_button_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog findFolder = new VistaFolderBrowserDialog();
            findFolder.ShowNewFolderButton = false;
            findFolder.Description = "Select the folder where your game packages are located";
            findFolder.UseDescriptionForTitle = true;
            findFolder.SelectedPath = TS4PathString.Text;
            var res = findFolder.ShowDialog();
            if (res == true)
            {
                TS4PathString.Text = findFolder.SelectedPath;
            }
        }

        private void Folder_button2_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog findFolder = new VistaFolderBrowserDialog();
            findFolder.ShowNewFolderButton = false;
            findFolder.Description = "Select the folder where your Sims 4 user files are located";
            findFolder.UseDescriptionForTitle = true;
            findFolder.SelectedPath = TS4UserPathString.Text;
            var res = findFolder.ShowDialog();
            if (res == true)
            {
                TS4UserPathString.Text = findFolder.SelectedPath;
            }
        }
    }
}
