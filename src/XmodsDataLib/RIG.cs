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
using System.Windows.Forms;

namespace Xmods.DataLib
{
    public class RIG
    {
        int version;                //3-4 - TS4 always 3?
        int minorVersion;           //1-2 - TS4 always 1?
        int boneCount;
        Bone[] bones;
        int IKchainCount;
        IKchain[] IKchains;

        public int NumberBones { get { return this.boneCount; } }

        public Bone[] Bones { get { return this.bones; } }

        public string GetBoneName(uint boneHash)
        {
            foreach (Bone bone in this.bones)
            {
                if (boneHash == bone.BoneHash) return bone.BoneName;
            }
            return "";
        }

        public Bone GetBone(uint boneHash)
        {
            foreach (Bone bone in this.bones)
            {
                if (boneHash == bone.BoneHash) return bone;
            }
            return null;
        }

        public int GetBoneIndex(uint boneHash)
        {
            for (int i = 0; i < this.NumberBones; i++)
            {
                if (boneHash == this.bones[i].BoneHash) return i;
            }
            return -1;
        }

        public List<uint> GetChildBoneHashes(uint boneHash)
        {
            List<uint> list = new List<uint>();
            RecurseChildBones(boneHash, list);
            return list;
        }
        internal void RecurseChildBones(uint parentHash, List<uint> childList)
        {
            for (int i = 0; i < this.NumberBones; i++)
            {
                if (this.bones[i].ParentBoneIndex >= 0 && this.bones[i].ParentBone.BoneHash == parentHash)
                {
                    childList.Add(this.bones[i].BoneHash);
                    RecurseChildBones(this.bones[i].BoneHash, childList);
                }
            }
        }

        public Quaternion GetBoneGlobalQuaternion(uint boneHash)
        {
            for (int i = 0; i < this.NumberBones; i++)
            {
                if (boneHash == this.bones[i].BoneHash) return this.bones[i].GlobalRotation;
            }
            return null;
        }
        public Quaternion GetBoneLocalQuaternion(uint boneHash)
        {
            for (int i = 0; i < this.NumberBones; i++)
            {
                if (boneHash == this.bones[i].BoneHash) return this.bones[i].LocalRotation;
            }
            return null;
        }
        public RIG(BinaryReader br)
        {
            br.BaseStream.Position = 0;
            version = br.ReadInt32();
            minorVersion = br.ReadInt32();
            boneCount = br.ReadInt32();
            bones = new Bone[boneCount];
            for (int i = 0; i < boneCount; i++)
            {
                bones[i] = new Bone(br, this, i);
            }
            IKchainCount = br.ReadInt32();
            IKchains = new IKchain[IKchainCount];
            for (int i = 0; i < IKchainCount; i++)
            {
                IKchains[i] = new IKchain(br, this);
            }
        }

        public override string ToString()
        {
            string tmp = "Version: " + version.ToString() + ", Minor Version: " + minorVersion.ToString() + Environment.NewLine;
            tmp += boneCount.ToString() + " Bones:" + Environment.NewLine + Environment.NewLine;
            for (int i = 0; i < boneCount; i++)
            {
                tmp += bones[i].ToString() + Environment.NewLine + Environment.NewLine;
            }
            tmp += IKchainCount.ToString() + " IK Chains:" + Environment.NewLine + Environment.NewLine;
            for (int i = 0; i < IKchainCount; i++)
            {
                tmp += IKchains[i].ToString() + Environment.NewLine + Environment.NewLine;
            }
            return tmp;
        }

        public class Bone
        {
            float[] position = new float[3];
            float[] orientation = new float[4];         //Quaternion 
            float[] scaling = new float[3];
            string boneName;
            int opposingBoneIndex;      // Same as the bone's index except in the case of Left/Right mirrored bones it is the index of its opposite
            int parentBoneIndex;
            uint boneHash;
            uint flags;                 //Usually 0x23, except with mirrored bones, then one will be 0x3F

            RIG rig;
            int index;
            Quaternion localRotation;
            Quaternion globalRotation;
            Matrix4D localTransform;
            Matrix4D globalTransform;
            Vector3 worldPosition;

