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
   The author may be contacted at modthesims.info, username cmarNYC. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Xmods.DataLib
{
    public class CASP       //Sims 4 CASP resource
    {
        uint version;	// 0x2E is current
        uint offset;	// to resource reference table from end of header (ie offset + 8)
        int presetCount; // Not used for TS4
        string partname;		// UnicodeBE - part name
        float sortPriority;	// CAS sorts on this value, from largest to smallest

        ushort swatchOrder;   // mSecondaryDisplayIndex - swatch order
        uint outfitID;    // Used to group CASPs
        uint materialHash;
        byte parameterFlags;  //parameter flags: 
                                // 1 bit RestrictOppositeGender
                                // 1 bit AllowForLiveRandom
                                // 1 bit Show in CAS Demo
                                // 1 bit ShowInSimInfoPanel
                                // 1 bit ShowInUI
                                // 1 bit AllowForCASRandom : 1;
                                // 1 bit DefaultThumbnailPart
                                // 1 bit Deprecated
        byte parameterFlags2; // additional parameter flags:
                                // 5 bits unused
                                // 1 bit CreateInGame
                                // 1 bit unknown
                                // 1 bit DefaultForBodyTypeFemale
                                // 1 bit DefaultForBodyTypeMale
                                // 1 bit RestrictOppositeFrame
        ulong excludePartFlags; // parts removed
        ulong excludePartFlags2;         // v0x29
        ulong excludeModifierRegionFlags;
        int tagCount;
        PartTag[] categoryTags; // [tagCount] PartTags
        uint price;             //deprecated
        uint titleKey;
        uint partDescKey;
        uint createDescriptionKey;       // added in version 0x2B
        byte textureSpace;
        uint bodyType;
        uint bodySubType;    // usually 8, not used before version 37
        AgeGender ageGender;
        uint species;          // added in version 0x20
        ushort packID;       // added in version 0x25
        byte packFlags;      // added in version 0x25
                                // bit 7 - reserved, set to 0
                                // bit 1 - hide pack icon
        byte[] Reserved2Set0;  // added in version 0x25, nine bytes set to 0
        byte Unused2;        //usually 1
        byte Unused3;        //if Unused2 > 0; usually 0
        byte usedColorCount;
        uint[] colorData;    // [usedColorCount] color code
        byte buffResKey;     // index to data file with custom icon and text info
        byte swatchIndex;    // index to swatch image
        ulong VoiceEffect;   // added in version 0x1C -  mVoiceEffectHash, a hash of a sound effect
        byte usedMaterialCount;       // added in version 0x1e - if not 0, should be 3
        uint materialSetUpperBodyHash;       // added in version 0x1e
        uint materialSetLowerBodyHash;       // added in version 0x1e
        uint materialSetShoesHash;       // added in version 0x1e 
        uint occultBitField;            // added in version 0x1f - disabled for occult types
                                        // 30 bits reserved
                                        //  1 bit alien
                                        //  1 bit human
        ulong unknown1;                 // Version 0x2E
        UInt64 oppositeGenderPart;      // Version 0x28 - If the current part is not compatible with the Sim due to frame/gender
                                        // restrictions, use this part instead. Maxis convention is to use this
                                        // to specify the opposite gender version of the part. Set to 0 for none.
        UInt64 fallbackPart;            // Version 0x28 - If the current part is not compatible with the Sim due to frame/gender
                                        // restrictions, and there is no mOppositeGenderPart specified, use this part.
                                        // Maxis convention is to use this to specify a replacement part which is not
                                        // necessarily the opposite gendered version of the part. Set to 0 for none.
        OpacitySettings opacitySlider;     //V 0x2C
        SliderSettings hueSlider;           // "
        SliderSettings saturationSlider;    // "
        SliderSettings brightnessSlider;    // "
        byte[] hairColorKeys;                    // Version 0x2E - count times TGI indexes to HairColor CASPs
        byte nakedKey;
        byte parentKey;
        int  sortLayer;    
        byte lodCount;
        MeshDesc[] lods;      // [count] mesh lod and part indexes 
        byte numSlotKeys;
        byte[] slotKeys;      // [numSlotKeys] bytes
        byte textureIndex;    // index to texture TGI (diffuse)
        byte shadowIndex;     // index to 'shadow' texture/overlay
        byte compositionMethod;
        byte regionMapIndex;  // index to RegionMap file
        byte numOverrides;
        Override[] overrides; // [numOverrides] Override
        byte normalMapIndex;
        byte specularIndex;   // DDSRLES 
        uint UVoverride;      //added in version 0x1b, so far same values as bodyType
        byte emissionIndex;   // added in version 0x1d, for alien glow 
        byte reserved;        // added in version 0x2A
        byte IGTcount;        // Resource reference table in I64GT format (not TGI64)
                              // --repeat(count)
        TGI[] IGTtable;

        uint currentVersion = 0x2E;

        public bool UpdateToLatestVersion()
        {
            return UpdateToLatestVersion(false);
        }
        public bool UpdateToLatestVersion(bool legacyCompatible)
        {
            // Latest Version is 0x2E
            if (version < 28)
            {
                VoiceEffect = 0;
            }
            if (version < 30)
            {
                usedMaterialCount = 3;
                materialSetUpperBodyHash = 0;
                materialSetLowerBodyHash = 0;
                materialSetShoesHash = 0;
                emissionIndex = (byte)this.EmptyLink;
            }
            if (version < 31)
            {
                occultBitField = 0;
            }
            if (version < 32)
            {
                species = 1;
            }
            if (version < 37)
            {
                packID = 0;
                packFlags = 0;
                Reserved2Set0 = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
            if (version < 40)
            {
                this.FlagAllowForLiveRandom = this.FlagAllowForCASRandom;
                this.FlagRestrictOppositeGender = true;
                this.FlagRestrictOppositeFrame = true;
                List<PartTag> tmp = new List<PartTag>(this.categoryTags);
                if (this.Gender == XmodsEnums.Gender.Male | this.Gender == XmodsEnums.Gender.Unisex)
                {
                    tmp.Add(new PartTag(111, 1529));
                }
                if (this.Gender == XmodsEnums.Gender.Female | this.Gender == XmodsEnums.Gender.Unisex)
                {
                    tmp.Add(new PartTag(111, 1530));
                }
                this.categoryTags = tmp.ToArray();
                this.tagCount = this.categoryTags.Length;
            }
            if (version < 41)
            {
                this.excludePartFlags2 = 0;
            }
            if (version < 42)
            {
                reserved = 0;
            }
            if (version < 43)
            {
                createDescriptionKey = 0;
            }
            if (this.version < 0x2C)
            {
                this.opacitySlider = new OpacitySettings();
                this.hueSlider = new SliderSettings();
                this.saturationSlider = new SliderSettings();
                this.brightnessSlider = new SliderSettings();
            }
            if (this.version < 0x2E)
            {
                this.hairColorKeys = new byte[0];
            }

            if (legacyCompatible)
            {
                if (this.version != 0x2A)
                {
                    this.version = 0x2A;
                    return true;
                }
            }
            else if (this.version < 0x2E)
            {
                this.version = 0x2E;
                return true;
            }
            return false;
        }

        public uint Version
        {
            get { return this.version; }
        }
        public string PartName
        {
            get { return this.partname; }
            set { this.partname = value; }
        }
        public float SortPriority
        {
            get { return this.sortPriority; }
            set { this.sortPriority = value; }
        }
        public ushort SwatchOrder
        {
            get { return this.swatchOrder; }
            set { this.swatchOrder = value; }
        }
        public byte CASparameterFlags
        {
            get { return this.parameterFlags; }
            set { this.parameterFlags = value; }
        }
        public bool FlagRestrictOppositeGender
        {
            get { return (this.parameterFlags & (byte)XmodsEnums.CASParamFlag.RestrictOppositeGender) > 0; }
            set
            {
                if (value) { this.parameterFlags |= (byte)XmodsEnums.CASParamFlag.RestrictOppositeGender; }
                else { this.parameterFlags &= (byte)(~XmodsEnums.CASParamFlag.RestrictOppositeGender); }
            }
        }
        public bool FlagAllowForLiveRandom
        {
            get { return (this.parameterFlags & (byte)XmodsEnums.CASParamFlag.AllowForLiveRandom) > 0; }
            set
            {
                if (value) { this.parameterFlags |= (byte)XmodsEnums.CASParamFlag.AllowForLiveRandom; }
                else { this.parameterFlags &= (byte)(~XmodsEnums.CASParamFlag.AllowForLiveRandom); }
            }
        }
        public bool FlagShowInDemo
        {
            get { return (this.parameterFlags & (byte)XmodsEnums.CASParamFlag.ShowInCASDemo) > 0; }
            set
            {
                if (value) { this.parameterFlags |= (byte)XmodsEnums.CASParamFlag.ShowInCASDemo; }
                else { this.parameterFlags &= (byte)(~XmodsEnums.CASParamFlag.ShowInCASDemo); }
            }
        }
        public bool FlagShowInSimInfoPanel
        {
            get { return (this.parameterFlags & (byte)XmodsEnums.CASParamFlag.ShowInSimInfoPanel) > 0; }
            set
            {
                if (value) { this.parameterFlags |= (byte)XmodsEnums.CASParamFlag.ShowInSimInfoPanel; }
                else { this.parameterFlags &= (byte)(~XmodsEnums.CASParamFlag.ShowInSimInfoPanel); }
            }
        }
        public bool FlagShowInUI
        {
            get { return (this.parameterFlags & (byte)XmodsEnums.CASParamFlag.ShowInUI) > 0; }
            set
            {
                if (value) { this.parameterFlags |= (byte)XmodsEnums.CASParamFlag.ShowInUI; }
                else { this.parameterFlags &= (byte)(~XmodsEnums.CASParamFlag.ShowInUI); }
            }
        }
        public bool FlagAllowForCASRandom
        {
            get { return (this.parameterFlags & (byte)XmodsEnums.CASParamFlag.AllowForCASRandom) > 0; }
            set
            {
                if (value) { this.parameterFlags |= (byte)XmodsEnums.CASParamFlag.AllowForCASRandom; }
                else { this.parameterFlags &= (byte)(~XmodsEnums.CASParamFlag.AllowForCASRandom); }
            }
        }
        public bool FlagDefaultForThumbnail
        {
            get { return (this.parameterFlags & (byte)XmodsEnums.CASParamFlag.DefaultThumbnailPart) > 0; }
            set
            {
                if (value) { this.parameterFlags |= (byte)XmodsEnums.CASParamFlag.DefaultThumbnailPart; }
                else { this.parameterFlags &= (byte)(~XmodsEnums.CASParamFlag.DefaultThumbnailPart); }
            }
        }
        public bool FlagDefaultForBodyType
        {
            get { return (this.parameterFlags & (byte)XmodsEnums.CASParamFlag.DefaultForBodyType) > 0; }
            set
            {
                if (value) { this.parameterFlags |= (byte)XmodsEnums.CASParamFlag.DefaultForBodyType; }
                else { this.parameterFlags &= (byte)(~XmodsEnums.CASParamFlag.DefaultForBodyType); }
            }
        }
        public byte CASparameterFlags2
        {
            get { return this.parameterFlags2; }
            set { this.parameterFlags2 = value; }
        }
        public bool FlagDefaultForBodyTypeFemale
        {
            get { return (this.parameterFlags2 & (byte)XmodsEnums.CASParamFlag2.DefaultForBodyTypeFemale) > 0; }
            set
            {
                if (value) { this.parameterFlags2 |= (byte)XmodsEnums.CASParamFlag2.DefaultForBodyTypeFemale; }
                else { this.parameterFlags2 &= (byte)(~XmodsEnums.CASParamFlag2.DefaultForBodyTypeFemale); }
            }
        }
        public bool FlagDefaultForBodyTypeMale
        {
            get { return (this.parameterFlags2 & (byte)XmodsEnums.CASParamFlag2.DefaultForBodyTypeMale) > 0; }
            set
            {
                if (value) { this.parameterFlags2 |= (byte)XmodsEnums.CASParamFlag2.DefaultForBodyTypeMale; }
                else { this.parameterFlags2 &= (byte)(~XmodsEnums.CASParamFlag2.DefaultForBodyTypeMale); }
            }
        }
        public bool FlagRestrictOppositeFrame
        {
            get { return (this.parameterFlags2 & (byte)XmodsEnums.CASParamFlag2.RestrictOppositeFrame) > 0; }
            set
            {
                if (value) { this.parameterFlags2 |= (byte)XmodsEnums.CASParamFlag2.RestrictOppositeFrame; }
                else { this.parameterFlags2 &= (byte)(~XmodsEnums.CASParamFlag2.RestrictOppositeFrame); }
            }
        }
        public bool FlagUnknown
        {
            get { return (this.parameterFlags2 & (byte)XmodsEnums.CASParamFlag2.Unknown) > 0; }
            set
            {
                if (value) { this.parameterFlags2 |= (byte)XmodsEnums.CASParamFlag2.Unknown; }
                else { this.parameterFlags2 &= (byte)(~XmodsEnums.CASParamFlag2.Unknown); }
            }
        }
        public bool FlagCreateInGame
        {
            get { return (this.parameterFlags2 & (byte)XmodsEnums.CASParamFlag2.CreateInGame) > 0; }
            set
            {
                if (value) { this.parameterFlags2 |= (byte)XmodsEnums.CASParamFlag2.CreateInGame; }
                else { this.parameterFlags2 &= (byte)(~XmodsEnums.CASParamFlag2.CreateInGame); }
            }
        }
        public ulong ExcludePartFlags
        {
            get { return this.excludePartFlags; }
            set { this.excludePartFlags = value; }
        }
        public ulong ExcludePartFlags2
        {
            get { return this.excludePartFlags2; }
            set { this.excludePartFlags2 = value; }
        }
        public ulong ExcludeModifierRegionFlags
        {
            get { return this.excludeModifierRegionFlags; }
            set { this.excludeModifierRegionFlags = value; }
        }
        public bool OccultDisableForHuman
        {
            get { return (this.occultBitField & (uint)XmodsEnums.OccultTypeFlags.Human) > 0; }
            set
            {
                if (value) { this.occultBitField |= (uint)XmodsEnums.OccultTypeFlags.Human; }
                else { this.occultBitField &= (uint)(~XmodsEnums.OccultTypeFlags.Human); }
            }
        }
        public bool OccultDisableForAlien
        {
            get { return (this.occultBitField & (uint)XmodsEnums.OccultTypeFlags.Alien) > 0; }
            set
            {
                if (value) { this.occultBitField |= (uint)XmodsEnums.OccultTypeFlags.Alien; }
                else { this.occultBitField &= (uint)(~XmodsEnums.OccultTypeFlags.Alien); }
            }
        }
        public bool OccultDisableForVampire
        {
            get { return (this.occultBitField & (uint)XmodsEnums.OccultTypeFlags.Vampire) > 0; }
            set
            {
                if (value) { this.occultBitField |= (uint)XmodsEnums.OccultTypeFlags.Vampire; }
                else { this.occultBitField &= (uint)(~XmodsEnums.OccultTypeFlags.Vampire); }
            }
        }
        public bool OccultDisableForMermaid
        {
            get { return (this.occultBitField & (uint)XmodsEnums.OccultTypeFlags.Mermaid) > 0; }
            set
            {
                if (value) { this.occultBitField |= (uint)XmodsEnums.OccultTypeFlags.Mermaid; }
                else { this.occultBitField &= (uint)(~XmodsEnums.OccultTypeFlags.Mermaid); }
            }
        }
        public bool OccultDisableForWitch
        {
            get { return (this.occultBitField & (uint)XmodsEnums.OccultTypeFlags.Spellcaster) > 0; }
            set
            {
                if (value) { this.occultBitField |= (uint)XmodsEnums.OccultTypeFlags.Spellcaster; }
                else { this.occultBitField &= (uint)(~XmodsEnums.OccultTypeFlags.Spellcaster); }
            }
        }
        public bool OccultDisableForWerewolf
        {
            get { return (this.occultBitField & (uint)XmodsEnums.OccultTypeFlags.Werewolf) > 0; }
            set
            {
                if (value) { this.occultBitField |= (uint)XmodsEnums.OccultTypeFlags.Werewolf; }
                else { this.occultBitField &= (uint)(~XmodsEnums.OccultTypeFlags.Werewolf); }
            }
        }
        public uint CreateDescriptionKey
        {
            get { return this.createDescriptionKey; }
            set { this.createDescriptionKey = value; }
        }
        public XmodsEnums.BodyType BodyType
        {
            get { return (XmodsEnums.BodyType)this.bodyType; }
            set { this.bodyType = (uint)value; }
        }
        public uint BodyTypeNumeric
        {
            get { return this.bodyType; }
            set { this.bodyType = value; this.UVoverride = value; }
        }
        public XmodsEnums.BodyType SharedUVSpace
        {
            get { return (XmodsEnums.BodyType)this.UVoverride; }
            set { this.UVoverride = (uint)value; }
        }
        public uint SharedUVNumeric
        {
            get { return this.UVoverride; }
            set { this.UVoverride = value; }
        }
        public XmodsEnums.BodySubType BodySubType
        {
            get { return (XmodsEnums.BodySubType)this.bodySubType; }
            set { this.bodySubType = (uint)value; }
        }
        public uint BodySubTypeNumeric
        {
            get { return this.bodySubType; }
            set { this.bodySubType = value; }
        }
        public List<uint[]> CategoryTags
        {
            get
            {
                List<uint[]> tmp = new List<uint[]>();
                foreach (PartTag t in this.categoryTags)
                {
                    tmp.Add(new uint[] { t.FlagCategory, t.FlagValue });
                }
                return tmp;
            }
            set
            {
                PartTag[] tmp = new PartTag[value.Count];
                for (int i = 0; i < value.Count; i++)
                {
                    tmp[i] = new PartTag((ushort)value[i][0], value[i][1]);
                }
                this.categoryTags = tmp;
                this.tagCount = value.Count;
            }
        }

        public XmodsEnums.Species Species
        {
            get { return (XmodsEnums.Species)this.species; }
            set { this.species = (uint)value; }
        }
        public int SpeciesNumeric
        {
            get { return (int)this.species; }
        }
        public XmodsEnums.Age Age
        {
            get { return this.ageGender.Age; }
            set { this.ageGender.Age = value; }
        }
        public int AgeNumeric
        {
            get { return this.ageGender.AgeNumeric; }
        }
        public XmodsEnums.Gender Gender
        {
            get { return this.ageGender.Gender; }
            set { this.ageGender.Gender = value; }

        }
        public int GenderNumeric
        {
            get { return this.ageGender.GenderNumeric; }
        }

        public uint PackID
        {
            get { return this.packID; }
        }
        public bool HidePackIcon
        {
            get { return (this.packFlags & (uint)XmodsEnums.PackFlags.HidePackIcon) > 0; }
            set
            {
                if (value) { this.packFlags |= (byte)XmodsEnums.PackFlags.HidePackIcon; }
                else { this.packFlags &= (byte)(~XmodsEnums.PackFlags.HidePackIcon); }
            }
        }

        public OpacitySettings OpacitySliderSettings
        {
            get { return this.opacitySlider; }
            set { this.opacitySlider = value; }
        }
        public SliderSettings HueSliderSettings
        {
            get { return this.hueSlider; }
            set { this.hueSlider = value; }
        }
        public SliderSettings SaturationSliderSettings
        {
            get { return this.saturationSlider; }
            set { this.saturationSlider = value; }
        }
        public SliderSettings BrightnessSliderSettings
        {
            get { return this.brightnessSlider; }
            set { this.brightnessSlider = value; }
        }

        public UInt64 MeshInstance
        {
            get
            {
                foreach (TGI t in IGTtable)
                {
                    if (t.Type == (uint)XmodsEnums.ResourceTypes.RMAP) return t.Instance;
                }
                return 0UL;
            }
        }
        public TGI[] MeshParts(int LOD)
        {
            List<TGI> tgi = new List<TGI>();
            foreach (MeshDesc m in lods)
            {
                if (m.lod == LOD)
                {
                    for (int i = 0; i < m.indexes.Length; i++)
                    {
                        tgi.Add(IGTtable[m.indexes[i]]);
                    }
                }
            }
            return tgi.ToArray();
        }

        public int getLOD(TGI tgi)
        {
            for (int i = 0; i < lods.Length; i++)
            {
                for (int j = 0; j < lods[i].indexes.Length; j++)
                {
                    if (IGTtable[lods[i].indexes[j]].Equals(tgi))
                    {
                        return lods[i].lod;
                    }
                }
            }
            return -1;
        }

        public int[] getLODandPart(TGI tgi)
        {
            for (int i = 0; i < lods.Length; i++)
            {
                for (int j = 0; j < lods[i].indexes.Length; j++)
                {
                    if (IGTtable[lods[i].indexes[j]].Equals(tgi))
                    {
                        return new int[] { lods[i].lod, j };
                    }
                }
            }
            return null;
        }

        public bool MultipleMeshParts
        {
            get
            {
                for (int i = 0; i < lods.Length; i++)
                {
                    if (this.lods[i].indexes.Length > 1) return true;
                }
                return false;
            }
        }

        public void RemoveMeshLink(TGI meshTGI)
        {
            foreach (MeshDesc md in this.lods)
            {
                md.removeMeshLink(meshTGI, this.LinkList);
            }
        }

        public void AddMeshLink(TGI meshTGI, int lod)
        {
            foreach (MeshDesc md in this.lods)
            {
                if (md.lod == lod)
                {
                    md.addMeshLink(meshTGI, this.LinkList);
                }
            }
        }

        public uint[] ColorList
        {
            get
            {
                uint[] tmp = new uint[this.colorData.Length];
                Array.Copy(this.colorData, tmp, this.colorData.Length);
                return tmp;
            }
            set
            {
                uint[] tmp = new uint[value.Length];
                Array.Copy(value, tmp, value.Length);
                this.colorData = tmp;
                usedColorCount = (byte)value.Length;
            }
        }

        public int CompositionMethod
        {
            get
            {
                return (int)this.compositionMethod;
            }
            set
            {
                this.compositionMethod = (byte)value;
            }
        }

        public int SortLayer
        {
            get
            {
                return this.sortLayer;
            }
            set
            {
                this.sortLayer = value;
            }
        }

        public TGI[] LinkList
        {
            get { return IGTtable; }
            set { IGTtable = value; }
        }
        public void setLink(int Index, TGI tgi)
        {
            this.IGTtable[Index] = new TGI(tgi);
        }
        /// <summary>
        /// Adds a TGI to the TGI list
        /// </summary>
        /// <param name="tgi">tgi to add</param>
        /// <returns>Index of added TGI</returns>
        public byte addLink(TGI tgi)
        {
            List<TGI> newLinks = new List<TGI>(this.IGTtable);
            newLinks.Add(tgi);
            this.IGTtable = newLinks.ToArray();
            return (byte)(this.IGTtable.Length - 1);
        }
        public bool replaceLink(TGI oldtgi, TGI newtgi)
        {
            for (int i = 0; i < this.IGTtable.Length; i++)
            {
                if (IGTtable[i].Equals(oldtgi))
                {
                    IGTtable[i] = new TGI(newtgi);
                    return true;
                }
            }
            return false;
        }
        public int EmptyLink
        {
            get
            {
                TGI empty = new TGI(0, 0, 0);
                for (int i = 0; i < this.LinkList.Length; i++)
                {
                    if (this.LinkList[i].Equals(empty)) return i;
                }
                return -1;
            }
        }

        public byte BuffResKey
        {
            get { return this.buffResKey; }
            set { this.buffResKey = (byte)value; }
        }
        public byte SwatchIndex
        {
            get { return this.swatchIndex; }
            set { this.swatchIndex = (byte)value; }
        }
        public TGI AlternateThumbLink
        {
            get { return this.LinkList[this.swatchIndex]; }
            set 
            {
                TGI empty = new TGI(0, 0, 0);
                if (!this.LinkList[this.swatchIndex].Equals(empty) && value.Equals(empty))   // going from link to empty tgi
                {
                    this.LinkList[this.swatchIndex] = value;
                    this.RebuildLinkList();
                }
                else if (this.LinkList[this.swatchIndex].Equals(empty) && !value.Equals(empty))  // going from empty to link
                {
                    this.addLink(value);
                }
                else
                {
                    this.LinkList[this.swatchIndex] = value;
                }
            }
        }
        public bool HasAlternateThumb
        {
            get { return this.LinkList[this.swatchIndex].Instance > 0; }
        }
        public byte NakedKey
        {
            get { return this.nakedKey; }
            set { this.nakedKey = (byte)value; }
        }
        public byte ParentKey
        {
            get { return this.parentKey; }
            set { this.parentKey = (byte)value; }
        }
        public byte[] SlotKeys
        {
            get { return this.slotKeys; }
            set { this.slotKeys = value; }
        }
        public byte TextureIndex
        {
            get { return this.textureIndex; }
            set { this.textureIndex = (byte)value; }
        }
        public byte ShadowIndex
        {
            get { return this.shadowIndex; }
            set { this.shadowIndex = (byte)value; }
        }
        public byte RegionMapIndex
        {
            get { return this.regionMapIndex; }
            set { this.regionMapIndex = (byte)value; }
        }
        public byte NormalMapIndex
        {
            get { return this.normalMapIndex; }
            set { this.normalMapIndex = (byte)value; }
        }
        public byte SpecularIndex
        {
            get { return this.specularIndex; }
            set { this.specularIndex = (byte)value; }
        }
        public byte EmissionIndex
        {
            get { if (this.version >= 30) { return this.emissionIndex; } else { return (byte)this.EmptyLink; } }
            set { this.emissionIndex = (byte)value; }
        }

        public void RemoveSpecular()
        {
            this.specularIndex = RemoveKey(this.specularIndex);
            this.RebuildLinkList();
        }
        public void RemoveEmission()
        {
            this.emissionIndex = RemoveKey(this.emissionIndex);
            this.RebuildLinkList();
        }

        public uint OutfitID
        {
            get { return this.outfitID; }
            set { this.outfitID = value; }
        }

        public UInt64 OppositeGenderPart
        {
            get { return this.oppositeGenderPart; }
            set { this.oppositeGenderPart = value; }
        }
        public UInt64 FallbackPart
        {
            get { return this.fallbackPart; }
            set { this.fallbackPart = value; }
        }

        public ulong Unknown1
        {
            get { return this.unknown1; }
            set { this.unknown1 = value; }
        }
        public byte[] HairColorKeys
        {
            get { return this.hairColorKeys; }
            set { this.hairColorKeys = value; }
        }
        public void RemoveHairColorKeys()
        {
            this.hairColorKeys = new byte[0];
            this.RebuildLinkList();
        }
        public void ReplaceHairColorKeys(TGI[] tgiList)
        {
            this.hairColorKeys = new byte[tgiList.Length];
            for (int i = 0; i < this.hairColorKeys.Length; i++)
            {
                this.hairColorKeys[i] = this.addLink(tgiList[i]);
            }
            this.RebuildLinkList();
        }

        public CASP(BinaryReader br)
        {
            br.BaseStream.Position = 0;
            if (br.BaseStream.Length < 32) throw new CASPEmptyException("Attempt to read empty CASP");
            version = br.ReadUInt32();
            if (version > currentVersion) throw new CASPVersionException("Unsupported version of CASP");
            offset = br.ReadUInt32();
            presetCount = br.ReadInt32();
            partname = new BinaryReader(br.BaseStream, Encoding.BigEndianUnicode).ReadString();
            sortPriority = br.ReadSingle();
            swatchOrder = br.ReadUInt16();
            outfitID = br.ReadUInt32();
            materialHash = br.ReadUInt32();
            parameterFlags = br.ReadByte();
            if (this.version >= 39) parameterFlags2 = br.ReadByte();
            excludePartFlags = br.ReadUInt64();
            if (version >= 41)
            {
                excludePartFlags2 = br.ReadUInt64();
            }
            if (version > 36)
            {
                excludeModifierRegionFlags = br.ReadUInt64();
            }
            else
            {
                excludeModifierRegionFlags = br.ReadUInt32();
            }
            tagCount = br.ReadInt32();
            categoryTags = new PartTag[tagCount];
            for (int i = 0; i < tagCount; i++)
            {
                categoryTags[i] = new PartTag(br, version >= 37 ? 4 : 2);
            }
            price = br.ReadUInt32();
            titleKey = br.ReadUInt32();
            partDescKey = br.ReadUInt32();
            if (version > 42)
            {
                createDescriptionKey = br.ReadUInt32();
            }
            textureSpace = br.ReadByte();
            bodyType = br.ReadUInt32();
            bodySubType = br.ReadUInt32();
            ageGender = new AgeGender(br);
            if (this.version >= 32) species = br.ReadUInt32();
            if (this.version >= 34)
            {
                packID = br.ReadUInt16();
                packFlags = br.ReadByte();
                Reserved2Set0 = br.ReadBytes(9);
            }
            else
            {
                Unused2 = br.ReadByte();
                if (Unused2 > 0) Unused3 = br.ReadByte();
            }
            usedColorCount = br.ReadByte();
            colorData = new uint[usedColorCount];
            for (int i = 0; i < usedColorCount; i++)
            {
                colorData[i] = br.ReadUInt32();
            }
            buffResKey = br.ReadByte();
            swatchIndex = br.ReadByte();
            if (version >= 28)
            {
                VoiceEffect = br.ReadUInt64();
            }
            if (version >= 30)
            {
                usedMaterialCount = br.ReadByte();
                if (usedMaterialCount > 0)
                {
                    materialSetUpperBodyHash = br.ReadUInt32();
                    materialSetLowerBodyHash = br.ReadUInt32();
                    materialSetShoesHash = br.ReadUInt32();
                }
            }
            if (version >= 31)
            {
                occultBitField = br.ReadUInt32();
            }
            if (version >= 0x2E)
            {
                unknown1 = br.ReadUInt64();
            }
            if (version >= 38)
            {
                oppositeGenderPart = br.ReadUInt64();
            }
            if (version >= 39)
            {
                fallbackPart = br.ReadUInt64();
            }
            if (version >= 44)
            {
                opacitySlider = new OpacitySettings(br);
                hueSlider = new SliderSettings(br);
                saturationSlider = new SliderSettings(br);
                brightnessSlider = new SliderSettings(br);
            }
            if (version >= 0x2E)
            {
                byte unknownCount = br.ReadByte();
                hairColorKeys = br.ReadBytes(unknownCount);
            }
            nakedKey = br.ReadByte();
            parentKey = br.ReadByte();
            sortLayer = br.ReadInt32();
            lodCount = br.ReadByte();
            lods = new MeshDesc[lodCount];
            for (int i = 0; i < lodCount; i++)
            {
                lods[i] = new MeshDesc(br);
            }
            numSlotKeys = br.ReadByte();
            slotKeys = br.ReadBytes(numSlotKeys);
            textureIndex = br.ReadByte(); 
            shadowIndex = br.ReadByte(); 
            compositionMethod = br.ReadByte();
            regionMapIndex = br.ReadByte(); 
            numOverrides = br.ReadByte();
            overrides = new Override[numOverrides];
            for (int i = 0; i < numOverrides; i++)
            {
                overrides[i] = new Override(br);
            }
            normalMapIndex = br.ReadByte();
            specularIndex = br.ReadByte();
            if (version >= 27)
            {
                UVoverride = br.ReadUInt32();
            }
            if (version >= 29)
            {
                emissionIndex = br.ReadByte();
            }
            if (version >= 42)
            {
                reserved = br.ReadByte();
            }
            IGTcount = br.ReadByte();
            IGTtable = new TGI[IGTcount];
            for (int i = 0; i < IGTcount; i++)
            {
                IGTtable[i] = new TGI(br, TGI.TGIsequence.IGT);
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(version);
            long offsetPos = bw.BaseStream.Position;
            bw.Write(0);
            bw.Write(presetCount);
            new BinaryWriter(bw.BaseStream, Encoding.BigEndianUnicode).Write(partname);
            bw.Write(sortPriority);
            bw.Write(swatchOrder);
            bw.Write(outfitID);
            bw.Write(materialHash);
            bw.Write(parameterFlags);
            if (this.version >= 39) bw.Write(parameterFlags2);
            bw.Write(excludePartFlags);
            if (version >= 41)
            {
                bw.Write(excludePartFlags2);
            }
            if (version > 36)
            {
                bw.Write(excludeModifierRegionFlags);
            }
            else
            {
                bw.Write((uint)excludeModifierRegionFlags);
            }
            bw.Write(tagCount);
            for (int i = 0; i < tagCount; i++)
            {
                categoryTags[i].Write(bw, version >= 37 ? 4 : 2);
            }
            bw.Write(price);
            bw.Write(titleKey);
            bw.Write(partDescKey);
            if (version > 42)
            {
                bw.Write(createDescriptionKey);
            }
            bw.Write(textureSpace);
            bw.Write(bodyType);
            bw.Write(bodySubType);
            ageGender.Write(bw);
            if (this.version >= 32) bw.Write(species);
            if (this.version >= 34)
            {
                bw.Write(packID);
                bw.Write(packFlags);
                bw.Write(Reserved2Set0);
            }
            else
            {
                bw.Write(Unused2);
                if (Unused2 > 0) bw.Write(Unused3);
            }
            bw.Write(usedColorCount);
            for (int i = 0; i < usedColorCount; i++)
            {
                bw.Write(colorData[i]);
            }
            bw.Write(buffResKey);
            bw.Write(swatchIndex);
            if (version >= 28)
            {
                bw.Write(VoiceEffect);
            }
            if (version >= 30)
            {
                bw.Write(usedMaterialCount);
                if (usedMaterialCount > 0)
                {
                     bw.Write(materialSetUpperBodyHash);
                     bw.Write(materialSetLowerBodyHash);
                     bw.Write(materialSetShoesHash);
                }
            }
            if (version >= 31)
            {
                bw.Write(occultBitField);
            }
            if (version >= 0x2E)
            {
                bw.Write(unknown1);
            }
            if (version >= 38)
            {
                 bw.Write(oppositeGenderPart);
            }
            if (version >= 39)
            {
                 bw.Write(fallbackPart);
            }
            if (version >= 44)
            {
                opacitySlider.Write(bw);
                hueSlider.Write(bw);
                saturationSlider.Write(bw);
                brightnessSlider.Write(bw);
            }
            if (version >= 0x2E)
            {
                bw.Write((byte)hairColorKeys.Length);
                if (hairColorKeys.Length > 0) bw.Write(hairColorKeys);
            }
            bw.Write(nakedKey);
            bw.Write(parentKey);
            bw.Write(sortLayer);
            bw.Write(lodCount);
            for (int i = 0; i < lodCount; i++)
            {
                lods[i].Write(bw);
            }
            bw.Write(numSlotKeys);
            bw.Write(slotKeys);
            bw.Write(textureIndex);
            bw.Write(shadowIndex);
            bw.Write(compositionMethod);
            bw.Write(regionMapIndex);
            bw.Write(numOverrides);
            for (int i = 0; i < numOverrides; i++)
            {
                overrides[i].Write(bw);
            }
            bw.Write(normalMapIndex);
            bw.Write(specularIndex);
            if (version >= 27)
            {
                bw.Write(UVoverride);
            }
            if (version >= 29)
            {
                 bw.Write(emissionIndex);
            }
            if (version >= 42)
            {
                bw.Write(reserved);
            }
            long tablePos = bw.BaseStream.Position;
            bw.BaseStream.Position = offsetPos;
            bw.Write((uint)(tablePos - 8));
            bw.BaseStream.Position = tablePos;
            bw.Write((byte)IGTtable.Length);
            for (int i = 0; i < IGTtable.Length; i++)
            {
                IGTtable[i].Write(bw, TGI.TGIsequence.IGT);
            }
        }

        internal class AgeGender
        {
            byte age;
            byte gender;
            UInt16 unused;

            public XmodsEnums.Age Age
            {
                get
                {
                    return (XmodsEnums.Age)age;
                }
                set
                {
                    this.age = (byte)value;
                }
            }
            public int AgeNumeric
            {
                get
                {
                    return age;
                }
            }
            public XmodsEnums.Gender Gender
            {
                get
                {
                    return (XmodsEnums.Gender)(byte)(this.gender >> 4);
                }
                set
                {
                    this.gender = (byte)((byte)value << 4);
                }

            }
            public int GenderNumeric
            {
                get
                {
                    return (byte)(this.gender >> 4);
                }
            }

            public AgeGender(XmodsEnums.Age Age, XmodsEnums.Gender Gender)
            {
                this.age = (byte)Age;
                this.gender = (byte)((byte)Gender << 4);
            }

            internal AgeGender(BinaryReader br)
            {
                this.age = br.ReadByte();
                this.gender = br.ReadByte();
                this.unused = br.ReadUInt16();
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(this.age);
                bw.Write(this.gender);
                bw.Write(this.unused);
            }
        }

        internal class PartTag
        {
            ushort flagCategory;
            uint flagValue;

            internal ushort FlagCategory
            {
                get { return this.flagCategory; }
                set { this.flagCategory = value; }
            }
            internal uint FlagValue
            {
                get { return this.flagValue; }
                set { this.flagValue = value; }
            }

            internal PartTag(BinaryReader br, int valueLength)
            {
                flagCategory = br.ReadUInt16();
                if (valueLength == 4)
                {
                    flagValue = br.ReadUInt32();
                }
                else
                {
                    flagValue = br.ReadUInt16();
                }
            }

            internal PartTag(ushort category, uint flagVal)
            {
                this.flagCategory = category;
                this.flagValue = flagVal;
            }

            internal void Write(BinaryWriter bw, int valueLength)
            {
                bw.Write(flagCategory);
                if (valueLength == 4)
                {
                    bw.Write(flagValue);
                }
                else
                {
                    bw.Write((ushort)flagValue);
                }
            }
        }

        internal class Override
        {
            byte region;
            float layer;

            internal Override(BinaryReader br)
            {
                region = br.ReadByte();
                layer = br.ReadSingle();
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(region);
                bw.Write(layer);
            }
        }

        internal class MeshDesc
        {
            internal byte lod;
            internal uint Unused1;
            LODasset[] assets;
            internal byte[] indexes;

            internal int Length
            {
                get
                {
                    return 7 + (12 * assets.Length) + indexes.Length;
                }
            }

            internal void removeMeshLink(TGI meshTGI, TGI[] caspTGIlist)
            {
                List<byte> tmp = new List<byte>();
                foreach (byte i in indexes)
                {
                    if (!meshTGI.Equals(caspTGIlist[i])) tmp.Add(i);
                }
                this.indexes = tmp.ToArray();
            }

            internal void addMeshLink(TGI meshTGI, TGI[] caspTGIlist)
            {
                List<byte> tmp = new List<byte>(this.indexes);
                for (byte i = 0; i < caspTGIlist.Length; i++)
                {
                    if (meshTGI.Equals(caspTGIlist[i]))
                    {
                        tmp.Add(i);
                        break;
                    }
                }
                this.indexes = tmp.ToArray();
            }

            internal MeshDesc(BinaryReader br)
            {
                lod = br.ReadByte();
                Unused1 = br.ReadUInt32();
                byte numAssets = br.ReadByte();
                assets = new LODasset[numAssets];
                for (int i = 0; i < numAssets; i++)
                {
                    assets[i] = new LODasset(br);
                }
                byte indexCount = br.ReadByte();
                indexes = new byte[indexCount];
                for (int i = 0; i < indexCount; i++)
                {
                    indexes[i] = br.ReadByte();
                }
            }

            internal void Write(BinaryWriter bw)
            {
                bw.Write(lod);
                bw.Write(Unused1);
                bw.Write((byte)assets.Length);
                for (int i = 0; i < assets.Length; i++)
                {
                    assets[i].Write(bw);
                }
                bw.Write((byte)indexes.Length);
                for (int i = 0; i < indexes.Length; i++)
                {
                    bw.Write(indexes[i]);
                }
            }

            internal class LODasset
            {
                internal int sorting;
                internal int specLevel;
                internal int castShadow;

                internal LODasset(BinaryReader br)
                {
                    this.sorting = br.ReadInt32();
                    this.specLevel = br.ReadInt32();
                    this.castShadow = br.ReadInt32();
                }

                internal void Write(BinaryWriter bw)
                {
                    bw.Write(this.sorting);
                    bw.Write(this.specLevel);
                    bw.Write(this.castShadow);
                }
            }
        }

        public class OpacitySettings
        {
            internal float minimum;
            internal float increment;

            public float Minimum
            {
                get { return this.minimum; }
                set { this.minimum = value; }
            }
            public float Increment
            {
                get { return this.increment; }
                set { this.increment = value; }
            }

            public OpacitySettings()
            {
                this.minimum = .2f;
                this.increment = .05f;
            }

            public OpacitySettings(float minimum, float increment)
            {
                this.minimum = minimum;
                this.increment = increment;
            }

            internal OpacitySettings(BinaryReader br)
            {
                this.minimum = br.ReadSingle();
                this.increment = br.ReadSingle();
            }

            internal virtual void Write(BinaryWriter bw)
            {
                bw.Write(this.minimum);
                bw.Write(this.increment);
            }
        }

        public class SliderSettings : OpacitySettings
        {
            internal float maximum;

            public float Maximum
            {
                get { return this.maximum; }
                set { this.maximum = value; }
            }

            public SliderSettings() 
            {
                this.minimum = -.5f;
                this.maximum = .5f;
                this.increment = .05f;
            }

            public SliderSettings(float minimum, float maximum, float increment) 
            {
                this.minimum = minimum;
                this.maximum = maximum;
                this.increment = increment;
            }

            internal SliderSettings(BinaryReader br)
            {
                this.minimum = br.ReadSingle();
                this.maximum = br.ReadSingle();
                this.increment = br.ReadSingle();
            }

            internal override void Write(BinaryWriter bw)
            {
                bw.Write(this.minimum);
                bw.Write(this.maximum);
                bw.Write(this.increment);
            }
        }

        internal byte RemoveKey(byte index)
        {
            TGI empty_tgi = new TGI(0, 0, 0);
            if (this.IGTtable[index].Equals(empty_tgi) || index < 0 || index > this.IGTtable.Length - 1) return index;  //if not needed or invalid index, don't do anything.
            byte empty_index = 0;
            bool found = false;
            for (int i = 0; i < this.IGTtable.Length; i++)
            {
                if (this.IGTtable[i].Equals(empty_tgi))
                {
                    empty_index = (byte) i;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                return empty_index;
            }
            else
            {
                TGI[] newTGIlist = new TGI[this.IGTtable.Length + 1];
                Array.Copy(this.IGTtable, newTGIlist, this.IGTtable.Length);
                newTGIlist[this.IGTtable.Length] = empty_tgi;
                this.IGTtable = newTGIlist;
                return (byte)(this.IGTtable.Length - 1);
            }
        }

        public void RebuildLinkList()
        {
            List<TGI> newLinks = new List<TGI>();
            newLinks.Add(new TGI(0, 0, 0));

            this.buffResKey = AddLink(this.buffResKey, newLinks);
            this.swatchIndex = AddLink(this.swatchIndex, newLinks);
            if (this.version >= 0x2E)
            {
                for (int i = 0; i < this.hairColorKeys.Length; i++)
                {
                    this.hairColorKeys[i] = AddLink(this.hairColorKeys[i], newLinks);
                }
            }
            this.nakedKey = AddLink(this.nakedKey, newLinks);
            this.parentKey = AddLink(this.parentKey, newLinks);
            for (int i = 0; i < this.lodCount; i++)
            {
                for (int j = 0; j < this.lods[i].indexes.Length; j++)
                {
                    this.lods[i].indexes[j] = AddLink(this.lods[i].indexes[j], newLinks);
                }
            }
            for (int i = 0; i < this.numSlotKeys; i++)
            {
                this.slotKeys[i] = AddLink(this.slotKeys[i], newLinks);
            }
            this.textureIndex = AddLink(this.textureIndex, newLinks);
            this.shadowIndex = AddLink(this.shadowIndex, newLinks);
            this.regionMapIndex = AddLink(this.regionMapIndex, newLinks);
            this.normalMapIndex = AddLink(this.normalMapIndex, newLinks);
            this.specularIndex = AddLink(this.specularIndex, newLinks);
            this.emissionIndex = AddLink(this.emissionIndex, newLinks);
            this.IGTtable = newLinks.ToArray();
        }

        internal byte AddLink(byte indexIn, List<TGI> linkList)
        {
            if (this.IGTtable[indexIn].Equals(new TGI(0, 0, 0)) | indexIn < 0 | indexIn > IGTtable.Length) // correct invalid links
            {
                return (byte)0;
            }
            else
            {
                byte tmp = (byte)linkList.Count;
                linkList.Add(this.IGTtable[indexIn]);
                return tmp;
            }
        }

        [global::System.Serializable]
        public class CASPEmptyException : ApplicationException
        {
            public CASPEmptyException() { }
            public CASPEmptyException(string message) : base(message) { }
            public CASPEmptyException(string message, Exception inner) : base(message, inner) { }
            protected CASPEmptyException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
        [global::System.Serializable]
        public class CASPVersionException : ApplicationException
        {
            public CASPVersionException() { }
            public CASPVersionException(string message) : base(message) { }
            public CASPVersionException(string message, Exception inner) : base(message, inner) { }
            protected CASPVersionException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
    }
}
