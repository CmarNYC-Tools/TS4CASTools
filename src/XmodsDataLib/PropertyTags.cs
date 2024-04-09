/* Xmods Data Library, a library to support tools for The Sims 4,
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
   The author may be contacted at modthesims.info, username cmarNYC.
   
   Example code provided by Buzzler */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace Xmods.DataLib
{
    public class PropertyTags
    {
        private const string CatalogTuningFileName = "S4_03B33DDF_00000000_D89CB9186B79ACB7.xml";

		public static List<uint> tagCategory;
        public static List<string> tagCategoryString;
		public static List<uint> tag;
        public static List<string> tagString;

		static PropertyTags()
		{
            tagCategory = new List<uint>();
            tagCategoryString = new List<string>();
            tag = new List<uint>();
            tagString = new List<string>();
			ParseCategories();
		}

		private static void ParseCategories()
		{
			string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string resourcePath = Path.Combine(executingPath, CatalogTuningFileName);
            if (!File.Exists(resourcePath))
			{
                MessageBox.Show(string.Format("'{0}' not found in CAS Tools directory '{1}'; property tags cannot be loaded.", CatalogTuningFileName, executingPath));
                return;
			}

            XmlTextReader reader = new XmlTextReader(resourcePath);
            string Type = "";
            uint val = 0;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        string s = reader.GetAttribute("n");
                        if (s != null && s.StartsWith("Tag"))
                        {
                            Type = s;
                        }
                        else if (String.Compare(reader.Name, "T") == 0)
                        {
                            val = UInt32.Parse(reader.GetAttribute("ev"));
                        }
                        break;
                    case XmlNodeType.Text:
                        if (String.Compare(Type, "TagCategory") == 0)
                        {
                            tagCategory.Add(val);
                            tagCategoryString.Add(reader.Value.Replace("'", "").Replace("-", "_"));
                        }
                        else if (String.Compare(Type, "Tag") == 0)
                        {
                            tag.Add(val);
                            tagString.Add(reader.Value.Replace("'", "").Replace("-", "_"));
                        }
                        break;
                }
            }
		}

        public static void ParseSpecialCategories(string tagsFilename, out string[] tagCategoryNames, out uint[] tagCategoryValues)
        {
            string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string resourcePath = Path.Combine(executingPath, tagsFilename);
            if (!File.Exists(resourcePath))
            {
                MessageBox.Show(string.Format("'{0}' not found in program directory '{1}'; property tags cannot be loaded.", tagsFilename, executingPath));
                tagCategoryNames = new string[0];
                tagCategoryValues = new uint[0];
                return;
            }

            XmlTextReader reader = new XmlTextReader(resourcePath);
            string Type = "";
            uint val = 0;
            List<string> catNames = new List<string>();
            List<uint> catValues = new List<uint>();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        string s = reader.GetAttribute("n");
                        if (s != null && s.StartsWith("Tag"))
                        {
                            Type = s;
                        }
                        else if (String.Compare(reader.Name, "T") == 0)
                        {
                            val = UInt32.Parse(reader.GetAttribute("ev"));
                        }
                        break;
                    case XmlNodeType.Text:
                        if (String.Compare(Type, "TagCategory") == 0)
                        {
                            catNames.Add(reader.Value.Replace("'", "").Replace("-", "_"));
                            catValues.Add(val);
                        }
                        break;
                }
            }
            tagCategoryNames = catNames.ToArray();
            tagCategoryValues = catValues.ToArray();
            return;
        }
    }
}