            public string BoneName { get { return this.boneName; } }
            public int ParentBoneIndex { get { return this.parentBoneIndex; } }
            public RIG.Bone ParentBone { get { return this.rig.bones[parentBoneIndex]; } }
            public string ParentName { get { if (this.parentBoneIndex >= 0) { return rig.bones[this.parentBoneIndex].boneName; } else { return ""; } } }
            public string OpposingBoneName
            {
                get
                {
                    if (this.opposingBoneIndex != this.index) { return rig.bones[this.opposingBoneIndex].boneName; }
                    else { return ""; }
                }
            }
            public uint BoneHash { get { return this.boneHash; } }
            /// <summary>
            /// Returns position relative to parent bone
            /// </summary>
            public Vector3 PositionVector { get { return new Vector3(this.position); } }
            public Vector3 ScalingVector { get { return new Vector3(this.scaling); } }
            public Quaternion LocalRotation { get { return new Quaternion(this.localRotation.Coordinates); } }
            public Quaternion GlobalRotation { get { return new Quaternion(this.globalRotation.Coordinates); } }
            public Vector3 WorldPosition
            {
                get { return this.worldPosition; }
                set { this.worldPosition = new Vector3(value); }
            }
            public Quaternion MorphRotation
            {
                get
                {
                    return (parentBoneIndex >= 0) ? new Quaternion(this.rig.bones[parentBoneIndex].globalRotation.Coordinates) : this.globalRotation;
                }
            }
            public Matrix4D LocalTransform { get { return new Matrix4D(this.localTransform.Matrix); } }
            public Matrix4D GlobalTransform { get { return new Matrix4D(this.globalTransform.Matrix); } }

            internal Bone(BinaryReader br, RIG r, int index)
            {
                this.rig = r;
                this.index = index;
                for (int i = 0; i < 3; i++)
                {
                    position[i] = br.ReadSingle();
                }
                for (int i = 0; i < 4; i++)
                {
                    orientation[i] = br.ReadSingle();
                }
                for (int i = 0; i < 3; i++)
                {
                    scaling[i] = br.ReadSingle();
                }
                int boneNameLength = br.ReadInt32();
                char[] tmp = br.ReadChars(boneNameLength);
                boneName = new string(tmp);
                opposingBoneIndex = br.ReadInt32();
                parentBoneIndex = br.ReadInt32();
                boneHash = br.ReadUInt32();
                flags = br.ReadUInt32();

                this.localRotation = new Quaternion(this.orientation);
                if (this.localRotation.isEmpty) localRotation = Quaternion.Identity;
                if (!this.localRotation.isNormalized) this.localRotation.Balance();
                localTransform = this.localRotation.toMatrix4D(new Vector3(position), new Vector3(scaling));

                if (this.parentBoneIndex >= 0 && this.parentBoneIndex < rig.NumberBones)
                {
                    this.globalTransform = rig.bones[this.parentBoneIndex].globalTransform * localTransform;
                    this.globalRotation = rig.bones[this.parentBoneIndex].globalRotation * this.localRotation;
                }
                else    //no parents
                {
                    this.globalTransform = localTransform;
                    this.globalRotation = localRotation;
                }
                this.worldPosition = this.globalTransform * new Vector3();
            }

            public override string ToString()
            {
                string tmp = "Position: " + position[0].ToString() + "," + position[1].ToString() + "," + position[2].ToString() + Environment.NewLine;
                tmp += "Rotation: " + orientation[0].ToString() + "," + orientation[1].ToString() + "," + orientation[2].ToString() + Environment.NewLine;
                tmp += "Scaling: " + scaling[0].ToString() + "," + scaling[1].ToString() + "," + scaling[2].ToString() + Environment.NewLine;
                tmp += "Bone Name: " + boneName + Environment.NewLine;
                tmp += "Opposing Bone Index: " + opposingBoneIndex.ToString() + " (" + rig.bones[opposingBoneIndex].boneName + ")" + Environment.NewLine;
                tmp += "Parent Bone Index: " + parentBoneIndex.ToString() + ((parentBoneIndex >= 0 && parentBoneIndex < rig.boneCount) ? " (" + rig.bones[parentBoneIndex].boneName + ")" : "") + Environment.NewLine;
                tmp += "Bone Hash : " + boneHash.ToString("X8") + Environment.NewLine;
                tmp += "Flags : " + flags.ToString("X8");
                return tmp;
            }
        }

        internal class IKchain
        {
            int boneListLength;
            int[] boneIndex;            //bone index, boneListLength times
            int poleVectorIndex;        //bone index 
            int slotOffsetIndex;        //bone index 
            int rootIndex;              //bone index 
            RIG rig;

            internal IKchain(BinaryReader br, RIG r)
            {
                boneListLength = br.ReadInt32();
                boneIndex = new int[boneListLength];
                for (int i = 0; i < boneListLength; i++)
                {
                    boneIndex[i] = br.ReadInt32();
                }
                poleVectorIndex = br.ReadInt32();
                slotOffsetIndex = br.ReadInt32();
                rootIndex = br.ReadInt32();
                this.rig = r;
            }

            public override string ToString()
            {
                string tmp = "Bones: ";
                for (int i = 0; i < boneListLength; i++)
                {
                    tmp += rig.bones[boneIndex[i]].BoneName + " ";
                }
                tmp += Environment.NewLine + "PoleVectorIndex: " + poleVectorIndex.ToString() + Environment.NewLine;
                tmp += "SlotOffsetIndex: " + slotOffsetIndex.ToString() + Environment.NewLine;
                tmp += "RootIndex: " + rootIndex.ToString();
                return tmp;
            }
        }
    }
}
