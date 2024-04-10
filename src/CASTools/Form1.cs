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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Xmods.DataLib;
using s4pi.ImageResource;
using s4pi.Interfaces;
using s4pi.Package;

namespace XMODS
{
    public partial class Form1 : Form
    {
        string CASToolsVersion = "3.8.2.0";

        string GEOMfilter = "GEOM mesh files (*.simgeom, *.geom)|*.simgeom;*.geom|All files (*.*)|*.*";
        string Meshfilter = "Mesh files (*.ms3d, *.simgeom, *.geom, *.obj, *.dae)|*.ms3d;*.simgeom;*.geom;*.obj;*.dae|All files (*.*)|*.*";
        string OBJfilter = "OBJ files (*.obj)|*.obj|All files (*.*)|*.*";
        string Packagefilter = "Package files (*.package)|*.package|All files (*.*)|*.*";
        //string DDSfilter = "DDS image files (*.dds)|*.dds|All files (*.*)|*.*";
        string MS3Dfilter = "Milkshape MS3D files (*.ms3d)|*.ms3d|All files (*.*)|*.*";
        string DAEfilter = "Collada .dae files (*.dae)|*.dae|All files (*.*)|*.*";
        //string MultipartMeshExportFilter = "MS3D or OBJ mesh files (*.ms3d, *.obj)|*.ms3d; *.obj|All files (*.*)|*.*";
        //string AllMeshExportFilter = "TS4, TS3, MS3D, or OBJ mesh files (*.simgeom, *.geom, *.ms3d, *.obj)|*.simgeom; *.geom; *.ms3d; *.obj|All files (*.*)|*.*";
        string MultipartMeshImportFilter = "MS3D or DAE mesh files (*.ms3d, *.dae)|*.ms3d; *.dae|All files (*.*)|*.*";
        string AllMeshImportFilter = "TS4, TS3, MS3D, or DAE mesh files (*.simgeom, *.geom, *.ms3d, *.dae)|*.simgeom; *.geom; *.ms3d; *.dae|All files (*.*)|*.*";
        string ConvertMeshImportFilter = "TS3, MS3D, or DAE mesh files (*.simgeom, *.geom, *.ms3d, *.dae)|*.simgeom; *.geom; *.ms3d; *.dae|All files (*.*)|*.*";
        Package myPack, clonePack, testPack;
        List<Package> resourcePacks, thumbsPacks;
        Package[] gamePacks0, gamePacksOther, gameThumbPacks;
        string[] gamePacks0Names;
        ItemCollection[] CASitems;
        int itemIndex = 0;
        List<IResourceIndexEntry> foundResList = new List<IResourceIndexEntry>();
        List<string> bodyTypeNames;
        List<uint>[] bodyTypeValues;

        Dictionary<uint, string> unlockLevels = new Dictionary<uint, string>();
        
        DdsFile defaultCustomIcon = new DdsFile();

       // List<string> catNames = new List<string>(Enum.GetNames(typeof(XmodsEnums.CatFlags)));
       // List<uint> catNumbers = new List<uint>((uint[])Enum.GetValues(typeof(XmodsEnums.CatFlags)));
       // List<string> catValues = new List<string>(Enum.GetNames(typeof(XmodsEnums.CatFlagValues)));
       // List<uint> catValueNumbers = new List<uint>((uint[])Enum.GetValues(typeof(XmodsEnums.CatFlagValues)));
        List<string> tagCategoryNames;
        List<uint> tagCategoryNumbers;
        List<string> tagNames;
        List<uint> tagNumbers;
        string[] tagCategoryNames4All;
        uint[] tagCategoryNumbers4All;
        string[] tagCategoryNamesInd;
        uint[] tagCategoryNumbersInd;

        string[] AgeNames = new string[] { "Infant", "Toddler", "Child", "Teen", "YoungAdult", "Adult", "Elder" };
        uint[] AgeValues = new uint[] { 128, 2, 4, 8, 16, 32, 64 };
        string[] GenderNames = new string[] { "Male", "Female" };
        uint[] GenderValues = new uint[] { 1, 2 };

        string[] MorphNamesToddler = new string[] { "None", "puBody_Heavy", "puBody_Lean" };
        string[] MorphNamesChild = new string[] { "None", "cuBody_Heavy", "cuBody_Lean" };
        string[] MorphNamesAdultFemale = new string[] { "None", 
            "yfBody_Heavy", "yfBody_Lean", "yfBody_Fit", "yfBody_Bony", "yfBody_Male",
            "yfNeck_Thick", "yfNeck_Thin", "yfNeck_BackContract", "yfNeck_BackExtend", 
            "yfShoulders_Narrow", "yfShoulders_Wide", "yfChest_BreastsBig", "yfChest_BreastsSmall", 
            "yfChest_Droop", "yfChest_Lift", "yfChest_In", "yfChest_Out", 
            "yfUpperArm_Big", "yfUpperArm_Small", "yfLowerArm_Big", "yfLowerArm_Small", 
            "yfWaist_Narrow", "yfWaist_Wide", "yfBelly_Big", "yfBelly_Small", "yfHips_Narrow", "yfHips_Wide", 
            "yfButt_Big", "yfButt_Small", "yfThighs_Big", "yfThighs_Small", 
            "yfLowerLeg_Big", "yfLowerLeg_Small", 
            "yfFeet_Big", "yfFeet_Small", "yfFeet_Long", "yfFeet_Short", 
            "efBody_Average", "efBody_Heavy", "efBody_Lean", "efBody_Fit", "efBody_Bony", 
            "yuHead_Wide", "yuHead_Narrow" };
        string[] MorphNamesAdultMale = new string[] { "None", 
            "ymBody_Heavy", "ymBody_Lean", "ymBody_Fit", "ymBody_Bony", "ymBody_Female",
            "ymNeck_BackContract", "ymNeck_BackExtend", "ymNeck_Thick", "ymNeck_Thin",
            "ymShoulders_Narrow", "ymShoulders_Wide", "ymChest_In", "ymChest_Out", 
            "ymUpperArm_Big", "ymUpperArm_Small", "ymLowerArm_Big", "ymLowerArm_Small", 
            "ymWaist_Narrow", "ymWaist_Wide", "ymBelly_Big", "ymBelly_Small", "ymHips_Narrow", "ymHips_Wide", 
            "ymButt_Big", "ymButt_Small", "ymThighs_Big", "ymThighs_Small", "ymLowerLeg_Big", "ymLowerLeg_Small", 
            "ymFeet_Big", "ymFeet_Small","ymFeet_Long", "ymFeet_Short", 
            "emBody_Average", "emBody_Heavy", "emBody_Lean", "emBody_Fit", "emBody_Bony", 
            "yuHead_Wide", "yuHead_Narrow" };

        RIG infantRig;
        RIG adultRig;
        RIG childRig;
        RIG toddlerRig;
        RIG acRig;
        RIG ccRig;
        RIG adRig;
        RIG cdRig;
        RIG alRig;

        Dictionary<uint, GEOM.SlotRayData> ahSlotRayData, chSlotRayData, phSlotRayData, ihSlotRayData, adSlotRayData, acSlotRayData, alSlotRayData, cdSlotRayData, ccSlotRayData;

        Vector3 layerStartPoint = new Vector3(0f, 1.7f, -0.05f);

        Dictionary<string, string> PackNames;

        string appStartupPath;

        Random ran;

        public enum ImageType
        {
            BumpMap,
            GlowMap,
            Material,
            Shadow,
            Specular,
            Swatch,
            Thumbnail
        }

        public Form1()
        {
            InitializeComponent();

            this.Text = "TS4 CAS Tools v" + CASToolsVersion;
            tabControl2.TabPages.Remove(CLIPEvent_tabPage);
            CASTools_tabControl.TabPages.Remove(SpecialTools_tabPage);
          //  GEOMConvert_tabControl.TabPages.Remove(GEOMclean_tabPage);

            ProgramSetup();
        }

        private Dictionary<string, string> ReadPackNames()
        {
            string path = appStartupPath + "\\Packs.txt";
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            Dictionary<string, string> tmp = new Dictionary<string, string>();
            string line;
            while ((line = file.ReadLine()) != null)
            {
                tmp.Add(line.Substring(0, 4), line.Substring(5));
            }
            return tmp;
        }

