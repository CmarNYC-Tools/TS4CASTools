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
   with thanks to EA/SimGuruModSquad for file description and code,
   and Kuree for initial translation into C# */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace Xmods.DataLib
{
    public class DMap
    {
        uint version;       // current is 7
        uint doubledWidth;
        uint height;
        uint ageGender;
        uint reserved1;     //must be set to 1
        byte physique;
        byte shapeOrNormals;
        uint minCol;
        uint maxCol;
        uint minRow;
        uint maxRow;
        byte robeChannel;
        float skinTightMinVal;
        float skinTightDelta;
        float robeMinVal;
        float robeDelta;
        int  totalBytes;
        ScanLine[] scanLines;

        public uint Version { get { return this.version; } }
        public uint Width { get { return this.doubledWidth/2; } }
        public uint Height { get { return this.height; } }
        public float SkinTightMinVal { get { return (this.version >= 7) ? this.skinTightMinVal : ((this.shapeOrNormals == 0) ? -0.2f : -0.75f); } }
        public float SkinTightDelta { get { return (this.version >= 7) ? this.skinTightDelta : ((this.shapeOrNormals == 0) ? 0.4f : 1.5f); } }
        public float RobeMinVal { get { return (this.version >= 7) ? this.robeMinVal : ((this.shapeOrNormals == 0) ? -0.2f : -0.75f); } }
        public float RobeDelta { get { return (this.version >= 7) ? this.robeDelta : ((this.shapeOrNormals == 0) ? 0.4f : 1.5f); ; } }
        public XmodsEnums.AgeGender AgeGender { get { return (XmodsEnums.AgeGender)this.ageGender; } }
        public Physiques Physique { get { return (Physiques)this.physique; } }
        public ShapeOrNormals ShapeOrNormal { get { return (ShapeOrNormals)this.shapeOrNormals; } }
        public bool RobeDataPresent { get { return (this.robeChannel == 0); } }

        public uint MinCol { get { return this.minCol; } }
        public uint MaxCol { get { return this.maxCol; } }
        public uint MinRow { get { return this.minRow; } }
        public uint MaxRow { get { return this.maxRow; } }
        public RobeChannel HasRobeChannel { get { return (RobeChannel)this.robeChannel; } }

        public bool HasData
        {
            get { return this.scanLines.Length > 0; }
        }

        public DMap(BinaryReader br)
        {
            br.BaseStream.Position = 0;
            this.version = br.ReadUInt32();
            this.doubledWidth = br.ReadUInt32();
            this.height = br.ReadUInt32();
            this.ageGender = br.ReadUInt32();
            if (version > 5) this.reserved1 = br.ReadUInt32();
            this.physique = br.ReadByte();
            this.shapeOrNormals = br.ReadByte();
            this.minCol = br.ReadUInt32();
            this.maxCol = br.ReadUInt32();
            this.minRow = br.ReadUInt32();
            this.maxRow = br.ReadUInt32();
            this.robeChannel = br.ReadByte();
            if (version > 6)
            {
                this.skinTightMinVal = br.ReadSingle();
                this.skinTightDelta = br.ReadSingle();
                if ((RobeChannel)robeChannel == RobeChannel.ROBECHANNEL_PRESENT)
                {
                    this.robeMinVal = br.ReadSingle();
                    this.robeDelta = br.ReadSingle();
                }
            }
            this.totalBytes = br.ReadInt32();
            if (this.totalBytes == 0)
            {
                scanLines = new ScanLine[0];
            }
            else
            {
                int width = (int)(maxCol - minCol + 1);
                uint numScanLines = maxRow - minRow + 1;
                scanLines = new ScanLine[numScanLines];
                for (int i = 0; i < numScanLines; i++)
                {
                    scanLines[i] = new ScanLine(width, br);
                }
            }
        }

        public void Write(BinaryWriter bw)
        {
            bw.Write(this.version);
            bw.Write(this.doubledWidth);
            bw.Write(this.height);
            bw.Write(this.ageGender);
            bw.Write(this.physique);
            bw.Write(this.shapeOrNormals);
            bw.Write(this.minCol);
            bw.Write(this.maxCol);
            bw.Write(this.minRow);
            bw.Write(this.maxRow);
            bw.Write(this.robeChannel);
            if (this.scanLines == null) this.scanLines = new ScanLine[0];
            uint totalBytes = 0;
            for (int i = 0; i < this.scanLines.Length; i++)
            {
                totalBytes += this.scanLines[i].scanLineDataSize;
            }
            bw.Write(totalBytes);
            for (int i = 0; i < this.scanLines.Length; i++)
            {
                this.scanLines[i].Write(bw);
            }
        }

        public enum Physiques : byte
        {
            BODYBLENDTYPE_HEAVY = 0,
            BODYBLENDTYPE_FIT = 1,
            BODYBLENDTYPE_LEAN = 2,
            BODYBLENDTYPE_BONY = 3,
            BODYBLENDTYPE_PREGNANT = 4,
            BODYBLENDTYPE_HIPS_WIDE = 5,
            BODYBLENDTYPE_HIPS_NARROW = 6,
            BODYBLENDTYPE_WAIST_WIDE = 7,
            BODYBLENDTYPE_WAIST_NARROW = 8,
            BODYBLENDTYPE_IGNORE = 9,   // Assigned to deformation maps associated with sculpts or modifiers, instead of a physique.
            BODYBLENDTYPE_AVERAGE = 100, // Special case used to indicate an "average" deformation map always applied for a given age
        }

        public enum ShapeOrNormals : byte
        {
            SHAPE_DEFORMER = 0,     // This resource contains positional deltas
            NORMALS_DEFORMER = 1    // This resource contains normal deltas
        }

        public enum RobeChannel : byte
        {
            ROBECHANNEL_PRESENT = 0,
            ROBECHANNEL_DROPPED = 1,
            ROBECHANNEL_ISCOPY = 2,     // Robe data not present but is the same as skin tight data.
        }

        private class ScanLine
        {
            internal UInt16 scanLineDataSize;
            internal CompressionType isCompressed;
            internal RobeChannel robeChannel;
            internal byte[] uncompressedPixels;
            internal byte numIndexes;
            internal UInt16[] pixelPosIndexes;
            internal UInt16[] dataPosIndexes;
            internal byte[] RLEArrayOfPixels;
            int width;

            public ScanLine(int width, BinaryReader br)
            {
                this.width = width;
                this.scanLineDataSize = br.ReadUInt16();
                this.isCompressed = (CompressionType)br.ReadByte();
                if (this.isCompressed == CompressionType.NoData)
                {
                    this.robeChannel = RobeChannel.ROBECHANNEL_DROPPED;
                }
                else
                {
                    this.robeChannel = (RobeChannel)br.ReadByte();
                }

                if (isCompressed == CompressionType.None)
                {
                    if (robeChannel == RobeChannel.ROBECHANNEL_PRESENT)
                    {
                        this.uncompressedPixels = br.ReadBytes(width * 6);
                    }
                    else
                    {
                        this.uncompressedPixels = br.ReadBytes(width * 3);
                    }
                }
                else if (isCompressed == CompressionType.RLE)
                {
                    this.numIndexes = br.ReadByte();
                    this.pixelPosIndexes = new UInt16[numIndexes];
                    this.dataPosIndexes = new UInt16[numIndexes];
                    for (int i = 0; i < numIndexes; i++) this.pixelPosIndexes[i] = br.ReadUInt16();
                    for (int i = 0; i < numIndexes; i++) this.dataPosIndexes[i] = br.ReadUInt16();
                    uint headerdatasize = 4U + 1U + (4U * numIndexes);
                    this.RLEArrayOfPixels = br.ReadBytes((int)(scanLineDataSize - headerdatasize));
                }

            }

            public void Write(BinaryWriter bw)
            {
                bw.Write(this.scanLineDataSize);
                bw.Write((byte)this.isCompressed);
                if (this.isCompressed != CompressionType.NoData) bw.Write((byte)this.robeChannel);

                if (isCompressed == CompressionType.None)
                {
                    bw.Write(this.uncompressedPixels);
                }
                else if (isCompressed == CompressionType.RLE)
                {
                    bw.Write(this.numIndexes);
                    for (int i = 0; i < numIndexes; i++) bw.Write(this.pixelPosIndexes[i]);
                    for (int i = 0; i < numIndexes; i++) bw.Write(this.dataPosIndexes[i]);
                    uint headerdatasize = 4U + 1U + (4U * numIndexes);
                    bw.Write(this.RLEArrayOfPixels);
                }

            }
        }

        public enum OutputType
        {
            Skin,
            Robe
        }

        public enum CompressionType : byte
        {
            None = 0,
            RLE = 1,
            NoData = 2
        }

        public bool ToPixelArrays(out byte[] skinTightArray, out byte[] robeArray)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            if (maxCol == 0)
            {
                skinTightArray = new byte[0];
                robeArray = new byte[0];
                return false;
            }
            int height = (int)(maxRow - minRow + 1);
            int width = (int)(this.maxCol - this.minCol + 1);

            byte[] pixelArraySkinTight = new byte[width * height * 3];
            byte[] pixelArrayRobe = new byte[width * height * 3];

            int destIndexRobe = 0;
            int destSkinTight = 0;

            int pixelsize = 0;

            for (int i = 0; i < height; i++)
            // for (int i = height - 1; i >= 0; i--)       //scan lines are inverted for some bizarre reason
            {
                if (scanLines[i].robeChannel == RobeChannel.ROBECHANNEL_PRESENT)
                {
                    pixelsize = 6;
                }
                else
                {
                    pixelsize = 3;
                }

                var scan = scanLines[i];
                if (scan.isCompressed == CompressionType.None)
                {
                    for (int j = 0; j < width; j++)
                    {
                        pixelArraySkinTight[destSkinTight++] = scan.uncompressedPixels[(j * pixelsize) + 0];
                        pixelArraySkinTight[destSkinTight++] = scan.uncompressedPixels[(j * pixelsize) + 1];
                        pixelArraySkinTight[destSkinTight++] = scan.uncompressedPixels[(j * pixelsize) + 2];

                        switch (scan.robeChannel)
                        {
                            case RobeChannel.ROBECHANNEL_PRESENT:
                                pixelArrayRobe[destIndexRobe++] = scan.uncompressedPixels[(j * pixelsize) + 3];
                                pixelArrayRobe[destIndexRobe++] = scan.uncompressedPixels[(j * pixelsize) + 4];
                                pixelArrayRobe[destIndexRobe++] = scan.uncompressedPixels[(j * pixelsize) + 5];
                                break;
                            case RobeChannel.ROBECHANNEL_DROPPED:
                                pixelArrayRobe[destIndexRobe++] = 0x80;
                                pixelArrayRobe[destIndexRobe++] = 0x80;
                                pixelArrayRobe[destIndexRobe++] = 0x80;
                                break;
                            case RobeChannel.ROBECHANNEL_ISCOPY:
                                pixelArrayRobe[destIndexRobe++] = scan.uncompressedPixels[(j * pixelsize) + 0];
                                pixelArrayRobe[destIndexRobe++] = scan.uncompressedPixels[(j * pixelsize) + 1];
                                pixelArrayRobe[destIndexRobe++] = scan.uncompressedPixels[(j * pixelsize) + 2];
                                break;
                        }
                    }
                }
                else if (scan.isCompressed == CompressionType.RLE)
                {

                    // Look up each pixel using index tables
                    for (int j = 0; j < width; j++)
                    {
                        // To get pointer to the RLE encoded data we need first find 
                        // proper RLE run in the buffer. Use index for this:

                        // Cache increment for indexing in pixel space?
                        int step = 1 + width / (scan.numIndexes - 1); // 1 entry was added for the remainder of the division

                        // Find index into the positions and data table:
                        int idx = j / step;

                        // This is location of the run first covering this interval.
                        int pixelPosX = scan.pixelPosIndexes[idx];

                        // Position of the RLE data of the place where need to unwind to the pixel. 
                        int dataPos = scan.dataPosIndexes[idx] * (pixelsize + 1); // +1 for run length byte

                        // This is run length for the RLE entry found at 
                        int runLength = scan.RLEArrayOfPixels[dataPos];

                        // Loop forward unwinding RLE data from the found indexed position. 
                        // Continue until the pixel position in question is not covered 
                        // by the current run interval. By design the loop should execute 
                        // only few times until we find the value we are looking for.
                        while (j >= pixelPosX + runLength)
                        {
                            pixelPosX += runLength;
                            dataPos += (1 + pixelsize); // 1 for run length, +pixelSize for the run value

                            runLength = scan.RLEArrayOfPixels[dataPos];
                        }

                        // After breaking out of the cycle, we have the current run length interval
                        // covering the pixel position x we are interested in. So just return the pointer
                        // to the pixel data we were after:
                        int pixelStart = dataPos + 1;

                        //
                        pixelArraySkinTight[destSkinTight++] = scan.RLEArrayOfPixels[pixelStart + 0];
                        pixelArraySkinTight[destSkinTight++] = scan.RLEArrayOfPixels[pixelStart + 1];
                        pixelArraySkinTight[destSkinTight++] = scan.RLEArrayOfPixels[pixelStart + 2];
                        switch (scan.robeChannel)
                        {
                            case RobeChannel.ROBECHANNEL_PRESENT:
                                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[pixelStart + 3];
                                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[pixelStart + 4];
                                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[pixelStart + 5];
                                break;
                            case RobeChannel.ROBECHANNEL_DROPPED:
                                pixelArrayRobe[destIndexRobe++] = 0x80;
                                pixelArrayRobe[destIndexRobe++] = 0x80;
                                pixelArrayRobe[destIndexRobe++] = 0x80;
                                break;
                            case RobeChannel.ROBECHANNEL_ISCOPY:
                                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[pixelStart + 0];
                                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[pixelStart + 1];
                                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[pixelStart + 2];
                                break;
                        }
                    }
                }
                else if (scan.isCompressed == CompressionType.NoData)
                {
                    for (int j = 0; j < width; j++)
                    {
                        pixelArraySkinTight[destSkinTight++] = 0x80;
                        pixelArraySkinTight[destSkinTight++] = 0x80;
                        pixelArraySkinTight[destSkinTight++] = 0x80;
                        pixelArrayRobe[destIndexRobe++] = 0x80;
                        pixelArrayRobe[destIndexRobe++] = 0x80;
                        pixelArrayRobe[destIndexRobe++] = 0x80;
                    }
                }
            }

            skinTightArray = pixelArraySkinTight;
            robeArray = pixelArrayRobe;
            return true;
        }
        
        public Stream ToBitMap(OutputType type)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            if (maxCol == 0) return null;
            int height = (int)(maxRow - minRow + 1);
            int width = (int)(this.maxCol - this.minCol + 1);

            byte[] pixelArraySkinTight;
            byte[] pixelArrayRobe;

            if (!this.ToPixelArrays(out pixelArraySkinTight, out pixelArrayRobe)) return null;

            w.Write((ushort)0x4d42);
            w.Write(0);
            w.Write(0);
            w.Write(54);
            w.Write(40);
            w.Write(width);
            w.Write(height);
            w.Write((ushort)1);
            w.Write((ushort)24);
            for (int i = 0; i < 6; i++) w.Write(0);

            int bytesPerLine = (int)Math.Ceiling(width * 24.0 / 8.0);
            int padding = 4 - bytesPerLine % 4;
            if (padding == 4) padding = 0;
            long sourcePosition = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width * 3; j++)
                {
                    w.Write(type == OutputType.Robe ? pixelArrayRobe[sourcePosition++] : pixelArraySkinTight[sourcePosition++]);
                }

                for (int j = 0; j < padding; j++)
                {
                    w.Write((byte)0);
                }
            }

            return ms;
        }

        public MorphMap ToMorphMap(bool useRobeData)
        {
            if (maxCol == 0) return null;
            int height = (int)(maxRow - minRow + 1);
            int width = (int)(this.maxCol - this.minCol + 1);

            byte[] pixelArraySkinTight;
            byte[] pixelArrayRobe;
            bool[] robeDataRow = new bool[height];

            for (int i = 0; i < height; i++)
            {
                robeDataRow[i] = (scanLines[i].robeChannel == RobeChannel.ROBECHANNEL_PRESENT);
            }

            if (!this.ToPixelArrays(out pixelArraySkinTight, out pixelArrayRobe)) return null;

            long sourcePosition = 0;
            Vector3[][] deltaSkin = new  Vector3[height][];
            Vector3[][] deltaRobe = new  Vector3[height][];
            for (int i = 0; i < height; i++)
            {
                deltaSkin[i] = new Vector3[width];
                deltaRobe[i] = new Vector3[width];
                for (int j = 0; j < width; j++)
                {
                    deltaSkin[i][j] = new Vector3(((pixelArraySkinTight[sourcePosition] * this.SkinTightDelta) / 255f) + this.SkinTightMinVal,
                                                  ((pixelArraySkinTight[sourcePosition + 1] * this.SkinTightDelta) / 255f) + this.SkinTightMinVal,
                                                  ((pixelArraySkinTight[sourcePosition + 2] * this.SkinTightDelta) / 255f) + this.SkinTightMinVal);
                    deltaRobe[i][j] = new Vector3(((pixelArrayRobe[sourcePosition] * this.RobeDelta) / 255f) + this.RobeMinVal,
                                                  ((pixelArrayRobe[sourcePosition + 1] * this.RobeDelta) / 255f) + this.RobeMinVal,
                                                  ((pixelArrayRobe[sourcePosition + 2] * this.RobeDelta) / 255f) + this.RobeMinVal);
                    sourcePosition += 3;
                }
            }

            return new MorphMap(this.Width, this.height, this.shapeOrNormals, this.minCol, this.maxCol,
                                this.minRow, this.maxRow, useRobeData ? this.robeChannel == 0 : false, 
                                deltaSkin, deltaRobe, robeDataRow);
        }
    }

    public class MorphMap
    {
        uint mapWidth;
        uint mapHeight;
        byte shapeOrNormals;
        uint minCol;
        uint maxCol;
        uint minRow;
        uint maxRow;
        bool robeChannel;
        Vector3[][] skinDeltas;
        Vector3[][] robeDeltas;
        bool[] robeDataRow;

        public uint MapWidth { get { return this.mapWidth; } }
        public uint MapHeight { get { return this.mapHeight; } }
        public bool RobeDataPresent { get { return this.robeChannel; } }
        public uint MinCol { get { return this.minCol; } }
        public uint MaxCol { get { return this.maxCol; } }
        public uint MinRow { get { return this.minRow; } }
        public uint MaxRow { get { return this.maxRow; } }

        public Vector3 GetAdjustedDelta(int x, int y, bool mirrorX, byte robeBlend)
        {
          //  float robeMult = (float)robeBlend / 255f;
            float robeMult = (float)robeBlend / 63f;            //new limit as of Cats & Dogs patch

            Vector3 tmp = new Vector3(skinDeltas[y][Math.Abs(x)]);
            if (mirrorX || x < 0) tmp.X = -tmp.X;
            Vector3 delta;
            if (this.robeChannel && this.robeDataRow[y])
            {
                Vector3 tmpR = new Vector3(robeDeltas[y][Math.Abs(x)]);
                if (mirrorX || x < 0) tmpR.X = -tmpR.X;
                delta = new Vector3((tmp.X * (1f - robeMult)) + (tmpR.X * robeMult),
                                    (tmp.Y * (1f - robeMult)) + (tmpR.Y * robeMult),
                                    (tmp.Z * (1f - robeMult)) + (tmpR.Z * robeMult));
            }
            else
            {
                delta = new Vector3(tmp);
            }
            return delta;
        }
        public Vector3 GetSkinDelta(int x, int y, bool mirrorX)
        {
            Vector3 tmp = new Vector3(skinDeltas[y][Math.Abs(x)]);
            if (mirrorX || x < 0) tmp.X = -tmp.X;
            return tmp;
        }
        public Vector3 GetRobeDelta(int x, int y, bool mirrorX)
        {
            if (robeDataRow[y])
            {
                Vector3 tmp = new Vector3(robeDeltas[y][Math.Abs(x)]);
                if (mirrorX || x < 0) tmp.X = -tmp.X;
                return tmp;
            }
            else
            {
                return new Vector3(0, 0, 0);
            }
        }

        public bool HasData
        {
            get { return this.skinDeltas.Length > 0; }
        }

        public MorphMap(uint mapWidth, uint mapHeight, byte shapeOrNormals, uint minCol, uint maxCol,
            uint minRow, uint maxRow, bool robeChannel, Vector3[][] skinDeltas, Vector3[][] robeDeltas, bool[] robeDataRow)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.shapeOrNormals = shapeOrNormals;
            this.minCol = minCol;
            this.maxCol = maxCol;
            this.minRow = minRow;
            this.maxRow = maxRow;
            this.robeChannel = robeChannel;
            this.skinDeltas = skinDeltas;
            this.robeDeltas = robeDeltas;
            this.robeDataRow = robeDataRow;
        }
    }
}
