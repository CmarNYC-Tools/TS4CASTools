//https://github.com/andrews4s/DotNetDDS
//MIT License
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DWORD = System.UInt32;
namespace DotNetDDS
{
    [Flags]
    public enum DDS_CAPS_FLAGS : DWORD
    {
        /// <summary>
        /// Optional; must be used on any file that contains more than one surface (a mipmap, a cubic environment map, or mipmapped volume texture).
        /// </summary>
        DDSCAPS_COMPLEX = 0x8,
        /// <summary>
        /// Optional; should be used for a mipmap.
        /// </summary>
        DDSCAPS_MIPMAP = 0x400000,
        /// <summary>
        /// Required
        /// </summary>
        DDSCAPS_TEXTURE = 0x1000
    }
    [Flags]
    public enum DDS_CAPS2_FLAGS : DWORD
    {
        /// <summary>
        /// Required for a cube map.	
        /// </summary>
        DDSCAPS2_CUBEMAP = 0x200,
        /// <summary>
        ///  Required when these surfaces are stored in a cube map.
        /// </summary>
        DDSCAPS2_CUBEMAP_POSITIVEX = 0x400,
        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800,
        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000,
        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000,
        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000,
        /// <summary>
        /// Required when these surfaces are stored in a cube map.
        /// </summary>
        DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000,
        /// <summary>
        /// Required for a volume texture.
        /// </summary>
        DDSCAPS2_VOLUME = 0x200000
    }
    [Flags]
    public enum DDS_HEADER_FLAGS : DWORD
    {
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        DDSD_CAPS = 0x1,
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        DDSD_HEIGHT = 0x2,
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        DDSD_WIDTH = 0x4,
        /// <summary>
        /// Required when pitch is provided for an uncompressed texture.
        /// </summary>
        DDSD_PITCH = 0x8,
        /// <summary>
        /// Required in every .dds file.
        /// </summary>
        DDSD_PIXELFORMAT = 0x1000,
        /// <summary>
        /// Required in a mipmapped texture.
        /// </summary>
        DDSD_MIPMAPCOUNT = 0x20000,
        /// <summary>
        /// Required when pitch is provided for a compressed texture.
        /// </summary>
        DDSD_LINEARSIZE = 0x80000,
        /// <summary>
        /// Required in a depth texture.
        /// </summary>
        DDSD_DEPTH = 0x800000,

        DDSD_TEXTURE = DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT,
        DDSD_MIPTEXTURE = DDSD_TEXTURE | DDSD_MIPMAPCOUNT


    }
    [Flags]
    public enum DDS_PIXELFORMAT_FLAGS : DWORD
    {
        /// <summary>
        /// Texture contains alpha data; dwRGBAlphaBitMask contains valid data.
        /// </summary>
        DDPF_ALPHAPIXELS = 0x1,
        /// <summary>
        /// Used in some older DDS files for alpha channel only uncompressed data (dwRGBBitCount contains the alpha channel bitcount; dwABitMask contains valid data)
        /// </summary>
        DDPF_ALPHA = 0x2,
        /// <summary>
        /// Texture contains compressed RGB data; dwFourCC contains valid data.
        /// </summary>
        DDPF_FOURCC = 0x4,
        /// <summary>
        /// Texture contains uncompressed RGB data; dwRGBBitCount and the RGB masks (dwRBitMask, dwGBitMask, dwBBitMask) contain valid data.	
        /// </summary>
        DDPF_RGB = 0x40,
        /// <summary>
        /// Used in some older DDS files for YUV uncompressed data (dwRGBBitCount contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask contains the U mask, dwBBitMask contains the V mask)
        /// </summary>
        DDPF_YUV = 0x200,
        /// <summary>
        /// Used in some older DDS files for single channel color uncompressed data (dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains the channel mask). Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.
        /// </summary>
        DDPF_LUMINANCE = 0x20000
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct DDS_PIXELFORMAT
    {
        public static readonly DWORD SIZE = (DWORD)Marshal.SizeOf(typeof(DDS_PIXELFORMAT));
        public DWORD dwSize;
        public DDS_PIXELFORMAT_FLAGS dwFlags;
        public DWORD dwFourCC;
        public DWORD dwRGBBitCount;
        public DWORD dwRBitMask;
        public DWORD dwGBitMask;
        public DWORD dwBBitMask;
        public DWORD dwABitMask;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct DDS_HEADER
    {
        public static readonly DWORD SIZE = (DWORD)Marshal.SizeOf(typeof(DDS_HEADER));
        public static readonly byte[] MAGIC = Encoding.ASCII.GetBytes("DDS ");
        public DWORD dwSize;
        public DDS_HEADER_FLAGS dwFlags;
        public DWORD dwHeight;
        public DWORD dwWidth;
        public DWORD dwPitchOrLinearSize;
        public DWORD dwDepth;
        public DWORD dwMipMapCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public DWORD[] dwReserved1;
        public DDS_PIXELFORMAT ddspf;
        public DDS_CAPS_FLAGS dwCaps;
        public DDS_CAPS2_FLAGS dwCaps2;
        public DWORD dwCaps3;
        public DWORD dwCaps4;
        public DWORD dwReserved2;

        public static DDS_HEADER FromStream(Stream s)
        {
            var bytes = new byte[Marshal.SizeOf(typeof(DDS_HEADER))];
            s.Read(bytes, 0, bytes.Length);
            return FromBytes(bytes);
        }
        public static DDS_HEADER FromBytes(byte[] bytes)
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var header = (DDS_HEADER)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DDS_HEADER));
            handle.Free();
            return header;
        }
        public byte[] ToBytes()
        {
            var buffer = new byte[SIZE];
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            Marshal.StructureToPtr(this, handle.AddrOfPinnedObject(), false);
            handle.Free();
            return buffer;
        }
    }
}