        private void ProgramSetup()
        {
            bodyTypeNames = new List<string> { 
                "All", "Hat", "Hair", "Head", "Teeth", "Body", "Top", 
                "Bottom", "Shoes", "Earrings", "Glasses", "Necklace", "Gloves", 
                "Bracelet", "Piercing", "Ring", "Facial Hair", "Makeup", "Facepaint",
                "Eyebrows", "Eyecolor", "Socks", "Skin Details", 
                "Skin Overlay", "Tights", "Tattoo", "Fur", 
                "Animal Ears", "Tail", "Nose Color", "Secondary Eye Color", "Occult", 
                "Fingernails", "Toenails", "Body Hair", "Body Scar"
            };
            bodyTypeValues = new List<uint>[] {
            new List<uint> { 0 }, new List<uint> { 1 }, new List<uint> { 2 }, new List<uint> { 3 }, new List<uint> { 4 }, new List<uint> { 5 }, new List<uint> { 6 },
            new List<uint> { 7 }, new List<uint> { 8 }, new List<uint> { 0x0A }, new List<uint> { 0x0B }, new List<uint> { 0x0C }, new List<uint> { 0x0D },
            new List<uint> { 0x0E, 0x0F }, new List<uint> { 0x10, 0x11, 0x12, 0x13, 0x14, 0x15 }, new List<uint> { 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B }, 
            new List<uint> { 0x1C }, new List<uint> { 0x1D, 0x1E, 0x1F, 0x20, 0x25 },  new List<uint> { 0x21 }, 
            new List<uint> { 0x22 }, new List<uint> { 0x23 }, new List<uint> { 0x24 }, new List<uint> { 0x26, 0x27, 0x28, 0x29, 0x2B, 0x2C, 0x37, 0x38, 0x39, 0x47, 0x48 }, 
            new List<uint> { 0x3A }, new List<uint> { 0x2A }, new List<uint> { 0x2D, 0x2E, 0x2F, 0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36 }, new List<uint> { 0x3B },
            new List<uint> { 0x3C }, new List<uint> { 0x3D }, new List<uint> { 0x3E }, new List<uint> { 0x3F }, new List<uint> { 0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46 }, 
            new List<uint> { 0x49 }, new List<uint> { 0x4A }, new List<uint> { 0x4E, 0x4F, 0x50, 0x51 }, new List<uint> { 0x52, 0x53, 0x54, 0x55, 0x56, 0x57 }
            };

            try
            {
                if (String.Compare(Properties.Settings.Default.TS4Path, " ") <= 0)
                {
                    string tmp = (string)Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Maxis\\The Sims 4", "Install Dir", null);
                    if (tmp != null)
                    {
                        Properties.Settings.Default.TS4Path = tmp;
                        //MessageBox.Show(tmp);
                        Properties.Settings.Default.Save();
                    }
                }
                if (String.Compare(Properties.Settings.Default.TS4UserPath, " ") <= 0)
                {
                    string tmp = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Electronic Arts\\The Sims 4";
                    if (tmp != null)
                    {
                        if (!Directory.Exists(tmp))
                        {
                            string[] tmp2 = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Electronic Arts", "localthumbcache.package", SearchOption.AllDirectories);
                            if (tmp2.Length > 0)
                            {
                                tmp = Path.GetDirectoryName(tmp2[0]) + Path.DirectorySeparatorChar;
                            }
                        }
                        Properties.Settings.Default.TS4UserPath = tmp;
                        //MessageBox.Show(tmp);
                        Properties.Settings.Default.Save();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in detecting game and/or user file paths: " + e.Message + Environment.NewLine + e.StackTrace);
                Form f = new CreatorPrompt((String.Compare(Properties.Settings.Default.Creator, "anon") == 0) ? "" : Properties.Settings.Default.Creator, Properties.Settings.Default.TS4Path, Properties.Settings.Default.TS4UserPath, Properties.Settings.Default.CASPupdateOption);
                f.ShowDialog();
            }
            if (String.Compare(Properties.Settings.Default.Creator, "anon") == 0 | Properties.Settings.Default.TS4Path == null)
            {
                Form f = new CreatorPrompt((String.Compare(Properties.Settings.Default.Creator, "anon") == 0) ? "" : Properties.Settings.Default.Creator, Properties.Settings.Default.TS4Path, Properties.Settings.Default.TS4UserPath, Properties.Settings.Default.CASPupdateOption);
                f.ShowDialog();
            }

            try
            {
                tagCategoryNames = Xmods.DataLib.PropertyTags.tagCategoryString;
                tagCategoryNumbers = Xmods.DataLib.PropertyTags.tagCategory;
                tagNames = Xmods.DataLib.PropertyTags.tagString;
                tagNumbers = Xmods.DataLib.PropertyTags.tag;
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message);
                //  Application.Exit();
            }

            try
            {
                appStartupPath = Application.StartupPath;
            }
            catch (Exception e)
            {
                MessageBox.Show("Can't find startup path! Creating MS3D meshes will not work." + Environment.NewLine + e.Message);
            }

            tagCategoryNames4All = Enum.GetNames(typeof(XmodsEnums.CatFlagsForAll));
            tagCategoryNumbers4All = (uint[])Enum.GetValues(typeof(XmodsEnums.CatFlagsForAll));
            // tagCategoryNamesInd = Enum.GetNames(typeof(XmodsEnums.CatFlagsIndividual));
            // tagCategoryNumbersInd = (uint[])Enum.GetValues(typeof(XmodsEnums.CatFlagsIndividual));
            uint[] excludeNumbers = (uint[])Enum.GetValues(typeof(XmodsEnums.CatFlagsExclude));
            List<string> tmpNames = new List<string>();
            List<uint> tmpNumbers = new List<uint>();
            for (int i = 0; i < tagCategoryNumbers.Count; i++)
            {
                if (!tagCategoryNumbers4All.Contains(tagCategoryNumbers[i]) && !excludeNumbers.Contains(tagCategoryNumbers[i]))
                {
                    tmpNames.Add(tagCategoryNames[i]);
                    tmpNumbers.Add(tagCategoryNumbers[i]);
                }
            }
            tagCategoryNamesInd = tmpNames.ToArray();
            tagCategoryNumbersInd = tmpNumbers.ToArray();

            unlockLevels.Add(0, "Not Applicable");
            unlockLevels.Add(0x05C96583, "Purchase at ThrifTea's thrift store");
            unlockLevels.Add(0xFE4B8009, "Knitting Skill Level 1");
            unlockLevels.Add(0x27C1F576, "Knitting Skill Level 2");
            unlockLevels.Add(0x021FEAE5, "Knitting Skill Level 5");
            unlockLevels.Add(0xC15C92E5, "Knitting Skill Level 8");
            unlockLevels.Add(0xB6C28AFE, "Knitting Skill Level 9");
            unlockLevels.Add(0xB9EA8E02, "Sacred Knitting Knowledge");
            ClonePropCreate_comboBox.DataSource = new BindingSource(unlockLevels, null);
            ClonePropCreate_comboBox.DisplayMember = "Value";
            ClonePropCreate_comboBox.ValueMember = "Key";

            SpeciesFilter_comboBox.Items.Add("All");
            SpeciesFilter_comboBox.Items.AddRange(Enum.GetNames(typeof(XmodsEnums.Species)));
            SpeciesFilter_comboBox.SelectedIndex = 0;
            AgeFilter_comboBox.Items.Add("All");
            AgeFilter_comboBox.Items.AddRange(AgeNames);
            AgeFilter_comboBox.SelectedIndex = 0;
            GenderFilter_comboBox.Items.Add("All");
            GenderFilter_comboBox.Items.AddRange(GenderNames);
            GenderFilter_comboBox.SelectedIndex = 0;
            PartFilter_comboBox.Items.AddRange(bodyTypeNames.ToArray());
            PartFilter_comboBox.Items.Add("Other");
            PartFilter_comboBox.SelectedIndex = 0;
            HardColor_comboBox.Items.AddRange(Enum.GetNames(typeof(XmodsEnums.VertexColorStandard)));
            AutoPartType_comboBox.SelectedIndex = 0;
            AutoSpecies_comboBox.SelectedIndex = 0;
            AutoGender_comboBox.SelectedIndex = 0;
            AutoAge_comboBox.SelectedIndex = 2;
            AutoLod_comboBox.SelectedIndex = 0;
            ClonePropTexture_comboBox.SelectedIndex = 0;
            cloneRegion_comboBox.Items.AddRange(Enum.GetNames(typeof(XmodsEnums.CASPartRegionTS4)));
            SwatchList_dataGridView.Columns["SwatchImage"].DefaultCellStyle.NullValue = null;
            PackNames = ReadPackNames();
            if (SetupGamePacks())
            {
                PackFilter_comboBox.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("CAS Tools was unable to open the game packages; cloning cannot be done.");
            }

            MeshVertexColorStandard_comboBox.Items.AddRange(Enum.GetNames(typeof(XmodsEnums.VertexColorStandard)));
            SortCategories(tagCategoryNames4All, tagCategoryNumbers4All);
            SortCategories(tagCategoryNamesInd, tagCategoryNumbersInd);

            for (int i = 0; i < tagCategoryNames.Count; i++) { tagCategoryNames[i] = tagCategoryNames[i].Replace("Occult", "OccultAllowedForRandom"); }
            for (int i = 0; i < tagNames.Count; i++) { tagNames[i] = tagNames[i].Replace("Occult", "OccultAllowedForRandom"); }
            for (int i = 0; i < tagCategoryNames4All.Length; i++) { tagCategoryNames4All[i] = tagCategoryNames4All[i].Replace("Occult", "OccultAllowedForRandom"); }
            for (int i = 0; i < tagCategoryNamesInd.Length; i++) { tagCategoryNamesInd[i] = tagCategoryNamesInd[i].Replace("Occult", "OccultAllowedForRandom"); }

            adultRig = GetTS4Rig(XmodsEnums.Species.Human, XmodsEnums.Age.Adult);
            childRig = GetTS4Rig(XmodsEnums.Species.Human, XmodsEnums.Age.Child);
            toddlerRig = GetTS4Rig(XmodsEnums.Species.Human, XmodsEnums.Age.Toddler);
            infantRig = GetTS4Rig(XmodsEnums.Species.Human, XmodsEnums.Age.Infant);
            acRig = GetTS4Rig(XmodsEnums.Species.Cat, XmodsEnums.Age.Adult);
            ccRig = GetTS4Rig(XmodsEnums.Species.Cat, XmodsEnums.Age.Child);
            adRig = GetTS4Rig(XmodsEnums.Species.Dog, XmodsEnums.Age.Adult);
            cdRig = GetTS4Rig(XmodsEnums.Species.Dog, XmodsEnums.Age.Child);
            alRig = GetTS4Rig(XmodsEnums.Species.LittleDog, XmodsEnums.Age.Adult);

            GEOMlayersCoordinates.Text = layerStartPoint.ToString();

            MeshSlotrayRef_comboBox.SelectedIndex = 0;
            auSlotRayData = ReadSlotData(adultRig, XmodsEnums.Species.Human, XmodsEnums.Age.Adult);
            cuSlotRayData = ReadSlotData(childRig, XmodsEnums.Species.Human, XmodsEnums.Age.Child);
            puSlotRayData = ReadSlotData(toddlerRig, XmodsEnums.Species.Human, XmodsEnums.Age.Toddler);
            iuSlotRayData = ReadSlotData(infantRig, XmodsEnums.Species.Human, XmodsEnums.Age.Infant);
            adSlotRayData = ReadSlotData(adRig, XmodsEnums.Species.Dog, XmodsEnums.Age.Adult);
            alSlotRayData = ReadSlotData(alRig, XmodsEnums.Species.LittleDog, XmodsEnums.Age.Adult);
            acSlotRayData = ReadSlotData(acRig, XmodsEnums.Species.Cat, XmodsEnums.Age.Adult);
            cdSlotRayData = ReadSlotData(cdRig, XmodsEnums.Species.Dog, XmodsEnums.Age.Child);
            ccSlotRayData = ReadSlotData(ccRig, XmodsEnums.Species.Cat, XmodsEnums.Age.Child);

            SearchCASPBodyType_comboBox.Items.AddRange(Enum.GetNames(typeof(XmodsEnums.BodyType)));

            PackageDiffType_comboBox.Items.AddRange(Enum.GetNames(typeof(XmodsEnums.ResourceTypes)));

            ran = new Random();
        }

        private Dictionary<uint, GEOM.SlotRayData> ReadSlotData(RIG rig, XmodsEnums.Species species, XmodsEnums.Age age)
        {
            string prefix = (age == XmodsEnums.Age.Toddler ? "p" : age.ToString().Substring(0, 1).ToLower()) + 
                (species == XmodsEnums.Species.LittleDog && age == XmodsEnums.Age.Child ? "d" :
                (species == XmodsEnums.Species.Human)?"u":
            string path = appStartupPath + "\\" + prefix + "SlotRayData.txt";
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            Dictionary<uint, GEOM.SlotRayData> tmp = new Dictionary<uint, GEOM.SlotRayData>();
            string line;
            char[] sep = new char[] { '\t' };
            while ((line = file.ReadLine()) != null)
            {
                string[] values = line.Split(sep);
                uint hash, pivot;
                if (UInt32.TryParse(values[0].Substring(2), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out hash))
                {
                    try
                    {
                        Vector3 avgPos = Vector3.Parse(values[1]);
                        Vector3 origin = Vector3.Parse(values[2]);
                        if (values[3].Length < 8 || !UInt32.TryParse(values[3].Substring(2), NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out pivot)) pivot = 0;
                        tmp.Add(hash, new SlotRayData((uint)rig.GetBoneIndex(hash), avgPos, origin, rig.GetBoneGlobalQuaternion(hash).Conjugate(), pivot, (byte)rig.GetBoneIndex(pivot)));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message + Environment.NewLine + e.InnerException + Environment.NewLine + e.StackTrace);
                    }
                }
            }
            return tmp;
        }

        public class SlotRayData : GEOM.SlotRayData
        {
            internal SlotRayData(uint slotIndex, Vector3 slotAveragePosition, Vector3 rayOrigin, Xmods.DataLib.Quaternion transform,
                                    uint pivotHash, byte pivotIndex)
            {
                this.slotIndex = slotIndex;
                this.slotAvgPos = slotAveragePosition;
                this.rayOrigin = rayOrigin;
                this.transform = transform;
                this.pivotHash = pivotHash;
                this.pivotIndex = pivotIndex;
            }
        }

        private Dictionary<uint, GEOM.SlotRayData> SelectSlotRayData(XmodsEnums.Species species, XmodsEnums.Age age)
        {
            if (species == XmodsEnums.Species.Human)
            {
                if (age == XmodsEnums.Age.Infant) return iuSlotRayData;
                else if (age == XmodsEnums.Age.Toddler) return puSlotRayData;
                else if (age == XmodsEnums.Age.Child) return cuSlotRayData;
                else return auSlotRayData;
            }
            else if (species == XmodsEnums.Species.Cat)
            {
                if (age == XmodsEnums.Age.Child) return ccSlotRayData;
                else return acSlotRayData;
            }
            else if (species == XmodsEnums.Species.Dog)
            {
                if (age == XmodsEnums.Age.Child) return cdSlotRayData;
                else return adSlotRayData;
            }
            else // if (species == XmodsEnums.Species.LittleDog)
            {
                if (age == XmodsEnums.Age.Child) return cdSlotRayData;
                else return alSlotRayData;
            }
        }

        private RIG GetTS4Rig(XmodsEnums.Species species, XmodsEnums.Age age)
        {
            BinaryReader br = null;
            RIG rig = null;
            string path = appStartupPath + "\\S4_" + Enum.GetName(typeof(XmodsEnums.Age), age) + Enum.GetName(typeof(XmodsEnums.Species), species) + "_RIG.grannyrig";
            if ((br = new BinaryReader(File.OpenRead(path))) != null)
            {
                using (br)
                {
                    rig = new RIG(br);
                }
                br.Dispose();
            }
            else
            {
                MessageBox.Show("Can't open " + age.ToString() + "RIG file!");
                return null;
            }
            return rig;
        }

        private RIG SelectRig(XmodsEnums.Species species, XmodsEnums.Age age)
        {
            if (species == XmodsEnums.Species.Human)
            {
                if (age == XmodsEnums.Age.Infant) return infantRig;
                else if (age == XmodsEnums.Age.Toddler) return toddlerRig;
                else if (age == XmodsEnums.Age.Child) return childRig;
                else return adultRig;
            }
            else if (species == XmodsEnums.Species.Cat)
            {
                if (age == XmodsEnums.Age.Child) return ccRig;
                else return acRig;
            }
            else if (species == XmodsEnums.Species.Dog)
            {
                if (age == XmodsEnums.Age.Child) return cdRig;
                else return adRig;
            }
            else // if (species == XmodsEnums.Species.LargeDog)
            {
                if (age == XmodsEnums.Age.Child) return cdRig;
                else return alRig;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TS4 CAS Mesh Tools " + CASToolsVersion + Environment.NewLine +
                "by cmar" + Environment.NewLine +
                "This is free software available from modthesims.info!");
        }

        private void licenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("TS4 CAS Mesh Tools, a tool for creating custom content for The Sims 4," + Environment.NewLine +
                "Copyright (C) 2014  C. Marinetti" + Environment.NewLine +
                "This program comes with ABSOLUTELY NO WARRANTY. This is free software," + Environment.NewLine +
                "and you are welcome to redistribute it under certain conditions." + Environment.NewLine +
                "See the GNU-3.0 license included with this distribution for details.");
        }

