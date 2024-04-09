using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace Xmods.DataLib
{
    public class Sims3pack
    {
        int strLength;
        string signature;
        ushort unknown;
        int xmlLength;
        string xmlDoc;
        PackageData[] packs;

        public int numberPackages
        {
            get { return this.packs.Length; }
        }

        public XmodsEnums.ClothingType getCASPtype(int sequence)
        {
            XmlDocument imp = new XmlDocument();
            imp.LoadXml(this.packs[sequence].MetaTags);
            XmlNodeList nodes = imp.GetElementsByTagName("bodypart");
            if (nodes.Count > 0)
            {
                return (XmodsEnums.ClothingType)Int32.Parse(nodes[0].InnerXml, System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                return XmodsEnums.ClothingType.None;
            }
        }

        public XmodsEnums.Age getCASPage(int sequence)
        {
            XmlDocument imp = new XmlDocument();
            imp.LoadXml(this.packs[sequence].MetaTags);
            XmlNodeList nodes = imp.GetElementsByTagName("agegenderflags");
            if (nodes.Count > 0)
            {
                return (XmodsEnums.Age)Int32.Parse(nodes[0].InnerXml.Substring(8, 2), System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                return XmodsEnums.Age.Undefined;
            }
        }

        public XmodsEnums.Gender getCASPgender(int sequence)
        {
            XmlDocument imp = new XmlDocument();
            imp.LoadXml(this.packs[sequence].MetaTags);
            XmlNodeList nodes = imp.GetElementsByTagName("agegenderflags");
            if (nodes.Count > 0)
            {
                return (XmodsEnums.Gender)Int32.Parse(nodes[0].InnerXml.Substring(6, 1), System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                return XmodsEnums.Gender.Undefined;
            }
        }

        public XmodsEnums.Species getCASPspecies(int sequence)
        {
            XmlDocument imp = new XmlDocument();
            imp.LoadXml(this.packs[sequence].MetaTags);
            XmlNodeList nodes = imp.GetElementsByTagName("agegenderflags");
            if (nodes.Count > 0)
            {
                return (XmodsEnums.Species)Int32.Parse(nodes[0].InnerXml.Substring(7, 1), System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                return XmodsEnums.Species.None;
            }
        }

        public byte[] getPackage(int sequence)
        {
            return this.packs[sequence].PackageArray; 
        }

        public Sims3pack(BinaryReader br)
        {
            this.strLength = br.ReadInt32();
            this.signature = new string(br.ReadChars(strLength));
            this.unknown = br.ReadUInt16();
            this.xmlLength = br.ReadInt32();
            this.xmlDoc = new string(br.ReadChars(xmlLength));
            MessageBox.Show(xmlDoc);
            char[] arr = xmlDoc.ToCharArray();
            arr = Array.FindAll<char>(arr, (c => (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c) || c == '-' || c == '_' || c == '"' || c == '<' || c == '>' || c == '/' || c == '=')));
            xmlDoc = new string(arr);
            XmlDocument imp = new XmlDocument();
            MessageBox.Show(xmlDoc);
            imp.LoadXml(xmlDoc);
            XmlNodeList nodes = imp.GetElementsByTagName("PackagedFile");
            packs = new PackageData[nodes.Count];
            int counter = 0;
            long archiveStart = br.BaseStream.Position;
            foreach (XmlNode n in nodes)
            {
                string[] paramList = new string[8] { "", "", "", "", "", "", "", "" };
                XmlNodeList pnodes = n.ChildNodes;
                foreach (XmlNode p in pnodes)
                {
                    if (String.CompareOrdinal(p.Name, "Name") == 0)
                    {
                        if (Path.GetExtension(p.InnerXml) != "package") continue;
                        paramList[0] = p.InnerXml;
                    }
                    else if (String.CompareOrdinal(p.Name, "Length") == 0) paramList[1] = p.InnerXml;
                    else if (String.CompareOrdinal(p.Name, "Offset") == 0) paramList[2] = p.InnerXml;
                    else if (String.CompareOrdinal(p.Name, "Crc") == 0) paramList[3] = p.InnerXml;
                    else if (String.CompareOrdinal(p.Name, "Guid") == 0) paramList[4] = p.InnerXml;
                    else if (String.CompareOrdinal(p.Name, "ContentType") == 0) paramList[5] = p.InnerXml;
                    else if (String.CompareOrdinal(p.Name, "metatags") == 0) paramList[6] = p.InnerXml;
                    else if (String.CompareOrdinal(p.Name, "EPFlags") == 0) paramList[7] = p.InnerXml;
                }
                packs[counter] = new PackageData(br, paramList, archiveStart);
            }
        }

        public Sims3pack(string packName, string packDesc, string packType, string metaTags, byte[] packData)
        {
        }

        internal class PackageData
        {
            string fileName;
            int fileLength;
            long offset;
            string crc;
            string guid;
            string contentType;
            string metaTags;
            string EPFlags;
            byte[] packageData;

            internal string MetaTags
            {
                get { return this.metaTags; }
            }
            internal byte[] PackageArray
            {
                get { return this.packageData; }
            }

            internal PackageData(BinaryReader br, string[] paramList, long archiveStart)
            {
                this.fileName = paramList[0];
                this.fileLength = Int32.Parse(paramList[1]);
                this.offset = Int64.Parse(paramList[2]);
                this.crc = paramList[3];
                this.guid = paramList[4];
                this.contentType = paramList[5];
                this.metaTags = paramList[6];
                this.EPFlags = paramList[7];
                br.BaseStream.Position = archiveStart + offset;
                this.packageData = br.ReadBytes(fileLength);
            }
        }
    }
}