        private void systemInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var os = (from x in new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().OfType<System.Management.ManagementObject>()
                      select x.GetPropertyValue("Caption")).FirstOrDefault();
            string info = "OS: " + (os != null ? os.ToString().Trim() : "Unknown ") + (Environment.Is64BitOperatingSystem ? " x64" : " x32") + Environment.NewLine;
            var processor = (from x in new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_Processor").Get().OfType<System.Management.ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault();
            info += "Processor: " + (processor != null ? processor.ToString() : "Unknown ") + Environment.NewLine;
            var video = (from x in new System.Management.ManagementObjectSearcher("SELECT * FROM Win32_VideoController").Get().OfType<System.Management.ManagementObject>()
                             select x.GetPropertyValue("Caption")).FirstOrDefault();
            info += "Video: " + (video != null ? video.ToString() : "Unknown ") + Environment.NewLine;
            Screen myScreen = Screen.FromControl(this);
            Rectangle area = myScreen.Bounds;
            info += "Screen resolution: " + area.Width.ToString() + "x" + area.Height.ToString() + Environment.NewLine;
            Graphics graphics = this.CreateGraphics();
            info += "DPI: " + graphics.DpiX.ToString() + Environment.NewLine;
            info += "CAS Tools version: " + CASToolsVersion;
            MessageBox.Show(info);
        }

        private void changeSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f = new CreatorPrompt(Properties.Settings.Default.Creator, Properties.Settings.Default.TS4Path, Properties.Settings.Default.TS4UserPath, Properties.Settings.Default.CASPupdateOption);
            f.ShowDialog();
        }

        private void fNVHashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FNVhashForm hasher = new FNVhashForm();
            hasher.Show();
        }

        private void enableSpecialToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CASTools_tabControl.TabPages.Contains(SpecialTools_tabPage))
            {
                CASTools_tabControl.TabPages.Remove(SpecialTools_tabPage);
                enableSpecialToolsToolStripMenuItem.Text = "Enable Special Tools";
            }
            else
            {
                CASTools_tabControl.TabPages.Add(SpecialTools_tabPage);
                enableSpecialToolsToolStripMenuItem.Text = "Hide Special Tools";
            }
        }

        private void normalEmissionMapResizerToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        internal string GetFilename(string title, string filter)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = filter;
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Title = title;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        internal bool GetGEOMData(string file, out GEOM outGEOM)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.FileName = file;
            GEOM newGEOM = new GEOM();
            try
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    using (myStream)
                    {
                        myStream.Position = 0;
                        BinaryReader br = new BinaryReader(myStream);
                        newGEOM.ReadFile(br);
                    }
                    myStream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file " + openFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                myStream.Close();
                outGEOM = newGEOM;
                return false;
            }
            outGEOM = newGEOM;
            return true;
        }

        internal string WriteGEOMFile(string title, GEOM myGEOM, string defaultFilename)
        {
            Stream myStream = null;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = GEOMfilter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = title;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "simgeom";
            saveFileDialog1.OverwritePrompt = true;
            if (String.CompareOrdinal(defaultFilename, " ") > 0) saveFileDialog1.FileName = defaultFilename;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            BinaryWriter bw = new BinaryWriter(myStream);
                            myGEOM.WriteFile(bw);
                        }
                        myStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file " + saveFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                    myStream.Close();
                }
                return saveFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        internal Package OpenPackage(string packagePath, bool readwrite)
        {
            try
            {
                Package package = (Package)Package.OpenPackage(0, packagePath, readwrite);
                return package;
            }
            catch (Exception e)
            {
                MessageBox.Show("Unable to read valid package data from " + packagePath + Environment.NewLine + ". Original error: " + e.Message + Environment.NewLine + e.StackTrace.ToString());
                return null;
            }
        }

        internal bool WritePackage(string title, Package pack, string defaultFilename)
        {
            string tmp;
            return WritePackage(title, pack, defaultFilename, out tmp);
        }
        
        internal bool WritePackage(string title, Package pack, string defaultFilename, out string newFilename)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = Packagefilter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = title;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "package";
            saveFileDialog1.OverwritePrompt = true;
            if (String.CompareOrdinal(defaultFilename, " ") > 0) saveFileDialog1.FileName = defaultFilename;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pack.SaveAs(saveFileDialog1.FileName);
                    newFilename = saveFileDialog1.FileName;
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file " + saveFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                    newFilename = saveFileDialog1.FileName;
                    return false;
                }
            }
            newFilename = "";
            return false;
        }

        internal bool GetOBJData(string file, out OBJ outOBJ)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.FileName = file;
            OBJ newOBJ = null;
            try
            {
                if ((myStream = openFileDialog1.OpenFile()) != null)
                {
                    using (myStream)
                    {
                        MemoryStream ms = new MemoryStream();
                        myStream.Position = 0;
                        myStream.CopyTo(ms);
                        ms.Position = 0;
                        StreamReader sr = new StreamReader(ms);
                        newOBJ = new OBJ(sr);
                    }
                    myStream.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file " + openFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                myStream.Close();
                outOBJ = newOBJ;
                return false;
            }
            outOBJ = newOBJ;
            return true;
        }

        internal string WriteOBJFile(string title, OBJ myOBJ, string defaultFilename)
        {
            Stream myStream = null;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = OBJfilter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = title;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "obj";
            saveFileDialog1.OverwritePrompt = true;
            if (String.CompareOrdinal(defaultFilename, " ") > 0) saveFileDialog1.FileName = defaultFilename;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = saveFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            StreamWriter sw = new StreamWriter(myStream);
                            myOBJ.Write(sw);
                        }
                        myStream.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not write file " + saveFileDialog1.FileName + ". Original error: " + ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                    myStream.Close();
                }
                return saveFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        internal string WriteDAEFile(string title, DAE dae, bool flipYZ, string defaultFilename)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = DAEfilter;
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.Title = title;
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "dae";
            saveFileDialog1.OverwritePrompt = true;
            if (String.CompareOrdinal(defaultFilename, " ") > 0) saveFileDialog1.FileName = defaultFilename;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                cloneWait_label.Visible = true;
                cloneWait_label.Refresh();
                dae.Write(saveFileDialog1.FileName, flipYZ);
                cloneWait_label.Visible = false;
                cloneWait_label.Refresh();
                return saveFileDialog1.FileName;
            }
            else
            {
                return "";
            }
        }

        internal class ItemCollection
        {
            internal uint outfitID;
            internal UInt64 meshinstance;
            internal string meshname;
            internal List<string> CASPnames;
            internal List<ulong> CASPids;
            internal List<Package> CASPpackages;
            internal List<ThumbnailResource> CASPthumbs;
            internal List<TGI> CASPswatchLinks;
            internal List<DdsFile> CASPswatches;
            internal TGI[][] lodarray;
            internal int species;
            internal int age;
            internal int gender;
            internal uint bodytype;

            internal ItemCollection(uint itemID, ulong meshID, string CASPname, ulong CASPid, TGI[][] lodArray, 
                int species, int age, int gender, uint bodytype, Package package, TGI swatchLink)
            {
                this.outfitID = itemID;
                this.meshinstance = meshID;
                this.lodarray = lodArray;
                this.CASPnames = new List<string>();
                this.CASPids = new List<ulong>();
                this.CASPpackages = new List<Package>();
                this.CASPswatchLinks = new List<TGI>();
                this.CASPswatches = new List<DdsFile>();
                this.species = species;
                this.age = age;
                this.gender = gender;
                this.bodytype = bodytype;
                this.AddCASP(CASPname, CASPid, package, swatchLink, bodytype);
            }

            internal void AddCASP(string CASPname, ulong CASPid, Package package, TGI swatchLink, uint bodyType)
            {
                int tmp = CASPname.LastIndexOf("_");
                if (tmp >= 0 && (XmodsEnums.BodyType)bodyType != XmodsEnums.BodyType.Fur)
                {
                    this.meshname = CASPname.Substring(0, tmp);
                    this.CASPnames.Add(CASPname.Substring(tmp + 1));
                }
                else
                {
                    this.meshname = CASPname;
                    this.CASPnames.Add("No swatch name");
                }
                this.CASPids.Add(CASPid);
                this.CASPpackages.Add(package);
                this.CASPswatchLinks.Add(swatchLink);
            }

            internal void AddThumb(int index, ThumbnailResource thumbImage)
            {
                if (this.CASPthumbs == null || this.CASPthumbs.Count == 0)
                {
                    this.CASPthumbs = new List<ThumbnailResource>(this.CASPids.Count);
                    for (int i = 0; i < this.CASPids.Count; i++)
                    {
                        this.CASPthumbs.Add(null);
                    }
                }
                this.CASPthumbs[index] = thumbImage;
            }

            internal void AddSwatch(int index, DdsFile swatchImage)
            {
                if (this.CASPswatches == null || this.CASPswatches.Count == 0)
                {
                    this.CASPswatches = new List<DdsFile>(this.CASPids.Count);
                    for (int i = 0; i < this.CASPids.Count; i++)
                    {
                        this.CASPswatches.Add(null);
                    }
                }
                this.CASPswatches[index] = swatchImage;
            }

        }

        private void SelectPack_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            PackSet_radioButton_Changed();
        }

        private void GamePack_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            PackSet_radioButton_Changed();
        }

        internal void PackSet_radioButton_Changed()
        {
            if (SelectPack_radioButton.Checked)
            {
                SelectPackFile.Enabled = true;
                SelectPack_button.Enabled = true;
                CASPfilters_panel.Enabled = false;
                MeshList_datagridview.Rows.Clear();
                SwatchList_dataGridView.Rows.Clear();
                CloneThumb_pictureBox.Image = null;
                New_radioButton.Checked = true;
                Default_radioButton.Enabled = false;
                NewMeshName.Text = "";
            }
            else if (GamePack_radioButton.Checked)
            {
                SelectPackFile.Enabled = false;
                SelectPackFile.Text = "";
                SelectPack_button.Enabled = false;
                CASPfilters_panel.Enabled = true;
                Default_radioButton.Enabled = true;
                SelectPackFile.Text = "";
                CloneThumb_pictureBox.Image = null;
                resourcePacks = new List<Package>(gamePacks0);
                resourcePacks.AddRange(gamePacksOther);
                thumbsPacks = new List<Package>(gameThumbPacks);
                ReadSourcePackageCASPs();
                PopulateItemsList();
            }
        }

        private bool SetupGamePacks()
        {
            string TS4FilesPath = Properties.Settings.Default.TS4Path;
            List<string> paths0 = new List<string>();
            try
            {
                List<string> pathsSim = new List<string>(Directory.GetFiles(TS4FilesPath, "Simulation*Build0.package", SearchOption.AllDirectories));
                List<string> pathsClient = new List<string>(Directory.GetFiles(TS4FilesPath, "Client*Build0.package", SearchOption.AllDirectories));
                pathsSim.Sort();
                pathsClient.Sort();
                paths0.AddRange(pathsSim);
                paths0.AddRange(pathsClient);
            }
            catch (DirectoryNotFoundException e)
            {
                MessageBox.Show("Your game packages path is invalid! Please go to File / Change Settings and correct it or make it blank to reset, then restart." 
                    + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                return false;
            }
            catch (IOException e)
            {
                MessageBox.Show("Your game packages path is invalid or a network error has occurred! Please go to File / Change Settings and correct it or make it blank to reset, then restart."
                    + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                return false;
            }
            catch (ArgumentException e)
            {
                MessageBox.Show("Your game packages path is not specified correctly! Please go to File / Change Settings and correct it or make it blank to reset, then restart."
                    + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                return false;
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show("You do not have the required permissions to access the game packages folder! Please restart with admin privileges."
                    + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                return false;
            }
            if (paths0.Count == 0)
            {
                MessageBox.Show("Can't find game packages! Please go to File / Change Settings and correct the game packages path or make it blank to reset, then restart.");
                return false;
            }

            try
            {
                List<string> contentPaths = new List<string>();
                string tmp = Properties.Settings.Default.TS4UserPath.Trim();
                if (string.Compare(tmp.Substring(tmp.Length - 1, 1), Path.DirectorySeparatorChar.ToString()) != 0) tmp += Path.DirectorySeparatorChar;
                tmp += "content";
                contentPaths = new List<string>(Directory.GetFiles(tmp, "*.package", SearchOption.AllDirectories));
                contentPaths.Sort();
                paths0.AddRange(contentPaths);
            }
            catch
            {
                MessageBox.Show("Either the path to your Sims 4 'content' folder in Documents is incorrect or you have no SDX content.");
            }

            try
            {
              /*  for (int i = 0; i < paths0.Count; i++)
                {
                    if (paths0[i].IndexOf("\\Data") < 0 && paths0[i].IndexOf("\\Delta") < 0)
                    {
                        paths0.RemoveAt(i);
                    }
                } */
                gamePacks0 = new Package[paths0.Count];
                gamePacks0Names = new string[paths0.Count];
                PackFilter_comboBox.Items.Add("All ");
                PackFilter_comboBox.Items.Add("BaseGame");
                List<string> packs = new List<string>();
                for (int i = 0; i < paths0.Count; i++)
                {
                    gamePacks0[i] = OpenPackage(paths0[i], false);
                    if (gamePacks0[i] == null)
                    {
                        MessageBox.Show("Can't read game packages!");
                        return false;
                    }
                    if (paths0[i].IndexOf("\\Data") < 0)
                    {
                        string tmp = Path.GetDirectoryName(paths0[i]);
                        string tmp2 = tmp.Substring(tmp.LastIndexOf("\\") + 1);
                        string pName = "";
                        PackNames.TryGetValue(tmp2, out pName);
                        string pack = tmp2 + " " + pName;
                        // if (pack != null && PackFilter_comboBox.Items.IndexOf(pack) < 0) PackFilter_comboBox.Items.Add(pack);
                        if (pack != null && packs.IndexOf(pack) < 0) packs.Add(pack);
                        gamePacks0Names[i] = pack;
                    }
                    else
                    {
                        gamePacks0Names[i] = "BaseGame";
                    }
                }
                packs.Sort();
                PackFilter_comboBox.Items.AddRange(packs.ToArray());

                string[] pathsAll = Directory.GetFiles(TS4FilesPath, "Client*Build*.package", SearchOption.AllDirectories);
                if (pathsAll.Length == 0)
                {
                    MessageBox.Show("Can't find game packages! Please go to File / Change Settings and correct the game packages path or make it blank to reset, then restart.");
                    return false;
                }
                List<string> pathsTmp = new List<string>(pathsAll);
                List<string> paths = new List<string>();
                foreach (string s in pathsTmp)
                {
                  //  if (s.IndexOf("Build0", StringComparison.CurrentCultureIgnoreCase) >= 0) continue;
                    if (s.IndexOf("Data", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        if (s.IndexOf("Build1", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                            s.IndexOf("Build2", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                            s.IndexOf("Build3", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                            s.IndexOf("Build4", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                            s.IndexOf("Build5", StringComparison.CurrentCultureIgnoreCase) >= 0) continue;
                    }
                    paths.Add(s);
                }
                paths.Sort();
                gamePacksOther = new Package[paths.Count];
                for (int i = 0; i < paths.Count; i++)
                {
                    gamePacksOther[i] = OpenPackage(paths[i], false);
                    if (gamePacksOther[i] == null)
                    {
                        MessageBox.Show("Can't read game packages!");
                        return false;
                    }
                }
                string[] thumbs = Directory.GetFiles(TS4FilesPath, "thumbnails.package", SearchOption.AllDirectories);
                string[] localthumbs;
                try
                {
                    localthumbs = Directory.GetFiles(Properties.Settings.Default.TS4UserPath,
                        "localthumbcache.package", SearchOption.TopDirectoryOnly);
                }
                catch
                {
                    MessageBox.Show("The path to your user Sims 4 folder in Documents is invalid! Please go to File / Change Settings and correct it or make it blank to reset, then restart.");
                    return false;
                }
                List<string> thumbPaths = new List<string>(thumbs);
                if (localthumbs != null && localthumbs.Length > 0) thumbPaths.AddRange(localthumbs);
                thumbPaths.Sort();
                gameThumbPacks = new Package[thumbPaths.Count];
                for (int i = 0; i < thumbPaths.Count; i++)
                {
                    gameThumbPacks[i] = OpenPackage(thumbPaths[i], false);
                    if (gameThumbPacks[i] == null)
                    {
                        MessageBox.Show("Can't read thumbnails packages!");
                        return false;
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show("You do not have the required permissions to open the game packages! Please restart with admin privileges."
                    + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + Environment.NewLine + e.StackTrace);
                return false;
            }
            return true;
        }

        private void SelectPack_button_Click(object sender, EventArgs e)
        {
            SelectPackFile.Text = GetFilename("Select Package File", Packagefilter);
            if (!File.Exists(SelectPackFile.Text))
            {
                MessageBox.Show("You have not selected a valid package file!");
                return;
            }
            myPack = OpenPackage(SelectPackFile.Text, false);
            if (myPack == null)
            {
                MessageBox.Show("Cannot read package file!");
                return;
            }
            resourcePacks = new List<Package>(gamePacks0);
            resourcePacks.AddRange(gamePacksOther);
            thumbsPacks = new List<Package>(new Package[] { myPack });
            thumbsPacks.AddRange(gameThumbPacks);
            ReadSourcePackageCASPs();
            PopulateItemsList();
            CloneThumb_pictureBox.Image = null;
            CloneThumb_pictureBox.Refresh();
        }

        private void SpeciesFilter_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GamePack_radioButton.Checked) PopulateItemsList();
        }

        private void GenderFilter_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GamePack_radioButton.Checked) PopulateItemsList();
        }

        private void AgeFilter_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GamePack_radioButton.Checked) PopulateItemsList();
        }

        private void PartFilter_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GamePack_radioButton.Checked) PopulateItemsList();
        }

        private void PackFilter_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GamePack_radioButton.Checked) PopulateItemsList();
        }

        private void TextFilter_TextChanged(object sender, EventArgs e)
        {
            if (GamePack_radioButton.Checked) PopulateItemsList();
        }

        private void ReadSourcePackageCASPs()
        {
            Predicate<IResourceIndexEntry> isCASP = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.CASP;
            List<ItemCollection> tmpItems = new List<ItemCollection>();

            Package[] packs;
            if (SelectPack_radioButton.Checked)
            {
                packs = new Package[] { myPack };
            }
            else
            {
                packs = gamePacks0; 
            }

            Clone_progressBar.Minimum = 0;
            Clone_progressBar.Maximum = packs.Length;
            Clone_progressBar.Value = 0;
            Clone_progressBar.Step = 1;
            Clone_progressBar.Visible = true;

            List<ulong> CASPinstances = new List<ulong>();
            int versionError = 0;
            foreach (Package p in packs)
            {
                List<IResourceIndexEntry> CASPlist = p.FindAll(isCASP);
                foreach (IResourceIndexEntry rcasp in CASPlist)
                {
                    Stream s = p.GetResource(rcasp);
                    s.Position = 0;
                    BinaryReader br = new BinaryReader(s);
                    CASP casp = null;
                    try
                    {
                        casp = new CASP(br);
                    }
                    catch (CASP.CASPEmptyException)
                    {
                        continue;
                    }
                    catch (CASP.CASPVersionException)
                    {
                        versionError++;
                        continue;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Can't read this CASP: " + rcasp.ToString() + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                        continue;
                    }

                    if (!CASPinstances.Contains(rcasp.Instance))
                    {
                        CASPinstances.Add(rcasp.Instance);
                        Predicate<IResourceIndexEntry> img = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.THUM & r.ResourceGroup == 2 &
                                                                    r.Instance == rcasp.Instance;
                        Predicate<ItemCollection> isColor = m => m.outfitID == casp.OutfitID & m.meshinstance == casp.MeshInstance;
                        int i = tmpItems.FindIndex(isColor);
                        if (i >= 0)
                        {
                            tmpItems[i].AddCASP(casp.PartName, rcasp.Instance, p, casp.LinkList[casp.SwatchIndex], (uint)casp.BodyType);
                        }
                        else
                        {
                            if (casp.MeshInstance > 0UL)
                            {
                                TGI[][] parts = new TGI[4][];
                                for (int j = 0; j < 4; j++)
                                {
                                    parts[j] = casp.MeshParts(j);
                                }
                                tmpItems.Add(new ItemCollection(casp.OutfitID, casp.MeshInstance, casp.PartName, rcasp.Instance, parts,
                                    casp.SpeciesNumeric, casp.AgeNumeric, casp.GenderNumeric, casp.BodyTypeNumeric, p, casp.LinkList[casp.SwatchIndex]));
                            }
                            else
                            {
                                tmpItems.Add(new ItemCollection(casp.OutfitID, 0UL, casp.PartName, rcasp.Instance, null,
                                    casp.SpeciesNumeric, casp.AgeNumeric, casp.GenderNumeric, casp.BodyTypeNumeric, p, casp.LinkList[casp.SwatchIndex]));
                            }
                        }
                    }
                }
                Clone_progressBar.PerformStep();
            }

            Clone_progressBar.Visible = false;

            if (versionError > 0) MessageBox.Show(versionError.ToString() + " CASPs with an unsupported version have been skipped.");

            if (tmpItems.Count == 0)
            {
                MessageBox.Show("No CAS parts found!");
                return;
            }
            CASitems = tmpItems.ToArray();
        }
        
        private void PopulateItemsList()
        {
            if (CASitems == null) return;
            MeshList_datagridview.Rows.Clear();
            SwatchList_dataGridView.Rows.Clear();
            NewMeshName.Text = "";
            int speciesFilter, ageFilter, genderFilter;
            speciesFilter = SpeciesFilter_comboBox.SelectedIndex;
            if (AgeFilter_comboBox.SelectedIndex == 0) ageFilter = 0;
            else ageFilter = (int)AgeValues[AgeFilter_comboBox.SelectedIndex - 1];
           // int ageFilter = (int)Enum.Parse(typeof(XmodsEnums.Age), AgeFilter_comboBox.SelectedItem.ToString());
            if (GenderFilter_comboBox.SelectedIndex == 0) genderFilter = 0;
            else genderFilter = (int)GenderValues[GenderFilter_comboBox.SelectedIndex - 1];
           // int genderFilter = (int)Enum.Parse(typeof(XmodsEnums.Gender), GenderFilter_comboBox.SelectedItem.ToString());
            string bodyTypeName = PartFilter_comboBox.SelectedItem.ToString();
            List<uint> bodyTypeTest = new List<uint>();
            bool isAll = false;
            bool isOther = false;
            if (string.Compare(bodyTypeName, "Other") == 0)
            {
                isOther = true;
            }
            else if (string.Compare(bodyTypeName, "All") == 0)
            {
                isAll = true;
            }
            else
            {
                bodyTypeTest = bodyTypeValues[bodyTypeNames.IndexOf(bodyTypeName)];
            }
            string packFilterValue = (((string)PackFilter_comboBox.SelectedItem).Substring(0, 4));

            for (int i = 0; i < CASitems.Length; i++)
            {
                if (GamePack_radioButton.Checked)
                {
                    if (!(speciesFilter == 0 || speciesFilter == CASitems[i].species)) continue;
                    if (!(ageFilter == 0 || (ageFilter & CASitems[i].age) > 0)) continue;
                    if (!(genderFilter == 0 || (genderFilter & CASitems[i].gender) > 0)) continue;
                    if (isAll)
                    { }
                    else if (isOther)
                    {
                        bool found = false;
                        for (int j = 0; j < bodyTypeValues.Length; j++)
                        {
                            if (bodyTypeValues[j].Contains(CASitems[i].bodytype))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found) continue;
                    }
                    else if (!bodyTypeTest.Contains(CASitems[i].bodytype))
                    {
                        continue;
                    }

                    if (!(String.Compare(packFilterValue, "All ") == 0))
                    {
                        if (String.Compare(packFilterValue, "Base") == 0)
                        {
                            bool isPack = false;
                            foreach (object o in PackFilter_comboBox.Items)
                            {
                                if (CASitems[i].meshname.IndexOf(((string)o).Substring(0, 4)) >= 0)
                                {
                                    isPack = true;
                                    break;
                                }
                            }
                            if (isPack) continue;
                        }
                        else if (CASitems[i].meshname.IndexOf(packFilterValue) < 0)
                        {
                            continue;
                        }
                    }

                    if (TextFilter.Text.Length > 1)
                    {
                        if (CASitems[i].meshname.IndexOf(TextFilter.Text, StringComparison.OrdinalIgnoreCase) < 0) continue;
                    }
                } 
                string[] tmp = new string[7];
                tmp[0] = CASitems[i].meshname;
                tmp[1] = CASitems[i].outfitID.ToString("X8");
                tmp[2] = (CASitems[i].meshinstance > 0UL) ? CASitems[i].meshinstance.ToString("X16") : "         -";
                tmp[3] = (CASitems[i].lodarray != null) ? CASitems[i].lodarray[0].Length.ToString() : "   -";
                tmp[4] = (CASitems[i].lodarray != null) ? CASitems[i].lodarray[1].Length.ToString() : "   -";
                tmp[5] = (CASitems[i].lodarray != null) ? CASitems[i].lodarray[2].Length.ToString() : "   -";
                tmp[6] = (CASitems[i].lodarray != null) ? CASitems[i].lodarray[3].Length.ToString() : "   -";
                int currentRow = MeshList_datagridview.RowCount;
                MeshList_datagridview.Rows.Add(tmp);
                MeshList_datagridview.Rows[currentRow].Tag = i;
            }
        }

        private void SourcePartList_datagridview_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            SwatchList_dataGridView.Rows.Clear();
            NewMeshName.Text = "";
            SwatchList_dataGridView.Refresh();
            itemIndex = (int)MeshList_datagridview.Rows[e.RowIndex].Tag;
            int swatchIndex = 0;
            foreach (string c in CASitems[itemIndex].CASPnames)
            {
                int currentRow = SwatchList_dataGridView.Rows.Count;
                SwatchList_dataGridView.Rows.Add();
                SwatchList_dataGridView.Rows[currentRow].Cells["SwatchName"].Value = c;
                SwatchList_dataGridView.Rows[currentRow].Cells["SwatchImage"].Value = GetSwatchImage(itemIndex, swatchIndex);
                SwatchList_dataGridView.Rows[currentRow].Tag = currentRow;
                swatchIndex++;
            }
            if (SwatchList_dataGridView.Rows.Count < 1)
            {
                SwatchList_dataGridView.Rows.Add("No swatches defined");
                SwatchList_dataGridView.Rows[0].Tag = 0;
            }
            NewMeshName.Text = Properties.Settings.Default.Creator + "_" + CASitems[itemIndex].meshname;

            bool gotit = false;
            for (int i = 0; i < CASitems[itemIndex].CASPids.Count; i++)
            {
                if (CASitems[itemIndex].CASPthumbs != null && CASitems[itemIndex].CASPthumbs[i] != null)
                {
                    CloneThumb_pictureBox.Image = CASitems[itemIndex].CASPthumbs[i].Image;
                    gotit = true;
                }
                else
                {
                    uint genderGroup = (CASitems[itemIndex].gender == 1) ? 0x0102u : 0x0002u;
                    Predicate<IResourceIndexEntry> img = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.THUM &
                                                r.ResourceGroup == genderGroup &
                                                r.Instance == CASitems[itemIndex].CASPids[i];
                    ThumbnailResource thumb = FindThumb(img);
                    if (thumb == null)
                    {
                        img = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.THUM &
                            r.Instance == CASitems[itemIndex].CASPids[i];
                        thumb = FindThumb(img);
                    }
                    if (thumb != null)
                    {
                        CASitems[itemIndex].AddThumb(i, thumb);
                        CloneThumb_pictureBox.Image = thumb.Image;
                        gotit = true;
                    }
                }
                if (gotit) break;
            }
            if (!gotit)
            {
                CloneThumb_pictureBox.Image = null;
                CloneThumb_pictureBox.Refresh();
            }
        }

        private ThumbnailResource FindThumb(Predicate<IResourceIndexEntry> searchCondition)
        {
            ThumbnailResource thumb = null;
            foreach (Package tp in thumbsPacks)
            {
                List<IResourceIndexEntry> rthumbs = tp.FindAll(searchCondition);
                if (rthumbs.Count > 0)
                {
                    rthumbs.Sort();
                    rthumbs.Reverse();
                    thumb = new ThumbnailResource(0, tp.GetResource(rthumbs[0]));
                    return thumb;
                }
            }
            return null;
        }

        private void SwatchList_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int swatchIndex = (int)SwatchList_dataGridView.Rows[e.RowIndex].Tag;

            if (CASitems[itemIndex].CASPthumbs != null && CASitems[itemIndex].CASPthumbs.Count > swatchIndex && 
                CASitems[itemIndex].CASPthumbs[swatchIndex] != null)
            {
                CloneThumb_pictureBox.Image = CASitems[itemIndex].CASPthumbs[swatchIndex].Image;
            }
            else
            {
                Predicate<IResourceIndexEntry> img = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.THUM &
                                            r.Instance == CASitems[itemIndex].CASPids[swatchIndex];
                ThumbnailResource thumb = null;
                bool gotit = false;
                foreach (Package tp in thumbsPacks)
                {
                    List<IResourceIndexEntry> rthumbs = tp.FindAll(img);
                    if (rthumbs.Count > 0)
                    {
                        rthumbs.Sort();
                        rthumbs.Reverse();
                        thumb = new ThumbnailResource(0, tp.GetResource(rthumbs[0]));
                        CASitems[itemIndex].AddThumb(swatchIndex, thumb);
                        gotit = true;
                        break;
                    }
                    if (gotit) break;
                }
                if (gotit)
                {
                    CloneThumb_pictureBox.Image = thumb.Image;
                }
                else
                {
                    CloneThumb_pictureBox.Image = null;
                    CloneThumb_pictureBox.Refresh();
                }
            }
        }

        private Image GetSwatchImage(int itemIndex, int swatchIndex)
        {
            if (!CASitems[itemIndex].CASPswatchLinks[swatchIndex].Equals(new TGI(0, 0, 0)))
            {
                if (CASitems[itemIndex].CASPswatches.Count > swatchIndex && CASitems[itemIndex].CASPswatches[swatchIndex] != null)
                {
                    return CASitems[itemIndex].CASPswatches[swatchIndex].Image;
                }
                else
                {
                    Predicate<IResourceIndexEntry> iswatch = r => r.ResourceType == CASitems[itemIndex].CASPswatchLinks[swatchIndex].Type &
                                            r.ResourceGroup == CASitems[itemIndex].CASPswatchLinks[swatchIndex].Group &
                                            r.Instance == CASitems[itemIndex].CASPswatchLinks[swatchIndex].Instance;
                    Package p;
                    IResourceIndexEntry rswatch = FindResource(iswatch, out p);
                    if (rswatch != null)
                    {
                        DSTResource dst = new DSTResource(0, p.GetResource(rswatch));
                        DdsFile dds = new DdsFile();
                        dds.Load(dst.ToDDS(), false);
                        CASitems[itemIndex].AddSwatch(swatchIndex, dds);
                        return dds.Image;
                    }
                    else
                    {
                        return Properties.Resources.NullImage;
                    }
                }
            }
            return null;
        }

        private void CloneAll_button_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in SwatchList_dataGridView.Rows)
            {
                r.Cells["CloneColor"].Value = true;
            }
        }

        private void CloneNone_button_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in SwatchList_dataGridView.Rows)
            {
                r.Cells["CloneColor"].Value = false;
            }
        }

        private void New_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            NewMeshName.Enabled = New_radioButton.Checked;
            //NewMeshName_label.Enabled = New_radioButton.Checked;
        }

        private void PackageEditFile_button_Click(object sender, EventArgs e)
        {
            if (clonePack != null)
            {
                DialogResult res = MessageBox.Show("Do you want to close the currently open package and open a new one?", "Open a new package", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel) return;
                if (!CheckForUnsaved(false)) return;
                clonePack.Dispose();
            }
            PackageEditFile.Text = GetFilename("Select Package File", Packagefilter);
            if (!File.Exists(PackageEditFile.Text))
            {
                MessageBox.Show("You have not selected a valid package file!");
                return;
            }
            clonePack = OpenPackage(PackageEditFile.Text, true);
            if (clonePack == null)
            {
                MessageBox.Show("Cannot read package file!");
                return;
            }
            Predicate<IResourceIndexEntry> isCASP = r => r.ResourceType == (uint)XmodsEnums.ResourceTypes.CASP;
            clonePackCASPs = new List<CASPinfo>();
            iresCASPs = clonePack.FindAll(isCASP);
            if (iresCASPs.Count == 0)
            {
                MessageBox.Show("No CAS Parts found in package!");
                return;
            }
            cloneWait_label.Location = new Point(this.Width / 2 - cloneWait_label.Width / 2, this.Height / 2 - cloneWait_label.Height / 2);
            cloneWait_label.Visible = true;
            cloneWait_label.Refresh();
            bool gotCreateInGame = false, gotMakeup = false;
            for (int i = 0; i < iresCASPs.Count; i++)
            {
                Stream sc = clonePack.GetResource(iresCASPs[i]);
                sc.Position = 0;
                BinaryReader brc = new BinaryReader(sc);
                try
                {
                    CASP casp = new CASP(brc);
                    if (casp.FlagCreateInGame || casp.CreateDescriptionKey > 0) gotCreateInGame = true;
                    if (IsMakeup(casp)) gotMakeup = true;
                    clonePackCASPs.Add(new CASPinfo(casp, FindCloneColorThumb(iresCASPs[i])));
                }
                catch
                {
                    MessageBox.Show("Can't read the CASP: " + iresCASPs[i].ToString());
                }
            }
            if (Properties.Settings.Default.CASPupdateOption == (uint)CASPUpdateOptions.Prompt || 
                Properties.Settings.Default.CASPupdateOption == (uint)CASPUpdateOptions.AutoUpdate)
            {
                bool needUpdate = false;
                for (int i = 0; i < clonePackCASPs.Count; i++)
                {
                    if (clonePackCASPs[i].Casp.Version < 0x2E)
                    {
                        needUpdate = true;
                    }
                }
                if (needUpdate)
                {
                    bool doUpdate = true;
                    if (Properties.Settings.Default.CASPupdateOption == (uint)CASPUpdateOptions.Prompt)
                    {
                        DialogResult res = MessageBox.Show("Update CASPs to latest version?", "CASP update", MessageBoxButtons.YesNo);
                        doUpdate = res == DialogResult.Yes;
                    }
                    if (doUpdate)
                    {
                        for (int i = 0; i < clonePackCASPs.Count; i++)
                        {
                            if (clonePackCASPs[i].Casp.UpdateToLatestVersion())
                            {
                                MemoryStream sw = new MemoryStream();
                                BinaryWriter bw = new BinaryWriter(sw);
                                clonePackCASPs[i].Casp.Write(bw);
                                sw.Position = 0;
                                ReplaceResource(clonePack, iresCASPs[i], sw);
                            }
                        }
                    }
                }
            }
            myRow = 0;
            CloneCASPsList(true);
            GetClonePackMeshes();
            ShowClonePackProperties();
            SetupClonePropTextureImage();
            ListClonePackMeshes();
            ListClonePackRegions();
            CloneThumbsList();
            if (clonePackCASPs.Count > 0) casPartPreviewer1.ResetPreviewerView(clonePackCASPs[0].Casp.Species, clonePackCASPs[0].Casp.Age);
            cloneWait_label.Visible = false;
            cloneWait_label.Refresh();
            SetAllChangesOff();
            StartPreview(CloneColorName.Text);
        }

        internal enum CASPUpdateOptions : uint
        {
            Prompt = 0,
            AutoUpdate = 1,
            NoUpdate = 2
        }

        private bool IsMakeup(CASP casp)
        {
            return (casp.BodyType == XmodsEnums.BodyType.Blush || casp.BodyType == XmodsEnums.BodyType.DimpleLeft ||
                casp.BodyType == XmodsEnums.BodyType.DimpleRight || casp.BodyType == XmodsEnums.BodyType.Eyebrows ||
                casp.BodyType == XmodsEnums.BodyType.Eyecolor || casp.BodyType == XmodsEnums.BodyType.Eyeliner ||
                casp.BodyType == XmodsEnums.BodyType.Eyeshadow || casp.BodyType == XmodsEnums.BodyType.Facepaint ||
                casp.BodyType == XmodsEnums.BodyType.ForeheadCrease || casp.BodyType == XmodsEnums.BodyType.Freckles ||
                casp.BodyType == XmodsEnums.BodyType.Lipstick || casp.BodyType == XmodsEnums.BodyType.Mascara ||
                casp.BodyType == XmodsEnums.BodyType.MoleLeftCheek || casp.BodyType == XmodsEnums.BodyType.MoleLeftLip ||
                casp.BodyType == XmodsEnums.BodyType.MoleRightCheek || casp.BodyType == XmodsEnums.BodyType.MoleRightLip ||
                casp.BodyType == XmodsEnums.BodyType.MouthCrease || casp.BodyType == XmodsEnums.BodyType.OccultBrow ||
                casp.BodyType == XmodsEnums.BodyType.OccultEyeLid || casp.BodyType == XmodsEnums.BodyType.OccultEyeSocket ||
                casp.BodyType == XmodsEnums.BodyType.OccultLeftCheek || casp.BodyType == XmodsEnums.BodyType.OccultMouth ||
                casp.BodyType == XmodsEnums.BodyType.OccultNeckScar || casp.BodyType == XmodsEnums.BodyType.OccultRightCheek ||
                casp.BodyType == XmodsEnums.BodyType.SecondaryEyeColor || casp.BodyType == XmodsEnums.BodyType.SkinDetailAcne ||
                casp.BodyType == XmodsEnums.BodyType.SkinDetailScar || casp.BodyType == XmodsEnums.BodyType.SkinOverlay);
        }

        private void SavePackage_button_Click(object sender, EventArgs e)
        {
            if (clonePack != null)
            {
                if (CheckForUnsaved(true)) clonePack.SavePackage();
            }
            else
            {
                MessageBox.Show("You don't have an open package to save!");
            }
        }

        private void SaveAsPackage_button_Click(object sender, EventArgs e)
        {
            if (clonePack != null)
            {
                if (!CheckForUnsaved(true)) return;
                string newName;
                if (!WritePackage("Save new package", clonePack, "", out newName))
                {
                    MessageBox.Show("Could not save package!");
                    return;
                }
                PackageEditFile.Text = newName;
                clonePack.Dispose();
                clonePack = OpenPackage(newName, true);
            }
            else
            {
                MessageBox.Show("You don't have an open package to save!");
            }
        }

        private void ClosePackage_button_Click(object sender, EventArgs e)
        {
            if (!CheckForUnsaved(false)) return;
            DialogResult res = MessageBox.Show("Are you sure you want to close the package?", "Package Close", MessageBoxButtons.OKCancel);
            if (res == DialogResult.OK)
            {
                clonePack.Dispose();
                clonePack = null;
                ClonePackageWipe();
                casPartPreviewer1.Stop_Mesh();
                CloneMeshWipe();
                CloneThumbsWipe();
                PackageEditFile.Text = "";
                SetAllChangesOff();
            }
        }

        private void SetAllChangesOff()
        {
            changesGeneral = false;
            changesMesh = false;
            changesRecolor = false;
            changesRegion = false;
            changesRegionMap = false;
            changesSliders = false;
            changesHairColor = false;
            changesThumbs = false;
        }

        private bool CheckForUnsaved(bool isSaving)
        {
            if (changesGeneral | changesRecolor | changesMesh | changesRegion | changesThumbs)
            {
                string unsaved = "";
                if (changesGeneral) unsaved += "You have unsaved General Properties changes" + Environment.NewLine;
                if (changesRecolor) unsaved += "You have unsaved changes to recolors" + Environment.NewLine;
                if (changesMesh) unsaved += "You have unsaved mesh changes" + Environment.NewLine;
                if (changesRegion) unsaved += "You have unsaved mesh region changes" + Environment.NewLine;
                if (changesThumbs) unsaved += "You have unsaved thumbnail changes" + Environment.NewLine;
                if (changesSliders) unsaved += "You have unsaved slider changes" + Environment.NewLine;
                if (changesHairColor) unsaved += "You have unsaved haircolor changes" + Environment.NewLine;
                unsaved += isSaving ? "Save all changes and continue?" : "Close package without saving?";
                DialogResult res = MessageBox.Show(unsaved, "Unsaved Changes", MessageBoxButtons.OKCancel);
                if (res == DialogResult.Cancel) return false;
                if (isSaving)
                {
                    if (changesGeneral)
                    {
                        ClonePropCommit();
                        changesGeneral = false;
                    }
                    if (changesRecolor)
                    {
                        CloneCASPCommit();
                        changesRecolor = false;
                    }
                    if (changesMesh)
                    {
                        MeshEditCommit();
                        changesMesh = false;
                    }
                    if (changesRegion)
                    {
                        cloneRegionCommit();
                        changesRegion = false;
                    }
                    if (changesThumbs)
                    {
                        CloneThumbsCommit();
                        changesThumbs = false;
                    }
                    if (changesSliders)
                    {
                        SliderCommit(CloneSliderAll_checkBox.Checked);
                        changesSliders = false;
                    }
                    if (changesHairColor)
                    {
                        HairColor_Commit(HairColorApplyAll_checkBox.Checked);
                        changesHairColor = false;
                    }
                    return true;
                }
                return true;
            }
            return true;
        }

        private void DeleteResource(Package package, TGI tgiToDelete)
        {
            IResourceKey key = new TGIBlock(1, null, tgiToDelete.Type, tgiToDelete.Group, tgiToDelete.Instance);
            DeleteResource(package, key);
        }
        private void DeleteResource(Package package, IResourceIndexEntry keyToDelete)
        {
            DeleteResource(package, (IResourceKey)keyToDelete);
        }
        private void DeleteResource(Package package, IResourceKey keyToDelete)
        {
            Predicate<IResourceIndexEntry> idel = r => r.ResourceType == keyToDelete.ResourceType &
                    r.ResourceGroup == keyToDelete.ResourceGroup & r.Instance == keyToDelete.Instance;
            List<IResourceIndexEntry> iries = package.FindAll(idel);
            foreach (IResourceIndexEntry irie in iries)
            {
                package.DeleteResource(irie);
            }
            iries = package.FindAll(idel);
            if (iries.Count > 0) MessageBox.Show("DeleteResource didn't work correctly! " + iries.Count.ToString() + " are left.");
            foreach (IResourceIndexEntry irie in iries)
            {
                package.DeleteResource(irie);
            }
            iries = package.FindAll(idel);
            if (iries.Count > 0) MessageBox.Show("DeleteResource didn't work correctly! " + iries.Count.ToString() + " are still left.");
        }

        private void ReplaceResource(Package package, IResourceIndexEntry keyToReplace, MemoryStream ms)
        {
            IResource res = new Resource(1, ms);
            package.ReplaceResource(keyToReplace, res);
        }

        internal class Resource : AResource
        {
            internal Resource(int APIversion, Stream s) : base(APIversion, s) { }

            public override int RecommendedApiVersion
            {
                get { return 1; }
            }

            protected override Stream UnParse()
            {
                return this.stream;
            }
        }

        private void fixDefaultEyeColorsAfterJune3PatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int eyesCount = 0;
            for (int i = 0; i < clonePackCASPs.Count; i++)
            {
                CASP casp = clonePackCASPs[i].Casp;
                if (casp.BodyType != XmodsEnums.BodyType.Eyecolor) continue;
                TGI imgTGI = casp.LinkList[casp.TextureIndex];
                Predicate<IResourceIndexEntry> imgIRE = r => r.ResourceType == imgTGI.Type & r.ResourceGroup == imgTGI.Group & r.Instance == imgTGI.Instance;
                IResourceIndexEntry irie = clonePack.Find(imgIRE);
                if (irie == null) continue;
                Stream si = clonePack.GetResource(irie);
                DeleteResource(clonePack, irie);
                TGIBlock newTGI = new TGIBlock(1, null, imgTGI.Type, imgTGI.Group | 0x80000000, imgTGI.Instance);
                clonePack.AddResource(newTGI, si, true);

                casp.LinkList[casp.TextureIndex] = new TGI(newTGI.ResourceType, newTGI.ResourceGroup, newTGI.Instance);
                DeleteResource(clonePack, iresCASPs[i]);
                Stream s = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(s);
                casp.Write(bw);
                clonePack.AddResource(iresCASPs[i], s, true);
                eyesCount++;
            }
            MessageBox.Show(eyesCount.ToString() + " eye colors updated");
            CloneCASPsList(false);
        }
    }
}
